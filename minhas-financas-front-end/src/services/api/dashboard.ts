import { DashboardData } from "@/types/dashboard";
import { apiRequest } from "./http";

export function buscarDashboard(usuarioId: string, token: string) {
  return apiRequest<DashboardData>(`/Dashboard/${usuarioId}`, {
    method: "GET",
    token,
  });
}
