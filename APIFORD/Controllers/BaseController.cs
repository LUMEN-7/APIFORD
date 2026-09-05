using APIFORD.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> : ControllerBase
    where TEntity : class
{
    protected readonly IBaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> Service;

    protected BaseController(IBaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> service)
    {
        Service = service;
    }

    /// <summary>
    /// Cria e registra um novo registro no sistema.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/Carro/registrar
    ///     {
    ///        "model": "Mustang",
    ///        "brand": "Ford",
    ///        "year": 2026
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">Os dados necessários para a criação da entidade.</param>
    /// <returns>O registro recém-criado convertido para o DTO de leitura.</returns>
    /// <response code="200">Retorna o objeto criado com sucesso.</response>
    /// <response code="400">Os dados enviados no DTO são inválidos ou estão incompletos.</response>
    [HttpPost("registrar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Create([FromBody] TCreateDTO dto)
    {
        var result = await Service.CreateAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Recupera todos os registros cadastrados no sistema.
    /// </summary>
    /// <returns>Uma coleção de registros convertidos para DTO de leitura.</returns>
    /// <response code="200">Retorna a lista de registros cadastrados.</response>
    [HttpGet("listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll()
    {
        return Ok(await Service.GetAllAsync());
    }

    /// <summary>
    /// Recupera um registro específico por meio da sua chave pAroária (ID).
    /// </summary>
    /// <param name="id">A chave pAroária do registro procurado.</param>
    /// <returns>Os dados do registro correspondente ao ID informado.</returns>
    /// <response code="200">Retorna o registro encontrado com sucesso.</response>
    /// <response code="404">Nenhum registro foi encontrado com o ID informado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> GetById(TKey id)
    {
        var result = await Service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Atualiza as informações de um registro existente.
    /// </summary>
    /// <param name="id">A chave pAroária do registro a ser atualizado.</param>
    /// <param name="dto">Os novos dados para atualização do registro.</param>
    /// <returns>O registro atualizado com sucesso.</returns>
    /// <response code="200">O registro foi atualizado e salvo com sucesso.</response>
    /// <response code="400">O DTO enviado possui dados inválidos.</response>
    /// <response code="404">Nenhum registro encontrado para o ID informado.</response>
    [HttpPut("atualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Update(TKey id, [FromBody] TUpdateDTO dto)
    {
        var result = await Service.UpdateAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Inativa logicamente um registro mudando o estado do campo "IsDeleted" para true (Soft Delete).
    /// </summary>
    /// <param name="id">A chave pAroária do registro a ser desativado.</param>
    /// <returns>Uma mensagem confirmando a inativação.</returns>
    /// <response code="200">Registro desativado com sucesso.</response>
    /// <response code="404">Nenhum registro encontrado com o ID informado.</response>
    [HttpDelete("deletar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> SoftDelete(TKey id)
    {
        await Service.SoftDeleteAsync(id);
        return Ok("Registro removido com sucesso.");
    }

    /// <summary>
    /// Executa a anonimização de um registro alterando strings para "Unknown" e zerando números.
    /// </summary>
    /// <param name="id">A chave pAroária do registro a ser anonimizado.</param>
    /// <returns>Confirmação de que o registro foi anonimizado.</returns>
    /// <response code="200">Os dados foram anonimizados com sucesso.</response>
    /// <response code="404">Nenhum registro encontrado com o ID informado.</response>
    [HttpDelete("anonimizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Anonymize(TKey id)
    {
        await Service.AnonymizeAsync(id);
        return Ok("Registro anonimizado com sucesso.");
    }
}