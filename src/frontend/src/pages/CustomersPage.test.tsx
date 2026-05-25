import type { ReactNode } from "react"
import { afterEach, describe, expect, it, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"

import { CustomersPage } from "@/pages/CustomersPage"

vi.mock("@/services/customerService", () => ({
  listCustomers: vi.fn().mockResolvedValue({
    items: [
      {
        id: "1",
        name: "Acme Ltda",
        email: "acme@x.com",
        phone: null,
        document: null,
        createdAt: "2026-01-01T00:00:00Z",
      },
    ],
    page: 1,
    pageSize: 10,
    totalCount: 1,
    totalPages: 1,
  }),
  createCustomer: vi.fn(),
  updateCustomer: vi.fn(),
  deleteCustomer: vi.fn(),
}))

function renderWithClient(ui: ReactNode) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>)
}

afterEach(() => {
  localStorage.clear()
})

describe("CustomersPage", () => {
  it("exibe os clientes retornados pelo serviço", async () => {
    renderWithClient(<CustomersPage />)

    expect(await screen.findByText("Acme Ltda")).toBeInTheDocument()
    expect(screen.getByText("acme@x.com")).toBeInTheDocument()
  })
})
