using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Comparison;
using APIFORD.Middleware;
using APIFORD.Model;
using APIFORD.Model.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.UserServices;

/// <summary>
/// Serviço responsável pelo gerenciamento do histórico pessoal do usuário, contemplando o
/// cadastro/remoção de modelos de carro favoritados e o salvamento/remoção de comparações
/// (grupo ou direta) realizadas anteriormente, para consulta futura.
/// </summary>
public class UsuarioHistoricoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="UsuarioHistoricoService"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados Ford.</param>
    /// <param name="mapper">Mapeador AutoMapper para conversão entre entidades e DTOs.</param>
    public UsuarioHistoricoService(FordDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // =======================================================================
    // GERENCIAMENTO DE MODELOS (FAVORITOS)
    // =======================================================================
    
    /// <summary>
    /// Favorita (salva) um carro para o usuário informado, impedindo duplicidade caso o
    /// mesmo carro já tenha sido favoritado anteriormente por ele.
    /// </summary>
    /// <param name="usuarioId">Id do usuário que está favoritando o carro.</param>
    /// <param name="dto">Dados do modelo a ser salvo, incluindo o Id do carro.</param>
    /// <returns><c>true</c> quando o modelo é salvo com sucesso.</returns>
    /// <exception cref="BadRequestException">Lançada quando o carro já havia sido favoritado por esse usuário.</exception>
    public async Task<bool> SalvarModeloAsync(string usuarioId, CreateModeloSalvoDTO dto)
    {
        var jaExiste = await _context.ModeloSalvos.AnyAsync(m => m.UserId == usuarioId && m.CarroId == dto.CarroId);

        if (jaExiste) throw new BadRequestException("Modelo já salvo.");

        var novoModelo = new ModeloSalvo
        {
            UserId = usuarioId,
            CarroId = dto.CarroId,
            DataSalvo = DateTime.UtcNow
        };

        _context.ModeloSalvos.Add(novoModelo);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Remove um carro previamente favoritado por um usuário.
    /// </summary>
    /// <param name="usuarioId">Id do usuário dono do favorito.</param>
    /// <param name="carroId">Id do carro a ser removido dos favoritos.</param>
    /// <returns><c>true</c> quando o modelo é removido com sucesso.</returns>
    /// <exception cref="NotFoundException">Lançada quando o carro não está favoritado por esse usuário.</exception>
    public async Task<bool> RemoverModeloAsync(string usuarioId, int carroId)
    {
        var modelo = await _context.ModeloSalvos
            .FirstOrDefaultAsync(m => m.UserId == usuarioId && m.CarroId == carroId);

        if (modelo == null) throw new NotFoundException("Modelo não encontrado.");

        _context.ModeloSalvos.Remove(modelo);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Lista todos os carros favoritados por um usuário.
    /// </summary>
    /// <param name="usuarioId">Id do usuário cujos favoritos serão listados.</param>
    /// <returns>Lista de <see cref="ReadCarroDTO"/> representando os carros favoritados pelo usuário.</returns>
    public async Task<List<ReadCarroDTO>> ListarModeloSalvosAsync(string usuarioId)
    {
        var carros = await _context.ModeloSalvos
            .Where(m => m.UserId == usuarioId)
            .Include(m => m.Carro) // Traz o carro do banco
            .Select(m => m.Carro)  // Extrai apenas a entidade Carro
            .ToListAsync();

        return _mapper.Map<List<ReadCarroDTO>>(carros);
    }

    // =======================================================================
    // GERENCIAMENTO DE COMPARAÇÕES (HISTÓRICO)
    // =======================================================================

    /// <summary>
    /// Salva uma comparação (grupo ou direta) no histórico do usuário, persistindo o payload
    /// original da requisição para possível reexecução ou consulta futura.
    /// </summary>
    /// <param name="usuarioId">Id do usuário dono da comparação.</param>
    /// <param name="dto">Dados da comparação a ser salva: título, tipo e payload da requisição original.</param>
    /// <returns>O <see cref="ReadComparacaoSalvaDTO"/> referente à comparação recém-salva.</returns>
    public async Task<ReadComparacaoSalvaDTO> SalvarComparacaoAsync(string usuarioId, SalvarComparacaoDTO dto)
    {
        var novaComparacao = new ComparacaoSalva
        {
            UserId = usuarioId,
            Titulo = dto.Titulo,
            Tipo = dto.Tipo,
            RequestPayload = dto.RequestPayload,
            DataSalvamento = DateTime.UtcNow
        };

        _context.ComparacoesSalvas.Add(novaComparacao);
        await _context.SaveChangesAsync();

        return _mapper.Map<ReadComparacaoSalvaDTO>(novaComparacao);
    }

    /// <summary>
    /// Remove uma comparação previamente salva no histórico de um usuário.
    /// </summary>
    /// <param name="usuarioId">Id do usuário dono da comparação.</param>
    /// <param name="comparacaoId">Id da comparação a ser removida.</param>
    /// <returns><c>true</c> quando a comparação é removida com sucesso.</returns>
    /// <exception cref="NotFoundException">Lançada quando a comparação não é encontrada para o usuário informado.</exception>
    public async Task<bool> RemoverComparacaoAsync(string usuarioId, int comparacaoId)
    {
        var comparacao = await _context.ComparacoesSalvas
            .FirstOrDefaultAsync(c => c.UserId == usuarioId && c.Id == comparacaoId);

        if (comparacao == null) throw new NotFoundException("Comparação não encontrada.");

        _context.ComparacoesSalvas.Remove(comparacao);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Lista o histórico de comparações salvas de um usuário, ordenadas da mais recente para a mais antiga.
    /// </summary>
    /// <param name="usuarioId">Id do usuário cujo histórico de comparações será listado.</param>
    /// <returns>Lista de <see cref="ReadComparacaoSalvaDTO"/> representando as comparações salvas do usuário.</returns>
    public async Task<List<ReadComparacaoSalvaDTO>> ListarComparacoesSalvasAsync(string usuarioId)
    {
        var historico = await _context.ComparacoesSalvas
            .Where(c => c.UserId == usuarioId)
            .OrderByDescending(c => c.DataSalvamento)
            .ToListAsync();

        return _mapper.Map<List<ReadComparacaoSalvaDTO>>(historico);
    }
}