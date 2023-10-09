using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _logic;

        public UsuarioController(IUsuarioService logic)
        {
            _logic = logic;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UsuarioDTO>> Login(AccesoDTO dto)
        {
            return await _logic.Login(dto);
        }

        [HttpPost("[action]/{idPersonal}")]
        public async Task<ActionResult<bool>> InsertarFirmaUsuario([FromBody] ImagenRequest imagenBase64, int idPersonal = 0)
        {
            await _logic.InsertarFirmaUsuario(imagenBase64, idPersonal);

            return true;
        }
    }
}
