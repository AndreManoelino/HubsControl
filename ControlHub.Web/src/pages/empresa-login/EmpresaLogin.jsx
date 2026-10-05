
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
        error.message || 'Não foi possível carregar os dados da empresa.'
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
      setErro('Este acesso não pertence ao portal desta empresa.')
      return
    }

    if (!resposta.empresaId) {
      setErro('Este usuário não está vinculado a uma empresa.')
      return
    }

    if (resposta.empresaId !== empresa.id) {
      setErro('Este usuário não pertence a esta empresa.')
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
      error.message || 'Não foi possível realizar o login.'
    )
  } finally {
    setEntrando(false)
  }
}

  if (carregando) {
    return (
      <div className="empresa-login-loading">
        <div className="empresa-login-spinner"></div>
        <p>Carregando...</p>
      </div>
    )
  }

  if (!empresa) {
    return (
      <div className="empresa-login-error-page">
        <div className="empresa-login-error-card">
          <h1>Ops!</h1>
          <p>{erro || 'Empresa não encontrada.'}</p>
          <button onClick={() => navigate('/')}>
            Voltar
          </button>
        </div>
      </div>
    )
  }

  return (
    <div className="empresa-login-page">

      <section
        className="empresa-login-imagem"
        style={{
          backgroundImage: empresa.imagemLoginUrl
            ? `url("${empresa.imagemLoginUrl}")`
            : 'none',
        }}
      >
        <div className="empresa-login-overlay"></div>

        <div className="empresa-login-marca">

          {empresa.logoUrl && (
            <img
              src={empresa.logoUrl}
              alt={`Logo ${empresa.nomeFantasia || empresa.nome}`}
              className="empresa-login-logo"
            />
          )}

          <div>
            <h1>
              {empresa.nomeFantasia || empresa.nome}
            </h1>

            <p>
              Seu espaço de gestão empresarial
            </p>
          </div>

        </div>
      </section>

      <section className="empresa-login-formulario">

        <div className="empresa-login-box">

          {empresa.logoUrl && (
            <img
              src={empresa.logoUrl}
              alt="Logo"
              className="empresa-login-logo-mobile"
            />
          )}

          <div className="empresa-login-titulo">
            <span>Bem-vindo</span>

            <h2>
              Acesse sua conta
            </h2>

            <p>
              Entre com seus dados para continuar.
            </p>
          </div>

          <form onSubmit={fazerLogin}>

            <div className="empresa-login-campo">
              <label htmlFor="email">
                E-mail
              </label>

              <input
                id="email"
                type="email"
                placeholder="Digite seu e-mail"
                value={email}
                onChange={(event) =>
                  setEmail(event.target.value)
                }
                autoComplete="email"
              />
            </div>

            <div className="empresa-login-campo">
              <label htmlFor="senha">
                Senha
              </label>

              <input
                id="senha"
                type="password"
                placeholder="Digite sua senha"
                value={senha}
                onChange={(event) =>
                  setSenha(event.target.value)
                }
                autoComplete="current-password"
              />
            </div>

            {erro && (
              <div className="empresa-login-mensagem">
                {erro}
              </div>
            )}

            <button
              type="submit"
              className="empresa-login-botao"
              disabled={entrando}
            >
              {entrando ? 'Entrando...' : 'Entrar'}
            </button>

          </form>

          <div className="empresa-login-rodape">
            <span>
              {empresa.nomeFantasia || empresa.nome}
            </span>
          </div>

        </div>

      </section>

    </div>
  )
}

export default EmpresaLogin

