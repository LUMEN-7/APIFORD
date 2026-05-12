namespace APIFORD.Model.CarroClasses.Intermedians;

public class CarroModo
{
    public int CarroId { get; set; }
    public int ModoId { get; set; }
    public virtual Carro Carro { get; set; } = null!;
    public virtual Modo Modo { get; set; } = null!;
}
