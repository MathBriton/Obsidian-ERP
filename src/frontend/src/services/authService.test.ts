import { afterEach, describe, expect, it, vi } from "vitest"

import { login } from "@/services/authService"

afterEach(() => {
  vi.unstubAllGlobals()
  localStorage.clear()
})

describe("authService", () => {
  it("login faz POST em /api/auth/login e retorna a resposta", async () => {
    const payload = {
      accessToken: "access",
      refreshToken: "refresh",
      expiresAt: "2026-01-01T00:00:00Z",
      user: { id: "1", name: "Ana", email: "ana@x.com" },
    }
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => payload,
    })
    vi.stubGlobal("fetch", fetchMock)

    const result = await login({ email: "ana@x.com", password: "senha123" })

    expect(result.accessToken).toBe("access")
    expect(result.user.email).toBe("ana@x.com")
    expect(fetchMock).toHaveBeenCalledWith(
      "/api/auth/login",
      expect.objectContaining({ method: "POST" }),
    )
  })

  it("lança erro quando a API responde com falha", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({ ok: false, status: 401, json: async () => ({}) }),
    )

    await expect(login({ email: "x@x.com", password: "errada" })).rejects.toThrow()
  })
})
