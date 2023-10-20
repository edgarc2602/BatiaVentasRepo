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

    @ViewChild('salarioMixtotxt',         { static: false }) salarioMixtotxt: ElementRef;
    @ViewChild('salarioMixtoFronteratxt', { static: false }) salarioMixtoFronteratxt: ElementRef;
    @ViewChild('salarioRealtxt',          { static: false }) salarioRealtxt: ElementRef;
    @ViewChild('salarioRealFronteratxt',  { static: false }) salarioRealFronteratxt: ElementRef;

    @ViewChild('costoIndirectotxt',     { static: false }) costoIndirectotxt: ElementRef;
    @ViewChild('utilidadtxt',           { static: false }) utilidadtxt: ElementRef;
    @ViewChild('comisionSobreVentatxt', { static: false }) comisionSobreVentatxt: ElementRef;
    @ViewChild('comisionExternatxt',    { static: false }) comisionExternatxt: ElementRef;
    @ViewChild('fechaAplicatxt',        { static: false }) fechaAplicatxt: ElementRef;

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

    costoIndirecto: number = 0;
    utilidad: number = 0;
    comisionSobreVenta: number = 0;
    comisionExterna: number = 0;
    fechaAplica: string = '';
    

    cotpor: CotizaPorcentajes = {
        idPersonal: 0, costoIndirecto: 0, utilidad: 0, comisionSobreVenta: 0, comisionExterna: 0, fechaAlta: null, personal: '', fechaAplica: null
    };

    sal: PuestoTabulador = {
        idPuesto: 0, idPuestoSalario: 0, salarioMixto: 0, salarioMixtoFrontera: 0, salarioReal: 0, salarioRealFrontera: 0
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

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
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
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;
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
    limpiarPorcentajesNG() {
        this.costoIndirecto = 0;
        this.utilidad = 0;
        this.comisionSobreVenta = 0;
        this.comisionExterna = 0;
        this.fechaAplica = '';
    }
    limpiarObjeto() {
        this.sal.salarioMixto = 0;
        this.sal.salarioMixtoFrontera = 0;
        this.sal.salarioReal = 0;
        this.sal.salarioRealFrontera = 0;
    }
    limpiarPorcentajes() {
        this.cotpor.costoIndirecto = 0;
        this.cotpor.utilidad = 0;
        this.cotpor.comisionSobreVenta = 0;
        this.cotpor.comisionExterna = 0;
        this.cotpor.fechaAplica = '';
    }
    obtenerValores() {
        this.sal.salarioMixto = parseFloat(this.salarioMixtotxt.nativeElement.value);
        this.sal.salarioMixtoFrontera = parseFloat(this.salarioMixtoFronteratxt.nativeElement.value);
        this.sal.salarioReal = parseFloat(this.salarioRealtxt.nativeElement.value);
        this.sal.salarioRealFrontera = parseFloat(this.salarioRealFronteratxt.nativeElement.value);
    }
    obtenerPorcentajesCotizacion() {
        this.cotpor.costoIndirecto = parseFloat(this.costoIndirectotxt.nativeElement.value);
        this.cotpor.utilidad = parseFloat(this.utilidadtxt.nativeElement.value);
        this.cotpor.comisionSobreVenta = parseFloat(this.comisionSobreVentatxt.nativeElement.value);
        this.cotpor.comisionExterna = parseFloat(this.comisionExternatxt.nativeElement.value);
        this.cotpor.fechaAplica = this.fechaAplicatxt.nativeElement.value;
        this.cotpor.idPersonal = this.user.idPersonal;
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
            //aplicar modelo y enviar
            //this.usuario.firma = this.selectedImage;

            // Convierte el ArrayBuffer a Base64
            if (this.selectedImage instanceof ArrayBuffer) {
                const base64Firma = this.arrayBufferToBase64(this.selectedImage);
                this.usuario.firma = base64Firma;
            } else if (typeof this.selectedImage === 'string') {
                // Si ya es una cadena, asigna directamente
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
}

