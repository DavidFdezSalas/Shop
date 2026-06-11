import { useContext, useState } from 'react'
import api from '@/api/client'
import { AuthContext } from '@/contexts/AuthContext'

export default function Profile() {
  const auth = useContext(AuthContext)
  const token = auth.token
  const user = auth.user

  const [editing, setEditing] = useState(false)
  const [userName, setUserName] = useState(user?.userName ?? '')
  const [email, setEmail] = useState(user?.email ?? '')
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  const handleUpdate = async () => {
    if (!token) return
    setError(null)
    setMessage(null)
    try {
      await api.users.updateMe(token, { userName, email })
      setMessage('Profile updated')
      await auth.refreshUser()
      setEditing(false)
    } catch (ex: any) {
      setError(ex?.data?.message || ex?.data || ex?.message || 'Update failed')
    }
  }

  const handleChangePassword = async () => {
    if (!token || !user) return
    setError(null)
    setMessage(null)
    try {
      const res = await api.users.changePassword(token, user.id, currentPassword, newPassword)
      setMessage('Password changed')
      setCurrentPassword('')
      setNewPassword('')
    } catch (ex: any) {
      setError(ex?.data?.message || ex?.data || ex?.message || 'Change password failed')
    }
  }

  const handleDelete = async () => {
    if (!token) return
    if (!confirm('Are you sure you want to delete your account? This action is irreversible.')) return
    setError(null)
    try {
      await api.users.deleteMe(token)
      auth.logout()
    } catch (ex: any) {
      setError(ex?.data?.message || ex?.data || ex?.message || 'Delete failed')
    }
  }

  return (
    <div className="bg-card p-6 rounded-lg shadow-md">
      <h2 className="text-2xl mb-4">Profile</h2>

      {message && <div className="mb-4 text-green-400">{message}</div>}
      {error && <div className="mb-4 text-destructive">{error}</div>}

      {user ? (
        <div>
          {!editing ? (
            <div className="mb-4">
              <div className="text-sm">Username</div>
              <div className="font-medium">{user.userName}</div>
              <div className="text-sm mt-2">Email</div>
              <div className="font-medium">{user.email}</div>
              <div className="text-xs text-muted mt-2">Role: {user.role}</div>
              <div className="mt-4 flex gap-2">
                <button className="px-3 py-1 rounded bg-primary text-primary-foreground" onClick={() => { setEditing(true); setUserName(user.userName); setEmail(user.email); }}>Edit</button>
                <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={handleDelete}>Delete account</button>
              </div>
            </div>
          ) : (
            <div className="mb-4">
              <label className="block mb-2">
                <span className="text-sm">Username</span>
                <input className="mt-1 block w-full rounded-md border border-border bg-input p-2" value={userName} onChange={(e) => setUserName(e.target.value)} />
              </label>
              <label className="block mb-2">
                <span className="text-sm">Email</span>
                <input className="mt-1 block w-full rounded-md border border-border bg-input p-2" value={email} onChange={(e) => setEmail(e.target.value)} />
              </label>
              <div className="flex gap-2 mt-2">
                <button className="px-3 py-1 rounded bg-primary text-primary-foreground" onClick={handleUpdate}>Save</button>
                <button className="px-3 py-1 rounded bg-muted text-muted-foreground" onClick={() => setEditing(false)}>Cancel</button>
              </div>
            </div>
          )}

          <div className="mt-6">
            <h3 className="font-medium mb-2">Change password</h3>
            <label className="block mb-2">
              <span className="text-sm">Current password</span>
              <input type="password" className="mt-1 block w-full rounded-md border border-border bg-input p-2" value={currentPassword} onChange={(e) => setCurrentPassword(e.target.value)} />
            </label>
            <label className="block mb-2">
              <span className="text-sm">New password</span>
              <input type="password" className="mt-1 block w-full rounded-md border border-border bg-input p-2" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
            </label>
            <div className="flex justify-end">
              <button className="px-3 py-1 rounded bg-primary text-primary-foreground" onClick={handleChangePassword}>Change password</button>
            </div>
          </div>
        </div>
      ) : (
        <div>No user data</div>
      )}
    </div>
  )
}
