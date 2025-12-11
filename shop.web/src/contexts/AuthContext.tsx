import { createContext, useEffect, useState } from 'react'
import api from '@/api/client'

type User = { id: string; userName: string; email: string; role: string }

type AuthContextType = {
  token: string | null
  user: User | null
  loading: boolean
  login: (token: string) => Promise<void>
  logout: () => void
  refreshUser: () => Promise<void>
}

export const AuthContext = createContext<AuthContextType>({
  token: null,
  user: null,
  loading: false,
  login: async () => {},
  logout: () => {},
  refreshUser: async () => {},
})

export function AuthProvider({ children }: { children: any }) {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'))
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (token) {
      refreshUser()
    }
  }, [])

  const login = async (t: string) => {
    localStorage.setItem('token', t)
    setToken(t)
    await refreshUser()
  }

  const logout = () => {
    localStorage.removeItem('token')
    setToken(null)
    setUser(null)
    location.hash = '#/login'
  }

  const refreshUser = async () => {
    if (!localStorage.getItem('token')) {
      setUser(null)
      return
    }
    setLoading(true)
    try {
      const t = localStorage.getItem('token')!
      const data = await api.users.me(t)
      setUser(data)
    } catch (e) {
      setUser(null)
      // token invalid -> logout
      logout()
    } finally {
      setLoading(false)
    }
  }

  return (
    <AuthContext.Provider value={{ token, user, loading, login, logout, refreshUser }}>
      {children}
    </AuthContext.Provider>
  )
}
