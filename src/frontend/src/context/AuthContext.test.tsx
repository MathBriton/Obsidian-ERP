import { afterEach, describe, expect, it, vi } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"

import { AuthProvider, useAuth } from "@/context/AuthContext"

function Consumer() {
  const { user, isAuthenticated, login, logout } = useAuth()
  return (
    <div>
      <span data-testid="status">{isAuthenticated ? "in" : "out"}</span>
      <span data-testid="email">{user?.email ?? ""}</span>
      <button onClick={() => login({ email: "ana@x.com", password: "senha123" })}>
        entrar
      </button>
      <button onClick={logout}>sair</button>
    </div>
  )
}

const payload = {
  accessToken: "access",
  refreshToken: "refresh",
  expiresAt: "2026-01-01T00:00:00Z",
  user: { id: "1", name: "Ana", email: "ana@x.com" },
}

afterEach(() => {
  vi.unstubAllGlobals()
  localStorage.clear()
})

describe("AuthContext", () => {
  it("login autentica e persiste a sessão", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => payload })
    )

    render(
      <AuthProvider>
        <Consumer />
      </AuthProvider>
    )
    expect(screen.getByTestId("status").textContent).toBe("out")

    await userEvent.click(screen.getByText("entrar"))

    await waitFor(() => expect(screen.getByTestId("status").textContent).toBe("in"))
    expect(screen.getByTestId("email").textContent).toBe("ana@x.com")
    expect(localStorage.getItem("obsidian.accessToken")).toBe("access")
  })

  it("restaura a sessão do localStorage ao montar", () => {
    localStorage.setItem("obsidian.accessToken", "access")
    localStorage.setItem(
      "obsidian.user",
      JSON.stringify({ id: "1", name: "Ana", email: "ana@x.com" })
    )

    render(
      <AuthProvider>
        <Consumer />
      </AuthProvider>
    )

    expect(screen.getByTestId("status").textContent).toBe("in")
    expect(screen.getByTestId("email").textContent).toBe("ana@x.com")
  })

  it("logout limpa a sessão", async () => {
    localStorage.setItem("obsidian.accessToken", "access")
    localStorage.setItem(
      "obsidian.user",
      JSON.stringify({ id: "1", name: "Ana", email: "ana@x.com" })
    )

    render(
      <AuthProvider>
        <Consumer />
      </AuthProvider>
    )
    await userEvent.click(screen.getByText("sair"))

    expect(screen.getByTestId("status").textContent).toBe("out")
    expect(localStorage.getItem("obsidian.accessToken")).toBeNull()
  })
})
