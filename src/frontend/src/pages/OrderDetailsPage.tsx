import { ArrowLeft } from "lucide-react"
import { Link, useParams } from "react-router-dom"
import { toast } from "sonner"

import { OrderStatusBadge } from "@/components/orders/OrderStatusBadge"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { useCancelOrder, useChangeOrderStatus, useOrder } from "@/hooks/useOrders"
import type { OrderStatus } from "@/types/order"

function formatCurrency(value: number): string {
  return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })
}

export function OrderDetailsPage() {
  const { id = "" } = useParams()
  const { data: order, isPending, isError } = useOrder(id)
  const cancelMutation = useCancelOrder()
  const changeStatusMutation = useChangeOrderStatus()

  const busy = cancelMutation.isPending || changeStatusMutation.isPending

  async function handleChangeStatus(status: OrderStatus) {
    try {
      await changeStatusMutation.mutateAsync({ id, status })
      toast.success("Status atualizado.")
    } catch {
      toast.error("Não foi possível alterar o status.")
    }
  }

  async function handleCancel() {
    try {
      await cancelMutation.mutateAsync(id)
      toast.success("Pedido cancelado.")
    } catch {
      toast.error("Não foi possível cancelar o pedido.")
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <Button asChild variant="ghost" size="sm" className="self-start">
        <Link to="/orders">
          <ArrowLeft />
          Voltar
        </Link>
      </Button>

      {isPending && <p className="text-muted-foreground text-sm">Carregando…</p>}
      {isError && <p className="text-destructive text-sm">Pedido não encontrado.</p>}

      {order && (
        <Card>
          <CardHeader>
            <div className="flex items-center justify-between gap-4">
              <div className="flex flex-col gap-1">
                <CardTitle>Pedido</CardTitle>
                <CardDescription>{order.customerName ?? "Cliente"}</CardDescription>
              </div>
              <OrderStatusBadge status={order.status} />
            </div>
          </CardHeader>
          <CardContent className="flex flex-col gap-6">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Produto</TableHead>
                  <TableHead>Qtd.</TableHead>
                  <TableHead>Preço unit.</TableHead>
                  <TableHead className="text-right">Subtotal</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {order.items.map((item) => (
                  <TableRow key={item.id}>
                    <TableCell className="font-medium">{item.productName}</TableCell>
                    <TableCell>{item.quantity}</TableCell>
                    <TableCell>{formatCurrency(item.unitPrice)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(item.lineTotal)}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>

            <div className="flex items-center justify-end gap-2 text-lg font-semibold">
              <span className="text-muted-foreground text-sm font-normal">Total</span>
              {formatCurrency(order.total)}
            </div>

            <div className="flex flex-wrap gap-2">
              {order.status === "Pending" && (
                <Button disabled={busy} onClick={() => handleChangeStatus("Confirmed")}>
                  Confirmar
                </Button>
              )}
              {order.status === "Confirmed" && (
                <Button disabled={busy} onClick={() => handleChangeStatus("Completed")}>
                  Concluir
                </Button>
              )}
              {order.status !== "Cancelled" && order.status !== "Completed" && (
                <Button variant="destructive" disabled={busy} onClick={handleCancel}>
                  Cancelar pedido
                </Button>
              )}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
