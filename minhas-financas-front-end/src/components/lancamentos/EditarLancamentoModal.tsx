"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import {
  normalizarSelecaoOpcional,
  resolverVinculoLancamento,
  SELECT_NONE,
} from "@/lib/lancamento-vinculo";
import { buscarCategorias } from "@/services/api/categories";
import { buscarCartoes, buscarContas } from "@/services/api/finance";
import { buscarLancamento, editarLancamento } from "@/services/api/lancamentos";
import { CategoriaResumo } from "@/types/categories";
import { CartaoResumo, ContaResumo } from "@/types/finance";
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
import { Textarea } from "@/components/ui/textarea";
import { EditarLancamentoPayload, LancamentoResumo } from "@/types/lancamentos";

const FREQUENCIA_PONTUAL = "0";
const FREQUENCIA_DIA_UTIL = "3";
const TIPO_DESPESA = "0";
const TIPO_RECEITA = "1";
const STATUS_PENDENTE = "0";
const STATUS_PAGO = "1";
const STATUS_RECEBIDO = "2";
const STATUS_CANCELADO = "3";

const formSchema = z
  .object({
    tipo: z.string(),
    valor: z.coerce.number().positive("Informe um valor maior que zero."),
    descricao: z.string().min(2, "Informe uma descricao."),
    observacao: z.string().optional(),
    dataVencimento: z.string().min(1, "Informe a data de vencimento."),
    dataLancamento: z.string().min(1, "Informe a data de lancamento."),
    dataEfetivacao: z.string().optional(),
    statusLancamento: z.string(),
    frequenciaLancamento: z.string(),
    contaId: z.string(),
    cartaoId: z.string(),
    categoriaId: z.string(),
    subCategoriaId: z.string(),
  })
  .superRefine((values, ctx) => {
    if (
      (values.statusLancamento === STATUS_PAGO || values.statusLancamento === STATUS_RECEBIDO) &&
      !values.dataEfetivacao
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["dataEfetivacao"],
        message: "Informe a data de efetivacao para lancamentos pagos ou recebidos.",
      });
    }

    if (values.tipo === TIPO_RECEITA && values.statusLancamento === STATUS_PAGO) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["statusLancamento"],
        message: "Receitas nao podem assumir o status Pago.",
      });
    }

    if (values.tipo === TIPO_DESPESA && values.statusLancamento === STATUS_RECEBIDO) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["statusLancamento"],
        message: "Despesas nao podem assumir o status Recebido.",
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

interface EditarLancamentoModalProps {
  lancamentoId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  usuarioId: string;
  token: string;
  onSaved: () => void;
}

export function EditarLancamentoModal({
  lancamentoId,
  open,
  onOpenChange,
  usuarioId,
  token,
  onSaved,
}: EditarLancamentoModalProps) {
  const [isLoadingData, setIsLoadingData] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [contas, setContas] = useState<ContaResumo[]>([]);
  const [cartoes, setCartoes] = useState<CartaoResumo[]>([]);
  const [categorias, setCategorias] = useState<CategoriaResumo[]>([]);
  const [lancamentoAtual, setLancamentoAtual] = useState<LancamentoResumo | null>(null);
  const previousTipoRef = useRef<string | null>(null);
  const previousCategoriaRef = useRef<string | null>(null);
  const isHydratingRef = useRef(false);

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      tipo: TIPO_RECEITA,
      valor: 0,
      descricao: "",
      observacao: "",
      dataVencimento: "",
      dataLancamento: "",
      dataEfetivacao: "",
      statusLancamento: STATUS_PENDENTE,
      frequenciaLancamento: FREQUENCIA_PONTUAL,
      contaId: SELECT_NONE,
      cartaoId: SELECT_NONE,
      categoriaId: SELECT_NONE,
      subCategoriaId: SELECT_NONE,
    },
  });

  useEffect(() => {
    async function carregarDados() {
      if (!open || !lancamentoId) {
        return;
      }

      try {
        setIsLoadingData(true);
        setErrorMessage("");

        const [contasResponse, cartoesResponse, categoriasResponse, lancamentoResponse] =
          await Promise.all([
            buscarContas(usuarioId, token).catch(() => ({ dados: [] })),
            buscarCartoes(usuarioId, token).catch(() => ({ dados: [] })),
            buscarCategorias(usuarioId, token),
            buscarLancamento(usuarioId, lancamentoId, token),
          ]);

        const lancamento = lancamentoResponse.dados;

        setContas(contasResponse.dados ?? []);
        setCartoes(cartoesResponse.dados ?? []);
        setCategorias(categoriasResponse.dados ?? []);

        if (!lancamento) {
          setErrorMessage("Nao foi possivel carregar o lancamento selecionado.");
          return;
        }

        setLancamentoAtual(lancamento);
        isHydratingRef.current = true;
        const nextTipo = String(lancamento.tipo);
        const nextCategoriaId = lancamento.categoriaId ?? SELECT_NONE;
        previousTipoRef.current = nextTipo;
        previousCategoriaRef.current = nextCategoriaId;

        form.reset({
          tipo: nextTipo,
          valor: lancamento.valor,
          descricao: lancamento.descricao ?? "",
          observacao: lancamento.observacao ?? "",
          dataVencimento: toDateInputValue(lancamento.dataVencimento),
          dataLancamento: toDateInputValue(lancamento.dataLancamento),
          dataEfetivacao: toDateInputValue(lancamento.dataEfetivacao),
          statusLancamento: String(lancamento.statusLancamento ?? 0),
          frequenciaLancamento: String(lancamento.frequenciaLancamento ?? 0),
          contaId: lancamento.contaId || SELECT_NONE,
          cartaoId: lancamento.cartaoId || SELECT_NONE,
          categoriaId: nextCategoriaId,
          subCategoriaId: lancamento.subCategoriaId || SELECT_NONE,
        });

        requestAnimationFrame(() => {
          isHydratingRef.current = false;
        });
      } catch (error) {
        if (error instanceof ApiError) {
          setErrorMessage(error.message);
        } else {
          setErrorMessage("Nao foi possivel carregar os dados do lancamento.");
        }
      } finally {
        setIsLoadingData(false);
      }
    }

    carregarDados();
  }, [form, lancamentoId, open, token, usuarioId]);

  const categoriasReceita = useMemo(
    () => categorias.filter((categoria) => categoria.tipo === 1),
    [categorias]
  );

  const categoriasDespesa = useMemo(
    () => categorias.filter((categoria) => categoria.tipo === 0),
    [categorias]
  );

  const tipoSelecionado = form.watch("tipo");
  const categoriaSelecionadaId = form.watch("categoriaId");
  const statusSelecionado = form.watch("statusLancamento");

  const categoriasDisponiveis = useMemo(() => {
    return tipoSelecionado === TIPO_DESPESA ? categoriasDespesa : categoriasReceita;
  }, [categoriasDespesa, categoriasReceita, tipoSelecionado]);

  const subCategoriasDisponiveis = useMemo(() => {
    if (categoriaSelecionadaId === SELECT_NONE) {
      return [];
    }

    const categoriaSelecionada = categoriasDisponiveis.find(
      (categoria) => categoria.id === categoriaSelecionadaId
    );

    return categoriaSelecionada?.subCategorias ?? [];
  }, [categoriaSelecionadaId, categoriasDisponiveis]);

  useEffect(() => {
    const previousTipo = previousTipoRef.current;
    previousTipoRef.current = tipoSelecionado;

    if (isHydratingRef.current || previousTipo === null || previousTipo === tipoSelecionado) {
      return;
    }

    form.setValue("categoriaId", SELECT_NONE);
    form.setValue("subCategoriaId", SELECT_NONE);
    previousCategoriaRef.current = SELECT_NONE;
  }, [form, tipoSelecionado]);

  useEffect(() => {
    const previousCategoria = previousCategoriaRef.current;
    previousCategoriaRef.current = categoriaSelecionadaId;

    if (
      isHydratingRef.current ||
      previousCategoria === null ||
      previousCategoria === categoriaSelecionadaId
    ) {
      return;
    }

    form.setValue("subCategoriaId", SELECT_NONE);
  }, [categoriaSelecionadaId, form]);

  useEffect(() => {
    if (
      statusSelecionado === STATUS_PENDENTE ||
      statusSelecionado === STATUS_CANCELADO
    ) {
      form.setValue("dataEfetivacao", "");
    }
  }, [form, statusSelecionado]);

  useEffect(() => {
    if (tipoSelecionado === TIPO_RECEITA && statusSelecionado === STATUS_PAGO) {
      form.setValue("statusLancamento", STATUS_PENDENTE);
      form.setValue("dataEfetivacao", "");
    }

    if (tipoSelecionado === TIPO_DESPESA && statusSelecionado === STATUS_RECEBIDO) {
      form.setValue("statusLancamento", STATUS_PENDENTE);
      form.setValue("dataEfetivacao", "");
    }
  }, [form, statusSelecionado, tipoSelecionado]);

  async function onSubmit(values: FormValues) {
    if (!lancamentoId) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");

      const contaSelecionada = normalizarSelecaoOpcional(values.contaId);
      const cartaoSelecionado = normalizarSelecaoOpcional(values.cartaoId);

      const payload: EditarLancamentoPayload = {
        id: lancamentoId,
        valor: values.valor,
        descricao: values.descricao,
        observacao: values.observacao || "",
        dataVencimento: `${values.dataVencimento}T00:00:00`,
        dataLancamento: `${values.dataLancamento}T00:00:00`,
        dataEfetivacao: values.dataEfetivacao ? `${values.dataEfetivacao}T00:00:00` : null,
        grupoParcelamentoId: lancamentoAtual?.grupoParcelamentoId ?? null,
        numeroParcela: lancamentoAtual?.numeroParcela ?? null,
        totalParcelas: lancamentoAtual?.totalParcelas ?? null,
        grupoLancamentoProgramadoId: lancamentoAtual?.grupoLancamentoProgramadoId ?? null,
        tipoProgramacao: lancamentoAtual?.tipoProgramacao ?? null,
        numeroDiaUtil: lancamentoAtual?.numeroDiaUtil ?? null,
        statusLancamento: Number(values.statusLancamento),
        frequenciaLancamento: Number(values.frequenciaLancamento),
        tipo: Number(values.tipo),
        vinculo: resolverVinculoLancamento(contaSelecionada, cartaoSelecionado, contas, cartoes),
        contaId: contaSelecionada,
        cartaoId: cartaoSelecionado,
        usuarioId,
        categoriaId: normalizarSelecaoOpcional(values.categoriaId),
        subCategoriaId: normalizarSelecaoOpcional(values.subCategoriaId),
      };

      await editarLancamento(usuarioId, lancamentoId, payload, token);
      onOpenChange(false);
      onSaved();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel salvar o lancamento.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl">
        <DialogHeader>
          <DialogTitle>Editar Lancamento</DialogTitle>
          <DialogDescription>
            Atualize os dados do lancamento selecionado.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="tipo"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Tipo</FormLabel>
                    <Select onValueChange={field.onChange} value={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione o tipo" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value={TIPO_RECEITA}>Receita</SelectItem>
                        <SelectItem value={TIPO_DESPESA}>Despesa</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="frequenciaLancamento"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Frequencia do lancamento</FormLabel>
                    <Select onValueChange={field.onChange} value={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione a frequencia" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="0">Pontual</SelectItem>
                        <SelectItem value="1" disabled>
                          Fixo
                        </SelectItem>
                        <SelectItem value="2" disabled>
                          Parcelado
                        </SelectItem>
                        <SelectItem value={FREQUENCIA_DIA_UTIL} disabled>
                          Dia util
                        </SelectItem>
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
                name="valor"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Valor</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.01" min="0" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="descricao"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Descricao</FormLabel>
                    <FormControl>
                      <Input
                        placeholder={
                          tipoSelecionado === TIPO_DESPESA
                            ? "Ex: Mercado, aluguel, farmacia"
                            : "Ex: Salario, freelance, comissao"
                        }
                        {...field}
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
                  <FormLabel>Observacao</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Observacoes adicionais sobre o lancamento"
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
                name="dataVencimento"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data de vencimento</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="dataLancamento"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data do lancamento</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="statusLancamento"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Status do lancamento</FormLabel>
                    <Select onValueChange={field.onChange} value={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione o status" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value={STATUS_PENDENTE}>Pendente</SelectItem>
                        {tipoSelecionado === TIPO_DESPESA ? (
                          <SelectItem value={STATUS_PAGO}>Pago</SelectItem>
                        ) : (
                          <SelectItem value={STATUS_RECEBIDO}>Recebido</SelectItem>
                        )}
                        <SelectItem value={STATUS_CANCELADO}>Cancelado</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="dataEfetivacao"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data de efetivacao</FormLabel>
                    <FormControl>
                      <Input
                        type="date"
                        {...field}
                        disabled={
                          statusSelecionado === STATUS_PENDENTE ||
                          statusSelecionado === STATUS_CANCELADO
                        }
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

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
                        <SelectItem value={SELECT_NONE}>Avulso / sem conta</SelectItem>
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
                    <FormLabel>Cartao</FormLabel>
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
                          <SelectValue placeholder="Selecione um cartao" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value={SELECT_NONE}>Nao utilizado agora</SelectItem>
                        {cartoes.map((cartao) => (
                          <SelectItem key={cartao.id} value={cartao.id}>
                            {cartao.nomeCartao} - {cartao.instituicao}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <p className="text-xs text-muted-foreground">
                      Ao escolher um cartao, a conta fica avulsa automaticamente.
                    </p>
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
                        <SelectItem value={SELECT_NONE}>Sem categoria</SelectItem>
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
                        <SelectItem value={SELECT_NONE}>Sem subcategoria</SelectItem>
                        {subCategoriasDisponiveis.map((subCategoria) => (
                          <SelectItem key={subCategoria.id} value={subCategoria.id}>
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

            {errorMessage ? (
              <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
                {errorMessage}
              </div>
            ) : null}

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={isSubmitting || isLoadingData}>
                {isSubmitting ? "Salvando..." : "Salvar alteracoes"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
