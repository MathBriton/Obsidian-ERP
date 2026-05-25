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
import { useDeleteCustomer } from "@/hooks/useCustomers"
import type { Customer } from "@/types/customer"

interface DeleteCustomerDialogProps {
  customer: Customer | null
  onOpenChange: (open: boolean) => void
}

export function DeleteCustomerDialog({ customer, onOpenChange }: DeleteCustomerDialogProps) {
  const deleteMutation = useDeleteCustomer()

  async function handleDelete() {
    if (!customer) {
      return
    }

    try {
      await deleteMutation.mutateAsync(customer.id)
      toast.success("Cliente excluído.")
      onOpenChange(false)
    } catch {
      toast.error("Não foi possível excluir o cliente.")
    }
  }

  return (
    <Dialog open={customer !== null} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Excluir cliente</DialogTitle>
          <DialogDescription>
            Tem certeza que deseja excluir {customer?.name}? Esta ação não pode ser desfeita.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button
            variant="destructive"
            onClick={handleDelete}
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending ? "Excluindo…" : "Excluir"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
