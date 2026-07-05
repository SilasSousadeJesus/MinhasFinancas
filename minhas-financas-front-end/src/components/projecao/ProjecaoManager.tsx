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
  DividaManualMensalProjecaoInput,
  LinhaResultadoProjecao,
  ProjecaoDetalhe,
  RendaExtraMensalProjecaoInput,
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
import { Switch } from "@/components/ui/switch";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ChartContainer, ChartTooltip, ChartTooltipContent } from "@/components/ui/chart";
import { ProjecaoLoadingOverlay } from "./ProjecaoLoadingOverlay";

const rendaSchema = z.object({
  nome: z.string().min(2, "Informe o nome da renda."),
  valorMensal: z.coerce.number().min(0, "Informe um valor valido."),
});

const formSchema = z.object({
  nome: z.string().min(2, "Informe o nome da projeção."),
  dataInicial: z.string().optional(),
  valorAcumuladoInicial: z.coerce.number().min(0, "O acumulado nao pode ser negativo."),
  valorObjetivo: z.coerce.number().min(0, "Informe um valor valido."),
  mesesLimite: z.coerce.number().min(1, "Informe ao menos 1 mes.").max(240, "Use no maximo 240 meses."),
  atreladaADespesas: z.boolean(),
  rendas: z.array(rendaSchema).min(1, "Informe ao menos uma renda."),
});

type FormValues = z.infer<typeof formSchema>;

const progressChartConfig = {
  concluido: {
    label: "Concluído",
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
    return "Ainda nÃ£o projetado";
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

function toMonthlyDebtsMap(items: DividaManualMensalProjecaoInput[] = []) {
  return items.reduce<Record<string, number>>((acc, item) => {
    acc[item.mesReferencia] = Number(item.valor) || 0;
    return acc;
  }, {});
}

function buildMonths(dataInicial: string, mesesLimite: number) {
  if (!dataInicial || mesesLimite <= 0) {
    return [];
  }

  const [year, month] = dataInicial.split("-").map(Number);
  if (!year || !month) {
    return [];
  }

  return Array.from({ length: mesesLimite }, (_, index) => {
    const current = new Date(year, month - 1 + index, 1);
    return `${current.getFullYear()}-${String(current.getMonth() + 1).padStart(2, "0")}`;
  });
}

interface ProjecaoManagerProps {
  projecaoId: string;
}

export function ProjecaoManager({ projecaoId }: ProjecaoManagerProps) {
  const { session } = useAuth();
  const [projecao, setProjecao] = useState<ProjecaoDetalhe | null>(null);
  const [extrasPorMes, setExtrasPorMes] = useState<Record<string, number>>({});
  const [dividasPorMes, setDividasPorMes] = useState<Record<string, number>>({});
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
      atreladaADespesas: true,
      rendas: [{ nome: "SalÃ¡rio principal", valorMensal: 0 }],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "rendas",
  });

  const rendasWatch = useWatch({ control: form.control, name: "rendas" });
  const valorObjetivoWatch = useWatch({ control: form.control, name: "valorObjetivo" });
  const valorAcumuladoInicialWatch = useWatch({
    control: form.control,
    name: "valorAcumuladoInicial",
  });
  const dataInicialWatch = useWatch({ control: form.control, name: "dataInicial" });
  const mesesLimiteWatch = useWatch({ control: form.control, name: "mesesLimite" });
  const atreladaADespesasWatch = useWatch({
    control: form.control,
    name: "atreladaADespesas",
  });

  const rendaBaseTotal = useMemo(() => {
    return (rendasWatch ?? []).reduce(
      (total, renda) => total + (Number(renda?.valorMensal) || 0),
      0
    );
  }, [rendasWatch]);

  const valorRestanteAtual = useMemo(() => {
    return Math.max(
      0,
      (Number(valorObjetivoWatch) || 0) - (Number(valorAcumuladoInicialWatch) || 0)
    );
  }, [valorAcumuladoInicialWatch, valorObjetivoWatch]);

  const percentualConcluidoAtual = useMemo(() => {
    const objetivo = Number(valorObjetivoWatch) || 0;
    const acumulado = Number(valorAcumuladoInicialWatch) || 0;

    if (objetivo <= 0) {
      return 0;
    }

    return Math.min(100, Math.max(0, (acumulado / objetivo) * 100));
  }, [valorAcumuladoInicialWatch, valorObjetivoWatch]);

  const progressChartData = useMemo(() => {
    const concluido = Number(percentualConcluidoAtual.toFixed(2));
    const restante = Math.max(0, Number((100 - concluido).toFixed(2)));

    return [
      { name: "concluido", value: concluido, fill: "var(--color-concluido)" },
      { name: "restante", value: restante, fill: "var(--color-restante)" },
    ];
  }, [percentualConcluidoAtual]);

  const mesesDaProjecao = useMemo(() => {
    const dataBase = dataInicialWatch || normalizeMonthInput(projecao?.dataInicial) || "";
    return buildMonths(dataBase, Number(mesesLimiteWatch) || 0);
  }, [dataInicialWatch, mesesLimiteWatch, projecao?.dataInicial]);

  const linhasOriginaisPorMes = useMemo(() => {
    return (projecao?.resultadoAtual?.linhas ?? []).reduce<Record<string, LinhaResultadoProjecao>>(
      (acc, linha) => {
      acc[linha.mesReferencia] = linha;
      return acc;
      },
      {}
    );
  }, [projecao?.resultadoAtual?.linhas]);

  const previewRows = useMemo(() => {
    let acumuladoAtual = Number(valorAcumuladoInicialWatch) || 0;

    return mesesDaProjecao.map((mesReferencia) => {
      const linhaOriginal = (linhasOriginaisPorMes as Record<string, any>)[mesReferencia];
      const rendaExtraMensal = Number(extrasPorMes[mesReferencia] ?? linhaOriginal?.rendaExtraMensal ?? 0);
      const dividasTotais = atreladaADespesasWatch
        ? Number(linhaOriginal?.dividasTotais ?? 0)
        : Number(dividasPorMes[mesReferencia] ?? linhaOriginal?.dividasTotais ?? 0);
      const receitaTotalMes = rendaBaseTotal + rendaExtraMensal;
      const sobraDoMes = receitaTotalMes - dividasTotais;
      acumuladoAtual += sobraDoMes;

      return {
        mesReferencia,
        dividasTotais,
        dividasEditaveis: !atreladaADespesasWatch,
        rendaExtraMensal,
        rendaManualTotal: rendaBaseTotal,
        receitaTotalMes,
        sobraDoMes,
        acumuladoProjetado: acumuladoAtual,
        objetivoAtingidoNoMes: acumuladoAtual >= (Number(valorObjetivoWatch) || 0),
      };
    });
  }, [
    atreladaADespesasWatch,
    dividasPorMes,
    extrasPorMes,
    linhasOriginaisPorMes,
    mesesDaProjecao,
    rendaBaseTotal,
    valorAcumuladoInicialWatch,
    valorObjetivoWatch,
  ]);

  const objetivoProjetado = useMemo(() => {
    const linha = previewRows.find((item) => item.objetivoAtingidoNoMes);
    return linha?.mesReferencia ?? null;
  }, [previewRows]);

  async function carregarProjecao() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("SessÃ£o invÃ¡lida. FaÃ§a login novamente.");
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarProjecao(session.usuario.id, projecaoId, session.token);
      const dados = response.dados;

      if (!dados) {
        setErrorMessage("Projeção não encontrada.");
        return;
      }

      setProjecao(dados);
      setExtrasPorMes(toMonthlyExtrasMap(dados.rendasExtrasMensais));
      setDividasPorMes(toMonthlyDebtsMap(dados.dividasManuaisMensais));

      form.reset({
        nome: dados.nome,
        dataInicial: normalizeMonthInput(dados.dataInicial),
        valorAcumuladoInicial: dados.valorAcumuladoInicial,
        valorObjetivo: dados.valorObjetivo,
        mesesLimite: dados.mesesLimite,
        atreladaADespesas: dados.atreladaADespesas,
        rendas:
          dados.rendas.length > 0
            ? dados.rendas
            : [{ nome: "SalÃ¡rio principal", valorMensal: 0 }],
      });
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar a projeção.");
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

  function buildDebtsPayload() {
    return Object.entries(dividasPorMes)
      .filter(([, valor]) => Number(valor) >= 0)
      .map(([mesReferencia, valor]) => ({
        mesReferencia,
        valor: Number(valor) || 0,
      }));
  }

  async function salvarProjecao(values: FormValues) {
    if (!session?.usuario.id || !session.token) {
      throw new Error("SessÃ£o invÃ¡lida.");
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
        atreladaADespesas: values.atreladaADespesas,
        rendas: values.rendas.map((renda) => ({
          nome: renda.nome,
          valorMensal: renda.valorMensal,
        })),
        rendasExtrasMensais: buildExtrasPayload(),
        dividasManuaisMensais: buildDebtsPayload(),
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
        setErrorMessage("Não foi possível salvar a projeção.");
      }
    } finally {
      setIsSaving(false);
    }
  }

  async function handleSalvarECalcular() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("SessÃ£o invÃ¡lida. FaÃ§a login novamente.");
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
      await carregarProjecao();
      await calcularProjecaoSalva(session.usuario.id, projecaoId, session.token);
      await carregarProjecao();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else if (error instanceof Error) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível calcular a projeção.");
      }
    } finally {
      setIsCalculating(false);
    }
  }

  const isBusy = isLoading || isSaving || isCalculating;

  return (
    <main className="relative flex-1 px-6 py-8 md:px-8">
      <ProjecaoLoadingOverlay
        visible={isBusy}
        message={
          isLoading
            ? "Carregando projeção..."
            : isCalculating
              ? "Salvando e gerando projeção..."
              : "Salvando projeção..."
        }
      />

      <div className="mx-auto max-w-7xl space-y-6">
        <Card className="border-0 shadow-none">
          <CardHeader className="px-0 pt-0">
            <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
              <div className="space-y-3">
                <Button asChild variant="ghost" className="w-fit px-0 text-muted-foreground">
                  <Link href="/projecao">
                    <ArrowLeft className="mr-2 h-4 w-4" />
                    Voltar para Projeções
                  </Link>
                </Button>
                <div>
                  <CardTitle className="text-3xl">Projeção detalhada</CardTitle>
                  <CardDescription className="mt-2 max-w-3xl text-base">
                    A renda base fica no cadastro da projeção. Preencha a renda extra
                    diretamente em cada mês e, se a projeção não estiver atrelada a despesas,
                    edite também as dívidas mensais.
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
                  {isCalculating ? "Calculando..." : "Salvar e gerar Projeção"}
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
                {objetivoProjetado ? formatMonth(objetivoProjetado) : "A calcular"}
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
              <CardTitle>Dados da projeção</CardTitle>
              <CardDescription>
                Some salário, aluguel, comissão e outras entradas recorrentes da projeção.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                  <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
                    <FormField
                      control={form.control}
                      name="nome"
                      render={({ field }) => (
                        <FormItem className="xl:col-span-2">
                          <FormLabel>Nome da projeção</FormLabel>
                          <FormControl>
                            <Input placeholder="Ex: Reserva de emergência" {...field} />
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

                  <div className="grid gap-4 md:grid-cols-[240px_1fr]">
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

                    <FormField
                      control={form.control}
                      name="atreladaADespesas"
                      render={({ field }) => (
                        <FormItem className="flex flex-row items-center justify-between rounded-xl border p-4">
                          <div className="space-y-1">
                            <FormLabel>Atrelada a despesas</FormLabel>
                            <p className="text-sm text-muted-foreground">
                              Se estiver em &quot;Sim&quot;, a projeção usa as despesas dos lançamentos.
                              Se estiver em &quot;Não&quot;, a coluna de dívidas fica livre para edição.
                            </p>
                          </div>
                          <FormControl>
                            <Switch checked={field.value} onCheckedChange={field.onChange} />
                          </FormControl>
                        </FormItem>
                      )}
                    />
                  </div>

                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <h3 className="text-lg font-semibold">Rendas base</h3>
                        <p className="text-sm text-muted-foreground">
                          Some salário, aluguel, comissão e outras entradas recorrentes da projeção.
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
                                    placeholder="Ex: salário, freelance, aluguel"
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
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Progresso do objetivo</CardTitle>
              <CardDescription>
                Uma leitura visual do quanto falta para atingir a meta desta projeção.
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
                            <span>{name === "concluido" ? "Concluído" : "Restante"}</span>
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
                  <span className="text-muted-foreground">Concluído</span>
                  <span className="font-semibold">{percentualConcluidoAtual.toFixed(1)}%</span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Restante</span>
                  <span className="font-semibold">{(100 - percentualConcluidoAtual).toFixed(1)}%</span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Acumulado atual</span>
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
            <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
              <div>
                <CardTitle>Resultado da projeção</CardTitle>
                <CardDescription className="mt-2">
                  Preencha a renda extra diretamente em cada mês e depois salve ou recalcule a projeção.
                </CardDescription>
              </div>
              <Button type="button" onClick={handleSalvarECalcular} disabled={isBusy}>
                <Sparkles className="mr-2 h-4 w-4" />
                {isCalculating ? "Calculando..." : "Salvar e gerar projeção"}
              </Button>
            </div>
          </CardHeader>
          <CardContent>
            {previewRows.length === 0 ? (
              <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                Salve e gere a projeção para visualizar a linha do tempo do objetivo.
              </div>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Mês</TableHead>
                    <TableHead>Dívidas totais</TableHead>
                    <TableHead>Renda Extra</TableHead>
                    <TableHead>Sobra do mês</TableHead>
                    <TableHead>Acumulado</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {previewRows.map((linha) => (
                    <TableRow key={linha.mesReferencia}>
                      <TableCell className="font-medium">{formatMonth(linha.mesReferencia)}</TableCell>
                      <TableCell className="min-w-[180px]">
                        {linha.dividasEditaveis ? (
                          <Input
                            type="number"
                            step="0.01"
                            min="0"
                            value={dividasPorMes[linha.mesReferencia] ?? linha.dividasTotais ?? 0}
                            onChange={(event) => {
                              const valor = Number(event.target.value) || 0;
                              setDividasPorMes((current) => ({
                                ...current,
                                [linha.mesReferencia]: valor,
                              }));
                            }}
                          />
                        ) : (
                          formatCurrency(linha.dividasTotais)
                        )}
                      </TableCell>
                      <TableCell className="min-w-[180px]">
                        <Input
                          type="number"
                          step="0.01"
                          min="0"
                          value={extrasPorMes[linha.mesReferencia] ?? linha.rendaExtraMensal ?? 0}
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
