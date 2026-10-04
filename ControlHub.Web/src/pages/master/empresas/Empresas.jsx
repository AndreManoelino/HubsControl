import './Empresas.css'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../../../api/api'

function Empresas() {
  const navigate = useNavigate()

  const [empresas, setEmpresas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [mensagem, setMensagem] = useState('')

  const [tipoBusca, setTipoBusca] = useState('empresa')
  const [termoBusca, setTermoBusca] = useState('')

  const [empresaSelecionada, setEmpresaSelecionada] = useState(null)
  const [empresaEditando, setEmpresaEditando] = useState(null)
  const [criandoEmpresa, setCriandoEmpresa] = useState(false)

  const [carregandoBusca, setCarregandoBusca] = useState(false)
  const [salvando, setSalvando] = useState(false)
  const [bloqueando, setBloqueando] = useState(false)

  const [formulario, setFormulario] = useState({
    cpf: '',
    nome: '',
    nomeFantasia: '',
    logoUrl: '',
    imagemLoginUrl: '',
    url: '',
    nomeDono: '',
    emailDono: '',
    senhaDono: '',
  })

  useEffect(() => {
    carregarEmpresas()
  }, [])

  async function carregarEmpresas() {
    setCarregando(true)
    setErro('')

    try {
      const resposta = await api.get('/Empresas')

      setEmpresas(Array.isArray(resposta) ? resposta : [])
    } catch (error) {
      setErro(
        error?.message ||
        'Não foi possível carregar as empresas.'
      )
    } finally {
      setCarregando(false)
    }
  }

  async function buscar() {
    const termo = termoBusca.trim()

    if (!termo) {
      carregarEmpresas()
      return
    }

    setCarregandoBusca(true)
    setErro('')
    setMensagem('')

    try {
      let resposta

      if (tipoBusca === 'empresa') {
        resposta = await api.get(
          `/Empresas/buscar?nome=${encodeURIComponent(termo)}`
        )
      }

      if (tipoBusca === 'dono') {
        resposta = await api.get(
          `/Empresas/buscar/dono?nome=${encodeURIComponent(termo)}`
        )
      }

      if (tipoBusca === 'id') {
        resposta = await api.get(
          `/Empresas/${encodeURIComponent(termo)}`
        )

        resposta = resposta ? [resposta] : []
      }

      setEmpresas(
        Array.isArray(resposta)
          ? resposta
          : []
      )
    } catch (error) {
      setEmpresas([])

      setErro(
        error?.message ||
        'Nenhuma empresa encontrada.'
      )
    } finally {
      setCarregandoBusca(false)
    }
  }

  function limparBusca() {
    setTermoBusca('')
    setErro('')
    carregarEmpresas()
  }

  function abrirEmpresa(empresa) {
    setEmpresaSelecionada(empresa)
  }

  function fecharDetalhes() {
    setEmpresaSelecionada(null)
  }

  function abrirEdicao(empresa) {
    setErro('')
    setMensagem('')

    setEmpresaEditando(empresa)

    setFormulario({
      cpf: empresa.cpf || '',
      nome: empresa.nome || '',
      nomeFantasia: empresa.nomeFantasia || '',
      logoUrl: empresa.logoUrl || '',
      imagemLoginUrl: empresa.imagemLoginUrl || '',
      url: empresa.url || '',
      nomeDono: empresa.dono?.nome || '',
      emailDono: empresa.dono?.email || '',
      senhaDono: '',
    })
  }

  function fecharEdicao() {
    if (salvando) return

    setEmpresaEditando(null)
  }

  function abrirCriacao() {
    setErro('')
    setMensagem('')

    setFormulario({
      cpf: '',
      nome: '',
      nomeFantasia: '',
      logoUrl: '',
      imagemLoginUrl: '',
      url: '',
      nomeDono: '',
      emailDono: '',
      senhaDono: '',
    })

    setCriandoEmpresa(true)
  }

  function fecharCriacao() {
    if (salvando) return

    setCriandoEmpresa(false)
  }

  function alterarFormulario(event) {
    const { name, value } = event.target

    setFormulario((estadoAtual) => ({
      ...estadoAtual,
      [name]: value,
    }))
  }

  async function criarEmpresa(event) {
    event.preventDefault()

    setSalvando(true)
    setErro('')
    setMensagem('')

    try {
      await api.post(
        '/Empresas',
        formulario
      )

      setMensagem(
        'Empresa criada com sucesso.'
      )

      setCriandoEmpresa(false)

      await carregarEmpresas()
    } catch (error) {
      setErro(
        error?.message ||
        'Não foi possível criar a empresa.'
      )
    } finally {
      setSalvando(false)
    }
  }

  async function salvarEdicao(event) {
    event.preventDefault()

    if (!empresaEditando) return

    setSalvando(true)
    setErro('')
    setMensagem('')

    try {
      const resposta = await api.put(
        `/Empresas/${empresaEditando.id}`,
        formulario
      )

      setMensagem(
        resposta?.mensagem ||
        'Empresa atualizada com sucesso.'
      )

      setEmpresaEditando(null)

      await carregarEmpresas()
    } catch (error) {
      setErro(
        error?.message ||
        'Não foi possível atualizar a empresa.'
      )
    } finally {
      setSalvando(false)
    }
  }

  async function bloquearEmpresa(empresa) {
    const confirmou = window.confirm(
      `Deseja realmente bloquear a empresa "${empresa.nome}"?`
    )

    if (!confirmou) return

    setBloqueando(true)
    setErro('')
    setMensagem('')

    try {
      const resposta = await api.put(
        `/Empresas/${empresa.id}/bloquear`
      )

      setMensagem(
        resposta?.mensagem ||
        'Empresa bloqueada com sucesso.'
      )

      setEmpresaSelecionada(null)

      await carregarEmpresas()
    } catch (error) {
      setErro(
        error?.message ||
        'Não foi possível bloquear a empresa.'
      )
    } finally {
      setBloqueando(false)
    }
  }

  function formatarData(data) {
    if (!data) return '-'

    return new Date(data).toLocaleDateString(
      'pt-BR',
      {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
      }
    )
  }

  return (
    <div className="empresas-page">

      <header className="empresas-header">

        <div className="empresas-header-left">

          <button
            type="button"
            className="empresas-back"
            onClick={() => navigate('/master')}
          >
            ←
          </button>

          <div>
            <div className="empresas-brand">
              Control<span>Hub</span>
            </div>

            <div className="empresas-header-title">
              Gerenciamento de empresas
            </div>
          </div>

        </div>

        <button
          type="button"
          className="empresas-create-button"
          onClick={abrirCriacao}
        >
          <span>+</span>
          Criar empresa
        </button>

      </header>

      <main className="empresas-content">

        <section className="empresas-intro">

          <div>

            <span className="empresas-label">
              MASTER / EMPRESAS
            </span>

            <h1>
              Empresas
            </h1>

            <p>
              Gerencie todas as empresas cadastradas no ControlHub.
            </p>

          </div>

          <div className="empresas-total">

            <strong>
              {empresas.length}
            </strong>

            <span>
              {empresas.length === 1
                ? 'empresa encontrada'
                : 'empresas encontradas'}
            </span>

          </div>

        </section>

        {erro && (
          <div className="empresas-alert empresas-alert-error">
            <span>!</span>
            {erro}
          </div>
        )}

        {mensagem && (
          <div className="empresas-alert empresas-alert-success">
            <span>✓</span>
            {mensagem}
          </div>
        )}

        <section className="empresas-search">

          <div className="empresas-search-title">

            <span>
              🔎
            </span>

            <div>

              <strong>
                Pesquisar empresas
              </strong>

              <small>
                Utilize os filtros para localizar uma empresa.
              </small>

            </div>

          </div>

          <div className="empresas-search-controls">

            <select
              value={tipoBusca}
              onChange={(event) =>
                setTipoBusca(event.target.value)
              }
            >
              <option value="empresa">
                Nome da empresa
              </option>

              <option value="dono">
                Nome do dono
              </option>

              <option value="id">
                ID da empresa
              </option>
            </select>

            <input
              type="text"
              value={termoBusca}
              onChange={(event) =>
                setTermoBusca(event.target.value)
              }
              onKeyDown={(event) => {
                if (event.key === 'Enter') {
                  buscar()
                }
              }}
              placeholder={
                tipoBusca === 'empresa'
                  ? 'Digite o nome da empresa'
                  : tipoBusca === 'dono'
                    ? 'Digite o nome do dono'
                    : 'Digite o ID da empresa'
              }
            />

            <button
              type="button"
              className="empresas-search-button"
              onClick={buscar}
              disabled={carregandoBusca}
            >
              {carregandoBusca
                ? 'Buscando...'
                : 'Buscar'}
            </button>

            <button
              type="button"
              className="empresas-clear-button"
              onClick={limparBusca}
            >
              Limpar
            </button>

          </div>

        </section>

        <section className="empresas-list-section">

          <div className="empresas-list-header">

            <div>

              <span className="empresas-label">
                CADASTRO
              </span>

              <h2>
                Empresas cadastradas
              </h2>

            </div>

            <button
              type="button"
              className="empresas-refresh"
              onClick={carregarEmpresas}
              disabled={carregando}
            >
              ↻ Atualizar
            </button>

          </div>

          {carregando ? (

            <div className="empresas-loading">

              <div className="empresas-spinner" />

              <span>
                Carregando empresas...
              </span>

            </div>

          ) : empresas.length === 0 ? (

            <div className="empresas-empty">

              <div className="empresas-empty-icon">
                ◇
              </div>

              <h3>
                Nenhuma empresa encontrada
              </h3>

              <p>
                Não existem empresas para os filtros utilizados.
              </p>

              <button
                type="button"
                onClick={abrirCriacao}
              >
                Criar primeira empresa
              </button>

            </div>

          ) : (

            <div className="empresas-grid">

              {empresas.map((empresa) => (

                <article
                  className="empresa-card"
                  key={empresa.id}
                >

                  <div className="empresa-card-top">

                    <div className="empresa-logo">

                      {empresa.logoUrl ? (

                        <img
                          src={empresa.logoUrl}
                          alt={
                            empresa.nomeFantasia ||
                            empresa.nome
                          }
                        />

                      ) : (

                        <span>
                          {(
                            empresa.nomeFantasia ||
                            empresa.nome ||
                            'E'
                          )
                            .charAt(0)
                            .toUpperCase()}
                        </span>

                      )}

                    </div>

                    <div className="empresa-status">

                      <span
                        className={
                          empresa.ativa
                            ? 'status-dot status-active'
                            : 'status-dot status-blocked'
                        }
                      />

                      {empresa.ativa
                        ? 'Ativa'
                        : 'Bloqueada'}

                    </div>

                  </div>

                  <div className="empresa-card-body">

                    <h3>
                      {empresa.nomeFantasia ||
                        empresa.nome}
                    </h3>

                    {empresa.nomeFantasia && (
                      <p className="empresa-razao">
                        {empresa.nome}
                      </p>
                    )}

                    <div className="empresa-info">

                      <div>
                        <span>
                          CPF/CNPJ
                        </span>

                        <strong>
                          {empresa.cpf || '-'}
                        </strong>
                      </div>

                      <div>
                        <span>
                          Dono
                        </span>

                        <strong>
                          {empresa.dono?.nome || '-'}
                        </strong>
                      </div>

                      <div>
                        <span>
                          URL
                        </span>

                        <strong>
                          {empresa.url || '-'}
                        </strong>
                      </div>

                      <div>
                        <span>
                          Cadastro
                        </span>

                        <strong>
                          {formatarData(
                            empresa.criadaEm
                          )}
                        </strong>
                      </div>

                    </div>

                  </div>

                  <div className="empresa-card-actions">

                    <button
                      type="button"
                      className="empresa-action-view"
                      onClick={() =>
                        abrirEmpresa(empresa)
                      }
                    >
                      Visualizar
                    </button>

                    <button
                      type="button"
                      className="empresa-action-edit"
                      onClick={() =>
                        abrirEdicao(empresa)
                      }
                    >
                      Editar
                    </button>

                    {empresa.ativa && (
                      <button
                        type="button"
                        className="empresa-action-block"
                        onClick={() =>
                          bloquearEmpresa(empresa)
                        }
                        disabled={bloqueando}
                      >
                        Bloquear
                      </button>
                    )}

                  </div>

                </article>

              ))}

            </div>

          )}

        </section>

      </main>

      {/* =====================================================
          MODAL - DETALHES
      ====================================================== */}

      {empresaSelecionada && (

        <div
          className="empresa-modal-overlay"
          onMouseDown={fecharDetalhes}
        >

          <div
            className="empresa-modal"
            onMouseDown={(event) =>
              event.stopPropagation()
            }
          >

            <div className="empresa-modal-header">

              <div>

                <span className="empresas-label">
                  DETALHES DA EMPRESA
                </span>

                <h2>
                  {empresaSelecionada.nomeFantasia ||
                    empresaSelecionada.nome}
                </h2>

              </div>

              <button
                type="button"
                onClick={fecharDetalhes}
              >
                ×
              </button>

            </div>

            <div className="empresa-detail-logo">

              {empresaSelecionada.logoUrl ? (

                <img
                  src={empresaSelecionada.logoUrl}
                  alt={empresaSelecionada.nome}
                />

              ) : (

                <span>
                  {(
                    empresaSelecionada.nomeFantasia ||
                    empresaSelecionada.nome ||
                    'E'
                  )
                    .charAt(0)
                    .toUpperCase()}
                </span>

              )}

            </div>

            <div className="empresa-details-grid">

              <div>
                <span>Nome</span>
                <strong>
                  {empresaSelecionada.nome}
                </strong>
              </div>

              <div>
                <span>Nome fantasia</span>
                <strong>
                  {empresaSelecionada.nomeFantasia || '-'}
                </strong>
              </div>

              <div>
                <span>CPF/CNPJ</span>
                <strong>
                  {empresaSelecionada.cpf || '-'}
                </strong>
              </div>

              <div>
                <span>URL</span>
                <strong>
                  {empresaSelecionada.url || '-'}
                </strong>
              </div>

              <div>
                <span>Status</span>
                <strong>
                  {empresaSelecionada.ativa
                    ? 'Ativa'
                    : 'Bloqueada'}
                </strong>
              </div>

              <div>
                <span>Criada em</span>
                <strong>
                  {formatarData(
                    empresaSelecionada.criadaEm
                  )}
                </strong>
              </div>

              <div>
                <span>ID da empresa</span>
                <strong className="empresa-id">
                  {empresaSelecionada.id}
                </strong>
              </div>

              <div>
                <span>ID do dono</span>
                <strong className="empresa-id">
                  {empresaSelecionada.donoId || '-'}
                </strong>
              </div>

            </div>

            <div className="empresa-owner-box">

              <div className="empresa-owner-title">
                Dono da empresa
              </div>

              {empresaSelecionada.dono ? (

                <div className="empresa-owner-content">

                  <div className="empresa-owner-avatar">
                    {empresaSelecionada.dono.nome
                      ?.charAt(0)
                      .toUpperCase()}
                  </div>

                  <div>

                    <strong>
                      {empresaSelecionada.dono.nome}
                    </strong>

                    <span>
                      {empresaSelecionada.dono.email}
                    </span>

                    <small>
                      Perfil: {empresaSelecionada.dono.perfil}
                    </small>

                  </div>

                </div>

              ) : (

                <span className="empresa-owner-empty">
                  Nenhum dono vinculado.
                </span>

              )}

            </div>

            <div className="empresa-modal-actions">

              <button
                type="button"
                className="empresa-action-edit"
                onClick={() => {
                  fecharDetalhes()
                  abrirEdicao(empresaSelecionada)
                }}
              >
                Editar empresa
              </button>

              {empresaSelecionada.ativa && (
                <button
                  type="button"
                  className="empresa-action-block"
                  onClick={() =>
                    bloquearEmpresa(
                      empresaSelecionada
                    )
                  }
                  disabled={bloqueando}
                >
                  {bloqueando
                    ? 'Bloqueando...'
                    : 'Bloquear empresa'}
                </button>
              )}

            </div>

          </div>

        </div>

      )}

      {/* =====================================================
          MODAL - EDIÇÃO
      ====================================================== */}

      {empresaEditando && (

        <div
          className="empresa-modal-overlay"
          onMouseDown={fecharEdicao}
        >

          <div
            className="empresa-modal empresa-modal-large"
            onMouseDown={(event) =>
              event.stopPropagation()
            }
          >

            <div className="empresa-modal-header">

              <div>

                <span className="empresas-label">
                  EDITAR EMPRESA
                </span>

                <h2>
                  {empresaEditando.nomeFantasia ||
                    empresaEditando.nome}
                </h2>

              </div>

              <button
                type="button"
                onClick={fecharEdicao}
              >
                ×
              </button>

            </div>

            <form onSubmit={salvarEdicao}>

              <div className="empresa-form-grid">

                <div className="empresa-form-group">

                  <label>
                    CPF/CNPJ
                  </label>

                  <input
                    name="cpf"
                    value={formulario.cpf}
                    onChange={alterarFormulario}
                    required
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    Nome da empresa
                  </label>

                  <input
                    name="nome"
                    value={formulario.nome}
                    onChange={alterarFormulario}
                    required
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    Nome fantasia
                  </label>

                  <input
                    name="nomeFantasia"
                    value={formulario.nomeFantasia}
                    onChange={alterarFormulario}
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    URL da empresa
                  </label>

                  <input
                    name="url"
                    value={formulario.url}
                    onChange={alterarFormulario}
                    required
                  />

                </div>

                <div className="empresa-form-group empresa-form-full">

                  <label>
                    Logo URL
                  </label>

                  <input
                    name="logoUrl"
                    value={formulario.logoUrl}
                    onChange={alterarFormulario}
                    placeholder="https://..."
                  />

                </div>

                <div className="empresa-form-group empresa-form-full">

                  <label>
                    Imagem da tela de login
                  </label>

                  <input
                    name="imagemLoginUrl"
                    value={formulario.imagemLoginUrl}
                    onChange={alterarFormulario}
                    placeholder="https://..."
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    Nome do dono
                  </label>

                  <input
                    name="nomeDono"
                    value={formulario.nomeDono}
                    onChange={alterarFormulario}
                    required
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    E-mail do dono
                  </label>

                  <input
                    name="emailDono"
                    type="email"
                    value={formulario.emailDono}
                    onChange={alterarFormulario}
                    required
                  />

                </div>

                <div className="empresa-form-group empresa-form-full">

                  <label>
                    Nova senha do dono
                  </label>

                  <input
                    name="senhaDono"
                    type="password"
                    value={formulario.senhaDono}
                    onChange={alterarFormulario}
                    placeholder="Deixe vazio se não quiser alterar"
                  />

                </div>

              </div>

              <div className="empresa-modal-actions">

                <button
                  type="button"
                  className="empresa-cancel-button"
                  onClick={fecharEdicao}
                  disabled={salvando}
                >
                  Cancelar
                </button>

                <button
                  type="submit"
                  className="empresa-save-button"
                  disabled={salvando}
                >
                  {salvando
                    ? 'Salvando...'
                    : 'Salvar alterações'}
                </button>

              </div>

            </form>

          </div>

        </div>

      )}

      {/* =====================================================
          MODAL - CRIAR EMPRESA
      ====================================================== */}

      {criandoEmpresa && (

        <div
          className="empresa-modal-overlay"
          onMouseDown={fecharCriacao}
        >

          <div
            className="empresa-modal empresa-modal-large"
            onMouseDown={(event) =>
              event.stopPropagation()
            }
          >

            <div className="empresa-modal-header">

              <div>

                <span className="empresas-label">
                  NOVA EMPRESA
                </span>

                <h2>
                  Criar empresa
                </h2>

              </div>

              <button
                type="button"
                onClick={fecharCriacao}
                disabled={salvando}
              >
                ×
              </button>

            </div>

            <form onSubmit={criarEmpresa}>

              <div className="empresa-form-grid">

                <div className="empresa-form-group">

                  <label>
                    CPF/CNPJ
                  </label>

                  <input
                    name="cpf"
                    value={formulario.cpf}
                    onChange={alterarFormulario}
                    placeholder="Digite o CPF/CNPJ"
                    required
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    Nome da empresa
                  </label>

                  <input
                    name="nome"
                    value={formulario.nome}
                    onChange={alterarFormulario}
                    placeholder="Razão social"
                    required
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    Nome fantasia
                  </label>

                  <input
                    name="nomeFantasia"
                    value={formulario.nomeFantasia}
                    onChange={alterarFormulario}
                    placeholder="Nome fantasia"
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    URL da empresa
                  </label>

                  <input
                    name="url"
                    value={formulario.url}
                    onChange={alterarFormulario}
                    placeholder="exemplo.controlhub.com"
                    required
                  />

                </div>

                <div className="empresa-form-group empresa-form-full">

                  <label>
                    Logo URL
                  </label>

                  <input
                    name="logoUrl"
                    value={formulario.logoUrl}
                    onChange={alterarFormulario}
                    placeholder="https://..."
                  />

                </div>

                <div className="empresa-form-group empresa-form-full">

                  <label>
                    Imagem da tela de login
                  </label>

                  <input
                    name="imagemLoginUrl"
                    value={formulario.imagemLoginUrl}
                    onChange={alterarFormulario}
                    placeholder="https://..."
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    Nome do dono
                  </label>

                  <input
                    name="nomeDono"
                    value={formulario.nomeDono}
                    onChange={alterarFormulario}
                    placeholder="Nome completo"
                    required
                  />

                </div>

                <div className="empresa-form-group">

                  <label>
                    E-mail do dono
                  </label>

                  <input
                    name="emailDono"
                    type="email"
                    value={formulario.emailDono}
                    onChange={alterarFormulario}
                    placeholder="email@empresa.com"
                    required
                  />

                </div>

                <div className="empresa-form-group empresa-form-full">

                  <label>
                    Senha do dono
                  </label>

                  <input
                    name="senhaDono"
                    type="password"
                    value={formulario.senhaDono}
                    onChange={alterarFormulario}
                    placeholder="Digite a senha inicial"
                    required
                  />

                </div>

              </div>

              <div className="empresa-modal-actions">

                <button
                  type="button"
                  className="empresa-cancel-button"
                  onClick={fecharCriacao}
                  disabled={salvando}
                >
                  Cancelar
                </button>

                <button
                  type="submit"
                  className="empresa-save-button"
                  disabled={salvando}
                >
                  {salvando
                    ? 'Criando...'
                    : 'Criar empresa'}
                </button>

              </div>

            </form>

          </div>

        </div>

      )}

    </div>
  )
}

export default Empresas