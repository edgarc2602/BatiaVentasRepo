import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ListaProspecto } from '../../models/listaprospecto';
import { ItemN } from 'src/app/models/item';
import { EliminaWidget } from 'src/app/widgets/elimina/elimina.widget';

import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';


@Component({
    selector: 'prospecto',
    templateUrl: './prospecto.component.html',
    animations: [fadeInOut],
})
export class ProspectoComponent {
    lspro: ListaProspecto = {
        idEstatusProspecto: 1, keywords: '', numPaginas: 0,
        pagina: 1, prospectos: [], rows: 0       
    };
    lests: ItemN[] = [];
    idpro: number = 0;
    @ViewChild(EliminaWidget, { static: false }) eliw: EliminaWidget;
    autorizacion: number = 0;
    
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private rter: Router, public user: StoreUser) {
        http.get<ItemN[]>(`${url}api/prospecto/getestatus`).subscribe(response => {
            this.lests = response;
        }, err => console.log(err));
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;
        }, err => console.log(err));
        this.lista();
    }

    lista() {
        let qust: string = this.lspro.keywords == '' ? '' : '?keywords=' + this.lspro.keywords;
        this.http.get<ListaProspecto>(`${this.url}api/prospecto/${this.user.idPersonal}/${this.lspro.pagina}/${this.lspro.idEstatusProspecto}${qust}`).subscribe(response => {
            this.lspro = response;
        }, err => console.log(err));
    }

    muevePagina(event) {
        this.lspro.pagina = event;
        this.lista();
    }

    nuevo() {
        this.rter.navigate(['/exclusivo/nuevopros']);
    }

    elige(id: number) {
        this.idpro = id;
        this.eliw.titulo = 'Desactivar';
        this.eliw.mensaje = 'El estatus cambiará a "Inactivo"';
        this.eliw.open();
    }

    elimina($event) {
        if ($event) {
            this.http.delete<boolean>(`${this.url}api/prospecto/${this.idpro}`).subscribe(response => {
                console.log(response);
            }, err => console.log(err));
        }
        this.lista();
    }
    goBack() {
        window.history.back();
    }
}