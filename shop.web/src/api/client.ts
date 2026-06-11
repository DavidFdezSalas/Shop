const getBase = (envKey: string, fallback: string) => (import.meta.env[envKey as any] as string) || fallback

// Default to API gateway on 7049 for all services
const IDENTITY_BASE = getBase('VITE_API_IDENTITY_BASE', 'https://localhost:7049')
const PRODUCTS_BASE = getBase('VITE_API_PRODUCTS_BASE', 'https://localhost:7049')
const ORDERS_BASE = getBase('VITE_API_ORDERS_BASE', 'https://localhost:7049')

const jsonHeaders = (token?: string) => {
  const h: Record<string, string> = { 'Content-Type': 'application/json' }
  if (token) h['Authorization'] = `Bearer ${token}`
  return h
}

async function handleResponse(res: Response) {
  const text = await res.text()
  try {
    const data = text ? JSON.parse(text) : null
    if (!res.ok) throw { status: res.status, data }
    return data
  } catch (e) {
    if (!res.ok) throw { status: res.status, data: text }
    return text
  }
}

export const api = {
  auth: {
    login: async (email: string, password: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Auth/login`, {
        method: 'POST',
        headers: jsonHeaders(),
        body: JSON.stringify({ email, password }),
      })
      return handleResponse(res)
    },
    register: async (username: string, email: string, password: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Auth/register`, {
        method: 'POST',
        headers: jsonHeaders(),
        body: JSON.stringify({ username, email, password }),
      })
      return handleResponse(res)
    },
  },
  users: {
    me: async (token?: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Users/me`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    updateMe: async (token: string, body: { userName?: string; email?: string }) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Users/me`, {
        method: 'PUT',
        headers: jsonHeaders(token),
        body: JSON.stringify(body),
      })
      return handleResponse(res)
    },
    changePassword: async (token: string, userId: string, currentPassword: string, newPassword: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Users/${userId}/change-password`, {
        method: 'PATCH',
        headers: jsonHeaders(token),
        body: JSON.stringify({ currentPassword, newPassword }),
      })
      return handleResponse(res)
    },
    deleteMe: async (token: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Users/me`, { method: 'DELETE', headers: jsonHeaders(token) })
      return handleResponse(res)
    },
  },
  admin: {
    getUsers: async (token: string, page = 1, pageSize = 10) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users?pageNumber=${page}&pageSize=${pageSize}`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    getUserById: async (token: string, userId: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users/${userId}`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    updateUser: async (token: string, userId: string, body: { userName?: string; email?: string }) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users/${userId}`, { method: 'PUT', headers: jsonHeaders(token), body: JSON.stringify(body) })
      return handleResponse(res)
    },
    deleteUser: async (token: string, userId: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users/${userId}`, { method: 'DELETE', headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    lockUser: async (token: string, userId: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users/${userId}/lock`, { method: 'PATCH', headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    unlockUser: async (token: string, userId: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users/${userId}/unlock`, { method: 'PATCH', headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    assignRole: async (token: string, userId: string, roleName: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users/${userId}/roles`, { method: 'POST', headers: jsonHeaders(token), body: JSON.stringify({ roleName }) })
      return handleResponse(res)
    },
    stats: async (token: string) => {
      const res = await fetch(`${IDENTITY_BASE}/api/v1/Admin/Users/stats`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
  },
  products: {
    getAll: async (token?: string) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Products`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    getById: async (token: string | undefined, id: string) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Products/${id}`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    getByCategory: async (token: string | undefined, categoryId: number) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Products/category/${categoryId}`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    create: async (token: string, body: any) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Products`, { method: 'POST', headers: jsonHeaders(token), body: JSON.stringify(body) })
      return handleResponse(res)
    },
    update: async (token: string, id: string, body: any) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Products/${id}`, { method: 'PUT', headers: jsonHeaders(token), body: JSON.stringify(body) })
      return handleResponse(res)
    },
    delete: async (token: string, id: string) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Products/${id}`, { method: 'DELETE', headers: jsonHeaders(token) })
      return handleResponse(res)
    },
  },
  categories: {
    getAll: async (token?: string) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Categories`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    getById: async (token: string | undefined, id: number) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Categories/${id}`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    create: async (token: string, body: any) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Categories`, { method: 'POST', headers: jsonHeaders(token), body: JSON.stringify(body) })
      return handleResponse(res)
    },
    update: async (token: string, id: number, body: any) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Categories/${id}`, { method: 'PUT', headers: jsonHeaders(token), body: JSON.stringify(body) })
      return handleResponse(res)
    },
    delete: async (token: string, id: number) => {
      const res = await fetch(`${PRODUCTS_BASE}/api/v1/Categories/${id}`, { method: 'DELETE', headers: jsonHeaders(token) })
      return handleResponse(res)
    },
  },
  orders: {
    create: async (token: string, body: any) => {
      const res = await fetch(`${ORDERS_BASE}/api/v1/Orders`, { method: 'POST', headers: jsonHeaders(token), body: JSON.stringify(body) })
      return handleResponse(res)
    },
    getById: async (token: string, id: string) => {
      const res = await fetch(`${ORDERS_BASE}/api/v1/Orders/${id}`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    myOrders: async (token: string) => {
      const res = await fetch(`${ORDERS_BASE}/api/v1/Orders/my-orders`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    all: async (token: string) => {
      const res = await fetch(`${ORDERS_BASE}/api/v1/Orders`, { headers: jsonHeaders(token) })
      return handleResponse(res)
    },
    updateStatus: async (token: string, id: string, status: string) => {
      const res = await fetch(`${ORDERS_BASE}/api/v1/Orders/${id}/status`, { method: 'PATCH', headers: jsonHeaders(token), body: JSON.stringify({ status }) })
      return handleResponse(res)
    },
    cancel: async (token: string, id: string) => {
      const res = await fetch(`${ORDERS_BASE}/api/v1/Orders/${id}/cancel`, { method: 'POST', headers: jsonHeaders(token) })
      return handleResponse(res)
    },
  },
}

export default api
