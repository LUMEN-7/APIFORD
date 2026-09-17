using APIFORD.Data.DTOS.Schedule;
using APIFORD.Model.Schedule;
using AutoMapper;

namespace APIFORD.Profiles;

public class AgendamentoProfile : Profile
{
    public AgendamentoProfile()
    {
        CreateMap<AgendamentoPesquisa, AgendamentoPesquisaDTO>();
        CreateMap<EscutaLancamento, EscutaLancamentoDTO>();
    }
}
