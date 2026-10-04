import {
  ArrowRight,
  ArrowUpRight,
  Building2,
  Camera,
  LockKeyhole,
  Mail,
  MessageCircle,
  ShieldCheck,
  Store,
  UsersRound,
  Workflow,
} from 'lucide-react'
import './Home.css'

const WHATSAPP_URL = 'https://wa.me/5531991070255'
const INSTAGRAM_URL = 'https://instagram.com/andremanoelino'
const EMAIL = 'agmphandre@gmail'

const recursos = [
  {
    icon: Building2,
    indice: '01',
    titulo: 'Empresas organizadas',
    descricao: 'Cadastre dados, identidade visual e o endereço público de cada empresa em um só lugar.',
  },
  {
    icon: UsersRound,
    indice: '02',
    titulo: 'Pessoas e permissões',
    descricao: 'Cada conta acessa o painel correspondente ao seu perfil e à empresa vinculada.',
  },
  {
    icon: ShieldCheck,
    indice: '03',
    titulo: 'Separação por empresa',
    descricao: 'A empresa define o ambiente de trabalho. Donos e equipes não consultam dados de outras empresas.',
  },
  {
    icon: Store,
    indice: '04',
    titulo: 'Endereço da loja',
    descricao: 'Cada loja pode ser acessada pelo endereço público cadastrado, como /loja/minha-loja.',
  },
]

const etapas = [
  { icon: Building2, titulo: 'A empresa é cadastrada', texto: 'Dados principais, visual e endereço da loja.' },
  { icon: UsersRound, titulo: 'O responsável recebe acesso', texto: 'O dono administra somente o próprio ambiente.' },
  { icon: LockKeyhole, titulo: 'A equipe entra no seu perfil', texto: 'As permissões seguem o vínculo com a empresa.' },
]

function App() {
  return (
    <div className="app home-page">
      <header className="header">
        <div className="container header-content">
          <a href="#inicio" className="logo" aria-label="ControlHub, início">
            Control<span>Hub</span>
          </a>

          <nav className="nav" aria-label="Navegação principal">
            <a href="#recursos">Recursos</a>
            <a href="#fluxo">Como funciona</a>
            <a href="#contato">Contato</a>
          </nav>

          <a href="/login" className="login-button">
            Entrar <ArrowUpRight size={16} aria-hidden="true" />
          </a>
        </div>
      </header>

      <main>
        <section id="inicio" className="hero-section">
          <div className="hero-background" aria-hidden="true" />
          <div className="container hero-content">
            <div className="hero-text">
              <div className="hero-badge">
                <span className="badge-dot" />
                Plataforma de gestão multiempresa
              </div>

              <h1>
                Cada empresa no seu espaço.
                <span>Todo mundo no controle.</span>
              </h1>

              <p className="hero-description">
                Organize empresas, responsáveis e equipes em ambientes separados.
                Acesse o painel certo ou visite a página pública de uma loja pelo endereço cadastrado.
              </p>

              <div className="hero-actions">
                <a href="/login" className="primary-button">
                  Acessar o painel <ArrowRight size={17} aria-hidden="true" />
                </a>
                <a href="#recursos" className="secondary-button">Conhecer a plataforma</a>
              </div>

              <div className="hero-signals" aria-label="Princípios da plataforma">
                <span><ShieldCheck size={16} aria-hidden="true" /> Dados isolados por empresa</span>
                <span><UsersRound size={16} aria-hidden="true" /> Acesso por perfil</span>
              </div>
            </div>

            <div className="hero-visual" aria-label="Prévia da navegação do ControlHub">
              <div className="home-preview">
                <div className="home-preview-topbar">
                  <span className="preview-mark">CH</span>
                  <span className="preview-brand">ControlHub</span>
                  <span className="preview-state"><i /> Multiempresa</span>
                </div>
                <div className="home-preview-body">
                  <aside className="preview-rail" aria-hidden="true">
                    <span className="selected"><Building2 size={17} /></span>
                    <span><UsersRound size={17} /></span>
                    <span><Store size={17} /></span>
                    <span><LockKeyhole size={17} /></span>
                  </aside>
                  <div className="preview-content">
                    <span className="preview-eyebrow">VISÃO GERAL</span>
                    <h2>Um painel para cada operação</h2>
                    <p>Empresas e acessos organizados por perfil.</p>
                    <div className="preview-lines">
                      <div><span className="preview-icon"><Building2 size={16} /></span><span><b>Empresas</b><small>Dados e identidade visual</small></span><ArrowRight size={15} /></div>
                      <div><span className="preview-icon teal"><UsersRound size={16} /></span><span><b>Usuários</b><small>Responsáveis e equipe</small></span><ArrowRight size={15} /></div>
                      <div><span className="preview-icon gold"><Store size={16} /></span><span><b>Lojas</b><small>Endereços públicos próprios</small></span><ArrowRight size={15} /></div>
                    </div>
                    <div className="preview-footer"><Workflow size={15} /> Vínculos claros. Acessos separados.</div>
                  </div>
                </div>
              </div>
              <div className="preview-caption">EMPRESA <span /> USUÁRIOS <span /> LOJA</div>
            </div>
          </div>
        </section>

        <section id="recursos" className="section home-features">
          <div className="container">
            <div className="section-heading home-section-heading">
              <span className="section-kicker">Uma base clara</span>
              <h2>Gestão por empresa, do cadastro ao acesso.</h2>
              <p>O ControlHub mantém cada organização no próprio ambiente e deixa as permissões ligadas ao perfil do usuário.</p>
            </div>

            <div className="home-feature-grid">
              {recursos.map((recurso) => {
                const Icone = recurso.icon
                return (
                  <article className="home-feature" key={recurso.indice}>
                    <div className="home-feature-top">
                      <span className="home-feature-icon"><Icone size={21} strokeWidth={1.8} aria-hidden="true" /></span>
                      <span className="home-feature-index">{recurso.indice}</span>
                    </div>
                    <h3>{recurso.titulo}</h3>
                    <p>{recurso.descricao}</p>
                  </article>
                )
              })}
            </div>
          </div>
        </section>

        <section id="fluxo" className="section home-flow">
          <div className="container home-flow-layout">
            <div className="home-flow-intro">
              <span className="section-kicker">Fluxo de acesso</span>
              <h2>Do cadastro ao ambiente certo.</h2>
              <p>Um caminho direto para cada pessoa encontrar a empresa e as ferramentas que pode acessar.</p>
              <a href="/login" className="text-link">Ir para o painel <ArrowRight size={16} aria-hidden="true" /></a>
            </div>

            <div className="home-steps">
              {etapas.map((etapa, index) => {
                const Icone = etapa.icon
                return (
                  <article className="home-step" key={etapa.titulo}>
                    <span className="home-step-number">0{index + 1}</span>
                    <span className="home-step-icon"><Icone size={19} aria-hidden="true" /></span>
                    <div>
                      <h3>{etapa.titulo}</h3>
                      <p>{etapa.texto}</p>
                    </div>
                  </article>
                )
              })}
            </div>
          </div>
        </section>

        <section id="contato" className="section home-contact">
          <div className="container home-contact-layout">
            <div>
              <span className="section-kicker">Contato</span>
              <h2>Fale com a equipe ControlHub.</h2>
              <p>Escolha o canal que for melhor para você.</p>
            </div>
            <div className="home-contact-links">
              <a href={WHATSAPP_URL} target="_blank" rel="noreferrer">
                <MessageCircle size={19} aria-hidden="true" /><span><b>WhatsApp</b><small>Conversa direta</small></span><ArrowUpRight size={16} aria-hidden="true" />
              </a>
              <a href={INSTAGRAM_URL} target="_blank" rel="noreferrer">
                <Camera size={19} aria-hidden="true" /><span><b>Instagram</b><small>@andremanoelino</small></span><ArrowUpRight size={16} aria-hidden="true" />
              </a>
              <a href={`mailto:${EMAIL}`}>
                <Mail size={19} aria-hidden="true" /><span><b>E-mail</b><small>{EMAIL}</small></span><ArrowUpRight size={16} aria-hidden="true" />
              </a>
            </div>
          </div>
        </section>
      </main>

      <footer className="footer home-footer">
        <div className="container footer-bottom">
          <a href="#inicio" className="logo">Control<span>Hub</span></a>
          <span>© 2026 ControlHub</span>
          <a href="/login">Entrar <ArrowUpRight size={14} aria-hidden="true" /></a>
        </div>
      </footer>
    </div>
  )
}

export default App
