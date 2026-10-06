using ControlHub.Api.Models;

namespace ControlHub.Api.Models
{
    public class Secao
    {
        public Guid Id {get; set;}
        public Guid EmpresaId { get; set;}
        public string Nome { get; set;} = string.Empty;
        public bool Ativa { get; set;} = true;
        public DateTime CriadaEm  { get; set;} = DateTime.UtcNow;
        public Empresa Empresa {get; set;} = null!;
        public ICollection<Produto> Produtos { get; set;} = new List<Produto>();
        
    }
}