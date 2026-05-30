import { QueryCache, QueryClient } from "@tanstack/react-query"
import { toast } from "sonner"

import { getErrorMessage } from "@/lib/apiClient"

/**
 * Cria o QueryClient com tratamento global de erros de leitura (queries):
 * qualquer falha de busca dispara um toast com a mensagem vinda da API.
 * Erros de mutação continuam tratados pontualmente em cada tela.
 */
export function createQueryClient(): QueryClient {
  return new QueryClient({
    queryCache: new QueryCache({
      onError: (error) => {
        toast.error(getErrorMessage(error, "Não foi possível carregar os dados."))
      },
    }),
    defaultOptions: {
      queries: {
        retry: 1,
        refetchOnWindowFocus: false,
      },
    },
  })
}
