import { afterEach, describe, expect, it, vi } from "vitest"

vi.mock("sonner", () => ({ toast: { error: vi.fn() } }))

import { toast } from "sonner"

import { ApiError } from "@/lib/apiClient"
import { createQueryClient } from "@/lib/queryClient"

afterEach(() => {
  vi.clearAllMocks()
})

describe("createQueryClient", () => {
  it("dispara um toast com a mensagem da API quando uma query falha", async () => {
    const client = createQueryClient()

    await client
      .fetchQuery({
        queryKey: ["falha"],
        queryFn: () => Promise.reject(new ApiError(500, "Falha ao carregar")),
        retry: false,
      })
      .catch(() => {})

    expect(toast.error).toHaveBeenCalledWith("Falha ao carregar")
  })
})
