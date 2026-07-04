"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { useFieldArray, useForm, useWatch } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowLeft, Plus, Save, Sparkles, Trash2 } from "lucide-react";
import { Cell, Pie, PieChart } from "recharts";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import {
  ProjecaoDetalhe,
  RendaExtraMensalProjecaoInput,
  ResultadoProjecao,
} from "@/types/projecao";
import { buscarProjecao, calcularProjecaoSalva, editarProjecao } from "@/services/api/projecao";
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
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ChartContainer, ChartTooltip, ChartTooltipContent } from "@/components/ui/chart";

const rendaSchema = z.object({
  nome: z.string().min(2, "Informe o nome da renda."),
  valorMensal: z.coerce.number().min(0, "Informe um valor valido."),
});

const formSchema = z.object({
  nome: z.string().min(2, "Informe o nome da projecao."),
  dataInicial: z.string().optional(),
  valorAcumuladoInicial: z.coerce.number().min(0, "O acumulado nao pode ser negativo."),
  valorObjetivo: z.coerce.number().min(0, "Informe um valor valido."),
  mesesLimite: z.coerce.number().min(1, "Informe ao menos 1 mes.").max(240, "Use no maximo 240 meses."),
  rendas: z.array(rendaSchema).min(1, "Informe ao menos uma renda."),
});

type FormValues = z.infer<typeof formSchema>;

const progressChartConfig = {
  concluido: {
    label: "Concluido",
    color: "hsl(150 59% 42%)",
  },
  restante: {
    label: "Restante",
    color: "hsl(24 95% 58%)",
  },
};

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function formatMonth(value?: string | null) {
  if (!value) {
    return "Ainda nao projetado";
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

function normalizeMonthInput(value?: string | null) {
  if (!value) {
    return "";
  }

  return value.slice(0, 7);
}

function toMonthlyExtrasMap(items: RendaExtraMensalProjecaoInput[] = []) {
  return items.reduce<Record<string, number>>((acc, item) => {
    acc[item.mesReferencia] = Number(item.valor) || 0;
    return acc;
  }, {});
}

interface ProjecaoManagerProps {
  projecaoId: string;
}

export function ProjecaoManager({ projecaoId }: ProjecaoManagerProps) {
  const { session } = useAuth();
  const [projecao, setProjecao] = useState<ProjecaoDetalhe | null>(null);
  const [resultado, setResultado] = useState<ResultadoProjecao | null>(null);
  const [extrasPorMes, setExtrasPorMes] = useState<Record<string, number>>({});
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isCalculating, setIsCalculating] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      nome: "",
      dataInicial: "",
      valorAcumuladoInicial: 0,
      valorObjetivo: 0,
      mesesLimite: 60,
      rendas: [{ nome: "Salario principal", valorMensal: 0 }],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "rendas",
  });

  const rendasWatch = useWatch({
    control: form.control,
    name: "rendas",
  });

  const valorObjetivoWatch = useWatch({
    control: form.control,
    name: "valorObjetivo",
  });

  const valorAcumuladoInicialWatch = useWatch({
    control: form.control,
    name: "valorAcumuladoInicial",
  });

  const rendaBaseTotal = useMemo(() => {
    return (rendasWatch ?? []).reduce(
      (total, renda) => total + (Number(renda?.valorMensal) || 0),
      0
    );
  }, [rendasWatch]);

  const valorRestanteAtual = useMemo(() => {
    if (resultado) {
      return resultado.valorRestanteParaObjetivo;
    }

    return Math.max(
      0,
      (Number(valorObjetivoWatch) || 0) - (Number(valorAcumuladoInicialWatch) || 0)
    );
  }, [resultado, valorAcumuladoInicialWatch, valorObjetivoWatch]);

  const percentualConcluidoAtual = useMemo(() => {
    if (resultado) {
      return Number(resultado.percentualConcluido) || 0;
    }

    const objetivo = Number(valorObjetivoWatch) || 0;
    const acumulado = Number(valorAcumuladoInicialWatch) || 0;

    if (objetivo <= 0) {
      return 0;
    }

    return Math.min(100, Math.max(0, (acumulado / objetivo) * 100));
  }, [resultado, valorAcumuladoInicialWatch, valorObjetivoWatch]);

  const progressChartData = useMemo(() => {
    const concluido = Number(percentualConcluidoAtual.toFixed(2));
    const restante = Math.max(0, Number((100 - concluido).toFixed(2)));

    return [
      { name: "concluido", value: concluido, fill: "var(--color-concluido)" },
      { name: "restante", value: restante, fill: "var(--color-restante)" },
    ];
  }, [percentualConcluidoAtual]);

  const linhasComExtras = useMemo(() => {
    return (resultado?.linhas ?? []).map((linha) => ({
      ...linha,
      rendaExtraMensal: extrasPorMes[linha.mesReferencia] ?? linha.rendaExtraMensal ?? 0,
    }));
  }, [extrasPorMes, resultado?.linhas]);

  async function carregarProjecao() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarProjecao(session.usuario.id, projecaoId, session.token);
      const dados = response.dados;

      if (!dados) {
        setErrorMessage("Projecao nao encontrada.");
        return;
      }

      setProjecao(dados);
      setExtrasPorMes(toMonthlyExtrasMap(dados.rendasExtrasMensais));

      form.reset({
        nome: dados.nome,
        dataInicial: normalizeMonthInput(dados.dataInicial),
        valorAcumuladoInicial: dados.valorAcumuladoInicial,
        valorObjetivo: dados.valorObjetivo,
        mesesLimite: dados.mesesLimite,
        rendas:
          dados.rendas.length > 0
            ? dados.rendas
            : [{ nome: "Salario principal", valorMensal: 0 }],
      });

      setResultado(dados.resultadoAtual ?? null);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel carregar a projecao.");
      }
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void carregarProjecao();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [projecaoId, session?.token, session?.usuario.id]);

  function buildExtrasPayload() {
    return Object.entries(extrasPorMes)
      .filter(([, valor]) => Number(valor) > 0)
      .map(([mesReferencia, valor]) => ({
        mesReferencia,
        valor: Number(valor) || 0,
      }));
  }

  async function salvarProjecao(values: FormValues) {
    if (!session?.usuario.id || !session.token) {
      throw new Error("Sessao invalida.");
    }

    await editarProjecao(
      session.usuario.id,
      projecaoId,
      {
        nome: values.nome,
        dataInicial: values.dataInicial ? `${values.dataInicial}-01T00:00:00` : null,
        valorAcumuladoInicial: values.valorAcumuladoInicial,
        valorObjetivo: values.valorObjetivo,
        mesesLimite: values.mesesLimite,
        rendas: values.rendas.map((renda) => ({
          nome: renda.nome,
          valorMensal: renda.valorMensal,
        })),
        rendasExtrasMensais: buildExtrasPayload(),
      },
      session.token
    );
  }

  async function onSubmit(values: FormValues) {
    try {
      setIsSaving(true);
      setErrorMessage("");
      await salvarProjecao(values);
      await carregarProjecao();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else if (error instanceof Error) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel salvar a projecao.");
      }
    } finally {
      setIsSaving(false);
    }
  }

  async function handleSalvarECalcular() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
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

      await salvarProjecao(values);
      const response = await calcularProjecaoSalva(session.usuario.id, projecaoId, session.token);
      setResultado(response.dados ?? null);
      setProjecao((current) =>
        current
          ? {
              ...current,
              rendasExtrasMensais: buildExtrasPayload(),
            }
          : current
      );
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else if (error instanceof Error) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel calcular a projecao.");
      }
    } finally {
      setIsCalculating(false);
    }
  }

  return (
    <main className="flex-1 px-6 py-8 md:px-8">
      <div className="mx-auto max-w-7xl space-y-6">
        <Card className="border-0 shadow-none">
          <CardHeader className="px-0 pt-0">
            <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
              <div className="space-y-3">
                <Button asChild variant="ghost" className="w-fit px-0 text-muted-foreground">
                  <Link href="/projecao">
                    <ArrowLeft className="mr-2 h-4 w-4" />
                    Voltar para as projecoes
                  </Link>
                </Button>
                <div>
                  <CardTitle className="text-3xl">Projecao detalhada</CardTitle>
                  <CardDescription className="mt-2 max-w-3xl text-base">
                    A renda base fica no cadastro da projecao. A renda extra agora pode ser ajustada por mes diretamente na tabela.
                  </CardDescription>
                </div>
              </div>

              <div className="flex flex-wrap gap-3">
                <Button
                  type="button"
                  variant="outline"
                  onClick={form.handleSubmit(onSubmit)}
                  disabled={isLoading || isSaving || isCalculating}
                >
                  <Save className="mr-2 h-4 w-4" />
                  {isSaving ? "Salvando..." : "Salvar alteracoes"}
                </Button>
                <Button
                  type="button"
                  onClick={handleSalvarECalcular}
                  disabled={isLoading || isSaving || isCalculating}
                >
                  <Sparkles className="mr-2 h-4 w-4" />
                  {isCalculating ? "Calculando..." : "Salvar e gerar projecao"}
                </Button>
              </div>
            </div>
          </CardHeader>
        </Card>

        <div className="grid gap-4 md:grid-cols-3">
          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Renda mensal informada</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(rendaBaseTotal)}</CardTitle>
            </CardHeader>
          </Card>
          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Objetivo projetado</CardDescription>
              <CardTitle className="text-3xl">
                {resultado ? formatMonth(resultado.mesObjetivo) : "A calcular"}
              </CardTitle>
            </CardHeader>
          </Card>
          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Valor restante</CardDescription>
              <CardTitle className="text-3xl">{formatCurrency(valorRestanteAtual)}</CardTitle>
            </CardHeader>
          </Card>
        </div>

        {errorMessage ? (
          <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
            {errorMessage}
          </div>
        ) : null}

        <div className="grid gap-4 lg:grid-cols-[1.2fr_0.8fr]">
          <Card>
            <CardHeader>
              <CardTitle>Dados da projecao</CardTitle>
              <CardDescription>
                As despesas continuam vindo automaticamente dos lancamentos. A renda extra agora e mensal e independente por linha.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                  Carregando projecao...
                </div>
              ) : (
                <Form {...form}>
                  <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                    <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
                      <FormField
                        control={form.control}
                        name="nome"
                        render={({ field }) => (
                          <FormItem className="xl:col-span-2">
                            <FormLabel>Nome da projecao</FormLabel>
                            <FormControl>
                              <Input placeholder="Ex: Reserva de emergencia" {...field} />
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
                            <FormLabel>Mes inicial</FormLabel>
                            <FormControl>
                              <Input type="month" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name="valorAcumuladoInicial"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Acumulado atual</FormLabel>
                            <FormControl>
                              <Input type="number" step="0.01" min="0" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name="valorObjetivo"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Objetivo final</FormLabel>
                            <FormControl>
                              <Input type="number" step="0.01" min="0" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>

                    <div className="grid gap-4 md:max-w-xs">
                      <FormField
                        control={form.control}
                        name="mesesLimite"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Meses a projetar</FormLabel>
                            <FormControl>
                              <Input type="number" min="1" max="240" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>

                    <div className="space-y-4">
                      <div className="flex items-center justify-between">
                        <div>
                          <h3 className="text-lg font-semibold">Rendas base</h3>
                          <p className="text-sm text-muted-foreground">
                            Some salario, aluguel, comissao e outras entradas recorrentes da projecao.
                          </p>
                        </div>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => append({ nome: "", valorMensal: 0 })}
                        >
                          <Plus className="mr-2 h-4 w-4" />
                          Nova renda
                        </Button>
                      </div>

                      <div className="space-y-4">
                        {fields.map((field, index) => (
                          <div
                            key={field.id}
                            className="grid gap-4 rounded-lg border p-4 md:grid-cols-[1fr_220px_auto]"
                          >
                            <FormField
                              control={form.control}
                              name={`rendas.${index}.nome`}
                              render={({ field: rendaField }) => (
                                <FormItem>
                                  <FormLabel>Nome da renda</FormLabel>
                                  <FormControl>
                                    <Input
                                      placeholder="Ex: Salario, freelance, aluguel"
                                      {...rendaField}
                                    />
                                  </FormControl>
                                  <FormMessage />
                                </FormItem>
                              )}
                            />

                            <FormField
                              control={form.control}
                              name={`rendas.${index}.valorMensal`}
                              render={({ field: rendaField }) => (
                                <FormItem>
                                  <FormLabel>Valor mensal</FormLabel>
                                  <FormControl>
                                    <Input type="number" step="0.01" min="0" {...rendaField} />
                                  </FormControl>
                                  <FormMessage />
                                </FormItem>
                              )}
                            />

                            <div className="flex items-end">
                              <Button
                                type="button"
                                variant="ghost"
                                size="icon"
                                className="text-destructive hover:text-destructive"
                                onClick={() => {
                                  if (fields.length > 1) {
                                    remove(index);
                                  }
                                }}
                                disabled={fields.length === 1}
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  </form>
                </Form>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Progresso do objetivo</CardTitle>
              <CardDescription>
                Uma leitura visual do quanto falta para atingir a meta desta projecao.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <ChartContainer config={progressChartConfig} className="mx-auto aspect-square max-h-[260px]">
                <PieChart>
                  <ChartTooltip
                    cursor={false}
                    content={
                      <ChartTooltipContent
                        hideLabel
                        formatter={(value, name) => (
                          <div className="flex min-w-[120px] items-center justify-between gap-3">
                            <span>{name === "concluido" ? "Concluido" : "Restante"}</span>
                            <span className="font-medium">{Number(value).toFixed(1)}%</span>
                          </div>
                        )}
                      />
                    }
                  />
                  <Pie
                    data={progressChartData}
                    dataKey="value"
                    nameKey="name"
                    innerRadius={70}
                    outerRadius={100}
                    strokeWidth={4}
                  >
                    {progressChartData.map((entry) => (
                      <Cell key={entry.name} fill={entry.fill} />
                    ))}
                  </Pie>
                </PieChart>
              </ChartContainer>

              <div className="space-y-3 rounded-2xl border bg-muted/20 p-4">
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Concluido</span>
                  <span className="font-semibold">{percentualConcluidoAtual.toFixed(1)}%</span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Restante</span>
                  <span className="font-semibold">
                    {(100 - percentualConcluidoAtual).toFixed(1)}%
                  </span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Acumulado inicial</span>
                  <span className="font-semibold">
                    {formatCurrency(Number(valorAcumuladoInicialWatch) || 0)}
                  </span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Objetivo final</span>
                  <span className="font-semibold">
                    {formatCurrency(Number(valorObjetivoWatch) || 0)}
                  </span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Resultado da projecao</CardTitle>
            <CardDescription>
              Preencha a renda extra diretamente em cada mes e depois salve ou recalcule a projecao.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {!resultado ? (
              <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                Salve e gere a projecao para visualizar a linha do tempo do objetivo.
              </div>
            ) : linhasComExtras.length === 0 ? (
              <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                Nenhum mes foi gerado para a projecao atual.
              </div>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Mes</TableHead>
                    <TableHead>Dividas totais</TableHead>
                    <TableHead>Renda Extra</TableHead>
                    <TableHead>Sobra do mes</TableHead>
                    <TableHead>Acumulado</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {linhasComExtras.map((linha) => (
                    <TableRow key={linha.mesReferencia}>
                      <TableCell className="font-medium">{formatMonth(linha.mesReferencia)}</TableCell>
                      <TableCell>{formatCurrency(linha.dividasTotais)}</TableCell>
                      <TableCell className="min-w-[180px]">
                        <Input
                          type="number"
                          step="0.01"
                          min="0"
                          value={extrasPorMes[linha.mesReferencia] ?? 0}
                          onChange={(event) => {
                            const valor = Number(event.target.value) || 0;
                            setExtrasPorMes((current) => ({
                              ...current,
                              [linha.mesReferencia]: valor,
                            }));
                          }}
                        />
                      </TableCell>
                      <TableCell>{formatCurrency(linha.sobraDoMes)}</TableCell>
                      <TableCell>{formatCurrency(linha.acumuladoProjetado)}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
