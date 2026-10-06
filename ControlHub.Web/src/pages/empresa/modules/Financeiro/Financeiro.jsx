import './Financeiro.css'

function Financeiro() {
  return (
    <section className="modulo">
      <div className="modulo-cabecalho">
        <div>
          <h1>Financeiro</h1>
          <p>Controle financeiro da sua empresa.</p>
        </div>
      </div>

      <div className="modulo-vazio">
        <h2>Nenhum lançamento</h2>
        <p>As movimentações financeiras aparecerão aqui.</p>
      </div>
    </section>
  )
}

export default Financeiro