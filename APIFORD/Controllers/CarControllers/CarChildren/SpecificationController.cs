using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.CarroServices.CarroChildren;
using APIFORD.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarroControllers.CarroChildren;

[ApiController]
[Route("[controller]")]
public class SpecificationController : BaseController<Especificacao, CreateEspecificacaoDTO, ReadEspecificacaoDTO, UpdateEspecificacaoDTO, int>
{
    private EspecificacaoService _specificationService;

    public SpecificationController(EspecificacaoService specificationService) : base(specificationService)
    {
    }
}
