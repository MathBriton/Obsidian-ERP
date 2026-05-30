import { useState, type FormEvent } from "react"
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
import { getErrorMessage } from "@/lib/apiClient"
import { useCreateCustomer, useUpdateCustomer } from "@/hooks/useCustomers"
import type { Customer } from "@/types/customer"

interface CustomerFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  customer: Customer | null
}

export function CustomerFormDialog({ open, onOpenChange, customer }: CustomerFormDialogProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{customer ? "Editar cliente" : "Novo cliente"}</DialogTitle>
          <DialogDescription>Preencha os dados do cliente.</DialogDescription>
        </DialogHeader>
        {/* key força um form com estado novo a cada abertura/cliente (sem useEffect) */}
        <CustomerForm
          key={customer?.id ?? "new"}
          customer={customer}
          onSaved={() => onOpenChange(false)}
        />
      </DialogContent>
    </Dialog>
  )
}

interface CustomerFormProps {
  customer: Customer | null
  onSaved: () => void
}

function CustomerForm({ customer, onSaved }: CustomerFormProps) {
  const isEdit = customer !== null
  const createMutation = useCreateCustomer()
  const updateMutation = useUpdateCustomer()

  const [name, setName] = useState(customer?.name ?? "")
  const [email, setEmail] = useState(customer?.email ?? "")
  const [phone, setPhone] = useState(customer?.phone ?? "")
  const [document, setDocument] = useState(customer?.document ?? "")

  const submitting = createMutation.isPending || updateMutation.isPending

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    const body = {
      name,
      email: email || null,
      phone: phone || null,
      document: document || null,
    }

    try {
      if (isEdit && customer) {
        await updateMutation.mutateAsync({ id: customer.id, body })
        toast.success("Cliente atualizado.")
      } else {
        await createMutation.mutateAsync(body)
        toast.success("Cliente criado.")
      }
      onSaved()
    } catch (error) {
      toast.error(getErrorMessage(error, "Não foi possível salvar o cliente."))
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div className="flex flex-col gap-2">
        <Label htmlFor="customer-name">Nome</Label>
        <Input
          id="customer-name"
          value={name}
          onChange={(event) => setName(event.target.value)}
          required
        />
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="customer-email">E-mail</Label>
        <Input
          id="customer-email"
          type="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
        />
      </div>
      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-2">
          <Label htmlFor="customer-phone">Telefone</Label>
          <Input
            id="customer-phone"
            value={phone}
            onChange={(event) => setPhone(event.target.value)}
          />
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="customer-document">Documento</Label>
          <Input
            id="customer-document"
            value={document}
            onChange={(event) => setDocument(event.target.value)}
          />
        </div>
      </div>
      <DialogFooter>
        <Button type="submit" disabled={submitting}>
          {submitting ? "Salvando…" : "Salvar"}
        </Button>
      </DialogFooter>
    </form>
  )
}
