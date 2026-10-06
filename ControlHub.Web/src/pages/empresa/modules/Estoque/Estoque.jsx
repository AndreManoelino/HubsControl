import './Estoque.css'

function Estoque() {
  return (
    <section className="modulo">
      <div className="modulo-cabecalho">
        <div>
          <h1>Estoque</h1>
          <p>Controle o estoque da sua empresa.</p>
        </div>
      </div>

      <div className="modulo-vazio">
        <h2>Estoque vazio</h2>
        <p>Os produtos e quantidades aparecerão aqui.</p>
      </div>
    </section>
  )
}

export default Estoque