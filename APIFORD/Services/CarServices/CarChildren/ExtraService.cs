using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;

namespace APIFORD.Services.CarroServices.CarroChildren;

public class ExtraService : BaseService<Extra, CreateExtraDTO, ReadExtraDTO, UpdateExtraDTO, int>
{
    public ExtraService(FordDbContext context, IMapper mapper) : base(context, mapper)
    { 
    }
}
