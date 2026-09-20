using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace APIFORD.Services.CarroServices;

public class FonteService : BaseService<Fonte, CreateFonteDTO, ReadFonteDTO, UpdateFonteDTO, int>
{
    private readonly FordDbContext _context;

    public FonteService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
    }

    public async Task<int> ObterOuCriarFonteAsync(string nomeFonte)
    {
        var fonteExistente = await _context.Fontes.FirstOrDefaultAsync(f => f.Nome == nomeFonte);
        if (fonteExistente != null) return fonteExistente.Id;

        var novaFonte = new Fonte { Nome = nomeFonte };
        await _context.Fontes.AddAsync(novaFonte);
        await _context.SaveChangesAsync();
        return novaFonte.Id;
    }
}

