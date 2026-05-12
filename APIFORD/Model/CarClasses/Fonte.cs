using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Model.CarroClasses;

public class Fonte
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Confiabilidade { get; set; }
    public DateTime DataAdicao { get; set; } = DateTime.UtcNow;
    public bool Excluido { get; set; } = false;

    // Tabela que sao conectadas diretamente com fonte
    public virtual ICollection<PneuFonte> PneuFontes { get; set; } = new List<PneuFonte>();
    public virtual ICollection<ExtraFonte> ExtraFontes { get; set; } = new List<ExtraFonte>();
    public virtual ICollection<ConsumoFonte> ConsumoFontes { get; set; } = new List<ConsumoFonte>();
    public virtual ICollection<ModoFonte> ModoFontes { get; set; } = new List<ModoFonte>();
    public virtual ICollection<DimensaoFonte> DimensaoFontes { get; set; } = new List<DimensaoFonte>();
    public virtual ICollection<EspecificacaoFonte> EspecificacaoFontes { get; set; } = new List<EspecificacaoFonte>();

}
