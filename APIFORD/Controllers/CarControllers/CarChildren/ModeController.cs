using APIFORD.Data.DTOS.Mode;
using APIFORD.Services;
using APIFORD.Services.CarroServices.CarroChildren;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Carro.CarroChildren;


[ApiController]
[Route("[controller]")]
public class ModeController : BaseController<Model.Modo, CreateModoDTO, ReadModoDTO, UpdateModoDTO, int>
{
    private readonly ModoService _modeService;

    public ModeController(ModoService modeService): base(modeService)
    {
        _modeService = modeService;
    }


}
