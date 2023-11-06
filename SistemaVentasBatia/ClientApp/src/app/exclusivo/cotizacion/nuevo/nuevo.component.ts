import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Prospecto } from 'src/app/models/prospecto';
import { Cotizacion } from 'src/app/models/cotizacion';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;
import { fadeInOut } from 'src/app/fade-in-out';

@Component({
    selector: 'cot-nuevo',
    templateUrl: './nuevo.component.html',
    animations: [fadeInOut],
})
export class CotizaComponent {
    lpros: Prospecto[] = [];
    model: Cotizacion = {} as Cotizacion;
    sers: ItemN[] = [];
    lerr: any = {};

    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient,
        private rtr: Router, private sinU: StoreUser
    ) {
        this.nuevo();
        http.post<Prospecto[]>(`${url}api/prospecto/getcatalogo`, this.sinU.idPersonal).subscribe(response => {
            this.lpros = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.model = {
            idCotizacion: 0, idProspecto: 0, idServicio: 0, total: 0,
            fechaAlta: fec.toISOString(), idCotizacionOriginal: 0,
            idPersonal: this.sinU.idPersonal, listaServicios: []
        };
    }

    guarda() {
        this.lerr = {};
        this.model.listaServicios = this.sers;
        console.log(this.model);
        if (this.valida()) {
            if (this.model.idCotizacion == 0) {
                this.http.post<boolean>(`${this.url}api/cotizacion`, this.model).subscribe(response => {
                    console.log(response);
                    this.closeNew();
                    this.closeSel();
                    this.rtr.navigate(['/exclusivo/cotiza/' + this.model.idProspecto ]);
                }, err => console.log(err));
            }
        }
    }

    valida() {
        let val: ItemN = this.sers.filter(x => x.act)[0];
        if (!val) {
            this.lerr['ListaServicio'] = ['Servicio es requerido'];
            return false;
        }
        return true;
    }

    ferr(nm: string) {
        let fld = this.lerr[nm];
        if (fld)
            return true;
        else
            return false;
    }

    terr(nm: string) {
        let fld = this.lerr[nm];
        let msg: string = fld.map((x: string) => "-" + x);
        return msg;
    }

    openSel() {
        this.lerr = {};
        let docModal = document.getElementById('modalSeleccionarProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    closeSel() {
        let docModal = document.getElementById('modalSeleccionarProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    openNew() {
        this.nuevo();
        this.lerr = {};
        let docModal = document.getElementById('modalCrearProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    closeNew() {
        let docModal = document.getElementById('modalCrearProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    savePros(event) {
        console.log(event);
        this.model.idProspecto = event;
        this.guarda();
        this.closeNew();
    }
    goBack() {
        window.history.back();
    }
}