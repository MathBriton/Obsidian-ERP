import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import { QueryClientProvider } from "@tanstack/react-query"
import { RouterProvider } from "react-router-dom"
import { Toaster } from "sonner"

import { ErrorBoundary } from "@/components/common/ErrorBoundary"
import { AuthProvider } from "@/context/AuthContext"
import { createQueryClient } from "@/lib/queryClient"
import { router } from "@/routes/router"
import "./index.css"

const queryClient = createQueryClient()

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ErrorBoundary>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <RouterProvider router={router} />
        </AuthProvider>
        <Toaster richColors position="top-right" />
      </QueryClientProvider>
    </ErrorBoundary>
  </StrictMode>
)
