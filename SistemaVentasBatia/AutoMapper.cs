using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia
{  
    public class AutoMapper : Profile  
    {
        public AutoMapper()
        {
            CreateMap<Cotizacion, CotizacionDTO>();
            CreateMap<CotizacionDTO, Cotizacion>();

            CreateMap<Prospecto, ProspectoDTO>();
            CreateMap<ProspectoDTO, Prospecto>();

            CreateMap<Direccion, DireccionDTO>();
            CreateMap<DireccionDTO, Direccion>();

            CreateMap<DireccionCotizacion, DireccionCotizacionDTO>();
            CreateMap<DireccionCotizacionDTO, DireccionCotizacion>();

            CreateMap<Estado, EstadoViewModel>();
            CreateMap<EstadoViewModel, Estado>();

            CreateMap<TipoInmueble, TipoInmuebleViewModel>();
            CreateMap<TipoInmuebleViewModel, TipoInmueble>();

            CreateMap<PuestoDireccionCotizacion, PuestoDireccionCotizacionDTO>();
            CreateMap<PuestoDireccionCotizacionDTO, PuestoDireccionCotizacion>();

            CreateMap<PuestoDireccionCotizacion, PuestoDireccionMinDTO>();

            CreateMap<Puesto, PuestoViewModel>();
            CreateMap<PuestoViewModel, Puesto>();

            CreateMap<Models.Turno, TurnoViewModel>();
            CreateMap<TurnoViewModel, Models.Turno>();

            CreateMap<ResumenCotizacionLimpieza, ResumenCotizacionLimpiezaDTO>();
            CreateMap<ResumenCotizacionLimpiezaDTO, ResumenCotizacionLimpieza>();

            CreateMap<MaterialCotizacion, MaterialCotizacionDTO>();
            CreateMap<MaterialCotizacionDTO, MaterialCotizacion>();
            CreateMap<MaterialCotizacion, MaterialCotizacionMinDTO>();
            CreateMap<MaterialPuesto, MaterialPuestoDTO>();
            CreateMap<MaterialPuestoDTO, MaterialPuesto>();
            CreateMap<ProductoItem, ProductoItemDTO>();
            CreateMap<ProductoItemDTO, ProductoItem>();
            CreateMap<Uniforme, UniformeDTO>();
            CreateMap<UniformeDTO, Uniforme>();

            CreateMap<Catalogo, CatalogoDTO>();
            CreateMap<CatalogoDTO, Catalogo>();

            CreateMap<DiaSemana, string>().ConstructUsing(o => o.ToString());
            CreateMap<Frecuencia, string>().ConstructUsing(o => o.ToString());

            CreateMap<AccesoDTO, Acceso>();
            CreateMap<Acceso, AccesoDTO>();
            CreateMap<UsuarioDTO, Usuario>();
            CreateMap<Usuario, UsuarioDTO>();

            CreateMap<Salario, SalarioDTO>();
            CreateMap<SalarioDTO, Salario>();
            CreateMap<Salario, SalarioMinDTO>();
            CreateMap<Tabulador, TabuladorDTO>();
            CreateMap<TabuladorDTO, Tabulador>();
        }
    }
}  