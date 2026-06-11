import { useEffect, useState } from 'react'
import api from '@/api/client'

export default function Orders() {
  const token = localStorage.getItem('token')
  const [orders, setOrders] = useState<any[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!token) return
    api.orders.myOrders(token).then((o) => setOrders(o || [])).catch((e) => setError(e?.data || e?.message || 'Failed to load orders'))
  }, [token])

  const handleCancel = async (id: string) => {
    if (!token) return
    try {
      await api.orders.cancel(token, id)
      setOrders((s) => s.filter((x) => x.id !== id))
    } catch (ex: any) {
      setError(ex?.data?.message || ex?.data || ex?.message || 'Cancel failed')
    }
  }

  return (
    <div className="bg-card p-6 rounded-lg shadow-md">
      <h2 className="text-2xl mb-4">My Orders</h2>
      {error && <div className="text-destructive mb-4">{error}</div>}

      {orders.length === 0 ? <div>No orders</div> : (
        <div className="grid grid-cols-1 gap-2">
          {orders.map((o: any) => (
            <div key={o.id} className="p-2 border border-border rounded flex justify-between items-center">
              <div>
                <div>Order {o.id}</div>
                <div className="text-sm text-muted">Status: {o.status}</div>
              </div>
              <div>
                <button className="px-2 py-1 text-sm underline" onClick={() => handleCancel(o.id)}>Cancel</button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
