
import { useEffect, useState } from 'react'
import { api } from '../../../../api/api'
import './Secoes.css'

function Secoes() {
  const [secoes, setSecoes] = useState([])
  const [carregando, setCarregando] = useState(false)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')
  const [modalAberto, setModalAberto] = useState(false)
  const [editando, setEditando] = useState(null)
  const [nome, setNome] = useState('')

  async function carregarSecoes() {
    try {
      setCarregando(true)
      setErro('')

      const dados = await api.get('/Secoes')

      setSecoes(dados)
    } catch (error) {
      setErro(error.message)
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarSecoes()
  }, [])

  function abrirNovaSecao() {
    setErro('')
    setEditando(null)
    setNome('')
    setModalAberto(true)
  }

  function abrirEdicao(secao) {
    setErro('')
    setEditando(secao)
    setNome(secao.nome)
    setModalAberto(true)
  }

  function fecharModal() {
    if (salvando) return

    setModalAberto(false)
    setEditando(null)
    setNome('')
    setErro('')
  }

  async function salvar() {
    const nomeLimpo = nome.trim()

    if (!nomeLimpo) {
      setErro('Digite o nome da seção.')
      return
    }

    try {
      setSalvando(true)
      setErro('')

      if (editando) {
        await api.put(`/Secoes/${editando.id}`, {
          nome: nomeLimpo,
          ativa: editando.ativa,
        })
      } else {
        await api.post('/Secoes', {
          nome: nomeLimpo,
        })
      }

      setModalAberto(false)
      setEditando(null)
      setNome('')

      await carregarSecoes()
    } catch (error) {
      setErro(error.message)
    } finally {
      setSalvando(false)
    }
  }

  async function bloquear(secao) {
    const confirmar = window.confirm(
      `Deseja realmente bloquear a seção "${secao.nome}"?`
    )

    if (!confirmar) return

    try {
      setErro('')

      await api.put(`/Secoes/${secao.id}/bloquear`)

      await carregarSecoes()
    } catch (error) {
      setErro(error.message)
    }
  }
  async function ativar(secao) {
    const confirmar = window.confirm(
      `Deseja ativar a seção "${secao.nome}"?`
    )

    if (!confirmar) return

    try {
      setErro('')

      await api.put(`/Secoes/${secao.id}/ativar`)

      await carregarSecoes()
    } catch (error) {
      setErro(error.message)
    }
  }
  const total = secoes.length
  const ativas = secoes.filter((secao) => secao.ativa).length
  const bloqueadas = secoes.filter((secao) => !secao.ativa).length

  return (
    <section className="secoes-page">

      <div className="secoes-header">
        <div className="secoes-header-info">
          <span className="secoes-header-label">
            ORGANIZAÇÃO
          </span>

          <h1>Seções</h1>

          <p>
            Organize seus produtos por categorias e mantenha
            sua loja organizada.
          </p>
        </div>

        <button
          type="button"
          className="secoes-btn-principal"
          onClick={abrirNovaSecao}
        >
          <span>+</span>
          Nova seção
        </button>
      </div>

      {erro && !modalAberto && (
        <div className="secoes-alerta">
          <span>!</span>
          <p>{erro}</p>
        </div>
      )}

      <div className="secoes-cards">

        <div className="secoes-card">
          <div className="secoes-card-icone">
            ☰
          </div>

          <div>
            <span>Total</span>
            <strong>{total}</strong>
          </div>
        </div>

        <div className="secoes-card">
          <div className="secoes-card-icone ativo">
            ✓
          </div>

          <div>
            <span>Ativas</span>
            <strong>{ativas}</strong>
          </div>
        </div>

        <div className="secoes-card">
          <div className="secoes-card-icone bloqueado">
            ×
          </div>

          <div>
            <span>Bloqueadas</span>
            <strong>{bloqueadas}</strong>
          </div>
        </div>

      </div>

      <div className="secoes-conteudo">

        <div className="secoes-conteudo-header">
          <div>
            <h2>Suas seções</h2>

            <p>
              Gerencie as seções cadastradas na sua empresa.
            </p>
          </div>
        </div>

        {carregando ? (

          <div className="secoes-loading">
            <div className="secoes-spinner"></div>
            <span>Carregando seções...</span>
          </div>

        ) : secoes.length === 0 ? (

          <div className="secoes-empty">

            <div className="secoes-empty-icone">
              ☰
            </div>

            <h3>Nenhuma seção cadastrada</h3>

            <p>
              Crie sua primeira seção para começar a
              cadastrar e organizar seus produtos.
            </p>

            <button
              type="button"
              className="secoes-empty-btn"
              onClick={abrirNovaSecao}
            >
              + Criar primeira seção
            </button>

          </div>

        ) : (

          <div className="secoes-table-wrapper">

            <table className="secoes-table">

              <thead>
                <tr>
                  <th>Seção</th>
                  <th>Status</th>
                  <th>Data de criação</th>
                  <th className="secoes-coluna-acoes">
                    Ações
                  </th>
                </tr>
              </thead>

              <tbody>

                {secoes.map((secao) => (

                  <tr key={secao.id}>

                    <td>
                      <div className="secoes-produto">

                        <div className="secoes-produto-icone">
                          ☰
                        </div>

                        <div className="secoes-produto-info">
                          <strong>{secao.nome}</strong>

                          <span>
                            ID: {secao.id}
                          </span>
                        </div>

                      </div>
                    </td>

                    <td>
                      <span
                        className={`secoes-status ${
                          secao.ativa
                            ? 'status-ativo'
                            : 'status-bloqueado'
                        }`}
                      >
                        <span></span>

                        {secao.ativa
                          ? 'Ativa'
                          : 'Bloqueada'}
                      </span>
                    </td>

                    <td>
                      <span className="secoes-data">
                        {new Date(
                          secao.criadaEm
                        ).toLocaleDateString('pt-BR')}
                      </span>
                    </td>

                    <td>
                      <div className="secoes-acoes">

                        <button
                          type="button"
                          className="secoes-btn-editar"
                          onClick={() => abrirEdicao(secao)}
                        >
                          Editar
                        </button>

                        {secao.ativa ? (
                          <button
                            type="button"
                            className="secoes-btn-bloquear"
                            onClick={() => bloquear(secao)}
                          >
                            Bloquear
                          </button>
                        ) : (
                          <button
                            type="button"
                            className="secoes-btn-ativar"
                            onClick={() => ativar(secao)}
                          >
                            Ativar
                          </button>
                        )}

                      </div>
                    </td>

                  </tr>

                ))}

              </tbody>

            </table>

          </div>

        )}

      </div>

      {modalAberto && (

        <div className="secoes-modal-overlay">

          <div
            className="secoes-modal"
            onClick={(event) => event.stopPropagation()}
          >

            <div className="secoes-modal-header">

              <div>

                <span className="secoes-modal-label">
                  {editando
                    ? 'EDITAR SEÇÃO'
                    : 'NOVA SEÇÃO'}
                </span>

                <h2>
                  {editando
                    ? 'Editar seção'
                    : 'Criar nova seção'}
                </h2>

                <p>
                  {editando
                    ? 'Atualize o nome da seção.'
                    : 'Informe o nome da nova seção.'}
                </p>

              </div>

              <button
                type="button"
                className="secoes-modal-close"
                onClick={fecharModal}
                disabled={salvando}
              >
                ×
              </button>

            </div>

            <div className="secoes-modal-body">

              <label htmlFor="nome-secao">
                Nome da seção
              </label>

              <input
                id="nome-secao"
                type="text"
                value={nome}
                onChange={(event) =>
                  setNome(event.target.value)
                }
                onKeyDown={(event) => {
                  if (event.key === 'Enter') {
                    salvar()
                  }
                }}
                placeholder="Ex.: Bebidas"
                maxLength={200}
                autoFocus
              />

              <small>
                Exemplos: Bebidas, Carnes, Roupas,
                Eletrônicos, Combos.
              </small>

              {erro && (
                <div className="secoes-modal-alerta">
                  {erro}
                </div>
              )}

            </div>

            <div className="secoes-modal-footer">

              <button
                type="button"
                className="secoes-btn-cancelar"
                onClick={fecharModal}
                disabled={salvando}
              >
                Cancelar
              </button>

              <button
                type="button"
                className="secoes-btn-salvar"
                onClick={salvar}
                disabled={salvando}
              >
                {salvando ? (
                  <>
                    <span className="secoes-btn-spinner"></span>
                    Salvando...
                  </>
                ) : (
                  <>
                    {editando
                      ? 'Salvar alterações'
                      : 'Salvar seção'}
                  </>
                )}
              </button>

            </div>

          </div>

        </div>

      )}

    </section>
  )
}

export default Secoes

