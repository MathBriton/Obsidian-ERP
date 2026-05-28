import { useState } from "react"
import { Link } from "react-router-dom"
import { Plus } from "lucide-react"

import { CreateOrderDialog } from "@/components/orders/CreateOrderDialog"
import { OrderStatusBadge } from "@/components/orders/OrderStatusBadge"
import { Button } from "@/components/ui/button"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { useOrders } from "@/hooks/useOrders"
import type { OrderStatus } from "@/types/order"

const PAGE_SIZE = 10

const FILTERS: { label: string; value: OrderStatus | "all" }[] = [
  { label: "Todos", value: "all" },
  { label: "Pendente", value: "Pending" },
  { label: "Confirmado", value: "Confirmed" },
  { label: "Concluído", value: "Completed" },
  { label: "Cancelado", value: "Cancelled" },
]

function formatCurrency(value: number): string {
  return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString("pt-BR")
}

export function OrdersPage() {
  const [statusFilter, setStatusFilter] = useState<OrderStatus | "all">("all")
  const [page, setPage] = useState(1)
  const [createOpen, setCreateOpen] = useState(false)

  const { data, isPending, isError } = useOrders({
    page,
    pageSize: PAGE_SIZE,
    status: statusFilter === "all" ? undefined : statusFilter,
  })

  const totalPages = data?.totalPages ?? 0

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-bold tracking-tight">Pedidos</h1>
          <p className="text-muted-foreground">Acompanhe e gerencie os pedidos.</p>
        </div>
        <Button onClick={() => setCreateOpen(true)}>
          <Plus />
          Novo pedido
        </Button>
      </div>

      <div className="flex flex-wrap gap-2">
        {FILTERS.map((filter) => (
          <Button
            key={filter.value}
            variant={statusFilter === filter.value ? "default" : "outline"}
            size="sm"
            onClick={() => {
              setStatusFilter(filter.value)
              setPage(1)
            }}
          >
            {filter.label}
          </Button>
        ))}
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Cliente</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Total</TableHead>
              <TableHead>Data</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isPending && (
              <TableRow>
                <TableCell colSpan={5} className="text-muted-foreground text-center">
                  Carregando…
                </TableCell>
              </TableRow>
            )}
            {isError && (
              <TableRow>
                <TableCell colSpan={5} className="text-destructive text-center">
                  Erro ao carregar pedidos.
                </TableCell>
              </TableRow>
            )}
            {data && data.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} className="text-muted-foreground text-center">
                  Nenhum pedido encontrado.
                </TableCell>
              </TableRow>
            )}
            {data?.items.map((order) => (
              <TableRow key={order.id}>
                <TableCell className="font-medium">{order.customerName ?? "—"}</TableCell>
                <TableCell>
                  <OrderStatusBadge status={order.status} />
                </TableCell>
                <TableCell>{formatCurrency(order.total)}</TableCell>
                <TableCell>{formatDate(order.createdAt)}</TableCell>
                <TableCell className="text-right">
                  <Button asChild variant="ghost" size="sm">
                    <Link to={`/orders/${order.id}`}>Ver</Link>
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {data && totalPages > 1 && (
        <div className="flex items-center justify-between">
          <span className="text-muted-foreground text-sm">
            Página {data.page} de {totalPages} · {data.totalCount} pedidos
          </span>
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={page <= 1}
              onClick={() => setPage((current) => current - 1)}
            >
              Anterior
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={page >= totalPages}
              onClick={() => setPage((current) => current + 1)}
            >
              Próxima
            </Button>
          </div>
        </div>
      )}

      <CreateOrderDialog open={createOpen} onOpenChange={setCreateOpen} />
    </div>
  )
}
