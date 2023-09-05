using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;

namespace SistemaVentasBatia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _logic;

        public MaterialController(IMaterialService service)
        {
            _logic = service;
        }

        [HttpGet("{id}/{pagina}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> LimpiezaMaterial(int idDir, int idPues, string keywords, int id, int pagina = 1)
        {
            var listaMaterialesVM = new ListaMaterialesCotizacionLimpiezaDTO()
            {
                Pagina = pagina,
                IdCotizacion = id,
                IdDireccionCotizacion = idDir,
                IdPuestoDireccionCotizacion = idPues,
                Keywords = keywords
            };

            await _logic.ObtenerListaMaterialesCotizacion(listaMaterialesVM);

            return listaMaterialesVM;
        }

        [HttpGet("[action]/{idPuestoDireccionCotizacion}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> GetByPuesto(int idPuestoDireccionCotizacion)
        {
            var materialCotizacion = await _logic.ObtenerListaMaterialesOperarioLimpieza(idPuestoDireccionCotizacion);

            return materialCotizacion;
        }

        [HttpGet("[action]/{idMaterialCotizacion}")]
        public async Task<ActionResult<MaterialCotizacionDTO>> GetById(int idMaterialCotizacion)
        {
            return await _logic.ObtenerMaterialCotizacionPorId(idMaterialCotizacion);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialCotizacionDTO>> AgregarMaterialOperario([FromBody] MaterialCotizacionDTO materialVM)
        {
            await _logic.AgregarMaterialOperario(materialVM);

            return materialVM;
        }

        [HttpPut]
        public async Task<ActionResult<MaterialCotizacionDTO>> EditarMaterialOperario([FromBody] MaterialCotizacionDTO materialVM)
        {
            await _logic.ActualizarMaterialCotizacion(materialVM);

            return materialVM;
        }

        [HttpDelete("{registroAEliminar}")]
        public async Task<ActionResult<bool>> EliminarMaterialOperario(int registroAEliminar)
        {
            //var idPuestoDireccionCotizacion = await _logic.ObtenerIdPuestoDireccionCotizacionPorMaterial(registroAEliminar);
         
            //var idDireccionCotizacion = await _logic.ObtenerIdDireccionCotizacionPorMaterial(registroAEliminar);

            //var idCotizacion = await _logic.ObtenerIdCotizacionPorMaterial(registroAEliminar);

            await _logic.EliminarMaterialDeCotizacion(registroAEliminar);

            return true;
        }
    }
}