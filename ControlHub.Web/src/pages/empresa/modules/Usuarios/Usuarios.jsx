import './Usuarios.css'

function Usuarios() {
  return (
    <section className="modulo">
      <div className="modulo-cabecalho">
        <div>
          <h1>Usuários</h1>
          <p>Gerencie os usuários da empresa.</p>
        </div>

        <button>+ Novo usuário</button>
      </div>

      <div className="modulo-vazio">
        <h2>Nenhum usuário cadastrado</h2>
        <p>Os usuários da empresa aparecerão aqui.</p>
      </div>
    </section>
  )
}

export default Usuarios