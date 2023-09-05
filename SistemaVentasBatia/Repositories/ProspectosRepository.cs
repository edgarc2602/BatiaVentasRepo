﻿using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Repositories
{
    public interface IProspectosRepository
    {
        Task InsertarProspecto(Prospecto prospecto);
        Task<List<Prospecto>> ObtenerProspectos(int pagina, EstatusProspecto idEstatusProspecto, string keywords);
        Task<Prospecto> ObtenerProspectoPorId(int idProspecto);
        Task ActualizarProspecto(Prospecto prospecto);
        Task<List<Direccion>> ObtenerDireccionesPorProspecto(int idProspecto, int pagina);
        Task InsertarDireccion(Direccion direccion);
        Task<List<Prospecto>> ObtenerCatalogoProspectos();
        Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion);
        Task<Prospecto> ObtenerProspectoPorCotizacion(int idCotizacion);
        Task InactivarProspecto(int registroAEliminar);
        Task<int> ContarProspectos(EstatusProspecto idEstatusProspecto, string keywords);
        Task<List<Prospecto>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc);
        Task<Direccion> ObtenerDireccionPorId(int id);
        Task ActualizarDireccion(Direccion direccion);
        Task<PuestoDireccionCotizacion> ObtenerPuestoDireccionCotizacionPorId(int id);
    }

    public class ProspectosRepository : IProspectosRepository
    {
        private readonly DapperContext ctx;

        public ProspectosRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task InsertarProspecto(Prospecto prospecto)
        {
            var query = @"insert into tb_prospecto (nombre_comercial, razon_social, rfc, domicilio_fiscal, telefono, representante_legal, documentacion,
                            id_estatus_prospecto, fecha_alta, id_personal, nombre_contacto, email_contacto, numero_contacto, ext_contacto)
                        values(@NombreComercial, @RazonSocial, @Rfc, @DomicilioFiscal, @Telefono,
                            @RepresentanteLegal, @Documentacion, @IdEstatusProspecto, @FechaAlta,
                            @IdPersonal, @NombreContacto, @EmailContacto, @NumeroContacto, @ExtContacto)
                          select scope_identity()";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospecto.IdProspecto = await connection.ExecuteScalarAsync<int>(query, prospecto);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> ContarProspectos(EstatusProspecto idEstatusProspecto, string keywords)
        {
            var query = @"SELECT count(id_prospecto) Rows 
                        FROM tb_prospecto
                        WHERE
                            ISNULL(NULLIF(@idEstatusProspecto,0), id_estatus_prospecto) = id_estatus_prospecto
                            AND nombre_comercial like '%' + @keywords + '%';";

            var numrows = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    numrows = await connection.QuerySingleAsync<int>(query, new { idEstatusProspecto, keywords = keywords ?? "" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return numrows;
        }

        public async Task<List<Prospecto>> ObtenerProspectos(int pagina, EstatusProspecto idEstatusProspecto, string keywords)
        {        
            var query = @"SELECT ROW_NUMBER() OVER ( ORDER BY id_prospecto desc ) AS RowNum, id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                domicilio_fiscal DomicilioFiscal, telefono Telefono, representante_legal RepresentanteLegal , documentacion Documentacion, 
				                id_estatus_prospecto IdEstatusProspecto, fecha_alta FechaAlta, id_personal IdPersonal
                        FROM tb_prospecto
                        WHERE
                            ISNULL(NULLIF(@idEstatusProspecto,0), id_estatus_prospecto) = id_estatus_prospecto AND
                            nombre_comercial like '%' + @keywords + '%'
                        ORDER BY id_prospecto
                        OFFSET ((@pagina - 1) * 10) ROWS
                        FETCH NEXT 10 ROWS ONLY;";

            var prospectos = new List<Prospecto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospectos = (await connection.QueryAsync<Prospecto>(query, new { pagina, idEstatusProspecto, keywords = keywords ?? "" })).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return prospectos;
        }

        public async Task<Prospecto> ObtenerProspectoPorId(int idProspecto)
        {
            var query = @"SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                           domicilio_fiscal DomicilioFiscal, telefono Telefono, representante_legal RepresentanteLegal , documentacion Documentacion, 
				                           id_estatus_prospecto IdEstatusProspecto, fecha_alta FechaAlta, id_personal IdPersonal, 
                                           nombre_contacto NombreContacto, numero_contacto NumeroContacto, ext_contacto ExtContacto, email_contacto EmailContacto
                          FROM tb_prospecto
                          WHERE id_prospecto = @idProspecto";

            var prospecto = new Prospecto();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospecto = await connection.QueryFirstAsync<Prospecto>(query, new { idProspecto });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prospecto;
        }

        public async Task ActualizarProspecto(Prospecto prospecto)
        {
            var query = @"update tb_prospecto 
	                      set nombre_comercial = @NombreComercial, razon_social = @RazonSocial, rfc = @Rfc, domicilio_fiscal = @DomicilioFiscal, /*telefono = @Telefono, */
		                      /*representante_legal = @RepresentanteLegal,*/ documentacion = @Documentacion, nombre_contacto = @NombreContacto, numero_contacto = @NumeroContacto,
                              ext_contacto = @ExtContacto, email_contacto = @EmailContacto
	                      where id_prospecto = @IdProspecto";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, prospecto);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Direccion>> ObtenerDireccionesPorProspecto(int idProspecto, int pagina)
        {
            var query = @"SELECT ROW_NUMBER() OVER ( ORDER BY id_direccion desc ) AS RowNum, id_direccion IdDireccion, nombre_sucursal NombreSucursal, id_tipo_inmueble IdTipoInmueble,
                                d.id_estado IdEstado, d.id_tabulador as IdTabulador, municipio Municipio, ciudad Ciudad, colonia Colonia, domicilio Domicilio, referencia Referencia, codigo_postal CodigoPostal,
                                contacto Contacto, telefono_contacto TelefonoContacto, id_estatus_direccion IdEstatusDireccion, fecha_alta FechaAlta, e.descripcion Estado, ti.descripcion TipoInmueble
                        FROM tb_direccion d
                        JOIN tb_estado e on d.id_estado = e.id_estado
                        JOIN tb_tipoinmueble ti on d.id_tipo_inmueble = ti.id_tipoinmueble
                        WHERE id_prospecto = @idProspecto and id_estatus_direccion = @idEstatusDireccion
                        ORDER BY id_direccion
                        OFFSET ((@pagina - 1) * 10) ROWS
                        FETCH NEXT 10 ROWS ONLY;";

            var direcciones = new List<Direccion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Direccion>(query, new { idProspecto, pagina, idEstatusDireccion = (int)EstatusDireccion.Activo })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }

        public async Task InsertarDireccion(Direccion direccion)
        {
            var query = @"insert into tb_direccion (id_prospecto, nombre_sucursal, id_tipo_inmueble, id_estado, id_tabulador, municipio, ciudad, colonia,
                            domicilio, referencia, codigo_postal, contacto, telefono_contacto, id_estatus_direccion, fecha_alta)
                        values(@IdProspecto, @NombreSucursal, @IdTipoInmueble, @IdEstado, @IdTabulador, @Municipio, @Ciudad, @Colonia, 
                            @Domicilio, @Referencia, @CodigoPostal, @Contacto, @TelefonoContacto, @IdEstatusDireccion, @FechaAlta)
                        select scope_identity()";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direccion.IdDireccion = await connection.ExecuteScalarAsync<int>(query, direccion);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Prospecto>> ObtenerCatalogoProspectos()
        {
            var query = @"SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial
                          FROM tb_prospecto WHERE id_estatus_prospecto = 1";

            var prospectos = new List<Prospecto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospectos = (await connection.QueryAsync<Prospecto>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prospectos;
        }

        public async Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion)
        {
            var query = @"SELECT id_prospecto IdProspecto
                          FROM tb_cotizacion
                          WHERE id_cotizacion = @idCotizacion";

            var idProspecto = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idProspecto = await connection.ExecuteScalarAsync<int>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idProspecto;
        }

        public async Task<Prospecto> ObtenerProspectoPorCotizacion(int idCotizacion)
        {
            var query = @"DECLARE @idProspecto int = (select id_prospecto from tb_cotizacion where id_cotizacion = @idCotizacion) 
                          SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                           domicilio_fiscal DomicilioFiscal, telefono Telefono, representante_legal RepresentanteLegal , documentacion Documentacion, 
				                           id_estatus_prospecto IdEstatusProspecto, fecha_alta FechaAlta, id_personal IdPersonal, 
                                           nombre_contacto NombreContacto, numero_contacto NumeroContacto, ext_contacto ExtContacto, email_contacto EmailContacto
                          FROM tb_prospecto
                          WHERE id_prospecto = @idProspecto";

            var prospecto = new Prospecto();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospecto = await connection.QueryFirstAsync<Prospecto>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prospecto;
        }

        public async Task InactivarProspecto(int registroAEliminar)
        {
            var query = @"UPDATE tb_prospecto set id_estatus_prospecto = @idEstatusProspecto where id_prospecto = @registroAEliminar 
                          UPDATE tb_cotizacion set id_estatus_cotizacion = @idEstatusCotizacion where id_prospecto = @registroAEliminar
                          UPDATE tb_direccion set id_estatus_direccion = @idEstatusDireccion where id_prospecto = @registroAEliminar";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idEstatusProspecto = EstatusProspecto.Inactivo, idEstatusCotizacion = EstatusCotizacion.Inactivo, idEstatusDireccion = EstatusDireccion.Inactivo, registroAEliminar });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Prospecto>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc)
        {
            var query = $@"SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial, rfc Rfc, fecha_alta FechaAlta 
                          FROM tb_prospecto                         
                            {(string.IsNullOrEmpty(nombreComercial) ? "" : $" WHERE nombre_comercial like '%' + '{nombreComercial}' + '%'")}
                            {(string.IsNullOrEmpty(rfc) ? "" : $" WHERE rfc = '{rfc}'")}   
                          ORDER BY id_prospecto desc";

            var coincidencias = new List<Prospecto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    coincidencias = (await connection.QueryAsync<Prospecto>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return coincidencias;
        }

        public async Task<Direccion> ObtenerDireccionPorId(int id)
        {
            var query = $@"SELECT id_direccion IdDireccion, nombre_sucursal NombreSucursal, id_tipo_inmueble IdTipoInmueble,
                                  d.id_estado IdEstado, d.id_tabulador IdTabulador, municipio Municipio, ciudad Ciudad, colonia Colonia, domicilio Domicilio, referencia Referencia, codigo_postal CodigoPostal,
                                  contacto Contacto, telefono_contacto TelefonoContacto, id_estatus_direccion IdEstatusDireccion, fecha_alta FechaAlta
                           FROM tb_direccion  d
                           WHERE id_direccion = @id
                                ";

            var direccion = new Direccion();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direccion = await connection.QueryFirstAsync<Direccion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direccion;
        }

        public async Task ActualizarDireccion(Direccion direccion)
        {
            var query = @"update tb_direccion 
	                      set nombre_sucursal = @NombreSucursal, id_tipo_inmueble = @IdTipoInmueble, id_estado = @IdEstado, municipio = @Municipio, ciudad = @Ciudad, 
		                      colonia = @Colonia, domicilio = @Domicilio, codigo_postal = @CodigoPostal, id_tabulador = @IdTabulador
	                      where id_direccion = @IdDireccion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, direccion);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PuestoDireccionCotizacion> ObtenerPuestoDireccionCotizacionPorId(int id)
        {
            var query = $@"SELECT id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, id_puesto IdPuesto, id_direccion_cotizacion IdDireccionCotizacion,
                            jornada Jornada, id_turno IdTurno, cantidad Cantidad, hr_inicio HrInicio, hr_fin HrFin, dia_inicio DiaInicio, dia_Fin DiaFin,
                            fecha_alta FechaAlta, sueldo Sueldo
                        FROM tb_puesto_direccion_cotizacion
                        WHERE id_puesto_direccioncotizacion = @id";

            var puestoDireccionCotizacion = new PuestoDireccionCotizacion();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestoDireccionCotizacion = await connection.QueryFirstAsync<PuestoDireccionCotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return puestoDireccionCotizacion;
        }
    }
}
