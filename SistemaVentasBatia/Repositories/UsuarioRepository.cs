using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(Acceso acceso);
        Task InsertarFirmaUsuario(ImagenRequest imagenBase64, int idPersonal);
        Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios();
        Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales();
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

        public async Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios()
        {
            var query = @"
SELECT
    p.Per_Nombre +' '+ p.Per_Paterno AS Nombre,
    av.IdPersonal AS IdPersonal,
    (SELECT COUNT(id_personal) FROM tb_cotizacion c WHERE c.id_personal = av.IdPersonal) AS Cotizaciones,
    (SELECT COUNT(id_personal) FROM tb_prospecto pros WHERE pros.id_personal = av.IdPersonal) AS Prospectos
FROM
    Autorizacion_ventas av
INNER JOIN
    Personal p ON av.IdPersonal = p.IdPersonal
WHERE
    av.per_revisa = 0 
";
            var usuarios = new List<UsuarioGrafica>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                        usuarios = (await connection.QueryAsync<UsuarioGrafica>(query)).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return usuarios;
        }
        public async Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales()
        {
            var query = @"
WITH MesesDeReferencia AS (
    SELECT 1 AS Mes
    UNION SELECT 2
    UNION SELECT 3
    UNION SELECT 4
    UNION SELECT 5
    UNION SELECT 6
    UNION SELECT 7
    UNION SELECT 8
    UNION SELECT 9
    UNION SELECT 10
    UNION SELECT 11
    UNION SELECT 12
)
SELECT
    p.Per_Nombre +' '+ p.Per_Paterno AS Nombre,
    av.IdPersonal AS IdPersonal,
    m.Mes AS Mes,
    COALESCE(SUM(CASE WHEN MONTH(c.fecha_alta) = m.Mes THEN 1 ELSE 0 END), 0) AS CotizacionesPorMes
FROM
    Autorizacion_ventas av
INNER JOIN
    Personal p ON av.IdPersonal = p.IdPersonal
CROSS JOIN
    MesesDeReferencia m
LEFT JOIN
    tb_cotizacion c ON av.IdPersonal = c.id_personal
        AND MONTH(c.fecha_alta) = m.Mes
WHERE
    av.per_revisa = 0
GROUP BY
    av.IdPersonal,
    p.Per_Nombre,
    p.Per_Paterno,
    m.Mes
ORDER BY
    av.IdPersonal,
    Mes;
";
            var usuarios = new List<UsuarioGraficaMensual>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    usuarios = (await connection.QueryAsync<UsuarioGraficaMensual>(query)).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return usuarios;
        }
    }
}
