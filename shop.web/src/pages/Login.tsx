import { useContext, useState } from 'react'
import api from '@/api/client'
import { AuthContext } from '@/contexts/AuthContext'

type Props = {
  onAuthSuccess: (token: string) => void
  onGotoRegister: () => void
}

export default function Login({ onAuthSuccess, onGotoRegister }: Props) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const auth = useContext(AuthContext)

  const handleSubmit = async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await api.auth.login(email, password)
      if (data && data.success) {
        await auth.login(data.token)
        if (onAuthSuccess) onAuthSuccess(data.token)
      } else {
        setError(data?.errorMessage || 'Invalid credentials')
      }
    } catch (e: any) {
      setError(e?.data?.message || e?.data || e?.message || 'Network error')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="bg-card p-6 rounded-lg shadow-md">
      <h2 className="text-2xl mb-4">Login</h2>
      {error && <div className="mb-4 text-destructive">{error}</div>}
      <label className="block mb-2">
        <span className="text-sm">Email</span>
        <input
          className="mt-1 block w-full rounded-md border border-border bg-input p-2"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="email@example.com"
        />
      </label>
      <label className="block mb-4">
        <span className="text-sm">Password</span>
        <input
          type="password"
          className="mt-1 block w-full rounded-md border border-border bg-input p-2"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="password"
        />
      </label>
      <div className="flex items-center justify-between">
        <button
          className="bg-primary text-primary-foreground px-4 py-2 rounded-md disabled:opacity-60"
          onClick={handleSubmit}
          disabled={loading}
        >
          {loading ? 'Signing...' : 'Sign in'}
        </button>
        <button className="text-sm underline" onClick={onGotoRegister}>
          Create account
        </button>
      </div>
    </div>
  )
}
