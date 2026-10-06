import Inicio from '../modules/Inicio/Inicio'
import Pedidos from '../modules/Pedidos/Pedidos'
import Produtos from '../modules/Produtos/Produtos'
import Estoque from '../modules/Estoque/Estoque'
import Financeiro from '../modules/Financeiro/Financeiro'
import Usuarios from '../modules/Usuarios/Usuarios'
import Configuracoes from '../modules/Configuracoes/Configuracoes'

export const menusEmpresa = [
  {
    id: 'inicio',
    nome: 'Início',
    icone: '⌂',
    componente: Inicio,
  },
  {
    id: 'pedidos',
    nome: 'Pedidos',
    icone: '🛒',
    componente: Pedidos,
  },
  {
    id: 'produtos',
    nome: 'Produtos',
    icone: '▣',
    componente: Produtos,
  },
  {
    id: 'estoque',
    nome: 'Estoque',
    icone: '▤',
    componente: Estoque,
  },
  {
    id: 'financeiro',
    nome: 'Financeiro',
    icone: 'R$',
    componente: Financeiro,
  },
  {
    id: 'usuarios',
    nome: 'Usuários',
    icone: '◉',
    componente: Usuarios,
  },
  {
    id: 'configuracoes',
    nome: 'Configurações',
    icone: '⚙',
    componente: Configuracoes,
  },
]