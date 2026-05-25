import { useState } from "react"
import { Pencil, Plus, Trash2 } from "lucide-react"

import { CustomerFormDialog } from "@/components/customers/CustomerFormDialog"
import { DeleteCustomerDialog } from "@/components/customers/DeleteCustomerDialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { useCustomers } from "@/hooks/useCustomers"
import type { Customer } from "@/types/customer"

const PAGE_SIZE = 10

export function CustomersPage() {
  const [search, setSearch] = useState("")
  const [page, setPage] = useState(1)
  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<Customer | null>(null)
  const [deleting, setDeleting] = useState<Customer | null>(null)

  const { data, isPending, isError } = useCustomers({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
  })

  const totalPages = data?.totalPages ?? 0

  function openCreate() {
    setEditing(null)
    setFormOpen(true)
  }

  function openEdit(customer: Customer) {
    setEditing(customer)
    setFormOpen(true)
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-bold tracking-tight">Clientes</h1>
          <p className="text-muted-foreground">Gerencie os clientes do sistema.</p>
        </div>
        <Button onClick={openCreate}>
          <Plus />
          Novo cliente
        </Button>
      </div>

      <Input
        placeholder="Buscar por nome, e-mail ou documento…"
        value={search}
        onChange={(event) => {
          setSearch(event.target.value)
          setPage(1)
        }}
        className="max-w-sm"
      />

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nome</TableHead>
              <TableHead>E-mail</TableHead>
              <TableHead>Telefone</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isPending && (
              <TableRow>
                <TableCell colSpan={4} className="text-muted-foreground text-center">
                  Carregando…
                </TableCell>
              </TableRow>
            )}
            {isError && (
              <TableRow>
                <TableCell colSpan={4} className="text-destructive text-center">
                  Erro ao carregar clientes.
                </TableCell>
              </TableRow>
            )}
            {data && data.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} className="text-muted-foreground text-center">
                  Nenhum cliente encontrado.
                </TableCell>
              </TableRow>
            )}
            {data?.items.map((customer) => (
              <TableRow key={customer.id}>
                <TableCell className="font-medium">{customer.name}</TableCell>
                <TableCell>{customer.email ?? "—"}</TableCell>
                <TableCell>{customer.phone ?? "—"}</TableCell>
                <TableCell>
                  <div className="flex justify-end gap-2">
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => openEdit(customer)}
                      aria-label="Editar"
                    >
                      <Pencil />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => setDeleting(customer)}
                      aria-label="Excluir"
                    >
                      <Trash2 />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {data && totalPages > 1 && (
        <div className="flex items-center justify-between">
          <span className="text-muted-foreground text-sm">
            Página {data.page} de {totalPages} · {data.totalCount} clientes
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

      <CustomerFormDialog open={formOpen} onOpenChange={setFormOpen} customer={editing} />
      <DeleteCustomerDialog
        customer={deleting}
        onOpenChange={(open) => {
          if (!open) {
            setDeleting(null)
          }
        }}
      />
    </div>
  )
}
