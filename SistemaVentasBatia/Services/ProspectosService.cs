using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Services
{
    public interface IProspectosService
    {
        Task CrearProspecto(ProspectoDTO prospectoVM);
        Task ObtenerListaProspectos(ListaProspectoDTO listaProspectosVM, int autorizacion, int idPersonal);
        Task<ProspectoDTO> ObtenerProspecto(int idProspecto);
        Task EditarProspecto(ProspectoDTO prospectoVM);
        Task ObtenerListaDirecciones(ListaDireccionDTO listaDireccionesVM);
        Task CrearDireccion(DireccionDTO direccionVM);
        Task<List<ProspectoDTO>> ObtenerCatalogoProspectos(int autorizacion, int idPersonal);
        Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion);
        Task<ProspectoDTO> ObtenerProspectoPorCotizacion(int idCotizacion);
        Task EliminarProspecto(int registroAEliminar);
        Task<List<ProspectoDTO>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc);
        Task<int> ObtenerNumeroCoincidenciasProspecto(string nombreComercial, string rfc);
        Task<DireccionDTO> ObtenerDireccionPorId(int id);
        Task ActualizarDireccion(DireccionDTO direccionVM);
        Task<PuestoDireccionCotizacionDTO> ObtenerOperarioPorId(int id);
    }

    public class ProspectosService : IProspectosService
    {
        private readonly IProspectosRepository prospectosRepo;
        private readonly IMapper mapper;

        public ProspectosService(IProspectosRepository prospectosRepo, IMapper mapper)
        {
            this.prospectosRepo = prospectosRepo;
            this.mapper = mapper;
        }

        public async Task CrearProspecto(ProspectoDTO prospectoVM)
        {
            var prospecto = mapper.Map<Prospecto>(prospectoVM);

            prospecto.IdEstatusProspecto = EstatusProspecto.Activo;

            prospecto.FechaAlta = DateTime.Now;

            prospecto.Documentacion = (Documento)prospectoVM.ListaDocumentos.Where(x => x.Act).Sum(x => x.Id);

            await prospectosRepo.InsertarProspecto(prospecto);

            prospectoVM.IdProspecto = prospecto.IdProspecto;
        }

        public async Task ObtenerListaProspectos(ListaProspectoDTO listaProspectosVM, int autorizacion, int idPersonal)
        {
            listaProspectosVM.Rows = await prospectosRepo.ContarProspectos(listaProspectosVM.IdEstatusProspecto, listaProspectosVM.Keywords, idPersonal, autorizacion);

            if (listaProspectosVM.Rows > 0)
            {
                listaProspectosVM.NumPaginas = (listaProspectosVM.Rows / 10);

                if (listaProspectosVM.Rows % 10 > 0)
                {
                    listaProspectosVM.NumPaginas++;
                }

                listaProspectosVM.Prospectos = mapper.Map<List<ProspectoDTO>>(
                    await prospectosRepo.ObtenerProspectos(listaProspectosVM.Pagina, listaProspectosVM.IdEstatusProspecto, listaProspectosVM.Keywords, autorizacion, idPersonal));
            }
            else
            {
                listaProspectosVM.Prospectos = new List<ProspectoDTO>();
            }
        }

        public async Task<ProspectoDTO> ObtenerProspecto(int idProspecto)
        {
            var prospecto = await prospectosRepo.ObtenerProspectoPorId(idProspecto);

            var prospectoVM = mapper.Map<ProspectoDTO>(prospecto);

            prospectoVM.ListaDocumentos = new List<Item<int>>();

            foreach(Documento documento in (Documento[])Enum.GetValues(typeof(Documento)))
            {
                Item<int> doc = new Item<int>() { Id = (int)documento, Nom = documento.ToString(), Act = false };
                if(prospecto.Documentacion.HasFlag(documento))
                {
                    doc.Act = true;
                }
                prospectoVM.ListaDocumentos.Add(doc);
            }

            return prospectoVM;
        }

        public async Task<ProspectoDTO> ObtenerProspectoPorCotizacion(int idCotizacion)
        {
            var prospecto = await prospectosRepo.ObtenerProspectoPorCotizacion(idCotizacion);

            var prospectoVM = mapper.Map<ProspectoDTO>(prospecto);

            prospectoVM.ListaDocumentos = new List<Item<int>>();

            foreach (Documento documento in (Documento[])Enum.GetValues(typeof(Documento)))
            {
                Item<int> doc = new Item<int>() { Id = (int)documento, Nom = documento.ToString(), Act = false };
                if(prospecto.Documentacion.HasFlag(documento))
                {
                    doc.Act = true;
                }
                prospectoVM.ListaDocumentos.Add(doc);
            }

            return prospectoVM;
        }

        public async Task EditarProspecto(ProspectoDTO prospectoVM)
        {
            var prospecto = mapper.Map<Prospecto>(prospectoVM);

            prospecto.Documentacion = (Documento)prospectoVM.ListaDocumentos.Where(x => x.Act).Sum(x => x.Id);

            await prospectosRepo.ActualizarProspecto(prospecto);

        }

        public async Task ObtenerListaDirecciones(ListaDireccionDTO listaDireccionesVM)
        {
            var lista = await prospectosRepo.ObtenerDireccionesPorProspecto(listaDireccionesVM.IdProspecto, listaDireccionesVM.Pagina);
            listaDireccionesVM.Direcciones = lista.Select(d =>
            new DireccionMinDTO
            {
                IdDireccion = d.IdDireccion,
                IdProspecto = d.IdProspecto,
                NombreSucursal = d.NombreSucursal,
                Estado = d.Estado,
                TipoInmueble = d.TipoInmueble,
                DomicilioCompleto = (d.Domicilio ?? "") + ", " + (d.Colonia ?? "") + ", " + (d.Municipio ?? "") + ", " + (d.Ciudad ?? "") + ", " + (d.Estado ?? "") + ", CP " + (d.CodigoPostal),
                IdDireccionCotizacion = d.IdDireccionCotizacion
            }).ToList();
        }

        public async Task CrearDireccion(DireccionDTO direccionVM)
        {
            var direccion = mapper.Map<Direccion>(direccionVM);

            direccion.IdEstatusDireccion = EstatusDireccion.Activo;

            direccion.FechaAlta = DateTime.Now;

            await prospectosRepo.InsertarDireccion(direccion);

            direccionVM.IdDireccion = direccion.IdDireccion;

        }

        public async Task<List<ProspectoDTO>> ObtenerCatalogoProspectos(int autorizacion, int idPersonal)
            {
            var prospectos = mapper.Map<List<ProspectoDTO>>(await prospectosRepo.ObtenerCatalogoProspectos(autorizacion, idPersonal));

            return prospectos;
        }

        public async Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion)
        {
            var idProspecto = await prospectosRepo.ObtenerIdProspectoPorCotizacion(idCotizacion);

            return idProspecto;
        }

        public async Task EliminarProspecto(int registroAEliminar)
        {
            await prospectosRepo.InactivarProspecto(registroAEliminar);
        }

        public async Task<List<ProspectoDTO>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc)
        {
            var prospectos = new List<ProspectoDTO>();

            if (!string.IsNullOrEmpty(nombreComercial) || !string.IsNullOrEmpty(rfc))
            {
                prospectos = mapper.Map<List<ProspectoDTO>>(await prospectosRepo.ObtenerCoincidenciasProspecto(nombreComercial, rfc));
            }

            return prospectos;
        }

        public async Task<int> ObtenerNumeroCoincidenciasProspecto(string nombreComercial, string rfc)
        {
            var numeroProspectos = 0;

            if (!string.IsNullOrEmpty(nombreComercial) || !string.IsNullOrEmpty(rfc))
            {
                numeroProspectos = (await prospectosRepo.ObtenerCoincidenciasProspecto(nombreComercial, rfc)).Count();
            }

            return numeroProspectos;
        }

        public async Task<DireccionDTO> ObtenerDireccionPorId(int id)
        {
            var direccionViewModel = mapper.Map<DireccionDTO>(await prospectosRepo.ObtenerDireccionPorId(id));

            return direccionViewModel;
        }

        public async Task ActualizarDireccion(DireccionDTO direccionVM)
        {
            var direccion = mapper.Map<Direccion>(direccionVM);

            await prospectosRepo.ActualizarDireccion(direccion);
        }

        public async Task<PuestoDireccionCotizacionDTO> ObtenerOperarioPorId(int id)
        {
            var puestoDireccionCotizacionViewModel = mapper.Map<PuestoDireccionCotizacionDTO>(await prospectosRepo.ObtenerPuestoDireccionCotizacionPorId(id));

            return puestoDireccionCotizacionViewModel;
        }
    }
}
