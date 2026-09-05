using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Comparison;
using APIFORD.Model;
using APIFORD.Model.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.UserServices;

public class UsuarioHistoricoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    public UsuarioHistoricoService(FordDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // =======================================================================
    // GERENCIAMENTO DE MODELOS (FAVORITOS)
    // =======================================================================

    public async Task<bool> SalvarModeloAsync(string usuarioId, CreateModeloSalvoDTO dto)
    {
        var jaExiste = await _context.ModeloSalvos.AnyAsync(m => m.UserId == usuarioId && m.CarroId == dto.CarroId);

        if (jaExiste) return false;

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

    public async Task<bool> RemoverModeloAsync(string usuarioId, int carroId)
    {
        var modelo = await _context.ModeloSalvos
            .FirstOrDefaultAsync(m => m.UserId == usuarioId && m.CarroId == carroId);

        if (modelo == null) return false;

        _context.ModeloSalvos.Remove(modelo);
        await _context.SaveChangesAsync();
        return true;
    }

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

    public async Task<bool> RemoverComparacaoAsync(string usuarioId, int comparacaoId)
    {
        var comparacao = await _context.ComparacoesSalvas
            .FirstOrDefaultAsync(c => c.UserId == usuarioId && c.Id == comparacaoId);

        if (comparacao == null) return false;

        _context.ComparacoesSalvas.Remove(comparacao);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ReadComparacaoSalvaDTO>> ListarComparacoesSalvasAsync(string usuarioId)
    {
        var historico = await _context.ComparacoesSalvas
            .Where(c => c.UserId == usuarioId)
            .OrderByDescending(c => c.DataSalvamento)
            .ToListAsync();

        return _mapper.Map<List<ReadComparacaoSalvaDTO>>(historico);
    }
}