using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Mode;
using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using APIFORD.Model.CarroClasses.Intermedians;
using AutoMapper;

namespace APIFORD.Perfils;

public class CarroPerfil : Profile
{
    public CarroPerfil()
    {
        // 1. MAPEAMENTO DE CRIAÇÃO (POST)
        CreateMap<CreateCarroDTO, Carro>()
            .ForMember(dest => dest.ModosCarro, opt => opt.Ignore())
            // Agora ignoramos as coleções das tabelas de ligação para controle manual no AfterMap
            .ForMember(dest => dest.Especificacaos, opt => opt.Ignore())
            .ForMember(dest => dest.Consumos, opt => opt.Ignore())
            .ForMember(dest => dest.Dimensoes, opt => opt.Ignore())
            .ForMember(dest => dest.Extras, opt => opt.Ignore())
            .ForMember(dest => dest.Pneus, opt => opt.Ignore())
            .AfterMap((src, dest, context) =>
            {
                // Como FonteId e Carro_id foram removidos de Fonte, não adicionamos mais à dest.Fontes diretamente 

                // Mapeia Consumo e sua ligação com a Fonte 

                if (src.Consumos != null)
                {
                    foreach (var consumeDto in src.Consumos)
                    {
                        var cons = context.Mapper.Map<Consumo>(consumeDto);
                        cons.ConsumoFontes.Add(new ConsumoFonte { Consumo = cons, FonteId = src.FonteId, DataReferencia = consumeDto.DataReferencia });
                        dest.Consumos.Add(cons);
                    }
                }

                // Mapeia Dimensões e sua ligação com a Fonte 
                if (src.Dimensoes != null)
                {
                    foreach (var dimensionsDto in src.Dimensoes)
                    {
                        var dim = context.Mapper.Map<Dimensao>(dimensionsDto);
                        dim.DimensaoFontes.Add(new DimensaoFonte { Dimensao = dim, FonteId = src.FonteId, DataReferencia = dimensionsDto.DataReferencia });
                        dest.Dimensoes.Add(dim);
                    }
                }

                // Mapeia Especificações (N:N via EspecificacaoFonte) 
                if (src.Especificacoes != null)
                {
                    foreach (var specDto in src.Especificacoes)
                    {
                        var spec = context.Mapper.Map<Especificacao>(specDto);
                        spec.EspecificacaoFontes.Add(new EspecificacaoFonte { Especificacao = spec, FonteId = src.FonteId, DataReferencia = specDto.DataReferencia });
                        dest.Especificacaos.Add(spec);
                    }
                }

                // Mapeia Extras (Additional) 
                if (src.Extras != null)
                {
                    foreach (var addDto in src.Extras)
                    {
                        var add = context.Mapper.Map<Extra>(addDto);
                        add.ExtraFontes.Add(new ExtraFonte { Extra = add, FonteId = src.FonteId, DataReferencia = addDto.DataReferencia });
                        dest.Extras.Add(add);
                    }
                }
                // Mapeia Pneus (N:N via TireFonte) 
                if (src.Pneus != null)
                {
                    foreach (var tireDto in src.Pneus)
                    {
                        var tire = context.Mapper.Map<Pneu>(tireDto);
                        tire.PneuFontes.Add(new PneuFonte { Pneu = tire, FonteId = src.FonteId, DataReferencia = tireDto.DataReferencia });
                        dest.Pneus.Add(tire);
                    }
                }
            });

   // 2. MAPEAMENTOS INDIVIDUAIS (DTO -> Entidade)
        CreateMap<CreateEspecificacaoDTO, Especificacao>();
        CreateMap<CreateConsumoDTO, Consumo>();
        CreateMap<CreateDimensaoDTO, Dimensao>();
        CreateMap<CreateExtraDTO, Extra>();
        CreateMap<CreatePneuDTO, Pneu>();
        CreateMap<CreateFonteDTO, Fonte>();
        CreateMap<CreateModoDTO, Modo>();
        CreateMap<CreateModeloSalvoDTO, ModeloSalvo>();

        // 3. MAPEAMENTOS DE LEITURA (GET)
        CreateMap<Carro, ReadCarroDTO>()
            .ForMember(dest => dest.ModosCarro, opt => opt.MapFrom(src =>
                src.ModosCarro.Select(cm => cm.Modo.Tipo).ToList()))
            .ForMember(dest => dest.Fontes, opt => opt.Ignore()) // Ignora para preenchimento manual
            .AfterMap((src, dest, context) =>
            {
                var allFontesList = new List<Fonte>();

                if (src.Especificacaos != null)
                    allFontesList.AddRange(src.Especificacaos.SelectMany(s => s.EspecificacaoFontes.Select(ss => ss.Fonte)));

                if (src.Consumos != null)
                    allFontesList.AddRange(src.Consumos.SelectMany(c => c.ConsumoFontes.Select(cs => cs.Fonte)));

                if (src.Dimensoes != null)
                    allFontesList.AddRange(src.Dimensoes.SelectMany(d => d.DimensaoFontes.Select(ds => ds.Fonte)));

                if (src.Pneus != null)
                    allFontesList.AddRange(src.Pneus.SelectMany(t => t.PneuFontes.Select(ts => ts.Fonte)));

                if (src.Extras != null)
                    allFontesList.AddRange(src.Extras.SelectMany(a => a.ExtraFontes.Select(ans => ans.Fonte)));

                // Deduplica fontes por ID
                var uniqueFontes = allFontesList
                    .Where(s => s != null)
                    .DistinctBy(s => s.Id)
                    .ToList();

                dest.Fontes = context.Mapper.Map<List<ReadFonteDTO>>(uniqueFontes);
            });

        // Mapeamentos Satélites com extração de FonteId e Datas das tabelas de ligação
        CreateMap<Especificacao, ReadEspecificacaoDTO>()
            .ForMember(dest => dest.FonteId, opt => opt.MapFrom(src => src.EspecificacaoFontes.Select(ss => ss.FonteId).FirstOrDefault()))

            .ForMember(dest => dest.DataColeta, opt => opt.MapFrom(src => src.EspecificacaoFontes.Select(ss => ss.DataColeta).FirstOrDefault()))

            .ForMember(dest => dest.DataReferencia, opt => opt.MapFrom(src => src.EspecificacaoFontes.Select(ss => ss.DataReferencia).FirstOrDefault()));


        CreateMap<Consumo, ReadConsumoDTO>()
            .ForMember(dest => dest.FonteId, opt => opt.MapFrom(src => src.ConsumoFontes.Select(cs => cs.FonteId).FirstOrDefault()))

            .ForMember(dest => dest.DataColeta, opt => opt.MapFrom(src => src.ConsumoFontes.Select(cs => cs.DataColeta).FirstOrDefault()))

            .ForMember(dest => dest.DataReferencia, opt => opt.MapFrom(src => src.ConsumoFontes.Select(cs => cs.DataReferencia).FirstOrDefault()));

        CreateMap<Pneu, ReadPneuDTO>()
            .ForMember(dest => dest.FonteId, opt => opt.MapFrom(src => src.PneuFontes.Select(ts => ts.FonteId).FirstOrDefault()))

            .ForMember(dest => dest.DataColeta, opt => opt.MapFrom(src => src.PneuFontes.Select(ts => ts.DataColeta).FirstOrDefault()))

            .ForMember(dest => dest.DataReferencia, opt => opt.MapFrom(src => src.PneuFontes.Select(ts => ts.DataReferencia).FirstOrDefault()));

        CreateMap<Dimensao, ReadDimensaoDTO>()
            .ForMember(dest => dest.FonteId, opt => opt.MapFrom(src => src.DimensaoFontes.Select(ds => ds.FonteId).FirstOrDefault()))

            .ForMember(dest => dest.DataColeta, opt => opt.MapFrom(src => src.DimensaoFontes.Select(ds => ds.DataColeta).FirstOrDefault()))

            .ForMember(dest => dest.DataReferencia, opt => opt.MapFrom(src => src.DimensaoFontes.Select(ds => ds.DataReferencia).FirstOrDefault()));


        CreateMap<Extra, ReadExtraDTO>()
            .ForMember(dest => dest.FonteId, opt => opt.MapFrom(src => src.ExtraFontes.Select(asrc => asrc.FonteId).FirstOrDefault()))

            .ForMember(dest => dest.DataColeta, opt => opt.MapFrom(src => src.ExtraFontes.Select(asrc => asrc.DataColeta).FirstOrDefault()))

            .ForMember(dest => dest.DataReferencia, opt => opt.MapFrom(src => src.ExtraFontes.Select(asrc => asrc.DataReferencia).FirstOrDefault()));

        CreateMap<IEnumerable<ModeloSalvo>, ReadModeloSalvoDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src =>src.Select(sm => sm.UserId).FirstOrDefault()))
            
            .ForMember(dest => dest.FavoriteCarros, opt => opt.MapFrom(src =>src.Select(sm => sm.Carro).ToList())); ;


        CreateMap<Fonte, ReadFonteDTO>();
        CreateMap<Modo, ReadModoDTO>();


        // 4. MAPEAMENTOS DE ATUALIZAÇÃO (PATCH)
        CreateMap<UpdateEspecificacaoDTO, Especificacao>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateConsumoDTO, Consumo>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateDimensaoDTO, Dimensao>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateExtraDTO, Extra>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdatePneuDTO, Pneu>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateModoDTO, Modo>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateModeloSalvoDTO, ModeloSalvo>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}