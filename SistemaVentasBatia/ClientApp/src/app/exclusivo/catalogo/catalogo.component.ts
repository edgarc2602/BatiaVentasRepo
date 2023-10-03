import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ProductoItem } from 'src/app/models/productoitem';
import { ProductoWidget } from 'src/app/widgets/producto/producto.widget';
import { AgregarServicioWidget  } from 'src/app/widgets/agregarservicio/agregarservicio.widget'

import { PuestoTabulador } from 'src/app/models/puestotabulador';
import { Subject } from 'rxjs';

@Component({
    selector: 'catalogo-comp',
    templateUrl: './catalogo.component.html'
})
export class CatalogoComponent {
    @ViewChild(ProductoWidget,{ static: false }) prow: ProductoWidget;
    @ViewChild(AgregarServicioWidget, { static: false }) addSer: AgregarServicioWidget;
    @ViewChild('salarioMixtotxt', { static: false }) salarioMixtotxt: ElementRef;
    @ViewChild('salarioMixtoFronteratxt', { static: false }) salarioMixtoFronteratxt: ElementRef;
    @ViewChild('salarioRealtxt', { static: false }) salarioRealtxt: ElementRef;
    @ViewChild('salarioRealFronteratxt', { static: false }) salarioRealFronteratxt: ElementRef;
    pues: Catalogo[] = [];
    selPuesto: number = 0;
    tipoServicio: number = 2;
    mates: ProductoItem[] = [];
    sers: Catalogo[] = [];
    tser: Catalogo[] = [];
    grupo: string = 'material';

    salarioMixto: number = 0;
    salarioMixtoFrontera: number = 0;
    salarioReal: number = 0;
    salarioRealFrontera: number = 0;

    sal: PuestoTabulador = {
        idPuesto: 0, idPuestoSalario: 0, salarioMixto: 0, salarioMixtoFrontera: 0, salarioReal: 0, salarioRealFrontera: 0
    };
    validaMess: string = '';
    evenSub: Subject<void> = new Subject<void>();


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {
        http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));
        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/gettiposervicio`).subscribe(response => {
            this.tser = response;
        }, err => console.log(err));
    }
    chgServicio() {

    }
    chgPuesto() {
        this.getMaterial();
        this.getTabulador();
    }

    chgTab(nm: string) {
        this.grupo = nm;
        this.getMaterial();
    }

    openMat() {
        this.grupo = 'material';
        this.prow.inicio();
    }

    openEqui() {
        this.grupo = 'equipo';
        this.prow.inicio();
    }

    openHer() {
        this.grupo = 'herramienta';
        this.prow.inicio();
    }

    openUni() {
        this.grupo = 'uniforme';
        this.prow.inicio();
    }

    openSer() {
        this.addSer.open();
    }
    closeMat($event) {
        this.getMaterial();
    }
    reloadServicios() {
        this.getServicios();
    }

    getServicios() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    getMaterial() {
        this.mates = [];
        this.http.get<ProductoItem[]>(`${this.url}api/producto/get${this.grupo}/${this.selPuesto}`).subscribe(response => {
            this.mates = response;
        }, err => console.log(err));
    }

    deleteMat(id: number) {
        this.http.delete<boolean>(`${this.url}api/producto/del${this.grupo}/${id}`).subscribe(response => {
            this.getMaterial();
        }, err => console.log(err));
    }

    deleteServ(id) {
        this.http.delete(`${this.url}api/producto/EliminarServicio/${id}`).subscribe(response => {
            this.getServicios();
        }, err => console.log(err));
    }
    limpiarngModel() {
        this.salarioMixto = 0;
        this.salarioMixtoFrontera = 0;
        this.salarioReal = 0;
        this.salarioRealFrontera = 0;
    }
    limpiarObjeto() {
        this.sal.salarioMixto = 0;
        this.sal.salarioMixtoFrontera = 0;
        this.sal.salarioReal = 0;
        this.sal.salarioRealFrontera = 0;
    }
    obtenerValores() {
        this.sal.salarioMixto = parseFloat(this.salarioMixtotxt.nativeElement.value);
        this.sal.salarioMixtoFrontera = parseFloat(this.salarioMixtoFronteratxt.nativeElement.value);
        this.sal.salarioReal = parseFloat(this.salarioRealtxt.nativeElement.value);
        this.sal.salarioRealFrontera = parseFloat(this.salarioRealFronteratxt.nativeElement.value);
    }
    getTabulador() {
        this.limpiarObjeto();
        this.limpiarngModel();
        this.http.get<PuestoTabulador>(`${this.url}api/tabulador/ObtenerTabuladorPuesto/${this.selPuesto}`).subscribe(response => {
            this.sal = response;
            this.salarioMixto = this.sal.salarioMixto;
            this.salarioMixtoFrontera = this.sal.salarioMixtoFrontera;
            this.salarioReal = this.sal.salarioReal;
            this.salarioRealFrontera = this.sal.salarioRealFrontera;
            this.limpiarObjeto();
        }, err => console.log(err));
    }
    actualizarSalarios(id: number) {
        this.limpiarObjeto();
        this.obtenerValores();
        this.http.post<PuestoTabulador>(`${this.url}api/cotizacion/actualizarsalarios`, this.sal).subscribe(response => {
            this.limpiarngModel();
            this.limpiarObjeto();
            this.getTabulador();
        }, err => console.log(err));
    }
}
