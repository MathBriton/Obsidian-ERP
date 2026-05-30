import { afterEach, describe, expect, it, vi } from "vitest"

import { getHealth } from "@/services/healthService"

afterEach(() => {
  vi.unstubAllGlobals()
})

describe("getHealth", () => {
  it("retorna o status quando a API responde 200", async () => {
    const payload = {
      status: "Healthy",
      timestamp: "2026-01-01T00:00:00Z",
      checks: [],
    }
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: true, json: async () => payload }))

    const result = await getHealth()

    expect(result.status).toBe("Healthy")
  })

  it("lanca erro quando a API responde com falha", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({ ok: false, status: 503, json: async () => ({}) }),
    )

    await expect(getHealth()).rejects.toThrow()
  })
})
