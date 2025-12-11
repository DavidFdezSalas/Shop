import { useEffect, useState } from 'react'
import api from '@/api/client'

export default function Products() {
  const token = localStorage.getItem('token')
  const [products, setProducts] = useState<any[]>([])
  const [error, setError] = useState<string | null>(null)
  const [creating, setCreating] = useState(false)
  const [name, setName] = useState('')
  const [price, setPrice] = useState('')

  useEffect(() => {
    if (!token) return
    api.products.getAll(token).then((p) => setProducts(p || [])).catch((e) => setError(e?.data || e?.message || 'Failed to load products'))
  }, [token])

  const handleCreate = async () => {
    if (!token) return
    setError(null)
    try {
      const body = { name, description: '', price: parseFloat(price) || 0, categoryId: 0 }
      await api.products.create(token, body)
      const updated = await api.products.getAll(token)
      setProducts(updated || [])
      setCreating(false)
      setName('')
      setPrice('')
    } catch (ex: any) {
      setError(ex?.data?.message || ex?.data || ex?.message || 'Create failed')
    }
  }

  return (
    <div className="bg-card p-6 rounded-lg shadow-md">
      <h2 className="text-2xl mb-4">Products</h2>
      {error && <div className="text-destructive mb-4">{error}</div>}

      <div className="mb-4">
        <button className="px-3 py-1 rounded bg-primary text-primary-foreground" onClick={() => setCreating((c) => !c)}>
          {creating ? 'Cancel' : 'Create product'}
        </button>
      </div>

      {creating && (
        <div className="mb-4">
          <input className="w-full mb-2 p-2 rounded border border-border bg-input" placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} />
          <input className="w-full mb-2 p-2 rounded border border-border bg-input" placeholder="Price" value={price} onChange={(e) => setPrice(e.target.value)} />
          <div className="flex justify-end">
            <button className="px-3 py-1 rounded bg-primary text-primary-foreground" onClick={handleCreate}>Create</button>
          </div>
        </div>
      )}

      {products.length === 0 ? <div>No products</div> : (
        <div className="grid grid-cols-1 gap-2">
          {products.map((p: any) => (
            <div key={p.id} className="p-2 border border-border rounded flex justify-between items-center">
              <div>
                <div className="font-medium">{p.name}</div>
                <div className="text-sm text-muted">{p.description}</div>
              </div>
              <div className="text-sm">${p.price?.toFixed?.(2) ?? p.price}</div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
