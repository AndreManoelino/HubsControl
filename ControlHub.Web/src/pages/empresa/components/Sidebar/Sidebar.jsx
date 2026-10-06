import { menusEmpresa } from '../../config/menuconfig'
import './Sidebar.css'

function Sidebar({
  aberto,
  setAberto,
  secaoAtiva,
  setSecaoAtiva,
  usuario,
  sair,
}) {
  return (
    <aside className={`empresa-sidebar ${aberto ? 'aberta' : 'fechada'}`}>
      <div className="empresa-sidebar-topo">
        {aberto && (
          <div className="empresa-sidebar-titulo">
            <strong>ControlHub</strong>
            <span>{usuario?.nome}</span>
          </div>
        )}

        <button
          className="empresa-sidebar-toggle"
          onClick={() => setAberto(!aberto)}
          title={aberto ? 'Recolher menu' : 'Expandir menu'}
        >
          {aberto ? '‹' : '›'}
        </button>
      </div>

      <nav className="empresa-menu">
        {menusEmpresa.map((menu) => (
          <button
            key={menu.id}
            className={`empresa-menu-item ${
              secaoAtiva === menu.id ? 'ativo' : ''
            }`}
            onClick={() => setSecaoAtiva(menu.id)}
            title={menu.nome}
          >
            <span className="empresa-menu-icone">
              {menu.icone}
            </span>

            {aberto && (
              <span className="empresa-menu-texto">
                {menu.nome}
              </span>
            )}
          </button>
        ))}
      </nav>

      <div className="empresa-sidebar-rodape">
        <button
          className="empresa-menu-item empresa-sair"
          onClick={sair}
          title="Sair"
        >
          <span className="empresa-menu-icone">↪</span>

          {aberto && (
            <span className="empresa-menu-texto">
              Sair
            </span>
          )}
        </button>
      </div>
    </aside>
  )
}

export default Sidebar