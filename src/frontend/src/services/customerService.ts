import { apiFetch } from "@/lib/apiClient"
import type {
  CreateCustomerRequest,
  Customer,
  CustomerQuery,
  PagedResult,
  UpdateCustomerRequest,
} from "@/types/customer"

export function listCustomers(query: CustomerQuery = {}): Promise<PagedResult<Customer>> {
  const params = new URLSearchParams()
  if (query.page) params.set("page", String(query.page))
  if (query.pageSize) params.set("pageSize", String(query.pageSize))
  if (query.search) params.set("search", query.search)

  const queryString = params.toString()
  return apiFetch<PagedResult<Customer>>(`/api/customers${queryString ? `?${queryString}` : ""}`)
}

export function createCustomer(body: CreateCustomerRequest): Promise<Customer> {
  return apiFetch<Customer>("/api/customers", {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function updateCustomer(id: string, body: UpdateCustomerRequest): Promise<Customer> {
  return apiFetch<Customer>(`/api/customers/${id}`, {
    method: "PUT",
    body: JSON.stringify(body),
  })
}

export function deleteCustomer(id: string): Promise<void> {
  return apiFetch<void>(`/api/customers/${id}`, { method: "DELETE" })
}
