export interface Customer {
  id: string
  name: string
  email: string | null
  phone: string | null
  document: string | null
  createdAt: string
}

export interface CreateCustomerRequest {
  name: string
  email?: string | null
  phone?: string | null
  document?: string | null
}

export type UpdateCustomerRequest = CreateCustomerRequest

export interface CustomerQuery {
  page?: number
  pageSize?: number
  search?: string
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}
