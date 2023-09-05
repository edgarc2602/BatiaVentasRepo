using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProspectoController : ControllerBase
    {
        private readonly ILogger<ProspectoController> _logger;
        private readonly IProspectosService prospectosSvc;
        private readonly ICatalogosService catalogosSvc;


        public ProspectoController(ILogger<ProspectoController> logger, IProspectosService prospectosSvc, ICatalogosService catalogosSvc)
        {
            _logger = logger;
            this.prospectosSvc = prospectosSvc;
            this.catalogosSvc = catalogosSvc;
        }

        [HttpGet("{pagina?}/{idEstatus?}")]
        public async Task<ActionResult<ListaProspectoDTO>> Index([FromQuery] string keywords, int pagina = 1, int idEstatus = 0)
        {
            ListaProspectoDTO listaProspectosVM = new ListaProspectoDTO()
            {
                IdEstatusProspecto = (EstatusProspecto)idEstatus,
                Pagina = pagina,
                Keywords = keywords ?? ""
            };

            await prospectosSvc.ObtenerListaProspectos(listaProspectosVM);

            return listaProspectosVM;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProspectoDTO>> EditarProspecto(int id)
        {
            return await prospectosSvc.ObtenerProspecto(id);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<ProspectoDTO>> GetCatalogo()
        {
            return await prospectosSvc.ObtenerCatalogoProspectos();
        }

        [HttpPut]
        public async Task<ActionResult<ProspectoDTO>> EditarProspecto(ProspectoDTO prospectoVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await prospectosSvc.EditarProspecto(prospectoVM);

            return Ok(prospectoVM);
        }

        [HttpDelete("{registroAEliminar}")]
        public async Task<ActionResult<bool>> EliminarProspecto(int registroAEliminar)
        {
            await prospectosSvc.EliminarProspecto(registroAEliminar);

            return true;
        }

        [HttpPost]
        public async Task<ActionResult<ProspectoDTO>> NuevoProspecto([FromBody] ProspectoDTO prospectoVM)
        {
            var coincidencias = await prospectosSvc.ObtenerCoincidenciasProspecto(nombreComercial: null, rfc: prospectoVM.Rfc);

            if (coincidencias.Count() > 0 )
            {
                ModelState.AddModelError("Rfc", "Este RFC ya se encuentra registrado");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await prospectosSvc.CrearProspecto(prospectoVM);        
            return prospectoVM;
        }

        [HttpGet]
        public async Task<int> ObtenerNumeroCoincidenciasProspecto(string nombreComercial = null, string rfc = null)
        {
            var coincidencias = await prospectosSvc.ObtenerNumeroCoincidenciasProspecto(nombreComercial, rfc);

            return coincidencias;
        }
        
        [HttpGet]
        public async Task<IActionResult> ObtenerCoincidenciasProspecto(string nombreComercial = null, string rfc = null)
        {
            var coincidenciasProspecto = await prospectosSvc.ObtenerCoincidenciasProspecto(nombreComercial, rfc);

            // return PartialView("_ModalCoincidenciasProspecto", coincidenciasProspecto);
            return Ok(coincidenciasProspecto);
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetDocumento()
        {
            List<Item<int>> ls = Enum.GetValues(typeof(Documento))
                .Cast<Documento>().Select(d => new Item<int>
                {
                    Id = (int)d,
                    Nom = d.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetEstatus()
        {
            List<Item<int>> ls = Enum.GetValues(typeof(EstatusProspecto))
                .Cast<EstatusProspecto>().Select(d => new Item<int>
                {
                    Id = (int)d,
                    Nom = d.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetServicio()
        {
            List<Item<int>> ls = Enum.GetValues(typeof(Servicio))
                .Cast<Servicio>().Select(s => new Item<int>
                {
                    Id = (int)s,
                    Nom = s.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }
    }
}
