using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;

namespace APIFORD.Services.CarroServices.CarroChildren;

public class EspecificacaoService : BaseService<Especificacao, CreateEspecificacaoDTO, ReadEspecificacaoDTO, UpdateEspecificacaoDTO, int>
{
    public EspecificacaoService(FordDbContext context, IMapper mapper) : base(context, mapper)
    { 
    }
}
