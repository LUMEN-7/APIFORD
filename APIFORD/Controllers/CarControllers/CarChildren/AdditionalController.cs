using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.CarroServices.CarroChildren;
using APIFORD.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarroControllers.CarroChildren;

[ApiController]
[Route("[controller]")]
public class AdditionalController : BaseController<Extra, CreateExtraDTO, ReadExtraDTO, UpdateExtraDTO, int>
{
    private ExtraService _additionalService;

    public AdditionalController(ExtraService additionalService) : base(additionalService)
    {
    }
}
