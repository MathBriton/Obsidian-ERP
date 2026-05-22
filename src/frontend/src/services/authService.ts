import { apiFetch } from "@/lib/apiClient"
import type { AuthResponse, LoginRequest, RegisterRequest, User } from "@/types/auth"

export function login(body: LoginRequest): Promise<AuthResponse> {
  return apiFetch<AuthResponse>("/api/auth/login", {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function register(body: RegisterRequest): Promise<AuthResponse> {
  return apiFetch<AuthResponse>("/api/auth/register", {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function getMe(): Promise<User> {
  return apiFetch<User>("/api/auth/me")
}
