"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { Plus, Trash2 } from "lucide-react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { ProjecaoResumo } from "@/types/projecao";
import { criarProjecao, excluirProjecao, listarProjecoes } from "@/services/api/projecao";
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
import { Switch } from "@/components/ui/switch";
import { ProjecaoLoadingOverlay } from "./ProjecaoLoadingOverlay";

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function formatMonth(value?: string | null) {
  if (!value) {
    return "Nao calculada";
  }

  const [year, month] = value.split("-");
  if (!year || !month) {
    return value;
  }

  return new Date(Number(year), Number(month) - 1, 1).toLocaleDateString("pt-BR", {
    month: "long",
    year: "numeric",
  });
}

function getTodayMonthDate() {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, "0")}-01T00:00:00`;
}

export function ProjecoesOverview() {
  const router = useRouter();
  const { session } = useAuth();
  const [projecoes, setProjecoes] = useState<ProjecaoResumo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<ProjecaoResumo | null>(null);
  const [novoNome, setNovoNome] = useState("");
  const [novaAtreladaADespesas, setNovaAtreladaADespesas] = useState(true);

  async function carregarProjecoes() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");
      const response = await listarProjecoes(session.usuario.id, session.token);
      setProjecoes(response.dados ?? []);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel carregar as projeçoes.");
      }
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void carregarProjecoes();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [session?.token, session?.usuario.id]);

  async function handleCriarProjecao() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      return;
    }

    if (!novoNome.trim()) {
      setErrorMessage("Informe um nome para a projeçao.");
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");

      const response = await criarProjecao(
        {
          nome: novoNome.trim(),
          usuarioId: session.usuario.id,
          dataInicial: getTodayMonthDate(),
          valorAcumuladoInicial: 0,
          valorObjetivo: 0,
          mesesLimite: 60,
          atreladaADespesas: novaAtreladaADespesas,
          rendas: [{ nome: "Salario principal", valorMensal: 0 }],
          rendasExtrasMensais: [],
          dividasManuaisMensais: [],
        },
        session.token
      );

      const projecaoId = response.dados;
      setCreateDialogOpen(false);
      setNovoNome("");
      setNovaAtreladaADespesas(true);

      if (projecaoId) {
        router.push(`/projecao/${projecaoId}`);
        return;
      }

      await carregarProjecoes();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel criar a projeçao.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleExcluirProjecao() {
    if (!deleteTarget || !session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");
      await excluirProjecao(session.usuario.id, deleteTarget.id, session.token);
      setDeleteTarget(null);
      await carregarProjecoes();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel excluir a projeçao.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="relative flex-1 px-6 py-8 md:px-8">
      <ProjecaoLoadingOverlay
        visible={isLoading || isSubmitting}
        message={isLoading ? "Carregando projeçoes..." : "Salvando projeçao..."}
      />

      <div className="mx-auto max-w-7xl space-y-6">
        <Card className="border-0 shadow-none">
          <CardHeader className="px-0 pt-0">
            <CardTitle className="text-3xl">Projeçoes</CardTitle>
            <CardDescription className="mt-2 max-w-3xl text-base">
              Crie cenarios independentes para simular objetivos diferentes e abra cada card
              para editar os dados completos da projecao.
            </CardDescription>
          </CardHeader>
        </Card>

        <div className="flex justify-end">
          <Button onClick={() => setCreateDialogOpen(true)}>
            <Plus className="mr-2 h-4 w-4" />
            Criar projeçoes
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
        ) : projecoes.length === 0 ? (
          <Card>
            <CardContent className="flex min-h-[220px] flex-col items-center justify-center gap-4 text-center">
              <div className="space-y-2">
                <h2 className="text-xl font-semibold">Nenhuma projeçao criada</h2>
                <p className="max-w-xl text-sm text-muted-foreground">
                  Crie sua primeira projeçao para acompanhar quando cada objetivo pode
                  ser alcancado com base nas suas rendas e nos lancamentos ja existentes.
                </p>
              </div>
              <Button onClick={() => setCreateDialogOpen(true)}>
                <Plus className="mr-2 h-4 w-4" />
                Criar primeira projeçao
              </Button>
            </CardContent>
          </Card>
        ) : (
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
            {projecoes.map((projecao) => (
              <Card
                key={projecao.id}
                className="group border-border/60 transition-all hover:-translate-y-1 hover:shadow-lg"
              >
                <CardHeader className="space-y-4">
                  <div className="flex items-start justify-between gap-4">
                    <div className="space-y-1">
                      <CardTitle className="text-2xl">{projecao.nome}</CardTitle>
                      <CardDescription>
                        Inicio em {formatMonth(projecao.dataInicial.slice(0, 7))}
                      </CardDescription>
                    </div>
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      className="text-muted-foreground hover:text-destructive"
                      onClick={() => setDeleteTarget(projecao)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>

                  <div className="rounded-2xl border bg-muted/30 p-4 text-center">
                    <p className="text-xs uppercase tracking-[0.2em] text-muted-foreground">
                      Objetivo
                    </p>
                    <p className="mt-3 text-2xl font-semibold">
                      {formatCurrency(projecao.valorObjetivo)}
                    </p>
                  </div>
                </CardHeader>

                <CardContent className="space-y-4">
                  <div className="grid gap-2 text-sm text-muted-foreground">
                    <div className="flex items-center justify-between">
                      <span>Acumulado inicial</span>
                      <span className="font-medium text-foreground">
                        {formatCurrency(projecao.valorAcumuladoInicial)}
                      </span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span>Atrelada a despesas</span>
                      <span className="font-medium text-foreground">
                        {projecao.atreladaADespesas ? "Sim" : "Nao"}
                      </span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span>Rendas cadastradas</span>
                      <span className="font-medium text-foreground">{projecao.quantidadeRendas}</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span>Meses simulados</span>
                      <span className="font-medium text-foreground">{projecao.mesesLimite}</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span>Status</span>
                      <span className="text-right font-medium text-foreground">
                        {projecao.resultadoAtual
                          ? projecao.resultadoAtual.objetivoAlcancado
                            ? `Atinge em ${formatMonth(projecao.resultadoAtual.mesObjetivo)}`
                            : `Falta ${formatCurrency(projecao.resultadoAtual.valorRestanteParaObjetivo)}`
                          : "Complete a configuraçao"}
                      </span>
                    </div>
                  </div>

                  <Button asChild className="w-full">
                    <Link href={`/projecao/${projecao.id}`}>Abrir projeçao</Link>
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
            <DialogTitle>Nova projeçao</DialogTitle>
            <DialogDescription>
              Crie o card agora e depois configure rendas, objetivo e acumulado na tela detalhada.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Nome da projeçao</label>
              <Input
                value={novoNome}
                onChange={(event) => setNovoNome(event.target.value)}
                placeholder="Ex: Reserva de emergencia, carro, viagem"
              />
            </div>

            <div className="flex items-center justify-between rounded-xl border p-3">
              <div className="space-y-1">
                <p className="text-sm font-medium">Atrelada a despesas</p>
                <p className="text-xs text-muted-foreground">
                  Se desligar, a coluna de dividas sera preenchida manualmente na tabela.
                </p>
              </div>
              <Switch
                checked={novaAtreladaADespesas}
                onCheckedChange={setNovaAtreladaADespesas}
              />
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setCreateDialogOpen(false)}>
              Cancelar
            </Button>
            <Button type="button" onClick={handleCriarProjecao} disabled={isSubmitting}>
              {isSubmitting ? "Criando..." : "Criar projeçoes"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir projeçao</AlertDialogTitle>
            <AlertDialogDescription>
              Essa acao remove a projeçao e todas as rendas vinculadas a ela.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={handleExcluirProjecao} disabled={isSubmitting}>
              Excluir
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </main>
  );
}
