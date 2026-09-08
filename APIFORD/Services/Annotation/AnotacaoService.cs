using APIFORD.Data;
using APIFORD.Data.DTOS.Annotations;
using APIFORD.Data.DTOS.Annotations.Cards;
using APIFORD.Middleware;
using APIFORD.Model.Annotation;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Annotation;

/// <summary>
/// Regras de negócio do sistema de anotações. Cada anotação é dona de uma lista de blocos ordenados,
/// que podem ser texto livre ou "cards" referenciando um carro (por linhagem, sempre resolvido pra
/// versão mais recente) ou uma comparação salva. Todas as operações são restritas ao dono (userId)
/// da anotação.
/// </summary>
public class AnotacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    public AnotacaoService(FordDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria uma nova anotação vazia para o usuário.
    /// </summary>
    /// <param name="userId">Id do usuário dono da anotação.</param>
    /// <param name="dto">Título e subtítulo da anotação.</param>
    public async Task<ReadAnotacaoDTO> CriarAsync(string userId, CriarAnotacaoDTO dto)
    {
        var anotacao = new Anotacao { UserId = userId, Titulo = dto.Titulo, Subtitulo = dto.Subtitulo };
        await _context.Anotacoes.AddAsync(anotacao);
        await _context.SaveChangesAsync();
        return await ResolverParaLeituraAsync(anotacao);
    }

    /// <summary>
    /// Lista todas as anotações de um usuário, da mais recentemente atualizada para a mais antiga,
    /// com os cards de carro/comparação de cada bloco já resolvidos.
    /// </summary>
    /// <param name="userId">Id do usuário.</param>
    public async Task<List<ReadAnotacaoDTO>> ListarPorUsuarioAsync(string userId)
    {
        var anotacoes = await _context.Anotacoes
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AtualizadoEm)
            .ToListAsync();

        var resultado = new List<ReadAnotacaoDTO>();
        foreach (var a in anotacoes)
            resultado.Add(await ResolverParaLeituraAsync(a));
        return resultado;
    }

    /// <summary>
    /// Mapeia um conjunto de blocos para DTO e resolve, em lote (poucas idas ao banco em vez de uma por bloco),
    /// os cards de preview de carro (sempre a versão mais recente da linhagem) e de comparação salva.
    /// </summary>
    private async Task<List<ReadBlocoDTO>> ResolverBlocosAsync(List<BlocoAnotacao> blocos)
    {
        var dtos = _mapper.Map<List<ReadBlocoDTO>>(blocos);

        var linhagens = blocos.Where(b => b.LinhagemIdReferenciado.HasValue).Select(b => b.LinhagemIdReferenciado!.Value).Distinct().ToList();
        var carrosAtuais = await _context.Carros
            .Where(c => linhagens.Contains(c.LinhagemId))
            .GroupBy(c => c.LinhagemId)
            .Select(g => g.OrderByDescending(c => c.Id).First())
            .ToListAsync();

        var comparacaoIds = blocos.Where(b => b.ComparacaoIdReferenciada.HasValue).Select(b => b.ComparacaoIdReferenciada!.Value).Distinct().ToList();
        var comparacoes = await _context.ComparacoesSalvas.Where(c => comparacaoIds.Contains(c.Id)).ToListAsync();

        foreach (var dto in dtos)
        {
            var origem = blocos.First(b => b.Id == dto.Id);

            if (origem.LinhagemIdReferenciado.HasValue)
            {
                var carro = carrosAtuais.FirstOrDefault(c => c.LinhagemId == origem.LinhagemIdReferenciado);
                if (carro != null)
                    dto.CardCarro = new CardCarroPreviewDTO { LinhagemId = carro.LinhagemId, Marca = carro.Marca, Modelo = carro.Modelo, Ano = carro.Ano };
            }

            if (origem.ComparacaoIdReferenciada.HasValue)
            {
                var comparacao = comparacoes.FirstOrDefault(c => c.Id == origem.ComparacaoIdReferenciada);
                if (comparacao != null)
                    dto.CardComparacao = new CardComparacaoPreviewDTO { ComparacaoId = comparacao.Id, Titulo = comparacao.Titulo ?? "Comparação sem título" };
            }
        }

        return dtos;
    }

    /// <summary>
    /// Insere um novo bloco numa posição específica da anotação, empurrando os blocos seguintes uma posição pra frente.
    /// </summary>
    /// <param name="anotacaoId">Id da anotação.</param>
    /// <param name="userId">Id do usuário dono (garante isolamento entre usuários).</param>
    /// <param name="dto">Dados do novo bloco e a posição de inserção (padrão: no final).</param>
    /// <exception cref="NotFoundException">Anotação não encontrada ou não pertence ao usuário.</exception>
    public async Task<ReadBlocoDTO> InserirBlocoAsync(int anotacaoId, string userId, InserirBlocoDTO dto)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");

        var posicao = Math.Clamp(dto.Posicao ?? anotacao.Blocos.Count, 0, anotacao.Blocos.Count);

        var novoBloco = new BlocoAnotacao
        {
            Tipo = dto.Tipo,
            Texto = dto.Texto,
            LinhagemIdReferenciado = dto.LinhagemIdReferenciado,
            ComparacaoIdReferenciada = dto.ComparacaoIdReferenciada,
            Ordem = posicao
        };

        foreach (var bloco in anotacao.Blocos.Where(b => b.Ordem >= posicao))
            bloco.Ordem++; // abre espaço pros blocos que vêm depois

        anotacao.Blocos.Add(novoBloco);
        anotacao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var resolvido = await ResolverBlocosAsync(new List<BlocoAnotacao> { novoBloco });
        return resolvido[0];
    }

    /// <summary>
    /// Remove um bloco de uma anotação.
    /// </summary>
    /// <param name="anotacaoId">Id da anotação.</param>
    /// <param name="userId">Id do usuário dono.</param>
    /// <param name="blocoId">Id do bloco a remover.</param>
    /// <exception cref="NotFoundException">Anotação ou bloco não encontrado.</exception>
    public async Task RemoverBlocoAsync(int anotacaoId, string userId, string blocoId)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");

        var bloco = anotacao.Blocos.FirstOrDefault(b => b.Id == blocoId);
        if (bloco == null) throw new NotFoundException("Bloco não encontrado.");

        anotacao.Blocos.Remove(bloco); // faltava no original — a checagem existia, mas nada era removido
        anotacao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Substitui todos os blocos da anotação de uma vez, recalculando a ordem pela posição na lista recebida.
    /// Usado pelo front pra reordenar, inserir e remover blocos em uma única chamada (drag-and-drop).
    /// </summary>
    /// <param name="anotacaoId">Id da anotação.</param>
    /// <param name="userId">Id do usuário dono.</param>
    /// <param name="dto">Lista completa de blocos, na ordem final desejada.</param>
    /// <exception cref="NotFoundException">Anotação não encontrada.</exception>
    public async Task<ReadAnotacaoDTO> AtualizarBlocosAsync(int anotacaoId, string userId, AtualizarBlocosDTO dto)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");

        anotacao.Blocos.Clear(); // era: anotacao.Blocos = dto.Blocos.Select(...).ToList();
        var indice = 0;
        foreach (var b in dto.Blocos)
        {
            anotacao.Blocos.Add(new BlocoAnotacao
            {
                Id = b.Id ?? Guid.NewGuid().ToString("N"),
                Tipo = b.Tipo,
                Texto = b.Texto,
                LinhagemIdReferenciado = b.LinhagemIdReferenciado,
                ComparacaoIdReferenciada = b.ComparacaoIdReferenciada,
                Ordem = indice++
            });
        }

        anotacao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return await ResolverParaLeituraAsync(anotacao);
    }

    /// <summary>
    /// Atualiza apenas o texto de um bloco, sem afetar posição ou referência.
    /// </summary>
    /// <param name="anotacaoId">Id da anotação.</param>
    /// <param name="userId">Id do usuário dono.</param>
    /// <param name="blocoId">Id do bloco.</param>
    /// <param name="texto">Novo texto do bloco.</param>
    /// <exception cref="NotFoundException">Anotação ou bloco não encontrado.</exception>
    public async Task AtualizarTextoBlocoAsync(int anotacaoId, string userId, string blocoId, string texto)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");

        var bloco = anotacao.Blocos.FirstOrDefault(b => b.Id == blocoId);
        if (bloco == null) throw new NotFoundException("Bloco não encontrado.");

        bloco.Texto = texto;
        anotacao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync(); // EF detecta a mutação no item da coleção rastreada e reescreve o JSON inteiro — normal desse modelo
    }

    /// <summary>
    /// Troca o veículo (por linhagem) ou a comparação salva referenciada por um bloco do tipo card.
    /// </summary>
    /// <param name="anotacaoId">Id da anotação.</param>
    /// <param name="userId">Id do usuário dono.</param>
    /// <param name="blocoId">Id do bloco — precisa ser CardCarro ou CardComparacao.</param>
    /// <param name="dto">Novo Id de referência.</param>
    /// <exception cref="NotFoundException">Anotação ou bloco não encontrado.</exception>
    /// <exception cref="BadRequestException">O bloco informado não é um card (não tem referência pra atualizar).</exception>
    public async Task<ReadBlocoDTO> AtualizarReferenciaBlocoAsync(int anotacaoId, string userId, string blocoId, AtualizarReferenciaBlocoDTO dto)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");

        var bloco = anotacao.Blocos.FirstOrDefault(b => b.Id == blocoId);
        if (bloco == null) throw new NotFoundException("Bloco não encontrado.");

        if (bloco.Tipo != TipoBloco.CardCarro && bloco.Tipo != TipoBloco.CardComparacao)
            throw new BadRequestException("Esse bloco não é um card — não tem referência pra atualizar.");

        bloco.LinhagemIdReferenciado = bloco.Tipo == TipoBloco.CardCarro ? dto.LinhagemIdReferenciado : null;
        bloco.ComparacaoIdReferenciada = bloco.Tipo == TipoBloco.CardComparacao ? dto.ComparacaoIdReferenciada : null;

        anotacao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var resolvido = await ResolverBlocosAsync(new List<BlocoAnotacao> { bloco });
        return resolvido[0];
    }

    /// <summary>
    /// Exclui uma anotação inteira, com todos os seus blocos.
    /// </summary>
    /// <param name="anotacaoId">Id da anotação.</param>
    /// <param name="userId">Id do usuário dono.</param>
    /// <exception cref="NotFoundException">Anotação não encontrada.</exception>
    public async Task ExcluirAsync(int anotacaoId, string userId)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");
        _context.Anotacoes.Remove(anotacao);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Monta o DTO de leitura de uma anotação, ordenando os blocos e resolvendo os cards de
    /// carro/comparação de todos eles em lote.
    /// </summary>
    private async Task<ReadAnotacaoDTO> ResolverParaLeituraAsync(Anotacao anotacao)
    {
        anotacao.Blocos = anotacao.Blocos.OrderBy(b => b.Ordem).ToList();
        var dto = _mapper.Map<ReadAnotacaoDTO>(anotacao);
        dto.Blocos = await ResolverBlocosAsync(anotacao.Blocos);
        return dto;
    }
}