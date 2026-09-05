using APIFORD.Data.DTOS.Comparison;
using APIFORD.Data.DTOS.User;
using APIFORD.Model.User;
using AutoMapper;

namespace APIFORD.Perfils;

public class UserPerfil : Profile
{
    public UserPerfil()
    {
        CreateMap<CreateUserDTO, User>();
        CreateMap<User, ShowUserDTO>();
        CreateMap<UpdateUserDTO, User>();
        CreateMap<User, UpdateUserDTO>();

        CreateMap<ComparacaoSalva, ReadComparacaoSalvaDTO>();
    }
}
