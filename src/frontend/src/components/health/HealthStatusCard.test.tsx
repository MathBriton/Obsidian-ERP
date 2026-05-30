import type { ReactNode } from "react"
import { afterEach, describe, expect, it, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"

import { HealthStatusCard } from "@/components/health/HealthStatusCard"

afterEach(() => {
  vi.unstubAllGlobals()
})

function renderWithClient(ui: ReactNode) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  })
  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>)
}

describe("HealthStatusCard", () => {
  it("exibe o status retornado pela API", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => ({
          status: "Healthy",
          timestamp: "2026-01-01T00:00:00Z",
          checks: [],
        }),
      }),
    )

    renderWithClient(<HealthStatusCard />)

    expect(await screen.findByText("Healthy")).toBeInTheDocument()
  })

  it("exibe mensagem de erro quando a API falha", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({ ok: false, status: 503, json: async () => ({}) }),
    )

    renderWithClient(<HealthStatusCard />)

    expect(await screen.findByText(/não foi possível conectar/i)).toBeInTheDocument()
  })
})
