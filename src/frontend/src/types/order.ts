export type OrderStatus = "Pending" | "Confirmed" | "Completed" | "Cancelled"

export interface OrderItem {
  id: string
  productName: string
  quantity: number
  unitPrice: number
  lineTotal: number
}

export interface Order {
  id: string
  customerId: string
  customerName: string | null
  status: OrderStatus
  total: number
  createdAt: string
  items: OrderItem[]
}

export interface OrderListItem {
  id: string
  customerId: string
  customerName: string | null
  status: OrderStatus
  total: number
  createdAt: string
}

export interface OrderItemRequest {
  productName: string
  quantity: number
  unitPrice: number
}

export interface CreateOrderRequest {
  customerId: string
  items: OrderItemRequest[]
}

export interface OrderQuery {
  page?: number
  pageSize?: number
  status?: OrderStatus
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}
