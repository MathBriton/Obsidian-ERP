import { createContext, useCallback, useContext, useState, type ReactNode } from "react"

import { clearSession, getStoredUser, saveSession } from "@/lib/authStorage"
import * as authService from "@/services/authService"
import type { AuthResponse, LoginRequest, RegisterRequest, User } from "@/types/auth"

interface AuthContextValue {
  user: User | null
  isAuthenticated: boolean
  login: (body: LoginRequest) => Promise<void>
  register: (body: RegisterRequest) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(() => getStoredUser())

  const persist = useCallback((response: AuthResponse) => {
    saveSession(response.accessToken, response.refreshToken, response.user)
    setUser(response.user)
  }, [])

  const login = useCallback(
    async (body: LoginRequest) => {
      persist(await authService.login(body))
    },
    [persist]
  )

  const register = useCallback(
    async (body: RegisterRequest) => {
      persist(await authService.register(body))
    },
    [persist]
  )

  const logout = useCallback(() => {
    clearSession()
    setUser(null)
  }, [])

  return (
    <AuthContext.Provider
      value={{ user, isAuthenticated: user !== null, login, register, logout }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error("useAuth deve ser usado dentro de um AuthProvider")
  }
  return context
}
