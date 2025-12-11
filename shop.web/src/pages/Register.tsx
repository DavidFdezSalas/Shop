import { useState } from 'react'

type Props = {
  onAuthSuccess: (token: string) => void
  onGotoLogin: () => void
}

type FieldErrors = Partial<Record<'username' | 'email' | 'password' | 'confirm', string>>

export default function Register({ onAuthSuccess, onGotoLogin }: Props) {
  const [email, setEmail] = useState('')
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [confirm, setConfirm] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({})

  const validate = (): boolean => {
    const errors: FieldErrors = {}
    const e = email.trim()
    const u = username.trim()

    if (!u) errors.username = 'Username is required.'
    if (!e) errors.email = 'Email is required.'
    else if (!/^\S+@\S+\.\S+$/.test(e)) errors.email = 'Invalid email address.'

    if (!password) errors.password = 'Password is required.'
    else if (password.length < 8) errors.password = 'Password must be at least 8 characters.'
    else if (!/[A-Z]/.test(password) || !/[a-z]/.test(password) || !/\d/.test(password))
      errors.password = 'Password must include upper, lower and number.'

    if (!confirm) errors.confirm = 'Please confirm your password.'
    else if (password !== confirm) errors.confirm = 'Passwords do not match.'

    setFieldErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSubmit = async (e?: React.FormEvent) => {
    e?.preventDefault()
    setError(null)

    if (!validate()) return

    setLoading(true)

    try {
      const base = (import.meta.env.VITE_API_IDENTITY_BASE as string) || 'https://localhost:7049'
      const url = `${base}/api/v1/Auth/register`

      const body = { username: username.trim(), email: email.trim(), password }

      const res = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      })

      if (!res.ok) {
        const text = await res.text()
        setError(text || `Registration failed (${res.status})`)
        setLoading(false)
        return
      }

      const registered = await res.json()
      if (!registered) {
        setError('Registration failed. Check your data and try again.')
        setLoading(false)
        return
      }

      // Obtain token by logging in
      const loginUrl = `${base}/api/v1/Auth/login`
      const loginRes = await fetch(loginUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: email.trim(), password }),
      })

      if (!loginRes.ok) {
        const t = await loginRes.text()
        setError(t || 'Registration succeeded but automatic login failed.')
        setLoading(false)
        return
      }

      const data = await loginRes.json()
      if (data && data.success && data.token) {
        onAuthSuccess(data.token)
        return
      }

      setError(data?.errorMessage || 'Login failed after registration')
    } catch (ex: any) {
      setError(ex?.message || 'Network error')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form className="bg-card p-6 rounded-lg shadow-md" onSubmit={handleSubmit} noValidate>
      <h2 className="text-2xl mb-4">Create account</h2>

      {error && <div role="alert" className="mb-4 text-destructive">{error}</div>}

      <label className="block mb-2">
        <span className="text-sm">Username</span>
        <input
          id="username"
          name="username"
          className={`mt-1 block w-full rounded-md border border-border bg-input p-2 ${fieldErrors.username ? 'ring-2 ring-destructive' : ''}`}
          value={username}
          onChange={(ev) => setUsername(ev.target.value)}
          placeholder="username"
          aria-invalid={!!fieldErrors.username}
          aria-describedby={fieldErrors.username ? 'username-error' : undefined}
        />
        {fieldErrors.username && (
          <div id="username-error" className="text-sm text-destructive mt-1">{fieldErrors.username}</div>
        )}
      </label>

      <label className="block mb-2">
        <span className="text-sm">Email</span>
        <input
          id="email"
          name="email"
          type="email"
          className={`mt-1 block w-full rounded-md border border-border bg-input p-2 ${fieldErrors.email ? 'ring-2 ring-destructive' : ''}`}
          value={email}
          onChange={(ev) => setEmail(ev.target.value)}
          placeholder="email@example.com"
          aria-invalid={!!fieldErrors.email}
          aria-describedby={fieldErrors.email ? 'email-error' : undefined}
        />
        {fieldErrors.email && <div id="email-error" className="text-sm text-destructive mt-1">{fieldErrors.email}</div>}
      </label>

      <label className="block mb-2">
        <span className="text-sm">Password</span>
        <input
          id="password"
          name="password"
          type="password"
          className={`mt-1 block w-full rounded-md border border-border bg-input p-2 ${fieldErrors.password ? 'ring-2 ring-destructive' : ''}`}
          value={password}
          onChange={(ev) => setPassword(ev.target.value)}
          placeholder="password"
          aria-invalid={!!fieldErrors.password}
          aria-describedby={fieldErrors.password ? 'password-error' : undefined}
        />
        {fieldErrors.password && (
          <div id="password-error" className="text-sm text-destructive mt-1">{fieldErrors.password}</div>
        )}
      </label>

      <label className="block mb-4">
        <span className="text-sm">Confirm password</span>
        <input
          id="confirm"
          name="confirm"
          type="password"
          className={`mt-1 block w-full rounded-md border border-border bg-input p-2 ${fieldErrors.confirm ? 'ring-2 ring-destructive' : ''}`}
          value={confirm}
          onChange={(ev) => setConfirm(ev.target.value)}
          placeholder="confirm password"
          aria-invalid={!!fieldErrors.confirm}
          aria-describedby={fieldErrors.confirm ? 'confirm-error' : undefined}
        />
        {fieldErrors.confirm && (
          <div id="confirm-error" className="text-sm text-destructive mt-1">{fieldErrors.confirm}</div>
        )}
      </label>

      <div className="flex items-center justify-between">
        <button
          type="submit"
          className={`px-4 py-2 rounded-md ${loading ? 'opacity-60' : ''} ${Object.keys(fieldErrors).length === 0 ? 'bg-primary text-primary-foreground' : 'bg-muted text-muted-foreground'}`}
          disabled={loading}
        >
          {loading ? 'Creating...' : 'Create account'}
        </button>
        <button type="button" className="text-sm underline" onClick={onGotoLogin}>
          Already have an account
        </button>
      </div>
    </form>
  )
}
