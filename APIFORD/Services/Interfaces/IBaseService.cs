namespace APIFORD.Services.Interfaces;

public interface IBaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> where TEntity : class
{
    Task<TReadDTO> GetByIdAsync(TKey id);
    Task<IEnumerable<TReadDTO>> GetAllAsync();
    Task<TReadDTO> CreateAsync(TCreateDTO createDto);
    Task<TReadDTO> UpdateAsync(TKey id, TUpdateDTO updateDto);
    Task<bool> SoftDeleteAsync(TKey id);
    Task<bool> AnonymizeAsync(TKey id);
}
