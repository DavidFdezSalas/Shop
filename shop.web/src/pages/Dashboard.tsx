import { useEffect, useState } from 'react'
import api from '@/api/client'

export default function Dashboard() {
  const token = localStorage.getItem('token')
  const [products, setProducts] = useState<any[]>([])
  const [categories, setCategories] = useState<any[]>([])
  const [orders, setOrders] = useState<any[]>([])
  const [users, setUsers] = useState<any | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!token) return

    const load = async () => {
      try {
        const [p, c, o] = await Promise.all([
          api.products.getAll(token),
          api.categories.getAll(token),
          api.orders.myOrders(token),
        ])
        setProducts(p || [])
        setCategories(c || [])
        setOrders(o || [])

        // If admin, load users
        try {
          const u = await api.admin.getUsers(token)
          setUsers(u)
        } catch (e) {
          // ignore - not admin
        }
      } catch (ex: any) {
        setError(ex?.data?.message || ex?.data || ex?.message || 'Failed to fetch data')
      }
    }

    load()
  }, [token])

  return (
    <div className="bg-card p-6 rounded-lg shadow-md">
      <h2 className="text-2xl mb-4">Dashboard</h2>
      {error && <div className="text-destructive mb-4">{error}</div>}

      <section className="mb-6">
        <h3 className="font-medium mb-2">Products</h3>
        {products.length === 0 ? <div>No products</div> : (
          <div className="grid grid-cols-1 gap-2">
            {products.map((p: any) => (
              <div key={p.id} className="p-2 border border-border rounded">{p.name}</div>
            ))}
          </div>
        )}
      </section>

      <section className="mb-6">
        <h3 className="font-medium mb-2">Categories</h3>
        {categories.length === 0 ? <div>No categories</div> : (
          <div className="grid grid-cols-1 gap-2">
            {categories.map((c: any) => (
              <div key={c.id} className="p-2 border border-border rounded">{c.name}</div>
            ))}
          </div>
        )}
      </section>

      <section>
        <h3 className="font-medium mb-2">My Orders</h3>
        {orders.length === 0 ? <div>No orders</div> : (
          <div className="grid grid-cols-1 gap-2">
            {orders.map((o: any) => (
              <div key={o.id} className="p-2 border border-border rounded">Order {o.id} - {o.status}</div>
            ))}
          </div>
        )}
      </section>

      {users && (
        <section className="mt-6">
          <h3 className="font-medium mb-2">Users (admin)</h3>
          <div className="grid grid-cols-1 gap-2">
            {users.users.map((u: any) => (
              <div key={u.id} className="p-2 border border-border rounded">{u.userName} - {u.email}</div>
            ))}
          </div>
        </section>
      )}
    </div>
  )
}
