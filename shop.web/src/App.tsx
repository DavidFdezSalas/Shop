import { useEffect, useState } from 'react'
import './App.css'
import Login from './pages/Login'
import Register from './pages/Register'
import Home from './pages/Home'
import Dashboard from './pages/Dashboard'
import Products from './pages/Products'
import Categories from './pages/Categories'
import Orders from './pages/Orders'
import Profile from './pages/Profile'

type Route = 'login' | 'register' | 'home' | 'dashboard' | 'products' | 'categories' | 'orders' | 'profile'

function getRouteFromHash(): Route {
  const hash = location.hash.replace('#/', '')
  if (hash === 'register') return 'register'
  if (hash === 'home') return 'home'
  if (hash === 'dashboard') return 'dashboard'
  if (hash === 'products') return 'products'
  if (hash === 'categories') return 'categories'
  if (hash === 'orders') return 'orders'
  if (hash === 'profile') return 'profile'
  return 'login'
}

function App() {
  const [route, setRoute] = useState<Route>(getRouteFromHash())

  useEffect(() => {
    const onHash = () => setRoute(getRouteFromHash())
    window.addEventListener('hashchange', onHash)
    return () => window.removeEventListener('hashchange', onHash)
  }, [])

  useEffect(() => {
    if (!location.hash) {
      const token = localStorage.getItem('token')
      if (token) {
        location.hash = '#/home'
      } else {
        location.hash = '#/login'
      }
    }
  }, [])

  const handleAuthSuccess = (token: string) => {
    localStorage.setItem('token', token)
    location.hash = '#/home'
  }

  const handleLogout = () => {
    localStorage.removeItem('token')
    location.hash = '#/login'
  }

  const isAuthPage = route === 'login' || route === 'register'

  return (
    <div className="min-h-screen flex items-center justify-center bg-background text-foreground p-4">
      <div className={isAuthPage ? 'w-full max-w-md' : 'w-full max-w-4xl'}>
        {!isAuthPage && (
          <div className="flex justify-between mb-4">
            <div className="flex gap-2">
              <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={() => (location.hash = '#/home')}>Home</button>
              <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={() => (location.hash = '#/dashboard')}>Dashboard</button>
              <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={() => (location.hash = '#/products')}>Products</button>
              <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={() => (location.hash = '#/categories')}>Categories</button>
              <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={() => (location.hash = '#/orders')}>Orders</button>
              <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={() => (location.hash = '#/profile')}>Profile</button>
            </div>
            <div />
          </div>
        )}

        {route === 'login' && (
          <Login onAuthSuccess={handleAuthSuccess} onGotoRegister={() => (location.hash = '#/register')} />
        )}
        {route === 'register' && (
          <Register onAuthSuccess={handleAuthSuccess} onGotoLogin={() => (location.hash = '#/login')} />
        )}
        {route === 'home' && <Home onLogout={handleLogout} />}
        {route === 'dashboard' && <Dashboard />}
        {route === 'products' && <Products />}
        {route === 'categories' && <Categories />}
        {route === 'orders' && <Orders />}
        {route === 'profile' && <Profile />}
      </div>
    </div>
  )
}

export default App
