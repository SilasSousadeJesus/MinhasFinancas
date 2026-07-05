"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { Plus, Sparkles, Trash2 } from "lucide-react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { SimulacaoFinanceiraResumo } from "@/types/simulacao-financeira";
import {
  criarSimulacaoFinanceira,
  inativarSimulacaoFinanceira,
  listarSimulacoesFinanceiras,
} from "@/services/api/simulacao-financeira";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
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
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Sidebar } from "@/components/Sidebar/Sidebar";
import { SimulacaoFinanceiraLoadingOverlay } from "./SimulacaoFinanceiraLoadingOverlay";

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function formatMonth(value?: string | null) {
  if (!value) {
    return "Não calculada";
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return date.toLocaleDateString("pt-BR", {
    month: "long",
    year: "numeric",
  });
}

function getTodayMonthDate() {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, "0")}-01T00:00:00`;
}

export function SimulacoesFinanceirasOverview() {
  const router = useRouter();
  const { session } = useAuth();
  const [simulacoes, setSimulacoes] = useState<SimulacaoFinanceiraResumo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<SimulacaoFinanceiraResumo | null>(null);
  const [novoNome, setNovoNome] = useState("");
  const [novaDescricao, setNovaDescricao] = useState("");

  async function carregarSimulacoes() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessão inválida. Faça login novamente.");
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");
      const response = await listarSimulacoesFinanceiras(session.usuario.id, session.token);
      setSimulacoes(response.dados ?? []);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar as simulações financeiras.");
      }
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void carregarSimulacoes();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [session?.token, session?.usuario.id]);

  async function handleCriarSimulacao() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessão inválida. Faça login novamente.");
      return;
    }

    if (!novoNome.trim()) {
      setErrorMessage("Informe um nome para a simulação.");
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");

      const response = await criarSimulacaoFinanceira(
        {
          usuarioId: session.usuario.id,
          nome: novoNome.trim(),
          descricao: novaDescricao.trim(),
          dataInicial: getTodayMonthDate(),
          quantidadeMeses: 12,
          acoes: [],
        },
        session.token
      );

      const simulacaoId = response.dados;
      setCreateDialogOpen(false);
      setNovoNome("");
      setNovaDescricao("");

      if (simulacaoId) {
        router.push(`/simulacoes-financeiras/${simulacaoId}`);
        return;
      }

      await carregarSimulacoes();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível criar a simulação financeira.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleInativarSimulacao() {
    if (!deleteTarget || !session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");
      await inativarSimulacaoFinanceira(session.usuario.id, deleteTarget.id, session.token);
      setDeleteTarget(null);
      await carregarSimulacoes();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível inativar a simulação financeira.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="relative flex-1 px-6 py-8 md:px-8">
        <SimulacaoFinanceiraLoadingOverlay
          visible={isLoading || isSubmitting}
          message={isLoading ? "Carregando simulações..." : "Salvando simulação..."}
        />

        <div className="mx-auto max-w-7xl space-y-6">
          <Card className="border-0 shadow-none">
            <CardHeader className="px-0 pt-0">
              <CardTitle className="text-3xl">Simulações Financeiras</CardTitle>
              <CardDescription className="mt-2 max-w-3xl text-base">
                Crie cenários hipotéticos para entender o impacto de decisões futuras sem alterar os dados reais do sistema.
              </CardDescription>
            </CardHeader>
          </Card>

          <div className="flex justify-end">
            <Button onClick={() => setCreateDialogOpen(true)}>
              <Plus className="mr-2 h-4 w-4" />
              Nova simulação
            </Button>
          </div>

          {errorMessage ? (
            <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
              {errorMessage}
            </div>
          ) : null}

          {isLoading ? (
            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
              {Array.from({ length: 3 }).map((_, index) => (
                <Card key={index} className="min-h-[220px] animate-pulse" />
              ))}
            </div>
          ) : simulacoes.length === 0 ? (
            <Card>
              <CardContent className="flex min-h-[220px] flex-col items-center justify-center gap-4 text-center">
                <div className="space-y-2">
                  <h2 className="text-xl font-semibold">Nenhuma simulação criada</h2>
                  <p className="max-w-xl text-sm text-muted-foreground">
                    Crie sua primeira simulação para testar compras parceladas, novas despesas mensais,
                    aumento de renda ou períodos sem entrada.
                  </p>
                </div>
                <Button onClick={() => setCreateDialogOpen(true)}>
                  <Plus className="mr-2 h-4 w-4" />
                  Criar primeira simulação
                </Button>
              </CardContent>
            </Card>
          ) : (
            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
              {simulacoes.map((simulacao) => (
                <Card
                  key={simulacao.id}
                  className="group border-border/60 transition-all hover:-translate-y-1 hover:shadow-lg"
                >
                  <CardHeader className="space-y-4">
                    <div className="flex items-start justify-between gap-4">
                      <div className="space-y-1">
                        <CardTitle className="text-2xl">{simulacao.nome}</CardTitle>
                        <CardDescription>
                          Início em {formatMonth(simulacao.dataInicial)}
                        </CardDescription>
                      </div>
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="text-muted-foreground hover:text-destructive"
                        onClick={() => setDeleteTarget(simulacao)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>

                    <div className="rounded-2xl border bg-muted/30 p-4 text-center">
                      <p className="text-xs uppercase tracking-[0.2em] text-muted-foreground">
                        Diferença acumulada
                      </p>
                      <p className="mt-3 text-2xl font-semibold">
                        {formatCurrency(simulacao.resultadoAtual?.diferencaAcumulada ?? 0)}
                      </p>
                    </div>
                  </CardHeader>

                  <CardContent className="space-y-4">
                    <div className="grid gap-2 text-sm text-muted-foreground">
                      <div className="flex items-center justify-between">
                        <span>Ações cadastradas</span>
                        <span className="font-medium text-foreground">{simulacao.quantidadeAcoes}</span>
                      </div>
                      <div className="flex items-center justify-between">
                        <span>Meses simulados</span>
                        <span className="font-medium text-foreground">{simulacao.quantidadeMeses}</span>
                      </div>
                      <div className="flex items-center justify-between">
                        <span>Saldo real acumulado</span>
                        <span className="font-medium text-foreground">
                          {formatCurrency(simulacao.resultadoAtual?.saldoRealAcumulado ?? 0)}
                        </span>
                      </div>
                      <div className="flex items-center justify-between">
                        <span>Saldo simulado acumulado</span>
                        <span className="font-medium text-foreground">
                          {formatCurrency(simulacao.resultadoAtual?.saldoSimuladoAcumulado ?? 0)}
                        </span>
                      </div>
                    </div>

                    <Button asChild className="w-full">
                      <Link href={`/simulacoes-financeiras/${simulacao.id}`}>
                        Abrir simulação
                      </Link>
                    </Button>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </div>

        <Dialog open={createDialogOpen} onOpenChange={setCreateDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Nova simulação</DialogTitle>
              <DialogDescription>
                Crie o cenário agora e depois cadastre as ações para visualizar o impacto mês a mês.
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Nome da simulação</label>
                <Input
                  value={novoNome}
                  onChange={(event) => setNovoNome(event.target.value)}
                  placeholder="Ex: compra do sofá, mudança de emprego"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Descrição</label>
                <Textarea
                  value={novaDescricao}
                  onChange={(event) => setNovaDescricao(event.target.value)}
                  placeholder="Descreva brevemente a decisão financeira que deseja testar"
                />
              </div>
            </div>

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setCreateDialogOpen(false)}>
                Cancelar
              </Button>
              <Button type="button" onClick={handleCriarSimulacao} disabled={isSubmitting}>
                {isSubmitting ? "Criando..." : "Criar simulação"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>

        <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Inativar simulação</AlertDialogTitle>
              <AlertDialogDescription>
                A simulação ficará fora da listagem principal, mas sem alterar nenhum lançamento real.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancelar</AlertDialogCancel>
              <AlertDialogAction onClick={handleInativarSimulacao} disabled={isSubmitting}>
                Inativar
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </main>
    </div>
  );
}
