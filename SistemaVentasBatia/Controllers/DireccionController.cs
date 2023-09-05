using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;

namespace SistemaVentasBatia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DireccionController : ControllerBase
    {
        private readonly ILogger<DireccionController> _logger;
        private readonly IProspectosService _prospectosSvc;
        private readonly ICotizacionesService _cotizaSvc;

        public DireccionController(
            ILogger<DireccionController> logger, IProspectosService prospectosSvc
          , ICotizacionesService cotizaSvc)
        {
            _logger = logger;
            _prospectosSvc = prospectosSvc;
            _cotizaSvc = cotizaSvc;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ListaDireccionDTO>> Directorio(int id)
        {
            var listaDireccionesVM = new ListaDireccionDTO()
            {
                IdProspecto = id,
                Pagina = 1
            };

            await _prospectosSvc.ObtenerListaDirecciones(listaDireccionesVM);

            return listaDireccionesVM;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<Item<int>>> GetCatalogo(int id)
        {
            IEnumerable<Item<int>> ls = (await _cotizaSvc.ObtenerCatalogoDireccionesPorProspecto(id)).Select(x => new Item<int> { Id = x.IdDireccion, Nom = x.NombreSucursal });

            return ls;
        }

        [HttpPost]
        public async Task<ActionResult<DireccionDTO>> AgregarDireccion([FromBody] DireccionDTO direccionVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _prospectosSvc.CrearDireccion(direccionVM);

            return direccionVM;
        }

        [HttpGet("{id}/{idProspecto}")]
        public async Task<IActionResult> EditarDireccion(int id, int idProspecto)
        {
            var direccion = await _prospectosSvc.ObtenerDireccionPorId(id);

            direccion.IdProspecto = idProspecto;

            // ViewBag.Estados = new List<SelectListItem>((await catalogosSvc.ObtenerEstados()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // ViewBag.TiposInmueble = new List<SelectListItem>((await catalogosSvc.ObtenerTiposInmueble()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // return PartialView("_ModalEditarDireccion", direccion);
            return Ok(direccion);
        }

        [HttpPut]
        public async Task<ActionResult<DireccionDTO>> EditarDireccion([FromBody] DireccionDTO direccionVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _prospectosSvc.ActualizarDireccion(direccionVM);

            return direccionVM;
        }
    }
}