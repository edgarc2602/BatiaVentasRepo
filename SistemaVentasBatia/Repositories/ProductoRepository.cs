using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Repositories
{
    public interface IProductoRepository
    {
        Task AgregarMaterialPuesto(MaterialPuesto mat);
        Task<bool> ActualizarMaterialPuesto(MaterialPuesto mat);
        Task<bool> EliminarMaterialPuesto(int id);
        Task<IEnumerable<ProductoItem>> ObtenerMaterialPuesto(int id);
        Task AgregarUniformePuesto(MaterialPuesto uni);
        Task<bool> ActualizarUniformePuesto(MaterialPuesto uni);
        Task<bool> EliminarUniformePuesto(int id);
        Task<bool> EliminarServicio(int id);
        Task<bool> AgregarServicio(string servicio, int idPersonal);
        Task<IEnumerable<ProductoItem>> ObtenerUniformePuesto(int id);
        Task AgregarEquipoPuesto(MaterialPuesto equi);
        Task<bool> ActualizarEquipoPuesto(MaterialPuesto equi);
        Task<bool> EliminarEquipoPuesto(int id);
        Task<IEnumerable<ProductoItem>> ObtenerEquipoPuesto(int id);
        Task AgregarHerramientaPuesto(MaterialPuesto her);
        Task<bool> ActualizarHerramientaPuesto(MaterialPuesto her);
        Task<bool> EliminarHerramientaPuesto(int id);
        Task<IEnumerable<ProductoItem>> ObtenerHerramientaPuesto(int id);
    }

    public class ProductoRepository : IProductoRepository
    {
        private readonly DapperContext ctx;

        public ProductoRepository(DapperContext context)
        {
            ctx = context;
        }

        public async Task<bool> ActualizarEquipoPuesto(MaterialPuesto equi)
        {
            bool reg = false;
            var query = @"update tb_cotiza_equipo set clave = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_equipo_puesto = @IdMaterialPuesto;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, equi);
                    reg = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public async Task<bool> ActualizarHerramientaPuesto(MaterialPuesto her)
        {
            bool reg = false;
            var query = @"update tb_herramienta_puesto set clave = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_herramienta_puesto = @IdMaterialPuesto;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, her);
                    reg = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public async Task<bool> ActualizarMaterialPuesto(MaterialPuesto mat)
        {
            bool reg = false;
            var query = @"update tb_material_puesto set clave_producto = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_material_puesto = @IdMaterialPuesto;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, mat);
                    reg = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public async Task<bool> ActualizarUniformePuesto(MaterialPuesto uni)
        {
            bool reg = false;
            var query = @"update tb_uniforme_puesto set clave = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_uniforme_puesto = @IdMaterialPuesto;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, uni);
                    reg = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public async Task AgregarEquipoPuesto(MaterialPuesto equi)
        {
            var query = @"insert into tb_equipo_puesto(clave, id_puesto, id_frecuencia, cantidad, id_personal, fecha_alta)
                        values(@ClaveProducto, @IdPuesto, @IdFrecuencia, @Cantidad, @IdPersonal, @FechaAlta);";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, equi);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AgregarHerramientaPuesto(MaterialPuesto her)
        {
            var query = @"insert into tb_herramienta_puesto(clave, id_puesto, id_frecuencia, cantidad, id_personal, fecha_alta)
                        values(@ClaveProducto, @IdPuesto, @IdFrecuencia, @Cantidad, @IdPersonal, @FechaAlta);";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, her);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AgregarMaterialPuesto(MaterialPuesto mat)
        {
            var query = @"insert into tb_material_puesto (clave_producto, id_puesto, id_frecuencia, cantidad, id_personal, fecha_alta)
                        values(@ClaveProducto, @IdPuesto, @IdFrecuencia, @Cantidad, @IdPersonal, @FechaAlta);";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, mat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AgregarUniformePuesto(MaterialPuesto uni)
        {
            var query = @"insert into tb_uniforme_puesto (id_puesto, clave, cantidad, id_frecuencia, id_personal, fecha_alta)
                        values(@IdPuesto, @ClaveProducto, @Cantidad, @IdFrecuencia, @IdPersonal, @FechaAlta);";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, uni);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> EliminarEquipoPuesto(int id)
        {
            bool reg = false;
            var query = @"delete from tb_equipo_puesto where id_equipo_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public async Task<bool> EliminarHerramientaPuesto(int id)
        {
            bool reg = false;
            var query = @"delete from tb_herramienta_puesto where id_herramienta_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public async Task<bool> EliminarMaterialPuesto(int id)
        {
            bool reg = false;
            var query = @"delete from tb_material_puesto where id_material_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }

        public async Task<bool> EliminarUniformePuesto(int id)
        {
            bool reg = false;
            var query = @"delete from tb_uniforme_puesto where id_uniforme_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }


        public async Task<bool> EliminarServicio(int id)
        {
            bool reg = false;
            var query = @"DELETE FROM tb_servicioextra WHERE id_servicioextra = @id";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }


        public async Task<bool> AgregarServicio(string servicio, int idPersonal)
        {
            bool reg = false;
            var query = @"INSERT INTO tb_servicioextra
(
descripcion,
fecha_alta,
id_personal
)
VALUES 
(
@servicio,
GETDATE(),
@idPersonal
)";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { servicio, idPersonal });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }



        public async Task<IEnumerable<ProductoItem>> ObtenerEquipoPuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_equipo_puesto IdMaterialPuesto, a.clave ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_equipo_puesto a
                        JOIN tb_producto b on a.clave = b.clave
                        WHERE a.id_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }

        public async Task<IEnumerable<ProductoItem>> ObtenerHerramientaPuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_herramienta_puesto IdMaterialPuesto, a.clave ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_herramienta_puesto a
                        JOIN tb_producto b on a.clave = b.clave
                        WHERE a.id_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }

        public async Task<IEnumerable<ProductoItem>> ObtenerMaterialPuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_material_puesto IdMaterialPuesto, a.clave_producto ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_material_puesto a
                        JOIN tb_producto b on a.clave_producto = b.clave
                        WHERE a.id_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }

        public async Task<IEnumerable<ProductoItem>> ObtenerUniformePuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_uniforme_puesto IdMaterialPuesto, a.clave ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_uniforme_puesto a
                        JOIN tb_producto b on a.clave = b.clave
                        WHERE a.id_puesto = @id;";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
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