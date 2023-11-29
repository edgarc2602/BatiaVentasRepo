import { Component, OnInit, Inject } from '@angular/core';
declare var bootstrap: any;
import { StoreUser } from 'src/app/stores/StoreUser';
import { HttpClient } from '@angular/common/http';


@Component({
    selector: 'lat-menu',
    templateUrl: './latmenu.component.html'
})
export class LatMenuComponent implements OnInit {
    autorizacion: number = 0;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;   
        }, err => console.log(err));
    }
    ngOnInit(): void {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });

    }
}