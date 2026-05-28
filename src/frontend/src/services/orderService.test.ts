import { afterEach, describe, expect, it, vi } from "vitest"

import { changeOrderStatus, listOrders } from "@/services/orderService"

afterEach(() => {
  vi.unstubAllGlobals()
  localStorage.clear()
})

describe("orderService", () => {
  it("listOrders monta a query string com status e paginação", async () => {
    const paged = { items: [], page: 1, pageSize: 10, totalCount: 0, totalPages: 0 }
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => paged })
    vi.stubGlobal("fetch", fetchMock)

    await listOrders({ page: 2, pageSize: 5, status: "Cancelled" })

    const url = fetchMock.mock.calls[0][0] as string
    expect(url).toContain("/api/orders?")
    expect(url).toContain("page=2")
    expect(url).toContain("status=Cancelled")
  })

  it("changeOrderStatus faz PUT com o status", async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => ({ id: "1", status: "Confirmed" }),
    })
    vi.stubGlobal("fetch", fetchMock)

    await changeOrderStatus("1", "Confirmed")

    const [url, init] = fetchMock.mock.calls[0]
    expect(url).toBe("/api/orders/1/status")
    expect((init as RequestInit).method).toBe("PUT")
  })
})
