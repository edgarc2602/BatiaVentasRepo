using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.DTOs
{
    public class ListaDireccionDTO
    {
        public int IdProspecto { get; set; }

        public int IdCotizacion { get; set; }

        public int IdDireccion { get; set; }

        public int Pagina { get; set; }

        public List<DireccionMinDTO> Direcciones { get; set; }

    }
}
