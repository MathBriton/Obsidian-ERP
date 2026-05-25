import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

import {
  createCustomer,
  deleteCustomer,
  listCustomers,
  updateCustomer,
} from "@/services/customerService"
import type {
  CreateCustomerRequest,
  CustomerQuery,
  UpdateCustomerRequest,
} from "@/types/customer"

const CUSTOMERS_KEY = "customers"

export function useCustomers(query: CustomerQuery) {
  return useQuery({
    queryKey: [CUSTOMERS_KEY, query],
    queryFn: () => listCustomers(query),
  })
}

export function useCreateCustomer() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: CreateCustomerRequest) => createCustomer(body),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [CUSTOMERS_KEY] }),
  })
}

export function useUpdateCustomer() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateCustomerRequest }) =>
      updateCustomer(id, body),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [CUSTOMERS_KEY] }),
  })
}

export function useDeleteCustomer() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => deleteCustomer(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [CUSTOMERS_KEY] }),
  })
}
