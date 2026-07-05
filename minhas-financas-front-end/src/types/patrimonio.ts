export interface AtivoPatrimonialItem {
  id: string;
  nome: string;
  descricao: string;
  tipo: number;
  valorAtual: number;
  dataReferenciaValor: string | null;
  dataAquisicao: string | null;
  ativo: boolean;
}

export interface PassivoPatrimonialItem {
  id: string;
  nome: string;
  descricao: string;
  tipo: number;
  valorAtual: number;
  dataReferenciaValor: string | null;
  dataInicio: string | null;
  dataFim: string | null;
  ativo: boolean;
}

export interface SnapshotPatrimonialItem {
  id: string;
  dataReferencia: string;
  totalAtivos: number;
  totalPassivos: number;
  patrimonioLiquido: number;
  observacao: string;
  dataCriacao: string;
}

export interface LinhaEvolucaoPatrimonial {
  dataReferencia: string;
  totalAtivos: number;
  totalPassivos: number;
  patrimonioLiquido: number;
}

export interface ResumoPatrimonial {
  totalAtivos: number;
  totalPassivos: number;
  patrimonioLiquido: number;
  quantidadeAtivos: number;
  quantidadePassivos: number;
}

export interface VisaoGeralPatrimonio {
  resumo: ResumoPatrimonial;
  ativos: AtivoPatrimonialItem[];
  passivos: PassivoPatrimonialItem[];
  snapshots: SnapshotPatrimonialItem[];
  evolucao: LinhaEvolucaoPatrimonial[];
}

export interface AtivoPatrimonialPayload {
  nomeBemPatrimonial: string;
  descricao: string;
  tipo: number;
  valorAtual: number;
  dataAquisicao: string | null;
  usuarioId: string;
}

export interface PassivoPatrimonialPayload {
  nomeBemPatrimonial: string;
  descricao: string;
  tipo: number;
  valorAtual: number;
  dataInicio: string | null;
  dataFim: string | null;
  usuarioId: string;
}

export interface SnapshotPatrimonialPayload {
  dataReferencia: string;
  observacao: string;
}
