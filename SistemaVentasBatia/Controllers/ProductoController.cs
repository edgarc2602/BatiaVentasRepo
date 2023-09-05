using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService logic;

        public ProductoController(IProductoService service)
        {
            logic = service;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetMaterial(int id)
        {
            return await logic.GetMaterialPuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostMaterial(MaterialPuestoDTO dto)
        {
            await logic.CreateMaterial(dto);
            return dto;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetHerramienta(int id)
        {
            return await logic.GetHerramientaPuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostHerramienta(MaterialPuestoDTO dto)
        {
            await logic.CreateHerramienta(dto);
            return dto;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetEquipo(int id)
        {
            return await logic.GetEquipoPuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostEquipo(MaterialPuestoDTO dto)
        {
            await logic.CreateEquipo(dto);
            return dto;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetUniforme(int id)
        {
            return await logic.GetUniformePuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostUniforme(MaterialPuestoDTO dto)
        {
            await logic.CreateUniforme(dto);
            return dto;
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelMaterial(int id)
        {
            return await logic.DeleteMaterial(id);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelHerramienta(int id)
        {
            return await logic.DeleteHerramienta(id);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelEquipo(int id)
        {
            return await logic.DeleteEquipo(id);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelUniforme(int id)
        {
            return await logic.DeleteUniforme(id);
        }
    }
}
