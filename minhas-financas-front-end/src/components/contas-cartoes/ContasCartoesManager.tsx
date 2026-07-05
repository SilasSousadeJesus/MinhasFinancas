"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { CreditCard, Landmark, Pencil, Plus, Trash2 } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { CartaoItem, CartaoPayload, ContaItem, ContaPayload } from "@/types/contas-cartoes";
import {
  buscarCartoes,
  buscarContas,
  cadastrarCartao,
  cadastrarConta,
  deletarCartao,
  deletarConta,
  editarCartao,
  editarConta,
} from "@/services/api/contas-cartoes";
import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
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
import { ContaModal } from "./ContaModal";
import { CartaoModal } from "./CartaoModal";

type ActiveTab = "contas" | "cartoes";

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function getTipoContaLabel(tipo: number) {
  switch (tipo) {
    case 0:
      return "Corrente";
    case 1:
      return "Poupança";
    case 2:
      return "Investimento";
    default:
      return "Outro";
  }
}

function getTipoCartaoLabel(tipo: number) {
  switch (tipo) {
    case 0:
      return "Crédito";
    case 1:
      return "Débito";
    default:
      return "Outro";
  }
}

function getDeleteTargetName(
  target: { kind: "conta"; item: ContaItem } | { kind: "cartao"; item: CartaoItem } | null
) {
  if (!target) {
    return "";
  }

  return target.kind === "conta" ? target.item.nomeConta : target.item.nomeCartao;
}

export function ContasCartoesManager() {
  const { session } = useAuth();
  const [activeTab, setActiveTab] = useState<ActiveTab>("contas");
  const [contas, setContas] = useState<ContaItem[]>([]);
  const [cartoes, setCartoes] = useState<CartaoItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [contaModalOpen, setContaModalOpen] = useState(false);
  const [cartaoModalOpen, setCartaoModalOpen] = useState(false);
  const [contaModalMode, setContaModalMode] = useState<"create" | "edit">("create");
  const [cartaoModalMode, setCartaoModalMode] = useState<"create" | "edit">("create");
  const [selectedConta, setSelectedConta] = useState<ContaItem | null>(null);
  const [selectedCartao, setSelectedCartao] = useState<CartaoItem | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<
    | { kind: "conta"; item: ContaItem }
    | { kind: "cartao"; item: CartaoItem }
    | null
  >(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const carregarDados = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const [contasResponse, cartoesResponse] = await Promise.allSettled([
        buscarContas(session.usuario.id, session.token),
        buscarCartoes(session.usuario.id, session.token),
      ]);

      const mensagensErro: string[] = [];

      if (contasResponse.status === "fulfilled") {
        setContas(contasResponse.value.dados ?? []);
      } else {
        setContas([]);
        mensagensErro.push(
          contasResponse.reason instanceof ApiError
            ? `Contas: ${contasResponse.reason.message}`
            : "Contas: não foi possível carregar."
        );
      }

      if (cartoesResponse.status === "fulfilled") {
        setCartoes(cartoesResponse.value.dados ?? []);
      } else {
        setCartoes([]);
        mensagensErro.push(
          cartoesResponse.reason instanceof ApiError
            ? `Cartões: ${cartoesResponse.reason.message}`
            : "Cartões: não foi possível carregar."
        );
      }

      setErrorMessage(mensagensErro.join(" "));
    } catch {
      setErrorMessage("Não foi possível carregar contas e cartões.");
    } finally {
      setIsLoading(false);
    }
  }, [session?.token, session?.usuario.id]);

  useEffect(() => {
    carregarDados();
  }, [carregarDados]);

  const resumoContas = useMemo(() => {
    return contas.reduce(
      (acc, conta) => {
        acc.saldo += conta.saldo;
        acc.investimento += conta.saldoInvestimento;
        return acc;
      },
      { saldo: 0, investimento: 0 }
    );
  }, [contas]);

  const resumoCartoes = useMemo(() => {
    return cartoes.reduce(
      (acc, cartao) => {
        acc.saldo += cartao.saldo;
        if (cartao.tipo === 0) {
          acc.credito += 1;
        }
        if (cartao.tipo === 1) {
          acc.debito += 1;
        }
        return acc;
      },
      { saldo: 0, credito: 0, debito: 0 }
    );
  }, [cartoes]);

  async function handleSalvarConta(payload: ContaPayload) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    setSuccessMessage("");
    setErrorMessage("");

    if (contaModalMode === "create") {
      await cadastrarConta({ ...payload, usuarioId: session.usuario.id }, session.token);
      setSuccessMessage("Conta criada com sucesso.");
    } else if (selectedConta) {
      await editarConta(session.usuario.id, selectedConta.id, payload, session.token);
      setSuccessMessage("Conta atualizada com sucesso.");
    }

    await carregarDados();
  }

  async function handleSalvarCartao(payload: CartaoPayload) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    setSuccessMessage("");
    setErrorMessage("");

    if (cartaoModalMode === "create") {
      await cadastrarCartao({ ...payload, usuarioId: session.usuario.id }, session.token);
      setSuccessMessage("Cartão criado com sucesso.");
    } else if (selectedCartao) {
      await editarCartao(
        session.usuario.id,
        selectedCartao.id,
        { ...payload, usuarioId: session.usuario.id },
        session.token
      );
      setSuccessMessage("Cartão atualizado com sucesso.");
    }

    await carregarDados();
  }

  async function confirmarExclusao() {
    if (!session?.usuario.id || !session.token || !deleteTarget) {
      return;
    }

    try {
      setIsDeleting(true);
      setErrorMessage("");

      if (deleteTarget.kind === "conta") {
        await deletarConta(session.usuario.id, deleteTarget.item.id, session.token);
        setSuccessMessage("Conta excluída com sucesso.");
      } else {
        await deletarCartao(session.usuario.id, deleteTarget.item.id, session.token);
        setSuccessMessage("Cartão excluído com sucesso.");
      }

      setDeleteTarget(null);
      await carregarDados();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível concluir a exclusão.");
      }
    } finally {
      setIsDeleting(false);
    }
  }

  function abrirNovaConta() {
    setContaModalMode("create");
    setSelectedConta(null);
    setContaModalOpen(true);
  }

  function abrirEditarConta(conta: ContaItem) {
    setContaModalMode("edit");
    setSelectedConta(conta);
    setContaModalOpen(true);
  }

  function abrirNovoCartao() {
    setCartaoModalMode("create");
    setSelectedCartao(null);
    setCartaoModalOpen(true);
  }

  function abrirEditarCartao(cartao: CartaoItem) {
    setCartaoModalMode("edit");
    setSelectedCartao(cartao);
    setCartaoModalOpen(true);
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
                  <CardTitle className="text-3xl">Contas e cartões</CardTitle>
                  <CardDescription className="mt-2 max-w-2xl text-base">
                    Cadastre e gerencie as contas e os cartões utilizados no seu controle financeiro.
                  </CardDescription>
                </div>
                <Button onClick={activeTab === "contas" ? abrirNovaConta : abrirNovoCartao}>
                  <Plus className="mr-2 h-4 w-4" />
                  {activeTab === "contas" ? "Nova conta" : "Novo cartão"}
                </Button>
              </div>
            </CardHeader>
          </Card>

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

          <Tabs value={activeTab} onValueChange={(value) => setActiveTab(value as ActiveTab)} className="space-y-4">
            <TabsList>
              <TabsTrigger value="contas">Contas</TabsTrigger>
              <TabsTrigger value="cartoes">Cartões</TabsTrigger>
            </TabsList>

            <TabsContent value="contas" className="space-y-4">
              <div className="grid gap-4 md:grid-cols-3">
                <Card>
                  <CardHeader className="pb-3">
                    <CardDescription>Total de contas</CardDescription>
                    <CardTitle className="text-3xl">{contas.length}</CardTitle>
                  </CardHeader>
                </Card>
                <Card>
                  <CardHeader className="pb-3">
                    <CardDescription>Saldo em contas</CardDescription>
                    <CardTitle className="text-3xl">{formatCurrency(resumoContas.saldo)}</CardTitle>
                  </CardHeader>
                </Card>
                <Card>
                  <CardHeader className="pb-3">
                    <CardDescription>Saldo em investimentos</CardDescription>
                    <CardTitle className="text-3xl">{formatCurrency(resumoContas.investimento)}</CardTitle>
                  </CardHeader>
                </Card>
              </div>

              <Card>
                <CardHeader>
                  <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <div>
                      <CardTitle>Lista de contas</CardTitle>
                      <CardDescription>Gerencie saldos, instituições e tipos de conta.</CardDescription>
                    </div>
                    <Button variant="outline" onClick={carregarDados}>
                      Atualizar lista
                    </Button>
                  </div>
                </CardHeader>
                <CardContent>
                  {isLoading ? (
                    <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                      Carregando contas...
                    </div>
                  ) : contas.length === 0 ? (
                    <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                      <Landmark className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                      <p className="text-sm text-muted-foreground">
                        Ainda não existem contas cadastradas.
                      </p>
                    </div>
                  ) : (
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Conta</TableHead>
                          <TableHead>Instituição</TableHead>
                          <TableHead>Tipo</TableHead>
                          <TableHead>Saldo</TableHead>
                          <TableHead>Investimento</TableHead>
                          <TableHead className="text-right">Ações</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {contas.map((conta) => (
                          <TableRow key={conta.id}>
                            <TableCell>
                              <div>
                                <p className="font-medium">{conta.nomeConta}</p>
                                <p className="text-xs text-muted-foreground">{conta.descricao || "Sem descrição"}</p>
                              </div>
                            </TableCell>
                            <TableCell>{conta.instituicao}</TableCell>
                            <TableCell>
                              <Badge variant="outline">{getTipoContaLabel(conta.tipo)}</Badge>
                            </TableCell>
                            <TableCell>{formatCurrency(conta.saldo)}</TableCell>
                            <TableCell>{formatCurrency(conta.saldoInvestimento)}</TableCell>
                            <TableCell>
                              <div className="flex justify-end gap-2">
                                <Button variant="ghost" size="icon" onClick={() => abrirEditarConta(conta)}>
                                  <Pencil className="h-4 w-4" />
                                </Button>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="text-destructive hover:text-destructive"
                                  onClick={() => setDeleteTarget({ kind: "conta", item: conta })}
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
            </TabsContent>

            <TabsContent value="cartoes" className="space-y-4">
              <div className="grid gap-4 md:grid-cols-3">
                <Card>
                  <CardHeader className="pb-3">
                    <CardDescription>Total de cartões</CardDescription>
                    <CardTitle className="text-3xl">{cartoes.length}</CardTitle>
                  </CardHeader>
                </Card>
                <Card>
                  <CardHeader className="pb-3">
                    <CardDescription>Cartões de crédito</CardDescription>
                    <CardTitle className="text-3xl">{resumoCartoes.credito}</CardTitle>
                  </CardHeader>
                </Card>
                <Card>
                  <CardHeader className="pb-3">
                    <CardDescription>Saldo total dos cartões</CardDescription>
                    <CardTitle className="text-3xl">{formatCurrency(resumoCartoes.saldo)}</CardTitle>
                  </CardHeader>
                </Card>
              </div>

              <Card>
                <CardHeader>
                  <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <div>
                      <CardTitle>Lista de cartões</CardTitle>
                      <CardDescription>Gerencie bandeira, vencimento e demais dados do cartão.</CardDescription>
                    </div>
                    <Button variant="outline" onClick={carregarDados}>
                      Atualizar lista
                    </Button>
                  </div>
                </CardHeader>
                <CardContent>
                  {isLoading ? (
                    <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                      Carregando cartões...
                    </div>
                  ) : cartoes.length === 0 ? (
                    <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                      <CreditCard className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                      <p className="text-sm text-muted-foreground">
                        Ainda não existem cartões cadastrados.
                      </p>
                    </div>
                  ) : (
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Cartão</TableHead>
                          <TableHead>Instituição</TableHead>
                          <TableHead>Tipo</TableHead>
                          <TableHead>Bandeira</TableHead>
                          <TableHead>Fechamento / Venc.</TableHead>
                          <TableHead className="text-right">Ações</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {cartoes.map((cartao) => (
                          <TableRow key={cartao.id}>
                            <TableCell>
                              <div>
                                <p className="font-medium">{cartao.nomeCartao}</p>
                                <p className="text-xs text-muted-foreground">
                                  Final {cartao.ultimos4Digitos}
                                  {cartao.descricao ? ` - ${cartao.descricao}` : ""}
                                </p>
                              </div>
                            </TableCell>
                            <TableCell>{cartao.instituicao}</TableCell>
                            <TableCell>
                              <Badge variant="outline">{getTipoCartaoLabel(cartao.tipo)}</Badge>
                            </TableCell>
                            <TableCell>{cartao.bandeira}</TableCell>
                            <TableCell>
                              {cartao.diaFechamento} / {cartao.diaVencimento}
                            </TableCell>
                            <TableCell>
                              <div className="flex justify-end gap-2">
                                <Button variant="ghost" size="icon" onClick={() => abrirEditarCartao(cartao)}>
                                  <Pencil className="h-4 w-4" />
                                </Button>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="text-destructive hover:text-destructive"
                                  onClick={() => setDeleteTarget({ kind: "cartao", item: cartao })}
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
            </TabsContent>
          </Tabs>
        </div>
      </div>

      <ContaModal
        open={contaModalOpen}
        onOpenChange={setContaModalOpen}
        mode={contaModalMode}
        initialData={selectedConta}
        onSubmit={handleSalvarConta}
      />

      <CartaoModal
        open={cartaoModalOpen}
        onOpenChange={setCartaoModalOpen}
        mode={cartaoModalMode}
        initialData={selectedCartao}
        onSubmit={handleSalvarCartao}
      />

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar exclusão</AlertDialogTitle>
            <AlertDialogDescription>
              {deleteTarget
                ? `Tem certeza que deseja excluir ${deleteTarget.kind === "conta" ? "a conta" : "o cartão"} "${getDeleteTargetName(deleteTarget)}"?`
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
