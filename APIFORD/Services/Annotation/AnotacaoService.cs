using APIFORD.Data;
using APIFORD.Data.DTOS.Annotations;
using APIFORD.Data.DTOS.Annotations.Cards;
using APIFORD.Middleware;
using APIFORD.Model.Annotation;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Annotation;

public class AnotacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    public AnotacaoService(FordDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ReadAnotacaoDTO> CriarAsync(string userId, CriarAnotacaoDTO dto)
    {
        var anotacao = new Anotacao { UserId = userId, Titulo = dto.Titulo, Subtitulo = dto.Subtitulo };
        await _context.Anotacoes.AddAsync(anotacao);
        await _context.SaveChangesAsync();
        return await ResolverParaLeituraAsync(anotacao);
    }

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

    public async Task<ReadAnotacaoDTO> AtualizarBlocosAsync(int anotacaoId, string userId, AtualizarBlocosDTO dto)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");

        anotacao.Blocos = dto.Blocos.Select((b, indice) => new BlocoAnotacao
        {
            Id = b.Id ?? Guid.NewGuid().ToString("N"),
            Tipo = b.Tipo,
            Texto = b.Texto,
            LinhagemIdReferenciado = b.LinhagemIdReferenciado,
            ComparacaoIdReferenciada = b.ComparacaoIdReferenciada,
            Ordem = indice
        }).ToList();

        anotacao.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return await ResolverParaLeituraAsync(anotacao);
    }

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

    public async Task ExcluirAsync(int anotacaoId, string userId)
    {
        var anotacao = await _context.Anotacoes.FirstOrDefaultAsync(a => a.Id == anotacaoId && a.UserId == userId);
        if (anotacao == null) throw new NotFoundException("Anotação não encontrada.");
        _context.Anotacoes.Remove(anotacao);
        await _context.SaveChangesAsync();
    }

    private async Task<ReadAnotacaoDTO> ResolverParaLeituraAsync(Anotacao anotacao)
    {
        anotacao.Blocos = anotacao.Blocos.OrderBy(b => b.Ordem).ToList();
        var dto = _mapper.Map<ReadAnotacaoDTO>(anotacao);

        var linhagens = anotacao.Blocos.Where(b => b.LinhagemIdReferenciado.HasValue)
            .Select(b => b.LinhagemIdReferenciado!.Value).Distinct().ToList();
        var carrosAtuais = await _context.Carros
            .Where(c => linhagens.Contains(c.LinhagemId))
            .GroupBy(c => c.LinhagemId)
            .Select(g => g.OrderByDescending(c => c.Id).First())
            .ToListAsync();

        var comparacaoIds = anotacao.Blocos.Where(b => b.ComparacaoIdReferenciada.HasValue)
            .Select(b => b.ComparacaoIdReferenciada!.Value).Distinct().ToList();
        var comparacoes = await _context.ComparacoesSalvas.Where(c => comparacaoIds.Contains(c.Id)).ToListAsync();

        foreach (var bloco in dto.Blocos)
        {
            var origem = anotacao.Blocos.First(b => b.Id == bloco.Id);

            if (origem.LinhagemIdReferenciado.HasValue)
            {
                var carro = carrosAtuais.FirstOrDefault(c => c.LinhagemId == origem.LinhagemIdReferenciado);
                if (carro != null)
                    bloco.CardCarro = new CardCarroPreviewDTO { LinhagemId = carro.LinhagemId, Marca = carro.Marca, Modelo = carro.Modelo, Ano = carro.Ano };
            }

            if (origem.ComparacaoIdReferenciada.HasValue)
            {
                var comparacao = comparacoes.FirstOrDefault(c => c.Id == origem.ComparacaoIdReferenciada);
                if (comparacao != null)
                    bloco.CardComparacao = new CardComparacaoPreviewDTO { ComparacaoId = comparacao.Id, Titulo = comparacao.Titulo ?? "Comparação sem título" };
            }
        }

        return dto;
    }
}