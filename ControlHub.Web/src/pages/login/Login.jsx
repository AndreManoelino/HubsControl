import './Login.css'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../../api/api'

function Login() {
  const navigate = useNavigate()

  const [login, setLogin] = useState('')
  const [senha, setSenha] = useState('')
  const [carregando, setCarregando] = useState(false)
  const [erro, setErro] = useState('')

  async function handleLogin(event) {
    event.preventDefault()

    setErro('')

    if (!login.trim()) {
      setErro('Informe seu e-mail.')
      return
    }

    if (!senha.trim()) {
      setErro('Informe sua senha.')
      return
    }

    setCarregando(true)

    try {
      const resposta = await api.post('/Auth/login', {
        email: login.trim(),
        senha,
      })

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

      const perfil = resposta.perfil

      if (perfil === 'Master' || perfil === 1) {
        navigate('/master')
        return
      }

      if (perfil === 'Dono' || perfil === 2) {
        navigate('/dono')
        return
      }

      if (perfil === 'Cliente' || perfil === 3) {
        navigate('/cliente')
        return
      }

      setErro('Perfil de usuário não identificado.')
    } catch (error) {
      setErro(
        error?.message ||
        'Não foi possível realizar o login.'
      )
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="login-page">
      <div className="login-card">

        <div className="login-header">

          <div className="login-logo">
            Control<span>Hub</span>
          </div>

          <h1>Entrar</h1>

          <p>
            Acesse o painel administrativo do ControlHub.
          </p>

        </div>

        <form onSubmit={handleLogin}>

          <div className="form-group">

            <label htmlFor="login">
              E-mail
            </label>

            <input
              id="login"
              type="email"
              value={login}
              onChange={(event) => setLogin(event.target.value)}
              placeholder="Digite seu e-mail"
              autoComplete="username"
              disabled={carregando}
              required
            />

          </div>

          <div className="form-group">

            <label htmlFor="senha">
              Senha
            </label>

            <input
              id="senha"
              type="password"
              value={senha}
              onChange={(event) => setSenha(event.target.value)}
              placeholder="Digite sua senha"
              autoComplete="current-password"
              disabled={carregando}
              required
            />

          </div>

          {erro && (
            <div className="login-error">
              {erro}
            </div>
          )}

          <button
            type="submit"
            className="login-submit"
            disabled={carregando}
          >
            {carregando ? 'Entrando...' : 'Entrar'}
          </button>

        </form>

        <button
          type="button"
          className="back-home"
          onClick={() => navigate('/')}
          disabled={carregando}
        >
          Voltar para o início
        </button>

      </div>
    </div>
  )
}

export default Login