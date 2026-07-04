"use client";

import { Loader2 } from "lucide-react";

interface ProjecaoLoadingOverlayProps {
  visible: boolean;
  message?: string;
}

export function ProjecaoLoadingOverlay({
  visible,
  message = "Carregando projeção...",
}: ProjecaoLoadingOverlayProps) {
  if (!visible) {
    return null;
  }

  return (
    <div className="absolute inset-0 z-30 flex items-center justify-center rounded-3xl bg-background/75 backdrop-blur-sm">
      <div className="flex items-center gap-3 rounded-2xl border bg-card px-5 py-4 text-sm font-medium shadow-lg">
        <Loader2 className="h-4 w-4 animate-spin" />
        <span>{message}</span>
      </div>
    </div>
  );
}
