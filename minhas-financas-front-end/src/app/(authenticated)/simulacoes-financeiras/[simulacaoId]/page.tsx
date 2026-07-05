import { SimulacaoFinanceiraManager } from "@/components/simulacao-financeira/SimulacaoFinanceiraManager";

interface SimulacaoFinanceiraDetalhePageProps {
  params: {
    simulacaoId: string;
  };
}

export default function SimulacaoFinanceiraDetalhePage({
  params,
}: SimulacaoFinanceiraDetalhePageProps) {
  return <SimulacaoFinanceiraManager simulacaoId={params.simulacaoId} />;
}
