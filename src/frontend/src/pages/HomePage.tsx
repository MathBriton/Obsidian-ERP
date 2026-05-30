import { HealthStatusCard } from "@/components/health/HealthStatusCard"

export function HomePage() {
  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col gap-1">
        <h1 className="text-2xl font-bold tracking-tight">Bem-vindo ao Obsidian ERP</h1>
        <p className="text-muted-foreground">Painel inicial do sistema.</p>
      </div>
      <HealthStatusCard />
    </div>
  )
}
