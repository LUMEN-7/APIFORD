using APIFORD.Data;
using APIFORD.Data.DTOS.Mode;
using APIFORD.Model;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace APIFORD.Services.CarroServices.CarroChildren;

public class ModoService : BaseService<Modo, CreateModoDTO, ReadModoDTO, UpdateModoDTO, int>
{
    public ModoService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
