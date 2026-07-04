"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { FiltroLancamentosParams, LancamentoResumo } from "@/types/lancamentos";
import { buscarLancamentos, deletarLancamento } from "@/services/api/lancamentos";
import { buscarCategorias } from "@/services/api/categories";
import { buscarCartoes, buscarContas } from "@/services/api/finance";
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
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
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
import { CategoriaResumo } from "@/types/categories";
import { CartaoResumo, ContaResumo } from "@/types/finance";

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
  const [categorias, setCategorias] = useState<CategoriaResumo[]>([]);
  const [contas, setContas] = useState<ContaResumo[]>([]);
  const [cartoes, setCartoes] = useState<CartaoResumo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isDeleting, setIsDeleting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [selectedLancamentoId, setSelectedLancamentoId] = useState<string | null>(null);
  const [editOpen, setEditOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<LancamentoResumo | null>(null);
  const [tipoFiltro, setTipoFiltro] = useState("all");
  const [categoriaFiltro, setCategoriaFiltro] = useState("all");
  const [contaFiltro, setContaFiltro] = useState("all");
  const [cartaoFiltro, setCartaoFiltro] = useState("all");
  const [statusFiltro, setStatusFiltro] = useState("all");
  const [dataInicialFiltro, setDataInicialFiltro] = useState("");
  const [dataFinalFiltro, setDataFinalFiltro] = useState("");
  const [buscaDescricao, setBuscaDescricao] = useState("");
  const [ordenarPor, setOrdenarPor] = useState<"data" | "valor">("data");
  const [direcaoOrdenacao, setDirecaoOrdenacao] = useState<"asc" | "desc">("desc");
  const [paginaAtual, setPaginaAtual] = useState(1);
  const [totalPaginas, setTotalPaginas] = useState(1);
  const [totalItens, setTotalItens] = useState(0);
  const [tamanhoPagina, setTamanhoPagina] = useState(10);

  const filtrosAtuais = useMemo<FiltroLancamentosParams>(
    () => ({
      buscaDescricao,
      tipo: tipoFiltro !== "all" ? tipoFiltro : undefined,
      categoriaId: categoriaFiltro !== "all" ? categoriaFiltro : undefined,
      contaId: contaFiltro !== "all" ? contaFiltro : undefined,
      cartaoId: cartaoFiltro !== "all" ? cartaoFiltro : undefined,
      realizado:
        statusFiltro === "all" ? undefined : statusFiltro === "realizado" ? "true" : "false",
      dataInicial: dataInicialFiltro || undefined,
      dataFinal: dataFinalFiltro || undefined,
      ordenarPor,
      direcao: direcaoOrdenacao,
      pagina: paginaAtual,
      tamanhoPagina,
    }),
    [
      buscaDescricao,
      categoriaFiltro,
      contaFiltro,
      cartaoFiltro,
      dataFinalFiltro,
      dataInicialFiltro,
      direcaoOrdenacao,
      ordenarPor,
      paginaAtual,
      statusFiltro,
      tamanhoPagina,
      tipoFiltro,
    ]
  );

  const carregarLancamentos = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const [lancamentosResponse, categoriasResponse, contasResponse, cartoesResponse] =
        await Promise.all([
          buscarLancamentos(session.usuario.id, session.token, filtrosAtuais),
          buscarCategorias(session.usuario.id, session.token),
          buscarContas(session.usuario.id, session.token).catch(() => ({ dados: [] })),
          buscarCartoes(session.usuario.id, session.token).catch(() => ({ dados: [] })),
        ]);

      const dadosPaginados = lancamentosResponse.dados;
      const dadosNormalizados = Array.isArray(dadosPaginados)
        ? {
            itens: dadosPaginados,
            paginaAtual: 1,
            tamanhoPagina: dadosPaginados.length || tamanhoPagina,
            totalItens: dadosPaginados.length,
            totalPaginas: 1,
          }
        : dadosPaginados;

      setLancamentos(ordenarLancamentos(dadosNormalizados?.itens ?? []));
      setTotalPaginas(dadosNormalizados?.totalPaginas ?? 1);
      setTotalItens(dadosNormalizados?.totalItens ?? 0);
      setPaginaAtual(dadosNormalizados?.paginaAtual ?? 1);
      setCategorias(categoriasResponse.dados ?? []);
      setContas(contasResponse.dados ?? []);
      setCartoes(cartoesResponse.dados ?? []);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel carregar os lancamentos.");
      }
    } finally {
      setIsLoading(false);
    }
  }, [filtrosAtuais, session?.token, session?.usuario.id, tamanhoPagina]);

  useEffect(() => {
    carregarLancamentos();
  }, [carregarLancamentos]);

  const resumo = useMemo(() => {
    return lancamentos.reduce(
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
  }, [lancamentos]);

  const categoriasDisponiveis = useMemo(() => {
    return [...categorias]
      .sort((a, b) => a.nomeCategoria.localeCompare(b.nomeCategoria))
      .map((categoria) => ({ id: categoria.id, nome: categoria.nomeCategoria }));
  }, [categorias]);

  const contasDisponiveis = useMemo(() => {
    return [...contas]
      .sort((a, b) => a.nomeConta.localeCompare(b.nomeConta))
      .map((conta) => ({ id: conta.id, nome: `${conta.nomeConta} - ${conta.instituicao}` }));
  }, [contas]);

  const cartoesDisponiveis = useMemo(() => {
    return [...cartoes]
      .sort((a, b) => a.nomeCartao.localeCompare(b.nomeCartao))
      .map((cartao) => ({
        id: cartao.id,
        nome: `${cartao.nomeCartao} - ${cartao.instituicao}`,
      }));
  }, [cartoes]);

  useEffect(() => {
    setPaginaAtual(1);
  }, [
    buscaDescricao,
    tipoFiltro,
    categoriaFiltro,
    contaFiltro,
    cartaoFiltro,
    statusFiltro,
    dataInicialFiltro,
    dataFinalFiltro,
    ordenarPor,
    direcaoOrdenacao,
    tamanhoPagina,
  ]);

  function limparFiltros() {
    setTipoFiltro("all");
    setCategoriaFiltro("all");
    setContaFiltro("all");
    setCartaoFiltro("all");
    setStatusFiltro("all");
    setDataInicialFiltro("");
    setDataFinalFiltro("");
    setBuscaDescricao("");
    setOrdenarPor("data");
    setDirecaoOrdenacao("desc");
    setPaginaAtual(1);
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
                <CardTitle className="text-3xl">{totalItens}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Receitas na pagina</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.receitas)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Despesas na pagina</CardDescription>
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
              <div className="mb-6 grid gap-4 md:grid-cols-2 xl:grid-cols-6">
                <div className="space-y-2 xl:col-span-2">
                  <p className="text-sm font-medium">Busca por descricao</p>
                  <Input
                    type="text"
                    value={buscaDescricao}
                    onChange={(event) => setBuscaDescricao(event.target.value)}
                    placeholder="Ex: mercado, salario, freelance"
                  />
                </div>

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

              <div className="mb-6 grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                <div className="space-y-2">
                  <p className="text-sm font-medium">Conta</p>
                  <Select
                    value={contaFiltro}
                    onValueChange={(value) => {
                      setContaFiltro(value);
                      if (value !== "all") {
                        setCartaoFiltro("all");
                      }
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Todas as contas" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">Todas as contas</SelectItem>
                      {contasDisponiveis.map((conta) => (
                        <SelectItem key={conta.id} value={conta.id}>
                          {conta.nome}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <p className="text-sm font-medium">Cartao</p>
                  <Select
                    value={cartaoFiltro}
                    onValueChange={(value) => {
                      setCartaoFiltro(value);
                      if (value !== "all") {
                        setContaFiltro("all");
                      }
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Todos os cartoes" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">Todos os cartoes</SelectItem>
                      {cartoesDisponiveis.map((cartao) => (
                        <SelectItem key={cartao.id} value={cartao.id}>
                          {cartao.nome}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <p className="text-sm font-medium">Ordenar por</p>
                  <Select value={ordenarPor} onValueChange={(value) => setOrdenarPor(value as "data" | "valor")}>
                    <SelectTrigger>
                      <SelectValue placeholder="Escolha a ordenacao" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="data">Data</SelectItem>
                      <SelectItem value="valor">Valor</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <p className="text-sm font-medium">Direcao</p>
                  <Select
                    value={direcaoOrdenacao}
                    onValueChange={(value) => setDirecaoOrdenacao(value as "asc" | "desc")}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Escolha a direcao" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="desc">Decrescente</SelectItem>
                      <SelectItem value="asc">Crescente</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
                <p className="text-sm text-muted-foreground">
                  {totalItens} resultado(s) encontrado(s).
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
              ) : totalItens === 0 ? (
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
                    {lancamentos.map((lancamento) => (
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

              {!isLoading && totalItens > 0 ? (
                <div className="mt-6 flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                  <div className="flex items-center gap-3">
                    <span className="text-sm text-muted-foreground">Itens por pagina</span>
                    <Select
                      value={String(tamanhoPagina)}
                      onValueChange={(value) => setTamanhoPagina(Number(value))}
                    >
                      <SelectTrigger className="w-24">
                        <SelectValue placeholder="Quantidade" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="10">10</SelectItem>
                        <SelectItem value="20">20</SelectItem>
                        <SelectItem value="30">30</SelectItem>
                        <SelectItem value="40">40</SelectItem>
                        <SelectItem value="50">50</SelectItem>
                        <SelectItem value="100">100</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <Pagination className="mx-0 w-auto justify-start md:justify-end">
                    <PaginationContent>
                      <PaginationItem>
                        <PaginationPrevious
                          href="#"
                          onClick={(event) => {
                            event.preventDefault();
                            if (paginaAtual > 1) {
                              setPaginaAtual((current) => current - 1);
                            }
                          }}
                          className={paginaAtual <= 1 ? "pointer-events-none opacity-50" : ""}
                        />
                      </PaginationItem>

                      {Array.from({ length: totalPaginas }, (_, index) => index + 1)
                        .filter((pagina) => {
                          if (totalPaginas <= 5) {
                            return true;
                          }

                          return (
                            pagina === 1 ||
                            pagina === totalPaginas ||
                            Math.abs(pagina - paginaAtual) <= 1
                          );
                        })
                        .map((pagina, index, paginas) => {
                          const paginaAnterior = paginas[index - 1];

                          return (
                            <div key={pagina} className="flex items-center">
                              {paginaAnterior && pagina - paginaAnterior > 1 ? (
                                <PaginationItem>
                                  <span className="px-2 text-sm text-muted-foreground">...</span>
                                </PaginationItem>
                              ) : null}
                              <PaginationItem>
                                <PaginationLink
                                  href="#"
                                  isActive={paginaAtual === pagina}
                                  onClick={(event) => {
                                    event.preventDefault();
                                    setPaginaAtual(pagina);
                                  }}
                                >
                                  {pagina}
                                </PaginationLink>
                              </PaginationItem>
                            </div>
                          );
                        })}

                      <PaginationItem>
                        <PaginationNext
                          href="#"
                          onClick={(event) => {
                            event.preventDefault();
                            if (paginaAtual < totalPaginas) {
                              setPaginaAtual((current) => current + 1);
                            }
                          }}
                          className={paginaAtual >= totalPaginas ? "pointer-events-none opacity-50" : ""}
                        />
                      </PaginationItem>
                    </PaginationContent>
                  </Pagination>
                </div>
              ) : null}
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
