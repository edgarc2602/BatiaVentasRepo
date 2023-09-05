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
    public interface ISalarioService
    {
        Task<int> Create(SalarioDTO dto);
        Task<SalarioDTO> Get(int id);
        Task<SalarioMinDTO> GetFind(int idTabulador, int idPuesto, int idTurno);
        Task<bool> Update(SalarioDTO dto);
        Task<bool> Delete(int id);
    }
    public class SalarioService : ISalarioService
    {
        private readonly ISalarioRepository _repo;
        private readonly IMapper _mapper;

        public SalarioService(ISalarioRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public Task<int> Create(SalarioDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<SalarioDTO> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<SalarioMinDTO> GetFind(int idTabulador, int idPuesto, int idTurno)
        {
            SalarioMinDTO reg;
            IEnumerable<SalarioMinDTO> lf = _mapper.Map<IEnumerable<SalarioMinDTO>>(
                await _repo.Busqueda(idTabulador, idPuesto, idTurno)).ToList();

            reg = lf.FirstOrDefault();
            if (reg == null)
            {
                SalarioMinimo sm = await _repo.ObtenerMinimo(DateTime.Today.Year);
                reg = new SalarioMinDTO
                {
                    SalarioI = sm.SalarioBase * 30.4167m
                };
            }
            return reg;
        }

        public Task<bool> Update(SalarioDTO dto)
        {
            throw new System.NotImplementedException();
        }
    }
}
