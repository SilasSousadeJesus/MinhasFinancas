"use client";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { FluxoCaixaSimplesManager } from "@/components/fluxo-caixa-simples/FluxoCaixaSimplesManager";

export default function FluxoCaixaSimplesPage() {
  return (
    <div className="flex flex-row">
      <Sidebar />
      <FluxoCaixaSimplesManager />
    </div>
  );
}
