using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Annotations;

/// <summary>Request — substitui todos os blocos de uma anotação de uma vez.</summary>
public class AtualizarBlocosDTO
{
    [Required(ErrorMessage = "A lista de blocos é obrigatória.")]
    public List<BlocoDTO> Blocos { get; set; } = new(); // lista vazia é válida — limpa a anotação
}

/// <summary>Request — atualiza só o texto de um bloco.</summary>
public class AtualizarTextoBlocoDTO
{
    [Required(ErrorMessage = "O texto é obrigatório.")]
    [StringLength(5000, ErrorMessage = "O texto deve ter no máximo 5000 caracteres.")]
    public string Texto { get; set; } = string.Empty;
}

/// <summary>Request — troca a referência (carro ou comparação) de um bloco do tipo card.</summary>
public class AtualizarReferenciaBlocoDTO : IValidatableObject
{
    public int? LinhagemIdReferenciado { get; set; }
    public int? ComparacaoIdReferenciada { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LinhagemIdReferenciado == null && ComparacaoIdReferenciada == null)
            yield return new ValidationResult(
                "Informe LinhagemIdReferenciado ou ComparacaoIdReferenciada.",
                new[] { nameof(LinhagemIdReferenciado), nameof(ComparacaoIdReferenciada) });
    }
}

/// <summary>Response — URL final da imagem depois de salva no armazenamento.</summary>
public class ImagemAnotacaoDTO
{
    public string Url { get; set; } = string.Empty;
}