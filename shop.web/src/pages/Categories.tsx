import { useEffect, useState } from 'react'
import api from '@/api/client'

export default function Categories() {
  const token = localStorage.getItem('token')
  const [categories, setCategories] = useState<any[]>([])
  const [error, setError] = useState<string | null>(null)
  const [creating, setCreating] = useState(false)
  const [name, setName] = useState('')

  useEffect(() => {
    if (!token) return
    api.categories.getAll(token).then((c) => setCategories(c || [])).catch((e) => setError(e?.data || e?.message || 'Failed to load categories'))
  }, [token])

  const handleCreate = async () => {
    if (!token || !name.trim()) return
    setError(null)
    try {
      const body = { name: name.trim(), description: '' }
      await api.categories.create(token, body)
      const updated = await api.categories.getAll(token)
      setCategories(updated || [])
      setCreating(false)
      setName('')
    } catch (ex: any) {
      setError(ex?.data?.message || ex?.data || ex?.message || 'Create failed')
    }
  }

  const handleDelete = async (id: number) => {
    if (!token) return
    try {
      await api.categories.delete(token, id)
      setCategories((s) => s.filter((c) => c.id !== id))
    } catch (ex: any) {
      setError(ex?.data?.message || ex?.data || ex?.message || 'Delete failed')
    }
  }

  return (
    <div className="bg-card p-6 rounded-lg shadow-md">
      <h2 className="text-2xl mb-4">Categories</h2>
      {error && <div className="text-destructive mb-4">{error}</div>}

      <div className="mb-4">
        <button className="px-3 py-1 rounded bg-primary text-primary-foreground" onClick={() => setCreating((c) => !c)}>
          {creating ? 'Cancel' : 'Create category'}
        </button>
      </div>

      {creating && (
        <div className="mb-4">
          <input className="w-full mb-2 p-2 rounded border border-border bg-input" placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} />
          <div className="flex justify-end">
            <button className="px-3 py-1 rounded bg-primary text-primary-foreground" onClick={handleCreate}>Create</button>
          </div>
        </div>
      )}

      {categories.length === 0 ? <div>No categories</div> : (
        <div className="grid grid-cols-1 gap-2">
          {categories.map((c: any) => (
            <div key={c.id} className="p-2 border border-border rounded flex justify-between items-center">
              <div>
                <div className="font-medium">{c.name}</div>
                <div className="text-sm text-muted">{c.description}</div>
              </div>
              <div>
                <button className="px-2 py-1 text-sm underline" onClick={() => handleDelete(c.id)}>Delete</button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
