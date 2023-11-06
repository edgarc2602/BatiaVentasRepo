﻿using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using System.Runtime.CompilerServices;
using SistemaVentasBatia.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Xml.Schema;

namespace SistemaVentasBatia.Services
{
    public interface ICotizacionesService
    {
        Task CrearCotizacion(CotizacionDTO cotizacionVM);
        Task ObtenerListaCotizaciones(ListaCotizacionDTO listaCotizacionesVM, int autorizacion, int idPersonal);
        Task<int> ObtenerAutorizacion(int idPersonal);
        Task ObtenerListaDireccionesPorCotizacion(ListaDireccionDTO listaDireccionesVM);
        Task<List<DireccionDTO>> ObtenerCatalogoDireccionesPorProspecto(int idProspecto);
        Task AgregarDireccionCotizacion(DireccionCotizacionDTO direccionCVM);
        Task ObtenerListaPuestosPorCotizacion(ListaPuestosDireccionCotizacionDTO listaPuestosDireccionCotizacionVM);
        Task ObtenerCatalogoDireccionesPorCotizacion(ListaPuestosDireccionCotizacionDTO listaPuestosDireccionCotizacionVM);
        Task CrearPuestoDireccionCotizacion(PuestoDireccionCotizacionDTO operarioVM);
        Task<ResumenCotizacionLimpiezaDTO> ObtenerResumenCotizacionLimpieza(int id);
        Task EliminarCotizacion(int registroAEliminar);
        Task EliminarDireccionCotizacion(int registroAEliminar);
        Task<int> ObtenerIdCotizacionPorDireccion(int registroAEliminar);
        Task<int> ObtenerIdDireccionCotizacionPorOperario(int registroAEliminar);
        Task EliminarOperario(int registroAEliminar);
        Task<int> DuplicarCotizacion(int idCotizacion);
        Task ActualizarIndirectoUtilidad(int idCotizacion, string indirecto, string utilidad, string comisionSV, string comisionExt);
        Task <bool>ActualizarCotizacion(int idCotizacion, int idProspecto, Servicio idServicio);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerMaterialCotizacionLimpieza(int id);
        Task ActualizarPuestoDireccionCotizacion(PuestoDireccionCotizacionDTO operarioVM);
        Task <Boolean>ActualizarSalarios(PuestoTabulador salarios);
        Task<int> ObtieneIdCotizacionPorOperario(int idPuestoDireccionCotizacion);
        Task<int> ObtieneIdDireccionCotizacionPorOperario(int idPuestoDireccionCotizacion);
        Task<int> ObtenerIdPuestoDireccionCotizacionPorMaterial(int registroAEliminar);
        Task<int> ObtenerIdDireccionCotizacionPorMaterial(int registroAEliminar);
        Task<int> ObtenerIdCotizacionPorMaterial(int idDireccionCotizacion);
        Task<CotizaPorcentajes> ObtenerPorcentajesCotizacion();
        Task<bool> ActualizarPorcentajesPredeterminadosCotizacion(CotizaPorcentajes porcentajes);
    }

    public class CotizacionesService : ICotizacionesService
    {
        private readonly ICotizacionesRepository cotizacionesRepo;
        private readonly ICatalogosRepository catalogosRepo;
        private readonly IMaterialRepository materialRepo;
        private readonly IMapper mapper;

        public CotizacionesService(IMapper mapper, ICotizacionesRepository cotizacionesRepo, ICatalogosRepository catalogosRepo, IMaterialRepository materialRepo)
        {
            this.cotizacionesRepo = cotizacionesRepo;
            this.catalogosRepo = catalogosRepo;
            this.mapper = mapper;
            this.materialRepo = materialRepo;
        }

        public async Task CrearCotizacion(CotizacionDTO cotizacionVM)
        {
            var cotizacion = mapper.Map<Cotizacion>(cotizacionVM);

            cotizacion.IdEstatusCotizacion = EstatusCotizacion.Activa;
            cotizacion.FechaAlta = DateTime.Now;

            await cotizacionesRepo.InsertaCotizacion(cotizacion);

            cotizacionVM.IdCotizacion = cotizacion.IdCotizacion;
        }
        
        public async Task ObtenerListaCotizaciones(ListaCotizacionDTO listaCotizacionesVM, int autorizacion, int idPersonal)
        {
            listaCotizacionesVM.Rows = await cotizacionesRepo.ContarCotizaciones(listaCotizacionesVM.IdProspecto, listaCotizacionesVM.IdEstatusCotizacion, listaCotizacionesVM.IdServicio, idPersonal, autorizacion);

            if (listaCotizacionesVM.Rows > 0)
            {
                listaCotizacionesVM.NumPaginas = (listaCotizacionesVM.Rows / 10);

                if (listaCotizacionesVM.Rows % 10 > 0)
                {
                    listaCotizacionesVM.NumPaginas++;
                }

                var lista = await cotizacionesRepo.ObtenerCotizaciones(listaCotizacionesVM.Pagina, listaCotizacionesVM.IdProspecto, listaCotizacionesVM.IdEstatusCotizacion, listaCotizacionesVM.IdServicio, autorizacion, idPersonal);
                listaCotizacionesVM.Cotizaciones = lista.Select(c =>
                    new CotizacionMinDTO
                    {
                        IdCotizacion = c.IdCotizacion,
                        IdProspecto = c.IdProspecto,
                        Servicio = c.IdServicio.ToString(),
                        Total = (decimal)c.Total,
                        FechaAlta = c.FechaAlta,
                        IdCotizacionOriginal = c.IdCotizacionOriginal,
                        NombreComercial = c.NombreComercial,
                        IdAlta = c.IdAlta
                    }).ToList();
            }
            else
            {
                listaCotizacionesVM.Cotizaciones = new List<CotizacionMinDTO>();
            }
        }
        public async Task<int> ObtenerAutorizacion(int idPersonal)
        {
            int autorizacion = await cotizacionesRepo.ObtenerAutorizacion(idPersonal);
            return autorizacion;
        }

        public async Task ObtenerListaDireccionesPorCotizacion(ListaDireccionDTO listaDireccionesVM)
        {
            var lista = await cotizacionesRepo.ObtenerDireccionesPorCotizacion(listaDireccionesVM.IdCotizacion, listaDireccionesVM.Pagina);

            listaDireccionesVM.Direcciones = lista.Select(d =>
            new DireccionMinDTO
            {
                IdDireccion = d.IdDireccion,
                IdProspecto = d.IdProspecto,
                Estado = d.Estado,
                TipoInmueble = d.TipoInmueble,
                DomicilioCompleto = (d.Domicilio ?? "") + ", " + (d.Colonia ?? "") + ", " + (d.Municipio ?? "") + ", " + (d.Ciudad ?? "") + ", " + (d.Estado ?? "") + ", CP " + (d.CodigoPostal),
                IdDireccionCotizacion = d.IdDireccionCotizacion,
                NombreSucursal = d.NombreSucursal
            }).ToList();
        }

        public async Task<List<DireccionDTO>> ObtenerCatalogoDireccionesPorProspecto(int idProspecto)
        {
            var direcciones = mapper.Map<List<DireccionDTO>>(await cotizacionesRepo.ObtenerCatalogoDirecciones(idProspecto));

            return direcciones;
        }

        public async Task AgregarDireccionCotizacion(DireccionCotizacionDTO direccionCVM)
        {
            var direccion = mapper.Map<DireccionCotizacion>(direccionCVM);

            await cotizacionesRepo.InsertarDireccionCotizacion(direccion);
        }

        public async Task ObtenerListaPuestosPorCotizacion(ListaPuestosDireccionCotizacionDTO listaPuestosDireccionCotizacionVM)
        {
            listaPuestosDireccionCotizacionVM.PuestosDireccionesCotizacion = mapper.Map<List<PuestoDireccionMinDTO>>(await cotizacionesRepo.ObtienePuestosPorCotizacion(listaPuestosDireccionCotizacionVM.IdCotizacion));
        }
        
        public async Task ObtenerCatalogoDireccionesPorCotizacion(ListaPuestosDireccionCotizacionDTO listaPuestosDireccionCotizacionVM)
        {
            listaPuestosDireccionCotizacionVM.DireccionesCotizacion = mapper.Map<List<DireccionDTO>>(await cotizacionesRepo.ObtenerCatalogoDireccionesCotizacion(listaPuestosDireccionCotizacionVM.IdCotizacion));
        }

        public async Task CrearPuestoDireccionCotizacion(PuestoDireccionCotizacionDTO operariosVM)
        {
            var operariosModel = mapper.Map<PuestoDireccionCotizacion>(operariosVM);

            CalcularCostosOperario(operariosModel);

            int idOperario = await cotizacionesRepo.InsertaPuestoDireccionCotizacion(operariosModel);

            var materialPuesto = await catalogosRepo.ObtenerMaterialDefaultPorPuesto(operariosModel.IdPuesto);
            if (materialPuesto.Count > 0)
            {
                await CalcularPreciosMaterial(materialPuesto, operariosModel);
            }

            var uniformePuesto = (await catalogosRepo.ObtenerUniformeDefaultPorPuesto(operariosModel.IdPuesto)).ToList();
            if (uniformePuesto.Count > 0)
            {
                await CalcularPreciosMaterial(uniformePuesto, operariosModel);
            }

            var equipoPuesto = (await catalogosRepo.ObtenerEquipoDefaultPorPuesto(operariosModel.IdPuesto)).ToList();
            if (equipoPuesto.Count > 0)
            {
                await CalcularPreciosMaterial(equipoPuesto, operariosModel);
            }

            var herraPuesto = (await catalogosRepo.ObtenerHerramientaDefaultPorPuesto(operariosModel.IdPuesto)).ToList();
            if (herraPuesto.Count > 0)
            {
                await CalcularPreciosMaterial(herraPuesto, operariosModel);
            }

            await InsertarMaterialesDefaultOperarios(materialPuesto, uniformePuesto, equipoPuesto, herraPuesto, idOperario, operariosModel.IdCotizacion, operariosModel.IdDireccionCotizacion, operariosVM.IdTurno, operariosModel.IdPersonal);
        }

        private void CalcularCostosOperario(PuestoDireccionCotizacion operariosModel)
        {
            //operariosModel.Aguinaldo = operariosModel.Vacaciones =
            //    Math.Round((operariosModel.Sueldo / 30m) * (15m / 365m) * 30m, 2);
            //operariosModel.PrimaVacacional =
            //    Math.Round((operariosModel.Vacaciones / 30m) * (15m / 365m) * 30m, 2);
            operariosModel.Aguinaldo =(((operariosModel.Sueldo / 30.4167m) * 15m) / 12m);
            //operariosModel.Vacaciones = Math.Round((((operariosModel.Sueldo / 30.4167m) * 12m)* .25m) / 12m, 2);
            operariosModel.PrimaVacacional =((((operariosModel.Sueldo / 30.4167m) * 12m) * .25m) / 12m);
            if (operariosModel.Sueldo > (207.44m * 30.4167m))
            {

            }
            else
            {

            }
            operariosModel.IMSS = 1834;/*operariosModel.Sueldo * .139M;*/
            operariosModel.ISN = operariosModel.Sueldo * .03M;
            operariosModel.Total = Math.Round(operariosModel.Sueldo + operariosModel.Aguinaldo + operariosModel.PrimaVacacional
                + operariosModel.IMSS + operariosModel.ISN, 2); //+ operariosModel.Vacaciones 
        }

        private async Task InsertarMaterialesDefaultOperarios(List<MaterialPuesto> materialPuesto, List<MaterialPuesto> uniformePuesto, List<MaterialPuesto> equipoPuesto, List<MaterialPuesto> herramientaPuesto, int idOperario, int idCotizacion, int idDireccionCotizacion, Enums.Turno idTurno, int idPersonal)
        {
            var materialCotizacion = new List<MaterialCotizacion>();

            foreach (var materialP in materialPuesto)
            {
                var idFrecuencia = CalcFrecuencia(idTurno, materialP.IdFrecuencia);

                var total = (materialP.Precio * materialP.Cantidad);
                decimal impMensual = total / (int)idFrecuencia;

                materialCotizacion.Add(new MaterialCotizacion
                {
                    ClaveProducto = materialP.ClaveProducto,
                    IdCotizacion = idCotizacion,
                    IdDireccionCotizacion = idDireccionCotizacion,
                    IdPuestoDireccionCotizacion = idOperario,
                    Cantidad = materialP.Cantidad,
                    PrecioUnitario = materialP.Precio,
                    IdFrecuencia = idFrecuencia,
                    FechaAlta = DateTime.Now,
                    Total = total,
                    ImporteMensual = impMensual,
                    IdPersonal = idPersonal
                });
            }
            if (materialCotizacion.Count > 0)
            {
                await materialRepo.InsertarMaterialesCotizacion(materialCotizacion);
            }

            materialCotizacion = uniformePuesto.Select(materialP =>
            new MaterialCotizacion
            {
                ClaveProducto = materialP.ClaveProducto,
                IdCotizacion = idCotizacion,
                IdDireccionCotizacion = idDireccionCotizacion,
                IdPuestoDireccionCotizacion = idOperario,
                Cantidad = materialP.Cantidad,
                PrecioUnitario = materialP.Precio,
                IdFrecuencia = CalcFrecuencia(idTurno, materialP.IdFrecuencia),
                FechaAlta = DateTime.Now,
                Total = (materialP.Precio * materialP.Cantidad),
                ImporteMensual = (materialP.Precio * materialP.Cantidad) / (int)CalcFrecuencia(idTurno, materialP.IdFrecuencia),
                IdPersonal = idPersonal
            }).ToList();
            if (materialCotizacion.Count > 0)
            {
                await materialRepo.InsertarUniformeCotizacion(materialCotizacion);
            }

            materialCotizacion = herramientaPuesto.Select(materialP =>
            new MaterialCotizacion
            {
                ClaveProducto = materialP.ClaveProducto,
                IdCotizacion = idCotizacion,
                IdDireccionCotizacion = idDireccionCotizacion,
                IdPuestoDireccionCotizacion = idOperario,
                Cantidad = materialP.Cantidad,
                PrecioUnitario = materialP.Precio,
                IdFrecuencia = CalcFrecuencia(idTurno, materialP.IdFrecuencia),
                FechaAlta = DateTime.Now,
                Total = (materialP.Precio * materialP.Cantidad),
                ImporteMensual = (materialP.Precio * materialP.Cantidad) / (int)CalcFrecuencia(idTurno, materialP.IdFrecuencia),
                IdPersonal = idPersonal
            }).ToList();
            if (materialCotizacion.Count > 0)
            {
                await materialRepo.InsertarHerramientaCotizacion(materialCotizacion);
            }

            materialCotizacion = equipoPuesto.Select(materialP =>
            new MaterialCotizacion
            {
                ClaveProducto = materialP.ClaveProducto,
                IdCotizacion = idCotizacion,
                IdDireccionCotizacion = idDireccionCotizacion,
                IdPuestoDireccionCotizacion = idOperario,
                Cantidad = materialP.Cantidad,
                PrecioUnitario = materialP.Precio,
                IdFrecuencia = CalcFrecuencia(idTurno, materialP.IdFrecuencia),
                FechaAlta = DateTime.Now,
                Total = (materialP.Precio * materialP.Cantidad),
                ImporteMensual = (materialP.Precio * materialP.Cantidad) / (int)CalcFrecuencia(idTurno, materialP.IdFrecuencia),
                IdPersonal = idPersonal
            }).ToList();
            if (materialCotizacion.Count > 0)
            {
                await materialRepo.InsertarEquipoCotizacion(materialCotizacion);
            }
        }

        public async Task CalcularPreciosMaterial(List<MaterialPuesto> materialPuesto, PuestoDireccionCotizacion operario)
        {
            var idEstado = await cotizacionesRepo.ObtenerIdEstadoDeDireccionCotizacion(operario.IdDireccionCotizacion);

            var listaClaves = string.Join(',', materialPuesto.Select(x => "'" + x.ClaveProducto + "'"));

            var preciosProductosPorEstado = await materialRepo.ObtenerPreciosProductosPorEstado(listaClaves, idEstado);

            var preciosBaseProductos = await materialRepo.ObtenerPreciosBaseProductos(listaClaves);

            foreach (var materialP in materialPuesto)
            {
                var preciosProducto = preciosProductosPorEstado.Where(x => x.Clave == materialP.ClaveProducto);

                if (preciosProducto.Count() > 0)
                {
                    materialP.Precio = preciosProducto.FirstOrDefault(x => x.Clave == materialP.ClaveProducto).Precio;
                }
                else
                {
                    materialP.Precio = preciosBaseProductos.FirstOrDefault(x => x.Clave == materialP.ClaveProducto).Precio;
                }
            }
        }

        public async Task<ResumenCotizacionLimpiezaDTO> ObtenerResumenCotizacionLimpieza(int id)
        {
            var resumenCotizacion = mapper.Map<ResumenCotizacionLimpiezaDTO>(await cotizacionesRepo.ObtenerResumenCotizacionLimpieza(id));
            var obtenercot = mapper.Map<Cotizacion>(await cotizacionesRepo.ObtenerCotizacion(id));
            var obtenernombre = mapper.Map<Cotizacion>(await cotizacionesRepo.ObtenerNombreComercialCotizacion(id));
            try
            {
                resumenCotizacion.SubTotal = resumenCotizacion.Salario + resumenCotizacion.Provisiones + resumenCotizacion.CargaSocial + resumenCotizacion.Material + resumenCotizacion.Uniforme + resumenCotizacion.Equipo + resumenCotizacion.Herramienta + resumenCotizacion.Servicio;
                resumenCotizacion.Indirecto = resumenCotizacion.SubTotal * obtenercot.CostoIndirecto;
                resumenCotizacion.Utilidad = (resumenCotizacion.SubTotal + resumenCotizacion.Indirecto) * obtenercot.Utilidad;
                resumenCotizacion.ComisionSV = (resumenCotizacion.SubTotal + resumenCotizacion.Indirecto + resumenCotizacion.Utilidad) * (obtenercot.ComisionSV);
                resumenCotizacion.ComisionExt = (resumenCotizacion.SubTotal + resumenCotizacion.Indirecto + resumenCotizacion.Utilidad + resumenCotizacion.ComisionSV) * (obtenercot.ComisionExt);
                resumenCotizacion.NombreComercial = obtenernombre.NombreComercial;
                decimal indirecto;
                if (resumenCotizacion.SubTotal != 0)
                {
                    indirecto = (resumenCotizacion.Indirecto / resumenCotizacion.SubTotal) * 100M;
                }
                else
                {
                    indirecto = 0;
                }

                decimal utilidad;
                if ((resumenCotizacion.Indirecto + resumenCotizacion.SubTotal) != 0)
                {
                    utilidad = (resumenCotizacion.Utilidad / (resumenCotizacion.Indirecto + resumenCotizacion.SubTotal)) * 100;
                }
                else
                {
                    utilidad = 0; 
                }

                decimal ComisionSV;
                if((resumenCotizacion.Indirecto + resumenCotizacion.Utilidad + resumenCotizacion.SubTotal) != 0)
                {
                    ComisionSV = (resumenCotizacion.ComisionSV / (resumenCotizacion.Utilidad + resumenCotizacion.Indirecto + resumenCotizacion.SubTotal)) * 100;
                }
                else
                {
                    ComisionSV = 0;
                }

                decimal ComisionExt;
                if ((resumenCotizacion.Indirecto + resumenCotizacion.Utilidad + resumenCotizacion.ComisionSV + resumenCotizacion.SubTotal) != 0)
                {
                    ComisionExt = (resumenCotizacion.ComisionExt / (resumenCotizacion.ComisionSV + resumenCotizacion.Utilidad + resumenCotizacion.Indirecto + resumenCotizacion.SubTotal)) * 100;
                }
                else
                {
                    ComisionExt = 0;
                }
                int indirectoint = Convert.ToInt32(indirecto);
                int utilidadint = Convert.ToInt32(utilidad);
                int comisionsvint = Convert.ToInt32(ComisionSV);
                int comisionExtint = Convert.ToInt32(ComisionExt);
                resumenCotizacion.IndirectoPor = indirectoint.ToString();
                resumenCotizacion.UtilidadPor = utilidadint.ToString();
                resumenCotizacion.CsvPor = comisionsvint.ToString();
                resumenCotizacion.ComisionExtPor = comisionExtint.ToString();
               if (resumenCotizacion.SubTotal == 0)
                {
                    resumenCotizacion.IndirectoPor = obtenercot.CostoIndirecto.ToString();
                    resumenCotizacion.UtilidadPor = obtenercot.Utilidad.ToString();
                    resumenCotizacion.CsvPor = obtenercot.ComisionSV.ToString();
                    resumenCotizacion.ComisionExtPor = obtenercot.ComisionExt.ToString();
                }
                decimal total = resumenCotizacion.SubTotal + resumenCotizacion.Indirecto + resumenCotizacion.Utilidad + resumenCotizacion.ComisionSV + resumenCotizacion.ComisionExt;
                string numerotxt = "";
                if (total == 0)
                {
                    numerotxt = "cero";
                }
                else
                {
                    numerotxt = NumeroEnLetras.NumeroALetras(total);
                    numerotxt = numerotxt.ToLower();
                    numerotxt = char.ToUpper(numerotxt[0]) + numerotxt.Substring(1);
                    numerotxt = "(" + numerotxt + " M.N.)";
                   
                }
                await cotizacionesRepo.InsertarTotalCotizacion(total, id, numerotxt);
                return resumenCotizacion;
            }
            catch
            {
                return null;
            }
        }

        public async Task EliminarCotizacion(int registroAEliminar)
        {
            await cotizacionesRepo.InactivarCotizacion(registroAEliminar);
        }

        public async Task EliminarDireccionCotizacion(int registroAEliminar)
        {
            await cotizacionesRepo.InactivarDireccionCotizacion(registroAEliminar);
        }

        public async Task<int> ObtenerIdCotizacionPorDireccion(int registroAEliminar)
        {
            var idCotizacion = await cotizacionesRepo.ObtenerIdCotizacionPorDireccion(registroAEliminar);

            return idCotizacion;
            }

        public async Task<int> ObtenerIdDireccionCotizacionPorOperario(int registroAEliminar)
        {
            var idDireccionCotizacion = await cotizacionesRepo.ObtenerIdDireccionCotizacionPorOperario(registroAEliminar);

            return idDireccionCotizacion;
        }

        public async Task EliminarOperario(int registroAEliminar)
        {
            await cotizacionesRepo.EliminarOperario(registroAEliminar);
        }

        public async Task<int> DuplicarCotizacion(int idCotizacion)
        {
            var idCotizacionNueva = await cotizacionesRepo.CopiarCotizacion(idCotizacion);

            await cotizacionesRepo.CopiarDirectorioCotizacion(idCotizacion, idCotizacionNueva);

            var direccionesCotizacion = await cotizacionesRepo.ObtieneDireccionesCotizacion(idCotizacion);

            var direccionesCotizacionNueva = await cotizacionesRepo.ObtieneDireccionesCotizacion(idCotizacionNueva);

            var productoscotizacion = await materialRepo.ObtieneMaterialesPorIdCotizacion(idCotizacion);

            var uniformescotizacion = await materialRepo.ObtieneUniformesPorIdCotizacion(idCotizacion);

            var equiposcotizacion = await materialRepo.ObtieneEquiposPorIdCotizacion(idCotizacion);

            var herramientascotizacion = await materialRepo.ObtieneHerramientasPorIdCotizacion(idCotizacion);

            foreach (var (direccionesNuevas, direccionesNUEVAS) in direccionesCotizacion.Zip(direccionesCotizacionNueva))
            {
                var direccionCotizacionNueva = direccionesCotizacionNueva.FirstOrDefault(x => x.IdDireccion == direccionesNuevas.IdDireccion);

                var idPuestoDireccionCotizacionNuevo = await cotizacionesRepo.CopiarPlantillaDireccionCotizacion(direccionesNuevas.IdDireccionCotizacion, direccionCotizacionNueva.IdDireccionCotizacion);
            }
            var operariosCotizacion = await cotizacionesRepo.ObtieneOperariosCotizacion(idCotizacionNueva);

            var operariosCotizacionAnteriores = await cotizacionesRepo.ObtieneOperariosCotizacion(idCotizacion);

            foreach (var (dir, dirnueva) in direccionesCotizacion.Zip(direccionesCotizacionNueva))
            {
                foreach (var (operario, operarioant) in operariosCotizacion.Zip(operariosCotizacionAnteriores))
                {
                    foreach (var producto in productoscotizacion)
                    {
                        if(
                            producto.IdPuestoDireccionCotizacion == operarioant.IdPuestoDireccionCotizacion &&
                            producto.IdDireccionCotizacion == dir.IdDireccionCotizacion
                        )
                        {
                            await cotizacionesRepo.CopiarMaterial(producto, idCotizacionNueva, operario.IdDireccionCotizacion, operario.IdPuestoDireccionCotizacion);
                        }
                    }
                    foreach (var uni in uniformescotizacion)
                    {
                        if (
                            uni.IdPuestoDireccionCotizacion == operarioant.IdPuestoDireccionCotizacion &&
                            uni.IdDireccionCotizacion == dir.IdDireccionCotizacion
                        )
                        {
                            await cotizacionesRepo.CopiarUniforme(uni, idCotizacionNueva, operario.IdDireccionCotizacion, operario.IdPuestoDireccionCotizacion);
                        }
                    }
                    foreach (var equipo in equiposcotizacion)
                    {
                        if (
                            equipo.IdPuestoDireccionCotizacion == operarioant.IdPuestoDireccionCotizacion &&
                            equipo.IdDireccionCotizacion == dir.IdDireccionCotizacion
                        )
                        {
                            await cotizacionesRepo.CopiarEquipo(equipo, idCotizacionNueva, operario.IdDireccionCotizacion, operario.IdPuestoDireccionCotizacion);
                        }
                    }
                    foreach (var herr in herramientascotizacion)
                    {
                        if (
                            herr.IdPuestoDireccionCotizacion == operarioant.IdPuestoDireccionCotizacion &&
                            herr.IdDireccionCotizacion == dir.IdDireccionCotizacion
                        )
                        {
                            await cotizacionesRepo.CopiarHerramienta(herr, idCotizacionNueva, operario.IdDireccionCotizacion, operario.IdPuestoDireccionCotizacion);
                        }
                    }
                }
            }
            //Agrega extra de la cotizacion --OK
            foreach (var (direcNuevas, direcAnteriores) in direccionesCotizacionNueva.Zip(direccionesCotizacion))
            {
                foreach (var prod in productoscotizacion)
                {
                    if (prod.IdPuestoDireccionCotizacion == 0 && prod.IdDireccionCotizacion == direcAnteriores.IdDireccionCotizacion)
                    {
                        await cotizacionesRepo.CopiarMaterial(prod, idCotizacionNueva, direcNuevas.IdDireccionCotizacion, 0);
                    }
                }
                foreach (var uni in uniformescotizacion)
                {
                    if (uni.IdPuestoDireccionCotizacion == 0 && uni.IdDireccionCotizacion == direcAnteriores.IdDireccionCotizacion)
                    {
                        await cotizacionesRepo.CopiarUniforme(uni, idCotizacionNueva, direcNuevas.IdDireccionCotizacion, 0);
                    }
                }
                foreach (var equipo in equiposcotizacion)
                {
                    if (equipo.IdPuestoDireccionCotizacion == 0 && equipo.IdDireccionCotizacion == direcAnteriores.IdDireccionCotizacion)
                    {
                        await cotizacionesRepo.CopiarEquipo(equipo, idCotizacionNueva, direcNuevas.IdDireccionCotizacion, 0);
                    }
                }
                foreach (var herr in herramientascotizacion)
                {
                    if (herr.IdPuestoDireccionCotizacion == 0 && herr.IdDireccionCotizacion == direcAnteriores.IdDireccionCotizacion)
                    {
                        await cotizacionesRepo.CopiarHerramienta(herr, idCotizacionNueva, direcNuevas.IdDireccionCotizacion, 0);
                    }
                }
            }
            return idCotizacionNueva;
        }

        public async Task ActualizarIndirectoUtilidad(int idCotizacion, string indirecto, string utilidad, string comisionSV, string comisionExt)
        {
            await cotizacionesRepo.ActualizarIndirectoUtilidad(idCotizacion, indirecto, utilidad,comisionSV,comisionExt);
        }

        public async Task <bool>ActualizarCotizacion(int idCotizacion, int idProspecto, Servicio idServicio)
        {
            bool result = await cotizacionesRepo.ValidarDirecciones(idCotizacion);

            if (result == true)
            {
                await cotizacionesRepo.ActualizarCotizacion(idCotizacion, idProspecto, idServicio);
            }
            else
            {
                result = false;
            }
            return result;
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerMaterialCotizacionLimpieza(int id)
        {
            await Task.Delay(10);

            return new ListaMaterialesCotizacionLimpiezaDTO();
        }

        public async Task ActualizarPuestoDireccionCotizacion(PuestoDireccionCotizacionDTO operarioVM)
        {
            var operario = mapper.Map<PuestoDireccionCotizacion>(operarioVM);

            CalcularCostosOperario(operario);

            await cotizacionesRepo.ActualizarPuestoDireccionCotizacion(operario);
        }

        public async Task<int> ObtieneIdCotizacionPorOperario(int idPuestoDireccionCotizacion)
        {
            return await cotizacionesRepo.ObtieneIdCotizacionPorOperario(idPuestoDireccionCotizacion);
        }

        public async Task<int> ObtieneIdDireccionCotizacionPorOperario(int idPuestoDireccionCotizacion)
        {
            return await cotizacionesRepo.ObtieneIdDireccionCotizacionPorOperario(idPuestoDireccionCotizacion);
        }

        public async Task<int> ObtenerIdPuestoDireccionCotizacionPorMaterial(int idMaterialCotizacion)
        {
            return await materialRepo.ObtieneIdPuestoDireccionCotizacionPorMaterial(idMaterialCotizacion);
        }

        public async Task<int> ObtenerIdDireccionCotizacionPorMaterial(int idMaterialCotizacion)
        {
            return await materialRepo.ObtieneIdDireccionCotizacionPorMaterial(idMaterialCotizacion);
        }

        public async Task<int> ObtenerIdCotizacionPorMaterial(int idMaterialCotizacion)
        {
            return await materialRepo.ObtieneIdCotizacionPorMaterial(idMaterialCotizacion);
        }

        private Frecuencia CalcFrecuencia(Enums.Turno turno, Frecuencia frecuencia)
        {
            return turno == Enums.Turno.MEDIO ? (Frecuencia)((int)frecuencia * 2) :
                        (turno == Enums.Turno.CUARTO ? (Frecuencia)((int)frecuencia * 4) :
                        (Frecuencia)frecuencia);
        }

        public async Task<bool> ActualizarSalarios(PuestoTabulador salarios)
        {
            bool result =  await cotizacionesRepo.ActualizarSalarios(salarios);
            return result;
        }

        public async Task<CotizaPorcentajes> ObtenerPorcentajesCotizacion()
        {
            CotizaPorcentajes porcentajes = new CotizaPorcentajes();

            porcentajes = await cotizacionesRepo.ObtenerPorcentajesCotizacion();
            return porcentajes;
        }

        public async Task<bool> ActualizarPorcentajesPredeterminadosCotizacion(CotizaPorcentajes porcentajes)
        {
            await cotizacionesRepo.ActualizarPorcentajesPredeterminadosCotizacion(porcentajes);
            return true;
        }
    }
}
