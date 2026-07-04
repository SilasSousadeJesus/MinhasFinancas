"use client";

import { LoaderCircle } from "lucide-react";

interface GlobalLoadingProps {
  visible: boolean;
  message?: string | null;
}

export function GlobalLoading({ visible, message }: GlobalLoadingProps) {
  if (!visible) {
    return null;
  }

  return (
    <div className="pointer-events-none fixed inset-0 z-[100] flex items-center justify-center bg-slate-950/14 backdrop-blur-[2px] dark:bg-slate-950/34">
      <div className="flex min-w-44 items-center gap-3 rounded-2xl border border-slate-200/70 bg-white/88 px-5 py-4 shadow-2xl shadow-slate-950/10 dark:border-slate-800/80 dark:bg-slate-900/84">
        <LoaderCircle className="h-6 w-6 animate-spin text-slate-900 dark:text-slate-100" />
        <div className="space-y-1">
          <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">
            Carregando
          </p>
          <p className="text-xs text-slate-600 dark:text-slate-300">
            {message || "Aguarde enquanto buscamos os dados mais recentes."}
          </p>
        </div>
      </div>
    </div>
  );
}

