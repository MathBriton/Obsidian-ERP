import type { ReactNode } from "react"
import { afterEach, describe, expect, it, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"

import { OrdersPage } from "@/pages/OrdersPage"

vi.mock("@/services/orderService", () => ({
  listOrders: vi.fn().mockResolvedValue({
    items: [
      {
        id: "1",
        customerId: "c1",
        customerName: "Acme Ltda",
        status: "Pending",
        total: 25,
        createdAt: "2026-01-01T00:00:00Z",
      },
    ],
    page: 1,
    pageSize: 10,
    totalCount: 1,
    totalPages: 1,
  }),
  getOrder: vi.fn(),
  createOrder: vi.fn(),
  cancelOrder: vi.fn(),
  changeOrderStatus: vi.fn(),
}))

function renderPage(ui: ReactNode) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>{ui}</MemoryRouter>
    </QueryClientProvider>
  )
}

afterEach(() => {
  localStorage.clear()
})

describe("OrdersPage", () => {
  it("lista os pedidos com o status traduzido", async () => {
    renderPage(<OrdersPage />)

    expect(await screen.findByText("Acme Ltda")).toBeInTheDocument()
    // "Pendente" aparece no filtro e no badge da linha
    expect(screen.getAllByText("Pendente").length).toBeGreaterThan(1)
  })
})
