using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.CarroServices.CarroChildren;
using APIFORD.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarroControllers.CarroChildren;

[ApiController]
[Route("[controller]")]
public class DimensionController : BaseController<Dimensao, CreateDimensaoDTO, ReadDimensaoDTO, UpdateDimensaoDTO, int>
{
    private DimensaoService _dimensionService;

    public DimensionController(DimensaoService dimensionService) : base(dimensionService)
    {
    }

}
