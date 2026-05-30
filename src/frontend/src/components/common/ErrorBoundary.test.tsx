import { describe, expect, it, vi } from "vitest"
import { render, screen } from "@testing-library/react"

import { ErrorBoundary } from "@/components/common/ErrorBoundary"

function Bomb(): never {
  throw new Error("boom")
}

describe("ErrorBoundary", () => {
  it("renderiza os filhos quando não há erro", () => {
    render(
      <ErrorBoundary>
        <p>conteúdo normal</p>
      </ErrorBoundary>
    )

    expect(screen.getByText("conteúdo normal")).toBeInTheDocument()
  })

  it("exibe o fallback quando um filho lança erro", () => {
    // Silencia o console.error esperado do React ao capturar o erro.
    const spy = vi.spyOn(console, "error").mockImplementation(() => {})

    render(
      <ErrorBoundary>
        <Bomb />
      </ErrorBoundary>
    )

    expect(screen.getByRole("alert")).toBeInTheDocument()
    expect(screen.getByText(/algo deu errado/i)).toBeInTheDocument()

    spy.mockRestore()
  })
})
