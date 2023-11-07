import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { Material } from 'src/app/models/material';
import { StoreUser } from 'src/app/stores/StoreUser';
import { numberFormat } from 'highcharts';
declare var bootstrap: any;
import { Subject } from 'rxjs';
import { ToastWidget } from '../toast/toast.widget';

@Component({
    selector: 'mateadd-widget',
    templateUrl: './materialadd.widget.html'
})
export class MaterialAddWidget {
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idS: number = 0;
    showSuc: boolean = false;
    tipo: string = 'material';
    edit: number = 0;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    @Output('returnModal') returnModal = new EventEmitter<boolean>();
    model: Material = {} as Material;
    dirs: Catalogo[] = [];
    pues: Catalogo[] = [];
    mats: Catalogo[] = [];
    fres: ItemN[] = [];
    hidedir: number = 0;
    evenSub: Subject<void> = new Subject<void>();
    isErr: boolean = false;
    validaMess: string = '';
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) { }

    lista() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idS}/${this.tipo}`).subscribe(response => {
            this.mats = response;
        }, err => console.log(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getpuestobycot/${this.idC}`).subscribe(response => {
            this.pues = response;
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
            idMaterialCotizacion: 0, claveProducto: '', idCotizacion: this.idC,
            idPuestoDireccionCotizacion: id, precioUnitario: 0, cantidad: 0, idFrecuencia: 0,
            total: 0, fechaAlta: fec.toISOString(), idDireccionCotizacion: this.idD, idPersonal: this.sinU.idPersonal, edit: this.edit
        };
    }

    existe(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<Material>(`${this.url}api/${this.tipo}/getbyid/${id}`).subscribe(response => {
            this.model = response;
            this.model.edit = this.edit;
        }, err => console.log(err));
    }
    guarda() {
        this.http.post<Material>(`${this.url}api/${this.tipo}`, this.model).subscribe(response => {
            this.close();
            this.sendEvent.emit(2);
            this.isErr = false;
            this.validaMess = 'Material agregado';
            this.evenSub.next();
        }, err => {
            console.log(err);
            this.isErr = true;
            this.validaMess = 'Ocurrio un error';
            this.evenSub.next();
        });
        if (this.model.idPuestoDireccionCotizacion != 0) {
            this.returnModal.emit(true);
        }
        else {
            this.returnModal.emit(false);
        }
    }

    open(cot: number, dir: number, pue: number, id: number, ser: number, tp: string, showS: boolean = false, edit: number) {
        this.edit = edit;
        this.idC = cot;
        this.idD = dir;
        this.idP = pue;
        //if (this.idP != 0) {
        //    this.hidedir = 1;
        //}
        //else {
        //    this.hidedir = 0;
        //}
        this.idS = ser;
        this.tipo = tp;
        this.showSuc = showS;
        this.lista();
        if (id == 0) {
            this.nuevo(this.idP);
        } else {
            this.existe(id);
        }
        let docModal = document.getElementById('modalLimpiezaAgregarMaterialCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalLimpiezaAgregarMaterialCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
        //this.returnModal.emit(true);
        if (this.model.idPuestoDireccionCotizacion != 0) {
            this.returnModal.emit(true);
        }
        else {
            this.returnModal.emit(false);
        }
    }
}