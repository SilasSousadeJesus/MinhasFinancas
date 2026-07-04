"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowLeft, Plus, Save, Sparkles, Trash2 } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { ResultadoProjecao } from "@/types/projecao";
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

interface ProjecaoManagerProps {
  projecaoId: string;
}

export function ProjecaoManager({ projecaoId }: ProjecaoManagerProps) {
  const { session } = useAuth();
  const [resultado, setResultado] = useState<ResultadoProjecao | null>(null);
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

  const rendaTotalInformada = useMemo(() => {
    return form
      .watch("rendas")
      .reduce((total, renda) => total + (Number(renda.valorMensal) || 0), 0);
  }, [form]);

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
      const projecao = response.dados;

      if (!projecao) {
        setErrorMessage("Projecao nao encontrada.");
        return;
      }

      form.reset({
        nome: projecao.nome,
        dataInicial: normalizeMonthInput(projecao.dataInicial),
        valorAcumuladoInicial: projecao.valorAcumuladoInicial,
        valorObjetivo: projecao.valorObjetivo,
        mesesLimite: projecao.mesesLimite,
        rendas:
          projecao.rendas.length > 0
            ? projecao.rendas
            : [{ nome: "Salario principal", valorMensal: 0 }],
      });

      setResultado(projecao.resultadoAtual ?? null);
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
                    Ajuste as rendas, o acumulado atual e o objetivo final. O sistema
                    combina esses dados com os lancamentos futuros ja cadastrados.
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
              <CardTitle className="text-3xl">{formatCurrency(rendaTotalInformada)}</CardTitle>
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
              <CardTitle className="text-3xl">
                {resultado
                  ? formatCurrency(resultado.valorRestanteParaObjetivo)
                  : formatCurrency(0)}
              </CardTitle>
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
            <CardTitle>Dados da projecao</CardTitle>
            <CardDescription>
              Cada projecao pode ter varias fontes de renda e um horizonte proprio de simulacao.
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
                        <h3 className="text-lg font-semibold">Rendas</h3>
                        <p className="text-sm text-muted-foreground">
                          Some salario, renda extra, aluguel, comissao e qualquer outra entrada mensal.
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
            <CardTitle>Resultado da projecao</CardTitle>
            <CardDescription>
              Dividas totais por mes, sobra mensal e evolucao do acumulado ate o objetivo.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {!resultado ? (
              <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                Salve e gere a projecao para visualizar a linha do tempo do objetivo.
              </div>
            ) : resultado.linhas.length === 0 ? (
              <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                Nenhum mes foi gerado para a projecao atual.
              </div>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Mes</TableHead>
                    <TableHead>Dividas totais</TableHead>
                    <TableHead>Receitas dos lancamentos</TableHead>
                    <TableHead>Sobra do mes</TableHead>
                    <TableHead>Acumulado</TableHead>
                    <TableHead>Status</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {resultado.linhas.map((linha) => (
                    <TableRow key={linha.mesReferencia}>
                      <TableCell className="font-medium">{formatMonth(linha.mesReferencia)}</TableCell>
                      <TableCell>{formatCurrency(linha.dividasTotais)}</TableCell>
                      <TableCell>{formatCurrency(linha.receitasDosLancamentos)}</TableCell>
                      <TableCell>{formatCurrency(linha.sobraDoMes)}</TableCell>
                      <TableCell>{formatCurrency(linha.acumuladoProjetado)}</TableCell>
                      <TableCell>
                        {linha.objetivoAtingidoNoMes ? "Objetivo atingido" : "Em progresso"}
                      </TableCell>
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
