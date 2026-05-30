import { Component, type ErrorInfo, type ReactNode } from "react"

import { Button } from "@/components/ui/button"

interface ErrorBoundaryProps {
  children: ReactNode
  fallback?: ReactNode
}

interface ErrorBoundaryState {
  hasError: boolean
}

/**
 * Captura erros de renderização em qualquer parte da árvore de componentes e
 * exibe uma UI de fallback, evitando que um erro derrube a aplicação inteira.
 */
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  state: ErrorBoundaryState = { hasError: false }

  static getDerivedStateFromError(): ErrorBoundaryState {
    return { hasError: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo): void {
    console.error("ErrorBoundary capturou um erro de renderização:", error, info)
  }

  private readonly handleReset = (): void => {
    this.setState({ hasError: false })
  }

  render(): ReactNode {
    if (!this.state.hasError) {
      return this.props.children
    }

    if (this.props.fallback) {
      return this.props.fallback
    }

    return (
      <div
        role="alert"
        className="flex min-h-screen flex-col items-center justify-center gap-4 p-6 text-center"
      >
        <h1 className="text-2xl font-semibold">Algo deu errado</h1>
        <p className="text-muted-foreground max-w-md">
          Ocorreu um erro inesperado na aplicação. Você pode tentar novamente ou recarregar a
          página.
        </p>
        <div className="flex gap-2">
          <Button onClick={this.handleReset}>Tentar novamente</Button>
          <Button variant="outline" onClick={() => window.location.reload()}>
            Recarregar página
          </Button>
        </div>
      </div>
    )
  }
}
