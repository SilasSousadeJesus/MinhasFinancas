"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { LancamentoResumo } from "@/types/lancamentos";
import { buscarLancamentos, deletarLancamento } from "@/services/api/lancamentos";
import { Sidebar } from "@/components/Sidebar/Sidebar";
import { NovoLancamentoModal } from "@/components/lancamentos/NovoLancamentoModal";
import { EditarLancamentoModal } from "@/components/lancamentos/EditarLancamentoModal";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
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

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR").format(new Date(value));
}

function getTipoLabel(tipo: number) {
  switch (tipo) {
    case 0:
      return "Despesa";
    case 1:
      return "Receita";
    case 2:
      return "Investimento";
    case 3:
      return "Saque investimento";
    case 4:
      return "Transferencia";
    case 5:
      return "Saque";
    case 6:
      return "Deposito";
    default:
      return "Outro";
  }
}

function getTipoVariant(tipo: number): "default" | "secondary" | "destructive" | "outline" {
  switch (tipo) {
    case 1:
      return "default";
    case 0:
      return "destructive";
    default:
      return "secondary";
  }
}

function ordenarLancamentos(lista: LancamentoResumo[]) {
  return [...lista].sort(
    (a, b) => new Date(b.dataLancamento).getTime() - new Date(a.dataLancamento).getTime()
  );
}

function toDateInputValue(dateValue: string) {
  return new Date(dateValue).toISOString().split("T")[0];
}

export function LancamentosManager() {
  const { session } = useAuth();
  const [lancamentos, setLancamentos] = useState<LancamentoResumo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isDeleting, setIsDeleting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [selectedLancamentoId, setSelectedLancamentoId] = useState<string | null>(null);
  const [editOpen, setEditOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<LancamentoResumo | null>(null);
  const [tipoFiltro, setTipoFiltro] = useState("all");
  const [categoriaFiltro, setCategoriaFiltro] = useState("all");
  const [statusFiltro, setStatusFiltro] = useState("all");
  const [dataInicialFiltro, setDataInicialFiltro] = useState("");
  const [dataFinalFiltro, setDataFinalFiltro] = useState("");

  const carregarLancamentos = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarLancamentos(session.usuario.id, session.token);
      setLancamentos(ordenarLancamentos(response.dados ?? []));
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel carregar os lancamentos.");
      }
    } finally {
      setIsLoading(false);
    }
  }, [session?.token, session?.usuario.id]);

  useEffect(() => {
    carregarLancamentos();
  }, [carregarLancamentos]);

  const categoriasDisponiveis = useMemo(() => {
    const mapa = new Map<string, string>();

    lancamentos.forEach((lancamento) => {
      if (lancamento.categoriaId && lancamento.categoria?.nomeCategoria) {
        mapa.set(lancamento.categoriaId, lancamento.categoria.nomeCategoria);
      }
    });

    return Array.from(mapa.entries())
      .map(([id, nome]) => ({ id, nome }))
      .sort((a, b) => a.nome.localeCompare(b.nome));
  }, [lancamentos]);

  const lancamentosFiltrados = useMemo(() => {
    return lancamentos.filter((lancamento) => {
      if (tipoFiltro !== "all" && String(lancamento.tipo) !== tipoFiltro) {
        return false;
      }

      if (categoriaFiltro !== "all" && lancamento.categoriaId !== categoriaFiltro) {
        return false;
      }

      if (statusFiltro === "realizado" && !lancamento.realizado) {
        return false;
      }

      if (statusFiltro === "pendente" && lancamento.realizado) {
        return false;
      }

      const dataLancamento = toDateInputValue(lancamento.dataLancamento);

      if (dataInicialFiltro && dataLancamento < dataInicialFiltro) {
        return false;
      }

      if (dataFinalFiltro && dataLancamento > dataFinalFiltro) {
        return false;
      }

      return true;
    });
  }, [categoriaFiltro, dataFinalFiltro, dataInicialFiltro, lancamentos, statusFiltro, tipoFiltro]);

  const resumo = useMemo(() => {
    return lancamentosFiltrados.reduce(
      (acc, lancamento) => {
        if (lancamento.tipo === 1) {
          acc.receitas += lancamento.valor;
        }

        if (lancamento.tipo === 0) {
          acc.despesas += lancamento.valor;
        }

        return acc;
      },
      { receitas: 0, despesas: 0 }
    );
  }, [lancamentosFiltrados]);

  function limparFiltros() {
    setTipoFiltro("all");
    setCategoriaFiltro("all");
    setStatusFiltro("all");
    setDataInicialFiltro("");
    setDataFinalFiltro("");
  }

  async function confirmarExclusao() {
    if (!session?.usuario.id || !session.token || !deleteTarget) {
      return;
    }

    try {
      setIsDeleting(true);
      setErrorMessage("");
      setSuccessMessage("");

      await deletarLancamento(session.usuario.id, deleteTarget.id, session.token);
      setDeleteTarget(null);
      setSuccessMessage("Lancamento excluido com sucesso.");
      await carregarLancamentos();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel excluir o lancamento.");
      }
    } finally {
      setIsDeleting(false);
    }
  }

  function abrirEdicao(lancamentoId: string) {
    setSelectedLancamentoId(lancamentoId);
    setEditOpen(true);
  }

  return (
    <div className="flex flex-row">
      <Sidebar />
      <div className="flex-1 px-6 py-8 md:px-8">
        <div className="mx-auto max-w-6xl space-y-6">
          <Card className="border-0 shadow-none">
            <CardHeader className="px-0 pt-0">
              <div className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
                <div>
                  <CardTitle className="text-3xl">Lancamentos</CardTitle>
                  <CardDescription className="mt-2 max-w-2xl text-base">
                    Visualize, ajuste e remova os lancamentos criados. Essa tela fecha o
                    ciclo iniciado no modal de novo lancamento.
                  </CardDescription>
                </div>
                <NovoLancamentoModal onCreated={carregarLancamentos} />
              </div>
            </CardHeader>
          </Card>

          <div className="grid gap-4 md:grid-cols-3">
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Total filtrado de lancamentos</CardDescription>
                <CardTitle className="text-3xl">{lancamentosFiltrados.length}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Receitas filtradas</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.receitas)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Despesas filtradas</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.despesas)}</CardTitle>
              </CardHeader>
            </Card>
          </div>

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
              <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                <div>
                  <CardTitle>Lista de lancamentos</CardTitle>
                  <CardDescription>
                    Filtre, edite e exclua os itens ja registrados no sistema.
                  </CardDescription>
                </div>
                <Button variant="outline" onClick={carregarLancamentos}>
                  Atualizar lista
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              <div className="mb-6 grid gap-4 md:grid-cols-2 xl:grid-cols-5">
                <div className="space-y-2">
                  <p className="text-sm font-medium">Tipo</p>
                  <Select value={tipoFiltro} onValueChange={setTipoFiltro}>
                    <SelectTrigger>
                      <SelectValue placeholder="Todos os tipos" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">Todos os tipos</SelectItem>
                      <SelectItem value="1">Receita</SelectItem>
                      <SelectItem value="0">Despesa</SelectItem>
                      <SelectItem value="2">Investimento</SelectItem>
                      <SelectItem value="4">Transferencia</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <p className="text-sm font-medium">Categoria</p>
                  <Select value={categoriaFiltro} onValueChange={setCategoriaFiltro}>
                    <SelectTrigger>
                      <SelectValue placeholder="Todas as categorias" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">Todas as categorias</SelectItem>
                      {categoriasDisponiveis.map((categoria) => (
                        <SelectItem key={categoria.id} value={categoria.id}>
                          {categoria.nome}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <p className="text-sm font-medium">Status</p>
                  <Select value={statusFiltro} onValueChange={setStatusFiltro}>
                    <SelectTrigger>
                      <SelectValue placeholder="Todos os status" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">Todos os status</SelectItem>
                      <SelectItem value="realizado">Realizado</SelectItem>
                      <SelectItem value="pendente">Pendente</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <p className="text-sm font-medium">Data inicial</p>
                  <Input type="date" value={dataInicialFiltro} onChange={(event) => setDataInicialFiltro(event.target.value)} />
                </div>

                <div className="space-y-2">
                  <p className="text-sm font-medium">Data final</p>
                  <Input type="date" value={dataFinalFiltro} onChange={(event) => setDataFinalFiltro(event.target.value)} />
                </div>
              </div>

              <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
                <p className="text-sm text-muted-foreground">
                  {lancamentosFiltrados.length} resultado(s) encontrado(s).
                </p>
                <Button variant="ghost" onClick={limparFiltros}>
                  Limpar filtros
                </Button>
              </div>

              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando lancamentos...
                </div>
              ) : lancamentos.length === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                  <Plus className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                  <p className="text-sm text-muted-foreground">
                    Ainda nao existem lancamentos cadastrados para esta conta.
                  </p>
                </div>
              ) : lancamentosFiltrados.length === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                  <p className="text-sm text-muted-foreground">
                    Nenhum lancamento encontrado com os filtros atuais.
                  </p>
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Descricao</TableHead>
                      <TableHead>Tipo</TableHead>
                      <TableHead>Categoria</TableHead>
                      <TableHead>Valor</TableHead>
                      <TableHead>Pagamento</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead className="text-right">Acoes</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {lancamentosFiltrados.map((lancamento) => (
                      <TableRow key={lancamento.id}>
                        <TableCell>
                          <div>
                            <p className="font-medium">{lancamento.descricao}</p>
                            <p className="text-xs text-muted-foreground">
                              Lancado em {formatDate(lancamento.dataLancamento)}
                            </p>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant={getTipoVariant(lancamento.tipo)}>
                            {getTipoLabel(lancamento.tipo)}
                          </Badge>
                        </TableCell>
                        <TableCell>{lancamento.categoria?.nomeCategoria ?? "Sem categoria"}</TableCell>
                        <TableCell className="font-medium">{formatCurrency(lancamento.valor)}</TableCell>
                        <TableCell>{formatDate(lancamento.dataPagamento)}</TableCell>
                        <TableCell>
                          <Badge variant={lancamento.realizado ? "secondary" : "outline"}>
                            {lancamento.realizado ? "Realizado" : "Pendente"}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="flex justify-end gap-2">
                            <Button variant="ghost" size="icon" onClick={() => abrirEdicao(lancamento.id)}>
                              <Pencil className="h-4 w-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="text-destructive hover:text-destructive"
                              onClick={() => setDeleteTarget(lancamento)}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </div>
      </div>

      {session?.usuario.id && session?.token ? (
        <EditarLancamentoModal
          lancamentoId={selectedLancamentoId}
          open={editOpen}
          onOpenChange={setEditOpen}
          usuarioId={session.usuario.id}
          token={session.token}
          onSaved={async () => {
            setSuccessMessage("Lancamento atualizado com sucesso.");
            await carregarLancamentos();
          }}
        />
      ) : null}

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir lancamento</AlertDialogTitle>
            <AlertDialogDescription>
              {deleteTarget
                ? `Tem certeza que deseja excluir "${deleteTarget.descricao}"? Essa acao nao pode ser desfeita.`
                : ""}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmarExclusao} disabled={isDeleting}>
              {isDeleting ? "Excluindo..." : "Excluir"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
