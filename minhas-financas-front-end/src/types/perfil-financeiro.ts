export interface ConfiguracaoPerfilFinanceiro {
  id: string;
  dataInicioVigencia: string;
  dataFimVigencia: string | null;
  percentualEconomiaMensalDesejado: number;
  percentualReservaEmergenciaDesejado: number;
  mesesReservaEmergenciaDesejados: number;
  percentualMaximoComprometimentoRenda: number;
  percentualMaximoEndividamento: number;
  percentualMinimoInvestimento: number;
  patrimonioLiquidoAlvo: number | null;
  observacao: string | null;
  origemPerfilFinanceiro: "PerfilInicialSistema" | "PersonalizadoPeloUsuario";
  vigente: boolean;
}

export interface VisaoGeralPerfilFinanceiro {
  perfilId: string | null;
  usaPerfilFinanceiroInicial: boolean;
  configuracaoVigente: ConfiguracaoPerfilFinanceiro | null;
  historico: ConfiguracaoPerfilFinanceiro[];
}

export interface SalvarPerfilFinanceiroPayload {
  percentualEconomiaMensalDesejado: number;
  percentualReservaEmergenciaDesejado: number;
  mesesReservaEmergenciaDesejados: number;
  percentualMaximoComprometimentoRenda: number;
  percentualMaximoEndividamento: number;
  percentualMinimoInvestimento: number;
  patrimonioLiquidoAlvo: number | null;
  observacao: string | null;
}
