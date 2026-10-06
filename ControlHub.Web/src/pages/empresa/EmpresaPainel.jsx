import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import Sidebar from './components/Sidebar/Sidebar'
import Header from './components/Header/Header'
import { menusEmpresa } from './config/menuconfig'
import { api } from "../../api/api";
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

  if (!usuario) {
    return null
  }

  const menuAtual =
    menusEmpresa.find((menu) => menu.id === secaoAtiva) ||
    menusEmpresa[0]

  const TelaAtual = menuAtual.componente

  return (
    <div className="empresa-painel">
      <Sidebar
        aberto={menuAberto}
        setAberto={setMenuAberto}
        secaoAtiva={secaoAtiva}
        setSecaoAtiva={setSecaoAtiva}
        usuario={usuario}
        sair={sair}
      />

      <main
        className={`empresa-conteudo ${
          menuAberto ? 'menu-aberto' : 'menu-fechado'
        }`}
      >
        <Header
          usuario={usuario}
          setMenuAberto={setMenuAberto}
        />

        <div className="empresa-area">
          <TelaAtual />
        </div>
      </main>
    </div>
  )
}

export default EmpresaPainel