using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SistemaVentasBatia.Repositories
{
    public interface IServicioRepository
    {
        //Task InsertarServicioesCotizacion(List<ServicioCotizacion> Servicio);
        //Task InsertarEquipoCotizacion(List<ServicioCotizacion> Servicio);




        Task<List<ServicioCotizacion>> ObtenerListaServiciosCotizacion(int idCotizacion, int direccioncotizacion);

    }

    public class ServicioRepository : IServicioRepository
    {
        private readonly DapperContext _ctx;

        public ServicioRepository(DapperContext context)
        {
            _ctx = context;
        }

        //public async Task InsertarServicioesCotizacion(List<ServicioCotizacion> ServicioCotizacion)
        //{
        //    var query = "insert into tb_cotiza_Servicio (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia) values";
        //    var row = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}),";

        //    foreach (var Servicio in ServicioCotizacion)
        //    {
        //        query += string.Format(row, Servicio.ClaveProducto, Servicio.IdCotizacion, Servicio.IdDireccionCotizacion, Servicio.IdPuestoDireccionCotizacion,
        //            Servicio.PrecioUnitario, Servicio.Cantidad, Servicio.Total, Servicio.ImporteMensual, DateTime.Now.ToString("yyyy/MM/dd"), Servicio.IdPersonal, (int)Servicio.IdFrecuencia);
        //    }

        //    query = query.Remove(query.Length - 1);

        //    try
        //    {
        //        using (var connection = _ctx.CreateConnection())
        //        {
        //            await connection.ExecuteAsync(query);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        
        public async Task<List<ServicioCotizacion>> ObtenerListaServiciosCotizacion(int idCotizacion, int direccioncotizacion)
        {
            var servicioscotizacion = new List<ServicioCotizacion>();
            var query = @"SELECT 
    cse.id_servicioextra_cotizacion IdServicioExtraCotizacion,
    cse.id_servicioextra IdServicioExtra,
    se.descripcion ServicioExtra,
    cse.id_cotizacion IdCotizacion,
    ISNULL(cse.id_direccion_cotizacion,0) IdDireccionCotizacion,
    ISNULL(d.nombre_sucursal,'General') Direccion,
    cse.precio_unitario PrecioUnitario,
    cse.cantidad Cantidad,
    cse.total Total,
    cse.importemensual ImporteMensual,
    cse.id_frecuencia IdFrecuencia,
    cse.fecha_alta FechaAlta,
    cse.id_personal IdPersonal
FROM tb_cotiza_servicioextra cse
INNER JOIN tb_servicioextra se ON cse.id_servicioextra = se.id_servicioextra
LEFT JOIN tb_direccion_cotizacion dc ON dc.id_direccion_cotizacion = cse.id_direccion_cotizacion
LEFT JOIN tb_direccion d ON d.id_direccion = dc.id_direccion
WHERE cse.id_cotizacion = @idCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    servicioscotizacion = (await connection.QueryAsync<ServicioCotizacion>(query, new{ idCotizacion})).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return servicioscotizacion;
        }

      //  public async Task<List<MaterialCotizacion>> ObtenerUniformeCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina)
      //  {
      //      var query = @"select mc.id_uniforme_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
      //                      mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
      //                      mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
      //                      d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
      //                  FROM tb_cotiza_uniforme mc
      //                  JOIN tb_producto p on p.clave = mc.clave_producto
      //                  JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
      //                  JOIN tb_direccion d on d.id_direccion = dc.id_direccion
      //                  left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
      //                  left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
      //                  WHERE mc.id_cotizacion = @idCotizacion AND
						//ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
						//mc.id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion AND
						//p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%'
      //                  order by mc.id_uniforme_cotizacion
      //                  OFFSET ((@pagina - 1) * 10) rows
      //                  fetch next 10 rows only;";

      //      var materialesCotizacion = new List<MaterialCotizacion>();

      //      try
      //      {
      //          using (var connection = _ctx.CreateConnection())
      //          {
      //              materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
      //              {
      //                  idCotizacion,
      //                  pagina,
      //                  idDireccionCotizacion,
      //                  idPuestoDireccionCotizacion,
      //                  keywords
      //              })).ToList();
      //          }
      //      }
      //      catch (Exception ex)
      //      {
      //          throw ex;
      //      }

      //      return materialesCotizacion;
      //  }









































        //public async Task<List<ProductoPrecio>> ObtenerPreciosProductosPorEstado(string listaClaves, int idEstado)
        //{
        //    var query = $@"SELECT pp.clave Clave, pp.id_proveedor IdProveedor, pp.precio Precio
        //                    FROM tb_productoprecio pp
        //                    JOIN tb_proveedor p ON pp.id_proveedor = p.id_proveedor
        //                    JOIN tb_proveedor_estado pe ON pe.id_estado = p.id_estado AND pe.id_proveedor = p.id_proveedor
        //                    WHERE pp.clave IN ({listaClaves}) AND pe.id_estado = {idEstado}";

        //    var preciosProductos = new List<ProductoPrecio>();

        //    try
        //    {
        //        using (var connection = _ctx.CreateConnection())
        //        {
        //            preciosProductos = (await connection.QueryAsync<ProductoPrecio>(query)).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return preciosProductos;
        //}


        //public async Task InsertarEquipoCotizacion(List<ServicioCotizacion> ServicioList)
        //{
        //    var query = "insert into tb_cotiza_equipo (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia) values";
        //    var row = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}),";

        //    foreach (var Servicio in ServicioList)
        //    {
        //        query += string.Format(row, Servicio.ClaveProducto, Servicio.IdCotizacion, Servicio.IdDireccionCotizacion, Servicio.IdPuestoDireccionCotizacion,
        //            Servicio.PrecioUnitario, Servicio.Cantidad, Servicio.Total, Servicio.ImporteMensual, DateTime.Now.ToString("yyyy/MM/dd"), Servicio.IdPersonal, (int)Servicio.IdFrecuencia);
        //    }

        //    query = query.Remove(query.Length - 1);

        //    try
        //    {
        //        using (var connection = _ctx.CreateConnection())
        //        {
        //            await connection.ExecuteAsync(query);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}
