import { getAccessToken } from "@/lib/authStorage"

/** Corpo de erro padronizado pela API (RFC 7807 / ProblemDetails). */
export interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  readonly status: number
  readonly problem?: ProblemDetails

  constructor(status: number, message: string, problem?: ProblemDetails) {
    super(message)
    this.name = "ApiError"
    this.status = status
    this.problem = problem
  }
}

/** Extrai uma mensagem amigável do corpo ProblemDetails, com fallback genérico. */
async function toApiError(response: Response): Promise<ApiError> {
  let message = `Requisição falhou (HTTP ${response.status})`
  let problem: ProblemDetails | undefined

  try {
    const contentType = response.headers?.get("content-type") ?? ""
    if (contentType.includes("json")) {
      problem = (await response.json()) as ProblemDetails

      const fieldErrors = problem?.errors ? Object.values(problem.errors).flat() : []

      if (fieldErrors.length > 0) {
        message = fieldErrors.join(" ")
      } else if (problem?.detail) {
        message = problem.detail
      } else if (problem?.title) {
        message = problem.title
      }
    }
  } catch {
    // Corpo ausente ou ilegível — mantém a mensagem genérica.
  }

  return new ApiError(response.status, message, problem)
}

/** Mensagem legível para qualquer erro (ApiError, Error ou desconhecido). */
export function getErrorMessage(
  error: unknown,
  fallback = "Algo deu errado. Tente novamente.",
): string {
  if (error instanceof ApiError) {
    return error.message
  }
  if (error instanceof Error && error.message) {
    return error.message
  }
  return fallback
}

export async function apiFetch<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers)
  headers.set("Content-Type", "application/json")

  const token = getAccessToken()
  if (token) {
    headers.set("Authorization", `Bearer ${token}`)
  }

  const response = await fetch(path, { ...init, headers })

  if (!response.ok) {
    throw await toApiError(response)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}
