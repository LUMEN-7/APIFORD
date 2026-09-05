using APIFORD.Data;
using APIFORD.Data.DTOS.Schedule;
using APIFORD.Data.DTOS.Search;
using APIFORD.Model.Enum;
using APIFORD.Model.Schedule;
using APIFORD.Services.Search;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Schedule;

public class AgendamentoPesquisaService
{
    private readonly FordDbContext _context;
    private readonly PesquisaService _pesquisaService;
    private readonly IMapper _mapper;
    public AgendamentoPesquisaService(FordDbContext context, PesquisaService pesquisaService, IMapper mapper)
    {
        _context = context;
        _pesquisaService = pesquisaService;
        _mapper = mapper;
    }

    public async Task<AgendamentoPesquisaDTO> CriarAsync(string userId, CriarAgendamentoDTO dto)
    {
        var agendamento = new AgendamentoPesquisa
        {
            UserId = userId,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Ano = dto.Ano,
            ProximaExecucao = dto.DataAgendada,
            Recorrencia = dto.Recorrencia
        };

        await _context.AgendamentosPesquisa.AddAsync(agendamento);
        await _context.SaveChangesAsync();
        return _mapper.Map<AgendamentoPesquisaDTO>(agendamento);
    }

    public async Task<List<AgendamentoPesquisaDTO>> ListarPorUsuarioAsync(string userId)
    {
        var agendamentos = await _context.AgendamentosPesquisa
            .Where(a => a.UserId == userId && a.Status != StatusAgendamento.Cancelado)
            .OrderBy(a => a.ProximaExecucao)
            .ToListAsync();
        return _mapper.Map<List<AgendamentoPesquisaDTO>>(agendamentos);
    }
    public async Task CancelarAsync(int agendamentoId, string userId)
    {
        var agendamento = await _context.AgendamentosPesquisa
            .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.UserId == userId);
        if (agendamento == null) throw new KeyNotFoundException("Agendamento não encontrado.");

        agendamento.Status = StatusAgendamento.Cancelado;
        await _context.SaveChangesAsync();
    }

    public async Task ProcessarPendentesAsync()
    {
        var agora = DateTime.UtcNow;

        var pendentes = await _context.AgendamentosPesquisa
            .Where(a => a.Status == StatusAgendamento.Ativo && a.ProximaExecucao <= agora)
            .ToListAsync();

        foreach (var agendamento in pendentes)
        {
            await _pesquisaService.IniciarBusca(new BuscaDTO
            {
                Brand = agendamento.Marca,
                Model = agendamento.Modelo,
                Year = agendamento.Ano
            });

            agendamento.UltimaExecucao = agora;

            if (agendamento.Recorrencia == RecorrenciaAgendamento.Unica)
            {
                agendamento.Status = StatusAgendamento.Concluido;
            }
            else
            {
                agendamento.ProximaExecucao = agendamento.Recorrencia == RecorrenciaAgendamento.Semanal
                    ? agora.AddDays(7)
                    : agora.AddMonths(1);
            }
        }

        await _context.SaveChangesAsync();
    }
}
