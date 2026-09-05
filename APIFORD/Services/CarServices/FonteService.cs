using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;

namespace APIFORD.Services.CarroServices;

public class FonteService : BaseService<Fonte, CreateFonteDTO, ReadFonteDTO, UpdateFonteDTO, int>
{
    public FonteService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}

