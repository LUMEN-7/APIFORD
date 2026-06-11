using APIFORD.Data.DTOS;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Mode;
using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using APIFORD.Model.CarroClasses.Intermedians;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace APIFORD.Perfils;

public class CarroPerfil : Profile
{
    public CarroPerfil()
    {
        // Mapeamento genérico para a estrutura open-ended de histórico do Scraping
        // Ensina o AutoMapper: Pegue "Fontes" do DTO e grave em "Fontes" da Entidade
        // 1. Mapeamento das listas (Itens)
        CreateMap<ItemFonteScrapingDTO<int>, ItemFonteScraping<int>>().ReverseMap();
        CreateMap<ItemFonteScrapingDTO<string>, ItemFonteScraping<string>>().ReverseMap();
        CreateMap<ItemFonteScrapingDTO<double>, ItemFonteScraping<double>>().ReverseMap();
        CreateMap<ItemFonteScrapingDTO<decimal>, ItemFonteScraping<decimal>>().ReverseMap();

        // 2. Mapeamento dos Envelopes FORÇANDO A CÓPIA DA LISTA
        CreateMap<PropriedadeScrapingDTO<int>, PropriedadeScraping<int>>()
            .ForMember(dest => dest.Fontes, opt => opt.MapFrom(src => src.Fontes)).ReverseMap();

        CreateMap<PropriedadeScrapingDTO<string>, PropriedadeScraping<string>>()
            .ForMember(dest => dest.Fontes, opt => opt.MapFrom(src => src.Fontes)).ReverseMap();

        CreateMap<PropriedadeScrapingDTO<double>, PropriedadeScraping<double>>()
            .ForMember(dest => dest.Fontes, opt => opt.MapFrom(src => src.Fontes)).ReverseMap();

        CreateMap<PropriedadeScrapingDTO<decimal>, PropriedadeScraping<decimal>>()
            .ForMember(dest => dest.Fontes, opt => opt.MapFrom(src => src.Fontes)).ReverseMap();

        // 1. MAPEAMENTO DE CRIAÇÃO (POST)
        CreateMap<CreateCarroDTO, Carro>()
            .ForMember(dest => dest.ModosCarro, opt => opt.Ignore());

        CreateMap<CreateEspecificacaoDTO, Especificacao>();
        CreateMap<CreateConsumoDTO, Consumo>();
        CreateMap<CreateDimensaoDTO, Dimensao>();
        CreateMap<CreatePneuDTO, Pneu>();
        CreateMap<CreateExtraDTO, Extra>();
        CreateMap<CreateFonteDTO, Fonte>();
        CreateMap<CreateModoDTO, Modo>();
        CreateMap<CreateModeloSalvoDTO, ModeloSalvo>();

        // 2. MAPEAMENTO DE LEITURA (GET)
        CreateMap<Carro, ReadCarroDTO>()
            .ForMember(dest => dest.ModosCarro, opt => opt.MapFrom(src =>
                src.ModosCarro.Select(cm => cm.Modo.Tipo).ToList()))
            .ForMember(dest => dest.Fontes, opt => opt.Ignore());

        CreateMap<Especificacao, ReadEspecificacaoDTO>();
        CreateMap<Consumo, ReadConsumoDTO>();
        CreateMap<Dimensao, ReadDimensaoDTO>();
        CreateMap<Pneu, ReadPneuDTO>();
        CreateMap<Extra, ReadExtraDTO>();

        CreateMap<Fonte, ReadFonteDTO>();
        CreateMap<Modo, ReadModoDTO>();

        CreateMap<IEnumerable<ModeloSalvo>, ReadModeloSalvoDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Select(sm => sm.UserId).FirstOrDefault()))
            .ForMember(dest => dest.FavoriteCarros, opt => opt.MapFrom(src => src.Select(sm => sm.Carro).ToList()));

        // 3. MAPEAMENTO DE ATUALIZAÇÃO (PATCH / PUT)
        CreateMap<UpdateCarroDTO, Carro>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateEspecificacaoDTO, Especificacao>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateConsumoDTO, Consumo>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateDimensaoDTO, Dimensao>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateExtraDTO, Extra>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdatePneuDTO, Pneu>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateModoDTO, Modo>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateModeloSalvoDTO, ModeloSalvo>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}