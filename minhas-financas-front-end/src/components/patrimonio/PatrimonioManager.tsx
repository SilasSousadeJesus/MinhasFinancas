"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Landmark, Pencil, Plus, Trash2, Wallet } from "lucide-react";
import { Sidebar } from "@/components/Sidebar/Sidebar";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import {
  AtivoPatrimonialItem,
  AtivoPatrimonialPayload,
  LinhaEvolucaoPatrimonial,
  PassivoPatrimonialItem,
  PassivoPatrimonialPayload,
  SnapshotPatrimonialPayload,
  VisaoGeralPatrimonio,
} from "@/types/patrimonio";
import {
  buscarVisaoGeralPatrimonio,
  cadastrarAtivoPatrimonial,
  cadastrarPassivoPatrimonial,
  editarAtivoPatrimonial,
  editarPassivoPatrimonial,
  gerarSnapshotPatrimonial,
  inativarAtivoPatrimonial,
  inativarPassivoPatrimonial,
} from "@/services/api/patrimonio";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
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
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import {
  CartesianGrid,
  Line,
  LineChart,
  XAxis,
  YAxis,
} from "recharts";
import { AtivoPatrimonialModal } from "./AtivoPatrimonialModal";
import { PassivoPatrimonialModal } from "./PassivoPatrimonialModal";
import { SnapshotPatrimonialModal } from "./SnapshotPatrimonialModal";

type DeleteTarget =
  | { kind: "ativo"; item: AtivoPatrimonialItem }
  | { kind: "passivo"; item: PassivoPatrimonialItem }
  | null;

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function formatDate(value?: string | null) {
  if (!value) {
    return "—";
  }

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "—";
  }

  return date.toLocaleDateString("pt-BR");
}

function getTipoAtivoLabel(tipo: number) {
  switch (tipo) {
    case 0:
      return "Imóvel";
    case 1:
      return "Automóvel";
    case 2:
      return "Investimento";
    case 3:
      return "Dinheiro em conta";
    case 4:
      return "Equipamento";
    case 5:
      return "Instrumento musical";
    default:
      return "Outro";
  }
}

function getTipoPassivoLabel(tipo: number) {
  switch (tipo) {
    case 0:
      return "Financiamento";
    case 1:
      return "Empréstimo";
    case 2:
      return "Dívida";
    case 3:
      return "Parcelamento";
    case 4:
      return "Obrigação financeira";
    default:
      return "Outro";
  }
}

function getDeleteTargetName(target: DeleteTarget) {
  if (!target) {
    return "";
  }

  return target.item.nome;
}

export function PatrimonioManager() {
  const { session } = useAuth();
  const [visaoGeral, setVisaoGeral] = useState<VisaoGeralPatrimonio | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [ativoModalOpen, setAtivoModalOpen] = useState(false);
  const [passivoModalOpen, setPassivoModalOpen] = useState(false);
  const [snapshotModalOpen, setSnapshotModalOpen] = useState(false);
  const [ativoModalMode, setAtivoModalMode] = useState<"create" | "edit">("create");
  const [passivoModalMode, setPassivoModalMode] = useState<"create" | "edit">("create");
  const [selectedAtivo, setSelectedAtivo] = useState<AtivoPatrimonialItem | null>(null);
  const [selectedPassivo, setSelectedPassivo] = useState<PassivoPatrimonialItem | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const carregarPatrimonio = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarVisaoGeralPatrimonio(session.usuario.id, session.token);
      setVisaoGeral(response.dados);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar o patrimônio.");
      }
      setVisaoGeral(null);
    } finally {
      setIsLoading(false);
    }
  }, [session?.token, session?.usuario.id]);

  useEffect(() => {
    carregarPatrimonio();
  }, [carregarPatrimonio]);

  const resumo = visaoGeral?.resumo ?? {
    totalAtivos: 0,
    totalPassivos: 0,
    patrimonioLiquido: 0,
    quantidadeAtivos: 0,
    quantidadePassivos: 0,
  };

  const evolucaoGrafico = useMemo(() => {
    return (visaoGeral?.evolucao ?? []).map((linha: LinhaEvolucaoPatrimonial) => ({
      data: new Date(linha.dataReferencia).toLocaleDateString("pt-BR", {
        month: "short",
        year: "numeric",
      }),
      patrimonio: linha.patrimonioLiquido,
      ativos: linha.totalAtivos,
      passivos: linha.totalPassivos,
    }));
  }, [visaoGeral?.evolucao]);

  async function handleSalvarAtivo(payload: Omit<AtivoPatrimonialPayload, "usuarioId">) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    setSuccessMessage("");
    setErrorMessage("");

    const completePayload = {
      ...payload,
      usuarioId: session.usuario.id,
    };

    if (ativoModalMode === "create") {
      await cadastrarAtivoPatrimonial(completePayload, session.token);
      setSuccessMessage("Ativo cadastrado com sucesso.");
    } else if (selectedAtivo) {
      await editarAtivoPatrimonial(
        session.usuario.id,
        selectedAtivo.id,
        completePayload,
        session.token
      );
      setSuccessMessage("Ativo atualizado com sucesso.");
    }

    await carregarPatrimonio();
  }

  async function handleSalvarPassivo(payload: Omit<PassivoPatrimonialPayload, "usuarioId">) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    setSuccessMessage("");
    setErrorMessage("");

    const completePayload = {
      ...payload,
      usuarioId: session.usuario.id,
    };

    if (passivoModalMode === "create") {
      await cadastrarPassivoPatrimonial(completePayload, session.token);
      setSuccessMessage("Passivo cadastrado com sucesso.");
    } else if (selectedPassivo) {
      await editarPassivoPatrimonial(
        session.usuario.id,
        selectedPassivo.id,
        completePayload,
        session.token
      );
      setSuccessMessage("Passivo atualizado com sucesso.");
    }

    await carregarPatrimonio();
  }

  async function handleGerarSnapshot(payload: SnapshotPatrimonialPayload) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    setSuccessMessage("");
    setErrorMessage("");

    await gerarSnapshotPatrimonial(session.usuario.id, payload, session.token);
    setSuccessMessage("Snapshot patrimonial gerado com sucesso.");
    await carregarPatrimonio();
  }

  async function confirmarInativacao() {
    if (!session?.usuario.id || !session.token || !deleteTarget) {
      return;
    }

    try {
      setIsDeleting(true);
      setErrorMessage("");

      if (deleteTarget.kind === "ativo") {
        await inativarAtivoPatrimonial(session.usuario.id, deleteTarget.item.id, session.token);
        setSuccessMessage("Ativo inativado com sucesso.");
      } else {
        await inativarPassivoPatrimonial(
          session.usuario.id,
          deleteTarget.item.id,
          session.token
        );
        setSuccessMessage("Passivo inativado com sucesso.");
      }

      setDeleteTarget(null);
      await carregarPatrimonio();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível concluir a inativação.");
      }
    } finally {
      setIsDeleting(false);
    }
  }

  function abrirNovoAtivo() {
    setAtivoModalMode("create");
    setSelectedAtivo(null);
    setAtivoModalOpen(true);
  }

  function abrirEditarAtivo(ativo: AtivoPatrimonialItem) {
    setAtivoModalMode("edit");
    setSelectedAtivo(ativo);
    setAtivoModalOpen(true);
  }

  function abrirNovoPassivo() {
    setPassivoModalMode("create");
    setSelectedPassivo(null);
    setPassivoModalOpen(true);
  }

  function abrirEditarPassivo(passivo: PassivoPatrimonialItem) {
    setPassivoModalMode("edit");
    setSelectedPassivo(passivo);
    setPassivoModalOpen(true);
  }

  return (
    <div className="flex flex-row">
      <Sidebar />
      <div className="flex-1 px-6 py-8 md:px-8">
        <div className="mx-auto max-w-7xl space-y-6">
          <Card className="border-0 shadow-none">
            <CardHeader className="px-0 pt-0">
              <div className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
                <div>
                  <CardTitle className="text-3xl">Patrimônio</CardTitle>
                  <CardDescription className="mt-2 max-w-3xl text-base">
                    Acompanhe ativos, passivos e a evolução do patrimônio líquido com histórico congelado em snapshots.
                  </CardDescription>
                </div>
                <div className="flex flex-wrap gap-2">
                  <Button variant="outline" onClick={abrirNovoAtivo}>
                    <Plus className="mr-2 h-4 w-4" />
                    Novo ativo
                  </Button>
                  <Button variant="outline" onClick={abrirNovoPassivo}>
                    <Plus className="mr-2 h-4 w-4" />
                    Novo passivo
                  </Button>
                  <Button onClick={() => setSnapshotModalOpen(true)}>
                    <Landmark className="mr-2 h-4 w-4" />
                    Gerar snapshot
                  </Button>
                </div>
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

          <div className="grid gap-4 md:grid-cols-3">
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Total de ativos</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.totalAtivos)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Total de passivos</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.totalPassivos)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Patrimônio líquido</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.patrimonioLiquido)}</CardTitle>
              </CardHeader>
            </Card>
          </div>

          <div className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
            <Card>
              <CardHeader>
                <CardTitle>Evolução patrimonial</CardTitle>
                <CardDescription>
                  O gráfico utiliza os snapshots salvos manualmente e preserva a fotografia de cada momento.
                </CardDescription>
              </CardHeader>
              <CardContent>
                {evolucaoGrafico.length === 0 ? (
                  <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                    Gere o primeiro snapshot para começar a acompanhar a evolução patrimonial.
                  </div>
                ) : (
                  <ChartContainer
                    className="h-[280px] w-full"
                    config={{
                      patrimonio: {
                        label: "Patrimônio líquido",
                        color: "hsl(var(--chart-1))",
                      },
                    }}
                  >
                    <LineChart data={evolucaoGrafico} margin={{ left: 8, right: 8 }}>
                      <CartesianGrid vertical={false} />
                      <XAxis dataKey="data" tickLine={false} axisLine={false} tickMargin={8} />
                      <YAxis hide />
                      <ChartTooltip
                        cursor={false}
                        content={
                          <ChartTooltipContent
                            formatter={(value, name) => (
                              <>
                                <div className="h-2.5 w-2.5 rounded-[2px] bg-primary" />
                                <div className="flex flex-1 items-center justify-between gap-2">
                                  <span className="text-muted-foreground">
                                    {name === "patrimonio" ? "Patrimônio líquido" : String(name)}
                                  </span>
                                  <span className="font-mono font-medium tabular-nums text-foreground">
                                    {formatCurrency(Number(value))}
                                  </span>
                                </div>
                              </>
                            )}
                          />
                        }
                      />
                      <Line
                        dataKey="patrimonio"
                        type="natural"
                        stroke="var(--color-patrimonio)"
                        strokeWidth={2.5}
                        dot={{ r: 4 }}
                      />
                    </LineChart>
                  </ChartContainer>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Leitura rápida</CardTitle>
                <CardDescription>
                  Uma visão resumida do estado patrimonial atual para decisão rápida.
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="rounded-2xl border bg-muted/20 p-4">
                  <p className="text-sm text-muted-foreground">Ativos cadastrados</p>
                  <p className="mt-2 text-2xl font-semibold">{resumo.quantidadeAtivos}</p>
                </div>
                <div className="rounded-2xl border bg-muted/20 p-4">
                  <p className="text-sm text-muted-foreground">Passivos cadastrados</p>
                  <p className="mt-2 text-2xl font-semibold">{resumo.quantidadePassivos}</p>
                </div>
                <div className="rounded-2xl border bg-muted/20 p-4">
                  <p className="text-sm text-muted-foreground">Snapshots salvos</p>
                  <p className="mt-2 text-2xl font-semibold">{visaoGeral?.snapshots.length ?? 0}</p>
                </div>
              </CardContent>
            </Card>
          </div>

          <Card>
            <CardHeader>
              <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                <div>
                  <CardTitle>Ativos</CardTitle>
                  <CardDescription>
                    Bens e recursos que contribuem positivamente para o patrimônio total.
                  </CardDescription>
                </div>
                <Button variant="outline" onClick={abrirNovoAtivo}>
                  <Plus className="mr-2 h-4 w-4" />
                  Novo ativo
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando ativos...
                </div>
              ) : (visaoGeral?.ativos.length ?? 0) === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                  <Wallet className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                  <p className="text-sm text-muted-foreground">
                    Ainda não existem ativos cadastrados.
                  </p>
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Ativo</TableHead>
                      <TableHead>Tipo</TableHead>
                      <TableHead>Valor atual</TableHead>
                      <TableHead>Data de aquisição</TableHead>
                      <TableHead>Última atualização</TableHead>
                      <TableHead className="text-right">Ações</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {visaoGeral?.ativos.map((ativo) => (
                      <TableRow key={ativo.id}>
                        <TableCell>
                          <div>
                            <p className="font-medium">{ativo.nome}</p>
                            <p className="text-xs text-muted-foreground">
                              {ativo.descricao || "Sem observação"}
                            </p>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline">{getTipoAtivoLabel(ativo.tipo)}</Badge>
                        </TableCell>
                        <TableCell>{formatCurrency(ativo.valorAtual)}</TableCell>
                        <TableCell>{formatDate(ativo.dataAquisicao)}</TableCell>
                        <TableCell>{formatDate(ativo.dataReferenciaValor)}</TableCell>
                        <TableCell>
                          <div className="flex justify-end gap-2">
                            <Button variant="ghost" size="icon" onClick={() => abrirEditarAtivo(ativo)}>
                              <Pencil className="h-4 w-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="text-destructive hover:text-destructive"
                              onClick={() => setDeleteTarget({ kind: "ativo", item: ativo })}
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

          <Card>
            <CardHeader>
              <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                <div>
                  <CardTitle>Passivos</CardTitle>
                  <CardDescription>
                    Obrigações financeiras que reduzem o patrimônio líquido do usuário.
                  </CardDescription>
                </div>
                <Button variant="outline" onClick={abrirNovoPassivo}>
                  <Plus className="mr-2 h-4 w-4" />
                  Novo passivo
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando passivos...
                </div>
              ) : (visaoGeral?.passivos.length ?? 0) === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                  <Landmark className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                  <p className="text-sm text-muted-foreground">
                    Ainda não existem passivos cadastrados.
                  </p>
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Passivo</TableHead>
                      <TableHead>Tipo</TableHead>
                      <TableHead>Valor atual</TableHead>
                      <TableHead>Início</TableHead>
                      <TableHead>Fim</TableHead>
                      <TableHead>Última atualização</TableHead>
                      <TableHead className="text-right">Ações</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {visaoGeral?.passivos.map((passivo) => (
                      <TableRow key={passivo.id}>
                        <TableCell>
                          <div>
                            <p className="font-medium">{passivo.nome}</p>
                            <p className="text-xs text-muted-foreground">
                              {passivo.descricao || "Sem observação"}
                            </p>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline">{getTipoPassivoLabel(passivo.tipo)}</Badge>
                        </TableCell>
                        <TableCell>{formatCurrency(passivo.valorAtual)}</TableCell>
                        <TableCell>{formatDate(passivo.dataInicio)}</TableCell>
                        <TableCell>{formatDate(passivo.dataFim)}</TableCell>
                        <TableCell>{formatDate(passivo.dataReferenciaValor)}</TableCell>
                        <TableCell>
                          <div className="flex justify-end gap-2">
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => abrirEditarPassivo(passivo)}
                            >
                              <Pencil className="h-4 w-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="text-destructive hover:text-destructive"
                              onClick={() => setDeleteTarget({ kind: "passivo", item: passivo })}
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

          <Card>
            <CardHeader>
              <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                <div>
                  <CardTitle>Histórico de snapshots</CardTitle>
                  <CardDescription>
                    Cada snapshot congela os totais daquele momento e não é recalculado após mudanças futuras.
                  </CardDescription>
                </div>
                <Button variant="outline" onClick={() => setSnapshotModalOpen(true)}>
                  <Plus className="mr-2 h-4 w-4" />
                  Gerar snapshot atual
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando snapshots...
                </div>
              ) : (visaoGeral?.snapshots.length ?? 0) === 0 ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Nenhum snapshot salvo até agora.
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Data de referência</TableHead>
                      <TableHead>Total de ativos</TableHead>
                      <TableHead>Total de passivos</TableHead>
                      <TableHead>Patrimônio líquido</TableHead>
                      <TableHead>Observação</TableHead>
                      <TableHead>Registrado em</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {visaoGeral?.snapshots.map((snapshot) => (
                      <TableRow key={snapshot.id}>
                        <TableCell>{formatDate(snapshot.dataReferencia)}</TableCell>
                        <TableCell>{formatCurrency(snapshot.totalAtivos)}</TableCell>
                        <TableCell>{formatCurrency(snapshot.totalPassivos)}</TableCell>
                        <TableCell>{formatCurrency(snapshot.patrimonioLiquido)}</TableCell>
                        <TableCell>{snapshot.observacao || "—"}</TableCell>
                        <TableCell>{formatDate(snapshot.dataCriacao)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </div>
      </div>

      <AtivoPatrimonialModal
        open={ativoModalOpen}
        onOpenChange={setAtivoModalOpen}
        mode={ativoModalMode}
        initialData={selectedAtivo}
        onSubmit={handleSalvarAtivo}
      />

      <PassivoPatrimonialModal
        open={passivoModalOpen}
        onOpenChange={setPassivoModalOpen}
        mode={passivoModalMode}
        initialData={selectedPassivo}
        onSubmit={handleSalvarPassivo}
      />

      <SnapshotPatrimonialModal
        open={snapshotModalOpen}
        onOpenChange={setSnapshotModalOpen}
        onSubmit={handleGerarSnapshot}
      />

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar inativação</AlertDialogTitle>
            <AlertDialogDescription>
              {deleteTarget
                ? `Tem certeza que deseja inativar ${deleteTarget.kind === "ativo" ? "o ativo" : "o passivo"} "${getDeleteTargetName(deleteTarget)}"?`
                : ""}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmarInativacao} disabled={isDeleting}>
              {isDeleting ? "Inativando..." : "Inativar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
