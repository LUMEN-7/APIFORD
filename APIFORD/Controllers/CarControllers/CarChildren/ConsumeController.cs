using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.CarroServices.CarroChildren;
using APIFORD.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarroControllers.CarroChildren;

[ApiController]
[Route("[controller]")]
public class ConsumoController : BaseController<Consumo, CreateConsumoDTO, ReadConsumoDTO, UpdateConsumoDTO, int>
{
    private ConsumoService _consumoService;

    public ConsumoController(ConsumoService consumoService) : base(consumoService)
    {
    }

}
