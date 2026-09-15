namespace APIFORD.Util;

public enum NotificationTypes
{
    EMPRESA,
    CARRO,
    UPDATE,
    LANCAMENTO,
    SYSTEM,
    REPORT,
    BUSCA_CONCLUIDA
}

public enum TipoDestinoNotificacao
{
    Todos = 0,
    UsuariosEspecificos = 1,
    FavoritantesDeCarro = 2
}
