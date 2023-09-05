import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListaMaterial } from 'src/app/models/listamaterial';
declare var bootstrap: any;

@Component({
    selector: 'mate-widget',
    templateUrl: './material.widget.html'
})
export class MaterialWidget {
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    tipo: string = '';
    edit: number = 0;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    model: ListaMaterial = {} as ListaMaterial;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {}

    existe(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<ListaMaterial>(`${this.url}api/${this.tipo}/getbypuesto/${id}`).subscribe(response => {
            this.model.edit = this.edit;
            this.model = response;
        }, err => console.log(err));
    }
    
    agregarMaterial() {
        this.model.edit = 0;
        this.sendEvent.emit(0);
    }
    
    select(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.sendEvent.emit(id);
    }

    remove(id: number) {
        this.http.delete<boolean>(`${this.url}api/${this.tipo}/${id}`).subscribe(response => {
                if (response) {
                    this.existe(this.idP);
                }
        }, err => console.log(err));
    }

    open(cot: number, dir: number, pue: number, tp: string,edit: number) {
        this.idC = cot;
        this.idD = dir;
        this.idP = pue;
        this.tipo = tp;
        this.existe(this.idP);
        let docModal = document.getElementById('modalLimpiezaMaterialOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalLimpiezaMaterialOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }
}