import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ProductoItem } from 'src/app/models/productoitem';
import { ProductoWidget } from 'src/app/widgets/producto/producto.widget';
import { AgregarServicioWidget  } from 'src/app/widgets/agregarservicio/agregarservicio.widget'

import { PuestoTabulador } from 'src/app/models/puestotabulador';
import { Subject } from 'rxjs';
import { fadeInOut } from 'src/app/fade-in-out';
import { CotizaPorcentajes } from 'src/app/models/cotizaporcentajes';

import { StoreUser } from 'src/app/stores/StoreUser';
import { UsuarioRegistro } from 'src/app/models/usuarioregistro';
import { Usuario } from '../../models/usuario';
import { UsuarioAddWidget } from 'src/app/widgets/usuarioadd/usuarioadd.widget';

@Component({
    selector: 'catalogo-comp',
    templateUrl: './catalogo.component.html',
    animations: [fadeInOut],
})
export class CatalogoComponent {
    @ViewChild(ProductoWidget,        { static: false }) prow:   ProductoWidget;
    @ViewChild(AgregarServicioWidget, { static: false }) addSer: AgregarServicioWidget;
    @ViewChild(UsuarioAddWidget,      { static: false }) addUsu: UsuarioAddWidget;

    @ViewChild('zona1txt',{ static: false }) zona1txt: ElementRef;
    @ViewChild('zona2txt',{ static: false }) zona2txt: ElementRef;
    @ViewChild('zona3txt',{ static: false }) zona3txt: ElementRef;
    @ViewChild('zona4txt',{ static: false }) zona4txt: ElementRef;
    @ViewChild('zona5txt',{ static: false }) zona5txt: ElementRef;

    @ViewChild('costoIndirectotxt',     { static: false }) costoIndirectotxt: ElementRef;
    @ViewChild('utilidadtxt',           { static: false }) utilidadtxt: ElementRef;
    @ViewChild('comisionSobreVentatxt', { static: false }) comisionSobreVentatxt: ElementRef;
    @ViewChild('comisionExternatxt',    { static: false }) comisionExternatxt: ElementRef;
    @ViewChild('fechaAplicatxt', { static: false }) fechaAplicatxt: ElementRef;

    @ViewChild('imsstxt', { static: false }) imsstxt: ElementRef;

    pues: Catalogo[] = [];
    selPuesto: number = 0;
    tipoServicio: number = 2;
    mates: ProductoItem[] = [];
    sers: Catalogo[] = [];
    tser: Catalogo[] = [];
    grupo: string = 'material';
    zona1: number = 0;
    zona2: number = 0;
    zona3: number = 0;
    zona4: number = 0;
    zona5: number = 0;
    costoIndirecto: number = 0;
    utilidad: number = 0;
    comisionSobreVenta: number = 0;
    comisionExterna: number = 0;
    fechaAplica: string = '';
    cotpor: CotizaPorcentajes = {
        idPersonal: 0, costoIndirecto: 0, utilidad: 0, comisionSobreVenta: 0, comisionExterna: 0, fechaAlta: null, personal: '', fechaAplica: null
    };
    sal: PuestoTabulador = {
       idSueldoZonaClase: 0,  idPuesto: 0, idClase: 0, zona1: 0, zona2: 0, zona3: 0, zona4: 0, zona5: 0
    };
    validaMess: string = '';
    evenSub: Subject<void> = new Subject<void>();
    selectedImage: string | ArrayBuffer | null = null;
    idPersonal: number = 0;
    autorizacion: number = 0;
    usuario: UsuarioRegistro = {
        idAutorizacionVentas: 0, idPersonal: 0, autoriza: 0, nombres: '', apellidos: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '',
        firma: '', revisa: 0
    }
    lclas: Catalogo[] = [];
    idClase: number = 1;
    tipoProd: string = '';
    imss: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/gettiposervicio`).subscribe(response => {
            this.tser = response;
        }, err => console.log(err));
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getclase`).subscribe(response => {
            this.lclas = response;
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

    openMat(id: number) {
        this.grupo = 'material';
        this.prow.inicio(id,0, this.selPuesto);
    }

    openEqui(id:number) {
        this.grupo = 'equipo';
        this.prow.inicio(id, 0, this.selPuesto);
    }

    openHer(id: number) {
        this.grupo = 'herramienta';
        this.prow.inicio(id, 0, this.selPuesto);
    }

    openUni(id: number) {
        this.grupo = 'uniforme';
        this.prow.inicio(id, 0, this.selPuesto);
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

    updateProd(id: number, tipo: number) {
        
        this.prow.inicio(id,tipo, this.selPuesto);
    }

    deleteServ(id) {
        this.http.delete(`${this.url}api/producto/EliminarServicio/${id}`).subscribe(response => {
            this.getServicios();
        }, err => console.log(err));
    }
    
    limpiarPorcentajesNG() {
        this.costoIndirecto = 0;
        this.utilidad = 0;
        this.comisionSobreVenta = 0;
        this.comisionExterna = 0;
        this.fechaAplica = '';
    }
    
    limpiarPorcentajes() {
        this.cotpor.costoIndirecto = 0;
        this.cotpor.utilidad = 0;
        this.cotpor.comisionSobreVenta = 0;
        this.cotpor.comisionExterna = 0;
        this.cotpor.fechaAplica = '';
    }
    
    obtenerPorcentajesCotizacion() {
        this.cotpor.costoIndirecto = parseFloat(this.costoIndirectotxt.nativeElement.value);
        this.cotpor.utilidad = parseFloat(this.utilidadtxt.nativeElement.value);
        this.cotpor.comisionSobreVenta = parseFloat(this.comisionSobreVentatxt.nativeElement.value);
        this.cotpor.comisionExterna = parseFloat(this.comisionExternatxt.nativeElement.value);
        this.cotpor.fechaAplica = this.fechaAplicatxt.nativeElement.value;
        this.cotpor.idPersonal = this.user.idPersonal;
    }
    
    getPorcentajes() {
        this.limpiarPorcentajes();
        this.limpiarPorcentajesNG();
        this.http.get<CotizaPorcentajes>(`${this.url}api/cotizacion/obtenerporcentajescotizacion`).subscribe(response => { //falta
            this.cotpor = response;
            this.costoIndirecto = this.cotpor.costoIndirecto;
            this.utilidad = this.cotpor.utilidad;
            this.comisionSobreVenta = this.cotpor.comisionSobreVenta;
            this.comisionExterna = this.cotpor.comisionExterna;
            this.fechaAplica = this.cotpor.fechaAplica;
            this.limpiarPorcentajesNG();
        }, err => console.log(err));
        this.getImss();
    }
    
    actualizarPorcentajesPredeterminadosCotizacion() {
        this.limpiarPorcentajes();
        this.obtenerPorcentajesCotizacion();
        this.http.post<CotizaPorcentajes>(`${this.url}api/cotizacion/actualizarporcentajespredeterminadoscotizacion`, this.cotpor).subscribe(response => { 
            this.limpiarPorcentajesNG();
            this.limpiarPorcentajes();
            this.getPorcentajes();
        }, err => console.log(err));    
    }

    onFileSelected(event: any): void {
        const selectedFile = event.target.files[0];
        if (selectedFile) {
            const reader = new FileReader();
            reader.onload = (e: any) => {
                this.selectedImage = e.target.result as string | ArrayBuffer | null;
            };
            reader.readAsDataURL(selectedFile);
        }
    }

    guardarImagen(): void {
        if (this.selectedImage) {
            if (this.selectedImage instanceof ArrayBuffer) {
                const base64Firma = this.arrayBufferToBase64(this.selectedImage);
                this.usuario.firma = base64Firma;
            } else if (typeof this.selectedImage === 'string') {
                this.usuario.firma = this.selectedImage;
            } else {
                console.error('Tipo no compatible para selectedImage');
            }
            if (this.usuario.autoriza == 1) {
                this.usuario.autoriza = 1;
            }
            else {
                this.usuario.autoriza = 0;
            }
            if (this.usuario.revisa == 1) {
                this.usuario.revisa = 1;
            }
            else {
                this.usuario.revisa = 0;
            }
            this.usuario.idPersonal = this.idPersonal;
            this.http.post<boolean>(`${this.url}api/usuario/agregarusuario`, this.usuario).subscribe(response => {
                this.nuevoUsuario();
            });
        }
    }
    goBack() {
        window.history.back();
    }
    arrayBufferToBase64(arrayBuffer: ArrayBuffer): string {
        const uint8Array = new Uint8Array(arrayBuffer);
        return btoa(String.fromCharCode.apply(null, uint8Array));
    }
    nuevoUsuario() {
        this.usuario = {
            idAutorizacionVentas: 0, idPersonal: 0, autoriza: 0, nombres: '', apellidos: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '',
            firma: '', revisa: 0
        }
    }
    openUsu() {
        this.addUsu.open();
    }

    getTabulador() {
        this.http.get<PuestoTabulador>(`${this.url}api/tabulador/ObtenerTabuladorPuesto/${this.selPuesto}/${this.idClase}`).subscribe(response => {
            this.sal = response;
        }, err => console.log(err));
    }
    actualizarSalarios(id: number) {
        this.obtenerValores();
        this.http.post<PuestoTabulador>(`${this.url}api/cotizacion/actualizarsalarios`, this.sal).subscribe(response => {
            this.getTabulador();
        }, err => console.log(err));
    }
    limpiarObjeto() {
        this.sal.zona1 = 0;
        this.sal.zona2 = 0;
        this.sal.zona3 = 0;
        this.sal.zona4 = 0;
        this.sal.zona5 = 0;
    }
    obtenerValores() {
        this.sal.zona1 = parseFloat(this.zona1txt.nativeElement.value);
        this.sal.zona2 = parseFloat(this.zona2txt.nativeElement.value);
        this.sal.zona3 = parseFloat(this.zona3txt.nativeElement.value);
        this.sal.zona4 = parseFloat(this.zona4txt.nativeElement.value);
        this.sal.zona5 = parseFloat(this.zona5txt.nativeElement.value);
        this.sal.idClase = this.idClase;
        this.sal.idPuesto = this.selPuesto;
    }

    getImss() {
        this.http.get<number>(`${this.url}api/cotizacion/obtenerimssbase`).subscribe(response => {
            this.imss = response;
        }, err => console.log(err));
    }

    updImss() {
        this.imss = parseFloat(this.imsstxt.nativeElement.value);
        this.http.put<boolean>(`${this.url}api/cotizacion/actualizarimssbase`, this.imss).subscribe(response => {
        }, err => console.log(err));
    }
}