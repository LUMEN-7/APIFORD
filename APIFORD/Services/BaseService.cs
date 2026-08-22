using APIFORD.Data;
using APIFORD.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIFORD.Services;

public class BaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> : IBaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> where TEntity : class
{
    protected readonly FordDbContext Context;
    protected readonly IMapper Mapper;
    protected readonly DbSet<TEntity> DbSet;

    public BaseService(FordDbContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
        DbSet = Context.Set<TEntity>();
    }


    protected virtual Task PostMappingAsync(TReadDTO dto, TEntity entity)
    {
        return Task.CompletedTask;
    }

    protected virtual Task PostMappingListAsync(List<TReadDTO> dtos, List<TEntity> entities)
    {
        return Task.CompletedTask;
    }

    public virtual async Task<TReadDTO> CreateAsync(TCreateDTO dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();

        var readDto = Mapper.Map<TReadDTO>(entity);
        await PostMappingAsync(readDto, entity);
        return readDto;
    }

    public virtual async Task<IEnumerable<TReadDTO>> GetAllAsync()
    {
        var query = DbSet.AsQueryable();

        var property = typeof(TEntity).GetProperty("Excluido");
        if (property != null)
        {
            query = query.Where(e => EF.Property<bool>(e, "Excluido") == false);
        }

        var entities = await query.ToListAsync();
        var dtos = Mapper.Map<List<TReadDTO>>(entities);

        await PostMappingListAsync(dtos, entities);
        return dtos;
    }

    public virtual async Task<TReadDTO> GetByIdAsync(TKey id)
    {
        var query = DbSet.AsQueryable();

        var entity = await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id));
        if (entity == null) return default!;

        var readDto = Mapper.Map<TReadDTO>(entity);
        await PostMappingAsync(readDto, entity);
        return readDto;
    }

    public virtual async Task<TReadDTO> UpdateAsync(TKey id, TUpdateDTO dto)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) return default!;

        Mapper.Map(dto, entity);
        await Context.SaveChangesAsync();

        var readDto = Mapper.Map<TReadDTO>(entity);
        await PostMappingAsync(readDto, entity);
        return readDto;
    }

    public virtual async Task<bool> SoftDeleteAsync(TKey id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) return false;

        var property = typeof(TEntity).GetProperty("Excluido");
        if (property != null)
        {
            property.SetValue(entity, true);
            await Context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public virtual async Task<bool> AnonymizeAsync(TKey id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) return false;

        var properties = typeof(TEntity).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.PropertyType == typeof(string) && prop.Name != "Id")
            {
                prop.SetValue(entity, "Unknown");
            }
        }

        await Context.SaveChangesAsync();
        return true;
    }
}