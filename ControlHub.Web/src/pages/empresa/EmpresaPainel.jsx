import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import './EmpresaPainel.css'

function EmpresaPainel() {
  const { empresaUrl } = useParams()
  const navigate = useNavigate()

  const [usuario, setUsuario] = useState(null)
  const [menuAberto, setMenuAberto] = useState(true)
  const [secaoAtiva, setSecaoAtiva] = useState('inicio')

  useEffect(() => {
    const token = localStorage.getItem('controlhub_token')
    const usuarioSalvo = localStorage.getItem('controlhub_usuario')

    if (!token || !usuarioSalvo) {
      navigate(`/${empresaUrl}`)
      return
    }

    try {
      const dados = JSON.parse(usuarioSalvo)

      if (dados.perfil !== 'Dono') {
        navigate(`/${empresaUrl}`)
        return
      }

      setUsuario(dados)
    } catch {
      localStorage.removeItem('controlhub_token')
      localStorage.removeItem('controlhub_usuario')

      navigate(`/${empresaUrl}`)
    }
  }, [empresaUrl, navigate])

  function sair() {
    localStorage.removeItem('controlhub_token')
    localStorage.removeItem('controlhub_usuario')

    navigate(`/${empresaUrl}`)
  }

  function selecionarMenu(menu) {
    setSecaoAtiva(menu)
  }

  if (!usuario) {
    return null
  }

  return (
    <div className="empresa-painel">

      {/* OVERLAY MOBILE */}
      {menuAberto && (
        <div
          className="empresa-sidebar-overlay"
          onClick={() => setMenuAberto(false)}
        />
      )}

      {/* SIDEBAR */}
      <aside
        className={`empresa-sidebar ${
          menuAberto ? 'aberta' : 'fechada'
        }`}
      >

        {/* LOGO */}
        <div className="empresa-sidebar-topo">

          <div className="empresa-sidebar-logo">

            <div className="empresa-sidebar-logo-icon">
              C
            </div>

            {menuAberto && (
              <div className="empresa-sidebar-logo-texto">
                <strong>ControlHub</strong>
                <span>Painel da empresa</span>
              </div>
            )}

          </div>

          <button
            className="empresa-sidebar-toggle"
            onClick={() => setMenuAberto(!menuAberto)}
            title={menuAberto ? 'Recolher menu' : 'Abrir menu'}
          >
            {menuAberto ? '‹' : '›'}
          </button>

        </div>

        {/* MENU */}
        <nav className="empresa-menu">

          <button
            className={`empresa-menu-item ${
              secaoAtiva === 'inicio' ? 'ativo' : ''
            }`}
            onClick={() => selecionarMenu('inicio')}
            title="Início"
          >
            <span className="empresa-menu-icone">⌂</span>

            {menuAberto && (
              <span className="empresa-menu-texto">
                Início
              </span>
            )}
          </button>

          <button
            className={`empresa-menu-item ${
              secaoAtiva === 'pedidos' ? 'ativo' : ''
            }`}
            onClick={() => selecionarMenu('pedidos')}
            title="Pedidos"
          >
            <span className="empresa-menu-icone">🛒</span>

            {menuAberto && (
              <span className="empresa-menu-texto">
                Pedidos
              </span>
            )}
          </button>

          <button
            className={`empresa-menu-item ${
              secaoAtiva === 'produtos' ? 'ativo' : ''
            }`}
            onClick={() => selecionarMenu('produtos')}
            title="Produtos"
          >
            <span className="empresa-menu-icone">▣</span>

            {menuAberto && (
              <span className="empresa-menu-texto">
                Produtos
              </span>
            )}
          </button>

          <button
            className={`empresa-menu-item ${
              secaoAtiva === 'estoque' ? 'ativo' : ''
            }`}
            onClick={() => selecionarMenu('estoque')}
            title="Estoque"
          >
            <span className="empresa-menu-icone">▤</span>

            {menuAberto && (
              <span className="empresa-menu-texto">
                Estoque
              </span>
            )}
          </button>

          <button
            className={`empresa-menu-item ${
              secaoAtiva === 'financeiro' ? 'ativo' : ''
            }`}
            onClick={() => selecionarMenu('financeiro')}
            title="Financeiro"
          >
            <span className="empresa-menu-icone">R$</span>

            {menuAberto && (
              <span className="empresa-menu-texto">
                Financeiro
              </span>
            )}
          </button>

          <button
            className={`empresa-menu-item ${
              secaoAtiva === 'usuarios' ? 'ativo' : ''
            }`}
            onClick={() => selecionarMenu('usuarios')}
            title="Usuários"
          >
            <span className="empresa-menu-icone">◉</span>

            {menuAberto && (
              <span className="empresa-menu-texto">
                Usuários
              </span>
            )}
          </button>

          <button
            className={`empresa-menu-item ${
              secaoAtiva === 'configuracoes' ? 'ativo' : ''
            }`}
            onClick={() => selecionarMenu('configuracoes')}
            title="Configurações"
          >
            <span className="empresa-menu-icone">⚙</span>

            {menuAberto && (
              <span className="empresa-menu-texto">
                Configurações
              </span>
            )}
          </button>

        </nav>

        {/* USUARIO / SAIR */}
        <div className="empresa-sidebar-rodape">

          {menuAberto && (
            <div className="empresa-sidebar-usuario">

              <div className="empresa-sidebar-avatar">
                {usuario.nome?.charAt(0)?.toUpperCase()}
              </div>

              <div className="empresa-sidebar-usuario-info">
                <strong>{usuario.nome}</strong>
                <span>Dono</span>
              </div>

            </div>
          )}

          <button
            className="empresa-menu-sair"
            onClick={sair}
            title="Sair"
          >
            <span>↪</span>

            {menuAberto && (
              <span>Sair</span>
            )}
          </button>

        </div>

      </aside>

      {/* CONTEUDO */}
      <main
        className={`empresa-conteudo ${
          menuAberto ? 'menu-aberto' : 'menu-fechado'
        }`}
      >

        {/* HEADER */}
        <header className="empresa-header">

          <div className="empresa-header-esquerda">

            <button
              className="empresa-mobile-menu"
              onClick={() => setMenuAberto(true)}
            >
              ☰
            </button>

            <div>

              <span className="empresa-header-pequeno">
                PAINEL ADMINISTRATIVO
              </span>

              <h1>
                Olá, {usuario.nome?.split(' ')[0]} 👋
              </h1>

              <p>
                Gerencie sua empresa de forma simples e organizada.
              </p>

            </div>

          </div>

          <div className="empresa-usuario">

            <div className="empresa-avatar">
              {usuario.nome?.charAt(0)?.toUpperCase()}
            </div>

            <div>
              <strong>{usuario.nome}</strong>
              <span>Dono da empresa</span>
            </div>

          </div>

        </header>

        {/* ÁREA INTERNA */}
        <div className="empresa-area">

          {secaoAtiva === 'inicio' && (
            <>

              {/* AÇÕES */}
              <section className="empresa-acoes">

                <button
                  className="empresa-card acao-principal"
                  onClick={() => selecionarMenu('pedidos')}
                >

                  <div className="empresa-card-icone">
                    🛒
                  </div>

                  <div className="empresa-card-conteudo">
                    <strong>Novo pedido</strong>
                    <span>
                      Registrar um novo pedido
                    </span>
                  </div>

                  <b>→</b>

                </button>

                <button
                  className="empresa-card"
                  onClick={() => selecionarMenu('produtos')}
                >

                  <div className="empresa-card-icone">
                    📦
                  </div>

                  <div className="empresa-card-conteudo">
                    <strong>Produtos</strong>
                    <span>
                      Cadastrar e gerenciar produtos
                    </span>
                  </div>

                  <b>→</b>

                </button>

                <button
                  className="empresa-card"
                  onClick={() => selecionarMenu('estoque')}
                >

                  <div className="empresa-card-icone">
                    📊
                  </div>

                  <div className="empresa-card-conteudo">
                    <strong>Estoque</strong>
                    <span>
                      Veja e controle seu estoque
                    </span>
                  </div>

                  <b>→</b>

                </button>

                <button
                  className="empresa-card"
                  onClick={() => selecionarMenu('financeiro')}
                >

                  <div className="empresa-card-icone">
                    💰
                  </div>

                  <div className="empresa-card-conteudo">
                    <strong>Financeiro</strong>
                    <span>
                      Acompanhe suas movimentações
                    </span>
                  </div>

                  <b>→</b>

                </button>

              </section>

              {/* RESUMO */}
              <section className="empresa-resumo">

                <div className="empresa-resumo-header">

                  <div>
                    <span>VISÃO GERAL</span>
                    <h2>Resumo da empresa</h2>
                  </div>

                  <span className="empresa-status">
                    <i></i>
                    Sistema ativo
                  </span>

                </div>

                <div className="empresa-indicadores">

                  <div className="empresa-indicador">

                    <span>Pedidos hoje</span>

                    <strong>0</strong>

                    <small>
                      Nenhum pedido registrado
                    </small>

                  </div>

                  <div className="empresa-indicador">

                    <span>Produtos</span>

                    <strong>0</strong>

                    <small>
                      Produtos cadastrados
                    </small>

                  </div>

                  <div className="empresa-indicador">

                    <span>Estoque baixo</span>

                    <strong>0</strong>

                    <small>
                      Itens precisam de atenção
                    </small>

                  </div>

                  <div className="empresa-indicador">

                    <span>Faturamento</span>

                    <strong>R$ 0,00</strong>

                    <small>
                      Movimentação de hoje
                    </small>

                  </div>

                </div>

              </section>

              {/* ATIVIDADE */}
              <section className="empresa-atividade">

                <div className="empresa-secao-titulo">

                  <div>
                    <span>ATIVIDADE</span>
                    <h2>Atividade recente</h2>
                  </div>

                  <button>
                    Ver tudo
                  </button>

                </div>

                <div className="empresa-atividade-vazia">

                  <div className="empresa-atividade-icone">
                    ◷
                  </div>

                  <strong>
                    Nenhuma atividade ainda
                  </strong>

                  <span>
                    Quando sua empresa começar a movimentar,
                    as atividades aparecerão aqui.
                  </span>

                </div>

              </section>

            </>
          )}

          {secaoAtiva === 'pedidos' && (
            <div className="empresa-modulo">

              <div className="empresa-modulo-header">
                <div>
                  <span>PEDIDOS</span>
                  <h2>Pedidos</h2>
                  <p>
                    Gerencie os pedidos da sua empresa.
                  </p>
                </div>

                <button className="empresa-botao-principal">
                  + Novo pedido
                </button>
              </div>

              <div className="empresa-modulo-vazio">
                <div>🛒</div>
                <strong>Nenhum pedido encontrado</strong>
                <span>
                  Os pedidos realizados aparecerão aqui.
                </span>
              </div>

            </div>
          )}

          {secaoAtiva === 'produtos' && (
            <div className="empresa-modulo">

              <div className="empresa-modulo-header">
                <div>
                  <span>CATÁLOGO</span>
                  <h2>Produtos</h2>
                  <p>
                    Cadastre e gerencie os produtos da empresa.
                  </p>
                </div>

                <button className="empresa-botao-principal">
                  + Novo produto
                </button>
              </div>

              <div className="empresa-modulo-vazio">
                <div>📦</div>
                <strong>Nenhum produto cadastrado</strong>
                <span>
                  Seus produtos aparecerão aqui.
                </span>
              </div>

            </div>
          )}

          {secaoAtiva === 'estoque' && (
            <div className="empresa-modulo">

              <div className="empresa-modulo-header">
                <div>
                  <span>INVENTÁRIO</span>
                  <h2>Estoque</h2>
                  <p>
                    Controle entradas, saídas e disponibilidade.
                  </p>
                </div>
              </div>

              <div className="empresa-modulo-vazio">
                <div>📊</div>
                <strong>Estoque vazio</strong>
                <span>
                  Os produtos e movimentações aparecerão aqui.
                </span>
              </div>

            </div>
          )}

          {secaoAtiva === 'financeiro' && (
            <div className="empresa-modulo">

              <div className="empresa-modulo-header">
                <div>
                  <span>FINANCEIRO</span>
                  <h2>Financeiro</h2>
                  <p>
                    Acompanhe receitas, despesas e movimentações.
                  </p>
                </div>
              </div>

              <div className="empresa-modulo-vazio">
                <div>💰</div>
                <strong>Nenhuma movimentação</strong>
                <span>
                  As movimentações financeiras aparecerão aqui.
                </span>
              </div>

            </div>
          )}

          {secaoAtiva === 'usuarios' && (
            <div className="empresa-modulo">

              <div className="empresa-modulo-header">
                <div>
                  <span>ACESSOS</span>
                  <h2>Usuários</h2>
                  <p>
                    Gerencie os usuários que possuem acesso.
                  </p>
                </div>

                <button className="empresa-botao-principal">
                  + Novo usuário
                </button>
              </div>

              <div className="empresa-modulo-vazio">
                <div>👥</div>
                <strong>Nenhum usuário adicional</strong>
                <span>
                  Os usuários cadastrados aparecerão aqui.
                </span>
              </div>

            </div>
          )}

          {secaoAtiva === 'configuracoes' && (
            <div className="empresa-modulo">

              <div className="empresa-modulo-header">
                <div>
                  <span>SISTEMA</span>
                  <h2>Configurações</h2>
                  <p>
                    Configure as informações e preferências da empresa.
                  </p>
                </div>
              </div>

              <div className="empresa-modulo-vazio">
                <div>⚙</div>
                <strong>Configurações da empresa</strong>
                <span>
                  Em breve você poderá configurar sua empresa por aqui.
                </span>
              </div>

            </div>
          )}

        </div>

      </main>

    </div>
  )
}

export default EmpresaPainel