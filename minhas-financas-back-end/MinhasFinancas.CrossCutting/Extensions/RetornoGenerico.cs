using System.Net;


public class RetornoGenerico
{
    public RetornoGenerico()
    {
    }

    public RetornoGenerico(bool sucesso, string mensagemSistema, HttpStatusCode httpStatusCode)
    {
        Sucesso = sucesso;
        MensagemSistema = mensagemSistema;
        HttpStatusCode = httpStatusCode;
        Dados = null;
        MensagemUsuario = "";
    }

    public RetornoGenerico(bool sucesso, string mensagemSistema, string mensagemUsuario, HttpStatusCode httpStatusCode)
    {
        Sucesso = sucesso;
        MensagemSistema = mensagemSistema;
        HttpStatusCode = httpStatusCode;
        Dados = null;
        MensagemUsuario = mensagemUsuario;
    }

    public RetornoGenerico(bool sucesso, string mensagemSistema, HttpStatusCode httpStatusCode, dynamic dados = null)
    {
        Sucesso = sucesso;
        MensagemSistema = mensagemSistema;
        Dados = dados;
        HttpStatusCode = httpStatusCode;
    }

    public RetornoGenerico(bool sucesso, string mensagemSistema, string mensagemUsuario, HttpStatusCode httpStatusCode, dynamic dados = null)
    {
        Sucesso = sucesso;
        MensagemSistema = mensagemSistema;
        Dados = dados;
        HttpStatusCode = httpStatusCode;
        MensagemUsuario = mensagemUsuario;
    }

    public bool Sucesso { get; set; }

    /// <summary>
    /// Utilizar propriedade para informar mensagem T�cnica
    /// </summary>
    public string MensagemSistema { get; set; }

    /// <summary>
    /// Utilizar propriedade para informar uma mensagem amig�vel ao usu�rio final
    /// </summary>
    public string MensagemUsuario { get; set; } = "";

    public HttpStatusCode HttpStatusCode { get; set; }

    public dynamic Dados { get; set; }
}

