import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { Servicio } from 'src/app/models/servicio';
import { StoreUser } from 'src/app/stores/StoreUser';
import { numberFormat } from 'highcharts';
declare var bootstrap: any;

@Component({
    selector: 'servadd-widget',
    templateUrl: './servicioadd.widget.html'
})
export class ServicioAddWidget {
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idS: number = 0;
    showSuc: boolean = false;
    tipo: string = 'servicio';
    edit: number = 0;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    @Output('returnModal') returnModal = new EventEmitter<boolean>();
    model: Servicio = {} as Servicio;
    dirs: Catalogo[] = [];
    pues: Catalogo[] = [];
    mats: Catalogo[] = [];
    sers: Catalogo[] = [];
    fres: ItemN[] = [];
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {}
        
    lista() {
        //this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idS}/${this.tipo}`).subscribe(response => {
        //    this.mats = response;
        //}, err => console.log(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));

        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getsucursalbycot/${this.idC}`).subscribe(response => {
            this.dirs = response;
        }, err => console.log(err));
        this.http.get<ItemN[]>(`${this.url}api/catalogo/getfrecuencia`).subscribe(response => {
            this.fres = response;
        }, err => console.log(err));
    }

    nuevo(id: number) {
        this.edit = 0;
        let fec: Date = new Date();
        this.model = {
            idServicioExtraCotizacion: 0,
            idServicioExtra: 0,
            ServicioExtra: '',
            idCotizacion: this.idC,
            idDireccionCotizacion: this.idD,
            direccion: '',
            precioUnitario: 0,
            cantidad: 0,
            total: 0,
            importeMensual: 0,
            idFrecuencia: 0,
            fechaAlta: fec.toISOString(),
            idPersonal: this.sinU.idPersonal,
            edit: this.edit
        };
    }
        
    existe(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<Servicio>(`${this.url}api/material/serviciogetbyid/${id}`).subscribe(response => {
            this.model = response;
            this.model.edit = this.edit;
        }, err => console.log(err));
    }
    guarda() {
        if (this.model.idDireccionCotizacion == 0) {
            this.model.idDireccionCotizacion = 0
        }


        if (this.edit == 0) {
            this.http.post<Servicio>(`${this.url}api/material/insertarserviciocotizacion`, this.model).subscribe(response => {
                this.close();
                this.sendEvent.emit(2);
            }, err => console.log(err));
        }
        if (this.edit == 1) {
            this.http.post<Servicio>(`${this.url}api/material/actualizarserviciocotizacion`, this.model).subscribe(response => {
                this.close();
                this.sendEvent.emit(2);
            }, err => console.log(err));
        }
    }

    open(id: number, idCotizacion) {
        this.idC = idCotizacion;
        //this.edit = edit;
        //this.idC = cot;
        //this.idD = dir;
        //this.idP = pue;
        //this.idS = ser;
        //this.tipo = tp;w
        //this.showSuc = showS;
        this.lista();
        if (id == 0) {
            this.nuevo(this.idP);
        } else {
            this.existe(id);
        }
        let docModal = document.getElementById('modalLimpiezaAgregarServicioCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalLimpiezaAgregarServicioCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();

        //if (this.model.idPuestoDireccionCotizacion != 0) {
        //    this.returnModal.emit(true);
        //}
    }
}