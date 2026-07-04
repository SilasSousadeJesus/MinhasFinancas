"use client";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { ProjecaoManager } from "@/components/projecao/ProjecaoManager";

interface ProjecaoDetalhePageProps {
  params: {
    projecaoId: string;
  };
}

export default function ProjecaoDetalhePage({ params }: ProjecaoDetalhePageProps) {
  return (
    <div className="flex flex-row">
      <Sidebar />
      <ProjecaoManager projecaoId={params.projecaoId} />
    </div>
  );
}
