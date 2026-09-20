using APIFORD.Model.CarroClasses;

namespace APIFORD.Model.User;

public class ModeloSalvo
{
    // Chave Estrangeira e parte da Chave Primária
    public string UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public DateTime DataSalvo { get; set; } = DateTime.UtcNow;

    // Chave Estrangeira e parte da Chave Primária
    public int CarroId { get; set; }
    public virtual Carro Carro { get; set; } = null!;
}
