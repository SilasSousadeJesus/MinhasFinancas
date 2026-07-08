"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { CheckCircle2, Handshake, Loader2, PenLine, Plus, Trash2, XCircle } from "lucide-react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import { ApiError } from "@/types/api";
import { useAuth } from "@/providers/auth-provider";
import { CompromissoFinanceiroItem, OrigemCompromissoFinanceiro, SalvarCompromissoFinanceiroPayload, StatusCompromissoFinanceiro } from "@/types/compromissos-financeiros";
import {
  cancelarCompromissoFinanceiro,
  cadastrarCompromissoFinanceiro,
  concluirCompromissoFinanceiro,
  editarCompromissoFinanceiro,
  excluirCompromissoFinanceiro,
  listarCompromissosFinanceiros,
} from "@/services/api/compromissos-financeiros";
import { CompromissoFinanceiroModal } from "./CompromissoFinanceiroModal";

type ModalMode = "create" | "edit";

function formatarDataHora(valor?: string | null) {
  if (!valor) {
    return "—";
  }

  const data = new Date(valor);
  if (Number.isNaN(data.getTime())) {
    return "—";
  }

  return data.toLocaleString("pt-BR", {
    dateStyle: "short",
    timeStyle: "short",
  });
}

function obterOrigemLabel(origem: OrigemCompromissoFinanceiro) {
  return origem === OrigemCompromissoFinanceiro.IA ? "IA" : "Manual";
}

function obterStatusLabel(status: StatusCompromissoFinanceiro) {
  switch (status) {
    case StatusCompromissoFinanceiro.Concluido:
      return "Concluído";
    case StatusCompromissoFinanceiro.Cancelado:
      return "Cancelado";
    default:
      return "Em andamento";
  }
}

function obterStatusBadgeVariant(status: StatusCompromissoFinanceiro): "default" | "secondary" | "destructive" | "outline" {
  switch (status) {
    case StatusCompromissoFinanceiro.Concluido:
      return "secondary";
    case StatusCompromissoFinanceiro.Cancelado:
      return "destructive";
    default:
      return "outline";
  }
}

export function CompromissosFinanceirosManager() {
  const { session } = useAuth();

  const [compromissos, setCompromissos] = useState<CompromissoFinanceiroItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<ModalMode>("create");
  const [selectedCompromisso, setSelectedCompromisso] = useState<CompromissoFinanceiroItem | null>(null);
  const [acaoPendente, setAcaoPendente] = useState<
    | { tipo: "concluir" | "cancelar" | "excluir"; compromisso: CompromissoFinanceiroItem }
    | null
  >(null);
  const [processandoId, setProcessandoId] = useState<string | null>(null);

  const carregarDados = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await listarCompromissosFinanceiros(session.usuario.id, session.token);
      setCompromissos(response.dados ?? []);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar os compromissos financeiros.");
      }
      setCompromissos([]);
    } finally {
      setIsLoading(false);
    }
  }, [session?.token, session?.usuario.id]);

  useEffect(() => {
    void carregarDados();
  }, [carregarDados]);

  const resumo = useMemo(() => {
    return compromissos.reduce(
      (acc, compromisso) => {
        if (compromisso.status === StatusCompromissoFinanceiro.EmAndamento) {
          acc.emAndamento += 1;
        } else if (compromisso.status === StatusCompromissoFinanceiro.Concluido) {
          acc.concluidos += 1;
        } else {
          acc.cancelados += 1;
        }

        if (compromisso.origem === OrigemCompromissoFinanceiro.IA) {
          acc.ia += 1;
        }

        return acc;
      },
      { emAndamento: 0, concluidos: 0, cancelados: 0, ia: 0 }
    );
  }, [compromissos]);

  function abrirNovo() {
    setModalMode("create");
    setSelectedCompromisso(null);
    setModalOpen(true);
  }

  function abrirEditar(compromisso: CompromissoFinanceiroItem) {
    setModalMode("edit");
    setSelectedCompromisso(compromisso);
    setModalOpen(true);
  }

  async function salvarCompromisso(payload: SalvarCompromissoFinanceiroPayload) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    setErrorMessage("");
    setSuccessMessage("");

    if (modalMode === "create") {
      await cadastrarCompromissoFinanceiro(
        session.usuario.id,
        { ...payload, usuarioId: session.usuario.id },
        session.token
      );
      setSuccessMessage("Compromisso financeiro criado com sucesso.");
    } else if (selectedCompromisso) {
      await editarCompromissoFinanceiro(
        session.usuario.id,
        selectedCompromisso.id,
        { ...payload, usuarioId: session.usuario.id },
        session.token
      );
      setSuccessMessage("Compromisso financeiro atualizado com sucesso.");
    }

    setModalOpen(false);
    await carregarDados();
  }

  async function executarAcao() {
    if (!session?.usuario.id || !session.token || !acaoPendente) {
      return;
    }

    try {
      setProcessandoId(acaoPendente.compromisso.id);
      setErrorMessage("");
      setSuccessMessage("");

      if (acaoPendente.tipo === "concluir") {
        await concluirCompromissoFinanceiro(session.usuario.id, acaoPendente.compromisso.id, session.token);
        setSuccessMessage("Compromisso concluído com sucesso.");
      } else if (acaoPendente.tipo === "cancelar") {
        await cancelarCompromissoFinanceiro(session.usuario.id, acaoPendente.compromisso.id, session.token);
        setSuccessMessage("Compromisso cancelado com sucesso.");
      } else {
        await excluirCompromissoFinanceiro(session.usuario.id, acaoPendente.compromisso.id, session.token);
        setSuccessMessage("Compromisso excluído com sucesso.");
      }

      setAcaoPendente(null);
      await carregarDados();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível concluir a ação solicitada.");
      }
    } finally {
      setProcessandoId(null);
    }
  }

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="flex-1 bg-gray-50 px-6 py-8 dark:bg-[#020817] md:px-8">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
          <section className="space-y-2">
            <h1 className="text-3xl font-semibold tracking-tight">Compromissos financeiros</h1>
            <p className="max-w-3xl text-sm text-muted-foreground">
              Registre decisões assumidas pelo usuário e acompanhe o que precisa ser mantido, concluído ou ajustado ao longo do tempo.
            </p>
          </section>

          <section className="grid gap-4 md:grid-cols-4">
            <Card>
              <CardHeader className="pb-2">
                <CardDescription>Em andamento</CardDescription>
                <CardTitle className="text-3xl">{resumo.emAndamento}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription>Concluídos</CardDescription>
                <CardTitle className="text-3xl">{resumo.concluidos}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription>Cancelados</CardDescription>
                <CardTitle className="text-3xl">{resumo.cancelados}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription>Origem IA</CardDescription>
                <CardTitle className="text-3xl">{resumo.ia}</CardTitle>
              </CardHeader>
            </Card>
          </section>

          {errorMessage ? (
            <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
              {errorMessage}
            </div>
          ) : null}

          {successMessage ? (
            <div className="rounded-md border border-emerald-500/20 bg-emerald-500/5 px-4 py-3 text-sm text-emerald-700 dark:text-emerald-300">
              {successMessage}
            </div>
          ) : null}

          <Card>
            <CardHeader>
              <div className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
                <div>
                  <CardTitle>Lista de compromissos</CardTitle>
                  <CardDescription>
                    Acompanhe as intenções assumidas e as ações que o Assistente Financeiro ajudou a estruturar.
                  </CardDescription>
                </div>
                <Button onClick={abrirNovo}>
                  <Plus className="mr-2 h-4 w-4" />
                  Novo compromisso
                </Button>
              </div>
            </CardHeader>

            <CardContent>
              {isLoading ? (
                <div className="flex items-center justify-center gap-2 rounded-lg border border-dashed px-6 py-10 text-sm text-muted-foreground">
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Carregando compromissos...
                </div>
              ) : compromissos.length === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                  <Handshake className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                  <p className="text-sm text-muted-foreground">
                    Ainda não existem compromissos registrados para este usuário.
                  </p>
                </div>
              ) : (
                <div className="overflow-hidden rounded-lg border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Descrição</TableHead>
                        <TableHead>Origem</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead>Criado em</TableHead>
                        <TableHead>Conclusão</TableHead>
                        <TableHead>Cancelamento</TableHead>
                        <TableHead className="text-right">Ações</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {compromissos.map((compromisso) => {
                        const desabilitado = processandoId === compromisso.id;
                        const podeConcluir = compromisso.status === StatusCompromissoFinanceiro.EmAndamento;

                        return (
                          <TableRow key={compromisso.id}>
                            <TableCell>
                              <div className="space-y-1">
                                <p className="font-medium">{compromisso.descricao}</p>
                                {compromisso.observacoes ? (
                                  <p className="max-w-xl text-xs text-muted-foreground">{compromisso.observacoes}</p>
                                ) : null}
                              </div>
                            </TableCell>
                            <TableCell>{obterOrigemLabel(compromisso.origem)}</TableCell>
                            <TableCell>
                              <Badge variant={obterStatusBadgeVariant(compromisso.status)}>
                                {obterStatusLabel(compromisso.status)}
                              </Badge>
                            </TableCell>
                            <TableCell className="text-sm">{formatarDataHora(compromisso.dataCriacao)}</TableCell>
                            <TableCell className="text-sm">{formatarDataHora(compromisso.dataConclusao)}</TableCell>
                            <TableCell className="text-sm">{formatarDataHora(compromisso.dataCancelamento)}</TableCell>
                            <TableCell>
                              <div className="flex justify-end gap-2">
                                <Button variant="ghost" size="icon" onClick={() => abrirEditar(compromisso)}>
                                  <PenLine className="h-4 w-4" />
                                </Button>
                                {podeConcluir ? (
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    onClick={() => setAcaoPendente({ tipo: "concluir", compromisso })}
                                    disabled={desabilitado}
                                  >
                                    <CheckCircle2 className="h-4 w-4" />
                                  </Button>
                                ) : null}
                                {compromisso.status !== StatusCompromissoFinanceiro.Cancelado ? (
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    onClick={() => setAcaoPendente({ tipo: "cancelar", compromisso })}
                                    disabled={desabilitado}
                                  >
                                    <XCircle className="h-4 w-4" />
                                  </Button>
                                ) : null}
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="text-destructive hover:text-destructive"
                                  onClick={() => setAcaoPendente({ tipo: "excluir", compromisso })}
                                  disabled={desabilitado}
                                >
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </div>
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
        </div>
      </main>

      <CompromissoFinanceiroModal
        open={modalOpen}
        onOpenChange={setModalOpen}
        mode={modalMode}
        initialData={selectedCompromisso}
        onSubmit={salvarCompromisso}
      />

      <AlertDialog open={acaoPendente !== null} onOpenChange={(open) => !open && setAcaoPendente(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              {acaoPendente?.tipo === "excluir"
                ? "Excluir compromisso"
                : acaoPendente?.tipo === "cancelar"
                  ? "Cancelar compromisso"
                  : "Concluir compromisso"}
            </AlertDialogTitle>
            <AlertDialogDescription>
              {acaoPendente?.tipo === "excluir"
                ? "Essa ação removerá o compromisso da lista."
                : acaoPendente?.tipo === "cancelar"
                  ? "Deseja cancelar este compromisso financeiro?"
                  : "Deseja marcar este compromisso como concluído?"}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Voltar</AlertDialogCancel>
            <AlertDialogAction onClick={() => void executarAcao()}>
              {processandoId ? "Processando..." : "Confirmar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
