using APIFORD.Data;
using APIFORD.Data.DTOS.Workspace;
using APIFORD.Model.User;
using APIFORD.Model.Workspace;
using APIFORD.Model.Workspace.enums;
using APIFORD.Services.Workspace.Teams;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Workspace;

public class WorkspaceService
{
    private readonly FordDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly EquipeService _equipeService;

    public WorkspaceService(FordDbContext context, UserManager<User> userManager, EquipeService equipeService)
    {
        _context = context;
        _userManager = userManager;
        _equipeService = equipeService;
    }

    private static readonly TipoPost[] TiposComStatus = { TipoPost.Revisao};

    public async Task<ReadPostDTO> CriarPostAsync(int equipeId, string userId, CriarPostDTO dto)
    {
        await _equipeService.GarantirMembroAsync(equipeId, userId);
        bool temStatus = TiposComStatus.Contains(dto.Tipo);

        var post = new WorkspacePost
        {
            EquipeId = equipeId,
            AutorUserId = userId,
            Tipo = dto.Tipo,
            Conteudo = dto.Conteudo,
            Tags = dto.Tags,
            ResponsavelUserId = temStatus ? dto.ResponsavelUserId : null,
            Status = temStatus ? (dto.Status ?? StatusAtividade.Pendente) : null,
            TipoConteudoVinculado = dto.TipoConteudoVinculado,
            ConteudoVinculadoId = dto.ConteudoVinculadoId,
            ConteudoVinculadoTitulo = dto.ConteudoVinculadoTitulo,
        };

        await _context.WorkspacePosts.AddAsync(post);
        await _context.SaveChangesAsync();
            var  postR = await ResolverPostAsync(post, userId, new Dictionary<string, AutorResumoDTO>());

        await RegistrarAtividadeAsync(equipeId, TipoAtividade.PostCriado, post.Id, userId);

        if (postR.Responsavel?.UserId != null)
            await RegistrarAtividadeAsync(equipeId, TipoAtividade.Atribuicao, postR.Id, userId, alvoUserId: postR.Responsavel.UserId);
        return postR;
    }

    private async Task RegistrarAtividadeAsync(int equipeId, TipoAtividade tipo, int? postId, string atorUserId, string? alvoUserId = null, string? statusNovo = null)
    {
        await _context.AtividadesWorkspace.AddAsync(new AtividadeWorkspace
        {
            EquipeId = equipeId,
            Tipo = tipo,
            PostId = postId,
            AtorUserId = atorUserId,
            AlvoUserId = alvoUserId,
            StatusNovo = statusNovo,
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<ReadPostDTO>> ListarAsync(int equipeId, string userIdAtual)
    {
        await _equipeService.GarantirMembroAsync(equipeId, userIdAtual);

        var posts = await _context.WorkspacePosts
            .Include(p => p.Comentarios)
            .Include(p => p.Curtidas)
            .Where(p => p.EquipeId == equipeId)
            .OrderByDescending(p => p.Fixado)
            .ThenByDescending(p => p.CriadoEm)
            .ToListAsync();

        var cacheAutores = new Dictionary<string, AutorResumoDTO>();
        var resultado = new List<ReadPostDTO>();
        foreach (var post in posts)
            resultado.Add(await ResolverPostAsync(post, userIdAtual, cacheAutores));
        return resultado;
    }

    public async Task<List<ReadAtividadeDTO>> ListarAtividadesAsync(int equipeId, string userIdSolicitante, int limite = 20)
    {
        await _equipeService.GarantirMembroAsync(equipeId, userIdSolicitante);

        var atividades = await _context.AtividadesWorkspace
            .Where(a => a.EquipeId == equipeId)
            .OrderByDescending(a => a.DataCriacao)
            .Take(limite)
            .ToListAsync();

        var resultado = new List<ReadAtividadeDTO>();
        foreach (var a in atividades)
        {
            var ator = await _userManager.FindByIdAsync(a.AtorUserId);
            var alvo = a.AlvoUserId != null ? await _userManager.FindByIdAsync(a.AlvoUserId) : null;
            var post = await _context.WorkspacePosts.FindAsync(a.PostId);

            resultado.Add(new ReadAtividadeDTO
            {
                Id = a.Id,
                Tipo = a.Tipo,
                AtorNome = ator?.NomeExibicao ?? ator?.UserName ?? "Usuário",
                AlvoNome = alvo?.NomeExibicao ?? alvo?.UserName,
                PostId = a.PostId,
                PostConteudoResumo = post?.Conteudo?.Length > 60 ? post.Conteudo[..60] + "..." : post?.Conteudo,
                StatusNovo = a.StatusNovo,
                DataCriacao = a.DataCriacao,
            });
        }
        return resultado;
    }

    public async Task<ReadComentarioDTO> ComentarAsync(int postId, string userId, string conteudo)
    {
        var post = await _context.WorkspacePosts.FindAsync(postId) ?? throw new KeyNotFoundException("Post not found.");
        await _equipeService.GarantirMembroAsync(post.EquipeId, userId);

        var comentario = new WorkspaceComentario { WorkspacePostId = postId, AutorUserId = userId, Conteudo = conteudo };
        await _context.WorkspaceComentarios.AddAsync(comentario);
        await _context.SaveChangesAsync();

        await RegistrarAtividadeAsync(post.EquipeId, TipoAtividade.Comentario, postId, userId); // novo

        var autor = await ResolverAutorAsync(userId, new Dictionary<string, AutorResumoDTO>());
        return new ReadComentarioDTO { Id = comentario.Id, Autor = autor, Conteudo = comentario.Conteudo, CriadoEm = comentario.CriadoEm };
    }

    public async Task<int> ToggleCurtidaAsync(int postId, string userId)
    {
        var post = await _context.WorkspacePosts.FindAsync(postId) ?? throw new KeyNotFoundException("Post not found.");
        await _equipeService.GarantirMembroAsync(post.EquipeId, userId);

        var existente = await _context.WorkspaceCurtidas.FirstOrDefaultAsync(c => c.WorkspacePostId == postId && c.UserId == userId);
        if (existente != null) _context.WorkspaceCurtidas.Remove(existente);
        else await _context.WorkspaceCurtidas.AddAsync(new WorkspaceCurtida { WorkspacePostId = postId, UserId = userId });

        await _context.SaveChangesAsync();
        return await _context.WorkspaceCurtidas.CountAsync(c => c.WorkspacePostId == postId);
    }

    public async Task TogglePinAsync(int postId, string userId)
    {
        var post = await _context.WorkspacePosts.FindAsync(postId) ?? throw new KeyNotFoundException("Post not found.");
        await _equipeService.GarantirMembroAsync(post.EquipeId, userId);

        post.Fixado = !post.Fixado;
        await _context.SaveChangesAsync();

        await RegistrarAtividadeAsync(post.EquipeId, post.Fixado ? TipoAtividade.PostFixado : TipoAtividade.PostDesafixado, postId, userId); // novo
    }

    public async Task AtualizarStatusAsync(int postId, StatusAtividade status, string userId)
    {
        var post = await _context.WorkspacePosts.FindAsync(postId) ?? throw new KeyNotFoundException("Post not found.");
        await _equipeService.GarantirMembroAsync(post.EquipeId, userId);
        if (!TiposComStatus.Contains(post.Tipo))
            throw new ArgumentException("Only Revisao posts have a status.");

        post.Status = status;
        post.ConcluidoEm = status == StatusAtividade.Resolvido ? DateTime.UtcNow : null; // limpa se reabrir
        await RegistrarAtividadeAsync(post.EquipeId, TipoAtividade.StatusAlterado, post.Id, userId, statusNovo: status.ToString());
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(int postId, string userId)
    {
        var post = await _context.WorkspacePosts.FirstOrDefaultAsync(p => p.Id == postId && p.AutorUserId == userId)
            ?? throw new KeyNotFoundException("Post not found, or you're not its author.");
        _context.WorkspacePosts.Remove(post);
        await _context.SaveChangesAsync();
    }

    private async Task<AutorResumoDTO> ResolverAutorAsync(string userId, Dictionary<string, AutorResumoDTO> cache)
    {
        if (cache.TryGetValue(userId, out var existente)) return existente;

        var user = await _userManager.FindByIdAsync(userId);
        var nome = user?.NomeExibicao ?? user?.UserName ?? "User";
        var resumo = new AutorResumoDTO { UserId = userId, Nome = nome, Iniciais = GerarIniciais(nome) };
        cache[userId] = resumo;
        return resumo;
    }

    private static string GerarIniciais(string nome)
    {
        var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return partes.Length >= 2 ? $"{partes[0][0]}{partes[^1][0]}".ToUpper()
            : partes.Length == 1 ? partes[0][0].ToString().ToUpper() : "?";
    }

    private async Task<ReadPostDTO> ResolverPostAsync(WorkspacePost post, string userIdAtual, Dictionary<string, AutorResumoDTO> cache)
    {
        var autor = await ResolverAutorAsync(post.AutorUserId, cache);
        var responsavel = post.ResponsavelUserId != null ? await ResolverAutorAsync(post.ResponsavelUserId, cache) : null;

        var comentarios = new List<ReadComentarioDTO>();
        foreach (var c in post.Comentarios.OrderBy(c => c.CriadoEm))
            comentarios.Add(new ReadComentarioDTO { Id = c.Id, Autor = await ResolverAutorAsync(c.AutorUserId, cache), Conteudo = c.Conteudo, CriadoEm = c.CriadoEm });

        return new ReadPostDTO
        {
            Id = post.Id,
            Autor = autor,
            Tipo = post.Tipo,
            Conteudo = post.Conteudo,
            Tags = post.Tags,
            Responsavel = responsavel,
            Status = post.Status,
            TipoConteudoVinculado = post.TipoConteudoVinculado,
            ConteudoVinculadoId = post.ConteudoVinculadoId,
            ConteudoVinculadoTitulo = post.ConteudoVinculadoTitulo,
            Fixado = post.Fixado,
            CriadoEm = post.CriadoEm,
            TotalCurtidas = post.Curtidas.Count,
            CurtidoPeloUsuarioAtual = post.Curtidas.Any(c => c.UserId == userIdAtual),
            Comentarios = comentarios,
            ConcluidoEm = post.ConcluidoEm,
        };
    }
}