import { afterEach, describe, expect, it, vi } from "vitest"

import { ApiError, apiFetch, getErrorMessage } from "@/lib/apiClient"

afterEach(() => {
  vi.unstubAllGlobals()
})

function mockResponse(body: unknown, status = 400) {
  return {
    ok: false,
    status,
    headers: { get: () => "application/problem+json" },
    json: async () => body,
  }
}

async function captureError(promise: Promise<unknown>): Promise<ApiError> {
  try {
    await promise
    throw new Error("esperava que a promise fosse rejeitada")
  } catch (error) {
    return error as ApiError
  }
}

describe("apiFetch", () => {
  it("usa o 'detail' do ProblemDetails como mensagem do erro", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue(
        mockResponse({ title: "Conflito", detail: "O e-mail já está em uso.", status: 409 }, 409)
      )
    )

    const error = await captureError(apiFetch("/api/x"))

    expect(error).toBeInstanceOf(ApiError)
    expect(error.status).toBe(409)
    expect(error.message).toBe("O e-mail já está em uso.")
  })

  it("concatena os erros de validação (ValidationProblemDetails)", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue(
        mockResponse({
          title: "Erro de validação",
          errors: { Email: ["E-mail inválido."], Name: ["Obrigatório."] },
        })
      )
    )

    const error = await captureError(apiFetch("/api/x"))

    expect(error.message).toContain("E-mail inválido.")
    expect(error.message).toContain("Obrigatório.")
  })

  it("usa o 'title' quando não há detail nem errors", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue(mockResponse({ title: "Recurso não encontrado", status: 404 }, 404))
    )

    const error = await captureError(apiFetch("/api/x"))

    expect(error.message).toBe("Recurso não encontrado")
  })

  it("cai em mensagem genérica quando não há corpo legível", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({ ok: false, status: 500, json: async () => ({}) })
    )

    const error = await captureError(apiFetch("/api/x"))

    expect(error).toBeInstanceOf(ApiError)
    expect(error.status).toBe(500)
    expect(error.message).toMatch(/HTTP 500/)
  })
})

describe("getErrorMessage", () => {
  it("retorna a mensagem do ApiError", () => {
    expect(getErrorMessage(new ApiError(404, "Não encontrado"))).toBe("Não encontrado")
  })

  it("retorna o fallback para erros desconhecidos", () => {
    expect(getErrorMessage(null, "padrão")).toBe("padrão")
  })
})
