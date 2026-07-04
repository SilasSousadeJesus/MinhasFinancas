"use client";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { ProjecoesOverview } from "@/components/projecao/ProjecoesOverview";

export default function Projecao() {
  return (
    <div className="flex flex-row">
      <Sidebar />
      <ProjecoesOverview />
    </div>
  );
}
