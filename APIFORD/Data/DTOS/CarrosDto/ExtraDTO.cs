using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record ExtraDTO(
    [Required] PropriedadeScrapingDTO<decimal> CapacidadeTanque,
    [Required] PropriedadeScrapingDTO<string> TipoCombustivel,
    [Required] PropriedadeScrapingDTO<decimal> CapacidadeCarga,
    [Required] PropriedadeScrapingDTO<decimal> CapacidadeReboque,
     [Required] PropriedadeScrapingDTO<string> Conforto,
      [Required] PropriedadeScrapingDTO<string> Tecnologia,
       [Required] PropriedadeScrapingDTO<string> Seguranca,
       [Required] PropriedadeScrapingDTO<string> Performace

)
{
    public ExtraDTO() : this(default!, default!, default!, default!, default!, default!, default!, default!) { }
};