//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using SistemaVentasBatia.DTOs;
//using SistemaVentasBatia.Services;

//namespace SistemaVentasBatia.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ServicioController : ControllerBase
//    {
//        private readonly IMaterialService _logic;
        

        
//        public ServicioController(IMaterialService service)
//        {
//            _logic = service;
//        }

//        [HttpGet("{idCotizacion}/{idDireccionCotizacion}")]
//        public async Task<ActionResult<ListaServiciosCotizacionLimpiezaDTO>> ObtenerListaServiciosCotizacion([FromBody]int idCotizacion = 0, int idDireccionCotizacion = 0)
//        {
//            var listaServiciosVM = new ListaServiciosCotizacionLimpiezaDTO()
//            {
//                IdCotizacion = idCotizacion,
//            };
            
//            await _logic.ObtenerListaServiciosCotizacion(idCotizacion, idDireccionCotizacion);

//            return listaServiciosVM;
//        }

        
//        //[HttpGet("[action]/{idServicioCotizacion}")]
//        //public async Task<ActionResult<ServicioCotizacionDTO>> GetById(int idServicioCotizacion)
//        //{
//        //    return await _logic.ObtenerServicioCotizacionPorId(idServicioCotizacion);
//        //}





//    }
//}