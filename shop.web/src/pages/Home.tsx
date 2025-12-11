import { AuthContext } from '@/contexts/AuthContext'
import { useContext, useEffect, useState } from 'react'

type Props = {
  onLogout: () => void
}

type UserInfo = {
  id: string
  userName: string
  email: string
  role: string
}

export default function Home({ onLogout }: Props) {
  const auth = useContext(AuthContext)
  const [user, setUser] = useState<UserInfo | null>(null)

  useEffect(() => {
    setUser(auth.user)
  }, [auth.user])

  return (
    <div className="bg-card p-6 rounded-lg shadow-md">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-2xl">Home</h2>
        <button className="text-sm underline" onClick={onLogout}>
          Logout
        </button>
      </div>

      {auth.loading && <div className="mb-4">Loading user...</div>}
      {auth.user && (
        <div className="mb-4">
          <div className="text-sm">Signed in as</div>
          <div className="font-medium">{auth.user.userName} ({auth.user.email})</div>
          <div className="text-xs text-muted">Role: {auth.user.role}</div>
        </div>
      )}

      {!auth.user && !auth.loading && <div className="mb-4">No user data available.</div>}

      <p>Welcome to the shop demo home page.</p>

      <div className="mt-6">
        <div className="grid grid-cols-1 gap-4">
          <div className="p-4 rounded border border-border">Product 1</div>
          <div className="p-4 rounded border border-border">Product 2</div>
          <div className="p-4 rounded border border-border">Product 3</div>
        </div>
      </div>

      <div className="mt-4 text-xs text-muted">Token: {auth.token ? auth.token.substring(0, 20) + '...' : 'not logged'}</div>
    </div>
  )
}
