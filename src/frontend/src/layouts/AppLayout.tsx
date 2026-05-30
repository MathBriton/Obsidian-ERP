import { NavLink, Outlet, useNavigate } from "react-router-dom"

import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { useAuth } from "@/context/AuthContext"

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    "text-sm font-medium transition-colors",
    isActive ? "text-foreground" : "text-muted-foreground hover:text-foreground",
  )

export function AppLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  function handleLogout() {
    logout()
    navigate("/login")
  }

  return (
    <div className="bg-background text-foreground min-h-svh">
      <header className="border-b">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-4">
          <div className="flex items-center gap-6">
            <span className="text-lg font-semibold tracking-tight">Obsidian ERP</span>
            <nav className="flex items-center gap-4">
              <NavLink to="/" end className={navLinkClass}>
                Início
              </NavLink>
              <NavLink to="/customers" className={navLinkClass}>
                Clientes
              </NavLink>
              <NavLink to="/orders" className={navLinkClass}>
                Pedidos
              </NavLink>
            </nav>
          </div>
          <div className="flex items-center gap-3">
            {user && <span className="text-muted-foreground text-sm">{user.email}</span>}
            <Button variant="outline" size="sm" onClick={handleLogout}>
              Sair
            </Button>
          </div>
        </div>
      </header>
      <main className="mx-auto max-w-5xl px-6 py-10">
        <Outlet />
      </main>
    </div>
  )
}
