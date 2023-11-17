import { Component, Inject, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;
import { AgregarUsuario } from 'src/app/models/agregarusuario';
import Swal from 'sweetalert2';


@Component({
    selector: 'usuarioadd-widget',
    templateUrl: './usuarioadd.widget.html'
})
export class UsuarioAddWidget {

    @Output('actualizarUsuarios') returnUsuarioAdd = new EventEmitter();
    edit: number = 0;
    response: boolean = false;
    idPersonal: number = 0;
    agregarusuario: AgregarUsuario = {
        idPersonal: 0, autorizaVentas: 0, estatusVentas: 0, cotizadorVentas: 0, revisaVentas: 0, nombres: '', apellidoPaterno: '',
        apellidoMaterno: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '', firma: '',
        usuario: '', password: ''
    }
    selectedImage: string | ArrayBuffer | null = null;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) { }

    guarda() {
        if (this.agregarusuario.autorizaVentas == 1) {
            this.agregarusuario.autorizaVentas = 1;
        }
        else {
            this.agregarusuario.autorizaVentas = 0;
        }
        if (this.agregarusuario.revisaVentas == 1) {
            this.agregarusuario.revisaVentas = 1;
        }
        else {
            this.agregarusuario.revisaVentas = 0;
        }
        if (this.agregarusuario.cotizadorVentas == 1) {
            this.agregarusuario.cotizadorVentas = 1;
        }
        else {
            this.agregarusuario.cotizadorVentas = 0;
        }
        if (this.agregarusuario.estatusVentas == 1) {
            this.agregarusuario.estatusVentas = 1;
        }
        else {
            this.agregarusuario.estatusVentas = 0;
        }
        if (this.selectedImage == null) {
            this.agregarusuario.firma = "";
        }
        else {
            this.agregarusuario.firma = this.selectedImage.toString();
        }
        
        if (this.edit > 0) {
            this.http.put<boolean>(`${this.url}api/usuario/actualizarusuario`, this.agregarusuario).subscribe(response => {
                this.response = response;
                this.close();
                this.returnUsuarioAdd.emit();
            }, err => {
                console.log(err);

            });
        }
        if(this.edit == 0){
            this.http.put<boolean>(`${this.url}api/usuario/agregarusuario`, this.agregarusuario).subscribe(response => {
                this.response = response;
                
                if (response == false) {
                    Swal.fire({
                        title: 'Error',
                        text: 'No se encuentra el usuario, porfavor verifique el nombre',
                        icon: 'error',
                        timer: 2000,
                        showConfirmButton: false,
                    });
                }
                else {
                    this.close();
                    this.returnUsuarioAdd.emit();
                    
                }
            }, err => {
                console.log(err);
                Swal.fire({
                    title: 'Error',
                    text: 'No se encuentra el usuario, porfavor verifique el nombre',
                    icon: 'error',
                    timer: 2000,
                    showConfirmButton: false,
                });
            });
        }
    }

    nuevo() {
        this.agregarusuario = {
            idPersonal: 0, autorizaVentas: 0, estatusVentas: 0, cotizadorVentas: 0, revisaVentas: 0, nombres: '', apellidoPaterno: '',
            apellidoMaterno: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '', firma: '',
            usuario: '', password: ''
        }
        this.selectedImage = null;
    }
    existe() {
        this.nuevo();
        this.http.get<AgregarUsuario>(`${this.url}api/usuario/obtenerusuarioporidpersonal/${this.idPersonal}`).subscribe(response => {
            this.agregarusuario = response;
            
            if (this.agregarusuario.firma != '') {
                this.agregarusuario.firma = 'data:image/jpeg;base64,' + this.agregarusuario.firma;
                this.selectedImage = this.agregarusuario.firma;
            }
            else {
                this.selectedImage = null;
            }
        })
    }

    open(idPersonal: number) {
        this.idPersonal = idPersonal;
        if (idPersonal > 0) {
            this.existe();
        }
        else {
            this.nuevo();

        }
        this.edit = idPersonal;
        let docModal = document.getElementById('modalAgregarUsuario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalAgregarUsuario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }


    arrayBufferToBase64(arrayBuffer: ArrayBuffer): string {
        const uint8Array = new Uint8Array(arrayBuffer);
        return btoa(String.fromCharCode.apply(null, uint8Array));
    }
    convertirImagen(): void {
        this.selectedImage = null;
        if (this.selectedImage) {
            if (this.selectedImage instanceof ArrayBuffer) {
                const base64Firma = this.arrayBufferToBase64(this.selectedImage);
                this.agregarusuario.firma = base64Firma;
            } else if (typeof this.selectedImage === 'string') {
                this.agregarusuario.firma = this.selectedImage;
            } else {
                console.error('Tipo no compatible para selectedImage');
            }
            //this.http.post<boolean>(`${this.url}api/usuario/agregarusuario`, this.agregarusuario).subscribe(response => {
                
            //});
        }
    }
    onFileSelected(event: any): void {
        const selectedFile = event.target.files[0];
        if (selectedFile) {
            const reader = new FileReader();
            reader.onload = (e: any) => {
                this.selectedImage = e.target.result as string | ArrayBuffer | null;
            };
            reader.readAsDataURL(selectedFile);
        }
    }

    limpiarImagen() {
        this.selectedImage = null;
    }
}