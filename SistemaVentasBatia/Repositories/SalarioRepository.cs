using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface ISalarioRepository
    {
        Task<int> Agregar(Salario model);
        Task<bool> Actualizar(Salario model);
        Task<bool> Eliminar(int id);
        Task<Salario> Obtener(int id);

        Task<decimal> ObtenerSalarioMixto(int idPuesto);
        Task<decimal> ObtenerSalarioMixtoFrontera(int idPuesto);
        Task<decimal> ObtenerSalarioReal(int idPuesto);
        Task<decimal> ObtenerSalarioRealFrontera(int idPuesto);



        Task<IEnumerable<Salario>> Busqueda(int idTabulador, int idPuesto, int idTurno);
        Task<SalarioMinimo> ObtenerMinimo(int year);
    }
    public class SalarioRepository : ISalarioRepository
    {
        private readonly DapperContext ctx;

        public SalarioRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }

        public Task<bool> Actualizar(Salario model)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> Agregar(Salario model)
        {
            throw new System.NotImplementedException();
        }

        public async Task<decimal> ObtenerSalarioMixto(int idPuesto)
        {
            var query = @"SELECT salariomixto FROM tb_puesto_salario pa WHERE id_puesto = @idPuesto";
            decimal result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {

                    result = await connection.QueryFirstOrDefaultAsync<decimal>(query, new { idPuesto });
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }
        public async Task<decimal> ObtenerSalarioMixtoFrontera(int idPuesto)
        {
            //var query = @"SELECT pa.id_puesto idPuesto, salariomixto_frontera salarioMixtoFrontera FROM tb_puesto_salario pa WHERE id_puesto = @idPuesto  ";
            var query = @"SELECT salariomixto_frontera FROM tb_puesto_salario pa WHERE id_puesto = @idPuesto";
            decimal result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {

                    result = await connection.QueryFirstOrDefaultAsync<decimal>(query, new { idPuesto });
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }
        public async Task<decimal> ObtenerSalarioReal(int idPuesto)
        {
            var query = @"SELECT salarioreal FROM tb_puesto_salario pa WHERE id_puesto = @idPuesto ";
            decimal result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {

                    result = await connection.QueryFirstOrDefaultAsync<decimal>(query, new { idPuesto });
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }
        public async Task<decimal> ObtenerSalarioRealFrontera(int idPuesto)
        {
            var query = @"SELECT salarioreal_frontera FROM tb_puesto_salario pa WHERE id_puesto = @idPuesto";
            decimal result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {

                    result = await connection.QueryFirstOrDefaultAsync<decimal>(query, new { idPuesto });
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public async Task<IEnumerable<Salario>> Busqueda(int idTabulador, int idPuesto, int idTurno)
        {
            IEnumerable<Salario> ls;
            var query = @"SELECT id_salario as IdSalario, id_tabulador as IdTabulador, id_puesto as IdPuesto, id_turno as IdTurno, salario as SalarioI, fecha_alta as FechaAlta, id_personal as IdPersonal
                        FROM tb_cotiza_salario a
                        WHERE a.id_tabulador = @idTabulador
						and a.id_puesto = isnull(nullif(@idPuesto, 0), a.id_puesto)
						and a.id_turno = isnull(nullif(@idTurno, 0), a.id_turno);";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ls = (await connection.QueryAsync<Salario>(query, new { idTabulador, idPuesto, idTurno })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }

        public Task<bool> Eliminar(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Salario> Obtener(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<SalarioMinimo> ObtenerMinimo(int year)
        {
            SalarioMinimo sm;
            var query = @"select id_salario as IdSalario, faplica as FechaAplica, salariobase as SalarioBase, zona as Zona
                        from tb_salariominimo
                        where year(faplica) = @anio
                        order by faplica desc;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    sm = await connection.QueryFirstOrDefaultAsync<SalarioMinimo>(query, new { anio = year });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return sm;
        }
    }
}
