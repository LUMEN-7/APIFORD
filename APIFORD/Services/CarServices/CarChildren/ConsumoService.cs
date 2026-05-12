using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;
namespace APIFORD.Services.CarroServices.CarroChildren;

public class ConsumoService : BaseService<Consumo, CreateConsumoDTO, ReadConsumoDTO, UpdateConsumoDTO, int>
{
    public ConsumoService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
