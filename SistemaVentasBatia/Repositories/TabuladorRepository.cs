using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface ITabuladorRepository
    {
        Task<int> Agregar(Tabulador model);
        Task<bool> Actualizar(Tabulador model);
        Task<bool> Eliminar(int id);
        Task<Tabulador> Obtener(int id);
        Task<IEnumerable<Tabulador>> ObtenerPorEstado(int id);
    }

    public class TabuladorRepository : ITabuladorRepository
    {
        private readonly DapperContext ctx;

        public TabuladorRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }

        public Task<bool> Actualizar(Tabulador model)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> Agregar(Tabulador model)
        {
            int reg = 0;
            var query = @"insert into tb_cotiza_tabulador(nombre, id_estado)
                        values(@Nombre, @IdEstado);
                        select SCOPE_IDENTITY();";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    reg = await connection.ExecuteScalarAsync<int>(query, model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public Task<bool> Eliminar(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Tabulador> Obtener(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Tabulador>> ObtenerPorEstado(int id)
        {
            IEnumerable<Tabulador> ls;
            var query = @"SELECT id_tabulador as IdTabulador, nombre as Nombre, id_estado as IdEstado
                        FROM tb_cotiza_tabulador a
                        WHERE a.id_estado = isnull(nullif(@id, 0), a.id_estado);";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ls = (await connection.QueryAsync<Tabulador>(query, new { id })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }
    }
}
