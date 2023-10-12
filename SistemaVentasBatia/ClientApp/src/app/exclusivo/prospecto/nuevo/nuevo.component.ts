import { Component, Inject, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { Prospecto } from '../../../models/prospecto';
import { ItemN } from '../../../models/item';
import { ListaDireccion } from 'src/app/models/listadireccion';
import { DireccionWidget } from 'src/app/widgets/direccion/direccion.widget';
import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';

@Component({
    selector: 'pros-nuevo',
    templateUrl: './nuevo.component.html',
    providers: [DatePipe],
    animations: [fadeInOut],
})
export class ProsNuevoComponent implements OnInit, OnDestroy {
    @ViewChild(DireccionWidget, { static: false }) dirAdd: DireccionWidget;
    pro: Prospecto = {} as Prospecto;
    sub: any;
    docs: ItemN[] = [];
    direcs: ListaDireccion = {
        idProspecto: 0, idCotizacion: 0, idDireccion: 0, pagina: 0, direcciones: []
    };
    idDirecc: number = 0;
    evenSub: Subject<void> = new Subject<void>();
    isErr: boolean = false;
    validaMess: string = '';
    lerr: any = {};
 
    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe,
        private router: Router, private route: ActivatedRoute, private sinU: StoreUser
    ) {
        http.get<ItemN[]>(`${url}api/prospecto/getdocumento`).subscribe(response => {
            this.docs = response;
        }, err => console.log(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.pro = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '',
            representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [],
            idPersonal: this.sinU.idPersonal
        };
    }

    existe(id: number) {
        this.http.get<Prospecto>(`${this.url}api/prospecto/${id}`).subscribe(response => {
            this.pro = response;
            this.docs = this.pro.listaDocumentos;
            this.getDir();
        }, err => console.log(err));
    }

    getDir() {
        this.http.get<ListaDireccion>(`${this.url}api/direccion/${this.pro.idProspecto}`).subscribe(response => {
            this.direcs = response;
        }, err => console.log(err));
    }

    selDir(idD: number) {
        this.idDirecc = idD;
        this.dirAdd.open(this.pro.idProspecto, idD);
    }

    guarda() {
        this.pro.listaDocumentos = this.docs;
        this.lerr = {};
        if (this.valida()) {
            if (this.pro.idProspecto == 0) {
                this.http.post<Prospecto>(`${this.url}api/prospecto`, this.pro).subscribe(response => {
                    this.pro.idProspecto = response.idProspecto;
                    this.isErr = false;
                    this.validaMess = 'Prospecto guardado';
                    this.evenSub.next();
                    console.log(response);
                }, err => {
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                    this.isErr = true;
                    this.validaMess = '¡Ha ocurrido un error!';
                    this.evenSub.next();
                });
            } else {
                this.http.put<Prospecto>(`${this.url}api/prospecto`, this.pro).subscribe(response => {
                    console.log(response);
                    this.isErr = false;
                    this.validaMess = 'Prospecto guardado';
                    this.evenSub.next();
                }, err => {
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                    this.isErr = true;
                    this.validaMess = '¡Ha ocurrido un error!';
                    this.evenSub.next();
                });
            }
        }
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

    recEvent($event) {
        this.selDir($event);
        this.getDir();
    }

    ngOnInit(): void {
        this.sub = this.route.params.subscribe(params => {
            let idpro: number = +params['id'];
            if (idpro) {
                if (idpro == 0)
                    this.nuevo();
                else
                    this.existe(+params['id']);
            } else
            this.nuevo();
        });
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }
}