import { useEffect, useState } from 'react'
import './Financeiro.css'
import { api } from "../../../../api/api";

function Financeiro() {
  const [dashboard, setDashboard] = useState(null)
  const [loading, setLoading] = useState(true)
  const [erro, setErro] = useState('')

  const hoje = new Date().toISOString().split('T')[0]

  const [dataInicio, setDataInicio] = useState(hoje)
  const [dataFim, setDataFim] = useState(hoje)

  const formatarMoeda = (valor) => {
    return Number(valor || 0).toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    })
  }

  const formatarNumero = (valor, casas = 2) => {
    return Number(valor || 0).toLocaleString('pt-BR', {
      minimumFractionDigits: casas,
      maximumFractionDigits: casas,
    })
  }

  const formatarData = (data) => {
    if (!data) return '-'

    return new Date(data).toLocaleDateString('pt-BR')
  }

  const formatarDataHora = (data) => {
    if (!data) return '-'

    return new Date(data).toLocaleString('pt-BR')
  }

  const carregarDashboard = async () => {
    try {
      setLoading(true)
      setErro('')

      const resultado = await api.get(
        `/FinanceiroDashboard?dataInicio=${dataInicio}&dataFim=${dataFim}`
      )

      setDashboard(resultado)
    } catch (error) {
      console.error(error)
      setErro(error.message || 'Não foi possível carregar o financeiro.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    carregarDashboard()
  }, [])

  const aplicarFiltro = () => {
    carregarDashboard()
  }

  const periodoRapido = (tipo) => {
    const agora = new Date()
    let inicio = new Date()

    if (tipo === 'hoje') {
      inicio = agora
    }

    if (tipo === '7dias') {
      inicio.setDate(agora.getDate() - 6)
    }

    if (tipo === 'mes') {
      inicio = new Date(
        agora.getFullYear(),
        agora.getMonth(),
        1
      )
    }

    if (tipo === 'mesAnterior') {
      inicio = new Date(
        agora.getFullYear(),
        agora.getMonth() - 1,
        1
      )

      const fim = new Date(
        agora.getFullYear(),
        agora.getMonth(),
        0
      )

      const inicioFormatado =
        inicio.toISOString().split('T')[0]

      const fimFormatado =
        fim.toISOString().split('T')[0]

      setDataInicio(inicioFormatado)
      setDataFim(fimFormatado)

      setTimeout(() => {
        carregarDashboardComPeriodo(
          inicioFormatado,
          fimFormatado
        )
      }, 0)

      return
    }

    const inicioFormatado =
      inicio.toISOString().split('T')[0]

    const fimFormatado =
      agora.toISOString().split('T')[0]

    setDataInicio(inicioFormatado)
    setDataFim(fimFormatado)

    setTimeout(() => {
      carregarDashboardComPeriodo(
        inicioFormatado,
        fimFormatado
      )
    }, 0)
  }

  const carregarDashboardComPeriodo = async (
    inicio,
    fim
  ) => {
    try {
      setLoading(true)
      setErro('')

      const resultado = await api.get(
        `/FinanceiroDashboard?dataInicio=${inicio}&dataFim=${fim}`
      )

      setDashboard(resultado)
    } catch (error) {
      console.error(error)
      setErro(error.message || 'Não foi possível carregar o financeiro.')
    } finally {
      setLoading(false)
    }
  }

  const exportar = async (tipo) => {
    try {
      setErro('')

      const token = localStorage.getItem('controlhub_token')
      const apiUrl = import.meta.env.VITE_API_URL

      if (!token) {
        throw new Error('Sessão expirada. Faça login novamente.')
      }

      const extensao =
        tipo === 'pdf'
          ? 'pdf'
          : tipo === 'excel'
            ? 'xlsx'
            : 'csv'

      const url =
        `${apiUrl}/api/FinanceiroDashboard/exportar/${tipo}` +
        `?dataInicio=${encodeURIComponent(dataInicio)}` +
        `&dataFim=${encodeURIComponent(dataFim)}`

      const response = await fetch(url, {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      if (!response.ok) {
        let mensagem = 'Não foi possível gerar o relatório.'

        const contentType =
          response.headers.get('content-type') || ''

        if (contentType.includes('application/json')) {
          try {
            const data = await response.json()

            mensagem =
              data?.mensagem ||
              data?.message ||
              mensagem
          } catch {
            // mantém mensagem padrão
          }
        } else {
          try {
            const texto = await response.text()

            if (texto) {
              mensagem = texto
            }
          } catch {
            // mantém mensagem padrão
          }
        }

        throw new Error(
          `Erro ${response.status}: ${mensagem}`
        )
      }

      const blob = await response.blob()

      if (!blob || blob.size === 0) {
        throw new Error(
          'O servidor retornou um arquivo vazio.'
        )
      }

      const urlDownload =
        window.URL.createObjectURL(blob)

      const link =
        document.createElement('a')

      link.href = urlDownload

      link.download =
        `relatorio-financeiro-${dataInicio}-${dataFim}.${extensao}`

      document.body.appendChild(link)

      link.click()

      document.body.removeChild(link)

      setTimeout(() => {
        window.URL.revokeObjectURL(urlDownload)
      }, 1000)

    } catch (error) {
      console.error(
        'Erro ao exportar relatório:',
        error
      )

      setErro(
        error.message ||
        'Não foi possível exportar o relatório.'
      )
    }
  }

  if (loading && !dashboard) {
    return (
      <section className="modulo financeiro">
        <div className="modulo-cabecalho">
          <div>
            <h1>Financeiro</h1>
            <p>
              Controle financeiro da sua empresa.
            </p>
          </div>
        </div>

        <div className="financeiro-loading">
          <div className="financeiro-spinner" />
          <span>Carregando financeiro...</span>
        </div>
      </section>
    )
  }

  return (
    <section className="modulo financeiro">
      <div className="modulo-cabecalho financeiro-topo">
        <div>
          <h1>Financeiro</h1>

          <p>
            Visão geral das entradas, saídas,
            vendas, compras e estoque.
          </p>
        </div>

        <div className="financeiro-exportacoes">
          <button
            className="btn-exportar btn-pdf"
            onClick={() => exportar('pdf')}
          >
            PDF
          </button>

          <button
            className="btn-exportar btn-excel"
            onClick={() => exportar('excel')}
          >
            Excel
          </button>

          <button
            className="btn-exportar btn-csv"
            onClick={() => exportar('csv')}
          >
            CSV
          </button>
        </div>
      </div>

      {erro && (
        <div className="financeiro-erro">
          <strong>Erro:</strong> {erro}
        </div>
      )}

      <div className="financeiro-filtros">
        <div className="financeiro-filtro-grupo">
          <label>Data inicial</label>

          <input
            type="date"
            value={dataInicio}
            onChange={(e) =>
              setDataInicio(e.target.value)
            }
          />
        </div>

        <div className="financeiro-filtro-grupo">
          <label>Data final</label>

          <input
            type="date"
            value={dataFim}
            onChange={(e) =>
              setDataFim(e.target.value)
            }
          />
        </div>

        <button
          className="btn-aplicar-filtro"
          onClick={aplicarFiltro}
          disabled={loading}
        >
          {loading ? 'Carregando...' : 'Aplicar'}
        </button>

        <div className="financeiro-periodos">
          <button
            onClick={() => periodoRapido('hoje')}
          >
            Hoje
          </button>

          <button
            onClick={() => periodoRapido('7dias')}
          >
            7 dias
          </button>

          <button
            onClick={() => periodoRapido('mes')}
          >
            Este mês
          </button>

          <button
            onClick={() => periodoRapido('mesAnterior')}
          >
            Mês anterior
          </button>
        </div>
      </div>

      {dashboard && (
        <>
          <div className="financeiro-cards principais">
            <div className="financeiro-card entrada">
              <span>Entradas</span>

              <strong>
                {formatarMoeda(
                  dashboard.totalEntradas
                )}
              </strong>

              <small>
                Total recebido no período
              </small>
            </div>

            <div className="financeiro-card saida">
              <span>Saídas</span>

              <strong>
                {formatarMoeda(
                  dashboard.totalSaidas
                )}
              </strong>

              <small>
                Total pago no período
              </small>
            </div>

            <div
              className={`financeiro-card ${
                Number(dashboard.saldo) >= 0
                  ? 'saldo-positivo'
                  : 'saldo-negativo'
              }`}
            >
              <span>Saldo</span>

              <strong>
                {formatarMoeda(
                  dashboard.saldo
                )}
              </strong>

              <small>
                Entradas menos saídas
              </small>
            </div>

            <div className="financeiro-card venda">
              <span>Vendas</span>

              <strong>
                {formatarMoeda(
                  dashboard.totalVendas
                )}
              </strong>

              <small>
                {dashboard.quantidadeVendas || 0}{' '}
                venda(s)
              </small>
            </div>
          </div>

          <div className="financeiro-cards secundarias">
            <div className="financeiro-card">
              <span>Compras</span>

              <strong>
                {formatarMoeda(
                  dashboard.totalCompras
                )}
              </strong>

              <small>
                {dashboard.quantidadeCompras || 0}{' '}
                compra(s)
              </small>
            </div>

            <div className="financeiro-card">
              <span>Custo das vendas</span>

              <strong>
                {formatarMoeda(
                  dashboard.custoDasVendas
                )}
              </strong>

              <small>
                Custo dos produtos vendidos
              </small>
            </div>

            <div className="financeiro-card lucro">
              <span>Lucro bruto</span>

              <strong>
                {formatarMoeda(
                  dashboard.lucroBruto
                )}
              </strong>

              <small>
                Vendas menos custo
              </small>
            </div>

            <div className="financeiro-card">
              <span>Produtos em estoque</span>

              <strong>
                {dashboard.produtosComEstoque || 0}
              </strong>

              <small>
                {formatarNumero(
                  dashboard.quantidadeTotalEstoque,
                  3
                )}{' '}
                unidades
              </small>
            </div>
          </div>

          <div className="financeiro-grid-duplo">
            <div className="financeiro-painel">
              <div className="financeiro-painel-header">
                <div>
                  <h2>Estoque</h2>

                  <p>
                    Valor atual dos produtos em
                    estoque.
                  </p>
                </div>
              </div>

              <div className="estoque-resumo">
                <div>
                  <span>Custo</span>

                  <strong>
                    {formatarMoeda(
                      dashboard.custoTotalEstoque
                    )}
                  </strong>
                </div>

                <div>
                  <span>Valor de venda</span>

                  <strong>
                    {formatarMoeda(
                      dashboard.valorVendaEstoque
                    )}
                  </strong>
                </div>

                <div>
                  <span>Lucro potencial</span>

                  <strong>
                    {formatarMoeda(
                      dashboard.lucroPotencialEstoque
                    )}
                  </strong>
                </div>
              </div>
            </div>

            <div className="financeiro-painel">
              <div className="financeiro-painel-header">
                <div>
                  <h2>Período</h2>

                  <p>
                    Informações do filtro atual.
                  </p>
                </div>
              </div>

              <div className="periodo-info">
                <div>
                  <span>Início</span>
                  <strong>
                    {formatarData(
                      dashboard.dataInicio
                    )}
                  </strong>
                </div>

                <div>
                  <span>Fim</span>
                  <strong>
                    {formatarData(
                      dashboard.dataFim
                    )}
                  </strong>
                </div>
              </div>
            </div>
          </div>

          <div className="financeiro-painel tabela-painel">
            <div className="financeiro-painel-header">
              <div>
                <h2>Estoque detalhado</h2>

                <p>
                  Produtos, custos e lucro potencial.
                </p>
              </div>
            </div>

            {dashboard.estoque?.length ? (
              <div className="tabela-wrapper">
                <table className="financeiro-tabela">
                  <thead>
                    <tr>
                      <th>Produto</th>
                      <th>Qtd.</th>
                      <th>Custo unit.</th>
                      <th>Custo total</th>
                      <th>Venda total</th>
                      <th>Lucro potencial</th>
                      <th>Status</th>
                    </tr>
                  </thead>

                  <tbody>
                    {dashboard.estoque.map(
                      (item) => (
                        <tr key={item.produtoId}>
                          <td>
                            <strong>
                              {item.nomeProduto}
                            </strong>
                          </td>

                          <td>
                            {formatarNumero(
                              item.quantidade,
                              3
                            )}
                          </td>

                          <td>
                            {formatarMoeda(
                              item.custoUnitario
                            )}
                          </td>

                          <td>
                            {formatarMoeda(
                              item.custoTotal
                            )}
                          </td>

                          <td>
                            {formatarMoeda(
                              item.valorVendaTotal
                            )}
                          </td>

                          <td className="valor-lucro">
                            {formatarMoeda(
                              item.lucroPotencial
                            )}
                          </td>

                          <td>
                            <span
                              className={`status-badge ${
                                item.ativo
                                  ? 'ativo'
                                  : 'bloqueado'
                              }`}
                            >
                              {item.ativo
                                ? 'Ativo'
                                : 'Bloqueado'}
                            </span>
                          </td>
                        </tr>
                      )
                    )}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className="tabela-vazia">
                Nenhum produto em estoque.
              </div>
            )}
          </div>

          <div className="financeiro-grid-duplo">
            <div className="financeiro-painel tabela-painel">
              <div className="financeiro-painel-header">
                <div>
                  <h2>Últimas movimentações</h2>

                  <p>
                    Entradas e saídas financeiras.
                  </p>
                </div>
              </div>

              {dashboard.ultimasMovimentacoes
                ?.length ? (
                <div className="tabela-wrapper">
                  <table className="financeiro-tabela">
                    <thead>
                      <tr>
                        <th>Data</th>
                        <th>Descrição</th>
                        <th>Tipo</th>
                        <th>Valor</th>
                      </tr>
                    </thead>

                    <tbody>
                      {dashboard.ultimasMovimentacoes.map(
                        (item) => (
                          <tr key={item.id}>
                            <td>
                              {formatarDataHora(
                                item.criadaEm
                              )}
                            </td>

                            <td>
                              <strong>
                                {item.descricao}
                              </strong>

                              <small className="categoria">
                                {item.categoria}
                              </small>
                            </td>

                            <td>
                              <span
                                className={`tipo-movimentacao ${
                                  item.entrada
                                    ? 'entrada'
                                    : 'saida'
                                }`}
                              >
                                {item.entrada
                                  ? 'Entrada'
                                  : 'Saída'}
                              </span>
                            </td>

                            <td>
                              {formatarMoeda(
                                item.valor
                              )}
                            </td>
                          </tr>
                        )
                      )}
                    </tbody>
                  </table>
                </div>
              ) : (
                <div className="tabela-vazia">
                  Nenhuma movimentação no período.
                </div>
              )}
            </div>

            <div className="financeiro-painel tabela-painel">
              <div className="financeiro-painel-header">
                <div>
                  <h2>Vendas</h2>

                  <p>
                    Resultado das vendas no período.
                  </p>
                </div>
              </div>

              {dashboard.vendas?.length ? (
                <div className="tabela-wrapper">
                  <table className="financeiro-tabela">
                    <thead>
                      <tr>
                        <th>Data</th>
                        <th>Venda</th>
                        <th>Custo</th>
                        <th>Lucro</th>
                      </tr>
                    </thead>

                    <tbody>
                      {dashboard.vendas.map(
                        (item) => (
                          <tr key={item.id}>
                            <td>
                              {formatarDataHora(
                                item.criadaEm
                              )}
                            </td>

                            <td>
                              {formatarMoeda(
                                item.valorTotal
                              )}
                            </td>

                            <td>
                              {formatarMoeda(
                                item.custoTotal
                              )}
                            </td>

                            <td className="valor-lucro">
                              {formatarMoeda(
                                item.lucro
                              )}
                            </td>
                          </tr>
                        )
                      )}
                    </tbody>
                  </table>
                </div>
              ) : (
                <div className="tabela-vazia">
                  Nenhuma venda no período.
                </div>
              )}
            </div>
          </div>

          <div className="financeiro-painel tabela-painel">
            <div className="financeiro-painel-header">
              <div>
                <h2>Compras de fornecedores</h2>

                <p>
                  Compras realizadas no período.
                </p>
              </div>
            </div>

            {dashboard.compras?.length ? (
              <div className="tabela-wrapper">
                <table className="financeiro-tabela">
                  <thead>
                    <tr>
                      <th>Data</th>
                      <th>Fornecedor</th>
                      <th>Nota</th>
                      <th>Valor</th>
                    </tr>
                  </thead>

                  <tbody>
                    {dashboard.compras.map(
                      (item) => (
                        <tr key={item.id}>
                          <td>
                            {formatarData(
                              item.dataCompra
                            )}
                          </td>

                          <td>
                            <strong>
                              {item.nomeFornecedor}
                            </strong>
                          </td>

                          <td>
                            {item.numeroNota || '-'}
                          </td>

                          <td>
                            {formatarMoeda(
                              item.valorTotal
                            )}
                          </td>
                        </tr>
                      )
                    )}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className="tabela-vazia">
                Nenhuma compra no período.
              </div>
            )}
          </div>
        </>
      )}
    </section>
  )
}

export default Financeiro