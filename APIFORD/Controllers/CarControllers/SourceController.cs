using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Mode;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.CarroServices;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Carro;


[ApiController]
[Route("[controller]")]
public class SourceController : BaseController<Fonte, CreateFonteDTO, ReadFonteDTO, UpdateFonteDTO, int>
{
    private readonly FonteService _sourceService;

    public SourceController(FonteService sourceService) : base(sourceService)
    {
    }

}
