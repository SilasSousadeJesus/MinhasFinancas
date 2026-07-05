"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { buscarFluxoCaixaSimples } from "@/services/api/fluxo-caixa-simples";
import { FluxoCaixaSimplesResumo } from "@/types/fluxo-caixa-simples";
import { Button } from "@/components/ui/button";
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

export function FluxoCaixaSimplesManager() {
  const { session } = useAuth();
  const [mesOffset, setMesOffset] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [fluxoCaixa, setFluxoCaixa] = useState<FluxoCaixaSimplesResumo | null>(null);

  const referenciaBase = useMemo(criarDataBaseMesAtual, []);
  const referenciaAtual = useMemo(
    () => adicionarMeses(referenciaBase, mesOffset),
    [mesOffset, referenciaBase]
  );

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
        setErrorMessage("Nao foi possivel carregar o fluxo de caixa simples.");
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
                  sem precisar navegar por varios graficos ou filtros.
                </CardDescription>
              </div>

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
              <CardDescription>Receitas do mes</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(receitasTotal)}</CardTitle>
            </CardHeader>
          </Card>

          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Despesas do mes</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(despesasTotal)}</CardTitle>
            </CardHeader>
          </Card>

          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Saldo do mes</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(saldoMes)}</CardTitle>
            </CardHeader>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Comparativo do mes</CardTitle>
            <CardDescription>
              Uma comparacao visual simples entre entradas e saidas previstas.
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
                Todas as despesas do mes, ordenadas por vencimento.
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
                      <TableHead>Descricao</TableHead>
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
                  Nenhuma despesa prevista para este mes.
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
                Todas as receitas do mes, ordenadas por data prevista de recebimento.
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
                      <TableHead>Descricao</TableHead>
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
                  Nenhuma receita prevista para este mes.
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
            <CardTitle>Saldo final do mes</CardTitle>
            <CardDescription>
              Conferencia final consolidada usando os lancamentos do mes selecionado.
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
      </div>
    </div>
  );
}
