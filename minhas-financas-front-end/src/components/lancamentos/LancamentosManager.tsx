"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Check, Pencil, Plus, Trash2 } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { FiltroLancamentosParams, LancamentoResumo } from "@/types/lancamentos";
import { buscarLancamentos, deletarLancamento, efetivarLancamento } from "@/services/api/lancamentos";
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

type FiltrosEdicaoLancamentos = {
  buscaDescricao: string;
  tipo: string;
  categoriaId: string;
  contaId: string;
  cartaoId: string;
  statusLancamento: string;
  dataInicialLancamento: string;
  dataFinalLancamento: string;
  dataInicialVencimento: string;
  dataFinalVencimento: string;
  dataInicialEfetivacao: string;
  dataFinalEfetivacao: string;
};

function criarFiltrosPadrao(): FiltrosEdicaoLancamentos {
  return {
    buscaDescricao: "",
    tipo: "all",
    categoriaId: "all",
    contaId: "all",
    cartaoId: "all",
    statusLancamento: "all",
    dataInicialLancamento: "",
    dataFinalLancamento: "",
    dataInicialVencimento: "",
    dataFinalVencimento: "",
    dataInicialEfetivacao: "",
    dataFinalEfetivacao: "",
  };
}

function converterFiltrosParaBusca(
  filtros: FiltrosEdicaoLancamentos,
  pagina: number,
  tamanhoPagina: number,
  ordenarPor: "data" | "valor",
  direcao: "asc" | "desc"
): FiltroLancamentosParams {
  return {
    buscaDescricao: filtros.buscaDescricao || undefined,
    tipo: filtros.tipo !== "all" ? filtros.tipo : undefined,
    categoriaId: filtros.categoriaId !== "all" ? filtros.categoriaId : undefined,
    contaId: filtros.contaId !== "all" ? filtros.contaId : undefined,
    cartaoId: filtros.cartaoId !== "all" ? filtros.cartaoId : undefined,
    statusLancamento: filtros.statusLancamento !== "all" ? filtros.statusLancamento : undefined,
    dataInicialLancamento: filtros.dataInicialLancamento || undefined,
    dataFinalLancamento: filtros.dataFinalLancamento || undefined,
    dataInicialVencimento: filtros.dataInicialVencimento || undefined,
    dataFinalVencimento: filtros.dataFinalVencimento || undefined,
    dataInicialEfetivacao: filtros.dataInicialEfetivacao || undefined,
    dataFinalEfetivacao: filtros.dataFinalEfetivacao || undefined,
    ordenarPor,
    direcao,
    pagina,
    tamanhoPagina,
  };
}

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

function getStatusLabel(status: number, tipo: number) {
  switch (status) {
    case 1:
      return "Pago";
    case 2:
      return "Recebido";
    case 3:
      return "Cancelado";
    default:
      return "Pendente";
  }
}

function getStatusVariant(status: number): "default" | "secondary" | "destructive" | "outline" {
  switch (status) {
    case 1:
    case 2:
      return "secondary";
    case 3:
      return "outline";
    default:
      return "destructive";
  }
}

function podeEfetivarRapido(lancamento: LancamentoResumo) {
  return lancamento.statusLancamento === 0 && (lancamento.tipo === 0 || lancamento.tipo === 1);
}

function getAcaoEfetivacaoLabel(lancamento: LancamentoResumo) {
  return lancamento.tipo === 0 ? "Pagar" : "Receber";
}

function toDateInputValue(dateValue: string) {
  return new Date(dateValue).toISOString().split("T")[0];
}

function ordenarLancamentosDaPagina(
  lista: LancamentoResumo[],
  ordenarPor: "data" | "valor",
  direcao: "asc" | "desc"
) {
  const multiplicador = direcao === "asc" ? 1 : -1;

  return [...lista].sort((a, b) => {
    if (ordenarPor === "valor") {
      if (a.valor !== b.valor) {
        return (a.valor - b.valor) * multiplicador;
      }
    } else {
      const dataVencimentoA = new Date(a.dataVencimento).getTime();
      const dataVencimentoB = new Date(b.dataVencimento).getTime();

      if (dataVencimentoA !== dataVencimentoB) {
        return (dataVencimentoA - dataVencimentoB) * multiplicador;
      }
    }

    const dataLancamentoA = new Date(a.dataLancamento).getTime();
    const dataLancamentoB = new Date(b.dataLancamento).getTime();

    if (dataLancamentoA !== dataLancamentoB) {
      return (dataLancamentoA - dataLancamentoB) * multiplicador;
    }

    return a.descricao.localeCompare(b.descricao, "pt-BR");
  });
}

export function LancamentosManager() {
  const { session } = useAuth();
  const ultimaRequisicaoRef = useRef(0);
  const ultimaRequisicaoDadosRef = useRef(0);
  const [lancamentos, setLancamentos] = useState<LancamentoResumo[]>([]);
  const [categorias, setCategorias] = useState<CategoriaResumo[]>([]);
  const [contas, setContas] = useState<ContaResumo[]>([]);
  const [cartoes, setCartoes] = useState<CartaoResumo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isDeleting, setIsDeleting] = useState(false);
  const [efetivandoId, setEfetivandoId] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [selectedLancamentoId, setSelectedLancamentoId] = useState<string | null>(null);
  const [editOpen, setEditOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<LancamentoResumo | null>(null);
  const [filtrosEmEdicao, setFiltrosEmEdicao] = useState<FiltrosEdicaoLancamentos>(
    criarFiltrosPadrao
  );
  const [filtrosAplicados, setFiltrosAplicados] = useState<FiltrosEdicaoLancamentos>(
    criarFiltrosPadrao
  );
  const [ordenarPor, setOrdenarPor] = useState<"data" | "valor">("data");
  const [direcaoOrdenacao, setDirecaoOrdenacao] = useState<"asc" | "desc">("desc");
  const [paginaAtual, setPaginaAtual] = useState(1);
  const [totalPaginas, setTotalPaginas] = useState(1);
  const [totalItens, setTotalItens] = useState(0);
  const [tamanhoPagina, setTamanhoPagina] = useState(10);

  const filtrosAtuais = useMemo<FiltroLancamentosParams>(
    () =>
      converterFiltrosParaBusca(
        filtrosAplicados,
        paginaAtual,
        tamanhoPagina,
        ordenarPor,
        direcaoOrdenacao
      ),
    [direcaoOrdenacao, filtrosAplicados, ordenarPor, paginaAtual, tamanhoPagina]
  );

  const carregarLancamentos = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    const requisicaoAtual = ++ultimaRequisicaoRef.current;

    try {
      setIsLoading(true);
      setErrorMessage("");

      const lancamentosResponse = await buscarLancamentos(
        session.usuario.id,
        session.token,
        filtrosAtuais
      );

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

      if (requisicaoAtual !== ultimaRequisicaoRef.current) {
        return;
      }

      const itensOrdenados = ordenarLancamentosDaPagina(
        dadosNormalizados?.itens ?? [],
        ordenarPor,
        direcaoOrdenacao
      );

      setLancamentos(itensOrdenados);
      setTotalPaginas(dadosNormalizados?.totalPaginas ?? 1);
      setTotalItens(dadosNormalizados?.totalItens ?? 0);
      setPaginaAtual(dadosNormalizados?.paginaAtual ?? 1);
    } catch (error) {
      if (requisicaoAtual !== ultimaRequisicaoRef.current) {
        return;
      }

      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar os lançamentos.");
      }
    } finally {
      if (requisicaoAtual === ultimaRequisicaoRef.current) {
        setIsLoading(false);
      }
    }
  }, [direcaoOrdenacao, filtrosAtuais, ordenarPor, session?.token, session?.usuario.id, tamanhoPagina]);

  const carregarDadosDeApoio = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    const requisicaoAtual = ++ultimaRequisicaoDadosRef.current;

    try {
      const [categoriasResponse, contasResponse, cartoesResponse] = await Promise.all([
        buscarCategorias(session.usuario.id, session.token),
        buscarContas(session.usuario.id, session.token).catch(() => ({ dados: [] })),
        buscarCartoes(session.usuario.id, session.token).catch(() => ({ dados: [] })),
      ]);

      if (requisicaoAtual !== ultimaRequisicaoDadosRef.current) {
        return;
      }

      setCategorias(categoriasResponse.dados ?? []);
      setContas(contasResponse.dados ?? []);
      setCartoes(cartoesResponse.dados ?? []);
    } catch (error) {
      if (requisicaoAtual !== ultimaRequisicaoDadosRef.current) {
        return;
      }

      if (error instanceof ApiError) {
        setErrorMessage((mensagemAtual) => mensagemAtual || error.message);
      }
    }
  }, [session?.token, session?.usuario.id]);

  useEffect(() => {
    carregarLancamentos();
  }, [carregarLancamentos]);

  useEffect(() => {
    carregarDadosDeApoio();
  }, [carregarDadosDeApoio]);

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

  function atualizarFiltroEmEdicao<K extends keyof FiltrosEdicaoLancamentos>(
    campo: K,
    valor: FiltrosEdicaoLancamentos[K]
  ) {
    setFiltrosEmEdicao((current) => ({
      ...current,
      [campo]: valor,
    }));
  }

  function aplicarFiltros() {
    setPaginaAtual(1);
    setFiltrosAplicados({ ...filtrosEmEdicao });
  }

  function limparFiltros() {
    const filtrosLimpos = criarFiltrosPadrao();
    setFiltrosEmEdicao(filtrosLimpos);
    setFiltrosAplicados(filtrosLimpos);
    setPaginaAtual(1);
  }

  function atualizarOrdenacao(value: "data" | "valor") {
    setOrdenarPor(value);
    setPaginaAtual(1);
  }

  function atualizarDirecao(value: "asc" | "desc") {
    setDirecaoOrdenacao(value);
    setPaginaAtual(1);
  }

  function atualizarTamanhoPagina(value: number) {
    setTamanhoPagina(value);
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
        setErrorMessage("Não foi possível excluir o lançamento.");
      }
    } finally {
      setIsDeleting(false);
    }
  }

  async function efetivarRapidamente(lancamento: LancamentoResumo) {
    if (!session?.usuario.id || !session.token || !podeEfetivarRapido(lancamento)) {
      return;
    }

    try {
      setEfetivandoId(lancamento.id);
      setErrorMessage("");
      setSuccessMessage("");

      await efetivarLancamento(session.usuario.id, lancamento.id, session.token);
      setSuccessMessage(
        lancamento.tipo === 0
          ? "Lancamento marcado como pago."
          : "Lancamento marcado como recebido."
      );
      await carregarLancamentos();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível efetivar o lançamento.");
      }
    } finally {
      setEfetivandoId(null);
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
                  <CardTitle className="text-3xl">Lançamentos</CardTitle>
                  <CardDescription className="mt-2 max-w-2xl text-base">
                    Visualize, ajuste e remova os lançamentos criados. Esta tela fecha o
                    ciclo iniciado no modal de novo lançamento.
                  </CardDescription>
                </div>
                <NovoLancamentoModal onCreated={carregarLancamentos} />
              </div>
            </CardHeader>
          </Card>

          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Total filtrado de lançamentos</CardDescription>
                <CardTitle className="text-3xl">{totalItens}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Receitas na página</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.receitas)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Despesas na página</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.despesas)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Saldo líquido da página</CardDescription>
                <CardTitle className="text-3xl">
                  {formatCurrency(resumo.receitas - resumo.despesas)}
                </CardTitle>
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
                  <CardTitle>Lista de lançamentos</CardTitle>
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
              <div className="mb-8 space-y-8">
                <section className="space-y-4">
                  <div className="space-y-1">
                    <h3 className="text-sm font-semibold tracking-wide text-foreground">
                      Pesquisa
                    </h3>
                    <p className="text-sm text-muted-foreground">
                      Local para encontrar rapidamente um lançamento específico.
                    </p>
                  </div>
                  <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
                    <div className="space-y-2 xl:col-span-2">
                      <p className="text-sm font-medium">Descrição</p>
                      <Input
                        type="text"
                        value={filtrosEmEdicao.buscaDescricao}
                        onChange={(event) =>
                          atualizarFiltroEmEdicao("buscaDescricao", event.target.value)
                        }
                        placeholder="Ex: mercado, salario, freelance"
                      />
                    </div>
                  </div>
                </section>

                <section className="space-y-4">
                  <div className="space-y-1">
                    <h3 className="text-sm font-semibold tracking-wide text-foreground">
                      Classificação
                    </h3>
                    <p className="text-sm text-muted-foreground">
                      Filtros ligados à natureza e ao enquadramento do lançamento.
                    </p>
                  </div>
                  <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
                    <div className="space-y-2">
                      <p className="text-sm font-medium">Tipo</p>
                      <Select
                        value={filtrosEmEdicao.tipo}
                        onValueChange={(value) => atualizarFiltroEmEdicao("tipo", value)}
                      >
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
                      <p className="text-sm font-medium">Status</p>
                      <Select
                        value={filtrosEmEdicao.statusLancamento}
                        onValueChange={(value) =>
                          atualizarFiltroEmEdicao("statusLancamento", value)
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Todos os status" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="all">Todos os status</SelectItem>
                          <SelectItem value="0">Pendente</SelectItem>
                          <SelectItem value="1">Pago</SelectItem>
                          <SelectItem value="2">Recebido</SelectItem>
                          <SelectItem value="3">Cancelado</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    <div className="space-y-2">
                      <p className="text-sm font-medium">Categoria</p>
                      <Select
                        value={filtrosEmEdicao.categoriaId}
                        onValueChange={(value) => atualizarFiltroEmEdicao("categoriaId", value)}
                      >
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
                      <p className="text-sm font-medium">Conta</p>
                      <Select
                        value={filtrosEmEdicao.contaId}
                        onValueChange={(value) => {
                          atualizarFiltroEmEdicao("contaId", value);
                          if (value !== "all") {
                            atualizarFiltroEmEdicao("cartaoId", "all");
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
                        value={filtrosEmEdicao.cartaoId}
                        onValueChange={(value) => {
                          atualizarFiltroEmEdicao("cartaoId", value);
                          if (value !== "all") {
                            atualizarFiltroEmEdicao("contaId", "all");
                          }
                        }}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Todos os cartões" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="all">Todos os cartões</SelectItem>
                          {cartoesDisponiveis.map((cartao) => (
                            <SelectItem key={cartao.id} value={cartao.id}>
                              {cartao.nome}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  </div>
                </section>

                <section className="space-y-4">
                  <div className="space-y-1">
                    <h3 className="text-sm font-semibold tracking-wide text-foreground">
                      Períodos
                    </h3>
                    <p className="text-sm text-muted-foreground">
                      Escolha com clareza qual janela temporal deseja analisar.
                    </p>
                  </div>

                  <div className="grid gap-6 xl:grid-cols-3">
                    <div className="space-y-3">
                      <h4 className="text-sm font-medium text-foreground">Período do lançamento</h4>
                      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-1">
                        <div className="space-y-2">
                          <p className="text-sm font-medium">Inicial</p>
                          <Input
                            type="date"
                            value={filtrosEmEdicao.dataInicialLancamento}
                            onChange={(event) =>
                              atualizarFiltroEmEdicao("dataInicialLancamento", event.target.value)
                            }
                          />
                        </div>
                        <div className="space-y-2">
                          <p className="text-sm font-medium">Final</p>
                          <Input
                            type="date"
                            value={filtrosEmEdicao.dataFinalLancamento}
                            onChange={(event) =>
                              atualizarFiltroEmEdicao("dataFinalLancamento", event.target.value)
                            }
                          />
                        </div>
                      </div>
                    </div>

                    <div className="space-y-3">
                      <h4 className="text-sm font-medium text-foreground">Período de vencimento</h4>
                      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-1">
                        <div className="space-y-2">
                          <p className="text-sm font-medium">Inicial</p>
                          <Input
                            type="date"
                            value={filtrosEmEdicao.dataInicialVencimento}
                            onChange={(event) =>
                              atualizarFiltroEmEdicao("dataInicialVencimento", event.target.value)
                            }
                          />
                        </div>
                        <div className="space-y-2">
                          <p className="text-sm font-medium">Final</p>
                          <Input
                            type="date"
                            value={filtrosEmEdicao.dataFinalVencimento}
                            onChange={(event) =>
                              atualizarFiltroEmEdicao("dataFinalVencimento", event.target.value)
                            }
                          />
                        </div>
                      </div>
                    </div>

                    <div className="space-y-3">
                      <h4 className="text-sm font-medium text-foreground">Período de efetivação</h4>
                      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-1">
                        <div className="space-y-2">
                          <p className="text-sm font-medium">Inicial</p>
                          <Input
                            type="date"
                            value={filtrosEmEdicao.dataInicialEfetivacao}
                            onChange={(event) =>
                              atualizarFiltroEmEdicao("dataInicialEfetivacao", event.target.value)
                            }
                          />
                        </div>
                        <div className="space-y-2">
                          <p className="text-sm font-medium">Final</p>
                          <Input
                            type="date"
                            value={filtrosEmEdicao.dataFinalEfetivacao}
                            onChange={(event) =>
                              atualizarFiltroEmEdicao("dataFinalEfetivacao", event.target.value)
                            }
                          />
                        </div>
                      </div>
                    </div>
                  </div>
                </section>

                <section className="space-y-4">
                  <div className="space-y-1">
                    <h3 className="text-sm font-semibold tracking-wide text-foreground">
                      Ordenação
                    </h3>
                    <p className="text-sm text-muted-foreground">
                      Defina como a lista deve ser organizada na tela.
                    </p>
                  </div>
                  <div className="grid gap-4 md:grid-cols-2 xl:max-w-2xl">
                    <div className="space-y-2">
                      <p className="text-sm font-medium">Ordenar por</p>
                      <Select
                        value={ordenarPor}
                        onValueChange={(value) => atualizarOrdenacao(value as "data" | "valor")}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Escolha a ordenacao" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="data">Data de vencimento</SelectItem>
                          <SelectItem value="valor">Valor</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    <div className="space-y-2">
                      <p className="text-sm font-medium">Direcao</p>
                      <Select
                        value={direcaoOrdenacao}
                        onValueChange={(value) => atualizarDirecao(value as "asc" | "desc")}
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
                </section>

                <section className="space-y-4">
                  <div className="space-y-1">
                    <h3 className="text-sm font-semibold tracking-wide text-foreground">
                      Ações
                    </h3>
                    <p className="text-sm text-muted-foreground">
                      Aplique ou limpe os filtros mantendo todas as opcoes visiveis.
                    </p>
                  </div>
                  <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <p className="text-sm text-muted-foreground">
                      {totalItens} resultado(s) encontrado(s).
                    </p>
                    <div className="flex flex-wrap items-center gap-2">
                      <Button variant="outline" onClick={limparFiltros}>
                        Limpar filtros
                      </Button>
                      <Button onClick={aplicarFiltros}>Buscar</Button>
                    </div>
                  </div>
                </section>
              </div>

              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando lançamentos...
                </div>
              ) : lancamentos.length === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                  <Plus className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                  <p className="text-sm text-muted-foreground">
                    Ainda não existem lançamentos cadastrados para esta conta.
                  </p>
                </div>
              ) : totalItens === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                  <p className="text-sm text-muted-foreground">
                    Nenhum lançamento encontrado com os filtros atuais.
                  </p>
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Descrição</TableHead>
                      <TableHead>Tipo</TableHead>
                      <TableHead>Categoria</TableHead>
                      <TableHead>Valor</TableHead>
                      <TableHead>Vencimento</TableHead>
                      <TableHead>Efetivacao</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead className="text-right">Ações</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {lancamentos.map((lancamento) => (
                      <TableRow key={lancamento.id}>
                        <TableCell>
                          <div>
                            <p className="font-medium">{lancamento.descricao}</p>
                            <p className="text-xs text-muted-foreground">
                              Lançado em {formatDate(lancamento.dataLancamento)}
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
                        <TableCell>{formatDate(lancamento.dataVencimento)}</TableCell>
                        <TableCell>
                          {lancamento.dataEfetivacao ? formatDate(lancamento.dataEfetivacao) : "-"}
                        </TableCell>
                        <TableCell>
                          <Badge variant={getStatusVariant(lancamento.statusLancamento)}>
                            {getStatusLabel(lancamento.statusLancamento, lancamento.tipo)}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="flex justify-end gap-2">
                            {podeEfetivarRapido(lancamento) ? (
                              <Button
                                variant="outline"
                                size="sm"
                                className="min-w-[7.5rem] justify-center"
                                onClick={() => efetivarRapidamente(lancamento)}
                                disabled={efetivandoId === lancamento.id}
                              >
                                <Check className="mr-2 h-4 w-4" />
                                {efetivandoId === lancamento.id
                                  ? "Salvando..."
                                  : getAcaoEfetivacaoLabel(lancamento)}
                              </Button>
                            ) : null}
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
                    <span className="text-sm text-muted-foreground">Itens por página</span>
                    <Select
                      value={String(tamanhoPagina)}
                      onValueChange={(value) => atualizarTamanhoPagina(Number(value))}
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
            <AlertDialogTitle>Excluir lançamento</AlertDialogTitle>
            <AlertDialogDescription>
              {deleteTarget
                ? `Tem certeza que deseja excluir "${deleteTarget.descricao}"? Esta ação não pode ser desfeita.`
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
