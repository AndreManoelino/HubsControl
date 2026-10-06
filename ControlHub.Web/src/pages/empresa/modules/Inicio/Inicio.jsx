import './Inicio.css'

function Inicio() {
  return (
    <section className="inicio">
      <div className="inicio-cabecalho">
        <div>
          <h1>Início</h1>
          <p>Visão geral da sua empresa.</p>
        </div>
      </div>

      <div className="inicio-cards">
        <div className="inicio-card">
          <span>Pedidos</span>
          <strong>0</strong>
        </div>

        <div className="inicio-card">
          <span>Produtos</span>
          <strong>0</strong>
        </div>

        <div className="inicio-card">
          <span>Estoque</span>
          <strong>0</strong>
        </div>

        <div className="inicio-card">
          <span>Vendas</span>
          <strong>R$ 0,00</strong>
        </div>
      </div>

      <div className="inicio-conteudo">
        <div className="inicio-bloco">
          <h2>Atividade recente</h2>
          <p>Nenhuma atividade registrada.</p>
        </div>

        <div className="inicio-bloco">
          <h2>Resumo</h2>
          <p>Os dados da sua empresa aparecerão aqui.</p>
        </div>
      </div>
    </section>
  )
}

export default Inicio