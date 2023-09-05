import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ProductoItem } from 'src/app/models/productoitem';
import { ProductoWidget } from 'src/app/widgets/producto/producto.widget';

@Component({
    selector: 'catalogo-comp',
    templateUrl: './catalogo.component.html'
})
export class CatalogoComponent {
    @ViewChild(ProductoWidget, { static: false }) prow: ProductoWidget;
    pues: Catalogo[] = [];
    selPuesto: number = 0;
    mates: ProductoItem[] = [];
    grupo: string = 'material';

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {
        http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
    }

    chgPuesto() {
        this.getMaterial();
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

    closeMat($event) {
        this.getMaterial();
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
}