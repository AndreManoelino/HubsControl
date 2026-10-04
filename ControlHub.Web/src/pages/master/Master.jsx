import './Master.css'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../../api/api'

function Master() {
  const navigate = useNavigate()

  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    const usuarioSalvo = localStorage.getItem('controlhub_usuario')
    const token = localStorage.getItem('controlhub_token')

    if (!token || !usuarioSalvo) {
      navigate('/login')
      return
    }

    try {
      const usuarioAtual = JSON.parse(usuarioSalvo)

      if (
        usuarioAtual.perfil !== 'Master' &&
        usuarioAtual.perfil !== 1
      ) {
        navigate('/login')
        return
      }

      setUsuario(usuarioAtual)
    } catch {
      localStorage.removeItem('controlhub_usuario')
      navigate('/login')
    } finally {
      setCarregando(false)
    }
  }, [navigate])

  function sair() {
    localStorage.removeItem('controlhub_token')
    localStorage.removeItem('controlhub_usuario')

    navigate('/login')
  }

  if (carregando) {
    return (
      <div className="master-page">
        <div className="master-loading">
          Carregando...
        </div>
      </div>
    )
  }

  return (
    <div className="master-page">

      <header className="master-header">

        <div className="master-brand">
          <div className="master-logo">
            Control<span>Hub</span>
          </div>

          <span className="master-badge">
            MASTER
          </span>
        </div>

        <div className="master-user">

          <div className="master-user-info">
            <strong>
              {usuario?.nome || 'Master'}
            </strong>

            <span>
              {usuario?.email || ''}
            </span>
          </div>

          <button
            type="button"
            className="master-logout"
            onClick={sair}
          >
            Sair
          </button>

        </div>

      </header>

      <main className="master-content">

        <section className="master-welcome">

          <span className="master-section-label">
            PAINEL ADMINISTRATIVO
          </span>

          <h1>
            Bem-vindo ao ControlHub
          </h1>

          <p>
            Gerencie empresas, usuários e acessos da plataforma.
          </p>

        </section>

        <section className="master-section">

          <div className="master-section-title">
            <div>
              <span className="master-section-label">
                EMPRESAS
              </span>

              <h2>
                Gerenciamento de empresas
              </h2>
            </div>
          </div>

          <div className="master-grid">

            <button
              type="button"
              className="master-card"
              onClick={() => navigate('/master/empresas/criar')}
            >
              <span className="master-card-icon">
                +
              </span>

              <strong>
                Criar empresa
              </strong>

              <span>
                Cadastrar uma nova empresa e seu dono.
              </span>
            </button>

            <button
              type="button"
              className="master-card"
              onClick={() => navigate('/master/empresas')}
            >
              <span className="master-card-icon">
                ≡
              </span>

              <strong>
                Buscar empresas
              </strong>

              <span>
                Consultar e visualizar empresas cadastradas.
              </span>
            </button>

            <button
              type="button"
              className="master-card"
              onClick={() => navigate('/master/empresas/editar')}
            >
              <span className="master-card-icon">
                ✎
              </span>

              <strong>
                Editar empresa
              </strong>

              <span>
                Alterar informações de uma empresa.
              </span>
            </button>

            <button
              type="button"
              className="master-card master-card-danger"
              onClick={() => navigate('/master/empresas/bloquear')}
            >
              <span className="master-card-icon">
                !
              </span>

              <strong>
                Bloquear empresa
              </strong>

              <span>
                Bloquear o acesso de uma empresa.
              </span>
            </button>

          </div>

        </section>

        <section className="master-section">

          <div className="master-section-title">
            <div>
              <span className="master-section-label">
                USUÁRIOS
              </span>

              <h2>
                Gerenciamento de usuários
              </h2>
            </div>
          </div>

          <div className="master-grid">

            <button
              type="button"
              className="master-card"
              onClick={() => navigate('/master/usuarios/criar')}
            >
              <span className="master-card-icon">
                +
              </span>

              <strong>
                Criar usuário
              </strong>

              <span>
                Cadastrar um novo usuário no sistema.
              </span>
            </button>

            <button
              type="button"
              className="master-card"
              onClick={() => navigate('/master/usuarios')}
            >
              <span className="master-card-icon">
                ≡
              </span>

              <strong>
                Buscar usuários
              </strong>

              <span>
                Consultar os usuários cadastrados.
              </span>
            </button>

          </div>

        </section>

      </main>

    </div>
  )
}

export default Master