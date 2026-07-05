"use client";

import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import {
  buscarParcelamento,
  editarParcelamentoEmLote,
} from "@/services/api/lancamentos";
import { buscarCategorias } from "@/services/api/categories";
import { buscarCartoes, buscarContas } from "@/services/api/finance";
import { CategoriaResumo } from "@/types/categories";
import { CartaoResumo, ContaResumo } from "@/types/finance";
import { DetalheParcelamento } from "@/types/lancamentos";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
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
import { Switch } from "@/components/ui/switch";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  normalizarSelecaoOpcional,
  SELECT_NONE,
} from "@/lib/lancamento-vinculo";

const STATUS_PAGO = 1;
const STATUS_RECEBIDO = 2;
const STATUS_CANCELADO = 3;

const formSchema = z
  .object({
    descricaoBase: z.string().min(2, "Informe a descrição base do parcelamento."),
    observacao: z.string().optional(),
    contaId: z.string(),
    cartaoId: z.string(),
    categoriaId: z.string(),
    subCategoriaId: z.string(),
    dataInicialParcelamento: z
      .string()
      .min(1, "Informe a data inicial do parcelamento."),
    alterarParcelasEfetivadas: z.boolean(),
  })
  .superRefine((values, ctx) => {
    if (values.contaId !== SELECT_NONE && values.cartaoId !== SELECT_NONE) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["cartaoId"],
        message: "Escolha apenas conta ou cartão no parcelamento.",
      });
    }

    if (
      values.subCategoriaId !== SELECT_NONE &&
      values.categoriaId === SELECT_NONE
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["subCategoriaId"],
        message: "Selecione uma categoria antes da subcategoria.",
      });
    }
  });

type FormValues = z.infer<typeof formSchema>;

function toDateInputValue(dateValue?: string | null) {
  if (!dateValue) {
    return "";
  }

  return new Date(dateValue).toISOString().split("T")[0];
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR").format(new Date(value));
}

function formatCurrency(value: number) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function getStatusLabel(status: number) {
  switch (status) {
    case STATUS_PAGO:
      return "Pago";
    case STATUS_RECEBIDO:
      return "Recebido";
    case STATUS_CANCELADO:
      return "Cancelado";
    default:
      return "Pendente";
  }
}

function getStatusVariant(
  status: number
): "default" | "secondary" | "destructive" | "outline" {
  switch (status) {
    case STATUS_PAGO:
    case STATUS_RECEBIDO:
      return "secondary";
    case STATUS_CANCELADO:
      return "outline";
    default:
      return "destructive";
  }
}

interface GerenciarParcelamentoModalProps {
  grupoParcelamentoId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  usuarioId: string;
  token: string;
  onSaved: () => void;
}

export function GerenciarParcelamentoModal({
  grupoParcelamentoId,
  open,
  onOpenChange,
  usuarioId,
  token,
  onSaved,
}: GerenciarParcelamentoModalProps) {
  const [isLoadingData, setIsLoadingData] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [detalhe, setDetalhe] = useState<DetalheParcelamento | null>(null);
  const [categorias, setCategorias] = useState<CategoriaResumo[]>([]);
  const [contas, setContas] = useState<ContaResumo[]>([]);
  const [cartoes, setCartoes] = useState<CartaoResumo[]>([]);

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      descricaoBase: "",
      observacao: "",
      contaId: SELECT_NONE,
      cartaoId: SELECT_NONE,
      categoriaId: SELECT_NONE,
      subCategoriaId: SELECT_NONE,
      dataInicialParcelamento: "",
      alterarParcelasEfetivadas: false,
    },
  });

  useEffect(() => {
    async function carregarDados() {
      if (!open || !grupoParcelamentoId) {
        return;
      }

      try {
        setIsLoadingData(true);
        setErrorMessage("");

        const [
          parcelamentoResponse,
          categoriasResponse,
          contasResponse,
          cartoesResponse,
        ] = await Promise.all([
          buscarParcelamento(usuarioId, grupoParcelamentoId, token),
          buscarCategorias(usuarioId, token),
          buscarContas(usuarioId, token).catch(() => ({ dados: [] })),
          buscarCartoes(usuarioId, token).catch(() => ({ dados: [] })),
        ]);

        const parcelamento = parcelamentoResponse.dados;
        setDetalhe(parcelamento ?? null);
        setCategorias(categoriasResponse.dados ?? []);
        setContas(contasResponse.dados ?? []);
        setCartoes(cartoesResponse.dados ?? []);

        if (!parcelamento) {
          setErrorMessage("Não foi possível carregar o parcelamento selecionado.");
          return;
        }

        form.reset({
          descricaoBase: parcelamento.descricaoBase ?? "",
          observacao: parcelamento.observacao ?? "",
          contaId: parcelamento.contaId ?? SELECT_NONE,
          cartaoId: parcelamento.cartaoId ?? SELECT_NONE,
          categoriaId: parcelamento.categoriaId ?? SELECT_NONE,
          subCategoriaId: parcelamento.subCategoriaId ?? SELECT_NONE,
          dataInicialParcelamento: toDateInputValue(
            parcelamento.dataInicialParcelamento
          ),
          alterarParcelasEfetivadas: false,
        });
      } catch (error) {
        if (error instanceof ApiError) {
          setErrorMessage(error.message);
        } else {
          setErrorMessage("Não foi possível carregar o parcelamento.");
        }
      } finally {
        setIsLoadingData(false);
      }
    }

    carregarDados();
  }, [form, grupoParcelamentoId, open, token, usuarioId]);

  const tipo = String(detalhe?.tipo ?? 1);
  const categoriaSelecionadaId = form.watch("categoriaId");

  const categoriasDisponiveis = useMemo(() => {
    return categorias.filter((categoria) => String(categoria.tipo) === tipo);
  }, [categorias, tipo]);

  const subCategoriasDisponiveis = useMemo(() => {
    if (categoriaSelecionadaId === SELECT_NONE) {
      return [];
    }

    const categoriaSelecionada = categoriasDisponiveis.find(
      (categoria) => categoria.id === categoriaSelecionadaId
    );

    return categoriaSelecionada?.subCategorias ?? [];
  }, [categoriaSelecionadaId, categoriasDisponiveis]);

  async function onSubmit(values: FormValues) {
    if (!grupoParcelamentoId) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");

      await editarParcelamentoEmLote(
        usuarioId,
        grupoParcelamentoId,
        {
          descricaoBase: values.descricaoBase,
          observacao: values.observacao || "",
          contaId: normalizarSelecaoOpcional(values.contaId),
          cartaoId: normalizarSelecaoOpcional(values.cartaoId),
          categoriaId: normalizarSelecaoOpcional(values.categoriaId),
          subCategoriaId: normalizarSelecaoOpcional(values.subCategoriaId),
          dataInicialParcelamento: `${values.dataInicialParcelamento}T00:00:00`,
          alterarParcelasEfetivadas: values.alterarParcelasEfetivadas,
        },
        token
      );

      onOpenChange(false);
      onSaved();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível salvar o parcelamento.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="flex max-h-[90vh] max-w-5xl flex-col overflow-hidden p-0">
        <DialogHeader className="border-b px-6 py-5">
          <DialogTitle>Gerenciar parcelamento</DialogTitle>
          <DialogDescription>
            Visualize todas as parcelas do grupo e ajuste os dados comuns em
            lote.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="flex min-h-0 flex-1 flex-col"
          >
            <div className="flex-1 space-y-6 overflow-y-auto px-6 py-5">
              <div className="rounded-lg border bg-muted/20 px-4 py-3 text-sm text-muted-foreground">
                {detalhe?.possuiParcelasEfetivadas
                  ? `Este grupo possui ${detalhe.quantidadeParcelasEfetivadas} parcela(s) efetivada(s). Por padrão, apenas parcelas pendentes serão alteradas.`
                  : "Este grupo ainda não possui parcelas efetivadas."}
              </div>

              {errorMessage ? (
                <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
                  {errorMessage}
                </div>
              ) : null}

              <div className="grid gap-4 md:grid-cols-2">
                <FormField
                  control={form.control}
                  name="descricaoBase"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Descrição base</FormLabel>
                      <FormControl>
                        <Input placeholder="Ex: Notebook" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="dataInicialParcelamento"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Data inicial do parcelamento</FormLabel>
                      <FormControl>
                        <Input type="date" {...field} />
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
                        placeholder="Observações comuns do parcelamento"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <div className="grid gap-4 md:grid-cols-2">
                <FormField
                  control={form.control}
                  name="contaId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Conta</FormLabel>
                      <Select
                        onValueChange={(value) => {
                          field.onChange(value);
                          if (value !== SELECT_NONE) {
                            form.setValue("cartaoId", SELECT_NONE);
                          }
                        }}
                        value={field.value}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione uma conta" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value={SELECT_NONE}>
                            Avulso / sem conta
                          </SelectItem>
                          {contas.map((conta) => (
                            <SelectItem key={conta.id} value={conta.id}>
                              {conta.nomeConta} - {conta.instituicao}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="cartaoId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Cartão</FormLabel>
                      <Select
                        onValueChange={(value) => {
                          field.onChange(value);
                          if (value !== SELECT_NONE) {
                            form.setValue("contaId", SELECT_NONE);
                          }
                        }}
                        value={field.value}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione um cartão" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value={SELECT_NONE}>
                            Não utilizar agora
                          </SelectItem>
                          {cartoes.map((cartao) => (
                            <SelectItem key={cartao.id} value={cartao.id}>
                              {cartao.nomeCartao} - {cartao.instituicao}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <div className="grid gap-4 md:grid-cols-2">
                <FormField
                  control={form.control}
                  name="categoriaId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Categoria</FormLabel>
                      <Select onValueChange={field.onChange} value={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione uma categoria" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value={SELECT_NONE}>
                            Sem categoria
                          </SelectItem>
                          {categoriasDisponiveis.map((categoria) => (
                            <SelectItem key={categoria.id} value={categoria.id}>
                              {categoria.nomeCategoria}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="subCategoriaId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Subcategoria</FormLabel>
                      <Select
                        onValueChange={field.onChange}
                        value={field.value}
                        disabled={categoriaSelecionadaId === SELECT_NONE}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue
                              placeholder={
                                categoriaSelecionadaId === SELECT_NONE
                                  ? "Selecione uma categoria primeiro"
                                  : "Selecione uma subcategoria"
                              }
                            />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value={SELECT_NONE}>
                            Sem subcategoria
                          </SelectItem>
                          {subCategoriasDisponiveis.map((subCategoria) => (
                            <SelectItem
                              key={subCategoria.id}
                              value={subCategoria.id}
                            >
                              {subCategoria.nomeSubCategoria}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <FormField
                control={form.control}
                name="alterarParcelasEfetivadas"
                render={({ field }) => (
                  <FormItem className="flex items-center justify-between rounded-xl border px-4 py-3">
                    <div className="space-y-1">
                      <FormLabel>Alterar também parcelas efetivadas</FormLabel>
                      <p className="text-sm text-muted-foreground">
                        Use esta opção apenas se quiser atualizar manualmente
                        parcelas já pagas ou recebidas.
                      </p>
                    </div>
                    <FormControl>
                      <Switch
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                  </FormItem>
                )}
              />

              <div className="space-y-3">
                <div>
                  <h3 className="text-sm font-semibold">Parcelas do grupo</h3>
                  <p className="text-sm text-muted-foreground">
                    Confira as parcelas antes de recalcular os vencimentos.
                  </p>
                </div>

                <div className="max-h-[260px] overflow-auto rounded-lg border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Descrição</TableHead>
                        <TableHead>Parcela</TableHead>
                        <TableHead>Valor</TableHead>
                        <TableHead>Vencimento</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead>Efetivação</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {detalhe?.parcelas.map((parcela) => (
                        <TableRow key={parcela.id}>
                          <TableCell className="font-medium">
                            {parcela.descricao}
                          </TableCell>
                          <TableCell>
                            {parcela.numeroParcela}/{parcela.totalParcelas}
                          </TableCell>
                          <TableCell>{formatCurrency(parcela.valor)}</TableCell>
                          <TableCell>
                            {formatDate(parcela.dataVencimento)}
                          </TableCell>
                          <TableCell>
                            <Badge
                              variant={getStatusVariant(
                                parcela.statusLancamento
                              )}
                            >
                              {getStatusLabel(parcela.statusLancamento)}
                            </Badge>
                          </TableCell>
                          <TableCell>
                            {parcela.dataEfetivacao
                              ? formatDate(parcela.dataEfetivacao)
                              : "-"}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </div>
            </div>

            <DialogFooter className="border-t bg-background px-6 py-4">
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Cancelar
              </Button>
              <Button type="submit" disabled={isSubmitting || isLoadingData}>
                {isSubmitting ? "Salvando..." : "Salvar alterações"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
