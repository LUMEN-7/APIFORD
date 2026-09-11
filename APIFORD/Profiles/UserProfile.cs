using APIFORD.Data.DTOS.Comparison;
using APIFORD.Data.DTOS.User;
using APIFORD.Model.User;
using AutoMapper;

namespace APIFORD.Perfils;

public class UserPerfil : Profile
{
    public UserPerfil()
    {
        
        CreateMap<User, ShowUserDTO>();
        CreateMap<UpdateUserDTO, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<User, UpdateUserDTO>();

        CreateMap<ComparacaoSalva, ReadComparacaoSalvaDTO>();
    }
}
