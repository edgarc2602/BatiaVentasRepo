using AutoMapper;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IMaterialService
    {
        Task AgregarMaterialOperario(MaterialCotizacionDTO materialVM);
        Task ActualizarMaterialCotizacion(MaterialCotizacionDTO materialVM);
        Task EliminarMaterialDeCotizacion(int registroAEliminar);

        Task AgregarEquipoOperario(MaterialCotizacionDTO dto);
        Task ActualizarEquipoCotizacion(MaterialCotizacionDTO dto);
        Task EliminarEquipoCotizacion(int idEquipo);

        Task AgregarHerramientaOperario(MaterialCotizacionDTO dto);
        Task ActualizarHerramientaOperario(MaterialCotizacionDTO dto);
        Task EliminarHerramientaCotizacion(int idHerramienta);

        Task AgregarUniformeOperario(MaterialCotizacionDTO dto);
        Task ActualizarUniformeCotizacion(MaterialCotizacionDTO dto);
        Task EliminarUniformeCotizacion(int idUniforme);

        Task ObtenerListaEquipoCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);
        Task ObtenerListaHerramientaCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);
        Task ObtenerListaMaterialesCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);
        Task ObtenerListaUniformeCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);

        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaMaterialesOperarioLimpieza(int idPuestoDireccionCotizacion);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaEquipoOperario(int idPuestoCotizacion);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaHerramientaOperario(int idPuestoCotizacion);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaUniformeOperario(int idPuestoCotizacion);

        Task<MaterialCotizacionDTO> ObtenerEquipoCotizacionPorId(int id);
        Task<MaterialCotizacionDTO> ObtenerHerramientaCotizacionPorId(int id);
        Task<MaterialCotizacionDTO> ObtenerMaterialCotizacionPorId(int id);
        Task<MaterialCotizacionDTO> ObtenerUniformeCotizacionPorId(int id);
        Task<ServicioCotizacionDTO> ServicioGetById(int id);

        Task InsertarServicioCotizacion(ServicioCotizacion servicio);
        Task ActualizarServicioCotizacion(ServicioCotizacion servicio);
        Task EliminarServicioCotizacion(int id);


        Task<ListaServiciosCotizacionLimpiezaDTO> ObtenerListaServiciosCotizacion(int idCotizacion, int idDireccionCotizacion);

    }

    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepo;
        private readonly ICotizacionesRepository _cotizaRepo;
        private readonly IMapper _mapper;
        private readonly IServicioRepository _servicioRepo;

        public MaterialService(IMaterialRepository materialRepo, ICotizacionesRepository cotizaRepo, IServicioRepository servicioRepo, IMapper mapper)
        {
            _materialRepo = materialRepo;
            _cotizaRepo = cotizaRepo;
            _mapper = mapper;
            _servicioRepo = servicioRepo;
        }
        public async Task AgregarMaterialOperario(MaterialCotizacionDTO materialVM)
        {
            int estado = await _materialRepo.ObtenerIdEstadoPorIdDireccionCotizacion(materialVM.IdDireccionCotizacion);
            if (materialVM.edit == 1)
            {
                decimal precio = await _materialRepo.ObtenerPrecioProductoPorClave(materialVM.ClaveProducto);
                var material = _mapper.Map<MaterialCotizacion>(materialVM);
                material.PrecioUnitario = precio;
                material.Cantidad = materialVM.Cantidad;
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;



                await _materialRepo.ActualizarMaterialCotizacion(material);
            }
            else
            {

                var material = _mapper.Map<MaterialCotizacion>(materialVM);

                var materialesOperario = await _materialRepo.ObtenerMaterialCotizacionOperario(materialVM.IdPuestoDireccionCotizacion, materialVM.IdCotizacion);

                var materialOperario =
                    materialesOperario.FirstOrDefault(x =>
                    x.ClaveProducto == material.ClaveProducto && x.IdFrecuencia == material.IdFrecuencia && x.IdPuestoDireccionCotizacion == materialVM.IdPuestoDireccionCotizacion && x.IdDireccionCotizacion == materialVM.IdDireccionCotizacion);

                if (materialOperario != null)
                {
                    materialOperario.Cantidad = materialVM.Cantidad;
                    materialOperario.Total = materialOperario.PrecioUnitario * materialOperario.Cantidad;
                    materialOperario.ImporteMensual = materialOperario.Total / (int)materialOperario.IdFrecuencia;

                    await _materialRepo.ActualizarMaterialCotizacion(materialOperario);
                }
                else
                {
                    var preciosBaseProductos = await _materialRepo.ObtenerPreciosBaseProductos("'" + material.ClaveProducto + "'");

                    var idEstado = await _cotizaRepo.ObtenerIdEstadoDeDireccionCotizacion(materialVM.IdDireccionCotizacion);

                    var preciosProductoPorEstado = await _materialRepo.ObtenerPreciosProductosPorEstado("'" + material.ClaveProducto + "'", idEstado);

                    if (preciosProductoPorEstado.Count() > 0)
                    {
                        material.PrecioUnitario = preciosProductoPorEstado.Min(x => x.Precio);
                    }
                    else
                    {
                        material.PrecioUnitario = preciosBaseProductos.FirstOrDefault().Precio;
                    }

                    material.Total = material.PrecioUnitario * material.Cantidad;
                    material.ImporteMensual = material.Total / (int)material.IdFrecuencia;
                    material.FechaAlta = DateTime.Now;

                    await _materialRepo.AgregarMaterialCotizacion(material);
                }
            }
        }

        public async Task ActualizarMaterialCotizacion(MaterialCotizacionDTO materialVM)
        {
            var material = _mapper.Map<MaterialCotizacion>(materialVM);

            material.Total = (material.PrecioUnitario * material.Cantidad) / (int)material.IdFrecuencia;

            await _materialRepo.ActualizarMaterialCotizacion(material);
        }

        public async Task EliminarMaterialDeCotizacion(int idMaterialCotizacion)
        {
            await _materialRepo.EliminarMaterialCotizacion(idMaterialCotizacion);
        }

        public async Task ObtenerListaMaterialesCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMaterialesVM)
        {
            listaMaterialesVM.Rows = await _materialRepo.ContarMaterialesCotizacion(
                listaMaterialesVM.IdCotizacion, listaMaterialesVM.IdDireccionCotizacion, listaMaterialesVM.IdPuestoDireccionCotizacion, listaMaterialesVM.Keywords);

            if (listaMaterialesVM.Rows > 0)
            {
                listaMaterialesVM.NumPaginas = (listaMaterialesVM.Rows / 10);

                if (listaMaterialesVM.Rows % 10 > 0)
                {
                    listaMaterialesVM.NumPaginas++;
                }

                listaMaterialesVM.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerMaterialesCotizacion(
                    listaMaterialesVM.IdCotizacion, listaMaterialesVM.IdDireccionCotizacion, listaMaterialesVM.IdPuestoDireccionCotizacion, listaMaterialesVM.Keywords, listaMaterialesVM.Pagina));
            }
            else
            {
                listaMaterialesVM.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaMaterialesOperarioLimpieza(int idPuestoDireccionCotizacion)
        {
            var materialCotizacionLimpieza = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoDireccionCotizacion };

            materialCotizacionLimpieza.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(materialCotizacionLimpieza.IdPuestoDireccionCotizacion);

            materialCotizacionLimpieza.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(materialCotizacionLimpieza.IdPuestoDireccionCotizacion);

            materialCotizacionLimpieza.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerMaterialCotizacionOperario(idPuestoDireccionCotizacion, materialCotizacionLimpieza.IdCotizacion));

            return materialCotizacionLimpieza;
        }


        




        public async Task<MaterialCotizacionDTO> ObtenerMaterialCotizacionPorId(int id)
        {
            var material = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerMaterialCotizacionPorId(id));

            return material;
        }

        public async Task ObtenerListaEquipoCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales)
        {
            listaMateriales.Rows = await _materialRepo.ContarEquipoCotizacion(
                listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords);

            if (listaMateriales.Rows > 0)
            {
                listaMateriales.NumPaginas = (listaMateriales.Rows / 10);

                if (listaMateriales.Rows % 10 > 0)
                {
                    listaMateriales.NumPaginas++;
                }

                listaMateriales.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerEquipoCotizacion(
                    listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords, listaMateriales.Pagina));
            }
            else
            {
                listaMateriales.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }

        public async Task ObtenerListaHerramientaCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales)
        {
            listaMateriales.Rows = await _materialRepo.ContarHerramientaCotizacion(
                listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords);

            if (listaMateriales.Rows > 0)
            {
                listaMateriales.NumPaginas = (listaMateriales.Rows / 10);

                if (listaMateriales.Rows % 10 > 0)
                {
                    listaMateriales.NumPaginas++;
                }

                listaMateriales.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerHerramientaCotizacion(
                    listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords, listaMateriales.Pagina));
            }
            else
            {
                listaMateriales.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }

        public async Task ObtenerListaUniformeCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales)
        {
            listaMateriales.Rows = await _materialRepo.ContarUniformeCotizacion(
                listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords);

            if (listaMateriales.Rows > 0)
            {
                listaMateriales.NumPaginas = (listaMateriales.Rows / 10);

                if (listaMateriales.Rows % 10 > 0)
                {
                    listaMateriales.NumPaginas++;
                }

                listaMateriales.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerUniformeCotizacion(
                    listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords, listaMateriales.Pagina));
            }
            else
            {
                listaMateriales.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }





        public async Task <ListaServiciosCotizacionLimpiezaDTO> ObtenerListaServiciosCotizacion(int idCotizacion, int idDireccioNCotizacion)
        {
            var servicioCotizacion = new ListaServiciosCotizacionLimpiezaDTO { IdCotizacion = idCotizacion };
            servicioCotizacion.ServiciosCotizacion = _mapper.Map<List<ServicioCotizacionMinDTO>>(await _servicioRepo.ObtenerListaServiciosCotizacion(idCotizacion, idDireccioNCotizacion));
            return servicioCotizacion;
        }

        //public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaUniformeOperario(int idPuestoCotizacion)
        //{
        //    var uniformeCotizacion = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoCotizacion };

        //    uniformeCotizacion.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(uniformeCotizacion.IdPuestoDireccionCotizacion);

        //    uniformeCotizacion.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(uniformeCotizacion.IdPuestoDireccionCotizacion);

        //    uniformeCotizacion.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerUniformeCotizacionOperario(idPuestoCotizacion, idPuestoCotizacion));

        //    return uniformeCotizacion;
        //}














        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaEquipoOperario(int idPuestoCotizacion)
        {
            var equipoCotizacion = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoCotizacion };

            equipoCotizacion.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(equipoCotizacion.IdPuestoDireccionCotizacion);

            equipoCotizacion.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(equipoCotizacion.IdPuestoDireccionCotizacion);

            equipoCotizacion.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerEquipoCotizacionOperario(idPuestoCotizacion, equipoCotizacion.IdCotizacion));

            return equipoCotizacion;
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaHerramientaOperario(int idPuestoCotizacion)
        {
            var herramientaCotizacion = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoCotizacion };

            herramientaCotizacion.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(herramientaCotizacion.IdPuestoDireccionCotizacion);

            herramientaCotizacion.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(herramientaCotizacion.IdPuestoDireccionCotizacion);

            herramientaCotizacion.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerHerramientaCotizacionOperario(idPuestoCotizacion, herramientaCotizacion.IdCotizacion));

            return herramientaCotizacion;
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaUniformeOperario(int idPuestoCotizacion)
        {
            var uniformeCotizacion = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoCotizacion };

            uniformeCotizacion.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(uniformeCotizacion.IdPuestoDireccionCotizacion);

            uniformeCotizacion.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(uniformeCotizacion.IdPuestoDireccionCotizacion);

            uniformeCotizacion.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerUniformeCotizacionOperario(idPuestoCotizacion, idPuestoCotizacion));

            return uniformeCotizacion;
        }

        public async Task<MaterialCotizacionDTO> ObtenerEquipoCotizacionPorId(int id)
        {
            var equipo = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerEquipoCotizacionPorId(id));

            return equipo;
        }

        public async Task<MaterialCotizacionDTO> ObtenerHerramientaCotizacionPorId(int id)
        {
            var herramienta = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerHerramientaCotizacionPorId(id));

            return herramienta;
        }

        public async Task<MaterialCotizacionDTO> ObtenerUniformeCotizacionPorId(int id)
        {
            var uniforme = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerUniformeCotizacionPorId(id));

            return uniforme;
        }

        public async Task<ServicioCotizacionDTO> ServicioGetById(int id)
        {
            var servicio = new ServicioCotizacionDTO();

            servicio = _mapper.Map<ServicioCotizacionDTO>(await _materialRepo.ServicioGetById(id));
                return servicio;
        }

        public async Task AgregarEquipoOperario(MaterialCotizacionDTO dto)
        {
            if (dto.edit == 1)
            {
                decimal precio = await _materialRepo.ObtenerPrecioProductoPorClave(dto.ClaveProducto);

                var material = _mapper.Map<MaterialCotizacion>(dto);
                material.PrecioUnitario = precio;
                material.Cantidad = dto.Cantidad;
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;

                await _materialRepo.ActualizarEquipoCotizacion(material);
            }
            else
            {
                var equipo = _mapper.Map<MaterialCotizacion>(dto);

                var materialesOperario = await _materialRepo.ObtenerEquipoCotizacionOperario(equipo.IdPuestoDireccionCotizacion, equipo.IdCotizacion);

                var materialOperario =
                    materialesOperario.FirstOrDefault(x =>
                    x.ClaveProducto == equipo.ClaveProducto && x.IdFrecuencia == equipo.IdFrecuencia && x.IdPuestoDireccionCotizacion == equipo.IdPuestoDireccionCotizacion && x.IdDireccionCotizacion == equipo.IdDireccionCotizacion);

                if (materialOperario != null)
                {
                    materialOperario.Cantidad = equipo.Cantidad;
                    materialOperario.Total = materialOperario.PrecioUnitario * materialOperario.Cantidad;
                    materialOperario.ImporteMensual = materialOperario.Total / (decimal)materialOperario.IdFrecuencia;

                    await _materialRepo.ActualizarEquipoCotizacion(materialOperario);
                }
                else
                {
                    var preciosBaseProductos = await _materialRepo.ObtenerPreciosBaseProductos("'" + equipo.ClaveProducto + "'");

                    var idEstado = await _cotizaRepo.ObtenerIdEstadoDeDireccionCotizacion(equipo.IdDireccionCotizacion);

                    var preciosProductoPorEstado = await _materialRepo.ObtenerPreciosProductosPorEstado("'" + equipo.ClaveProducto + "'", idEstado);

                    if (preciosProductoPorEstado.Count() > 0)
                    {
                        equipo.PrecioUnitario = preciosProductoPorEstado.Min(x => x.Precio);
                    }
                    else
                    {
                        equipo.PrecioUnitario = preciosBaseProductos.FirstOrDefault().Precio;
                    }

                    equipo.Total = equipo.PrecioUnitario * equipo.Cantidad;
                    equipo.ImporteMensual = equipo.Total / (decimal)equipo.IdFrecuencia;
                    equipo.FechaAlta = DateTime.Now;

                    await _materialRepo.AgregarEquipoCotizacion(equipo);
                }
            }
        }

        public async Task ActualizarEquipoCotizacion(MaterialCotizacionDTO dto)
        {
            var equipo = _mapper.Map<MaterialCotizacion>(dto);

            equipo.Total = (equipo.PrecioUnitario * equipo.Cantidad);
            equipo.ImporteMensual = equipo.Total / (int)equipo.IdFrecuencia;

            await _materialRepo.ActualizarEquipoCotizacion(equipo);
        }

        public async Task EliminarEquipoCotizacion(int idEquipo)
        {
            await _materialRepo.EliminarEquipoCotizacion(idEquipo);
        }

        public async Task AgregarUniformeOperario(MaterialCotizacionDTO dto)
        {
            if (dto.edit == 1)
            {
                decimal precio = await _materialRepo.ObtenerPrecioProductoPorClave(dto.ClaveProducto);

                var material = _mapper.Map<MaterialCotizacion>(dto);
                material.PrecioUnitario = precio;
                material.Cantidad = dto.Cantidad;
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;

                await _materialRepo.ActualizarUniformeCotizacion(material);
            }
            else
            {
                var uniforme = _mapper.Map<MaterialCotizacion>(dto);

                var materialesOperario = await _materialRepo.ObtenerUniformeCotizacionOperario(uniforme.IdPuestoDireccionCotizacion, uniforme.IdCotizacion);

                var materialOperario =
                materialesOperario.FirstOrDefault(x =>
                x.ClaveProducto == uniforme.ClaveProducto && x.IdFrecuencia == uniforme.IdFrecuencia && x.IdPuestoDireccionCotizacion == uniforme.IdPuestoDireccionCotizacion && x.IdDireccionCotizacion == uniforme.IdDireccionCotizacion);

                if (materialOperario != null)
                {
                    materialOperario.Cantidad = uniforme.Cantidad;
                    materialOperario.Total = materialOperario.PrecioUnitario * materialOperario.Cantidad;
                    materialOperario.ImporteMensual = materialOperario.Total / (decimal)materialOperario.IdFrecuencia;

                    await _materialRepo.ActualizarUniformeCotizacion(materialOperario);
                }
                else
                {
                    var preciosBaseProductos = await _materialRepo.ObtenerPreciosBaseProductos("'" + uniforme.ClaveProducto + "'");

                    var idEstado = await _cotizaRepo.ObtenerIdEstadoDeDireccionCotizacion(uniforme.IdDireccionCotizacion);

                    var preciosProductoPorEstado = await _materialRepo.ObtenerPreciosProductosPorEstado("'" + uniforme.ClaveProducto + "'", idEstado);

                    if (preciosProductoPorEstado.Count() > 0)
                    {
                        uniforme.PrecioUnitario = preciosProductoPorEstado.Min(x => x.Precio);
                    }
                    else
                    {
                        uniforme.PrecioUnitario = preciosBaseProductos.FirstOrDefault().Precio;
                    }

                    uniforme.Total = uniforme.PrecioUnitario * uniforme.Cantidad;
                    uniforme.ImporteMensual = uniforme.Total / (decimal)uniforme.IdFrecuencia;
                    uniforme.FechaAlta = DateTime.Now;

                    await _materialRepo.AgregarUniformeCotizacion(uniforme);
                }
            }
        }

        public async Task InsertarServicioCotizacion(ServicioCotizacion servicio)
        {
            servicio.Total = servicio.PrecioUnitario * servicio.Cantidad;
            servicio.ImporteMensual = servicio.Total / (int)servicio.IdFrecuencia;
            servicio.FechaAlta = DateTime.Now;
            await _materialRepo.InsertarServicioCotizacion(servicio);
        }

        public async Task ActualizarServicioCotizacion(ServicioCotizacion servicio)
        {
            servicio.Total = servicio.PrecioUnitario * servicio.Cantidad;
            servicio.ImporteMensual = servicio.Total / (int)servicio.IdFrecuencia;
            servicio.FechaAlta = DateTime.Now;
            await _materialRepo.ActualizarServicioCotizacion(servicio);
        }

        public async Task EliminarServicioCotizacion(int id)
        {
            await _materialRepo.EliminarServicioCotizacion(id);
        }








        public async Task ActualizarUniformeCotizacion(MaterialCotizacionDTO dto)
        {
            var uniforme = _mapper.Map<MaterialCotizacion>(dto);

            uniforme.Total = (uniforme.PrecioUnitario * uniforme.Cantidad);
            uniforme.ImporteMensual = uniforme.Total / (int)uniforme.IdFrecuencia;

            await _materialRepo.ActualizarUniformeCotizacion(uniforme);
        }

        public async Task EliminarUniformeCotizacion(int idUniforme)
        {
            await _materialRepo.EliminarUniformeCotizacion(idUniforme);
        }

        public async Task AgregarHerramientaOperario(MaterialCotizacionDTO dto)
        {
            if (dto.edit == 1)
            {
                decimal precio = await _materialRepo.ObtenerPrecioProductoPorClave(dto.ClaveProducto);

                var material = _mapper.Map<MaterialCotizacion>(dto);
                material.PrecioUnitario = precio;
                material.Cantidad = dto.Cantidad;
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;

                await _materialRepo.ActualizarHerramientaCotizacion(material);
            }
            else
            {
                var herramienta = _mapper.Map<MaterialCotizacion>(dto);

                var materialesOperario = await _materialRepo.ObtenerHerramientaCotizacionOperario(herramienta.IdPuestoDireccionCotizacion, herramienta.IdCotizacion);

                var materialOperario =
                    materialesOperario.FirstOrDefault(x =>
                    x.ClaveProducto == herramienta.ClaveProducto && x.IdFrecuencia == herramienta.IdFrecuencia && x.IdPuestoDireccionCotizacion == herramienta.IdPuestoDireccionCotizacion && x.IdDireccionCotizacion == herramienta.IdDireccionCotizacion);

                if (materialOperario != null)
                {
                    materialOperario.Cantidad = herramienta.Cantidad;
                    materialOperario.Total = materialOperario.PrecioUnitario * materialOperario.Cantidad;
                    materialOperario.ImporteMensual = materialOperario.Total / (decimal)materialOperario.IdFrecuencia;

                    await _materialRepo.ActualizarHerramientaCotizacion(materialOperario);
                }
                else
                {
                    var preciosBaseProductos = await _materialRepo.ObtenerPreciosBaseProductos("'" + herramienta.ClaveProducto + "'");

                    var idEstado = await _cotizaRepo.ObtenerIdEstadoDeDireccionCotizacion(herramienta.IdDireccionCotizacion);

                    var preciosProductoPorEstado = await _materialRepo.ObtenerPreciosProductosPorEstado("'" + herramienta.ClaveProducto + "'", idEstado);

                    if (preciosProductoPorEstado.Count() > 0)
                    {
                        herramienta.PrecioUnitario = preciosProductoPorEstado.Min(x => x.Precio);
                    }
                    else
                    {
                        herramienta.PrecioUnitario = preciosBaseProductos.FirstOrDefault().Precio;
                    }

                    herramienta.Total = herramienta.PrecioUnitario * herramienta.Cantidad;
                    herramienta.ImporteMensual = herramienta.Total / (decimal)herramienta.IdFrecuencia;
                    herramienta.FechaAlta = DateTime.Now;

                    await _materialRepo.AgregarHerramientaCotizacion(herramienta);
                }
            }
        }

        public async Task ActualizarHerramientaOperario(MaterialCotizacionDTO dto)
        {
            var herramienta = _mapper.Map<MaterialCotizacion>(dto);

            herramienta.Total = (herramienta.PrecioUnitario * herramienta.Cantidad);
            herramienta.ImporteMensual = herramienta.Total / (int)herramienta.IdFrecuencia;

            await _materialRepo.ActualizarHerramientaCotizacion(herramienta);
        }

        public async Task EliminarHerramientaCotizacion(int idHerramienta)
        {
            await _materialRepo.EliminarHerramientaCotizacion(idHerramienta);
        }
    }
}
