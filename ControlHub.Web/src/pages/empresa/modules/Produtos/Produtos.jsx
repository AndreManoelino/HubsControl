import { useEffect, useState } from "react";
import "./Produtos.css";

import { api } from "../../../../api/api";

export default function Produtos() {
  const [produtos, setProdutos] = useState([]);
  const [secoes, setSecoes] = useState([]);
  const [carregando, setCarregando] = useState(true);
  const [salvando, setSalvando] = useState(false);

  const [busca, setBusca] = useState("");

  const [modalAberto, setModalAberto] = useState(false);
  const [modoEdicao, setModoEdicao] = useState(false);
  const [produtoSelecionado, setProdutoSelecionado] = useState(null);

  const [mensagem, setMensagem] = useState("");
  const [erro, setErro] = useState("");

  const [formulario, setFormulario] = useState({
    secaoId: "",
    nome: "",
    descricao: "",
    precoVenda: "",
    imagemUrl: "",
    controlaEstoque: false,
    quantidadeInicial: "",
    quantidadeMinima: "",
  });

  useEffect(() => {
    carregarDados();
  }, []);

  async function carregarDados() {
    try {
      setCarregando(true);
      setErro("");

      const [produtosData, secoesData] = await Promise.all([
        api.get("/Produtos"),
        api.get("/Secoes"),
      ]);

      console.log("PRODUTOS RECEBIDOS:", produtosData);
      console.log("SEÇÕES RECEBIDAS:", secoesData);

      setProdutos(produtosData || []);
      setSecoes(secoesData || []);
    } catch (error) {
      console.error("Erro ao carregar produtos/seções:", error);
      setErro(error.message);
    } finally {
      setCarregando(false);
    }
  }

  function abrirNovoProduto() {
    const secoesAtivas = secoes.filter((secao) => secao.ativa);

    if (secoesAtivas.length === 0) {
      setErro(
        "É necessário cadastrar uma seção ativa antes de criar um produto."
      );
      return;
    }

    setModoEdicao(false);
    setProdutoSelecionado(null);

    setFormulario({
      secaoId: "",
      nome: "",
      descricao: "",
      precoVenda: "",
      imagemUrl: "",
      controlaEstoque: false,
      quantidadeInicial: "",
      quantidadeMinima: "",
    });

    setErro("");
    setMensagem("");
    setModalAberto(true);
  }

  function abrirEditarProduto(produto) {
      setModoEdicao(true);
      setProdutoSelecionado(produto);

      setFormulario({
          secaoId: produto.secaoId,
          nome: produto.nome,
          descricao: produto.descricao || "",
          precoVenda: produto.precoVenda,
          imagemUrl: produto.imagemUrl || "",
          controlaEstoque: produto.controlaEstoque,
          quantidadeInicial: "",
          quantidadeMinima: produto.quantidadeMinima ?? "",
      });

      setErro("");
      setMensagem("");
      setModalAberto(true);
  }

  function fecharModal() {
    if (salvando) return;

    setModalAberto(false);
    setProdutoSelecionado(null);
  }

  function alterarCampo(event) {
    const { name, value, type, checked } = event.target;

    setFormulario((estado) => ({
      ...estado,
      [name]: type === "checkbox" ? checked : value,
    }));
  }

  async function salvarProduto(event) {
    event.preventDefault();

    if (!formulario.secaoId) {
      setErro("Selecione uma seção para o produto.");
      return;
    }

    try {
      setSalvando(true);
      setErro("");
      setMensagem("");

      const body = {
        secaoId: formulario.secaoId,
        nome: formulario.nome,
        descricao: formulario.descricao || null,
        precoVenda: Number(formulario.precoVenda),
        imagemUrl: formulario.imagemUrl || null,
        controlaEstoque: formulario.controlaEstoque,
      };

      if (!modoEdicao) {
        body.quantidadeInicial = formulario.controlaEstoque
          ? Number(formulario.quantidadeInicial || 0)
          : null;

        body.quantidadeMinima = formulario.controlaEstoque
          ? Number(formulario.quantidadeMinima || 0)
          : null;
      } else {
        body.quantidadeMinima = formulario.controlaEstoque
          ? Number(formulario.quantidadeMinima || 0)
          : null;
      }

      if (modoEdicao) {
        await api.put(
          `/Produtos/${produtoSelecionado.id}`,
          body
        );
      } else {
        await api.post("/Produtos", body);
      }

      setMensagem(
        modoEdicao
          ? "Produto atualizado com sucesso."
          : "Produto criado com sucesso."
      );

      setModalAberto(false);
      setProdutoSelecionado(null);

      await carregarDados();
    } catch (error) {
      console.error("Erro ao salvar produto:", error);
      setErro(error.message);
    } finally {
      setSalvando(false);
    }
  }

  async function bloquearProduto(produto) {
    const confirmar = window.confirm(
      `Deseja bloquear o produto "${produto.nome}"?`
    );

    if (!confirmar) return;

    const motivo = window.prompt(
      "Informe o motivo do bloqueio:",
      "Produto bloqueado pelo proprietário."
    );

    try {
      setErro("");
      setMensagem("");

      await api.put(
        `/Produtos/${produto.id}/bloquear`,
        {
          motivo: motivo || null,
        }
      );

      setMensagem("Produto bloqueado com sucesso.");

      await carregarDados();
    } catch (error) {
      console.error("Erro ao bloquear produto:", error);
      setErro(error.message);
    }
  }
  async function ativarProduto(produto) {
    const confirmar = window.confirm(
      `Deseja ativar o produto "${produto.nome}"?`
    );

    if (!confirmar) return;

    try {
      setErro("");
      setMensagem("");

      await api.put(
        `/Produtos/${produto.id}/ativar`
      );

      setMensagem("Produto ativado com sucesso.");

      await carregarDados();
    } catch (error) {
      console.error("Erro ao ativar produto:", error);
      setErro(error.message);
    }
  }
  const produtosFiltrados = produtos.filter((produto) => {
    const texto = busca.toLowerCase();

    return (
      produto.nome?.toLowerCase().includes(texto) ||
      produto.descricao?.toLowerCase().includes(texto)
    );
  });

  const secoesAtivas = secoes.filter(
    (secao) => secao.ativa
  );

  function nomeSecao(secaoId) {
    const secao = secoes.find(
      (item) => item.id === secaoId
    );

    return secao?.nome || "Sem seção";
  }

  function formatarPreco(valor) {
    return Number(valor).toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  }

  if (carregando) {
    return (
      <div className="produtos-page">
        <div className="produtos-loading">
          Carregando produtos...
        </div>
      </div>
    );
  }

  return (
    <div className="produtos-page">
      <div className="produtos-header">
        <div>
          <h1>Produtos</h1>
          <p>Gerencie os produtos da sua loja.</p>
        </div>

        <button
          className="btn-primary"
          onClick={abrirNovoProduto}
          disabled={secoesAtivas.length === 0}
        >
          + Novo produto
        </button>
      </div>

      {secoesAtivas.length === 0 && (
        <div className="alert alert-error">
          Cadastre uma seção ativa antes de cadastrar produtos.
        </div>
      )}

      {mensagem && (
        <div className="alert alert-success">
          {mensagem}
        </div>
      )}

      {erro && (
        <div className="alert alert-error">
          {erro}
        </div>
      )}

      <div className="produtos-toolbar">
        <input
          type="text"
          placeholder="Buscar produto..."
          value={busca}
          onChange={(event) =>
            setBusca(event.target.value)
          }
        />
      </div>

      <div className="produtos-table-container">
        <table className="produtos-table">
          <thead>
            <tr>
              <th>Produto</th>
              <th>Seção</th>
              <th>Preço</th>
              <th>Estoque</th>
              <th>Status</th>
              <th>Ações</th>
            </tr>
          </thead>

          <tbody>
            {produtosFiltrados.length === 0 ? (
              <tr>
                <td
                  colSpan="6"
                  className="produtos-empty"
                >
                  Nenhum produto encontrado.
                </td>
              </tr>
            ) : (
              produtosFiltrados.map((produto) => (
                <tr key={produto.id}>
                  <td>
                    <div className="produto-info">
                      {produto.imagemUrl ? (
                        <img
                          src={produto.imagemUrl}
                          alt={produto.nome}
                        />
                      ) : (
                        <div className="produto-sem-imagem">
                          📦
                        </div>
                      )}

                      <div>
                        <strong>{produto.nome}</strong>

                        {produto.descricao && (
                          <span>
                            {produto.descricao}
                          </span>
                        )}
                      </div>
                    </div>
                  </td>

                  <td>
                    {nomeSecao(produto.secaoId)}
                  </td>

                  <td>
                    {formatarPreco(
                      produto.precoVenda
                    )}
                  </td>

                  <td>
                    {produto.controlaEstoque ? (
                      <span>
                        {produto.quantidadeEstoque ?? 0}
                      </span>
                    ) : (
                      <span className="estoque-nao-controlado">
                        Não controla
                      </span>
                    )}
                  </td>

                  <td>
                    {produto.ativo ? (
                      <span className="status ativo">
                        Ativo
                      </span>
                    ) : (
                      <span className="status bloqueado">
                        Bloqueado
                      </span>
                    )}
                  </td>

                  <td>
                    <div className="produto-acoes">
                      <button
                        className="btn-edit"
                        onClick={() =>
                          abrirEditarProduto(produto)
                        }
                        disabled={!produto.ativo}
                      >
                        Editar
                      </button>

                      {produto.ativo ? (
                        <button
                          className="btn-delete"
                          onClick={() =>
                            bloquearProduto(produto)
                          }
                        >
                          Bloquear
                        </button>
                      ) : (
                        <button
                          className="btn-activate"
                          onClick={() =>
                            ativarProduto(produto)
                          }
                        >
                          Ativar
                        </button>
                      )}
                    
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {modalAberto && (
        <div className="modal-overlay">
          <div className="produto-modal">
            <div className="modal-header">
              <div>
                <h2>
                  {modoEdicao
                    ? "Editar produto"
                    : "Novo produto"}
                </h2>

                <p>
                  Preencha os dados do produto.
                </p>
              </div>

              <button
                className="modal-close"
                onClick={fecharModal}
                type="button"
              >
                ×
              </button>
            </div>

            <form onSubmit={salvarProduto}>
              <div className="form-group">
                <label>Seção</label>

                <select
                  name="secaoId"
                  value={formulario.secaoId}
                  onChange={alterarCampo}
                  required
                >
                  <option value="">
                    Selecione uma seção
                  </option>

                  {secoesAtivas.map((secao) => (
                    <option
                      key={secao.id}
                      value={secao.id}
                    >
                      {secao.nome}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label>Nome</label>

                <input
                  type="text"
                  name="nome"
                  value={formulario.nome}
                  onChange={alterarCampo}
                  placeholder="Ex: Coca-Cola 2L"
                  required
                />
              </div>

              <div className="form-group">
                <label>Descrição</label>

                <textarea
                  name="descricao"
                  value={formulario.descricao}
                  onChange={alterarCampo}
                  placeholder="Descrição do produto"
                  rows="3"
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>Preço de venda</label>

                  <input
                    type="number"
                    name="precoVenda"
                    value={formulario.precoVenda}
                    onChange={alterarCampo}
                    placeholder="0,00"
                    min="0.01"
                    step="0.01"
                    required
                  />
                </div>

                <div className="form-group">
                  <label>Imagem URL</label>

                  <input
                    type="url"
                    name="imagemUrl"
                    value={formulario.imagemUrl}
                    onChange={alterarCampo}
                    placeholder="https://..."
                  />
                </div>
              </div>

              <div className="estoque-config">
                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    name="controlaEstoque"
                    checked={
                      formulario.controlaEstoque
                    }
                    onChange={alterarCampo}
                  />

                  <span>
                    Este produto controla estoque
                  </span>
                </label>

                {formulario.controlaEstoque && (
                  <div className="form-row">
                    {!modoEdicao && (
                      <div className="form-group">
                        <label>
                          Quantidade inicial
                        </label>

                        <input
                          type="number"
                          name="quantidadeInicial"
                          value={
                            formulario.quantidadeInicial
                          }
                          onChange={alterarCampo}
                          min="0"
                          step="0.001"
                        />
                      </div>
                    )}

                    <div className="form-group">
                      <label>
                        Quantidade mínima
                      </label>

                      <input
                        type="number"
                        name="quantidadeMinima"
                        value={
                          formulario.quantidadeMinima
                        }
                        onChange={alterarCampo}
                        min="0"
                        step="0.001"
                      />
                    </div>
                  </div>
                )}
              </div>

              <div className="modal-actions">
                <button
                  type="button"
                  className="btn-secondary"
                  onClick={fecharModal}
                  disabled={salvando}
                >
                  Cancelar
                </button>

                <button
                  type="submit"
                  className="btn-primary"
                  disabled={salvando}
                >
                  {salvando
                    ? "Salvando..."
                    : modoEdicao
                    ? "Salvar alterações"
                    : "Criar produto"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}