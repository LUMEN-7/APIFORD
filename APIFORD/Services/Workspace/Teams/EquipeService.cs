using APIFORD.Data;
using APIFORD.Data.DTOS.Workspace.teams;
using APIFORD.Model.User;
using APIFORD.Model.Workspace.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace APIFORD.Services.Workspace.Teams;

public class EquipeService
{
    private readonly FordDbContext _context;
    private readonly UserManager<User> _userManager;

    public EquipeService(FordDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ReadEquipeDTO> CriarAsync(string userId, CriarEquipeDTO dto)
    {
        var equipe = new Equipe
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            CriadorUserId = userId,
            CodigoConvite = await GerarCodigoConviteUnicoAsync()
        };
        equipe.Membros.Add(new EquipeMembro { UserId = userId, Papel = PapelEquipe.Administrador });
        await _context.Equipes.AddAsync(equipe);
        await _context.SaveChangesAsync();
        return MapearParaDTO(equipe, PapelEquipe.Administrador);
    }

    private async Task<string> GerarCodigoConviteUnicoAsync()
    {
        string codigo;
        do { codigo = $"BCI-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}"; }
        while (await _context.Equipes.AnyAsync(e => e.CodigoConvite == codigo));
        return codigo;
    }

    public async Task<ReadEquipeDTO> EntrarComCodigoAsync(string userId, string codigo)
    {
        var equipe = await _context.Equipes
            .Include(e => e.Membros)
            .FirstOrDefaultAsync(e => e.CodigoConvite == codigo.Trim().ToUpperInvariant())
            ?? throw new KeyNotFoundException("Código de convite inválido.");

        var vinculo = equipe.Membros.FirstOrDefault(m => m.UserId == userId);
        if (vinculo == null)
        {
            vinculo = new EquipeMembro { EquipeId = equipe.Id, UserId = userId, Papel = PapelEquipe.Membro };
            await _context.EquipeMembros.AddAsync(vinculo);
            await _context.SaveChangesAsync();
        }

        return MapearParaDTO(equipe, vinculo.Papel);
    }

    private static ReadEquipeDTO MapearParaDTO(Equipe equipe, PapelEquipe meuPapel) => new()
    {
        Id = equipe.Id,
        Nome = equipe.Nome,
        Descricao = equipe.Descricao,
        CodigoConvite = equipe.CodigoConvite,
        TotalMembros = equipe.Membros.Count,
        MeuPapel = meuPapel
    };

    public async Task<List<ReadEquipeDTO>> ListarMinhasAsync(string userId)
    {
        var vinculos = await _context.EquipeMembros
            .Include(m => m.Equipe).ThenInclude(e => e.Membros)
            .Where(m => m.UserId == userId)
            .ToListAsync();

        return vinculos.Select(v => MapearParaDTO(v.Equipe, v.Papel)).ToList();
    }

    public async Task<ReadEquipeDTO> ObterPorIdAsync(int equipeId, string userId)
    {
        await GarantirMembroAsync(equipeId, userId);

        var equipe = await _context.Equipes
            .Include(e => e.Membros)
            .FirstOrDefaultAsync(e => e.Id == equipeId)
            ?? throw new KeyNotFoundException("Equipe não encontrada.");

        var meuPapel = equipe.Membros.First(m => m.UserId == userId).Papel;
        return MapearParaDTO(equipe, meuPapel);
    }

    public async Task<List<ReadMembroDTO>> ListarMembrosAsync(int equipeId, string userIdSolicitante) // getTeamWorkers
    {
        await GarantirMembroAsync(equipeId, userIdSolicitante);

        var membros = await _context.EquipeMembros.Where(m => m.EquipeId == equipeId).ToListAsync();
        var resultado = new List<ReadMembroDTO>();
        foreach (var m in membros)
        {
            var user = await _userManager.FindByIdAsync(m.UserId);
            var nome = user?.NomeExibicao ?? user?.UserName ?? "Usuário";
            resultado.Add(new ReadMembroDTO { UserId = m.UserId, Nome = nome, Iniciais = GerarIniciais(nome), Papel = m.Papel });
        }
        return resultado;
    }

    public async Task AdicionarMembroAsync(int equipeId, string novoUserId, string userIdSolicitante)
    {
        await GarantirAdministradorAsync(equipeId, userIdSolicitante);
        if (await _context.EquipeMembros.AnyAsync(m => m.EquipeId == equipeId && m.UserId == novoUserId))
            throw new ArgumentException("This user is already on the team.");

        await _context.EquipeMembros.AddAsync(new EquipeMembro { EquipeId = equipeId, UserId = novoUserId });
        await _context.SaveChangesAsync();
    }

    public async Task RemoverMembroAsync(int equipeId, string userIdAlvo, string userIdSolicitante)
    {
        if (userIdAlvo != userIdSolicitante) await GarantirAdministradorAsync(equipeId, userIdSolicitante); // leaving yourself doesn't need admin rights
        var vinculo = await _context.EquipeMembros.FirstOrDefaultAsync(m => m.EquipeId == equipeId && m.UserId == userIdAlvo)
            ?? throw new KeyNotFoundException("Member not found on this team.");
        _context.EquipeMembros.Remove(vinculo);
        await _context.SaveChangesAsync();
    }

    public async Task GarantirMembroAsync(int equipeId, string userId)
    {
        if (!await _context.EquipeMembros.AnyAsync(m => m.EquipeId == equipeId && m.UserId == userId))
            throw new UnauthorizedAccessException("You're not a member of this team.");
    }

    private async Task GarantirAdministradorAsync(int equipeId, string userId)
    {
        var vinculo = await _context.EquipeMembros.FirstOrDefaultAsync(m => m.EquipeId == equipeId && m.UserId == userId);
        if (vinculo == null || vinculo.Papel != PapelEquipe.Administrador)
            throw new UnauthorizedAccessException("Only team admins can do this.");
    }

    private static string GerarIniciais(string nome)
    {
        var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return partes.Length >= 2 ? $"{partes[0][0]}{partes[^1][0]}".ToUpper() : partes.Length == 1 ? partes[0][0].ToString().ToUpper() : "?";
    }
}
