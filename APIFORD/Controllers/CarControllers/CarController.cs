using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

using APIFORD.Services.CarroServices;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarroControllers;


[ApiController]
[Route("[controller]")]
public class CarroController : BaseController<Model.CarroClasses.Carro, CreateCarroDTO, ReadCarroDTO, UpdateCarroDTO, int>
{
    private readonly CarroService _CarroService;

    public CarroController(CarroService CarroService): base(CarroService)
    {
    }

}
