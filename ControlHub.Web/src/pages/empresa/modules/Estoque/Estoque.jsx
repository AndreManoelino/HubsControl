import { useEffect, useState } from 'react'
import { api } from "../../../../api/api";
import './Estoque.css'

function Estoque() {
  const [estoques, setEstoques] = useState([])
  const [carregando, setCarregando] = useState(false)
  const [salvando, setSalvando] = useState(false)

  const [erro, setErro] = useState('')
  const [mensagem, setMensagem] = useState('')

  const [modalAberto, setModalAberto] = useState(false)
  const [tipoMovimentacao, setTipoMovimentacao] = useState('')
  const [estoqueSelecionado, setEstoqueSelecionado] = useState(null)

  const [quantidade, setQuantidade] = useState('')
  const [observacao, setObservacao] = useState('')

  async function carregarEstoques() {
    try {
      setCarregando(true)
      setErro('')

      const dados = await api.get('/Estoque')

      setEstoques(dados)
    } catch (error) {
      setErro(error.message)
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarEstoques()
  }, [])

  function abrirMovimentacao(estoque, tipo) {
    setErro('')
    setMensagem('')

    setEstoqueSelecionado(estoque)
    setTipoMovimentacao(tipo)
    setQuantidade('')
    setObservacao('')
    setModalAberto(true)
  }

  function fecharModal() {
    if (salvando) return

    setModalAberto(false)
    setEstoqueSelecionado(null)
    setTipoMovimentacao('')
    setQuantidade('')
    setObservacao('')
    setErro('')
  }

  async function salvarMovimentacao() {
    const quantidadeNumero = Number(quantidade)

    if (!quantidade || quantidadeNumero <= 0) {
      setErro('Informe uma quantidade maior que zero.')
      return
    }

    if (!estoqueSelecionado) {
      setErro('Estoque não selecionado.')
      return
    }

    try {
      setSalvando(true)
      setErro('')
      setMensagem('')

      if (tipoMovimentacao === 'entrada') {
        await api.post('/Estoque/entrada', {
          produtoId: estoqueSelecionado.produtoId,
          quantidade: quantidadeNumero,
          observacao: observacao.trim() || null,
        })
      }

      if (tipoMovimentacao === 'saida') {
        await api.post('/Estoque/saida', {
          produtoId: estoqueSelecionado.produtoId,
          quantidade: quantidadeNumero,
          observacao: observacao.trim() || null,
        })
      }

      if (tipoMovimentacao === 'ajuste') {
        await api.post('/Estoque/ajuste', {
          produtoId: estoqueSelecionado.produtoId,
          novaQuantidade: quantidadeNumero,
          observacao: observacao.trim() || null,
        })
      }

      setMensagem('Movimentação realizada com sucesso.')

      fecharModal()

      await carregarEstoques()
    } catch (error) {
      setErro(error.message)
    } finally {
      setSalvando(false)
    }
  }

  function obterStatus(estoque) {
    if (estoque.quantidade <= 0) {
      return {
        texto: 'Sem estoque',
        classe: 'sem-estoque',
      }
    }

    if (estoque.quantidade <= estoque.quantidadeMinima) {
      return {
        texto: 'Estoque baixo',
        classe: 'estoque-baixo',
      }
    }

    return {
      texto: 'Normal',
      classe: 'estoque-normal',
    }
  }

  const totalProdutos = estoques.length

  const semEstoque = estoques.filter(
    (estoque) => estoque.quantidade <= 0
  ).length

  const estoqueBaixo = estoques.filter(
    (estoque) =>
      estoque.quantidade > 0 &&
      estoque.quantidade <= estoque.quantidadeMinima
  ).length

  const estoqueNormal = estoques.filter(
    (estoque) =>
      estoque.quantidade > estoque.quantidadeMinima
  ).length

  return (
    <section className="estoque-page">

      <div className="estoque-header">

        <div className="estoque-header-info">

          <span className="estoque-header-label">
            CONTROLE
          </span>

          <h1>Estoque</h1>

          <p>
            Controle as quantidades dos produtos da sua empresa.
          </p>

        </div>

      </div>

      {erro && !modalAberto && (
        <div className="estoque-alerta">
          <span>!</span>
          <p>{erro}</p>
        </div>
      )}

      {mensagem && (
        <div className="estoque-mensagem">
          <span>✓</span>
          <p>{mensagem}</p>
        </div>
      )}

      <div className="estoque-cards">

        <div className="estoque-card">

          <div className="estoque-card-icone">
            📦
          </div>

          <div>
            <span>Produtos</span>
            <strong>{totalProdutos}</strong>
          </div>

        </div>

        <div className="estoque-card">

          <div className="estoque-card-icone normal">
            ✓
          </div>

          <div>
            <span>Estoque normal</span>
            <strong>{estoqueNormal}</strong>
          </div>

        </div>

        <div className="estoque-card">

          <div className="estoque-card-icone baixo">
            !
          </div>

          <div>
            <span>Estoque baixo</span>
            <strong>{estoqueBaixo}</strong>
          </div>

        </div>

        <div className="estoque-card">

          <div className="estoque-card-icone sem">
            ×
          </div>

          <div>
            <span>Sem estoque</span>
            <strong>{semEstoque}</strong>
          </div>

        </div>

      </div>

      <div className="estoque-conteudo">

        <div className="estoque-conteudo-header">

          <div>
            <h2>Produtos em estoque</h2>

            <p>
              Gerencie a quantidade disponível de cada produto.
            </p>
          </div>

        </div>

        {carregando ? (

          <div className="estoque-loading">
            <div className="estoque-spinner"></div>
            <span>Carregando estoque...</span>
          </div>

        ) : estoques.length === 0 ? (

          <div className="estoque-empty">

            <div className="estoque-empty-icone">
              📦
            </div>

            <h3>Nenhum estoque cadastrado</h3>

            <p>
              Os produtos configurados para controlar estoque
              aparecerão aqui.
            </p>

          </div>

        ) : (

          <div className="estoque-table-wrapper">

            <table className="estoque-table">

              <thead>

                <tr>
                  <th>Produto</th>
                  <th>Quantidade</th>
                  <th>Estoque mínimo</th>
                  <th>Status</th>
                  <th>Atualizado em</th>
                  <th className="estoque-coluna-acoes">
                    Ações
                  </th>
                </tr>

              </thead>

              <tbody>

                {estoques.map((estoque) => {

                  const status = obterStatus(estoque)

                  return (
                    <tr key={estoque.id}>

                      <td>

                        <div className="estoque-produto">

                          <div className="estoque-produto-icone">
                            📦
                          </div>

                          <div className="estoque-produto-info">

                            <strong>
                              {estoque.nomeProduto}
                            </strong>

                            <span>
                              ID: {estoque.produtoId}
                            </span>

                          </div>

                        </div>

                      </td>

                      <td>

                        <strong className="estoque-quantidade">
                          {estoque.quantidade}
                        </strong>

                      </td>

                      <td>

                        <span className="estoque-minimo">
                          {estoque.quantidadeMinima}
                        </span>

                      </td>

                      <td>

                        <span
                          className={`estoque-status ${status.classe}`}
                        >
                          <span></span>

                          {status.texto}
                        </span>

                      </td>

                      <td>

                        <span className="estoque-data">
                          {new Date(
                            estoque.atualizadoEm
                          ).toLocaleDateString('pt-BR')}
                        </span>

                      </td>

                      <td>

                        <div className="estoque-acoes">

                          <button
                            type="button"
                            className="estoque-btn-entrada"
                            onClick={() =>
                              abrirMovimentacao(
                                estoque,
                                'entrada'
                              )
                            }
                          >
                            Entrada
                          </button>

                          <button
                            type="button"
                            className="estoque-btn-saida"
                            onClick={() =>
                              abrirMovimentacao(
                                estoque,
                                'saida'
                              )
                            }
                          >
                            Saída
                          </button>

                          <button
                            type="button"
                            className="estoque-btn-ajuste"
                            onClick={() =>
                              abrirMovimentacao(
                                estoque,
                                'ajuste'
                              )
                            }
                          >
                            Ajustar
                          </button>

                        </div>

                      </td>

                    </tr>
                  )
                })}

              </tbody>

            </table>

          </div>

        )}

      </div>

      {modalAberto && (

        <div className="estoque-modal-overlay">

          <div
            className="estoque-modal"
            onClick={(event) =>
              event.stopPropagation()
            }
          >

            <div className="estoque-modal-header">

              <div>

                <span className="estoque-modal-label">
                  {tipoMovimentacao === 'entrada'
                    ? 'ENTRADA DE ESTOQUE'
                    : tipoMovimentacao === 'saida'
                      ? 'SAÍDA DE ESTOQUE'
                      : 'AJUSTE DE ESTOQUE'}
                </span>

                <h2>
                  {tipoMovimentacao === 'entrada'
                    ? 'Adicionar estoque'
                    : tipoMovimentacao === 'saida'
                      ? 'Retirar estoque'
                      : 'Ajustar estoque'}
                </h2>

                <p>
                  Produto: {estoqueSelecionado?.nomeProduto}
                </p>

              </div>

              <button
                type="button"
                className="estoque-modal-close"
                onClick={fecharModal}
                disabled={salvando}
              >
                ×
              </button>

            </div>

            <div className="estoque-modal-body">

              <div className="estoque-modal-atual">

                <span>Quantidade atual</span>

                <strong>
                  {estoqueSelecionado?.quantidade}
                </strong>

              </div>

              <label htmlFor="quantidade-estoque">
                {tipoMovimentacao === 'ajuste'
                  ? 'Nova quantidade'
                  : 'Quantidade'}
              </label>

              <input
                id="quantidade-estoque"
                type="number"
                min="0"
                step="0.01"
                value={quantidade}
                onChange={(event) =>
                  setQuantidade(event.target.value)
                }
                placeholder={
                  tipoMovimentacao === 'ajuste'
                    ? 'Ex.: 50'
                    : 'Ex.: 10'
                }
                autoFocus
              />

              <label htmlFor="observacao-estoque">
                Observação
              </label>

              <textarea
                id="observacao-estoque"
                value={observacao}
                onChange={(event) =>
                  setObservacao(event.target.value)
                }
                placeholder="Ex.: Compra de mercadoria"
                rows="3"
              />

              {erro && (
                <div className="estoque-modal-alerta">
                  {erro}
                </div>
              )}

            </div>

            <div className="estoque-modal-footer">

              <button
                type="button"
                className="estoque-btn-cancelar"
                onClick={fecharModal}
                disabled={salvando}
              >
                Cancelar
              </button>

              <button
                type="button"
                className="estoque-btn-salvar"
                onClick={salvarMovimentacao}
                disabled={salvando}
              >
                {salvando ? (
                  <>
                    <span className="estoque-btn-spinner"></span>
                    Salvando...
                  </>
                ) : (
                  'Confirmar'
                )}
              </button>

            </div>

          </div>

        </div>

      )}

    </section>
  )
}

export default Estoque