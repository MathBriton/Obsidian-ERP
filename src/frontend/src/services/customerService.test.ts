import { afterEach, describe, expect, it, vi } from "vitest"

import { createCustomer, listCustomers } from "@/services/customerService"

afterEach(() => {
  vi.unstubAllGlobals()
  localStorage.clear()
})

describe("customerService", () => {
  it("listCustomers monta a query string e retorna o resultado paginado", async () => {
    const paged = { items: [], page: 2, pageSize: 5, totalCount: 0, totalPages: 0 }
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => paged,
    })
    vi.stubGlobal("fetch", fetchMock)

    const result = await listCustomers({ page: 2, pageSize: 5, search: "ac" })

    expect(result.page).toBe(2)
    const calledUrl = fetchMock.mock.calls[0][0] as string
    expect(calledUrl).toContain("/api/customers?")
    expect(calledUrl).toContain("page=2")
    expect(calledUrl).toContain("search=ac")
  })

  it("createCustomer faz POST em /api/customers", async () => {
    const created = {
      id: "1",
      name: "Acme",
      email: null,
      phone: null,
      document: null,
      createdAt: "2026-01-01T00:00:00Z",
    }
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => created,
    })
    vi.stubGlobal("fetch", fetchMock)

    const result = await createCustomer({ name: "Acme" })

    expect(result.name).toBe("Acme")
    expect(fetchMock).toHaveBeenCalledWith(
      "/api/customers",
      expect.objectContaining({ method: "POST" }),
    )
  })
})
