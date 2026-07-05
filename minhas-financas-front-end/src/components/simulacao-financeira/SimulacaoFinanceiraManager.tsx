"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowLeft, Plus, Save, Sparkles, Trash2 } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import {
  ResultadoSimulacaoFinanceira,
  SimulacaoFinanceiraDetalhe,
  TipoAcaoSimulacao,
} from "@/types/simulacao-financeira";
import {
  buscarSimulacaoFinanceira,
  calcularSimulacaoFinanceira,
  editarSimulacaoFinanceira,
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
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Sidebar } from "@/components/Sidebar/Sidebar";
import { SimulacaoFinanceiraLoadingOverlay } from "./SimulacaoFinanceiraLoadingOverlay";

const acaoSchema = z.object({
  tipoAcao: z.coerce.number().min(0).max(4),
  descricao: z.string().min(2, "Informe a descrição da ação."),
  valor: z.coerce.number().min(0.01, "Informe um valor maior que zero."),
  dataInicial: z.string().min(1, "Informe a data inicial."),
  dataFinal: z.string().optional(),
  quantidadeParcelas: z.coerce.number().nullable().optional(),
  observacao: z.string().optional(),
});

const formSchema = z.object({
  nome: z.string().min(2, "Informe o nome da simulação."),
  descricao: z.string().optional(),
  dataInicial: z.string().min(1, "Informe o mês inicial."),
  quantidadeMeses: z.coerce.number().min(1, "Informe ao menos 1 mês.").max(12, "Use no máximo 12 meses."),
  acoes: z.array(acaoSchema),
});

type FormValues = z.infer<typeof formSchema>;

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function formatMonth(value?: string | null) {
  if (!value) {
    return "—";
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

function getTipoAcaoLabel(tipo: TipoAcaoSimulacao | number) {
  switch (Number(tipo)) {
    case 0:
      return "Receita única";
    case 1:
      return "Despesa única";
    case 2:
      return "Receita recorrente mensal";
    case 3:
      return "Despesa recorrente mensal";
    case 4:
      return "Despesa parcelada";
    default:
      return "Ação";
  }
}

function isRecurring(tipo: number) {
  return tipo === 2 || tipo === 3;
}

function isParcelado(tipo: number) {
  return tipo === 4;
}

function toDateInput(value?: string | null) {
  if (!value) return "";
  return value.slice(0, 10);
}

interface SimulacaoFinanceiraManagerProps {
  simulacaoId: string;
}

export function SimulacaoFinanceiraManager({ simulacaoId }: SimulacaoFinanceiraManagerProps) {
  const { session } = useAuth();
  const [simulacao, setSimulacao] = useState<SimulacaoFinanceiraDetalhe | null>(null);
  const [resultado, setResultado] = useState<ResultadoSimulacaoFinanceira | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isCalculating, setIsCalculating] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      nome: "",
      descricao: "",
      dataInicial: "",
      quantidadeMeses: 12,
      acoes: [],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "acoes",
  });

  const acoes = form.watch("acoes");

  async function carregarSimulacao() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessão inválida. Faça login novamente.");
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarSimulacaoFinanceira(session.usuario.id, simulacaoId, session.token);
      const dados = response.dados;

      if (!dados) {
        setErrorMessage("Simulação não encontrada.");
        return;
      }

      setSimulacao(dados);
      setResultado(dados.resultadoAtual ?? null);
      form.reset({
        nome: dados.nome,
        descricao: dados.descricao,
        dataInicial: dados.dataInicial.slice(0, 10),
        quantidadeMeses: dados.quantidadeMeses,
        acoes:
          dados.acoes.length > 0
            ? dados.acoes.map((acao) => ({
                tipoAcao: acao.tipoAcao,
                descricao: acao.descricao,
                valor: acao.valor,
                dataInicial: toDateInput(acao.dataInicial),
                dataFinal: toDateInput(acao.dataFinal),
                quantidadeParcelas: acao.quantidadeParcelas ?? null,
                observacao: acao.observacao,
              }))
            : [],
      });
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar a simulação financeira.");
      }
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void carregarSimulacao();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [simulacaoId, session?.token, session?.usuario.id]);

  async function salvarSimulacao(values: FormValues) {
    if (!session?.usuario.id || !session.token) {
      throw new Error("Sessão inválida.");
    }

    await editarSimulacaoFinanceira(
      session.usuario.id,
      simulacaoId,
      {
        nome: values.nome,
        descricao: values.descricao || "",
        dataInicial: `${values.dataInicial}T00:00:00`,
        quantidadeMeses: values.quantidadeMeses,
        acoes: values.acoes.map((acao) => ({
          tipoAcao: Number(acao.tipoAcao) as TipoAcaoSimulacao,
          descricao: acao.descricao,
          valor: acao.valor,
          dataInicial: `${acao.dataInicial}T00:00:00`,
          dataFinal: acao.dataFinal ? `${acao.dataFinal}T00:00:00` : null,
          quantidadeParcelas: isParcelado(Number(acao.tipoAcao))
            ? Number(acao.quantidadeParcelas) || null
            : null,
          observacao: acao.observacao || "",
        })),
      },
      session.token
    );
  }

  async function onSubmit(values: FormValues) {
    try {
      setIsSaving(true);
      setErrorMessage("");
      await salvarSimulacao(values);
      await carregarSimulacao();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else if (error instanceof Error) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível salvar a simulação.");
      }
    } finally {
      setIsSaving(false);
    }
  }

  async function handleSalvarECalcular() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessão inválida. Faça login novamente.");
      return;
    }

    try {
      setIsCalculating(true);
      setErrorMessage("");
      const values = form.getValues();
      const isValid = await form.trigger();

      if (!isValid) {
        return;
      }

      await salvarSimulacao(values);
      const calculo = await calcularSimulacaoFinanceira(session.usuario.id, simulacaoId, session.token);
      setResultado(calculo.dados ?? null);
      await carregarSimulacao();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else if (error instanceof Error) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível calcular a simulação.");
      }
    } finally {
      setIsCalculating(false);
    }
  }

  const isBusy = isLoading || isSaving || isCalculating;

  const resumo = useMemo(() => {
    return {
      totalAcoes: acoes?.length ?? 0,
      saldoReal: resultado?.saldoRealAcumulado ?? 0,
      saldoSimulado: resultado?.saldoSimuladoAcumulado ?? 0,
      diferenca: resultado?.diferencaAcumulada ?? 0,
    };
  }, [acoes?.length, resultado]);

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="relative flex-1 px-6 py-8 md:px-8">
        <SimulacaoFinanceiraLoadingOverlay
          visible={isBusy}
          message={
            isLoading
              ? "Carregando simulação..."
              : isCalculating
                ? "Salvando e calculando simulação..."
                : "Salvando simulação..."
          }
        />

        <div className="mx-auto max-w-7xl space-y-6">
          <Card className="border-0 shadow-none">
            <CardHeader className="px-0 pt-0">
              <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
                <div className="space-y-3">
                  <Button asChild variant="ghost" className="w-fit px-0 text-muted-foreground">
                    <Link href="/simulacoes-financeiras">
                      <ArrowLeft className="mr-2 h-4 w-4" />
                      Voltar para Simulações Financeiras
                    </Link>
                  </Button>
                  <div>
                    <CardTitle className="text-3xl">Simulação detalhada</CardTitle>
                    <CardDescription className="mt-2 max-w-3xl text-base">
                      Cadastre ações hipotéticas e compare o fluxo real com o fluxo simulado mês a mês, sem alterar nenhum dado real.
                    </CardDescription>
                  </div>
                </div>

                <div className="flex flex-wrap gap-3">
                  <Button
                    type="button"
                    variant="outline"
                    onClick={form.handleSubmit(onSubmit)}
                    disabled={isBusy}
                  >
                    <Save className="mr-2 h-4 w-4" />
                    {isSaving ? "Salvando..." : "Salvar alterações"}
                  </Button>
                  <Button type="button" onClick={handleSalvarECalcular} disabled={isBusy}>
                    <Sparkles className="mr-2 h-4 w-4" />
                    {isCalculating ? "Calculando..." : "Salvar e calcular"}
                  </Button>
                </div>
              </div>
            </CardHeader>
          </Card>

          <div className="grid gap-4 md:grid-cols-4">
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Ações cadastradas</CardDescription>
                <CardTitle className="text-3xl">{resumo.totalAcoes}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Saldo real acumulado</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.saldoReal)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Saldo simulado acumulado</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.saldoSimulado)}</CardTitle>
              </CardHeader>
            </Card>
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Diferença acumulada</CardDescription>
                <CardTitle className="text-3xl">{formatCurrency(resumo.diferenca)}</CardTitle>
              </CardHeader>
            </Card>
          </div>

          {errorMessage ? (
            <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
              {errorMessage}
            </div>
          ) : null}

          <Card>
            <CardHeader>
              <CardTitle>Dados da simulação</CardTitle>
              <CardDescription>
                Defina o período base e descreva o cenário que será comparado com os lançamentos reais.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                  <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                    <FormField
                      control={form.control}
                      name="nome"
                      render={({ field }) => (
                        <FormItem className="xl:col-span-2">
                          <FormLabel>Nome da simulação</FormLabel>
                          <FormControl>
                            <Input placeholder="Ex: compra do sofá parcelado" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="dataInicial"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Mês inicial</FormLabel>
                          <FormControl>
                            <Input type="date" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="quantidadeMeses"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Quantidade de meses</FormLabel>
                          <FormControl>
                            <Input type="number" min="1" max="12" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>

                  <FormField
                    control={form.control}
                    name="descricao"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Descrição</FormLabel>
                        <FormControl>
                          <Textarea
                            placeholder="Explique qual escolha financeira você deseja testar"
                            {...field}
                            value={field.value ?? ""}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <h3 className="text-lg font-semibold">Ações da simulação</h3>
                        <p className="text-sm text-muted-foreground">
                          Cada ação representa uma mudança hipotética, sem criar lançamentos reais.
                        </p>
                      </div>
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() =>
                          append({
                            tipoAcao: 1,
                            descricao: "",
                            valor: 0,
                            dataInicial: form.getValues("dataInicial") || new Date().toISOString().slice(0, 10),
                            dataFinal: "",
                            quantidadeParcelas: null,
                            observacao: "",
                          })
                        }
                      >
                        <Plus className="mr-2 h-4 w-4" />
                        Nova ação
                      </Button>
                    </div>

                    <div className="space-y-4">
                      {fields.length === 0 ? (
                        <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                          Nenhuma ação cadastrada ainda. Adicione a primeira para começar a simulação.
                        </div>
                      ) : (
                        fields.map((field, index) => {
                          const tipoAtual = Number(acoes?.[index]?.tipoAcao ?? field.tipoAcao);

                          return (
                            <div
                              key={field.id}
                              className="space-y-4 rounded-lg border p-4"
                            >
                              <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                                <FormField
                                  control={form.control}
                                  name={`acoes.${index}.tipoAcao`}
                                  render={({ field: acaoField }) => (
                                    <FormItem>
                                      <FormLabel>Tipo de ação</FormLabel>
                                      <Select
                                        onValueChange={(value) => acaoField.onChange(Number(value))}
                                        value={String(acaoField.value)}
                                      >
                                        <FormControl>
                                          <SelectTrigger>
                                            <SelectValue placeholder="Selecione o tipo" />
                                          </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                          <SelectItem value="0">Receita única</SelectItem>
                                          <SelectItem value="1">Despesa única</SelectItem>
                                          <SelectItem value="2">Receita recorrente mensal</SelectItem>
                                          <SelectItem value="3">Despesa recorrente mensal</SelectItem>
                                          <SelectItem value="4">Despesa parcelada</SelectItem>
                                        </SelectContent>
                                      </Select>
                                      <FormMessage />
                                    </FormItem>
                                  )}
                                />

                                <FormField
                                  control={form.control}
                                  name={`acoes.${index}.descricao`}
                                  render={({ field: acaoField }) => (
                                    <FormItem className="xl:col-span-2">
                                      <FormLabel>Descrição</FormLabel>
                                      <FormControl>
                                        <Input placeholder="Ex: compra do sofá" {...acaoField} />
                                      </FormControl>
                                      <FormMessage />
                                    </FormItem>
                                  )}
                                />

                                <FormField
                                  control={form.control}
                                  name={`acoes.${index}.valor`}
                                  render={({ field: acaoField }) => (
                                    <FormItem>
                                      <FormLabel>Valor</FormLabel>
                                      <FormControl>
                                        <Input type="number" step="0.01" min="0" {...acaoField} />
                                      </FormControl>
                                      <FormMessage />
                                    </FormItem>
                                  )}
                                />
                              </div>

                              <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                                <FormField
                                  control={form.control}
                                  name={`acoes.${index}.dataInicial`}
                                  render={({ field: acaoField }) => (
                                    <FormItem>
                                      <FormLabel>Data inicial</FormLabel>
                                      <FormControl>
                                        <Input type="date" {...acaoField} />
                                      </FormControl>
                                      <FormMessage />
                                    </FormItem>
                                  )}
                                />

                                {isRecurring(tipoAtual) ? (
                                  <FormField
                                    control={form.control}
                                    name={`acoes.${index}.dataFinal`}
                                    render={({ field: acaoField }) => (
                                      <FormItem>
                                        <FormLabel>Data final</FormLabel>
                                        <FormControl>
                                          <Input type="date" {...acaoField} value={acaoField.value ?? ""} />
                                        </FormControl>
                                        <FormMessage />
                                      </FormItem>
                                    )}
                                  />
                                ) : (
                                  <div />
                                )}

                                {isParcelado(tipoAtual) ? (
                                  <FormField
                                    control={form.control}
                                    name={`acoes.${index}.quantidadeParcelas`}
                                    render={({ field: acaoField }) => (
                                      <FormItem>
                                        <FormLabel>Quantidade de parcelas</FormLabel>
                                        <FormControl>
                                          <Input
                                            type="number"
                                            min="2"
                                            {...acaoField}
                                            value={acaoField.value ?? ""}
                                          />
                                        </FormControl>
                                        <FormMessage />
                                      </FormItem>
                                    )}
                                  />
                                ) : (
                                  <div />
                                )}

                                <div className="flex items-end justify-end">
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="text-destructive hover:text-destructive"
                                    onClick={() => remove(index)}
                                  >
                                    <Trash2 className="h-4 w-4" />
                                  </Button>
                                </div>
                              </div>

                              <FormField
                                control={form.control}
                                name={`acoes.${index}.observacao`}
                                render={({ field: acaoField }) => (
                                  <FormItem>
                                    <FormLabel>Observação</FormLabel>
                                    <FormControl>
                                      <Textarea
                                        placeholder="Contexto opcional desta ação simulada"
                                        {...acaoField}
                                        value={acaoField.value ?? ""}
                                      />
                                    </FormControl>
                                    <FormMessage />
                                  </FormItem>
                                )}
                              />
                            </div>
                          );
                        })
                      )}
                    </div>
                  </div>
                </form>
              </Form>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
                <div>
                  <CardTitle>Resultado da simulação</CardTitle>
                  <CardDescription className="mt-2">
                    Compare mês a mês o fluxo real com o impacto das ações simuladas.
                  </CardDescription>
                </div>
                <Button type="button" onClick={handleSalvarECalcular} disabled={isBusy}>
                  <Sparkles className="mr-2 h-4 w-4" />
                  {isCalculating ? "Calculando..." : "Salvar e calcular"}
                </Button>
              </div>
            </CardHeader>
            <CardContent className="space-y-6">
              {resultado ? (
                <>
                  <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                    <div className="rounded-2xl border bg-muted/20 p-4">
                      <p className="text-sm text-muted-foreground">Receitas reais</p>
                      <p className="mt-2 text-xl font-semibold">
                        {formatCurrency(resultado.totalReceitasReais)}
                      </p>
                    </div>
                    <div className="rounded-2xl border bg-muted/20 p-4">
                      <p className="text-sm text-muted-foreground">Despesas reais</p>
                      <p className="mt-2 text-xl font-semibold">
                        {formatCurrency(resultado.totalDespesasReais)}
                      </p>
                    </div>
                    <div className="rounded-2xl border bg-muted/20 p-4">
                      <p className="text-sm text-muted-foreground">Receitas simuladas</p>
                      <p className="mt-2 text-xl font-semibold">
                        {formatCurrency(resultado.totalReceitasSimuladas)}
                      </p>
                    </div>
                    <div className="rounded-2xl border bg-muted/20 p-4">
                      <p className="text-sm text-muted-foreground">Despesas simuladas</p>
                      <p className="mt-2 text-xl font-semibold">
                        {formatCurrency(resultado.totalDespesasSimuladas)}
                      </p>
                    </div>
                  </div>

                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Mês</TableHead>
                        <TableHead>Receitas reais</TableHead>
                        <TableHead>Despesas reais</TableHead>
                        <TableHead>Saldo real</TableHead>
                        <TableHead>Receitas simuladas</TableHead>
                        <TableHead>Despesas simuladas</TableHead>
                        <TableHead>Saldo simulado</TableHead>
                        <TableHead>Diferença</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {resultado.linhas.map((linha) => (
                        <TableRow key={linha.mesReferencia}>
                          <TableCell className="font-medium">{formatMonth(linha.mesReferencia)}</TableCell>
                          <TableCell>{formatCurrency(linha.receitasReais)}</TableCell>
                          <TableCell>{formatCurrency(linha.despesasReais)}</TableCell>
                          <TableCell>{formatCurrency(linha.saldoReal)}</TableCell>
                          <TableCell>{formatCurrency(linha.receitasSimuladas)}</TableCell>
                          <TableCell>{formatCurrency(linha.despesasSimuladas)}</TableCell>
                          <TableCell>{formatCurrency(linha.saldoSimulado)}</TableCell>
                          <TableCell>{formatCurrency(linha.diferenca)}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </>
              ) : (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Salve e calcule a simulação para visualizar o comparativo real x simulado.
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  );
}
