import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

import {
  cancelOrder,
  changeOrderStatus,
  createOrder,
  getOrder,
  listOrders,
} from "@/services/orderService"
import type { CreateOrderRequest, OrderQuery, OrderStatus } from "@/types/order"

const ORDERS_KEY = "orders"

export function useOrders(query: OrderQuery) {
  return useQuery({
    queryKey: [ORDERS_KEY, query],
    queryFn: () => listOrders(query),
  })
}

export function useOrder(id: string) {
  return useQuery({
    queryKey: [ORDERS_KEY, "detail", id],
    queryFn: () => getOrder(id),
  })
}

export function useCreateOrder() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: CreateOrderRequest) => createOrder(body),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [ORDERS_KEY] }),
  })
}

export function useCancelOrder() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => cancelOrder(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [ORDERS_KEY] }),
  })
}

export function useChangeOrderStatus() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: OrderStatus }) =>
      changeOrderStatus(id, status),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [ORDERS_KEY] }),
  })
}
