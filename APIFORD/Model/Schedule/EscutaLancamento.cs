using APIFORD.Model.Enum;
using APIFORD.Model.User;

namespace APIFORD.Model.Schedule;

public class EscutaLancamento
{
    public int Id { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int? Ano { get; set; }

    public Guid? UltimoJobId { get; set; } // job em andamento, se houver
    public DateTime ProximaTentativa { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiraEm { get; set; } // null = nunca expira sozinha

    public StatusAgendamento Status { get; set; } = StatusAgendamento.Ativo;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<EscutaLancamentoUsuario> Usuarios { get; set; } = new List<EscutaLancamentoUsuario>();
}