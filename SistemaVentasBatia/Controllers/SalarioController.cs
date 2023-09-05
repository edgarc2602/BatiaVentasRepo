using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalarioController : ControllerBase
    {
        private readonly ISalarioService _logic;

        public SalarioController(ISalarioService logic)
        {
            _logic = logic;
        }

        [HttpGet("{idtab}/{idpue}/{idtur}")]
        public async Task<SalarioMinDTO> GetFind(int idtab, int idpue, int idtur)
        {
            return await _logic.GetFind(idtab, idpue, idtur);
        }
    }
}
