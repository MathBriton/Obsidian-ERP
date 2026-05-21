import type { HealthStatus } from "@/types/health"

export async function getHealth(): Promise<HealthStatus> {
  const response = await fetch("/health")

  if (!response.ok) {
    throw new Error(`Falha ao consultar /health (HTTP ${response.status})`)
  }

  return (await response.json()) as HealthStatus
}
