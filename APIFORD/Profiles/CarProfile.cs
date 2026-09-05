using APIFORD.Data.DTOS;
using APIFORD.Data.DTOS.CarrosDto;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using APIFORD.Model.User;
using AutoMapper;

namespace APIFORD.Perfils;

public class CarroPerfil : Profile
{
    public CarroPerfil()
    {
        // ====================================================================
        // 1. A MÁGICA DOS GENÉRICOS ABERTOS (Mapeia TODOS os tipos de uma vez)
        // ====================================================================
        CreateMap(typeof(ItemFonteScrapingDTO<>), typeof(ItemFonteScraping<>)).ReverseMap();
        CreateMap(typeof(PropriedadeScrapingDTO<>), typeof(PropriedadeScraping<>)).ReverseMap();

        // ====================================================================
        // 2. OS ENVELOPES JSON (Apenas 1 DTO para Create, Read e Update)
        // ====================================================================
        CreateMap<EspecificacaoDTO, Especificacao>().ReverseMap();
        CreateMap<ConsumoDTO, Consumo>().ReverseMap();
        CreateMap<DimensaoDTO, Dimensao>().ReverseMap();
        CreateMap<PneuDTO, Pneu>().ReverseMap();
        CreateMap<ExtraDTO, Extra>().ReverseMap();

        // ====================================================================
        // 3. A RAIZ DE AGREGAÇÃO (A única que tem Create, Read e Update)
        // ====================================================================

        // POST (Sem ignorar o ModosCarro, o AutoMapper cuida dele agora!)
        CreateMap<CreateCarroDTO, Carro>();

        // GET (Limpo e direto. O AutoMapper converte os JSONs automaticamente)
        // 2. MAPEAMENTO DE LEITURA (GET)
        CreateMap<Carro, ReadCarroDTO>()
            .ForMember(dest => dest.Fontes, opt => opt.Ignore());
                       

        // PATCH/PUT (Atualiza apenas o que não for nulo)
        CreateMap<UpdateCarroDTO, Carro>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ====================================================================
        // 4. ENTIDADES RELACIONAIS (Mantidas intactas)
        // ====================================================================
        CreateMap<Fonte, ReadFonteDTO>();

        CreateMap<CreateModeloSalvoDTO, ModeloSalvo>();
        CreateMap<UpdateModeloSalvoDTO, ModeloSalvo>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<IEnumerable<ModeloSalvo>, ReadModeloSalvoDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Select(sm => sm.UserId).FirstOrDefault()))
            .ForMember(dest => dest.FavoriteCarros, opt => opt.Ignore()); // preenchido manualmente no service, precisa de acesso ao banco
    }
}