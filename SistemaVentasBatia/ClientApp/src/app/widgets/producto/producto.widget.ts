import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { MaterialPuesto } from 'src/app/models/materialpuesto';
import { StoreUser } from '../../stores/StoreUser';
declare var bootstrap: any;

@Component({
    selector: 'producto-widget',
    templateUrl: './producto.widget.html'
})
export class ProductoWidget implements OnChanges {
    @Input() idP: number = 0;
    @Input() grupo: string = '';
    @Output('saveEvent') sendEvent = new EventEmitter<boolean>();
    model: MaterialPuesto = {} as MaterialPuesto;
    sers: ItemN[] = [];
    fres: ItemN[] = [];
    lsmat: Catalogo[] = [];
    idSer: number = 2;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        http.get<ItemN[]>(`${url}api/catalogo/getfrecuencia`).subscribe(response => {
            this.fres = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    inicio(id: number, tipo: number, idPuesto: number) {
        if (id != 0) {
            this.existe(id,tipo,idPuesto);
        }
        else {
            this.model = {
                idMaterialPuesto: 0, claveProducto: '', idPuesto: this.idP,
                precio: 0, cantidad: 0, idFrecuencia: 0, idPersonal: this.sinU.idPersonal
            };
            this.idSer = 2;
            this.getProductos();
            this.open();
        }
    }

    existe(idProducto: number, tipo: number, idPuesto: number) {
        this.http.get<MaterialPuesto>(`${this.url}api/producto/obtenerproductodefault/${idProducto}/${tipo}/${idPuesto}`).subscribe(response => {
            this.model = response;
        }, err => console.log(err));
        this.open();
        this.getProductos();
    }

    guarda() {
        this.lerr = {};
        if (this.valida()) {
            if (this.model.idMaterialPuesto == 0) {
                this.http.post<MaterialPuesto>(`${this.url}api/producto/post${this.grupo}`, this.model).subscribe(response => {
                    this.sendEvent.emit(true);
                    this.close();
                }, err => {
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }
            if (this.model.idMaterialPuesto != 0) {
                this.http.post<MaterialPuesto>(`${this.url}api/producto/post${this.grupo}`, this.model).subscribe(response => {
                    this.sendEvent.emit(true);
                    this.close();
                }, err => {
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }
        }
    }

    getProductos() {
        this.lsmat = [];
        if (this.idSer > 0) {
            this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idSer}/${this.grupo}`).subscribe(response => {
                this.lsmat = response;
            }, err => console.log(err));
        }
    }

    chgServicio() {
        this.model.claveProducto = '';
        this.getProductos();
    }

    open() {
        let docModal = document.getElementById('modalAgregarProductoPuesto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalAgregarProductoPuesto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    valida() {
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

    ngOnChanges(changes: SimpleChanges): void {
    }
}