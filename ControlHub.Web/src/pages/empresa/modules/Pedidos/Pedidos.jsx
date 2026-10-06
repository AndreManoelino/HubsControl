import './Pedidos.css'

function Pedidos() {
  return (
    <section className="pedidos">
      <div className="pedidos-cabecalho">
        <div>
          <h1>Pedidos</h1>
          <p>Gerencie os pedidos da sua empresa.</p>
        </div>

        <button className="pedidos-novo">
          + Novo pedido
        </button>
      </div>

      <div className="pedidos-filtros">
        <button className="ativo">Todos</button>
        <button>Pendentes</button>
        <button>Em andamento</button>
        <button>Concluídos</button>
        <button>Cancelados</button>
      </div>

      <div className="pedidos-vazio">
        <div className="pedidos-vazio-icone">🛒</div>

        <h2>Nenhum pedido encontrado</h2>

        <p>
          Os pedidos realizados pela sua empresa aparecerão aqui.
        </p>
      </div>
    </section>
  )
}

export default Pedidos