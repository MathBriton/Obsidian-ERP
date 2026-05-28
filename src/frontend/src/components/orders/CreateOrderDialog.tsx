import { useState, type FormEvent } from "react"
import { Plus, Trash2 } from "lucide-react"
import { toast } from "sonner"

import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useCustomers } from "@/hooks/useCustomers"
import { useCreateOrder } from "@/hooks/useOrders"

interface CreateOrderDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function CreateOrderDialog({ open, onOpenChange }: CreateOrderDialogProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Novo pedido</DialogTitle>
          <DialogDescription>Selecione o cliente e adicione os itens.</DialogDescription>
        </DialogHeader>
        <CreateOrderForm key={open ? "open" : "closed"} onSaved={() => onOpenChange(false)} />
      </DialogContent>
    </Dialog>
  )
}

interface ItemRow {
  productName: string
  quantity: string
  unitPrice: string
}

function emptyRow(): ItemRow {
  return { productName: "", quantity: "1", unitPrice: "0" }
}

function CreateOrderForm({ onSaved }: { onSaved: () => void }) {
  const { data: customersData } = useCustomers({ page: 1, pageSize: 100 })
  const createMutation = useCreateOrder()

  const [customerId, setCustomerId] = useState("")
  const [items, setItems] = useState<ItemRow[]>([emptyRow()])

  function updateItem(index: number, field: keyof ItemRow, value: string) {
    setItems((current) =>
      current.map((item, i) => (i === index ? { ...item, [field]: value } : item))
    )
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()

    if (!customerId) {
      toast.error("Selecione um cliente.")
      return
    }

    const payloadItems = items
      .filter((item) => item.productName.trim())
      .map((item) => ({
        productName: item.productName.trim(),
        quantity: Number(item.quantity) || 0,
        unitPrice: Number(item.unitPrice) || 0,
      }))

    if (payloadItems.length === 0) {
      toast.error("Adicione ao menos um item.")
      return
    }

    try {
      await createMutation.mutateAsync({ customerId, items: payloadItems })
      toast.success("Pedido criado.")
      onSaved()
    } catch {
      toast.error("Não foi possível criar o pedido.")
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div className="flex flex-col gap-2">
        <Label htmlFor="order-customer">Cliente</Label>
        <select
          id="order-customer"
          value={customerId}
          onChange={(event) => setCustomerId(event.target.value)}
          className="border-input bg-transparent focus-visible:border-ring focus-visible:ring-ring/50 h-9 w-full rounded-md border px-3 text-sm shadow-xs outline-none focus-visible:ring-[3px]"
          required
        >
          <option value="" disabled>
            Selecione…
          </option>
          {customersData?.items.map((customer) => (
            <option key={customer.id} value={customer.id}>
              {customer.name}
            </option>
          ))}
        </select>
      </div>

      <div className="flex flex-col gap-2">
        <Label>Itens</Label>
        {items.map((item, index) => (
          <div key={index} className="flex items-end gap-2">
            <div className="flex-1">
              <Input
                placeholder="Produto"
                value={item.productName}
                onChange={(event) => updateItem(index, "productName", event.target.value)}
              />
            </div>
            <div className="w-20">
              <Input
                type="number"
                min="1"
                placeholder="Qtd"
                value={item.quantity}
                onChange={(event) => updateItem(index, "quantity", event.target.value)}
              />
            </div>
            <div className="w-28">
              <Input
                type="number"
                min="0"
                step="0.01"
                placeholder="Preço"
                value={item.unitPrice}
                onChange={(event) => updateItem(index, "unitPrice", event.target.value)}
              />
            </div>
            <Button
              type="button"
              variant="ghost"
              size="icon"
              onClick={() => setItems((current) => current.filter((_, i) => i !== index))}
              disabled={items.length === 1}
              aria-label="Remover item"
            >
              <Trash2 />
            </Button>
          </div>
        ))}
        <Button
          type="button"
          variant="outline"
          size="sm"
          className="self-start"
          onClick={() => setItems((current) => [...current, emptyRow()])}
        >
          <Plus />
          Adicionar item
        </Button>
      </div>

      <DialogFooter>
        <Button type="submit" disabled={createMutation.isPending}>
          {createMutation.isPending ? "Criando…" : "Criar pedido"}
        </Button>
      </DialogFooter>
    </form>
  )
}
