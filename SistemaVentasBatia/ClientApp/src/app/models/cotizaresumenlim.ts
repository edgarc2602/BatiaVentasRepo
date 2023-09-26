export interface CotizaResumenLim {
    idCotizacion: number;
    idProspecto: number;
    idServicio: number;
    salario: number;
    cargaSocial: number;
    provisiones: number;
    material: number;
    uniforme: number;
    equipo: number;
    herramienta: number;
    subTotal: number;
    indirecto: number;
    utilidad: number;
    comisionSV: number;
    total: number;
    idCotizacionOriginal: number;

    nombreComercial: string;
    utilidadPor: string;
    indirectoPor: string;
    csvPor: string;
    
}