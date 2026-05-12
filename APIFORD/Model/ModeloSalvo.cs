using APIFORD.Model.CarroClasses;

namespace APIFORD.Model;

public class ModeloSalvo
{
    // Chave Estrangeira e parte da Chave Primária
    public string UserId { get; set; }
    public virtual User User { get; set; } = null!;

    // Chave Estrangeira e parte da Chave Primária
    public int CarroId { get; set; }
    public virtual Carro Carro { get; set; } = null!;
}
