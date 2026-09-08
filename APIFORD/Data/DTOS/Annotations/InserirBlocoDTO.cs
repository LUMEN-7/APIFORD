using APIFORD.Model.Annotation;
using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Annotations;

/// <summary>Request — insere um novo bloco de conteúdo numa anotação.</summary>
public class InserirBlocoDTO : IValidatableObject
{
    [EnumDataType(typeof(TipoBloco), ErrorMessage = "Tipo de bloco inválido.")]
    public TipoBloco Tipo { get; set; }

    public string? Texto { get; set; }
    public int? LinhagemIdReferenciado { get; set; }
    public int? ComparacaoIdReferenciada { get; set; }
    public int? Posicao { get; set; } // null = insere no final

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Tipo == TipoBloco.CardCarro && LinhagemIdReferenciado == null)
            yield return new ValidationResult("LinhagemIdReferenciado é obrigatório para blocos do tipo CardCarro.", new[] { nameof(LinhagemIdReferenciado) });

        if (Tipo == TipoBloco.CardComparacao && ComparacaoIdReferenciada == null)
            yield return new ValidationResult("ComparacaoIdReferenciada é obrigatório para blocos do tipo CardComparacao.", new[] { nameof(ComparacaoIdReferenciada) });

        if (string.IsNullOrWhiteSpace(Texto) && Tipo != TipoBloco.CardCarro && Tipo != TipoBloco.CardComparacao)
            yield return new ValidationResult("Texto é obrigatório para blocos que não são cards.", new[] { nameof(Texto) });

        if (Posicao.HasValue && Posicao < 0)
            yield return new ValidationResult("Posição não pode ser negativa.", new[] { nameof(Posicao) });
    }
}