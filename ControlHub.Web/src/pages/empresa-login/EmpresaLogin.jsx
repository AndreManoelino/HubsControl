
import { useEffect, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { api } from '../../api/api'
import './EmpresaLogin.css'

function EmpresaLogin() {
  const location = useLocation()
  const navigate = useNavigate()

  const [empresa, setEmpresa] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [mostrarSenha, setMostrarSenha] = useState(false)
  const [entrando, setEntrando] = useState(false)

  useEffect(() => {
    carregarEmpresa()
  }, [])

  async function carregarEmpresa() {
    try {
      setCarregando(true)
      setErro('')

      const urlEmpresa = location.pathname
        .replace('/', '')
        .trim()

      if (!urlEmpresa) {
        setErro('Empresa não identificada.')
        return
      }

      const dados = await api.get(`/Empresas/publica/${urlEmpresa}`)

      setEmpresa(dados)
    } catch (error) {
      setErro(
        error.message ||
          'Não foi possível carregar os dados da empresa.'
      )
    } finally {
      setCarregando(false)
    }
  }

  async function fazerLogin(event) {
    event.preventDefault()

    if (!email.trim()) {
      setErro('Informe seu e-mail.')
      return
    }

    if (!senha) {
      setErro('Informe sua senha.')
      return
    }

    try {
      setEntrando(true)
      setErro('')

      const resposta = await api.post('/Auth/login', {
        email: email.trim(),
        senha,
      })

      if (resposta.perfil !== 'Dono') {
        setErro(
          'Este acesso não pertence ao portal desta empresa.'
        )
        return
      }

      if (!resposta.empresaId) {
        setErro(
          'Este usuário não está vinculado a uma empresa.'
        )
        return
      }

      if (resposta.empresaId !== empresa.id) {
        setErro(
          'Este usuário não pertence a esta empresa.'
        )
        return
      }

      localStorage.setItem(
        'controlhub_token',
        resposta.token
      )

      localStorage.setItem(
        'controlhub_usuario',
        JSON.stringify({
          id: resposta.usuarioId,
          nome: resposta.nome,
          email: resposta.email,
          perfil: resposta.perfil,
          empresaId: resposta.empresaId,
        })
      )

      navigate(`/${empresa.url}/painel`)
    } catch (error) {
      setErro(
        error.message ||
          'Não foi possível realizar o login.'
      )
    } finally {
      setEntrando(false)
    }
  }

  if (carregando) {
    return (
      <div className="empresa-login-loading">
        <div className="empresa-login-loading-card">
          <div className="empresa-login-spinner"></div>

          <strong>Carregando seu acesso</strong>

          <p>
            Preparando o ambiente da empresa...
          </p>
        </div>
      </div>
    )
  }

  if (!empresa) {
    return (
      <div className="empresa-login-error-page">
        <div className="empresa-login-error-card">

          <div className="empresa-login-error-icon">
            !
          </div>

          <span className="empresa-login-error-label">
            Acesso indisponível
          </span>

          <h1>
            Empresa não encontrada
          </h1>

          <p>
            {erro ||
              'Não foi possível localizar esta empresa.'}
          </p>

          <button
            onClick={() => navigate('/')}
            className="empresa-login-error-button"
          >
            Voltar para o início
          </button>

        </div>
      </div>
    )
  }

  const nomeEmpresa =
    empresa.nomeFantasia || empresa.nome

  return (
    <div className="empresa-login-page">

      {/* =========================================
          LADO DA IMAGEM
      ========================================= */}

      <section
        className="empresa-login-imagem"
        style={{
          backgroundImage: empresa.imagemLoginUrl
            ? `url("${empresa.imagemLoginUrl}")`
            : 'linear-gradient(135deg, #111827, #374151)',
        }}
      >

        <div className="empresa-login-image-gradient"></div>

        <div className="empresa-login-image-glow"></div>

        <div className="empresa-login-image-content">

          <div className="empresa-login-brand">

            {empresa.logoUrl ? (
              <div className="empresa-login-logo-wrapper">
                <img
                  src={empresa.logoUrl}
                  alt={`Logo ${nomeEmpresa}`}
                  className="empresa-login-logo"
                />
              </div>
            ) : (
              <div className="empresa-login-logo-placeholder">
                {nomeEmpresa
                  .charAt(0)
                  .toUpperCase()}
              </div>
            )}

            <div className="empresa-login-brand-info">

              <span className="empresa-login-brand-label">
                Bem-vindo ao
              </span>

              <h1>
                {nomeEmpresa}
              </h1>

            </div>

          </div>

          <div className="empresa-login-image-message">

            <span className="empresa-login-image-line"></span>

            <p>
              Seu espaço de gestão,
              <br />
              simples e inteligente.
            </p>

          </div>

          <div className="empresa-login-image-footer">

            <span>
              Ambiente exclusivo da empresa
            </span>

            <span className="empresa-login-image-dot">
              ●
            </span>

            <span>
              Acesso seguro
            </span>

          </div>

        </div>

      </section>

      {/* =========================================
          LADO DO LOGIN
      ========================================= */}

      <section className="empresa-login-formulario">

        <div className="empresa-login-box">

          <div className="empresa-login-mobile-brand">

            {empresa.logoUrl ? (
              <img
                src={empresa.logoUrl}
                alt={`Logo ${nomeEmpresa}`}
              />
            ) : (
              <div>
                {nomeEmpresa
                  .charAt(0)
                  .toUpperCase()}
              </div>
            )}

          </div>

          <div className="empresa-login-titulo">

            <span className="empresa-login-welcome">
              Acesso do proprietário
            </span>

            <h2>
              Olá, seja bem-vindo!
            </h2>

            <p>
              Entre com seus dados para acessar
              o painel da sua empresa.
            </p>

          </div>

          <form
            onSubmit={fazerLogin}
            className="empresa-login-form"
          >

            {/* E-MAIL */}

            <div className="empresa-login-campo">

              <label htmlFor="email">
                E-mail
              </label>

              <div className="empresa-login-input-wrapper">

                <span className="empresa-login-input-icon">
                  <svg
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="1.8"
                  >
                    <rect
                      x="3"
                      y="5"
                      width="18"
                      height="14"
                      rx="2"
                    />

                    <path d="M3 7l9 6 9-6" />
                  </svg>
                </span>

                <input
                  id="email"
                  type="email"
                  placeholder="seu@email.com"
                  value={email}
                  onChange={(event) =>
                    setEmail(event.target.value)
                  }
                  autoComplete="email"
                  disabled={entrando}
                />

              </div>

            </div>

            {/* SENHA */}

            <div className="empresa-login-campo">

              <div className="empresa-login-label-row">

                <label htmlFor="senha">
                  Senha
                </label>

              </div>

              <div className="empresa-login-input-wrapper">

                <span className="empresa-login-input-icon">
                  <svg
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="1.8"
                  >
                    <rect
                      x="4"
                      y="10"
                      width="16"
                      height="11"
                      rx="2"
                    />

                    <path d="M8 10V7a4 4 0 018 0v3" />
                  </svg>
                </span>

                <input
                  id="senha"
                  type={
                    mostrarSenha
                      ? 'text'
                      : 'password'
                  }
                  placeholder="Digite sua senha"
                  value={senha}
                  onChange={(event) =>
                    setSenha(event.target.value)
                  }
                  autoComplete="current-password"
                  disabled={entrando}
                />

                <button
                  type="button"
                  className="empresa-login-password-toggle"
                  onClick={() =>
                    setMostrarSenha(
                      !mostrarSenha
                    )
                  }
                  tabIndex="-1"
                >
                  {mostrarSenha ? (
                    <svg
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="1.8"
                    >
                      <path d="M2 2l20 20" />
                      <path d="M6.7 6.7C4.2 8.2 2.7 10.2 2 12c1.7 4.1 5.4 7 10 7 1.8 0 3.4-.4 4.8-1.1" />
                      <path d="M9.9 4.3C10.6 4.1 11.3 4 12 4c4.6 0 8.3 2.9 10 8-.5 1.2-1.2 2.3-2 3.3" />
                      <path d="M14.1 14.1A3 3 0 019.9 9.9" />
                    </svg>
                  ) : (
                    <svg
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="1.8"
                    >
                      <path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12z" />
                      <circle
                        cx="12"
                        cy="12"
                        r="3"
                      />
                    </svg>
                  )}
                </button>

              </div>

            </div>

            {/* ERRO */}

            {erro && (
              <div className="empresa-login-mensagem">

                <div className="empresa-login-mensagem-icon">
                  !
                </div>

                <span>
                  {erro}
                </span>

              </div>
            )}

            {/* BOTÃO */}

            <button
              type="submit"
              className="empresa-login-botao"
              disabled={entrando}
            >

              {entrando ? (
                <>
                  <span className="empresa-login-button-spinner"></span>
                  Entrando...
                </>
              ) : (
                <>
                  Entrar no painel

                  <svg
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                  >
                    <path d="M5 12h14" />
                    <path d="M13 6l6 6-6 6" />
                  </svg>
                </>
              )}

            </button>

          </form>

          <div className="empresa-login-security">

            <div className="empresa-login-security-icon">

              <svg
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="1.8"
              >
                <path d="M12 3l7 3v5c0 4.6-3 8.4-7 10-4-1.6-7-5.4-7-10V6l7-3z" />
                <path d="M9 12l2 2 4-4" />
              </svg>

            </div>

            <div>
              <strong>
                Ambiente seguro
              </strong>

              <span>
                Seus dados são protegidos durante o acesso.
              </span>
            </div>

          </div>

          <div className="empresa-login-rodape">

            <span>
              {nomeEmpresa}
            </span>

            <span className="empresa-login-rodape-separador">
              •
            </span>

            <span>
              Portal administrativo
            </span>

          </div>

        </div>

      </section>

    </div>
  )
}

export default EmpresaLogin

