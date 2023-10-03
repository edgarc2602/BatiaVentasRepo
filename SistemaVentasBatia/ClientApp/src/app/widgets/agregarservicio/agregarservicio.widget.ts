import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;

@Component({
    selector: 'agregarservicio-widget',
    templateUrl: './agregarservicio.widget.html'
})
export class AgregarServicioWidget {
    @Output('serEvent') sendEvent = new EventEmitter<number>();
    servicio: string = '';
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) { }

    guarda() {
        this.http.get<boolean>(`${this.url}api/producto/agregarservicio/${this.servicio}/${this.sinU.idPersonal}`).subscribe(response => {
            this.close();
            this.sendEvent.emit(4);
        }, err => console.log(err));
    }

    open() {
        let docModal = document.getElementById('modalLimpiezaAgregarServicio');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }
    close() {
        let docModal = document.getElementById('modalLimpiezaAgregarServicio');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }
}