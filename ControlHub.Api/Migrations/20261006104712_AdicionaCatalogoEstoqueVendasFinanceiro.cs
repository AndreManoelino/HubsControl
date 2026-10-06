using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCatalogoEstoqueVendasFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fornecedores_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesFinanceiras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Entrada = table.Column<bool>(type: "boolean", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesFinanceiras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesFinanceiras_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Secoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secoes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComprasFornecedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NumeroNota = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataCompra = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasFornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasFornecedor_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasFornecedor_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SecaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PrecoVenda = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ImagemUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImagemArquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ControlaEstoque = table.Column<bool>(type: "boolean", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produtos_Secoes_SecaoId",
                        column: x => x.SecaoId,
                        principalTable: "Secoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estoques",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantidadeMinima = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estoques_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estoques_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensCompraFornecedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompraFornecedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CustoTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCompraFornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCompraFornecedor_ComprasFornecedor_CompraFornecedorId",
                        column: x => x.CompraFornecedorId,
                        principalTable: "ComprasFornecedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensCompraFornecedor_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeProduto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CustoTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Lucro = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantidadeAnterior = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantidadeAtual = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFornecedor_EmpresaId_DataCompra",
                table: "ComprasFornecedor",
                columns: new[] { "EmpresaId", "DataCompra" });

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFornecedor_FornecedorId",
                table: "ComprasFornecedor",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_EmpresaId_ProdutoId",
                table: "Estoques",
                columns: new[] { "EmpresaId", "ProdutoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_ProdutoId",
                table: "Estoques",
                column: "ProdutoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_EmpresaId_Nome",
                table: "Fornecedores",
                columns: new[] { "EmpresaId", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompraFornecedor_CompraFornecedorId",
                table: "ItensCompraFornecedor",
                column: "CompraFornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompraFornecedor_ProdutoId",
                table: "ItensCompraFornecedor",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_ProdutoId",
                table: "ItensVenda",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_VendaId",
                table: "ItensVenda",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_EmpresaId_ProdutoId_CriadaEm",
                table: "MovimentacoesEstoque",
                columns: new[] { "EmpresaId", "ProdutoId", "CriadaEm" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesFinanceiras_EmpresaId_CriadaEm",
                table: "MovimentacoesFinanceiras",
                columns: new[] { "EmpresaId", "CriadaEm" });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_EmpresaId_Nome",
                table: "Produtos",
                columns: new[] { "EmpresaId", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_SecaoId",
                table: "Produtos",
                column: "SecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Secoes_EmpresaId_Nome",
                table: "Secoes",
                columns: new[] { "EmpresaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_EmpresaId_CriadaEm",
                table: "Vendas",
                columns: new[] { "EmpresaId", "CriadaEm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estoques");

            migrationBuilder.DropTable(
                name: "ItensCompraFornecedor");

            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "MovimentacoesFinanceiras");

            migrationBuilder.DropTable(
                name: "ComprasFornecedor");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "Secoes");
        }
    }
}
