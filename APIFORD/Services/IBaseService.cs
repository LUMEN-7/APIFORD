namespace APIFORD.Services;

public interface IBaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey> where TEntity : class
{
    Task<TReadDTO> CreateAsync(TCreateDTO dto);
    Task<IEnumerable<TReadDTO>> GetAllAsync();
    Task<TReadDTO> GetByIdAsync(TKey id);
    Task<TReadDTO> UpdateAsync(TKey id, TUpdateDTO dto);
    Task<bool> SoftDeleteAsync(TKey id);
    Task<bool> AnonymizeAsync(TKey id);
}