using APIFORD.Data.DTOS.Annotations;
using APIFORD.Model.Annotation;
using AutoMapper;

namespace APIFORD.Profiles;

public class AnotacaoProfile: Profile
{
    public AnotacaoProfile()
    {
        CreateMap<Anotacao, ReadAnotacaoDTO>();
        CreateMap<BlocoAnotacao, ReadBlocoDTO>()
            .ForMember(dest => dest.CardCarro, opt => opt.Ignore())
            .ForMember(dest => dest.CardComparacao, opt => opt.Ignore());
    }
}
