"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import {
  buscarFluxoCaixaSimples,
  exportarFluxoCaixaSimplesExcel,
} from "@/services/api/fluxo-caixa-simples";
import { FluxoCaixaSimplesResumo } from "@/types/fluxo-caixa-simples";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

const LIMITE_MESES = 12;

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR").format(new Date(value));
}

function criarDataBaseMesAtual() {
  const hoje = new Date();
  return new Date(hoje.getFullYear(), hoje.getMonth(), 1);
}

function adicionarMeses(data: Date, quantidade: number) {
  return new Date(data.getFullYear(), data.getMonth() + quantidade, 1);
}

function formatarReferenciaMes(data: Date) {
  const texto = data.toLocaleDateString("pt-BR", {
    month: "long",
    year: "numeric",
  });

  return texto.charAt(0).toUpperCase() + texto.slice(1);
}

function formatMonthInput(data: Date) {
  const ano = data.getFullYear();
  const mes = String(data.getMonth() + 1).padStart(2, "0");
  return `${ano}-${mes}`;
}

function parseMonthInput(value: string) {
  const [ano, mes] = value.split("-").map(Number);

  if (!ano || !mes) {
    return null;
  }

  return new Date(ano, mes - 1, 1);
}

export function FluxoCaixaSimplesManager() {
  const { session } = useAuth();
  const [mesOffset, setMesOffset] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [isExporting, setIsExporting] = useState(false);
  const [isExportDialogOpen, setIsExportDialogOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [fluxoCaixa, setFluxoCaixa] = useState<FluxoCaixaSimplesResumo | null>(null);
  const [tipoExportacao, setTipoExportacao] = useState<"mes-atual" | "intervalo" | "ano">(
    "mes-atual"
  );

  const referenciaBase = useMemo(criarDataBaseMesAtual, []);
  const referenciaAtual = useMemo(
    () => adicionarMeses(referenciaBase, mesOffset),
    [mesOffset, referenciaBase]
  );
  const [mesInicialExportacao, setMesInicialExportacao] = useState(formatMonthInput(referenciaAtual));
  const [mesFinalExportacao, setMesFinalExportacao] = useState(formatMonthInput(referenciaAtual));
  const [anoExportacao, setAnoExportacao] = useState(String(referenciaAtual.getFullYear()));

  useEffect(() => {
    const referencia = formatMonthInput(referenciaAtual);
    setMesInicialExportacao(referencia);
    setMesFinalExportacao(referencia);
    setAnoExportacao(String(referenciaAtual.getFullYear()));
  }, [referenciaAtual]);

  const carregarFluxoCaixa = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarFluxoCaixaSimples(
        session.usuario.id,
        referenciaAtual.getFullYear(),
        referenciaAtual.getMonth() + 1,
        session.token
      );

      setFluxoCaixa(response.dados ?? null);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar o fluxo de caixa simples.");
      }
    } finally {
      setIsLoading(false);
    }
  }, [referenciaAtual, session?.token, session?.usuario.id]);

  useEffect(() => {
    carregarFluxoCaixa();
  }, [carregarFluxoCaixa]);

  const receitasTotal = fluxoCaixa?.receitasTotal ?? 0;
  const despesasTotal = fluxoCaixa?.despesasTotal ?? 0;
  const saldoMes = fluxoCaixa?.saldoMes ?? 0;
  const maxComparativo = Math.max(receitasTotal, despesasTotal, 1);

  const barrasComparativas = [
    {
      label: "Receitas",
      value: receitasTotal,
      className: "bg-emerald-500/90",
    },
    {
      label: "Despesas",
      value: despesasTotal,
      className: "bg-rose-500/85",
    },
  ];

  async function exportarExcel() {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsExporting(true);
      setErrorMessage("");

      let blob: Blob;

      if (tipoExportacao === "ano") {
        blob = await exportarFluxoCaixaSimplesExcel(
          session.usuario.id,
          {
            tipoPeriodo: "ano",
            ano: Number(anoExportacao),
          },
          session.token
        );
      } else if (tipoExportacao === "intervalo") {
        const dataInicial = parseMonthInput(mesInicialExportacao);
        const dataFinal = parseMonthInput(mesFinalExportacao);

        if (!dataInicial || !dataFinal) {
          throw new Error("Informe o intervalo de meses para exportação.");
        }

        const quantidadeMeses =
          (dataFinal.getFullYear() - dataInicial.getFullYear()) * 12 +
          (dataFinal.getMonth() - dataInicial.getMonth()) +
          1;

        if (quantidadeMeses < 1) {
          throw new Error("O período final não pode ser anterior ao período inicial.");
        }

        if (quantidadeMeses > 12) {
          throw new Error("A exportação permite no máximo 12 meses por vez.");
        }

        blob = await exportarFluxoCaixaSimplesExcel(
          session.usuario.id,
          {
            tipoPeriodo: "intervalo",
            anoInicial: dataInicial.getFullYear(),
            mesInicial: dataInicial.getMonth() + 1,
            anoFinal: dataFinal.getFullYear(),
            mesFinal: dataFinal.getMonth() + 1,
          },
          session.token
        );
      } else {
        blob = await exportarFluxoCaixaSimplesExcel(
          session.usuario.id,
          {
            tipoPeriodo: "mes-atual",
            ano: referenciaAtual.getFullYear(),
            mes: referenciaAtual.getMonth() + 1,
          },
          session.token
        );
      }

      const url = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = "fluxo-caixa.xlsx";
      document.body.appendChild(link);
      link.click();
      link.remove();
      URL.revokeObjectURL(url);
      setIsExportDialogOpen(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else if (error instanceof Error) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível exportar o fluxo de caixa simples.");
      }
    } finally {
      setIsExporting(false);
    }
  }

  return (
    <div className="flex-1 px-6 py-8 md:px-8">
      <div className="mx-auto max-w-6xl space-y-6">
        <Card className="border-0 shadow-none">
          <CardHeader className="px-0 pt-0">
            <div className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
              <div>
                <CardTitle className="text-3xl">Fluxo de Caixa Simples</CardTitle>
                <CardDescription className="mt-2 max-w-3xl text-base">
                  Uma leitura mensal direta para conferir receitas, despesas e saldo
                  sem precisar navegar por vários gráficos ou filtros.
                </CardDescription>
              </div>

              <div className="flex flex-col gap-3 md:items-end">
                <Button variant="outline" onClick={() => setIsExportDialogOpen(true)}>
                  Exportar Excel
                </Button>

                <div className="flex items-center gap-3 rounded-full border bg-background px-3 py-2 shadow-sm">
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setMesOffset((current) => current - 1)}
                    disabled={mesOffset <= -LIMITE_MESES}
                  >
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <div className="min-w-[11rem] text-center text-sm font-semibold">
                    {formatarReferenciaMes(referenciaAtual)}
                  </div>
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setMesOffset((current) => current + 1)}
                    disabled={mesOffset >= LIMITE_MESES}
                  >
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>
          </CardHeader>
        </Card>

        {errorMessage ? (
          <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
            {errorMessage}
          </div>
        ) : null}

        <div className="grid gap-4 md:grid-cols-3">
          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Receitas do mês</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(receitasTotal)}</CardTitle>
            </CardHeader>
          </Card>

          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Despesas do mês</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(despesasTotal)}</CardTitle>
            </CardHeader>
          </Card>

          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Saldo do mês</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(saldoMes)}</CardTitle>
            </CardHeader>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Comparativo do mês</CardTitle>
            <CardDescription>
              Uma comparação visual simples entre entradas e saídas previstas.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                Carregando comparativo...
              </div>
            ) : (
              <div className="grid gap-6 md:grid-cols-2">
                {barrasComparativas.map((barra) => (
                  <div key={barra.label} className="space-y-3">
                    <div className="flex items-center justify-between text-sm">
                      <span className="font-medium text-foreground">{barra.label}</span>
                      <span className="text-muted-foreground">
                        {formatCurrency(barra.value)}
                      </span>
                    </div>
                    <div className="flex h-56 items-end rounded-2xl border bg-muted/20 p-4">
                      <div
                        className={`w-full rounded-xl transition-all ${barra.className}`}
                        style={{
                          height: `${Math.max((barra.value / maxComparativo) * 100, barra.value > 0 ? 10 : 0)}%`,
                        }}
                      />
                    </div>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>

        <div className="grid gap-6 xl:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle>Despesas previstas</CardTitle>
              <CardDescription>
                Todas as despesas do mês, ordenadas por vencimento.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando despesas...
                </div>
              ) : fluxoCaixa?.despesas.length ? (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Descrição</TableHead>
                      <TableHead>Categoria</TableHead>
                      <TableHead>Vencimento</TableHead>
                      <TableHead className="text-right">Valor</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {fluxoCaixa.despesas.map((item) => (
                      <TableRow key={item.id}>
                        <TableCell className="font-medium">{item.descricao}</TableCell>
                        <TableCell>{item.categoria || "-"}</TableCell>
                        <TableCell>{formatDate(item.dataVencimento)}</TableCell>
                        <TableCell className="text-right font-medium">
                          {formatCurrency(item.valor)}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              ) : (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Nenhuma despesa prevista para este mês.
                </div>
              )}

              <div className="mt-4 flex items-center justify-between border-t pt-4 text-sm font-semibold">
                <span>Total das despesas</span>
                <span>{formatCurrency(despesasTotal)}</span>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Receitas previstas</CardTitle>
              <CardDescription>
                Todas as receitas do mês, ordenadas por data prevista de recebimento.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando receitas...
                </div>
              ) : fluxoCaixa?.receitas.length ? (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Descrição</TableHead>
                      <TableHead>Categoria</TableHead>
                      <TableHead>Recebimento</TableHead>
                      <TableHead className="text-right">Valor</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {fluxoCaixa.receitas.map((item) => (
                      <TableRow key={item.id}>
                        <TableCell className="font-medium">{item.descricao}</TableCell>
                        <TableCell>{item.categoria || "-"}</TableCell>
                        <TableCell>{formatDate(item.dataVencimento)}</TableCell>
                        <TableCell className="text-right font-medium">
                          {formatCurrency(item.valor)}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              ) : (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Nenhuma receita prevista para este mês.
                </div>
              )}

              <div className="mt-4 flex items-center justify-between border-t pt-4 text-sm font-semibold">
                <span>Total das receitas</span>
                <span>{formatCurrency(receitasTotal)}</span>
              </div>
            </CardContent>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Saldo final do mês</CardTitle>
            <CardDescription>
              Conferência final consolidada usando os lançamentos do mês selecionado.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid gap-4 md:grid-cols-3">
              <div className="rounded-xl border bg-muted/20 p-4">
                <p className="text-sm text-muted-foreground">Receitas</p>
                <p className="mt-2 text-2xl font-semibold">{formatCurrency(receitasTotal)}</p>
              </div>
              <div className="rounded-xl border bg-muted/20 p-4">
                <p className="text-sm text-muted-foreground">Despesas</p>
                <p className="mt-2 text-2xl font-semibold">{formatCurrency(despesasTotal)}</p>
              </div>
              <div className="rounded-xl border bg-muted/20 p-4">
                <p className="text-sm text-muted-foreground">Saldo final</p>
                <p className="mt-2 text-2xl font-semibold">{formatCurrency(saldoMes)}</p>
              </div>
            </div>
          </CardContent>
        </Card>
        <Dialog open={isExportDialogOpen} onOpenChange={setIsExportDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Exportar Excel</DialogTitle>
              <DialogDescription>
                Escolha se deseja exportar o mês selecionado, um intervalo de meses ou um ano inteiro.
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4">
              <div className="flex flex-wrap gap-2">
                <Button
                  type="button"
                  variant={tipoExportacao === "mes-atual" ? "default" : "outline"}
                  onClick={() => setTipoExportacao("mes-atual")}
                >
                  Mês atual
                </Button>
                <Button
                  type="button"
                  variant={tipoExportacao === "intervalo" ? "default" : "outline"}
                  onClick={() => setTipoExportacao("intervalo")}
                >
                  Intervalo de meses
                </Button>
                <Button
                  type="button"
                  variant={tipoExportacao === "ano" ? "default" : "outline"}
                  onClick={() => setTipoExportacao("ano")}
                >
                  Ano inteiro
                </Button>
              </div>

              {tipoExportacao === "mes-atual" ? (
                <div className="rounded-lg border bg-muted/20 px-4 py-3 text-sm text-muted-foreground">
                  Será exportado o mês atualmente exibido na tela:{" "}
                  <span className="font-medium text-foreground">
                    {formatarReferenciaMes(referenciaAtual)}
                  </span>
                </div>
              ) : null}

              {tipoExportacao === "intervalo" ? (
                <div className="grid gap-4 md:grid-cols-2">
                  <div className="space-y-2">
                    <p className="text-sm font-medium">Mês inicial</p>
                    <Input
                      type="month"
                      value={mesInicialExportacao}
                      onChange={(event) => setMesInicialExportacao(event.target.value)}
                    />
                  </div>
                  <div className="space-y-2">
                    <p className="text-sm font-medium">Mês final</p>
                    <Input
                      type="month"
                      value={mesFinalExportacao}
                      onChange={(event) => setMesFinalExportacao(event.target.value)}
                    />
                  </div>
                </div>
              ) : null}

              {tipoExportacao === "ano" ? (
                <div className="space-y-2">
                  <p className="text-sm font-medium">Ano</p>
                  <Input
                    type="number"
                    min="2000"
                    max="2100"
                    value={anoExportacao}
                    onChange={(event) => setAnoExportacao(event.target.value)}
                  />
                </div>
              ) : null}
            </div>

            <DialogFooter>
              <Button variant="outline" onClick={() => setIsExportDialogOpen(false)}>
                Cancelar
              </Button>
              <Button onClick={exportarExcel} disabled={isExporting}>
                {isExporting ? "Exportando..." : "Exportar Excel"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}
