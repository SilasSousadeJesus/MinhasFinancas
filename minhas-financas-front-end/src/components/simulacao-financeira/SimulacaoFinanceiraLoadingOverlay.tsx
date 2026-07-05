"use client";

import { Loader2 } from "lucide-react";

interface SimulacaoFinanceiraLoadingOverlayProps {
  visible: boolean;
  message: string;
}

export function SimulacaoFinanceiraLoadingOverlay({
  visible,
  message,
}: SimulacaoFinanceiraLoadingOverlayProps) {
  if (!visible) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-background/70 backdrop-blur-sm">
      <div className="flex items-center gap-3 rounded-2xl border bg-card px-5 py-4 shadow-lg">
        <Loader2 className="h-5 w-5 animate-spin" />
        <span className="text-sm font-medium">{message}</span>
      </div>
    </div>
  );
}
