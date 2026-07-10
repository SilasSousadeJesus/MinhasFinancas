"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { CheckCircle2, Gauge, Loader2, Plus, ShieldCheck, Trash2 } from "lucide-react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Textarea } from "@/components/ui/textarea";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import {
  MfScorePersonaItem,
  ResultadoRodarMfScorePersona,
  SalvarMfScorePersonaPayload,
  StatusPersonaMfScore,
} from "@/types/mf-score-personas";
import {
  cadastrarMfScorePersona,
  editarMfScorePersona,
  inativarMfScorePersona,
  listarMfScorePersonas,
  marcarPersonaAuditada,
  marcarPersonaCasoCanonico,
  rodarMfScorePersona,
} from "@/services/api/mf-score-personas";

interface PersonaFormState {
  nome: string;
  descricao: string;
  objetivoDaPersona: string;
  rendaMensal: string;
  receitasPrevistas30Dias: string;
  receitasPrevistas90Dias: string;
  receitasPrevistas180Dias: string;
  receitasPrevistas12Meses: string;
  despesasMensais: string;
  obrigacoes30Dias: string;
  obrigacoes90Dias: string;
  obrigacoes180Dias: string;
  obrigacoes12Meses: string;
  reservaEmergencia: string;
  patrimonioBruto: string;
  passivos: string;
  patrimonioLiquido: string;
  possuiPerfilFinanceiroConfigurado: boolean;
  possuiPlanoEstrategico: boolean;
  possuiMetas: boolean;
  possuiCompromissos: boolean;
  compromissosCumpridos: string;
  possuiInadimplencia: boolean;
  scoreHumanoSugerido: string;
  faixaEsperadaMin: string;
  faixaEsperadaMax: string;
  justificativaNotaHumana: string;
  observacoes: string;
}

const formularioInicial: PersonaFormState = {
  nome: "",
  descricao: "",
  objetivoDaPersona: "",
  rendaMensal: "0",
  receitasPrevistas30Dias: "0",
  receitasPrevistas90Dias: "0",
  receitasPrevistas180Dias: "0",
  receitasPrevistas12Meses: "0",
  despesasMensais: "0",
  obrigacoes30Dias: "0",
  obrigacoes90Dias: "0",
  obrigacoes180Dias: "0",
  obrigacoes12Meses: "0",
  reservaEmergencia: "0",
  patrimonioBruto: "0",
  passivos: "0",
  patrimonioLiquido: "0",
  possuiPerfilFinanceiroConfigurado: false,
  possuiPlanoEstrategico: false,
  possuiMetas: false,
  possuiCompromissos: false,
  compromissosCumpridos: "0",
  possuiInadimplencia: false,
  scoreHumanoSugerido: "",
  faixaEsperadaMin: "",
  faixaEsperadaMax: "",
  justificativaNotaHumana: "",
  observacoes: "",
};

function obterStatusLabel(status: StatusPersonaMfScore) {
  switch (status) {
    case StatusPersonaMfScore.EmAuditoria:
      return "Em auditoria";
    case StatusPersonaMfScore.Auditada:
      return "Auditada";
    case StatusPersonaMfScore.CasoCanonico:
      return "Caso canônico";
    case StatusPersonaMfScore.Inativa:
      return "Inativa";
    default:
      return "Rascunho";
  }
}

function obterStatusVariant(status: StatusPersonaMfScore): "default" | "secondary" | "destructive" | "outline" {
  switch (status) {
    case StatusPersonaMfScore.CasoCanonico:
      return "default";
    case StatusPersonaMfScore.Auditada:
      return "secondary";
    case StatusPersonaMfScore.Inativa:
      return "destructive";
    default:
      return "outline";
  }
}

function numeroParaTexto(valor?: number | null) {
  return valor == null ? "" : String(valor);
}

function personaParaFormulario(persona: MfScorePersonaItem): PersonaFormState {
  return {
    nome: persona.nome,
    descricao: persona.descricao,
    objetivoDaPersona: persona.objetivoDaPersona,
    rendaMensal: String(persona.rendaMensal),
    receitasPrevistas30Dias: String(persona.receitasPrevistas30Dias),
    receitasPrevistas90Dias: String(persona.receitasPrevistas90Dias),
    receitasPrevistas180Dias: String(persona.receitasPrevistas180Dias),
    receitasPrevistas12Meses: String(persona.receitasPrevistas12Meses),
    despesasMensais: String(persona.despesasMensais),
    obrigacoes30Dias: String(persona.obrigacoes30Dias),
    obrigacoes90Dias: String(persona.obrigacoes90Dias),
    obrigacoes180Dias: String(persona.obrigacoes180Dias),
    obrigacoes12Meses: String(persona.obrigacoes12Meses),
    reservaEmergencia: String(persona.reservaEmergencia),
    patrimonioBruto: String(persona.patrimonioBruto),
    passivos: String(persona.passivos),
    patrimonioLiquido: String(persona.patrimonioLiquido),
    possuiPerfilFinanceiroConfigurado: persona.possuiPerfilFinanceiroConfigurado,
    possuiPlanoEstrategico: persona.possuiPlanoEstrategico,
    possuiMetas: persona.possuiMetas,
    possuiCompromissos: persona.possuiCompromissos,
    compromissosCumpridos: String(persona.compromissosCumpridos),
    possuiInadimplencia: persona.possuiInadimplencia,
    scoreHumanoSugerido: numeroParaTexto(persona.scoreHumanoSugerido),
    faixaEsperadaMin: numeroParaTexto(persona.faixaEsperadaMin),
    faixaEsperadaMax: numeroParaTexto(persona.faixaEsperadaMax),
    justificativaNotaHumana: persona.justificativaNotaHumana ?? "",
    observacoes: persona.observacoes ?? "",
  };
}

function textoParaDecimal(valor: string) {
  const normalizado = valor.replace(",", ".").trim();
  const numero = Number(normalizado);
  return Number.isFinite(numero) ? numero : 0;
}

function textoParaInteiroOpcional(valor: string) {
  if (!valor.trim()) {
    return null;
  }

  const numero = Number(valor);
  return Number.isFinite(numero) ? Math.trunc(numero) : null;
}

function formularioParaPayload(formulario: PersonaFormState): SalvarMfScorePersonaPayload {
  return {
    nome: formulario.nome.trim(),
    descricao: formulario.descricao.trim(),
    objetivoDaPersona: formulario.objetivoDaPersona.trim(),
    rendaMensal: textoParaDecimal(formulario.rendaMensal),
    receitasPrevistas30Dias: textoParaDecimal(formulario.receitasPrevistas30Dias),
    receitasPrevistas90Dias: textoParaDecimal(formulario.receitasPrevistas90Dias),
    receitasPrevistas180Dias: textoParaDecimal(formulario.receitasPrevistas180Dias),
    receitasPrevistas12Meses: textoParaDecimal(formulario.receitasPrevistas12Meses),
    despesasMensais: textoParaDecimal(formulario.despesasMensais),
    obrigacoes30Dias: textoParaDecimal(formulario.obrigacoes30Dias),
    obrigacoes90Dias: textoParaDecimal(formulario.obrigacoes90Dias),
    obrigacoes180Dias: textoParaDecimal(formulario.obrigacoes180Dias),
    obrigacoes12Meses: textoParaDecimal(formulario.obrigacoes12Meses),
    reservaEmergencia: textoParaDecimal(formulario.reservaEmergencia),
    patrimonioBruto: textoParaDecimal(formulario.patrimonioBruto),
    passivos: textoParaDecimal(formulario.passivos),
    patrimonioLiquido: textoParaDecimal(formulario.patrimonioLiquido),
    possuiPerfilFinanceiroConfigurado: formulario.possuiPerfilFinanceiroConfigurado,
    possuiPlanoEstrategico: formulario.possuiPlanoEstrategico,
    possuiMetas: formulario.possuiMetas,
    possuiCompromissos: formulario.possuiCompromissos,
    compromissosCumpridos: Math.max(0, Number(formulario.compromissosCumpridos || "0")),
    possuiInadimplencia: formulario.possuiInadimplencia,
    scoreHumanoSugerido: textoParaInteiroOpcional(formulario.scoreHumanoSugerido),
    faixaEsperadaMin: textoParaInteiroOpcional(formulario.faixaEsperadaMin),
    faixaEsperadaMax: textoParaInteiroOpcional(formulario.faixaEsperadaMax),
    justificativaNotaHumana: formulario.justificativaNotaHumana.trim() || null,
    observacoes: formulario.observacoes.trim() || null,
  };
}

export function MfScorePersonasManager() {
  const { session } = useAuth();

  const [personas, setPersonas] = useState<MfScorePersonaItem[]>([]);
  const [personaSelecionadaId, setPersonaSelecionadaId] = useState<string | null>(null);
  const [formulario, setFormulario] = useState<PersonaFormState>(formularioInicial);
  const [resultado, setResultado] = useState<ResultadoRodarMfScorePersona | null>(null);
  const [loading, setLoading] = useState(true);
  const [salvando, setSalvando] = useState(false);
  const [rodandoScore, setRodandoScore] = useState(false);
  const [mensagemErro, setMensagemErro] = useState("");
  const [mensagemSucesso, setMensagemSucesso] = useState("");
  const [personaParaInativar, setPersonaParaInativar] = useState<MfScorePersonaItem | null>(null);

  const personaSelecionada = useMemo(
    () => personas.find((item) => item.id === personaSelecionadaId) ?? null,
    [personas, personaSelecionadaId]
  );

  const estatisticas = useMemo(() => {
    return personas.reduce(
      (acc, persona) => {
        if (persona.status === StatusPersonaMfScore.CasoCanonico) {
          acc.canonicas += 1;
        }

        if (persona.status === StatusPersonaMfScore.Auditada) {
          acc.auditadas += 1;
        }

        if (persona.status !== StatusPersonaMfScore.Inativa) {
          acc.ativas += 1;
        }

        return acc;
      },
      { ativas: 0, auditadas: 0, canonicas: 0 }
    );
  }, [personas]);

  const carregarPersonas = useCallback(
    async (personaPreferencialId?: string | null) => {
      if (!session?.token) {
        return;
      }

      try {
        setLoading(true);
        setMensagemErro("");

        const response = await listarMfScorePersonas(session.token);
        const itens = response.dados ?? [];
        setPersonas(itens);

        const proximaSelecionada =
          (personaPreferencialId && itens.find((item) => item.id === personaPreferencialId)?.id) ??
          (personaSelecionadaId && itens.find((item) => item.id === personaSelecionadaId)?.id) ??
          itens[0]?.id ??
          null;

        setPersonaSelecionadaId(proximaSelecionada);

        if (proximaSelecionada) {
          const persona = itens.find((item) => item.id === proximaSelecionada);
          if (persona) {
            setFormulario(personaParaFormulario(persona));
          }
        } else {
          setFormulario(formularioInicial);
          setResultado(null);
        }
      } catch (error) {
        if (error instanceof ApiError) {
          setMensagemErro(error.message);
        } else {
          setMensagemErro("Não foi possível carregar as personas de calibração.");
        }
      } finally {
        setLoading(false);
      }
    },
    [personaSelecionadaId, session?.token]
  );

  useEffect(() => {
    void carregarPersonas();
  }, [carregarPersonas]);

  function atualizarCampo<K extends keyof PersonaFormState>(campo: K, valor: PersonaFormState[K]) {
    setFormulario((anterior) => ({ ...anterior, [campo]: valor }));
  }

  function selecionarPersona(persona: MfScorePersonaItem) {
    setPersonaSelecionadaId(persona.id);
    setFormulario(personaParaFormulario(persona));
    setResultado(null);
    setMensagemErro("");
    setMensagemSucesso("");
  }

  function iniciarNovaPersona() {
    setPersonaSelecionadaId(null);
    setFormulario(formularioInicial);
    setResultado(null);
    setMensagemErro("");
    setMensagemSucesso("");
  }

  async function salvarPersona() {
    if (!session?.token) {
      return;
    }

    try {
      setSalvando(true);
      setMensagemErro("");
      setMensagemSucesso("");

      const payload = formularioParaPayload(formulario);
      let personaSalva: MfScorePersonaItem;

      if (personaSelecionadaId) {
        const response = await editarMfScorePersona(personaSelecionadaId, payload, session.token);
        if (!response.dados) {
          throw new Error("Resposta da API sem persona atualizada.");
        }
        personaSalva = response.dados;
        setMensagemSucesso("Persona atualizada com sucesso.");
      } else {
        const response = await cadastrarMfScorePersona(payload, session.token);
        if (!response.dados) {
          throw new Error("Resposta da API sem persona criada.");
        }
        personaSalva = response.dados;
        setMensagemSucesso("Persona criada com sucesso.");
      }

      setResultado(null);
      await carregarPersonas(personaSalva.id);
    } catch (error) {
      if (error instanceof ApiError) {
        setMensagemErro(error.message);
      } else {
        setMensagemErro("Não foi possível salvar a persona.");
      }
    } finally {
      setSalvando(false);
    }
  }

  async function executarScore(personaId: string) {
    if (!session?.token) {
      return;
    }

    try {
      setRodandoScore(true);
      setMensagemErro("");
      setMensagemSucesso("");

      const response = await rodarMfScorePersona(personaId, session.token);
      setResultado(response.dados);
      setMensagemSucesso("MF Score executado com sucesso para a persona selecionada.");
    } catch (error) {
      if (error instanceof ApiError) {
        setMensagemErro(error.message);
      } else {
        setMensagemErro("Não foi possível rodar o MF Score da persona.");
      }
    } finally {
      setRodandoScore(false);
    }
  }

  async function mudarStatus(tipo: "auditada" | "canonica") {
    if (!session?.token || !personaSelecionadaId) {
      return;
    }

    try {
      setMensagemErro("");
      setMensagemSucesso("");

      const response =
        tipo === "auditada"
          ? await marcarPersonaAuditada(personaSelecionadaId, session.token)
          : await marcarPersonaCasoCanonico(personaSelecionadaId, session.token);

      if (!response.dados) {
        throw new Error("Resposta da API sem persona atualizada.");
      }

      setMensagemSucesso(
        tipo === "auditada"
          ? "Persona marcada como auditada com sucesso."
          : "Persona marcada como caso canônico com sucesso."
      );

      await carregarPersonas(response.dados.id);
    } catch (error) {
      if (error instanceof ApiError) {
        setMensagemErro(error.message);
      } else {
        setMensagemErro("Não foi possível atualizar o status da persona.");
      }
    }
  }

  async function confirmarInativacao() {
    if (!session?.token || !personaParaInativar) {
      return;
    }

    try {
      setMensagemErro("");
      setMensagemSucesso("");

      await inativarMfScorePersona(personaParaInativar.id, session.token);
      setMensagemSucesso("Persona inativada com sucesso.");

      const removidaEraSelecionada = personaSelecionadaId === personaParaInativar.id;
      setPersonaParaInativar(null);
      setResultado(null);

      await carregarPersonas(removidaEraSelecionada ? null : personaSelecionadaId);
    } catch (error) {
      if (error instanceof ApiError) {
        setMensagemErro(error.message);
      } else {
        setMensagemErro("Não foi possível inativar a persona.");
      }
    }
  }

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="flex-1 bg-gray-50 px-6 py-8 dark:bg-[#020817] md:px-8">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
          <section className="space-y-2">
            <h1 className="text-3xl font-semibold tracking-tight">Personas de calibração do MF Score</h1>
            <p className="max-w-4xl text-sm text-muted-foreground">
              Ferramenta interna de desenvolvimento para criar cenários sintéticos, rodar o Motor Financeiro oficial e
              comparar a nota calculada com a avaliação humana esperada.
            </p>
          </section>

          <section className="grid gap-4 md:grid-cols-3">
            <Card>
              <CardHeader className="pb-2">
                <CardDescription>Personas ativas</CardDescription>
                <CardTitle className="text-3xl">{estatisticas.ativas}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription>Auditadas</CardDescription>
                <CardTitle className="text-3xl">{estatisticas.auditadas}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription>Casos canônicos</CardDescription>
                <CardTitle className="text-3xl">{estatisticas.canonicas}</CardTitle>
              </CardHeader>
            </Card>
          </section>

          {mensagemErro ? (
            <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
              {mensagemErro}
            </div>
          ) : null}

          {mensagemSucesso ? (
            <div className="rounded-md border border-emerald-500/20 bg-emerald-500/5 px-4 py-3 text-sm text-emerald-700 dark:text-emerald-300">
              {mensagemSucesso}
            </div>
          ) : null}

          <section className="grid gap-6 xl:grid-cols-[1.1fr_1.4fr]">
            <Card>
              <CardHeader>
                <div className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
                  <div>
                    <CardTitle>Lista de personas</CardTitle>
                    <CardDescription>
                      Cada persona representa um cenário sintético para calibrar o MF Score sem usar usuários reais.
                    </CardDescription>
                  </div>
                  <Button onClick={iniciarNovaPersona}>
                    <Plus className="mr-2 h-4 w-4" />
                    Nova persona
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                {loading ? (
                  <div className="flex items-center justify-center gap-2 rounded-lg border border-dashed px-6 py-10 text-sm text-muted-foreground">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Carregando personas...
                  </div>
                ) : personas.length === 0 ? (
                  <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                    Nenhuma persona cadastrada ainda. Crie a primeira para iniciar a calibração estruturada do MF Score.
                  </div>
                ) : (
                  <div className="overflow-hidden rounded-lg border">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Persona</TableHead>
                          <TableHead>Status</TableHead>
                          <TableHead>Faixa humana</TableHead>
                          <TableHead>Caso canônico</TableHead>
                          <TableHead className="text-right">Ações</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {personas.map((persona) => (
                          <TableRow
                            key={persona.id}
                            className={persona.id === personaSelecionadaId ? "bg-muted/50" : undefined}
                          >
                            <TableCell>
                              <button
                                type="button"
                                onClick={() => selecionarPersona(persona)}
                                className="space-y-1 text-left"
                              >
                                <p className="font-medium">{persona.nome}</p>
                                <p className="line-clamp-2 max-w-xs text-xs text-muted-foreground">
                                  {persona.objetivoDaPersona}
                                </p>
                              </button>
                            </TableCell>
                            <TableCell>
                              <Badge variant={obterStatusVariant(persona.status)}>
                                {obterStatusLabel(persona.status)}
                              </Badge>
                            </TableCell>
                            <TableCell className="text-sm">
                              {persona.faixaEsperadaMin != null && persona.faixaEsperadaMax != null
                                ? `${persona.faixaEsperadaMin} - ${persona.faixaEsperadaMax}`
                                : "Não definida"}
                            </TableCell>
                            <TableCell>{persona.ehCasoCanonico ? "Sim" : "Não"}</TableCell>
                            <TableCell>
                              <div className="flex justify-end gap-2">
                                <Button variant="ghost" size="sm" onClick={() => selecionarPersona(persona)}>
                                  Editar
                                </Button>
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => void executarScore(persona.id)}
                                  disabled={rodandoScore || persona.status === StatusPersonaMfScore.Inativa}
                                >
                                  Rodar score
                                </Button>
                              </div>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <div className="flex flex-col gap-3 xl:flex-row xl:items-start xl:justify-between">
                  <div>
                    <CardTitle>{personaSelecionada ? "Editar persona" : "Nova persona"}</CardTitle>
                    <CardDescription>
                      Organize a persona em blocos de identificação, dados simulados, estrutura de planejamento e
                      avaliação humana.
                    </CardDescription>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    {personaSelecionada ? (
                      <>
                        <Badge variant={obterStatusVariant(personaSelecionada.status)}>
                          {obterStatusLabel(personaSelecionada.status)}
                        </Badge>
                        {personaSelecionada.ehCasoCanonico ? <Badge>Caso canônico</Badge> : null}
                      </>
                    ) : (
                      <Badge variant="outline">Rascunho novo</Badge>
                    )}
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="grid gap-4 md:grid-cols-2">
                  <div className="space-y-2">
                    <h3 className="text-base font-semibold">1. Identificação da persona</h3>
                    <p className="text-sm text-muted-foreground">
                      Contextualize o cenário que será testado pelo motor oficial.
                    </p>
                  </div>
                  {personaSelecionada ? (
                    <div className="flex flex-wrap justify-start gap-2 md:justify-end">
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() => void mudarStatus("auditada")}
                        disabled={personaSelecionada.status === StatusPersonaMfScore.Inativa}
                      >
                        <CheckCircle2 className="mr-2 h-4 w-4" />
                        Marcar auditada
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() => void mudarStatus("canonica")}
                        disabled={personaSelecionada.status === StatusPersonaMfScore.Inativa}
                      >
                        <ShieldCheck className="mr-2 h-4 w-4" />
                        Marcar caso canônico
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() => setPersonaParaInativar(personaSelecionada)}
                      >
                        <Trash2 className="mr-2 h-4 w-4" />
                        Inativar
                      </Button>
                    </div>
                  ) : null}
                </div>

                <div className="grid gap-4 md:grid-cols-2">
                  <div className="space-y-2">
                    <label className="text-sm font-medium">Nome</label>
                    <Input value={formulario.nome} onChange={(e) => atualizarCampo("nome", e.target.value)} />
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium">Objetivo da persona</label>
                    <Input
                      value={formulario.objetivoDaPersona}
                      onChange={(e) => atualizarCampo("objetivoDaPersona", e.target.value)}
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium">Descrição</label>
                  <Textarea
                    value={formulario.descricao}
                    onChange={(e) => atualizarCampo("descricao", e.target.value)}
                    className="min-h-[90px]"
                  />
                </div>

                <div className="border-t pt-6">
                  <div className="space-y-2">
                    <h3 className="text-base font-semibold">2. Dados financeiros simulados</h3>
                    <p className="text-sm text-muted-foreground">
                      Preencha o cenário sintético que será convertido em contexto para o motor oficial do MF Score.
                    </p>
                  </div>
                </div>

                <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                  <CampoNumero label="Renda mensal" value={formulario.rendaMensal} onChange={(v) => atualizarCampo("rendaMensal", v)} />
                  <CampoNumero label="Despesas mensais" value={formulario.despesasMensais} onChange={(v) => atualizarCampo("despesasMensais", v)} />
                  <CampoNumero label="Reserva de emergência" value={formulario.reservaEmergencia} onChange={(v) => atualizarCampo("reservaEmergencia", v)} />
                  <CampoNumero label="Patrimônio líquido" value={formulario.patrimonioLiquido} onChange={(v) => atualizarCampo("patrimonioLiquido", v)} />
                  <CampoNumero label="Patrimônio bruto" value={formulario.patrimonioBruto} onChange={(v) => atualizarCampo("patrimonioBruto", v)} />
                  <CampoNumero label="Passivos" value={formulario.passivos} onChange={(v) => atualizarCampo("passivos", v)} />
                </div>

                <div className="space-y-3">
                  <h4 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Receitas previstas</h4>
                  <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                    <CampoNumero label="30 dias" value={formulario.receitasPrevistas30Dias} onChange={(v) => atualizarCampo("receitasPrevistas30Dias", v)} />
                    <CampoNumero label="90 dias" value={formulario.receitasPrevistas90Dias} onChange={(v) => atualizarCampo("receitasPrevistas90Dias", v)} />
                    <CampoNumero label="180 dias" value={formulario.receitasPrevistas180Dias} onChange={(v) => atualizarCampo("receitasPrevistas180Dias", v)} />
                    <CampoNumero label="12 meses" value={formulario.receitasPrevistas12Meses} onChange={(v) => atualizarCampo("receitasPrevistas12Meses", v)} />
                  </div>
                </div>

                <div className="space-y-3">
                  <h4 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Obrigações futuras</h4>
                  <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                    <CampoNumero label="30 dias" value={formulario.obrigacoes30Dias} onChange={(v) => atualizarCampo("obrigacoes30Dias", v)} />
                    <CampoNumero label="90 dias" value={formulario.obrigacoes90Dias} onChange={(v) => atualizarCampo("obrigacoes90Dias", v)} />
                    <CampoNumero label="180 dias" value={formulario.obrigacoes180Dias} onChange={(v) => atualizarCampo("obrigacoes180Dias", v)} />
                    <CampoNumero label="12 meses" value={formulario.obrigacoes12Meses} onChange={(v) => atualizarCampo("obrigacoes12Meses", v)} />
                  </div>
                </div>

                <div className="border-t pt-6">
                  <div className="space-y-2">
                    <h3 className="text-base font-semibold">3. Estrutura de planejamento</h3>
                    <p className="text-sm text-muted-foreground">
                      Registre a estrutura sintética de planejamento que ajuda a auditar a cobertura conceitual do motor.
                    </p>
                  </div>
                </div>

                <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
                  <CampoBooleano
                    label="Perfil financeiro configurado"
                    checked={formulario.possuiPerfilFinanceiroConfigurado}
                    onCheckedChange={(valor) => atualizarCampo("possuiPerfilFinanceiroConfigurado", valor)}
                  />
                  <CampoBooleano
                    label="Plano estratégico"
                    checked={formulario.possuiPlanoEstrategico}
                    onCheckedChange={(valor) => atualizarCampo("possuiPlanoEstrategico", valor)}
                  />
                  <CampoBooleano
                    label="Metas"
                    checked={formulario.possuiMetas}
                    onCheckedChange={(valor) => atualizarCampo("possuiMetas", valor)}
                  />
                  <CampoBooleano
                    label="Compromissos"
                    checked={formulario.possuiCompromissos}
                    onCheckedChange={(valor) => atualizarCampo("possuiCompromissos", valor)}
                  />
                  <CampoBooleano
                    label="Inadimplência"
                    checked={formulario.possuiInadimplencia}
                    onCheckedChange={(valor) => atualizarCampo("possuiInadimplencia", valor)}
                  />
                  <CampoNumero
                    label="Compromissos cumpridos"
                    value={formulario.compromissosCumpridos}
                    onChange={(v) => atualizarCampo("compromissosCumpridos", v)}
                    inteiro
                  />
                </div>

                <div className="border-t pt-6">
                  <div className="space-y-2">
                    <h3 className="text-base font-semibold">4. Avaliação humana</h3>
                    <p className="text-sm text-muted-foreground">
                      Use este bloco para registrar a leitura humana esperada antes de promover a persona a caso canônico. O MF Score final usa escala de 0 a 1000 e os pilares continuam em 0 a 100.
                    </p>
                  </div>
                </div>

                <div className="grid gap-4 md:grid-cols-3">
                  <CampoNumero
                    label="Score humano sugerido (0 a 1000)"
                    value={formulario.scoreHumanoSugerido}
                    onChange={(v) => atualizarCampo("scoreHumanoSugerido", v)}
                    inteiro
                  />
                  <CampoNumero
                    label="Faixa esperada mínima (0 a 1000)"
                    value={formulario.faixaEsperadaMin}
                    onChange={(v) => atualizarCampo("faixaEsperadaMin", v)}
                    inteiro
                  />
                  <CampoNumero
                    label="Faixa esperada máxima (0 a 1000)"
                    value={formulario.faixaEsperadaMax}
                    onChange={(v) => atualizarCampo("faixaEsperadaMax", v)}
                    inteiro
                  />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium">Justificativa da nota humana</label>
                  <Textarea
                    value={formulario.justificativaNotaHumana}
                    onChange={(e) => atualizarCampo("justificativaNotaHumana", e.target.value)}
                    className="min-h-[110px]"
                  />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium">Observações</label>
                  <Textarea
                    value={formulario.observacoes}
                    onChange={(e) => atualizarCampo("observacoes", e.target.value)}
                    className="min-h-[100px]"
                  />
                </div>

                <div className="flex flex-wrap gap-3 border-t pt-6">
                  <Button onClick={() => void salvarPersona()} disabled={salvando}>
                    {salvando ? "Salvando..." : personaSelecionada ? "Salvar alterações" : "Criar persona"}
                  </Button>
                  <Button
                    variant="outline"
                    onClick={() => (personaSelecionada ? void executarScore(personaSelecionada.id) : undefined)}
                    disabled={!personaSelecionada || rodandoScore}
                  >
                    <Gauge className="mr-2 h-4 w-4" />
                    {rodandoScore ? "Executando..." : "Rodar MF Score"}
                  </Button>
                </div>
              </CardContent>
            </Card>
          </section>

          <Card>
            <CardHeader>
              <CardTitle>5. Resultado do motor</CardTitle>
              <CardDescription>
                Após rodar o score, esta área mostra o resultado calculado pelo motor oficial. O MF Score aparece em escala 0 a 1000 e os pilares continuam em escala 0 a 100.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {!resultado ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Selecione uma persona salva e clique em <strong>Rodar MF Score</strong> para visualizar score, pilares,
                  penalizações e comparação humana.
                </div>
              ) : (
                <div className="space-y-6">
                  <div className="grid gap-4 md:grid-cols-4">
                    <ResumoResultado titulo="MF Score base" valor={String(resultado.mfScoreBase)} />
                    <ResumoResultado titulo="MF Score final" valor={String(resultado.mfScoreFinal)} />
                    <ResumoResultado titulo="Classificação" valor={resultado.classificacao} />
                    <ResumoResultado titulo="Risco" valor={resultado.risco} />
                  </div>

                  <div className="grid gap-4 md:grid-cols-3">
                    <ResumoResultado titulo="Penalidade total" valor={String(resultado.penalidadeTotal)} />
                    <ResumoResultado
                      titulo="Score humano"
                      valor={resultado.scoreHumanoSugerido != null ? String(resultado.scoreHumanoSugerido) : "Não informado"}
                    />
                    <ResumoResultado
                      titulo="Faixa esperada"
                      valor={
                        resultado.faixaEsperadaMin != null && resultado.faixaEsperadaMax != null
                          ? `${resultado.faixaEsperadaMin} - ${resultado.faixaEsperadaMax}`
                          : "Não informada"
                      }
                    />
                  </div>

                  {resultado.observacaoComparativa ? (
                    <div className="rounded-lg border bg-muted/30 px-4 py-3 text-sm text-muted-foreground">
                      {resultado.observacaoComparativa}
                    </div>
                  ) : null}

                  <div className="grid gap-6 xl:grid-cols-2">
                    <div className="space-y-3">
                      <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Notas dos pilares</h3>
                      <div className="overflow-hidden rounded-lg border">
                        <Table>
                          <TableHeader>
                            <TableRow>
                              <TableHead>Pilar</TableHead>
                              <TableHead>Nota</TableHead>
                              <TableHead>Peso</TableHead>
                            </TableRow>
                          </TableHeader>
                          <TableBody>
                            {resultado.pilares.map((pilar) => (
                              <TableRow key={pilar.pilar}>
                                <TableCell>
                                  <div className="space-y-1">
                                    <p className="font-medium">{pilar.pilar}</p>
                                    <p className="text-xs text-muted-foreground">{pilar.descricao}</p>
                                  </div>
                                </TableCell>
                                <TableCell>{pilar.nota}</TableCell>
                                <TableCell>{pilar.peso}%</TableCell>
                              </TableRow>
                            ))}
                          </TableBody>
                        </Table>
                      </div>
                    </div>

                    <div className="space-y-3">
                      <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Indicadores críticos e penalizações</h3>
                      <div className="overflow-hidden rounded-lg border">
                        <Table>
                          <TableHeader>
                            <TableRow>
                              <TableHead>Indicador</TableHead>
                              <TableHead>Pilar</TableHead>
                              <TableHead>Penalidade</TableHead>
                            </TableRow>
                          </TableHeader>
                          <TableBody>
                            {resultado.indicadoresCriticos.length === 0 ? (
                              <TableRow>
                                <TableCell colSpan={3} className="text-sm text-muted-foreground">
                                  Nenhum indicador crítico foi acionado para esta persona.
                                </TableCell>
                              </TableRow>
                            ) : (
                              resultado.indicadoresCriticos.map((indicador) => (
                                <TableRow key={`${indicador.indicador}-${indicador.motivo}`}>
                                  <TableCell>
                                    <div className="space-y-1">
                                      <p className="font-medium">{indicador.indicador}</p>
                                      <p className="text-xs text-muted-foreground">{indicador.motivo}</p>
                                    </div>
                                  </TableCell>
                                  <TableCell>{indicador.pilarRelacionado}</TableCell>
                                  <TableCell>{indicador.penalidade}</TableCell>
                                </TableRow>
                              ))
                            )}
                          </TableBody>
                        </Table>
                      </div>
                    </div>
                  </div>

                  <div className="space-y-3">
                    <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Regras críticas aplicadas</h3>
                    {resultado.penalizacoesAplicadas.length === 0 ? (
                      <div className="rounded-lg border border-dashed px-4 py-6 text-sm text-muted-foreground">
                        Nenhuma regra crítica foi aplicada neste cenário.
                      </div>
                    ) : (
                      <ul className="space-y-2 rounded-lg border px-4 py-4 text-sm">
                        {resultado.penalizacoesAplicadas.map((item) => (
                          <li key={item}>• {item}</li>
                        ))}
                      </ul>
                    )}
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </main>

      <AlertDialog open={personaParaInativar !== null} onOpenChange={(open) => !open && setPersonaParaInativar(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Inativar persona</AlertDialogTitle>
            <AlertDialogDescription>
              A persona continuará registrada para rastreabilidade, mas deixará de ser tratada como cenário ativo de calibração.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={() => void confirmarInativacao()}>Confirmar</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}

function CampoNumero({
  label,
  value,
  onChange,
  inteiro = false,
}: {
  label: string;
  value: string;
  onChange: (valor: string) => void;
  inteiro?: boolean;
}) {
  return (
    <div className="space-y-2">
      <label className="text-sm font-medium">{label}</label>
      <Input
        type="number"
        step={inteiro ? "1" : "0.01"}
        value={value}
        onChange={(event) => onChange(event.target.value)}
      />
    </div>
  );
}

function CampoBooleano({
  label,
  checked,
  onCheckedChange,
}: {
  label: string;
  checked: boolean;
  onCheckedChange: (valor: boolean) => void;
}) {
  return (
    <div className="flex items-center justify-between rounded-lg border px-4 py-3">
      <div className="space-y-1">
        <p className="text-sm font-medium">{label}</p>
      </div>
      <Switch checked={checked} onCheckedChange={onCheckedChange} />
    </div>
  );
}

function ResumoResultado({ titulo, valor }: { titulo: string; valor: string }) {
  return (
    <div className="rounded-lg border px-4 py-4">
      <p className="text-sm text-muted-foreground">{titulo}</p>
      <p className="mt-2 text-2xl font-semibold tracking-tight">{valor}</p>
    </div>
  );
}
