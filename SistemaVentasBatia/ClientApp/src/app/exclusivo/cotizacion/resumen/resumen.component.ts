import { Component, Inject, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
//import { HttpClientModule } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { CotizaResumenLim } from 'src/app/models/cotizaresumenlim';
import { ItemN } from 'src/app/models/item';
import { DireccionCotizacion } from 'src/app/models/direccioncotizacion';
import { ListaDireccion } from 'src/app/models/listadireccion';
import { ListaPuesto } from 'src/app/models/listapuesto';
import { ListaMaterial } from 'src/app/models/listamaterial';
import { Catalogo } from 'src/app/models/catalogo';
import { MaterialAddWidget } from 'src/app/widgets/materialadd/materialadd.widget';
import { MaterialWidget } from 'src/app/widgets/material/material.widget';
import { DireccionWidget } from 'src/app/widgets/direccion/direccion.widget';
import { PuestoWidget } from 'src/app/widgets/puesto/puesto.widget';

import { EliminaWidget } from 'src/app/widgets/elimina/elimina.widget';
import { EliminaOperarioWidget } from 'src/app/widgets/eliminaOperario/eliminaOperario.widget';
import { MaterialOperarioAddWidget } from 'src/app/widgets/materialoperarioadd/materialoperarioadd.widget';
import { EliminaDirectorioWidget } from 'src/app/widgets/eliminadirectorio/eliminadirectorio.widget';
import { ActualizaCotizacionWidget } from 'src/app/widgets/actualizacotizacion/actualizacotizacion.widget';


import { Cotizacionupd } from 'src/app/models/cotizacionupd';

import { Router } from '@angular/router';
import { ReportService } from 'src/app/report.service';

@Component({
    selector: 'resumen',
    templateUrl: './resumen.component.html'
})
export class ResumenComponent implements OnInit, OnDestroy {
    @ViewChild(MaterialAddWidget, { static: false }) proAdd: MaterialAddWidget;
    @ViewChild(MaterialWidget, { static: false }) proPue: MaterialWidget;
    @ViewChild(MaterialOperarioAddWidget, { static: false }) opeMat: MaterialOperarioAddWidget;
    @ViewChild(DireccionWidget, { static: false }) dirAdd: DireccionWidget;
    @ViewChild(PuestoWidget, { static: false }) pueAdd: PuestoWidget;
    @ViewChild(EliminaWidget, { static: false }) eliw: EliminaWidget;
    @ViewChild(ActualizaCotizacionWidget, { static: false }) actCot: ActualizaCotizacionWidget;
    @ViewChild(EliminaOperarioWidget, { static: false }) eliope: EliminaOperarioWidget;
    @ViewChild(EliminaDirectorioWidget, { static: false }) elidir: EliminaDirectorioWidget;

    @ViewChild('resumen', { static: false }) resumen: ElementRef;
    @ViewChild('pdfCanvas', { static: true }) pdfCanvas: ElementRef;

    @ViewChild('indirectotxt', { static: false }) indirectotxt: ElementRef;
    @ViewChild('utilidadtxt', { static: false }) utilidadtxt: ElementRef;
    @ViewChild('CSVtxt', { static: false }) CSVtxt: ElementRef;



    sub: any;
    model: CotizaResumenLim = {
        idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, provisiones: 0,
        material: 0, uniforme: 0, equipo: 0, herramienta: 0,
        subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', utilidadPor: '', indirectoPor: '', csvPor: '', comisionSV: 0
    };
    dirs: ItemN[] = [];
    cotdirs: Catalogo[] = [];
    lsdir: ListaDireccion = {} as ListaDireccion;
    lspue: ListaPuesto = {} as ListaPuesto;
    lsmat: ListaMaterial = {} as ListaMaterial;
    lsher: ListaMaterial = {} as ListaMaterial;
    selDireccion: number = 0;
    selPuesto: number = 0;
    selMatDir: number = 0;
    selMatPue: number = 0;

    edit: number = 0;

    selTipo: string = 'material';
    txtMatKey: string = '';
    sDir: boolean = false;
    modelcot: Cotizacionupd = {
        idCotizacion: 0, indirecto: '', utilidad: '', comisionSV: ''
    };
    indirectoValue: string = this.model.utilidadPor;
    utilidadValue: string = this.model.indirectoPor;
    CSV: string = this.model.csvPor;


    modelDir: DireccionCotizacion = {
        idCotizacion: 0, idDireccionCotizacion: 0, idDireccion: 0, nombreSucursal: ''
    };
    idpro: number = 0;
    idope: number = 0;
    idDC: number = 0;
    urlF: string = '';

    reportData: Blob;
    pdfUrl: string;

    constructor(
        @Inject('BASE_URL') private url: string,
        private http: HttpClient,
        private route: ActivatedRoute,
        private router: Router,
        private reportService: ReportService,
    ) {
        this.nuevo();
        this.lsdir = {
            pagina: 1, idCotizacion: this.model.idProspecto, idProspecto: this.model.idProspecto,
            idDireccion: 0, direcciones: []



        };

    }


    nuevo() {
        this.model = {
            idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, provisiones: 0,
            material: 0, uniforme: 0, equipo: 0, herramienta: 0,
            subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', utilidadPor: '', indirectoPor: '', csvPor: '', comisionSV: 0
        };
    }

    existe(id: number) {
        this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${id}`).subscribe(response => {
            this.model = response;
            this.getAllDirs();
            this.getDirs();
            this.getPlan();
        }, err => console.log(err));
    }

    savePros(event) {
        this.model.idProspecto = event;
        this.getAllDirs();
    }

    savePlan(event) {
        this.getPlan();
    }

    getAllDirs() {
        this.http.get<ItemN[]>(`${this.url}api/direccion/getcatalogo/${this.model.idProspecto}`).subscribe(response => {
            this.dirs = response;
        }, err => console.log(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getsucursalbycot/${this.model.idCotizacion}`).subscribe(response => {
            this.cotdirs = response;
        }, err => console.log(err));
    }

    getDirs() {
        this.http.get<ListaDireccion>(`${this.url}api/cotizacion/limpiezadirectorio/${this.model.idCotizacion}`).subscribe(response => {
            this.lsdir = response;
        }, err => console.log(err));
    }

    saveDir($event) {
        this.modelDir.idDireccion = $event;
        this.addDir();
        this.getAllDirs();
    }

    addDir() {
        this.modelDir.idCotizacion = this.model.idCotizacion;
        this.modelDir.idDireccionCotizacion = 0;
        if (this.modelDir.idDireccion != 0) {
            this.http.post<DireccionCotizacion>(`${this.url}api/cotizacion/agregardireccion`, this.modelDir).subscribe(response => {
                this.getDirs();
            }, err => console.log(err));
        }
    }
    
    getPlan() {
        this.http.get<ListaPuesto>(`${this.url}api/cotizacion/${this.model.idCotizacion}/0/0`).subscribe(response => {
            this.lspue = response;
        }, err => console.log(err));
    }

    addPlan(id: number, tb: number) {
        this.selDireccion = id;
        this.pueAdd.open(this.model.idCotizacion, id, tb, 0);
    }

    updPlan(id: number, tb: number) {
        this.selPuesto = id;
        this.pueAdd.open(this.model.idCotizacion, this.selDireccion, tb, id);
    }

    removePlan(id: number) {
        this.http.delete<boolean>(`${this.url}api/puesto/${id}`).subscribe(response => {
            if (response) {
                this.getPlan();
            }
        }, err => console.log(err));
    }

    filtroPlan(id: number) {
        let list = this.lspue.puestosDireccionesCotizacion.filter(p => p.idDireccionCotizacion == id);
        return list;
    }

    getMat(tb: string) {
        this.selTipo = tb;
        let fil: string = (this.txtMatKey != '' ? 'keywords=' + this.txtMatKey : '');
        if (fil.length > 0) fil += '&';
        fil += 'idDir=' + this.selMatDir + '&idPues=' + this.selMatPue;
        this.http.get<ListaMaterial>(`${this.url}api/${tb}/${this.model.idCotizacion}/${this.lsmat.pagina == undefined ? 1 : this.lsmat.pagina}?${fil}`).subscribe(response => {
            this.lsmat = response;
        }, err => console.log(err));
    }

    getNewDir() {
        this.dirAdd.open(this.model.idProspecto, 0);
    }

    getMatPues(id: number, dir: number, tp: string) {
        this.edit = 0;

        this.selPuesto = id;
        this.selDireccion = dir;
        this.selTipo = tp;
        this.proPue.open(this.model.idCotizacion, dir, id, tp, this.edit);
    }

    //opeMate(event) {
    //    this.cloMate();
    //    this.sDir = false;
    //    this.opeMat.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, event, this.model.idServicio, this.selTipo, false);
    //}
    //opeMate(event) {
    //    this.cloMate();
    //    this.sDir = false;
    //    this.opeMat.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, event, this.model.idServicio, this.selTipo, false);
    //}

    newMate(event) {
        this.cloMate();
        this.sDir = false;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, event, this.model.idServicio, this.selTipo, false, this.edit);
    }

    saveMate(event) {
        this.getMat(this.selTipo);
    }

    getNewMat(tp: string) {
        this.selPuesto = 0;
        this.selDireccion = 0;
        this.sDir = true;
        this.selTipo = tp;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, 0, this.model.idServicio, tp, true, this.edit);
    }

    selNewMat(id: number, tp: string, edit: number) {
        this.edit = 1;
        this.sDir = true;
        this.selTipo = tp;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, id, this.model.idServicio, tp, true, this.edit);
    }

    removeMat(id: number) {
        this.http.delete<boolean>(`${this.url}api/${this.selTipo}/${id}`).subscribe(response => {
            if (response) {
                this.getMat(this.selTipo);
            }
        }, err => console.log(err));
    }

    cloMate() {
        this.proPue.close();
    }

    matPagina(event) {
        this.lsmat.pagina = event;
        this.getMat(this.selTipo);
    }

    ngOnInit(): void {
        this.sub = this.route.params.subscribe(params => {
            let idcot: number = +params['id'];
            this.existe(idcot);
        });

    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }

    elige(idCotizacion) {
        this.idpro = idCotizacion;
        this.eliw.titulo = 'Duplicar cotización';
        this.eliw.mensaje = '¿Está seguro de que desea duplicar la cotización?';
        this.eliw.open();
    }
    elimina($event) {
        if ($event == true) {
            this.model.idCotizacion = this.model.idCotizacion;
            if (this.model.idCotizacion != 0) {
                this.http.post<DireccionCotizacion>(`${this.url}api/cotizacion/DuplicarCotizacion`, this.model.idCotizacion).subscribe(response => {
                }, err => console.log(err));
            }
        }
        if ($event == false) {
            this.http.delete<boolean>(`${this.url}api/puesto/${this.idope}`).subscribe(response => {
                if (response) {
                    this.getPlan();
                }
            }, err => console.log(err));
        }
    }
    eligeOperario(idOperario) {
        this.idope = idOperario;
        this.eliope.titulo = 'Eliminar Operario';
        this.eliope.mensaje = 'Al eliminar al operario se borraran los registros relacionados';
        this.eliope.open();
    }

    limpiarInputs() {
        this.modelcot.indirecto = "";
        this.modelcot.utilidad = "";
        this.modelcot.comisionSV = "";
        this.indirectoValue = "";
        this.utilidadValue = "";
        this.CSV = "";
    }
    actualizarIndirectoUtilidad() {
        this.modelcot.idCotizacion = this.model.idCotizacion;
        //this.modelcot.indirecto = this.indirectoValue;
        //this.modelcot.utilidad = this.utilidadValue;
        //this.modelcot.comisionSV = this.CSV;


        this.modelcot.indirecto = this.indirectotxt.nativeElement.value;
        this.modelcot.utilidad = this.utilidadtxt.nativeElement.value;
        this.modelcot.comisionSV = this.CSVtxt.nativeElement.value;


        if (this.model.idCotizacion != 0) {
            this.http.post<Cotizacionupd>(`${this.url}api/cotizacion/ActualizarIndirectoUtilidadService`, this.modelcot).subscribe(response => {
                this.getDirs();
            }, err => console.log(err));
        }
        location.reload();
        this.limpiarInputs();
    }
    validarSoloNumeros(utilidadValue: string): boolean {
        utilidadValue = this.utilidadValue;
        const regex = /^[0-9]+$/;
        return regex.test(utilidadValue);
    }
    validaDireccionCotizacion(idDireccionCotizacion) {
        this.idDC = idDireccionCotizacion;
        this.elidir.titulo = 'Eliminar directorio';
        this.elidir.mensaje = 'Nota: Eliminar el directorio tambien afectara a los items relacionados a el, ¿Está seguro de que desea eliminar el directorio seleccionado?';
        this.elidir.open();
    }
    eliminaDireccionCotizacion($event) {
        if ($event == true) {
            this.http.post(`${this.url}api/cotizacion/EliminarDireccionCotizacion`, this.idDC).subscribe(response => {
                this.getDirs();
            }, err => console.log(err));
        }
    }
    return($event) {
        if ($event = true) {
            this.getMatPues(this.selPuesto, this.selDireccion, this.selTipo);
        }
    }
    validaActualizacionCotizacion() {
        this.actCot.open();
    }
    descargarCotizacionComponent() {
        this.iniciarAnimacion();
        this.http.post(`${this.url}api/report/DescargarReporteCotizacion`, this.model.idCotizacion, { responseType: 'arraybuffer' })
            .subscribe(
                (data: ArrayBuffer) => {
                    const pdfDataUrl = this.arrayBufferToDataUrl(data);
                    window.open(pdfDataUrl, '_blank');
                },
                error => {
                    console.error('Error al obtener el archivo PDF', error);
                }
            );
    }
    arrayBufferToDataUrl(buffer: ArrayBuffer): string {
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const dataUrl = URL.createObjectURL(blob);
        return dataUrl;
    }

    iniciarAnimacion() {
        const boton = document.getElementById("miBoton") as HTMLButtonElement;
        boton.disabled = true;

        setTimeout(function () {
            boton.disabled = false;
        }, 2000);
    }
}
