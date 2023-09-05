using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabuladorController : ControllerBase
    {
        private readonly ITabuladorService _logic;

        public TabuladorController(ITabuladorService logic)
        {
            _logic = logic;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<CatalogoDTO>> GetByEdo(int id)
        {
            return await _logic.GetPorEstado(id);
        }
    }
}
