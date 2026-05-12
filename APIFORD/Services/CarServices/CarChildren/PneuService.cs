using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;

namespace APIFORD.Services.CarroServices.CarroChildren;

public class PneuService : BaseService<Pneu, CreatePneuDTO, ReadPneuDTO, UpdatePneuDTO, int>
{
    public PneuService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
