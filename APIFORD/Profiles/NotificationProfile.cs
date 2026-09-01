using APIFORD.Data.DTOS.Notifications;
using APIFORD.Model;
using AutoMapper;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace APIFORD.Perfils;

public class NotificationPerfil : Profile
{
    public NotificationPerfil()
    {
        CreateMap<CreateNotificationDTO, Notificacao>()
            // Ignora a data para que o SQL Server use o GETUTCDATE() padrão
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore());

        CreateMap<Notificacao, ReadNotificationDTO>();
            //.ForMember(dest => dest.Mensagem, opt => opt.MapFrom(src =>
            //    string.IsNullOrEmpty(src.Mensagem)
            //        ? null
            //        : ConvertJson(JsonSerializer.Deserialize<JsonElement>(src.Mensagem))));
    }

// Função auxiliar para converter JsonElement em tipos pAroitivos do C#
    private static object ConvertJson(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dictionary = new Dictionary<string, object>();
                    foreach (var property in element.EnumerateObject())
                    {
                        dictionary[property.Name] = ConvertJson(property.Value);
                    }
                    return dictionary;
                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ConvertJson(item));
                    }
                    return list;
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    return element.TryGetInt32(out int i) ? i : element.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                default:
                    return element.GetRawText();
            }
        }

}
