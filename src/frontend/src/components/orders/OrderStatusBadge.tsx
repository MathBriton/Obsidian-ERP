import { cn } from "@/lib/utils"
import type { OrderStatus } from "@/types/order"

const STATUS_MAP: Record<OrderStatus, { label: string; className: string }> = {
  Pending: {
    label: "Pendente",
    className: "bg-amber-100 text-amber-800 dark:bg-amber-900/40 dark:text-amber-300",
  },
  Confirmed: {
    label: "Confirmado",
    className: "bg-blue-100 text-blue-800 dark:bg-blue-900/40 dark:text-blue-300",
  },
  Completed: {
    label: "Concluído",
    className: "bg-green-100 text-green-800 dark:bg-green-900/40 dark:text-green-300",
  },
  Cancelled: {
    label: "Cancelado",
    className: "bg-red-100 text-red-800 dark:bg-red-900/40 dark:text-red-300",
  },
}

export function OrderStatusBadge({ status }: { status: OrderStatus }) {
  const item = STATUS_MAP[status]
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium",
        item.className
      )}
    >
      {item.label}
    </span>
  )
}
