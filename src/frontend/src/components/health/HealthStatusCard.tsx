import { RefreshCw } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { cn } from "@/lib/utils"
import { useHealth } from "@/hooks/useHealth"

export function HealthStatusCard() {
  const { data, isPending, isError, isFetching, refetch } = useHealth()
  const isHealthy = data?.status === "Healthy"

  return (
    <Card className="w-full max-w-md">
      <CardHeader>
        <CardTitle>Status da API</CardTitle>
        <CardDescription>Conexão com o backend Obsidian ERP</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
        {isPending && <p className="text-muted-foreground text-sm">Verificando…</p>}
        {isError && <p className="text-destructive text-sm">Não foi possível conectar à API.</p>}
        {data && (
          <div className="flex items-center gap-2">
            <span
              className={cn("size-2.5 rounded-full", isHealthy ? "bg-success" : "bg-destructive")}
              aria-hidden
            />
            <span className="font-medium">{data.status}</span>
          </div>
        )}
      </CardContent>
      <CardFooter>
        <Button variant="outline" size="sm" onClick={() => refetch()} disabled={isFetching}>
          <RefreshCw className={cn(isFetching && "animate-spin")} />
          {isFetching ? "Atualizando…" : "Atualizar"}
        </Button>
      </CardFooter>
    </Card>
  )
}
