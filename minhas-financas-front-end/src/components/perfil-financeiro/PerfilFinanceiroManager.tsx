"use client";

import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Sidebar } from "@/components/Sidebar/Sidebar";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import {
  ConfiguracaoPerfilFinanceiro,
  SalvarPerfilFinanceiroPayload,
  VisaoGeralPerfilFinanceiro,
} from "@/types/perfil-financeiro";
import {
  buscarPerfilFinanceiro,
  salvarPerfilFinanceiro,
} from "@/services/api/perfil-financeiro";
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
import { Textarea } from "@/components/ui/textarea";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";

const formSchema = z.object({
  percentualEconomiaMensalDesejado: z.coerce.number().min(0, "Informe um percentual válido."),
  percentualReservaEmergenciaDesejado: z.coerce.number().min(0, "Informe um percentual válido."),
  mesesReservaEmergenciaDesejados: z.coerce.number().int().min(0, "Informe uma quantidade válida."),
  percentualMaximoComprometimentoRenda: z.coerce.number().min(0, "Informe um percentual válido."),
  percentualMaximoEndividamento: z.coerce.number().min(0, "Informe um percentual válido."),
  percentualMinimoInvestimento: z.coerce.number().min(0, "Informe um percentual válido."),
  patrimonioLiquidoAlvo: z
    .union([z.coerce.number().min(0, "Informe um valor válido."), z.nan()])
    .optional(),
  observacao: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

function formatPercentual(value: number) {
  return `${value.toLocaleString("pt-BR", {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  })}%`;
}

function formatCurrency(value?: number | null) {
  if (value == null) return "—";

  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value);
}

function formatDate(value?: string | null) {
  if (!value) return "Vigente";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "—";

  return date.toLocaleDateString("pt-BR");
}

function mapToFormValues(configuracao?: ConfiguracaoPerfilFinanceiro | null): FormValues {
  return {
    percentualEconomiaMensalDesejado: configuracao?.percentualEconomiaMensalDesejado ?? 20,
    percentualReservaEmergenciaDesejado: configuracao?.percentualReservaEmergenciaDesejado ?? 100,
    mesesReservaEmergenciaDesejados: configuracao?.mesesReservaEmergenciaDesejados ?? 6,
    percentualMaximoComprometimentoRenda: configuracao?.percentualMaximoComprometimentoRenda ?? 50,
    percentualMaximoEndividamento: configuracao?.percentualMaximoEndividamento ?? 50,
    percentualMinimoInvestimento: configuracao?.percentualMinimoInvestimento ?? 10,
    patrimonioLiquidoAlvo: configuracao?.patrimonioLiquidoAlvo ?? 0,
    observacao: configuracao?.observacao ?? "",
  };
}

export function PerfilFinanceiroManager() {
  const { session } = useAuth();
  const [visaoGeral, setVisaoGeral] = useState<VisaoGeralPerfilFinanceiro | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: mapToFormValues(null),
  });

  async function carregarPerfilFinanceiro() {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarPerfilFinanceiro(session.usuario.id, session.token);
      const dados = response.dados;

      setVisaoGeral(dados);
      form.reset(mapToFormValues(dados?.configuracaoVigente ?? null));
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível carregar o perfil financeiro.");
      }
      setVisaoGeral(null);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    carregarPerfilFinanceiro();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [session?.token, session?.usuario.id]);

  const configuracaoVigente = visaoGeral?.configuracaoVigente ?? null;

  const cards = useMemo(() => {
    return [
      {
        titulo: "Economia desejada",
        valor: formatPercentual(configuracaoVigente?.percentualEconomiaMensalDesejado ?? 20),
      },
      {
        titulo: "Meses de reserva",
        valor: `${configuracaoVigente?.mesesReservaEmergenciaDesejados ?? 6} meses`,
      },
      {
        titulo: "Endividamento máximo",
        valor: formatPercentual(configuracaoVigente?.percentualMaximoEndividamento ?? 50),
      },
      {
        titulo: "Investimento mínimo",
        valor: formatPercentual(configuracaoVigente?.percentualMinimoInvestimento ?? 10),
      },
    ];
  }, [configuracaoVigente]);

  async function onSubmit(values: FormValues) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsSaving(true);
      setErrorMessage("");
      setSuccessMessage("");

      const payload: SalvarPerfilFinanceiroPayload = {
        percentualEconomiaMensalDesejado: values.percentualEconomiaMensalDesejado,
        percentualReservaEmergenciaDesejado: values.percentualReservaEmergenciaDesejado,
        mesesReservaEmergenciaDesejados: values.mesesReservaEmergenciaDesejados,
        percentualMaximoComprometimentoRenda: values.percentualMaximoComprometimentoRenda,
        percentualMaximoEndividamento: values.percentualMaximoEndividamento,
        percentualMinimoInvestimento: values.percentualMinimoInvestimento,
        patrimonioLiquidoAlvo:
          typeof values.patrimonioLiquidoAlvo === "number" && !Number.isNaN(values.patrimonioLiquidoAlvo)
            ? values.patrimonioLiquidoAlvo
            : null,
        observacao: values.observacao?.trim() ? values.observacao.trim() : null,
      };

      const response = await salvarPerfilFinanceiro(session.usuario.id, payload, session.token);
      const dados = response.dados;

      setVisaoGeral(dados);
      form.reset(mapToFormValues(dados?.configuracaoVigente ?? null));
      setSuccessMessage("Perfil financeiro salvo com sucesso.");
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível salvar o perfil financeiro.");
      }
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div className="flex flex-row bg-background">
      <Sidebar />

      <main className="flex-1 px-6 py-8 md:px-8">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
          <section className="space-y-2">
            <h1 className="text-3xl font-semibold tracking-tight">Perfil Financeiro</h1>
            <p className="max-w-3xl text-sm text-muted-foreground">
              Defina os parâmetros que representam como você deseja medir sua saúde
              financeira. Cada alteração relevante gera histórico para análises futuras.
            </p>
          </section>

          <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
            {cards.map((card) => (
              <Card key={card.titulo}>
                <CardHeader className="pb-2">
                  <CardDescription>{card.titulo}</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-semibold">{card.valor}</div>
                </CardContent>
              </Card>
            ))}
          </section>

          <Card>
            <CardHeader>
              <CardTitle>Parâmetros atuais</CardTitle>
              <CardDescription>
                Os parâmetros vigentes definem a referência que o sistema usará no futuro
                para indicadores, alertas e leituras de saúde financeira.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="rounded-lg border border-dashed px-4 py-12 text-center text-sm text-muted-foreground">
                  Carregando perfil financeiro...
                </div>
              ) : (
                <Form {...form}>
                  <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                    {visaoGeral?.usaPerfilFinanceiroInicial ? (
                      <div className="rounded-md border border-amber-500/20 bg-amber-500/5 px-4 py-3 text-sm text-amber-700 dark:text-amber-300">
                        Seu perfil foi criado com os parâmetros padrão do sistema para
                        permitir que o Motor Financeiro realize análises desde o primeiro
                        uso. Você pode alterar qualquer configuração quando quiser para que
                        as análises reflitam melhor a sua realidade.
                      </div>
                    ) : null}

                    <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
                      <FormField
                        control={form.control}
                        name="percentualEconomiaMensalDesejado"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Economia mensal desejada (%)</FormLabel>
                            <FormControl>
                              <Input type="number" step="0.01" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name="percentualReservaEmergenciaDesejado"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Reserva de emergência desejada (%)</FormLabel>
                            <FormControl>
                              <Input type="number" step="0.01" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name="mesesReservaEmergenciaDesejados"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Meses de reserva desejados</FormLabel>
                            <FormControl>
                              <Input type="number" step="1" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name="percentualMaximoComprometimentoRenda"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Comprometimento máximo da renda (%)</FormLabel>
                            <FormControl>
                              <Input type="number" step="0.01" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name="percentualMaximoEndividamento"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Endividamento máximo (%)</FormLabel>
                            <FormControl>
                              <Input type="number" step="0.01" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name="percentualMinimoInvestimento"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Investimento mínimo (%)</FormLabel>
                            <FormControl>
                              <Input type="number" step="0.01" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name="patrimonioLiquidoAlvo"
                        render={({ field }) => (
                          <FormItem className="md:col-span-2 xl:col-span-1">
                            <FormLabel>Patrimônio líquido alvo</FormLabel>
                            <FormControl>
                              <Input
                                type="number"
                                step="0.01"
                                value={field.value ?? ""}
                                onChange={(event) =>
                                  field.onChange(
                                    event.target.value === "" ? undefined : event.target.value
                                  )
                                }
                              />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>

                    <FormField
                      control={form.control}
                      name="observacao"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Observação</FormLabel>
                          <FormControl>
                            <Textarea
                              placeholder="Contexto opcional sobre a lógica que você quer seguir na sua vida financeira."
                              {...field}
                              value={field.value ?? ""}
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    {errorMessage ? (
                      <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
                        {errorMessage}
                      </div>
                    ) : null}

                    {successMessage ? (
                      <div className="rounded-md border border-emerald-500/20 bg-emerald-500/5 px-4 py-3 text-sm text-emerald-600 dark:text-emerald-400">
                        {successMessage}
                      </div>
                    ) : null}

                    <div className="flex justify-end">
                      <Button type="submit" disabled={isSaving}>
                        {isSaving ? "Salvando..." : "Salvar parâmetros"}
                      </Button>
                    </div>
                  </form>
                </Form>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Histórico</CardTitle>
              <CardDescription>
                Cada alteração relevante fecha a configuração vigente e cria um novo
                registro histórico, preservando rastreabilidade analítica.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {(visaoGeral?.historico.length ?? 0) === 0 ? (
                <div className="rounded-lg border border-dashed px-4 py-12 text-center text-sm text-muted-foreground">
                  Ainda não existe histórico para este usuário.
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Vigência</TableHead>
                        <TableHead>Economia</TableHead>
                        <TableHead>Reserva</TableHead>
                        <TableHead>Meses de reserva</TableHead>
                        <TableHead>Comprometimento máximo</TableHead>
                        <TableHead>Endividamento máximo</TableHead>
                        <TableHead>Investimento mínimo</TableHead>
                        <TableHead>Patrimônio alvo</TableHead>
                        <TableHead>Origem</TableHead>
                        <TableHead>Status</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {visaoGeral?.historico.map((item) => (
                        <TableRow key={item.id}>
                          <TableCell className="min-w-[180px]">
                            <div className="font-medium">{formatDate(item.dataInicioVigencia)}</div>
                            <div className="text-xs text-muted-foreground">
                              até {formatDate(item.dataFimVigencia)}
                            </div>
                          </TableCell>
                          <TableCell>{formatPercentual(item.percentualEconomiaMensalDesejado)}</TableCell>
                          <TableCell>{formatPercentual(item.percentualReservaEmergenciaDesejado)}</TableCell>
                          <TableCell>{item.mesesReservaEmergenciaDesejados}</TableCell>
                          <TableCell>{formatPercentual(item.percentualMaximoComprometimentoRenda)}</TableCell>
                          <TableCell>{formatPercentual(item.percentualMaximoEndividamento)}</TableCell>
                          <TableCell>{formatPercentual(item.percentualMinimoInvestimento)}</TableCell>
                          <TableCell>{formatCurrency(item.patrimonioLiquidoAlvo)}</TableCell>
                          <TableCell>
                            {item.origemPerfilFinanceiro === "PerfilInicialSistema"
                              ? "Perfil inicial do sistema"
                              : "Personalizado pelo usuário"}
                          </TableCell>
                          <TableCell>
                            <Badge variant={item.vigente ? "default" : "secondary"}>
                              {item.vigente ? "Vigente" : "Histórico"}
                            </Badge>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  );
}
