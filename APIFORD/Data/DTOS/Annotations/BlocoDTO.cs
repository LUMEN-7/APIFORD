using APIFORD.Model.Annotation;

namespace APIFORD.Data.DTOS.Annotations;

public class BlocoDTO
{
    public string? Id { get; set; } // null = bloco novo; preenchido = bloco existente sendo mantido/reordenado
    public TipoBloco Tipo { get; set; }
    public string? Texto { get; set; }
    public int? LinhagemIdReferenciado { get; set; }
    public int? ComparacaoIdReferenciada { get; set; }
}

