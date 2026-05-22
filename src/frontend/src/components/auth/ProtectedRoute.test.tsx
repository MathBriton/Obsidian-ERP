import { afterEach, describe, expect, it } from "vitest"
import { render, screen } from "@testing-library/react"
import { MemoryRouter, Route, Routes } from "react-router-dom"

import { AuthProvider } from "@/context/AuthContext"
import { ProtectedRoute } from "@/components/auth/ProtectedRoute"

function renderAt(path: string) {
  return render(
    <AuthProvider>
      <MemoryRouter initialEntries={[path]}>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route path="/" element={<div>conteúdo protegido</div>} />
          </Route>
          <Route path="/login" element={<div>página de login</div>} />
        </Routes>
      </MemoryRouter>
    </AuthProvider>
  )
}

afterEach(() => localStorage.clear())

describe("ProtectedRoute", () => {
  it("redireciona para /login quando não autenticado", () => {
    renderAt("/")
    expect(screen.getByText("página de login")).toBeInTheDocument()
  })

  it("renderiza o conteúdo quando autenticado", () => {
    localStorage.setItem("obsidian.accessToken", "access")
    localStorage.setItem(
      "obsidian.user",
      JSON.stringify({ id: "1", name: "Ana", email: "ana@x.com" })
    )
    renderAt("/")
    expect(screen.getByText("conteúdo protegido")).toBeInTheDocument()
  })
})
