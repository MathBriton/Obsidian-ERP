export interface HealthCheck {
  name: string
  status: string
}

export interface HealthStatus {
  status: string
  timestamp: string
  checks: HealthCheck[]
}
