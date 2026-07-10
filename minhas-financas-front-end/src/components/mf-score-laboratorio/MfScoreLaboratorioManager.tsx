"use client";

import type { ReactNode } from "react";
import { useEffect, useMemo, useState } from "react";
import { Gauge, Loader2, Search } from "lucide-react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { useAuth } from "@/providers/auth-provider";
import { buscarScoreUsuarioMfScoreLaboratorio, listarUsuariosMfScoreLaboratorio } from "@/services/api/mf-score-laboratorio";
import { ApiError } from "@/types/api";
import { IndicadorMfScoreLaboratorio, MfScoreLaboratorioDetalhe, UsuarioMfScoreLaboratorio } from "@/types/mf-score-laboratorio";

const moeda = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

const numero = new Intl.NumberFormat("pt-BR", {
  maximumFractionDigits: 2,
  minimumFractionDigits: 0,
});

const dataHora = new Intl.DateTimeFormat("pt-BR", {
  dateStyle: "short",
  timeStyle: "short",
});

export function MfScoreLaboratorioManager() {
  const { session } = useAuth();

  const [usuarios, setUsuarios] = useState<UsuarioMfScoreLaboratorio[]>([]);
  const [usuarioSelecionadoId, setUsuarioSelecionadoId] = useState<string | null>(null);
  const [detalhe, setDetalhe] = useState<MfScoreLaboratorioDetalhe | null>(null);
  const [busca, setBusca] = useState("");
  const [carregandoUsuarios, setCarregandoUsuarios] = useState(true);
  const [carregandoScore, setCarregandoScore] = useState(false);
  const [mensagemErro, setMensagemErro] = useState("");

  const usuariosFiltrados = useMemo(() => {
    const termo = busca.trim().toLowerCase();
    if (!termo) {
      return usuarios;
    }

    return usuarios.filter((usuario) =>
      [usuario.nome, usuario.email].some((valor) => valor.toLowerCase().includes(termo))
    );
  }, [busca, usuarios]);

  useEffect(() => {
    if (!session?.token) {
      return;
    }

    const token = session.token;
    async function carregarUsuarios(tokenAtual: string) {
      try {
        setCarregandoUsuarios(true);
        setMensagemErro("");

        const response = await listarUsuariosMfScoreLaboratorio(tokenAtual);
        const itens = response.dados ?? [];
        setUsuarios(itens);
      } catch (error) {
        if (error instanceof ApiError) {
          setMensagemErro(error.message);
        } else {
          setMensagemErro("Não foi possível carregar os usuários do laboratório do MF Score.");
        }
      } finally {
        setCarregandoUsuarios(false);
      }
    }

    void carregarUsuarios(token);
  }, [session?.token]);

  async function analisarUsuario(usuario: UsuarioMfScoreLaboratorio) {
    if (!session?.token) {
      return;
    }

    const token = session.token;

    try {
      setUsuarioSelecionadoId(usuario.usuarioId);
      setCarregandoScore(true);
      setMensagemErro("");

      const response = await buscarScoreUsuarioMfScoreLaboratorio(usuario.usuarioId, token);
      setDetalhe(response.dados ?? null);
    } catch (error) {
      setDetalhe(null);

      if (error instanceof ApiError) {
        setMensagemErro(error.message);
      } else {
        setMensagemErro("Não foi possível analisar o MF Score do usuário selecionado.");
      }
    } finally {
      setCarregandoScore(false);
    }
  }

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="flex-1 bg-gray-50 px-6 py-8 dark:bg-[#020817] md:px-8">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
          <section className="space-y-2">
            <h1 className="text-3xl font-semibold tracking-tight">Laboratório do MF Score</h1>
            <p className="max-w-4xl text-sm text-muted-foreground">
              Ferramenta interna de leitura para inspecionar usuários reais e entender como o Motor Financeiro construiu
              o MF Score, sem criar personas, sem editar dados e sem alterar nenhuma regra do cálculo.
            </p>
          </section>

          {mensagemErro ? (
            <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
              {mensagemErro}
            </div>
          ) : null}

          <section className="grid gap-6 xl:grid-cols-[1.1fr_1.6fr]">
            <Card>
              <CardHeader className="gap-4">
                <div>
                  <CardTitle>Usuários reais</CardTitle>
                  <CardDescription>
                    Selecione um usuário para abrir a leitura completa do MF Score calculado pelo motor oficial.
                  </CardDescription>
                </div>

                <div className="relative">
                  <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <Input
                    value={busca}
                    onChange={(event) => setBusca(event.target.value)}
                    placeholder="Pesquisar por nome ou e-mail"
                    className="pl-9"
                  />
                </div>
              </CardHeader>
              <CardContent>
                {carregandoUsuarios ? (
                  <EstadoCarregando texto="Carregando usuários..." />
                ) : usuariosFiltrados.length === 0 ? (
                  <EstadoVazio texto="Nenhum usuário encontrado para os filtros atuais." />
                ) : (
                  <div className="overflow-hidden rounded-lg border">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Usuário</TableHead>
                          <TableHead>Cadastro</TableHead>
                          <TableHead className="text-right">Ação</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {usuariosFiltrados.map((usuario) => {
                          const selecionado = usuario.usuarioId === usuarioSelecionadoId;

                          return (
                            <TableRow
                              key={usuario.usuarioId}
                              className={selecionado ? "bg-muted/50" : undefined}
                            >
                              <TableCell>
                                <div className="space-y-1">
                                  <p className="font-medium">{usuario.nome}</p>
                                  <p className="text-xs text-muted-foreground">{usuario.email}</p>
                                </div>
                              </TableCell>
                              <TableCell className="text-sm text-muted-foreground">
                                {formatarDataCadastro(usuario.dataCadastro)}
                              </TableCell>
                              <TableCell className="text-right">
                                <Button
                                  size="sm"
                                  variant={selecionado ? "default" : "outline"}
                                  onClick={() => void analisarUsuario(usuario)}
                                  disabled={carregandoScore && selecionado}
                                >
                                  <Gauge className="mr-2 h-4 w-4" />
                                  {carregandoScore && selecionado ? "Analisando..." : "Analisar MF Score"}
                                </Button>
                              </TableCell>
                            </TableRow>
                          );
                        })}
                      </TableBody>
                    </Table>
                  </div>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Painel de resultado</CardTitle>
                <CardDescription>
                  O laboratório apenas consulta e explica o resultado do motor oficial. Nenhum dado do usuário é alterado
                  por esta tela.
                </CardDescription>
              </CardHeader>
              <CardContent>
                {carregandoScore ? (
                  <EstadoCarregando texto="Calculando MF Score do usuário selecionado..." />
                ) : !detalhe ? (
                  <EstadoVazio texto='Selecione um usuário e clique em "Analisar MF Score" para abrir o laboratório.' />
                ) : (
                  <div className="space-y-6">
                    <div className="flex flex-wrap items-center justify-between gap-3 rounded-lg border px-4 py-4">
                      <div className="space-y-1">
                        <p className="text-sm text-muted-foreground">Usuário selecionado</p>
                        <h2 className="text-xl font-semibold">{detalhe.usuario.nome}</h2>
                        <p className="text-sm text-muted-foreground">{detalhe.usuario.email}</p>
                      </div>
                      <div className="flex flex-wrap gap-2">
                        <Badge variant="outline">Modelo {detalhe.versaoModelo}</Badge>
                        <Badge variant="outline">Risco {detalhe.risco}</Badge>
                        <Badge variant={obterVariantClassificacao(detalhe.classificacao)}>{detalhe.classificacao}</Badge>
                      </div>
                    </div>

                    <div className="grid gap-4 md:grid-cols-5">
                      <ResumoNumero titulo="MF Score base" valor={String(detalhe.mfScoreBase)} />
                      <ResumoNumero titulo="MF Score final" valor={String(detalhe.mfScoreFinal)} />
                      <ResumoNumero titulo="Classificação" valor={detalhe.classificacao} />
                      <ResumoNumero titulo="Risco" valor={detalhe.risco} />
                      <ResumoNumero titulo="Penalidade total" valor={numero.format(detalhe.penalidadeTotal)} />
                    </div>

                    <div className="grid gap-4 xl:grid-cols-[1.1fr_1fr]">
                      <Card className="border-dashed">
                        <CardHeader className="pb-3">
                          <CardTitle className="text-base">Tendência</CardTitle>
                          <CardDescription>{detalhe.tendencia.descricao}</CardDescription>
                        </CardHeader>
                        <CardContent className="space-y-3 text-sm">
                          <div className="flex flex-wrap items-center gap-2">
                            <span className="font-medium">Direção:</span>
                            <Badge variant="secondary">{formatarDirecao(detalhe.tendencia.direcao)}</Badge>
                          </div>
                          <p className="text-muted-foreground">
                            Histórico disponível:{" "}
                            {detalhe.tendencia.historicoNotas.length > 0
                              ? detalhe.tendencia.historicoNotas.join(" → ")
                              : "Ainda sem série histórica suficiente."}
                          </p>
                        </CardContent>
                      </Card>

                      <Card className="border-dashed">
                        <CardHeader className="pb-3">
                          <CardTitle className="text-base">Leitura executiva do motor</CardTitle>
                          <CardDescription>{detalhe.descricao}</CardDescription>
                        </CardHeader>
                        <CardContent>
                          {detalhe.resumoExecutivoDosPilares.length === 0 ? (
                            <p className="text-sm text-muted-foreground">O motor não devolveu resumo textual dos pilares.</p>
                          ) : (
                            <ul className="space-y-2 text-sm">
                              {detalhe.resumoExecutivoDosPilares.map((item) => (
                                <li key={item}>• {item}</li>
                              ))}
                            </ul>
                          )}
                        </CardContent>
                      </Card>
                    </div>

                    <div className="grid gap-6 xl:grid-cols-2">
                      <BlocoTabela
                        titulo="Pilares"
                        descricao="Notas, pesos e indicadores agrupados por pilar do MF Score."
                        colunas={["Pilar", "Nota", "Peso"]}
                        vazio="Nenhum pilar retornado pelo motor."
                        linhas={detalhe.pilares.map((pilar) => (
                          <TableRow key={pilar.codigo}>
                            <TableCell>
                              <div className="space-y-1">
                                <p className="font-medium">{pilar.nome}</p>
                                <p className="text-xs text-muted-foreground">{pilar.descricao}</p>
                                <p className="text-xs text-muted-foreground">
                                  Indicadores: {pilar.indicadores.join(", ")}
                                </p>
                              </div>
                            </TableCell>
                            <TableCell>{pilar.nota}</TableCell>
                            <TableCell>{pilar.peso}%</TableCell>
                          </TableRow>
                        ))}
                      />

                      <BlocoTabela
                        titulo="Indicadores críticos"
                        descricao="Eventos críticos e materializações de risco que acionaram penalização."
                        colunas={["Indicador", "Pilar", "Penalidade"]}
                        vazio="Nenhum indicador crítico foi acionado para este usuário."
                        linhas={detalhe.indicadoresCriticos.map((indicador) => (
                          <TableRow key={`${indicador.codigo}-${indicador.nome}-${indicador.motivo}`}>
                            <TableCell>
                              <div className="space-y-1">
                                <p className="font-medium">{indicador.nome}</p>
                                <p className="text-xs text-muted-foreground">{indicador.motivo}</p>
                              </div>
                            </TableCell>
                            <TableCell>{indicador.pilarRelacionado}</TableCell>
                            <TableCell>{numero.format(indicador.penalidade)}</TableCell>
                          </TableRow>
                        ))}
                      />
                    </div>

                    <BlocoTabela
                      titulo="Indicadores completos"
                      descricao="Leitura detalhada dos indicadores usados pelo Motor Financeiro."
                      colunas={["Indicador", "Atual", "Ideal", "Status"]}
                      vazio="Nenhum indicador retornado pelo motor."
                      linhas={detalhe.indicadores.map((indicador) => (
                        <TableRow key={indicador.codigo}>
                          <TableCell>
                            <div className="space-y-1">
                              <p className="font-medium">{indicador.nome}</p>
                              <p className="text-xs text-muted-foreground">{indicador.descricao}</p>
                              <p className="text-xs text-muted-foreground">{indicador.observacao}</p>
                              {possuiDetalhesTemporais(indicador) ? (
                                <p className="text-xs text-muted-foreground">
                                  Obrigações: {moeda.format(indicador.valorObrigacoesPrevistas ?? 0)} | Receita:{" "}
                                  {moeda.format(indicador.valorReceitaPrevista ?? 0)} | Comprometimento:{" "}
                                  {numero.format(indicador.percentualComprometimento ?? 0)}%
                                </p>
                              ) : null}
                            </div>
                          </TableCell>
                          <TableCell>{formatarValorIndicador(indicador, indicador.valorAtual)}</TableCell>
                          <TableCell>{formatarValorIndicador(indicador, indicador.valorIdeal)}</TableCell>
                          <TableCell>
                            <Badge variant={obterVariantStatusIndicador(indicador.status)}>
                              {formatarStatusIndicador(indicador.status)}
                            </Badge>
                          </TableCell>
                        </TableRow>
                      ))}
                    />

                    <div className="grid gap-6 xl:grid-cols-2">
                      <BlocoTabela
                        titulo="Penalizações aplicadas"
                        descricao="Detalhamento das penalizações críticas efetivamente descontadas."
                        colunas={["Penalização", "Pilar", "Valor"]}
                        vazio="Nenhuma penalização crítica foi aplicada."
                        linhas={detalhe.penalizacoes.map((penalizacao) => (
                          <TableRow key={`${penalizacao.nome}-${penalizacao.motivo}`}>
                            <TableCell>
                              <div className="space-y-1">
                                <p className="font-medium">{penalizacao.nome}</p>
                                <p className="text-xs text-muted-foreground">{penalizacao.motivo}</p>
                              </div>
                            </TableCell>
                            <TableCell>{penalizacao.pilarRelacionado}</TableCell>
                            <TableCell>{numero.format(penalizacao.penalidade)}</TableCell>
                          </TableRow>
                        ))}
                      />

                      <Card>
                        <CardHeader>
                          <CardTitle className="text-base">Regras críticas aplicadas</CardTitle>
                          <CardDescription>Lista textual das regras críticas efetivamente acionadas.</CardDescription>
                        </CardHeader>
                        <CardContent>
                          {detalhe.regrasCriticasAplicadas.length === 0 ? (
                            <EstadoVazioInterno texto="Nenhuma regra crítica textual foi registrada para este cálculo." />
                          ) : (
                            <ul className="space-y-2 text-sm">
                              {detalhe.regrasCriticasAplicadas.map((regra) => (
                                <li key={regra}>• {regra}</li>
                              ))}
                            </ul>
                          )}
                        </CardContent>
                      </Card>
                    </div>

                    <Card>
                      <CardHeader>
                        <CardTitle className="text-base">Dados usados no cálculo</CardTitle>
                        <CardDescription>
                          Resumo operacional do contexto consumido pelo motor oficial na geração deste MF Score.
                        </CardDescription>
                      </CardHeader>
                      <CardContent className="grid gap-3 md:grid-cols-2 xl:grid-cols-4">
                        <ItemDado titulo="Data de referência" valor={dataHora.format(new Date(detalhe.dadosEntrada.dataReferencia))} />
                        <ItemDado titulo="Lançamentos considerados" valor={String(detalhe.dadosEntrada.quantidadeLancamentos)} />
                        <ItemDado titulo="Receitas do mês" valor={moeda.format(detalhe.dadosEntrada.receitaMensalConsiderada)} />
                        <ItemDado titulo="Despesas do mês" valor={moeda.format(detalhe.dadosEntrada.despesaMensalConsiderada)} />
                        <ItemDado titulo="Ativos considerados" valor={`${detalhe.dadosEntrada.quantidadeAtivos} (${moeda.format(detalhe.dadosEntrada.valorAtivosConsiderados)})`} />
                        <ItemDado titulo="Passivos considerados" valor={`${detalhe.dadosEntrada.quantidadePassivos} (${moeda.format(detalhe.dadosEntrada.valorPassivosConsiderados)})`} />
                        <ItemDado titulo="Metas consideradas" valor={String(detalhe.dadosEntrada.quantidadeMetas)} />
                        <ItemDado titulo="Perfil financeiro" valor={detalhe.dadosEntrada.possuiPerfilFinanceiroConfigurado ? "Disponível" : "Ausente"} />
                        <ItemDado titulo="Plano estratégico" valor={detalhe.dadosEntrada.possuiPlanoEstrategicoVigente ? "Disponível" : "Ausente"} />
                        <ItemDado titulo="Compromissos financeiros" valor={detalhe.dadosEntrada.possuiCompromissosFinanceiros ? "Disponíveis" : "Ausentes"} />
                        <ItemDado titulo="Inadimplência atual" valor={detalhe.dadosEntrada.possuiInadimplencia ? `Nível ${detalhe.dadosEntrada.nivelInadimplencia}` : "Não"} />
                        <ItemDado titulo="Meses negativos consecutivos" valor={String(detalhe.dadosEntrada.mesesConsecutivosFluxoNegativo)} />
                        <ItemDado titulo="Parâmetros de planejamento" valor={`${detalhe.dadosEntrada.quantidadeParametrosPlanejamentoConfigurados}/${detalhe.dadosEntrada.totalParametrosPlanejamentoEsperados}`} />
                        <ItemDado titulo="Nota de configuração do planejamento" valor={String(detalhe.dadosEntrada.notaConfiguracaoPlanejamento)} />
                        <ItemDado titulo="Nota do plano estratégico" valor={detalhe.dadosEntrada.notaPlanoEstrategico != null ? String(detalhe.dadosEntrada.notaPlanoEstrategico) : "Não aplicável"} />
                        <ItemDado titulo="Nota dos compromissos" valor={detalhe.dadosEntrada.notaCompromissosFinanceiros != null ? String(detalhe.dadosEntrada.notaCompromissosFinanceiros) : "Não aplicável"} />
                      </CardContent>
                    </Card>

                    <Card>
                      <CardHeader>
                        <CardTitle className="text-base">Observações de limitação e cobertura</CardTitle>
                        <CardDescription>
                          Sinais que ajudam a entender onde o cálculo já está bem coberto e onde ainda depende de contexto ausente.
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        {detalhe.observacoesLimitacoes.length === 0 ? (
                          <EstadoVazioInterno texto="Nenhuma limitação adicional foi registrada para este cálculo." />
                        ) : (
                          <ul className="space-y-2 text-sm">
                            {detalhe.observacoesLimitacoes.map((observacao) => (
                              <li key={observacao}>• {observacao}</li>
                            ))}
                          </ul>
                        )}
                      </CardContent>
                    </Card>
                  </div>
                )}
              </CardContent>
            </Card>
          </section>
        </div>
      </main>
    </div>
  );
}

function BlocoTabela({
  titulo,
  descricao,
  colunas,
  linhas,
  vazio,
}: {
  titulo: string;
  descricao: string;
  colunas: string[];
  linhas: ReactNode[];
  vazio: string;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">{titulo}</CardTitle>
        <CardDescription>{descricao}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="overflow-hidden rounded-lg border">
          <Table>
            <TableHeader>
              <TableRow>
                {colunas.map((coluna) => (
                  <TableHead key={coluna}>{coluna}</TableHead>
                ))}
              </TableRow>
            </TableHeader>
            <TableBody>
              {linhas.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={colunas.length} className="text-sm text-muted-foreground">
                    {vazio}
                  </TableCell>
                </TableRow>
              ) : (
                linhas
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  );
}

function ResumoNumero({ titulo, valor }: { titulo: string; valor: string }) {
  return (
    <div className="rounded-lg border px-4 py-4">
      <p className="text-sm text-muted-foreground">{titulo}</p>
      <p className="mt-2 text-2xl font-semibold tracking-tight">{valor}</p>
    </div>
  );
}

function ItemDado({ titulo, valor }: { titulo: string; valor: string }) {
  return (
    <div className="rounded-lg border px-4 py-3">
      <p className="text-xs uppercase tracking-wide text-muted-foreground">{titulo}</p>
      <p className="mt-2 text-sm font-medium">{valor}</p>
    </div>
  );
}

function EstadoCarregando({ texto }: { texto: string }) {
  return (
    <div className="flex items-center justify-center gap-2 rounded-lg border border-dashed px-6 py-10 text-sm text-muted-foreground">
      <Loader2 className="h-4 w-4 animate-spin" />
      {texto}
    </div>
  );
}

function EstadoVazio({ texto }: { texto: string }) {
  return (
    <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">{texto}</div>
  );
}

function EstadoVazioInterno({ texto }: { texto: string }) {
  return <p className="text-sm text-muted-foreground">{texto}</p>;
}

function formatarValorIndicador(indicador: IndicadorMfScoreLaboratorio, valor: number) {
  switch (indicador.formato) {
    case "Percentual":
      return `${numero.format(valor)}%`;
    case "Meses":
      return `${numero.format(valor)} meses`;
    default:
      return moeda.format(valor);
  }
}

function formatarDataCadastro(data?: string | null) {
  if (!data) {
    return "Não disponível";
  }

  return dataHora.format(new Date(data));
}

function formatarStatusIndicador(status: string) {
  switch (status) {
    case "Atencao":
      return "Atenção";
    case "Critico":
      return "Crítico";
    default:
      return status;
  }
}

function formatarDirecao(direcao: string) {
  switch (direcao) {
    case "Melhorando":
      return "Melhorando";
    case "Piorando":
      return "Piorando";
    case "Estavel":
      return "Estável";
    default:
      return "Indeterminada";
  }
}

function obterVariantStatusIndicador(status: string): "default" | "secondary" | "destructive" | "outline" {
  switch (status) {
    case "Excelente":
      return "default";
    case "Bom":
      return "secondary";
    case "Critico":
      return "destructive";
    default:
      return "outline";
  }
}

function obterVariantClassificacao(classificacao: string): "default" | "secondary" | "destructive" | "outline" {
  if (classificacao.includes("Excelente") || classificacao.includes("Muito Bom")) {
    return "default";
  }

  if (classificacao.includes("Bom")) {
    return "secondary";
  }

  if (classificacao.includes("Crítico")) {
    return "destructive";
  }

  return "outline";
}

function possuiDetalhesTemporais(indicador: IndicadorMfScoreLaboratorio) {
  return (
    indicador.valorObrigacoesPrevistas != null ||
    indicador.valorReceitaPrevista != null ||
    indicador.percentualComprometimento != null
  );
}
