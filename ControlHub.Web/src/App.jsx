import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Home from './pages/home/Home'
import Login from './pages/login/Login'
import Master from './pages/master/Master'
import Empresas from './pages/master/empresas/Empresas'

function App() {
  return (
    <BrowserRouter>
      <Routes>

        <Route path="/" element={<Home />} />

        <Route path="/login" element={<Login />} />

        <Route path="/master" element={<Master />} />

        <Route path="/master/empresas" element={<Empresas />} />

        <Route path="/master/empresas/criar" element={<Empresas />} />

      </Routes>
    </BrowserRouter>
  )
}

export default App