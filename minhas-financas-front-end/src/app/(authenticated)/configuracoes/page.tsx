"use client";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { CategoriasManager } from "@/components/configuracoes/CategoriasManager";

export default function Configuracoes() {
  return (
    <div className="flex flex-row">
      <Sidebar />
      <CategoriasManager />
    </div>
  );
}
