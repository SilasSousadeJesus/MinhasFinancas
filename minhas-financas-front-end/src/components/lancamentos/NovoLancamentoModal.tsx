"use client";

import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import { useAuth } from "@/providers/auth-provider";
import { buscarCategorias } from "@/services/api/categories";
import {
  normalizarSelecaoOpcional,
  resolverVinculoLancamento,
  SELECT_NONE,
} from "@/lib/lancamento-vinculo";
import {
  buscarContas,
  buscarCartoes,
  cadastrarLancamento,
} from "@/services/api/finance";
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
  DialogTrigger,
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

const FREQUENCIA_PONTUAL = "0";
const FREQUENCIA_FIXA = "1";
const FREQUENCIA_PARCELADA = "2";
const FREQUENCIA_DIA_UTIL = "3";
const TIPO_DESPESA = "0";
const TIPO_RECEITA = "1";
const STATUS_PENDENTE = "0";

const formSchema = z
  .object({
    tipo: z.string(),
    valor: z.coerce.number().positive("Informe um valor maior que zero."),
    descricao: z.string().min(2, "Informe uma descricao."),
    observacao: z.string().optional(),
    dataVencimento: z.string().min(1, "Informe a data de vencimento."),
    dataLancamento: z.string().min(1, "Informe a data de lancamento."),
    frequenciaLancamento: z.string(),
    quantidadeParcelas: z.coerce.number().nullable().optional(),
    numeroDiaUtil: z.coerce.number().nullable().optional(),
    contaId: z.string(),
    cartaoId: z.string(),
    categoriaId: z.string(),
    subCategoriaId: z.string(),
  })
  .superRefine((values, ctx) => {
    if (
      values.frequenciaLancamento === FREQUENCIA_PARCELADA &&
      (!values.quantidadeParcelas || values.quantidadeParcelas <= 1)
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["quantidadeParcelas"],
        message: "Informe uma quantidade de parcelas maior que 1.",
      });
    }

    if (
      values.frequenciaLancamento === FREQUENCIA_DIA_UTIL &&
      (!values.numeroDiaUtil || values.numeroDiaUtil <= 0)
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["numeroDiaUtil"],
        message: "Informe um numero de dia util maior que zero.",
      });
    }
  });

type FormValues = z.infer<typeof formSchema>;

function getToday() {
  return new Date().toISOString().split("T")[0];
}

function getDataVencimentoLabel(frequencia: string) {
  return frequencia === FREQUENCIA_DIA_UTIL
    ? "Data inicial da programacao"
    : "Data de vencimento";
}

function getDataVencimentoHint(frequencia: string) {
  return frequencia === FREQUENCIA_DIA_UTIL
    ? "O sistema usa esta data apenas como mes inicial e calcula automaticamente o N-esimo dia util de cada mes."
    : "";
}

interface NovoLancamentoModalProps {
  onCreated: () => void;
}

export function NovoLancamentoModal({ onCreated }: NovoLancamentoModalProps) {
  const { session } = useAuth();
  const [open, setOpen] = useState(false);
  const [isLoadingData, setIsLoadingData] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [contas, setContas] = useState<ContaResumo[]>([]);
  const [cartoes, setCartoes] = useState<CartaoResumo[]>([]);
  const [categorias, setCategorias] = useState<CategoriaResumo[]>([]);

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      tipo: TIPO_RECEITA,
      valor: 0,
      descricao: "",
      observacao: "",
      dataVencimento: getToday(),
      dataLancamento: getToday(),
      frequenciaLancamento: FREQUENCIA_PONTUAL,
      quantidadeParcelas: null,
      numeroDiaUtil: null,
      contaId: SELECT_NONE,
      cartaoId: SELECT_NONE,
      categoriaId: SELECT_NONE,
      subCategoriaId: SELECT_NONE,
    },
  });

  useEffect(() => {
    async function carregarDados() {
      if (!open || !session?.usuario.id || !session.token) {
        return;
      }

      try {
        setIsLoadingData(true);
        setErrorMessage("");

        const [contasResponse, cartoesResponse, categoriasResponse] = await Promise.all([
          buscarContas(session.usuario.id, session.token).catch(() => ({ dados: [] })),
          buscarCartoes(session.usuario.id, session.token).catch(() => ({ dados: [] })),
          buscarCategorias(session.usuario.id, session.token),
        ]);

        setContas(contasResponse.dados ?? []);
        setCartoes(cartoesResponse.dados ?? []);
        setCategorias(categoriasResponse.dados ?? []);
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
  }, [open, session?.token, session?.usuario.id]);

  const categoriasReceita = useMemo(
    () => categorias.filter((categoria) => categoria.tipo === 1),
    [categorias]
  );

  const categoriasDespesa = useMemo(
    () => categorias.filter((categoria) => categoria.tipo === 0),
    [categorias]
  );

  const tipoSelecionado = form.watch("tipo");
  const frequenciaSelecionada = form.watch("frequenciaLancamento");
  const categoriaSelecionadaId = form.watch("categoriaId");

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
    form.setValue("subCategoriaId", SELECT_NONE);
  }, [categoriaSelecionadaId, form]);

  useEffect(() => {
    form.setValue("categoriaId", SELECT_NONE);
    form.setValue("subCategoriaId", SELECT_NONE);
  }, [tipoSelecionado, form]);

  useEffect(() => {
    if (frequenciaSelecionada !== FREQUENCIA_PARCELADA) {
      form.setValue("quantidadeParcelas", null);
    }

    if (frequenciaSelecionada !== FREQUENCIA_DIA_UTIL) {
      form.setValue("numeroDiaUtil", null);
    }
  }, [frequenciaSelecionada, form]);

  async function onSubmit(values: FormValues) {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");

      const contaSelecionada = normalizarSelecaoOpcional(values.contaId);
      const cartaoSelecionado = normalizarSelecaoOpcional(values.cartaoId);
      const vinculo = resolverVinculoLancamento(
        contaSelecionada,
        cartaoSelecionado,
        contas,
        cartoes
      );

      await cadastrarLancamento(
        {
          valor: values.valor,
          descricao: values.descricao,
          observacao: values.observacao || "",
          dataVencimento: `${values.dataVencimento}T00:00:00`,
          dataLancamento: `${values.dataLancamento}T00:00:00`,
          dataEfetivacao: null,
          statusLancamento: Number(STATUS_PENDENTE),
          frequenciaLancamento: Number(values.frequenciaLancamento),
          quantidadeParcelas:
            values.frequenciaLancamento === FREQUENCIA_PARCELADA
              ? values.quantidadeParcelas ?? null
              : null,
          numeroDiaUtil:
            values.frequenciaLancamento === FREQUENCIA_DIA_UTIL
              ? values.numeroDiaUtil ?? null
              : null,
          tipo: Number(values.tipo),
          vinculo,
          contaId: contaSelecionada,
          cartaoId: cartaoSelecionado,
          usuarioId: session.usuario.id,
          categoriaId: normalizarSelecaoOpcional(values.categoriaId),
          subCategoriaId: normalizarSelecaoOpcional(values.subCategoriaId),
        },
        session.token
      );

      form.reset({
        tipo: values.tipo,
        valor: 0,
        descricao: "",
        observacao: "",
        dataVencimento: getToday(),
        dataLancamento: getToday(),
        frequenciaLancamento: FREQUENCIA_PONTUAL,
        quantidadeParcelas: null,
        numeroDiaUtil: null,
        contaId: SELECT_NONE,
        cartaoId: SELECT_NONE,
        categoriaId: SELECT_NONE,
        subCategoriaId: SELECT_NONE,
      });
      setOpen(false);
      onCreated();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel cadastrar o lancamento.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="default">Novo Lancamento</Button>
      </DialogTrigger>
      <DialogContent className="max-w-3xl">
        <DialogHeader>
          <DialogTitle>Novo Lancamento</DialogTitle>
          <DialogDescription>
            Primeiro fluxo de lancamento simples para receita e despesa.
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
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione a frequencia" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="0">Pontual</SelectItem>
                        <SelectItem value={FREQUENCIA_FIXA}>
                          Fixo
                        </SelectItem>
                        <SelectItem value={FREQUENCIA_PARCELADA}>
                          Parcelado
                        </SelectItem>
                        <SelectItem value={FREQUENCIA_DIA_UTIL}>
                          Dia util
                        </SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {frequenciaSelecionada === FREQUENCIA_PARCELADA ? (
              <FormField
                control={form.control}
                name="quantidadeParcelas"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Quantidade de parcelas</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min="2"
                        step="1"
                        value={field.value ?? ""}
                        onChange={(event) => {
                          const value = event.target.value;
                          field.onChange(value === "" ? null : Number(value));
                        }}
                      />
                    </FormControl>
                    <p className="text-xs text-muted-foreground">
                      O sistema vai gerar todas as parcelas mensalmente de uma vez.
                    </p>
                    <FormMessage />
                  </FormItem>
                )}
              />
            ) : null}

            {frequenciaSelecionada === FREQUENCIA_DIA_UTIL ? (
              <FormField
                control={form.control}
                name="numeroDiaUtil"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Numero do dia util</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min="1"
                        step="1"
                        value={field.value ?? ""}
                        onChange={(event) => {
                          const value = event.target.value;
                          field.onChange(value === "" ? null : Number(value));
                        }}
                      />
                    </FormControl>
                    <p className="text-xs text-muted-foreground">
                      Exemplo: 5 para gerar sempre no 5o dia util de cada mes.
                    </p>
                    <FormMessage />
                  </FormItem>
                )}
              />
            ) : null}

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
                    <FormLabel>{getDataVencimentoLabel(frequenciaSelecionada)}</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    {getDataVencimentoHint(frequenciaSelecionada) ? (
                      <p className="text-xs text-muted-foreground">
                        {getDataVencimentoHint(frequenciaSelecionada)}
                      </p>
                    ) : null}
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

            <div className="rounded-md border p-4">
              <p className="text-sm font-medium">Status inicial</p>
              <p className="mt-1 text-sm text-muted-foreground">
                Todo novo lancamento nasce como pendente. A efetivacao acontece depois, quando ele for pago ou recebido.
              </p>
            </div>

            {errorMessage ? (
              <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
                {errorMessage}
              </div>
            ) : null}

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setOpen(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={isSubmitting || isLoadingData}>
                {isSubmitting ? "Salvando..." : "Salvar lancamento"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
