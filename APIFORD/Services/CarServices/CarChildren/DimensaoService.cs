using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;
namespace APIFORD.Services.CarroServices.CarroChildren;

public class DimensaoService : BaseService<Dimensao, CreateDimensaoDTO, ReadDimensaoDTO, UpdateDimensaoDTO, int>
{
    public DimensaoService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
