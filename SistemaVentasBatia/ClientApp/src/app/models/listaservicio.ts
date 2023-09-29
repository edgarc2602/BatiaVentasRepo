import { ServicioMin } from './serviciomin';

export interface ListaServicio {
    idCotizacion: number;
    idDireccionCotizacion: number;
    Direccion: string;
    keywords: string;
    pagina: number;
    rows: number;
    numPaginas: number; 
    ServiciosCotizacion: ServicioMin[];
    edit: number;
}