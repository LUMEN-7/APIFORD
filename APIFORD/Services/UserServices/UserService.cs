using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.User;
using APIFORD.Model;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace APIFORD.Services.UserServices;

public class UserService
{
    private IMapper _mapper;
    protected readonly FordDbContext _context;
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private TokenService _tokenService;

    public UserService(IMapper mapper, FordDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService)
    {
        _mapper = mapper;
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task CreateUser(CreateUserDTO dto)
    {
        User user = _mapper.Map<User>(dto);

        IdentityResult resultado = await _userManager.CreateAsync(user, dto.Password);


        var erros = resultado.Errors.Select(e => e.Description);

        if (!resultado.Succeeded) throw new ApplicationException($"Falha ao criar usuário, \nError: {erros}");

    }

    public async Task<User> FindUser(string identifier)
    {   
        var a =  _userManager.Users.ToList();
        User user = await _userManager.FindByNameAsync(identifier);
        Console.WriteLine(a);
        Console.WriteLine(user.UserName);

        if (user == null) throw new ApplicationException("Falha ao autentiCarro: Credenciais inválidas.");

        return user;
    }

    public async Task<string> Login(LoginUserDTO dto)
    {
        User user = await FindUser(dto.UserIdentifier);

        var resultado = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);


        if (!resultado.Succeeded) throw new ApplicationException($"Falha ao autentiCarro usuário");

        await _signInManager.SignInAsync(user, false);

        var token = _tokenService.GenerateToken(user);

        return token;
    }


    public async Task UpdateUser(string id, UpdateUserDTO dto)
    {
        User user = await _userManager.FindByIdAsync(id);

        if (user == null) throw new ApplicationException("Usuário não encontrado.");

        _mapper.Map(dto, user);
        
        IdentityResult resultado = await _userManager.UpdateAsync(user);
        var erros = resultado.Errors.Select(e => e.Description);
        if (!resultado.Succeeded) throw new ApplicationException($"Falha ao atualizar usuário, \nError: {erros}");
    }

    public async Task SaveModel(CreateModeloSalvoDTO dto)
    {
        var savedModel = _mapper.Map<ModeloSalvo>(dto);
        _context.ModeloSalvos.Add(savedModel);
        await _context.SaveChangesAsync();
    }


    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    public List<ShowUserDTO> GetUsers()
    {
        List<User> users = _userManager.Users.ToList();
        return _mapper.Map<List<ShowUserDTO>>(users); ;
    }


    public async Task SoftDelete(string id)
    {
        User? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        user.Excluido = true;
        var resultado = await _userManager.UpdateAsync(user);
        if (!resultado.Succeeded)
        {
            var erros = string.Join(" | ", resultado.Errors.Select(e => e.Description));
            throw new ApplicationException($"Falha ao deletar usuário. Erro: {erros}");
        }

    }
    public async Task AnonymizeAsync(string id)
    {
        User? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        // Usamos Reflexão para varrer todas as propriedades e alterar os valores
        var properties = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.Name != "Id"); // Mantém o ID intacto

        foreach (var prop in properties)
        {
            if (prop.Name == "IsDeleted") prop.SetValue(user, true);
            // Se for string, vira "Unknown"
            if (prop.PropertyType == typeof(string)) prop.SetValue(user, "Unknown");
            // Se for número, zera (opcional)
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal)) prop.SetValue(user, 0);
            // Se for data, pode ser nula
            else if (prop.PropertyType == typeof(DateTime?)) prop.SetValue(user, null);

        }


        var resultado = await _userManager.UpdateAsync(user);
        if (!resultado.Succeeded)
        {
            var erros = string.Join(" | ", resultado.Errors.Select(e => e.Description));
            throw new ApplicationException($"Falha ao deletar usuário. Erro: {erros}");
        }

    }



}
