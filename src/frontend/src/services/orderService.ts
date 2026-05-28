import { apiFetch } from "@/lib/apiClient"
import type {
  CreateOrderRequest,
  Order,
  OrderListItem,
  OrderQuery,
  OrderStatus,
  PagedResult,
} from "@/types/order"

export function listOrders(query: OrderQuery = {}): Promise<PagedResult<OrderListItem>> {
  const params = new URLSearchParams()
  if (query.page) params.set("page", String(query.page))
  if (query.pageSize) params.set("pageSize", String(query.pageSize))
  if (query.status) params.set("status", query.status)

  const queryString = params.toString()
  return apiFetch<PagedResult<OrderListItem>>(
    `/api/orders${queryString ? `?${queryString}` : ""}`
  )
}

export function getOrder(id: string): Promise<Order> {
  return apiFetch<Order>(`/api/orders/${id}`)
}

export function createOrder(body: CreateOrderRequest): Promise<Order> {
  return apiFetch<Order>("/api/orders", {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function cancelOrder(id: string): Promise<Order> {
  return apiFetch<Order>(`/api/orders/${id}/cancel`, { method: "POST" })
}

export function changeOrderStatus(id: string, status: OrderStatus): Promise<Order> {
  return apiFetch<Order>(`/api/orders/${id}/status`, {
    method: "PUT",
    body: JSON.stringify({ status }),
  })
}
