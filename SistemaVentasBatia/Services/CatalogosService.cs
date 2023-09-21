using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Options;
using Microsoft.Extensions.Options;

namespace SistemaVentasBatia.Services
{
    public interface ICatalogosService
    {
        Task<List<CatalogoDTO>> ObtenerEstados();
        Task<List<CatalogoDTO>> ObtenerMunicipios (int idEstado);
        Task<List<CatalogoDTO>> ObtenerTiposInmueble();
        Task<List<CatalogoDTO>> ObtenerCatalogoPuestos();
        Task<List<CatalogoDTO>> ObtenerCatalogoTurnos();
        Task<List<CatalogoDTO>> ObtenerCatalogoSucursalesCotizacion(int idCotizacion);
        Task<List<CatalogoDTO>> ObtenerCatalogoPuestosCotizacion(int idCotizacion);
        Task<List<CatalogoDTO>> ObtenerCatalogoProductos(Servicio servicio);
        Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoProductosGrupo(Servicio servicio, string grupo);
    }

    public class CatalogosService : ICatalogosService
    {
        private readonly ICatalogosRepository catalogosRepo;
        private readonly IMapper mapper;
        private readonly ProductoOption _option;

        public CatalogosService(ICatalogosRepository catalogosRepo, IMapper mapper, IOptions<ProductoOption> options)
        {
            this.catalogosRepo = catalogosRepo;
            this.mapper = mapper;
            _option = options.Value;
        }

        public async Task<List<CatalogoDTO>> ObtenerEstados()
        {
            var estados = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerEstados());

            return estados;
        }

        public async Task<List<CatalogoDTO>> ObtenerMunicipios(int idEstado)
        {
            var municipios = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerMunicipios(idEstado));

            return municipios;
        }

        public async Task<List<CatalogoDTO>> ObtenerTiposInmueble()
        {
            var tiposInmueble = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerTiposInmueble());

            return tiposInmueble;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoPuestos()
        {
            var puestos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoPuestos());

            return puestos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoTurnos()
        {
            var turnos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoTurnos());

            return turnos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoSucursalesCotizacion(int idCotizacion)
        {
            var sucursales = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoDireccionesCotizacion(idCotizacion));

            return sucursales;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoPuestosCotizacion(int idCotizacion)
        {
            var puestos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoPuestosCotizacion(idCotizacion));

            return puestos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoProductos(Servicio servicio)
        {
            var productos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoProductos(servicio));

            return productos;
        }

        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoProductosGrupo(Servicio servicio, string grupo)
        {
            int[] fams;
            switch (grupo.ToLower())
            {
                case "material":
                    fams = _option.Material;
                    break;
                case "uniforme":
                    fams = _option.Uniforme;
                    break;
                case "equipo":
                    fams = _option.Equipo;
                    break;
                case "herramienta":
                    fams = _option.Herramienta;
                    break;
                default:
                    fams = new int[] {};
                    break;
            }
            return mapper.Map<IEnumerable<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoProductosByFamilia(servicio, fams));
        }
    }
}
