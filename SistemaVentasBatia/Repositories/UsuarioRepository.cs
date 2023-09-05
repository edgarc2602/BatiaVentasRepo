using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(Acceso acceso);
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
    }
}
