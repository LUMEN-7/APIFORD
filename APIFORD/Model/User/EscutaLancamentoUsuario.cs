using APIFORD.Model.Schedule;

namespace APIFORD.Model.User;

public class EscutaLancamentoUsuario
{
    public int Id { get; set; }
    public int EscutaLancamentoId { get; set; }
    public EscutaLancamento EscutaLancamento { get; set; }
    public string UserId { get; set; }
}
