using APIFORD.Data;
using APIFORD.Data.DTOS.Schedule;
using APIFORD.Data.DTOS.Search;
using APIFORD.Middleware;
using APIFORD.Model.Enum;
using APIFORD.Model.Schedule;
using APIFORD.Services.Search;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Schedule;

/// <summary>
/// Serviço responsável pelo gerenciamento de agendamentos de pesquisa de carros, incluindo
/// criação, listagem, cancelamento e o processamento periódico dos agendamentos pendentes
/// (disparando a busca real via <see cref="PesquisaService"/> quando chega a hora agendada).
/// </summary>
public class AgendamentoPesquisaService
{
    private readonly FordDbContext _context;
    private readonly PesquisaService _pesquisaService;
    private readonly IMapper _mapper;
    /// <summary>
    /// Inicializa uma nova instância do <see cref="AgendamentoPesquisaService"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados Ford.</param>
    /// <param name="pesquisaService">Serviço utilizado para disparar a busca de carros quando um agendamento é executado.</param>
    /// <param name="mapper">Mapeador AutoMapper para conversão entre entidades e DTOs.</param>
    public AgendamentoPesquisaService(FordDbContext context, PesquisaService pesquisaService, IMapper mapper)
    {
        _context = context;
        _pesquisaService = pesquisaService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um novo agendamento de pesquisa para o usuário informado, com base na marca,
    /// modelo, ano, data agendada e recorrência desejados.
    /// </summary>
    /// <param name="userId">Id do usuário dono do agendamento.</param>
    /// <param name="dto">Dados do agendamento a ser criado.</param>
    /// <returns>O <see cref="AgendamentoPesquisaDTO"/> referente ao agendamento recém-criado.</returns>
    public async Task<AgendamentoPesquisaDTO> CriarAsync(string userId, CriarAgendamentoDTO dto)
    {
        var agendamento = new AgendamentoPesquisa
        {
            UserId = userId,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Ano = dto.Ano,
            LinhagemId = dto.LinhagemId,
            Notas = dto.Notas,
            ProximaExecucao = dto.DataAgendada,
            Recorrencia = dto.Recorrencia
        };

        await _context.AgendamentosPesquisa.AddAsync(agendamento);
        await _context.SaveChangesAsync();
        return _mapper.Map<AgendamentoPesquisaDTO>(agendamento);
    }


    /// <summary>
    /// Lista todos os agendamentos de pesquisa não cancelados de um usuário, ordenados
    /// pela data da próxima execução.
    /// </summary>
    /// <param name="userId">Id do usuário cujos agendamentos serão listados.</param>
    /// <returns>Lista de <see cref="AgendamentoPesquisaDTO"/> pertencentes ao usuário.</returns>
    public async Task<List<AgendamentoPesquisaDTO>> ListarPorUsuarioAsync(string userId)
    {
        var agendamentos = await _context.AgendamentosPesquisa
            .Where(a => a.UserId == userId && a.Status != StatusAgendamento.Cancelado)
            .OrderBy(a => a.ProximaExecucao)
            .ToListAsync();
        return _mapper.Map<List<AgendamentoPesquisaDTO>>(agendamentos);
    }

    public async Task<Guid> ExecutarAgoraAsync(int agendamentoId, string userId)
    {
        var agendamento = await _context.AgendamentosPesquisa.FirstOrDefaultAsync(a => a.Id == agendamentoId && a.UserId == userId);
        if (agendamento == null) throw new KeyNotFoundException("Agendamento não encontrado.");

        var jobId = await _pesquisaService.BuscarOuIniciarAsync(new BuscaDTO { Brand = agendamento.Marca, Model = agendamento.Modelo, Year = agendamento.Ano });
        agendamento.UltimaExecucao = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return jobId;
    }

    public async Task<AgendamentoPesquisaDTO> AlternarStatusAsync(int agendamentoId, string userId)
    {
        var agendamento = await _context.AgendamentosPesquisa.FirstOrDefaultAsync(a => a.Id == agendamentoId && a.UserId == userId);
        if (agendamento == null) throw new KeyNotFoundException("Agendamento não encontrado.");

        agendamento.Status = agendamento.Status == StatusAgendamento.Ativo ? StatusAgendamento.Pausado : StatusAgendamento.Ativo;
        await _context.SaveChangesAsync();
        return _mapper.Map<AgendamentoPesquisaDTO>(agendamento);
    }


    /// <summary>
    /// Cancela um agendamento de pesquisa específico, alterando seu status para
    /// <see cref="StatusAgendamento.Cancelado"/>.
    /// </summary>
    /// <param name="agendamentoId">Id do agendamento a ser cancelado.</param>
    /// <param name="userId">Id do usuário dono do agendamento, usado para garantir que ele só cancele os próprios agendamentos.</param>
    /// <exception cref="NotFoundException">Lançada quando o agendamento não é encontrado para o usuário informado.</exception>
    public async Task CancelarAsync(int agendamentoId, string userId)
    {
        var agendamento = await _context.AgendamentosPesquisa
            .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.UserId == userId);
        if (agendamento == null) throw new NotFoundException("Agendamento não encontrado.");

        agendamento.Status = StatusAgendamento.Cancelado;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Processa todos os agendamentos ativos cuja próxima execução já chegou, disparando a
    /// busca de carros correspondente para cada um. Agendamentos de recorrência única são
    /// marcados como concluídos após a execução; os demais têm sua próxima execução recalculada
    /// (semanal ou mensal). Método destinado a ser chamado periodicamente por um job/worker.
    /// </summary>
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
                agendamento.ProximaExecucao = agendamento.Recorrencia switch
                {
                    RecorrenciaAgendamento.Diaria => agora.AddDays(1),
                    RecorrenciaAgendamento.Semanal => agora.AddDays(7),
                    _ => agora.AddMonths(1)
                };
            }
        }

        await _context.SaveChangesAsync();
    }
}
