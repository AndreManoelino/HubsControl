import './Produtos.css'

function Produtos() {
  return (
    <section className="modulo">
      <div className="modulo-cabecalho">
        <div>
          <h1>Produtos</h1>
          <p>Gerencie os produtos da sua empresa.</p>
        </div>

        <button>+ Novo produto</button>
      </div>

      <div className="modulo-vazio">
        <h2>Nenhum produto cadastrado</h2>
        <p>Os produtos da empresa aparecerão aqui.</p>
      </div>
    </section>
  )
}

export default Produtos