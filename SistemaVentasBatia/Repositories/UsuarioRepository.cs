using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(Acceso acceso);
        Task InsertarFirmaUsuario(ImagenRequest imagenBase64, int idPersonal);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DapperContext _ctx;

        public UsuarioRepository(DapperContext context)
        {
            _ctx = context;
        }

        public async Task<Usuario> Login(Acceso acceso)
        {
            Usuario usu;
            string query = @"SELECT per_usuario Identificador, per_nombre Nombre, idpersonal as IdPersonal,
                    per_interno as IdInterno, per_status Estatus, id_empleado as IdEmpleado
                FROM personal where per_usuario = @Usuario and per_password = @Contrasena;"; // and per_status=0

            using (var connection = _ctx.CreateConnection())
            {
                usu = (await connection.QueryFirstOrDefaultAsync<Usuario>(query, acceso));
            }
            return usu;
        }

        public async Task InsertarFirmaUsuario(ImagenRequest imagenBase64, int idPersonal)
        {
            var base64Data = imagenBase64.ImagenBase64;

            if (base64Data.StartsWith("data:image/jpeg;base64,"))
            {
                base64Data = base64Data.Substring("data:image/jpeg;base64,".Length);
            }
            if (base64Data.StartsWith("data:image/png;base64,"))
            {
                base64Data = base64Data.Substring("data:image/png;base64,".Length);
            }

            var query = @"UPDATE Autorizacion_ventas SET per_firma = @base64Data WHERE IdPersonal = @idPersonal";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { base64Data, idPersonal });
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
