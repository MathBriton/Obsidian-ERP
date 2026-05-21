import { Outlet } from "react-router-dom"

export function AppLayout() {
  return (
    <div className="bg-background text-foreground min-h-svh">
      <header className="border-b">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-4">
          <span className="text-lg font-semibold tracking-tight">
            Obsidian ERP
          </span>
        </div>
      </header>
      <main className="mx-auto max-w-5xl px-6 py-10">
        <Outlet />
      </main>
    </div>
  )
}
