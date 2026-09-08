using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Export;

/// <summary>Request — um item da lista de veículos a exportar.</summary>
public class ItemExportacaoDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "LinhagemId inválido.")]
    public int LinhagemId { get; set; }

    public int? CarroId { get; set; } // se vier preenchido, usa essa versão específica; se não vier, pega a mais recente da linhagem
}