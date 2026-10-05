
import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import './EmpresaPainel.css'

function EmpresaPainel() {
  const { empresaUrl } = useParams()
  const navigate = useNavigate()

  const [usuario, setUsuario] = useState(null)

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

  if (!usuario) {
    return null
  }

  return (
    <div className="empresa-painel">

      <aside className="empresa-sidebar">

        <div className="empresa-sidebar-logo">
          <div className="empresa-sidebar-logo-icon">
            C
          </div>

          <div>
            <strong>ControlHub</strong>
            <span>Painel da empresa</span>
          </div>
        </div>

        <nav className="empresa-menu">

          <button className="empresa-menu-item ativo">
            <span>🏠</span>
            Início
          </button>

          <button className="empresa-menu-item">
            <span>🛒</span>
            Pedidos
          </button>

          <button className="empresa-menu-item">
            <span>📦</span>
            Produtos
          </button>

          <button className="empresa-menu-item">
            <span>📊</span>
            Estoque
          </button>

          <button className="empresa-menu-item">
            <span>💰</span>
            Financeiro
          </button>

          <button className="empresa-menu-item">
            <span>👥</span>
            Usuários
          </button>

          <button className="empresa-menu-item">
            <span>⚙️</span>
            Configurações
          </button>

        </nav>

        <button
          className="empresa-menu-sair"
          onClick={sair}
        >
          <span>🚪</span>
          Sair
        </button>

      </aside>

      <main className="empresa-conteudo">

        <header className="empresa-header">

          <div>
            <span className="empresa-header-pequeno">
              Painel administrativo
            </span>

            <h1>
              Olá, {usuario.nome} 👋
            </h1>

            <p>
              O que você deseja fazer hoje?
            </p>
          </div>

          <div className="empresa-usuario">
            <div className="empresa-avatar">
              {usuario.nome?.charAt(0)?.toUpperCase()}
            </div>

            <div>
              <strong>{usuario.nome}</strong>
              <span>Dono</span>
            </div>
          </div>

        </header>

        <section className="empresa-acoes">

          <button className="empresa-card acao-principal">
            <div className="empresa-card-icone">
              🛒
            </div>

            <div>
              <strong>Novo pedido</strong>
              <span>Registrar um novo pedido</span>
            </div>

            <b>→</b>
          </button>

          <button className="empresa-card">
            <div className="empresa-card-icone">
              📦
            </div>

            <div>
              <strong>Produtos</strong>
              <span>Cadastrar e gerenciar produtos</span>
            </div>

            <b>→</b>
          </button>

          <button className="empresa-card">
            <div className="empresa-card-icone">
              📊
            </div>

            <div>
              <strong>Estoque</strong>
              <span>Veja e controle seu estoque</span>
            </div>

            <b>→</b>
          </button>

          <button className="empresa-card">
            <div className="empresa-card-icone">
              💰
            </div>

            <div>
              <strong>Financeiro</strong>
              <span>Acompanhe suas movimentações</span>
            </div>

            <b>→</b>
          </button>

        </section>

        <section className="empresa-resumo">

          <div className="empresa-resumo-header">
            <div>
              <span>Visão geral</span>
              <h2>Resumo da empresa</h2>
            </div>

            <span className="empresa-status">
              ● Sistema ativo
            </span>
          </div>

          <div className="empresa-indicadores">

            <div>
              <span>Pedidos hoje</span>
              <strong>0</strong>
            </div>

            <div>
              <span>Produtos</span>
              <strong>0</strong>
            </div>

            <div>
              <span>Estoque baixo</span>
              <strong>0</strong>
            </div>

            <div>
              <span>Faturamento</span>
              <strong>R$ 0,00</strong>
            </div>

          </div>

        </section>

      </main>

    </div>
  )
}

export default EmpresaPainel

