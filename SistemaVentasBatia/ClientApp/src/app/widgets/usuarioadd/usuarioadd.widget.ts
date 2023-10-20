import { Component, Inject,Output, EventEmitter} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;

@Component({
    selector: 'usuadd-widget',
    templateUrl: './usuarioadd.widget.html'
})
export class UsuarioAddWidget {
    
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {}
        
        
    //existe(id: number) {
    //    this.edit = 1;
    //    this.model.edit = this.edit;
    //    this.http.get<Material>(`${this.url}api/${this.tipo}/getbyid/${id}`).subscribe(response => {
    //        this.model = response;
    //        this.model.edit = this.edit;
    //    }, err => console.log(err));
    //}
    //guarda() {
    //    this.http.post<Material>(`${this.url}api/${this.tipo}`, this.model).subscribe(response => {
    //        this.close();
    //        this.sendEvent.emit(2);
    //    }, err => console.log(err));
    //    if (this.model.idPuestoDireccionCotizacion != 0) {
    //        this.returnModal.emit(true);
    //    }
    //}

    open() {
        //this.edit = edit;
        //this.idC = cot;
        //this.idD = dir;
        //this.idP = pue;
        //this.idS = ser;
        //this.tipo = tp;
        //if (this.idP != 0) {
        //    this.tipo = this.tipo.toString() + 'ope';
        //}
        //this.lista();
        //if (id == 0) {
        //    this.nuevo(this.idP);
        //} else {
        //    this.existe(id);
        //}
        let docModal = document.getElementById('modalAgregarUsuario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalAgregarUsuario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();

        //if (this.model.idPuestoDireccionCotizacion != 0) {
        //    this.returnModal.emit(true);
        //}
    }
}