import { createBrowserRouter } from "react-router-dom"

import { ProtectedRoute } from "@/components/auth/ProtectedRoute"
import { AppLayout } from "@/layouts/AppLayout"
import { CustomersPage } from "@/pages/CustomersPage"
import { HomePage } from "@/pages/HomePage"
import { LoginPage } from "@/pages/LoginPage"
import { RegisterPage } from "@/pages/RegisterPage"

export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  { path: "/register", element: <RegisterPage /> },
  {
    element: <ProtectedRoute />,
    children: [
      {
        path: "/",
        element: <AppLayout />,
        children: [
          { index: true, element: <HomePage /> },
          { path: "customers", element: <CustomersPage /> },
        ],
      },
    ],
  },
])
