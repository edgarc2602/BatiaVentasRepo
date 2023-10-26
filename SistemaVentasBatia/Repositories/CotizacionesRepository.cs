using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Controllers;
using System.Reflection;
using Microsoft.AspNetCore.Connections;
using System.Reflection.Metadata.Ecma335;
using SistemaVentasBatia.DTOs;

namespace SistemaVentasBatia.Repositories
{
    public interface ICotizacionesRepository
    {
        Task InsertaCotizacion(Cotizacion cotizacion);
        Task<int> ContarCotizaciones(int idProspecto, EstatusCotizacion idEstatusCotizacion, int idServicio,int idPersonal, int autorizacion);
        Task<List<Cotizacion>> ObtenerCotizaciones(int pagina, int idProspecto, EstatusCotizacion idEstatusCotizacion, int idServicio, int admin, int idPersonal);
        Task<int> ObtenerAutorizacion(int idPersonal);
        Task<List<Direccion>> ObtenerDireccionesPorCotizacion(int idCotizacion, int pagina);
        Task<List<Direccion>> ObtenerCatalogoDirecciones(int idProspecto);
        Task InsertarDireccionCotizacion(DireccionCotizacion direccionCVM);
        Task<List<PuestoDireccionCotizacion>> ObtienePuestosPorCotizacion(int idCotizacion);
        Task<List<Direccion>> ObtenerCatalogoDireccionesCotizacion(int idCotizacion);
        Task<int> InsertaPuestoDireccionCotizacion(PuestoDireccionCotizacion operario);
        Task<decimal> ObtenerSueldoPorIdTabuladorIdClase(int idPuesto, int idClase, int idZona);
        Task<int> ObtenerIdZona(int idPuestoDireccion);
        Task<ResumenCotizacionLimpieza> ObtenerResumenCotizacionLimpieza(int idCotizacion);
        Task InsertarTotalCotizacion(decimal total, int idCotizacion, string numerotxt);
        Task<Cotizacion> ObtenerNombreComercialCotizacion(int idCotizacion);
        Task<ResumenCotizacionLimpieza> ObtenerResumenCotizacionLimpieza2(int idCotizacion);
        Task<Cotizacion> ObtenerCotizacion(int id);
        Task InactivarCotizacion(int idCotizacion);
        Task<int> ObtenerIdCotizacionPorDireccion(int idDireccionCotizacion);
        Task InactivarDireccionCotizacion(int idDireccionCotizacion);
        Task<int> ObtenerIdDireccionCotizacionPorOperario(int registroAEliminar);
        Task EliminarOperario(int registroAEliminar);
        Task<int> CopiarCotizacion(int idCotizacion);

        Task ActualizarIndirectoUtilidad(int idCotizacion, string indirecto, string utilidad, string comisionSV, string comisionExt);
        Task ActualizarCotizacion(int idCotizacion, int idProspecto, Servicio idServicio);
        Task<bool> ValidarDirecciones(int idCotizacion);
        Task CopiarDirectorioCotizacion(int idCotizacion, int idCotizacionNueva);
        Task<int> CopiarPlantillaDireccionCotizacion(int direccionCotizacion, int direccionCotizacionNueva);
        Task<List<DireccionCotizacion>> ObtieneDireccionesCotizacion(int idCotizacion);
        Task<int> ObtenerIdEstadoDeDireccionCotizacion(int idDireccionCotizacion);
        Task<string> ObtenerDescripcionPuestoPorIdOperario(int id);
        Task<string> ObtenerNombreSucursalPorIdOperario(int id);
        Task ActualizarPuestoDireccionCotizacion(PuestoDireccionCotizacion operario);
        Task<int> ObtieneIdCotizacionPorOperario(int idPuestoDireccionCotizacion);
        Task<int> ObtieneIdDireccionCotizacionPorOperario(int idPuestoDireccionCotizacion);

        Task CopiarMaterialCotizacion(int idCotizacionNueva, int idCotizacion);
        Task CopiarHerramientaCotizacion(int idCotizacionNueva, int idDireccionCotizacionNueva, int idCotizacion);
        Task CopiarEquipoCotizacion(int idCotizacionNueva, int idDireccionCotizacionNueva, int idCotizacion);
        Task CopiarUniformeCotizacion(int idCotizacionNueva, int idDireccionCotizacionNueva, int idCotizacion);

        Task<List<MaterialCotizacion>> ObtieneMaterialesCotizacion(int idCotizacion);
        Task<List<PuestoDireccionCotizacion>> ObtieneOperariosCotizacion(int idCotizacion);

        Task CopiarMaterial(MaterialCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo);
        Task CopiarUniforme(UniformeCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo);
        Task CopiarEquipo(EquipoCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo);
        Task CopiarHerramienta(HerramientaCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo);

        Task<bool> ActualizarSalarios(PuestoTabulador salarios);

        Task<CotizaPorcentajes> ObtenerPorcentajesCotizacion();
        Task ActualizarPorcentajesPredeterminadosCotizacion(CotizaPorcentajes porcentajes);
    }

    public class CotizacionesRepository : ICotizacionesRepository
    {
        private readonly DapperContext ctx;
        public CotizacionesRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task InsertaCotizacion(Cotizacion cotizacion)
        {
            var query = @"declare @idp int = 0, @pci decimal(5, 4), @pu decimal (5, 4), @cv decimal (5,4), @ce decimal (5, 4);

                        select top (1) @idp = id_porcentaje, @pci = costoindirecto, @pu = utilidad, @cv = comision_venta, @ce = comision_externa
                        from tb_cotiza_porcentaje
                        where fechaaplica <= @FechaAlta
                        order by id_porcentaje desc;

                        insert into tb_cotizacion(id_prospecto, id_servicio, costo_indirecto, utilidad, total, id_estatus_cotizacion, fecha_alta, id_personal, id_porcentaje, comision_venta, comision_externa)
                        values(@IdProspecto, @IdServicio, @pci, @pu, @Total, @IdEstatusCotizacion, @FechaAlta, @IdPersonal, @idp, @cv, @ce)
                        select scope_identity()";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    cotizacion.IdCotizacion = await connection.ExecuteScalarAsync<int>(query, cotizacion);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> ContarCotizaciones(int idProspecto, EstatusCotizacion idEstatusCotizacion, int idServicio, int idPersonal, int autorizacion)
        {
            var queryuser = @"SELECT count(*) Rows
                                FROM tb_cotizacion c
                                JOIN tb_prospecto p on c.id_prospecto = p.id_prospecto
                                WHERE 
                                    c.id_personal = @idPersonal AND
                                    ISNULL(NULLIF(@idProspecto,0), c.id_prospecto) = c.id_prospecto AND
                                    ISNULL(NULLIF(@idEstatusCotizacion,0), c.id_estatus_cotizacion) = c.id_estatus_cotizacion AND
                                    ISNULL(NULLIF(@idServicio,0), c.id_servicio) = c.id_servicio";
            var queryadmin = @"SELECT count(*) Rows
                                FROM tb_cotizacion c
                                JOIN tb_prospecto p on c.id_prospecto = p.id_prospecto
                                WHERE 
                                    ISNULL(NULLIF(@idProspecto,0), c.id_prospecto) = c.id_prospecto AND
                                    ISNULL(NULLIF(@idEstatusCotizacion,0), c.id_estatus_cotizacion) = c.id_estatus_cotizacion AND
                                    ISNULL(NULLIF(@idServicio,0), c.id_servicio) = c.id_servicio";

            var rows = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    if(autorizacion == 0)
                    {
                        rows = await connection.QuerySingleAsync<int>(queryuser, new { idProspecto, idEstatusCotizacion, idServicio, idPersonal });
                    }
                    else
                    {
                        rows = await connection.QuerySingleAsync<int>(queryadmin, new { idProspecto, idEstatusCotizacion, idServicio });
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rows;
        }

        public async Task<List<Cotizacion>> ObtenerCotizaciones(int pagina, int idProspecto, EstatusCotizacion idEstatusCotizacion, int idServicio, int admin, int idPersonal)
        {

            var queryadmin = @"SELECT  *
                          FROM (SELECT ROW_NUMBER() OVER ( ORDER BY id_cotizacion desc ) AS RowNum, id_cotizacion IdCotizacion, id_servicio IdServicio, nombre_comercial NombreComercial, 
                                id_estatus_Cotizacion IdEstatusCotizacion, c.fecha_alta FechaAlta, c.id_personal IdPersonal, c.total Total, c.nombre Nombre, per.Per_Nombre + ' ' + per.Per_Paterno AS IdAlta
                                FROM tb_cotizacion c
                                JOIN tb_prospecto p on c.id_prospecto = p.id_prospecto
                                INNER JOIN dbo.Personal per ON c.id_personal = per.IdPersonal 
                                JOIN (SELECT * FROM fn_resumencotizacion(null)) r on c.id_Cotizacion = r.IdCotizacion
                                WHERE 
                                    ISNULL(NULLIF(@idProspecto,0), c.id_prospecto) = c.id_prospecto AND
                                    ISNULL(NULLIF(@idEstatusCotizacion,0), c.id_estatus_cotizacion) = c.id_estatus_cotizacion AND
                                    ISNULL(NULLIF(@idServicio,0), c.id_servicio) = c.id_servicio
                                    

                               ) AS Cotizaciones
                          WHERE   RowNum >= ((@pagina - 1) * 10) + 1
                              AND RowNum <= (@pagina * 10)
                          ORDER BY RowNum";
            var queryuser = @"SELECT  *
                          FROM (SELECT ROW_NUMBER() OVER ( ORDER BY id_cotizacion desc ) AS RowNum, id_cotizacion IdCotizacion, id_servicio IdServicio, nombre_comercial NombreComercial, 
                                id_estatus_Cotizacion IdEstatusCotizacion, c.fecha_alta FechaAlta, c.id_personal IdPersonal, c.total Total, c.nombre Nombre, per.Per_Nombre + ' ' + per.Per_Paterno AS IdAlta
                                FROM tb_cotizacion c
                                JOIN tb_prospecto p on c.id_prospecto = p.id_prospecto
                                INNER JOIN dbo.Personal per ON c.id_personal = per.IdPersonal 
                                JOIN (SELECT * FROM fn_resumencotizacion(null)) r on c.id_Cotizacion = r.IdCotizacion
                                WHERE 
                                    ISNULL(NULLIF(@idProspecto,0), c.id_prospecto) = c.id_prospecto AND
                                    ISNULL(NULLIF(@idEstatusCotizacion,0), c.id_estatus_cotizacion) = c.id_estatus_cotizacion AND
                                    ISNULL(NULLIF(@idServicio,0), c.id_servicio) = c.id_servicio AND
                                    c.id_personal = @idPersonal

                               ) AS Cotizaciones
                          WHERE   RowNum >= ((@pagina - 1) * 10) + 1
                              AND RowNum <= (@pagina * 10)
                          ORDER BY RowNum";


            var cotizaciones = new List<Cotizacion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    if (admin == 1)
                    {
                        cotizaciones = (await connection.QueryAsync<Cotizacion>(queryadmin, new { pagina, idProspecto, idEstatusCotizacion, idServicio })).ToList();
                    }
                    else if (admin == 0)
                    {
                        cotizaciones = (await connection.QueryAsync<Cotizacion>(queryuser, new { pagina, idProspecto, idEstatusCotizacion, idServicio, idPersonal })).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cotizaciones;
        }

        public async Task<int> ObtenerAutorizacion(int idPersonal)
        {
            var query = @"SELECT per_autoriza FROM Autorizacion_ventas WHERE idPersonal = @idPersonal";
            int autorizacion = 0;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    autorizacion = await connection.QueryFirstAsync<int>(query, new { idPersonal });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return autorizacion;
        }

        public async Task<List<Direccion>> ObtenerDireccionesPorCotizacion(int idCotizacion, int pagina)
        {
            var query = @"SELECT  *
                          FROM (SELECT ROW_NUMBER() OVER ( ORDER BY dc.id_direccion_cotizacion desc ) AS RowNum, dc.id_direccion_cotizacion IdDireccionCotizacion, d.id_direccion IdDireccion, nombre_sucursal NombreSucursal, id_tipo_inmueble IdTipoInmueble,
                                    d.id_estado IdEstado, d.id_tabulador IdTabulador, municipio Municipio, ciudad Ciudad, colonia Colonia, domicilio Domicilio, referencia Referencia, codigo_postal CodigoPostal,
                                    contacto Contacto, telefono_contacto TelefonoContacto, id_estatus_direccion IdEstatusDireccion, fecha_alta FechaAlta, e.descripcion Estado, ti.descripcion TipoInmueble
                                FROM tb_direccion d
                                JOIN tb_estado e on d.id_estado = e.id_estado
                                JOIN tb_tipoinmueble ti on d.id_tipo_inmueble = ti.id_tipoinmueble
                                JOIN tb_direccion_cotizacion dc on dc.Id_Direccion = d.Id_Direccion
                                WHERE dc.id_cotizacion = @idCotizacion and id_estatus_direccion = @idEstatusDireccion
                               ) AS Direcciones
                          WHERE   RowNum >= 1 * @pagina
                              AND RowNum < 20 * @pagina
                          ORDER BY RowNum";

            var direcciones = new List<Direccion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Direccion>(query, new { idCotizacion, pagina, idEstatusDireccion = (int)EstatusDireccion.Activo })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }

        public async Task<List<Direccion>> ObtenerCatalogoDirecciones(int idProspecto)
        {
            var query = @"SELECT id_direccion IdDireccion, nombre_sucursal NombreSucursal
                        FROM tb_direccion d
                        WHERE id_prospecto = @idProspecto AND id_estatus_direccion = 1";

            var direcciones = new List<Direccion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Direccion>(query, new { idProspecto })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }

        public async Task InsertarDireccionCotizacion(DireccionCotizacion direccionCVM)
        {
            var query = @"insert into tb_direccion_cotizacion values(@IdDireccion, @IdCotizacion)
                          select scope_identity()";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direccionCVM.IdCotizacion = await connection.ExecuteScalarAsync<int>(query, direccionCVM);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PuestoDireccionCotizacion>> ObtienePuestosPorCotizacion(int idCotizacion)
        {
            var query = @"select pdc.cantidad Cantidad, p.descripcion Puesto, jornada Jornada,j.descripcion JornadaDesc, t.descripcion Turno, hr_inicio HrInicio, hr_fin HrFin, dia_inicio DiaInicio, dia_fin DiaFin,
	                               sueldo Sueldo, dc.id_cotizacion IdCotizacion, pdc.id_direccion_cotizacion IdDireccionCotizacion, id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
                                   aguinaldo Aguinaldo, vacaciones Vacaciones, prima_vacacional PrimaVacacional, isn ISN, imss IMSS, total Total
                            from tb_puesto_direccion_cotizacion pdc
                            join tb_puesto p on p.id_puesto = pdc.id_puesto
                            join tb_turno t on t.id_turno = pdc.id_turno
                            join tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = pdc.id_direccion_cotizacion
							join tb_jornada j on jornada = j.id_jornada
                            where dc.id_cotizacion = @idCotizacion order by pdc.id_puesto_direccioncotizacion desc";

            var puestosDirecciones = new List<PuestoDireccionCotizacion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestosDirecciones = (await connection.QueryAsync<PuestoDireccionCotizacion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return puestosDirecciones;
        }

        public async Task<List<Direccion>> ObtenerCatalogoDireccionesCotizacion(int idCotizacion)
        {
            var query = @"SELECT d.id_direccion IdDireccion, nombre_sucursal NombreSucursal, id_tipo_inmueble IdTipoInmueble, dc.id_direccion_cotizacion IdDireccionCotizacion,
                                    d.id_estado IdEstado, d.id_tabulador IdTabulador, municipio Municipio, ciudad Ciudad, colonia Colonia, domicilio Domicilio, referencia Referencia, codigo_postal CodigoPostal, e.descripcion Estado
                          FROM tb_direccion d
                          RIGHT JOIN tb_direccion_cotizacion dc on dc.id_direccion = d.id_direccion
                          JOIN tb_estado e on d.id_estado = e.id_estado
                          WHERE dc.id_cotizacion = @idCotizacion ORDER BY nombre_sucursal";

            var direcciones = new List<Direccion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Direccion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }

        public async Task<int> InsertaPuestoDireccionCotizacion(PuestoDireccionCotizacion operario)
        {
            int ids = 0;
            string query = @$"INSERT INTO tb_puesto_direccion_cotizacion (id_puesto, id_direccion_cotizacion, jornada, id_turno, id_salario, cantidad, hr_inicio, hr_fin,
                            dia_inicio, dia_fin, fecha_alta, sueldo, aguinaldo, vacaciones, prima_vacacional, isn, imss, total, id_tabulador, id_clase)
                        VALUES(@IdPuesto, @IdDireccionCotizacion, @Jornada, @IdTurno, @IdSalario, @Cantidad, @HrInicio, @HrFin,
                            @DiaInicio, @DiaFin, getdate(), @Sueldo, @Aguinaldo, @Vacaciones, @PrimaVacacional, @ISN, @IMSS, @Total, @IdTabulador, @IdClase);
                        select SCOPE_IDENTITY() as ID;";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ids = await connection.QueryFirstAsync<int>(query, operario);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ids;
        }

        public async Task<ResumenCotizacionLimpieza> ObtenerResumenCotizacionLimpieza(int idCotizacion)
        {
            var query = @"SELECT * FROM fn_resumencotizacion(@idCotizacion)";
            var queryserv = @"SELECT ISNULL(SUM(ISNULL(importemensual,0)),0) AS Servicio
FROM tb_cotiza_servicioextra
WHERE id_cotizacion = @idCotizacion";

            var resumen = new ResumenCotizacionLimpieza();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    resumen = await connection.QueryFirstAsync<ResumenCotizacionLimpieza>(query, new { idCotizacion });

                    resumen.Servicio = await connection.QueryFirstAsync<decimal>(queryserv, new { idCotizacion });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resumen;
        }


        public async Task InsertarTotalCotizacion(decimal total, int idCotizacion, string numerotxt)
        {
            var query = @"UPDATE tb_cotizacion set total = @total, total_letra = @numerotxt where id_cotizacion = @idCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {

                    await connection.ExecuteAsync(query, new { idCotizacion, total, numerotxt });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<Cotizacion> ObtenerNombreComercialCotizacion(int idCotizacion)
        {
            var query = @"SELECT p.nombre_comercial NombreComercial FROM tb_cotizacion c
                          INNER JOIN tb_prospecto p on c.id_prospecto = p.id_prospecto
                          WHERE id_cotizacion = @idCotizacion";
            var cot = new Cotizacion();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    cot = await connection.QueryFirstAsync<Cotizacion>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cot;
        }


        public async Task<ResumenCotizacionLimpieza> ObtenerResumenCotizacionLimpieza2(int idCotizacion)
        {
            var query = @"SELECT * FROM fn_resumencotizacion(@idCotizacion)";

            var resumen = new ResumenCotizacionLimpieza();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    resumen = await connection.QueryFirstAsync<ResumenCotizacionLimpieza>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resumen;
        }



        public async Task<Cotizacion> ObtenerCotizacion(int id)
        {
            var cotizacion = new Cotizacion();

            var query = @"SELECT id_cotizacion IdCotizacion,
costo_indirecto CostoIndirecto,
utilidad Utilidad,
comision_venta ComisionSV,
comision_externa ComisionExt
FROM tb_cotizacion  WHERE id_cotizacion = @id ";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    cotizacion = await connection.QueryFirstAsync<Cotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cotizacion;
        }


















        public async Task InactivarCotizacion(int idCotizacion)
        {
            var query = @"UPDATE tb_cotizacion set id_estatus_cotizacion = @idEstatusCotizacion where id_cotizacion = @idCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idEstatusCotizacion = EstatusCotizacion.Inactivo, idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> ObtenerIdCotizacionPorDireccion(int idDireccionCotizacion)
        {
            var query = @"SELECT id_cotizacion idCotizacion FROM tb_direccion_cotizacion where id_direccion_cotizacion = @idDireccionCotizacion";

            var idCotizacion = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idCotizacion = await connection.QuerySingleAsync<int>(query, new { idDireccionCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idCotizacion;
        }

        public async Task InactivarDireccionCotizacion(int idDireccionCotizacion)
        {
            var query = @"
DELETE FROM tb_direccion_cotizacion WHERE id_direccion_cotizacion = @idDireccionCotizacion
DELETE FROM tb_puesto_direccion_cotizacion WHERE id_direccion_cotizacion = @idDireccionCotizacion
DELETE FROM tb_cotiza_material WHERE id_direccion_cotizacion = @idDireccionCotizacion
DELETE FROM tb_cotiza_uniforme WHERE id_direccion_cotizacion = @idDireccionCotizacion
DELETE FROM tb_cotiza_equipo WHERE id_direccion_cotizacion = @idDireccionCotizacion
DELETE FROM tb_cotiza_herramienta WHERE id_direccion_cotizacion = @idDireccionCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idDireccionCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> ObtenerIdDireccionCotizacionPorOperario(int registroAEliminar)
        {
            var query = @"SELECT id_direccion_cotizacion idDireccionCotizacion FROM tb_puesto_direccion_cotizacion where id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion";

            var idCotizacion = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idCotizacion = await connection.QuerySingleAsync<int>(query, new { idPuestoDireccionCotizacion = registroAEliminar });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idCotizacion;
        }

        public async Task EliminarOperario(int registroAEliminar)
        {
            var query = @"DELETE FROM tb_puesto_direccion_cotizacion where id_puesto_direccioncotizacion = @registroAEliminar
DELETE FROM tb_cotiza_material WHERE id_puesto_direccioncotizacion = @registroAEliminar
DELETE FROM tb_cotiza_equipo WHERE id_puesto_direccioncotizacion = @registroAEliminar
DELETE FROM tb_cotiza_uniforme WHERE id_puesto_direccioncotizacion = @registroAEliminar
DELETE FROM tb_cotiza_herramienta WHERE id_puesto_direccioncotizacion = @registroAEliminar";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { registroAEliminar });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CopiarCotizacion(int idCotizacion)
        {
            var query = @"INSERT INTO tb_cotizacion(id_prospecto, id_servicio, costo_indirecto,utilidad,total, id_estatus_cotizacion, fecha_alta, id_personal, id_cotizacion_original, id_porcentaje,comision_venta, comision_externa)
                          SELECT  id_prospecto, id_servicio,costo_indirecto,utilidad, total, id_estatus_cotizacion, getdate(), id_personal, id_cotizacion, id_porcentaje, comision_venta, comision_externa
                          FROM tb_cotizacion
                          WHERE id_cotizacion = @idCotizacion;
                        
                          select scope_identity()";

            var idCotizacionNueva = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idCotizacionNueva = await connection.ExecuteScalarAsync<int>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idCotizacionNueva;
        }

        public async Task ActualizarIndirectoUtilidad(int idCotizacion, string indirecto, string utilidad, string comisionSV, string comisionExt)
        {
            decimal indirectoval = decimal.Parse(indirecto);
            decimal utilidadval = decimal.Parse(utilidad);
            decimal comisionSVval = decimal.Parse(comisionSV);
            decimal comisionExtval = decimal.Parse(comisionExt);

            string basemenor = ".0";
            string basemayor = ".";

            string indirectofin = "";
            string utilidadfin = "";
            string comisionSVfin = "";
            string comisionExtfin = "";

            if (indirectoval < 10)
            {
                indirectofin = basemenor + indirecto;
            }
            else
            {
                indirectofin = basemayor + indirecto;
            }
            if (utilidadval < 10)
            {
                utilidadfin = basemenor + utilidad;
            }
            else
            {
                utilidadfin = basemayor + utilidad;
            }

            if (comisionSVval < 10)
            {
                comisionSVfin = basemenor + comisionSV;
            }
            else
            {
                comisionSVfin = basemayor + comisionSV;
            }
            if (comisionExtval < 10)
            {
                comisionExtfin = basemenor + comisionExt;
            }
            else
            {
                comisionExtfin = basemayor + comisionExt;
            }

            decimal indirectodec = decimal.Parse(indirectofin);
            decimal utilidaddec = decimal.Parse(utilidadfin);
            decimal comisionSVdec = decimal.Parse(comisionSVfin);
            decimal comisionExtdec = decimal.Parse(comisionExtfin);


            var query = @"UPDATE tb_cotizacion set 
costo_indirecto = @indirectodec, 
utilidad = @utilidaddec,
comision_venta = @comisionSVdec,
comision_externa = @comisionExtdec
where id_cotizacion = @idCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idCotizacion, indirectodec, utilidaddec, comisionSVdec, comisionExtdec });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ActualizarCotizacion(int idCotizacion, int idProspecto, Servicio idServicio)
        {
            var query = @"UPDATE tb_cotizacion set id_prospecto = @idProspecto, id_servicio = @idServicio where id_cotizacion = @idCotizacion";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idCotizacion, idProspecto, idServicio });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ValidarDirecciones(int idCotizacion)
        {
            var query = @"SELECT COUNT(*) FROM tb_direccion_cotizacion WHERE id_cotizacion = @idCotizacion";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var rowCount = await connection.ExecuteScalarAsync<int>(query, new { idCotizacion });

                    if (rowCount == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public async Task CopiarDirectorioCotizacion(int idCotizacion, int idCotizacionNueva)
        {
            var query = @"INSERT INTO tb_direccion_cotizacion(id_direccion, id_cotizacion)
                          SELECT  id_direccion, @idCotizacionNueva
                          FROM tb_direccion_cotizacion
                          WHERE id_cotizacion = @idCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idCotizacion, idCotizacionNueva });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<int> CopiarPlantillaDireccionCotizacion(int idDireccionCotizacion, int idDireccionCotizacionNueva)
        {
            var query = @"INSERT INTO tb_puesto_direccion_cotizacion(
id_puesto, 
id_direccion_cotizacion,
jornada, 
id_turno, 
id_salario,
cantidad, 
hr_inicio,
hr_fin, 
dia_inicio, 
dia_fin, 
fecha_alta, 
sueldo,
aguinaldo,
vacaciones,
prima_vacacional,
isn,
imss,
total

)
SELECT  
id_puesto,
@idDireccionCotizacionNueva,
jornada, 
id_turno, 
id_salario,
cantidad, 
hr_inicio,
hr_fin, 
dia_inicio, 
dia_fin, 
fecha_alta, 
sueldo,
aguinaldo,
vacaciones,
prima_vacacional,
isn,
imss,
total
FROM tb_puesto_direccion_cotizacion
WHERE id_direccion_cotizacion = @idDireccionCotizacion;
select scope_identity()";
            var idPuestoDireccionCotizacionNuevo = 0;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    //idPuestoDireccionCotizacionNuevo = await connection.ExecuteAsync(query, new { idDireccionCotizacion, idDireccionCotizacionNueva });
                    idPuestoDireccionCotizacionNuevo = await connection.ExecuteScalarAsync<int>(query, new { idDireccionCotizacion, idDireccionCotizacionNueva });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idPuestoDireccionCotizacionNuevo;
        }




        public async Task CopiarMaterial(MaterialCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo)
        {
            var query = @"INSERT INTO tb_cotiza_material(
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal)
VALUES(
@clave_producto,
@id_cotizacion,
@id_direccion_cotizacion,
@id_puesto_direccioncotizacion,
@precio_unitario,
@cantidad,
@total,
@importemensual,
@id_frecuencia,
@fecha_alta,
@id_personal
)";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var res = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        clave_producto = producto.ClaveProducto,
                        id_cotizacion = idCotizacionNueva,
                        id_direccion_cotizacion = idDireccionCotizacion,
                        id_puesto_direccioncotizacion = idPuestoDireccionCotizacionNuevo,
                        precio_unitario = producto.PrecioUnitario,
                        cantidad = producto.Cantidad,
                        total = producto.Total,
                        importemensual = producto.ImporteMensual,
                        id_frecuencia = producto.IdFrecuencia,
                        fecha_alta = producto.FechaAlta,
                        id_personal = producto.IdPersonal,
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CopiarUniforme(UniformeCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo)
        {
            var query = @"INSERT INTO tb_cotiza_uniforme(
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal)
VALUES(
@clave_producto,
@id_cotizacion,
@id_direccion_cotizacion,
@id_puesto_direccioncotizacion,
@precio_unitario,
@cantidad,
@total,
@importemensual,
@id_frecuencia,
@fecha_alta,
@id_personal
)";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var res = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        clave_producto = producto.ClaveProducto,
                        id_cotizacion = idCotizacionNueva,
                        id_direccion_cotizacion = idDireccionCotizacion,
                        id_puesto_direccioncotizacion = idPuestoDireccionCotizacionNuevo,
                        precio_unitario = producto.PrecioUnitario,
                        cantidad = producto.Cantidad,
                        total = producto.Total,
                        importemensual = producto.ImporteMensual,
                        id_frecuencia = producto.IdFrecuencia,
                        fecha_alta = producto.FechaAlta,
                        id_personal = producto.IdPersonal,
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CopiarEquipo(EquipoCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo)
        {
            var query = @"INSERT INTO tb_cotiza_equipo(
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal)
VALUES(
@clave_producto,
@id_cotizacion,
@id_direccion_cotizacion,
@id_puesto_direccioncotizacion,
@precio_unitario,
@cantidad,
@total,
@importemensual,
@id_frecuencia,
@fecha_alta,
@id_personal
)";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var res = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        clave_producto = producto.ClaveProducto,
                        id_cotizacion = idCotizacionNueva,
                        id_direccion_cotizacion = idDireccionCotizacion,
                        id_puesto_direccioncotizacion = idPuestoDireccionCotizacionNuevo,
                        precio_unitario = producto.PrecioUnitario,
                        cantidad = producto.Cantidad,
                        total = producto.Total,
                        importemensual = producto.ImporteMensual,
                        id_frecuencia = producto.IdFrecuencia,
                        fecha_alta = producto.FechaAlta,
                        id_personal = producto.IdPersonal,
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CopiarHerramienta(HerramientaCotizacion producto, int idCotizacionNueva, int idDireccionCotizacion, int idPuestoDireccionCotizacionNuevo)
        {
            var query = @"INSERT INTO tb_cotiza_herramienta(
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal)
VALUES(
@clave_producto,
@id_cotizacion,
@id_direccion_cotizacion,
@id_puesto_direccioncotizacion,
@precio_unitario,
@cantidad,
@total,
@importemensual,
@id_frecuencia,
@fecha_alta,
@id_personal
)";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var res = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        clave_producto = producto.ClaveProducto,
                        id_cotizacion = idCotizacionNueva,
                        id_direccion_cotizacion = idDireccionCotizacion,
                        id_puesto_direccioncotizacion = idPuestoDireccionCotizacionNuevo,
                        precio_unitario = producto.PrecioUnitario,
                        cantidad = producto.Cantidad,
                        total = producto.Total,
                        importemensual = producto.ImporteMensual,
                        id_frecuencia = producto.IdFrecuencia,
                        fecha_alta = producto.FechaAlta,
                        id_personal = producto.IdPersonal,
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }








        public async Task<List<DireccionCotizacion>> ObtieneDireccionesCotizacion(int idCotizacion)
        {
            var query = @"SELECT id_direccion IdDireccion, id_direccion_cotizacion IdDireccionCotizacion
                          FROM  tb_direccion_cotizacion 
                          WHERE id_cotizacion = @idCotizacion
                          ORDER BY id_direccion_cotizacion";

            var direcciones = new List<DireccionCotizacion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<DireccionCotizacion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }

        public async Task<int> ObtenerIdEstadoDeDireccionCotizacion(int idDireccionCotizacion)
        {
            var query = $@"SELECT d.id_estado IdEstado 
                            FROM tb_direccion_cotizacion dc
                            JOIN tb_direccion d ON d.id_direccion = dc.id_direccion
                            WHERE dc.id_direccion_cotizacion = @idDireccionCotizacion";

            var idEstado = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idEstado = await connection.QuerySingleAsync<int>(query, new { idDireccionCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idEstado;
        }

        public async Task<string> ObtenerDescripcionPuestoPorIdOperario(int id)
        {
            var query = $@"SELECT p.descripcion DescripcionPuesto
                            FROM tb_puesto_direccion_cotizacion pdc
                            JOIN tb_puesto p on p.id_puesto = pdc.id_puesto
                            WHERE id_puesto_direccioncotizacion = @id";

            var descripcionPuesto = "";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    descripcionPuesto = await connection.QueryFirstAsync<string>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return descripcionPuesto;
        }

        public async Task<string> ObtenerNombreSucursalPorIdOperario(int id)
        {
            var query = $@"SELECT d.nombre_sucursal NombreSucursal
                            FROM tb_puesto_direccion_cotizacion pdc
                            JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = pdc.id_direccion_cotizacion
                            JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                            WHERE id_puesto_direccioncotizacion = @id";

            var nombreSucursal = "";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    nombreSucursal = await connection.QueryFirstAsync<string>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return nombreSucursal;
        }

        public async Task ActualizarPuestoDireccionCotizacion(PuestoDireccionCotizacion operario)
        {
            var query = @"UPDATE tb_puesto_direccion_cotizacion 
                        SET id_puesto = @IdPuesto, jornada = @Jornada, id_turno = @IdTurno, cantidad = @Cantidad, hr_inicio = @HrInicio, hr_fin = @HrFin, 
                            dia_inicio = @DiaInicio, dia_fin = @DiaFin, sueldo = @Sueldo, aguinaldo = @Aguinaldo,
                            vacaciones = @Vacaciones, prima_vacacional = @PrimaVacacional, isn= @ISN, imss = @IMSS, total = @Total, id_tabulador = @IdTabulador, id_clase = @IdClase
                        WHERE id_puesto_direccioncotizacion = @IdPuestoDireccionCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, operario);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> ObtieneIdCotizacionPorOperario(int idPuestoDireccionCotizacion)
        {
            var IdCotizacion = 0;

            var query = @"SELECT dc.id_cotizacion IdCotizacion 
                          FROM tb_puesto_direccion_cotizacion pdc
                          JOIN tb_direccion_cotizacion dc ON dc.id_direccion_cotizacion = pdc.id_direccion_cotizacion
                          WHERE pdc.id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    IdCotizacion = await connection.ExecuteScalarAsync<int>(query, new { idPuestoDireccionCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return IdCotizacion;
        }

        public async Task<int> ObtieneIdDireccionCotizacionPorOperario(int idPuestoDireccionCotizacion)
        {
            var idDireccionCotizacion = 0;

            var query = @"SELECT id_direccion_cotizacion IdDireccionCotizacion 
                          FROM tb_puesto_direccion_cotizacion
                          WHERE id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idDireccionCotizacion = await connection.ExecuteScalarAsync<int>(query, new { idPuestoDireccionCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idDireccionCotizacion;
        }



        public async Task CopiarMaterialCotizacion(int idCotizacionNueva, int idCotizacion)
        {
            var query = @"
INSERT INTO tb_cotiza_material(
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
)
SELECT
clave_producto,
@idCotizacionNueva,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
FROM tb_cotiza_material
WHERE id_cotizacion = @idCotizacion";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idCotizacionNueva, idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MaterialCotizacion>> ObtieneMaterialesCotizacion(int idCotizacion)
        {
            var query = @"SELECT id_material_cotizacion id_cotizacion, id_direccion_cotizacion id_puesto_direccioncotizacion
                          FROM  tb_cotiza_material 
                          WHERE id_cotizacion = @idCotizacion ";

            var direcciones = new List<MaterialCotizacion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<MaterialCotizacion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }


        public async Task<List<PuestoDireccionCotizacion>> ObtieneOperariosCotizacion(int idCotizacion)
        {
            var query = @"
SELECT 
	pdc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
	pdc.id_puesto IdPuesto,
	pdc.id_direccion_cotizacion IdDireccionCotizacion,
	dc.id_direccion IdDireccion,
	dc.id_cotizacion IdCotizacion,
	pdc.jornada Jornada,
	pdc.id_turno IdTurno,
	pdc.id_salario IdSalario,
	pdc.cantidad Cantidad,
	pdc.hr_inicio HrInicio,
	pdc.hr_fin HrFin,
	pdc.dia_inicio DiaInicio,
	pdc.dia_fin DiaFin,
	pdc.fecha_alta FechaAlta,
	pdc.sueldo Sueldo,
	pdc.aguinaldo Aguinaldo,
	pdc.vacaciones Vacaciones,
	pdc.prima_vacacional PrimaVacacional,
	pdc.isn Isn,
	pdc.imss Imss,
	pdc.total Total
	FROM tb_direccion_cotizacion dc
	INNER JOIN tb_puesto_direccion_cotizacion pdc ON pdc.id_direccion_cotizacion = dc.id_direccion_cotizacion
	WHERE dc.id_cotizacion = @idCotizacion
    ORDER By FechaAlta
";
            var operarios = new List<PuestoDireccionCotizacion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    operarios = (await connection.QueryAsync<PuestoDireccionCotizacion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return operarios;
        }







        public async Task CopiarEquipoCotizacion(int idCotizacionNueva, int idDireccionCotizacionNueva, int idCotizacion)
        {
            var query = @"
INSERT INTO tb_cotiza_equipo(
id_equipo_cotizacion,
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
)
SELECT
id_material_cotizacion,
clave_producto,
@idCotizacionNueva,
@idDireccionCotizacionNueva,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
FROM tb_cotiza_material
WHERE id_cotizacion = @idCotizacion";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idCotizacionNueva, idDireccionCotizacionNueva, idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CopiarHerramientaCotizacion(int idCotizacionNueva, int idDireccionCotizacionNueva, int idCotizacion)
        {
            var query = @"
INSERT INTO tb_cotiza_herramienta(
id_herramienta_cotizacion,
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
)
SELECT
id_material_cotizacion,
clave_producto,
@idCotizacionNueva,
@idDireccionCotizacionNueva,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
FROM tb_cotiza_material
WHERE id_cotizacion = @idCotizacion";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idCotizacionNueva, idDireccionCotizacionNueva, idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CopiarUniformeCotizacion(int idCotizacionNueva, int idDireccionCotizacionNueva, int idCotizacion)
        {
            var query = @"
INSERT INTO tb_cotiza_uniforme(
id_uniforme_cotizacion,
clave_producto,
id_cotizacion,
id_direccion_cotizacion,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
)
SELECT
id_material_cotizacion,
clave_producto,
@idCotizacionNueva,
@idDireccionCotizacionNueva,
id_puesto_direccioncotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
FROM tb_cotiza_material
WHERE id_cotizacion = @idCotizacion";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idCotizacionNueva, idDireccionCotizacionNueva, idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> ActualizarSalarios(PuestoTabulador salarios)
        {
            var query = @"UPDATE tb_puesto_salario
                        SET 
salariomixto = @SalarioMixto, 
salariomixto_frontera = @SalarioMixtoFrontera,
salarioreal = @SalarioReal, 
salarioreal_frontera = @SalarioRealFrontera
                        WHERE id_puesto = @IdPuesto";
            try
            {
                using (var connectiom = ctx.CreateConnection())
                {
                    await connectiom.ExecuteAsync(query, salarios);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<CotizaPorcentajes> ObtenerPorcentajesCotizacion()
        {
            var query = @"SELECT 
cp.id_personal IdPersonal,
cp.fechaaplica FechaAplica,
p.Per_Nombre +' '+ p.Per_Paterno +' '+ p.Per_Materno Personal,
cp.costoindirecto CostoIndirecto,
cp.utilidad Utilidad,
cp.comision_venta ComisionSobreVenta,
cp.comision_externa ComisionExterna,
cp.fechaalta FechaAlta
FROM tb_cotiza_porcentaje cp
INNER JOIN Personal p on cp.id_personal =  p.IdPersonal
ORDER BY id_porcentaje desc";
            CotizaPorcentajes porcentajes = new CotizaPorcentajes();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    porcentajes = await connection.QueryFirstAsync<CotizaPorcentajes>(query);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return porcentajes;
        }

        public async Task ActualizarPorcentajesPredeterminadosCotizacion(CotizaPorcentajes porcentajes)
        {
            var query = @"INSERT INTO tb_cotiza_porcentaje
(
costoindirecto,
utilidad,
comision_venta,
comision_externa,
fechaaplica,
fechaalta,
id_personal,
activo
)
VALUES
(
@CostoIndirecto,
@Utilidad,
@ComisionSobreVenta,
@ComisionExterna,
@FechaAplica,
GetDate(),
@IdPersonal,
1
)";
            try
            {
                using (var connecion = ctx.CreateConnection())
                {
                    await connecion.ExecuteAsync(query, porcentajes);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<decimal> ObtenerSueldoPorIdTabuladorIdClase(int idPuesto, int idClase, int idZona)
        {
            var query = @"
SELECT ISNULL(sueldo,0) Sueldo
FROM tb_sueldozonaclase
WHERE id_puesto = @idPuesto AND id_clase = @idClase AND id_zona = @idZona
";
            decimal result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    result = await connection.QueryFirstOrDefaultAsync<decimal>(query, new {idPuesto, idClase, idZona});
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task <int> ObtenerIdZona(int idPuestoDireccion)
        {
            var query = @"
SELECT e.id_zona FROM tb_direccion_cotizacion dc
INNER JOIN tb_direccion d ON d.id_direccion = dc.id_direccion
INNER JOIN tb_estado e ON e.id_estado = d.id_estado
WHERE dc.id_direccion_cotizacion = @idPuestoDireccion
";
            int result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    result = await connection.QueryFirstAsync<int>(query, new { idPuestoDireccion });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}