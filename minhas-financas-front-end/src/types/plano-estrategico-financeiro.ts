export enum PrioridadeObjetivoPlanoEstrategico {
  Baixa = 0,
  Media = 1,
  Alta = 2,
  Critica = 3,
}

export enum StatusObjetivoPlanoEstrategico {
  Planejado = 0,
  EmAndamento = 1,
  Concluido = 2,
  Cancelado = 3,
}

export interface ObjetivoPlanoEstrategicoPayload {
  id?: string | null;
  titulo: string;
  descricao?: string | null;
  prioridade: PrioridadeObjetivoPlanoEstrategico;
  status: StatusObjetivoPlanoEstrategico;
  ordem: number;
  dataAlvo?: string | null;
  valorAlvo?: number | null;
  valorAtual?: number | null;
  observacao?: string | null;
}

export interface SalvarPlanoEstrategicoFinanceiroPayload {
  nome: string;
  descricao?: string | null;
  observacao?: string | null;
  dataInicioVigencia?: string | null;
  objetivos: ObjetivoPlanoEstrategicoPayload[];
}

export interface ObjetivoPlanoEstrategico {
  id?: string | null;
  titulo: string;
  descricao?: string | null;
  prioridade: PrioridadeObjetivoPlanoEstrategico;
  status: StatusObjetivoPlanoEstrategico;
  ordem: number;
  dataAlvo?: string | null;
  valorAlvo?: number | null;
  valorAtual?: number | null;
  observacao?: string | null;
}

export interface PlanoEstrategicoFinanceiroResumo {
  id: string;
  planoRaizId: string;
  nome: string;
  descricao?: string | null;
  numeroVersao: number;
  dataInicioVigencia: string;
  dataFimVigencia?: string | null;
  dataCadastro: string;
  dataAtualizacao: string;
  ativo: boolean;
  quantidadeObjetivos: number;
}

export interface PlanoEstrategicoFinanceiroDetalhe {
  id: string;
  planoRaizId: string;
  nome: string;
  descricao?: string | null;
  observacao?: string | null;
  numeroVersao: number;
  dataInicioVigencia: string;
  dataFimVigencia?: string | null;
  dataCadastro: string;
  dataAtualizacao: string;
  ativo: boolean;
  objetivos: ObjetivoPlanoEstrategico[];
}
