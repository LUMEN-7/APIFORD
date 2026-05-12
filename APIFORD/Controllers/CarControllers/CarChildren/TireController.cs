using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.CarroServices.CarroChildren;
using APIFORD.Services.Interfaces;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarroControllers.CarroChildren;

[ApiController]
[Route("[controller]")]
public class TireController : BaseController<Pneu, CreatePneuDTO, ReadPneuDTO, UpdatePneuDTO, int>
{
    private PneuService _tireService;

    public TireController(PneuService tireService) : base(tireService)
    {
    }
}
