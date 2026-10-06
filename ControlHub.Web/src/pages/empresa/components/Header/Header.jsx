import './Header.css'

function Header({ usuario, setMenuAberto }) {
  return (
    <header className="empresa-header">
      <button
        className="empresa-header-menu"
        onClick={() => setMenuAberto((valor) => !valor)}
      >
        ☰
      </button>

      <div className="empresa-header-direita">
        <div className="empresa-header-usuario">
          <strong>{usuario?.nome}</strong>
          <span>{usuario?.perfil}</span>
        </div>

        <div className="empresa-header-avatar">
          {usuario?.nome?.charAt(0)?.toUpperCase() || 'U'}
        </div>
      </div>
    </header>
  )
}

export default Header