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
    public interface IProductoService
    {
        Task CreateMaterial(MaterialPuestoDTO mat);
        Task CreateUniforme(MaterialPuestoDTO uni);
        Task CreateHerramienta(MaterialPuestoDTO her);
        Task CreateEquipo(MaterialPuestoDTO equi);
        Task<bool> DeleteMaterial(int id);
        Task<bool> DeleteUniforme(int id);
        Task<bool> DeleteHerramienta(int id);
        Task<bool> DeleteEquipo(int id);

        Task<IEnumerable<ProductoItemDTO>> GetMaterialPuesto(int idPuesto);
        Task<IEnumerable<ProductoItemDTO>> GetHerramientaPuesto(int idPuesto);
        Task<IEnumerable<ProductoItemDTO>> GetUniformePuesto(int idPuesto);
        Task<IEnumerable<ProductoItemDTO>> GetEquipoPuesto(int idPuesto);
    }

    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository repo;
        private readonly IMapper mapper;

        public ProductoService(IProductoRepository repository, IMapper imapper)
        {
            repo = repository;
            mapper = imapper;
        }

        public async Task CreateEquipo(MaterialPuestoDTO equi)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(equi);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarEquipoPuesto(model);
        }

        public async Task CreateHerramienta(MaterialPuestoDTO her)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(her);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarHerramientaPuesto(model);
        }

        public async Task CreateMaterial(MaterialPuestoDTO mat)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(mat);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarMaterialPuesto(model);
        }

        public async Task CreateUniforme(MaterialPuestoDTO uni)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(uni);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarUniformePuesto(model);
        }

        public async Task<bool> DeleteEquipo(int id)
        {
            return await repo.EliminarEquipoPuesto(id);
        }

        public async Task<bool> DeleteHerramienta(int id)
        {
            return await repo.EliminarHerramientaPuesto(id);
        }

        public async Task<bool> DeleteMaterial(int id)
        {
            return await repo.EliminarMaterialPuesto(id);
        }

        public async Task<bool> DeleteUniforme(int id)
        {
            return await repo.EliminarUniformePuesto(id);
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetEquipoPuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerEquipoPuesto(idPuesto));
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetHerramientaPuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerHerramientaPuesto(idPuesto));
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetMaterialPuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerMaterialPuesto(idPuesto));
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetUniformePuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerUniformePuesto(idPuesto));
        }
    }
}