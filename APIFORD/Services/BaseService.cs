using APIFORD.Data;
using APIFORD.Data.DTOS.User;
using APIFORD.Model;
using APIFORD.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace APIFORD.Services;

public abstract class BaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> : IBaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey>
    where TEntity : class
{
    protected readonly FordDbContext Context;
    protected readonly IMapper Mapper;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseService(FordDbContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
        DbSet = Context.Set<TEntity>();
    }


    // Gancho (Hook) para Carroregar os Includes. Sobrescreva se a tabela tiver relacionamentos
    protected virtual IQueryable<TEntity> AddIncludes(IQueryable<TEntity> query) => query;


    public async virtual Task<TReadDTO> CreateAsync(TCreateDTO createDto)
    {
        var entity = Mapper.Map<TEntity>(createDto);
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
        return Mapper.Map<TReadDTO>(entity);
    }
    public async virtual Task<IEnumerable<TReadDTO>> GetAllAsync()
    {
        IQueryable<TEntity> query = DbSet;
        query = AddIncludes(query); // Aplica os includes se houverem

        var results = await query.ToListAsync();
        return Mapper.Map<List<TReadDTO>>(results);
    }
    public async virtual Task<TReadDTO> GetByIdAsync(TKey id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) throw new KeyNotFoundException("Registro não encontrado para o ID informado.");
        return Mapper.Map<TReadDTO>(entity);
    }


    public async virtual Task<TReadDTO> UpdateAsync(TKey id, TUpdateDTO updateDto)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) throw new KeyNotFoundException("Registro não encontrado para o ID informado.");
        entity = Mapper.Map(updateDto, entity);

        
        await Context.SaveChangesAsync();
        return Mapper.Map<TReadDTO>(entity);
    }

    public async virtual Task<bool> SoftDeleteAsync(TKey id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) throw new KeyNotFoundException("Registro não encontrado para o ID informado.");

        entity.GetType().GetProperty("IsDeleted")?.SetValue(entity, true);
        await Context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> AnonymizeAsync(TKey id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) throw new KeyNotFoundException("Registro não encontrado para o ID informado.");

        // Usamos Reflexão para varrer todas as propriedades e alterar os valores
        var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.Name != "Id"); // Mantém o ID intacto

        foreach (var prop in properties)
        {
            if (prop.Name == "IsDeleted") prop.SetValue(entity, true);
            // Se for string, vira "Unknown"
            if (prop.PropertyType == typeof(string)) prop.SetValue(entity, "Unknown");
            // Se for número, zera (opcional)
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal)) prop.SetValue(entity, 0);
            // Se for data, pode ser nula
            else if (prop.PropertyType == typeof(DateTime?)) prop.SetValue(entity, null);

        }
        await Context.SaveChangesAsync();
        return true;
    }



}