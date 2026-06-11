using System.Text.Json.Serialization;

namespace APIFORD.Model.CarroClasses;

public class PropriedadeScraping<T>
{
    
    public double ConfiabilidadeGlobal { get; set; }
    public bool Conflito { get; set; }
    public List<ItemFonteScraping<T>> HistoricoFontes { get; set; } = new();
}

public class ItemFonteScraping<T>
{
    public T Valor { get; set; } = default!;
    public double Confianca { get; set; }
    public string NomeFontePython { get; set; } = string.Empty; // Recebe "icarros.com.br" do Python

    // Campos de auditoria controlados pelo C#
    public int FonteId { get; set; }
    public DateTime DataColeta { get; set; } = DateTime.UtcNow;
    public DateTime? DataReferencia { get; set; }
}