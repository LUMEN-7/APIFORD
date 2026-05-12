using AutoMapper;
using APIFORD.Data.DTOS.User;
using APIFORD.Model;

namespace APIFORD.Perfils;

public class UserPerfil : Profile
{
    public UserPerfil()
    {
        CreateMap<CreateUserDTO, User>();
        CreateMap<User, ShowUserDTO>();
        CreateMap<UpdateUserDTO, User>();
        CreateMap<User, UpdateUserDTO>();
    }
}
