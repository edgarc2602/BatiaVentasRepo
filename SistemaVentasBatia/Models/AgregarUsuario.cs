using System;

namespace SistemaVentasBatia.Models
{
    public class AgregarUsuario
    {
        public int IdPersonal { get; set; }
        public int AutorizaVentas { get; set; }
        public int EstatusVentas { get; set; }
        public int CotizadorVentas { get; set; }
        public int RevisaVentas { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Puesto { get; set; }
        public string Telefono { get; set; }
        public string TelefonoExtension { get; set; }
        public string TelefonoMovil { get; set; }
        public string Email { get; set; }
        public string Firma { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
    }
}