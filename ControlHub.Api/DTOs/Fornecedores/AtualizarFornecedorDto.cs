

namespace ControlHub.Api.DTOs.Fornecedores
{
    public class AtualizarFornecedorDto
    {
        public string Nome { get; set;} = string.Empty;
        public string? Documento { get; set;}
        public string? Email {get;set;}
        public string? Telefone { get;set;}
        public bool Ativo {get; set;}
    }
}