"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useAuth } from "@/providers/auth-provider";
import { cadastrarCartao, cadastrarConta } from "@/services/api/contas-cartoes";
import { ApiError } from "@/types/api";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";

type ActiveTab = "contas" | "cartoes";

const contaSchema = z.object({
  nomeConta: z
    .string()
    .min(2, "Informe o nome da conta.")
    .max(20, "O nome da conta deve ter no maximo 20 caracteres."),
  instituicao: z
    .string()
    .min(2, "Informe a instituicao.")
    .max(20, "A instituicao deve ter no maximo 20 caracteres."),
  tipo: z.string(),
  saldo: z.coerce.number(),
  saldoInvestimento: z.coerce.number(),
  descricao: z.string().optional(),
});

const cartaoSchema = z.object({
  nomeCartao: z
    .string()
    .min(2, "Informe o nome do cartao.")
    .max(20, "O nome do cartao deve ter no maximo 20 caracteres."),
  instituicao: z.string().min(2, "Informe a instituicao."),
  tipo: z.string(),
  bandeira: z.string().min(2, "Informe a bandeira."),
  ultimos4Digitos: z.string().regex(/^\d{4}$/, "Informe exatamente 4 digitos."),
  diaFechamento: z.string().min(1, "Informe o dia de fechamento."),
  diaVencimento: z.string().min(1, "Informe o dia de vencimento."),
  contaPadraoPagamento: z.string().optional(),
  saldo: z.coerce.number(),
  descricao: z.string().optional(),
});

type ContaFormValues = z.infer<typeof contaSchema>;
type CartaoFormValues = z.infer<typeof cartaoSchema>;

interface GerenciarContasCartoesModalProps {
  onCreated?: () => void;
}

export function GerenciarContasCartoesModal({
  onCreated,
}: GerenciarContasCartoesModalProps) {
  const { session } = useAuth();
  const [open, setOpen] = useState(false);
  const [activeTab, setActiveTab] = useState<ActiveTab>("contas");
  const [errorMessage, setErrorMessage] = useState("");
  const [isSubmittingConta, setIsSubmittingConta] = useState(false);
  const [isSubmittingCartao, setIsSubmittingCartao] = useState(false);

  const contaForm = useForm<ContaFormValues>({
    resolver: zodResolver(contaSchema),
    defaultValues: {
      nomeConta: "",
      instituicao: "",
      tipo: "0",
      saldo: 0,
      saldoInvestimento: 0,
      descricao: "",
    },
  });

  const cartaoForm = useForm<CartaoFormValues>({
    resolver: zodResolver(cartaoSchema),
    defaultValues: {
      nomeCartao: "",
      instituicao: "",
      tipo: "0",
      bandeira: "",
      ultimos4Digitos: "",
      diaFechamento: "",
      diaVencimento: "",
      contaPadraoPagamento: "",
      saldo: 0,
      descricao: "",
    },
  });

  function resetForms() {
    contaForm.reset();
    cartaoForm.reset();
    setErrorMessage("");
  }

  async function handleCadastrarConta(values: ContaFormValues) {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      return;
    }

    try {
      setIsSubmittingConta(true);
      setErrorMessage("");

      await cadastrarConta(
        {
          nomeConta: values.nomeConta,
          instituicao: values.instituicao,
          tipo: Number(values.tipo),
          saldo: values.saldo,
          saldoInvestimento: values.saldoInvestimento,
          descricao: values.descricao || "",
          usuarioId: session.usuario.id,
        },
        session.token
      );

      resetForms();
      setOpen(false);
      onCreated?.();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel cadastrar a conta.");
      }
    } finally {
      setIsSubmittingConta(false);
    }
  }

  async function handleCadastrarCartao(values: CartaoFormValues) {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      return;
    }

    try {
      setIsSubmittingCartao(true);
      setErrorMessage("");

      await cadastrarCartao(
        {
          nomeCartao: values.nomeCartao,
          instituicao: values.instituicao,
          tipo: Number(values.tipo),
          bandeira: values.bandeira,
          ultimos4Digitos: values.ultimos4Digitos,
          diaFechamento: values.diaFechamento,
          diaVencimento: values.diaVencimento,
          contaPadraoPagamento: values.contaPadraoPagamento || "",
          saldo: values.saldo,
          descricao: values.descricao || "",
          usuarioId: session.usuario.id,
        },
        session.token
      );

      resetForms();
      setOpen(false);
      onCreated?.();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel cadastrar o cartao.");
      }
    } finally {
      setIsSubmittingCartao(false);
    }
  }

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        setOpen(nextOpen);
        if (!nextOpen) {
          resetForms();
          setActiveTab("contas");
        }
      }}
    >
      <DialogTrigger asChild>
        <Button variant="outline">Gerenciar contas e cartoes</Button>
      </DialogTrigger>
      <DialogContent className="max-w-3xl">
        <DialogHeader>
          <DialogTitle>Gerenciar contas e cartoes</DialogTitle>
          <DialogDescription>
            Escolha o tipo de cadastro que deseja fazer e conclua tudo aqui mesmo.
          </DialogDescription>
        </DialogHeader>

        <Tabs
          value={activeTab}
          onValueChange={(value) => {
            setActiveTab(value as ActiveTab);
            setErrorMessage("");
          }}
          className="space-y-4"
        >
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="contas">Contas</TabsTrigger>
            <TabsTrigger value="cartoes">Cartoes</TabsTrigger>
          </TabsList>

          <TabsContent value="contas">
            <Form {...contaForm}>
              <form
                onSubmit={contaForm.handleSubmit(handleCadastrarConta)}
                className="space-y-4"
              >
                <div className="grid gap-4 md:grid-cols-2">
                  <FormField
                    control={contaForm.control}
                    name="nomeConta"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Nome da conta</FormLabel>
                        <FormControl>
                          <Input placeholder="Ex: Conta principal" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={contaForm.control}
                    name="instituicao"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Instituicao</FormLabel>
                        <FormControl>
                          <Input placeholder="Ex: Nubank, Itau" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <div className="grid gap-4 md:grid-cols-3">
                  <FormField
                    control={contaForm.control}
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
                            <SelectItem value="0">Corrente</SelectItem>
                            <SelectItem value="1">Poupanca</SelectItem>
                            <SelectItem value="2">Investimento</SelectItem>
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={contaForm.control}
                    name="saldo"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Saldo</FormLabel>
                        <FormControl>
                          <Input type="number" step="0.01" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={contaForm.control}
                    name="saldoInvestimento"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Saldo investimento</FormLabel>
                        <FormControl>
                          <Input type="number" step="0.01" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <FormField
                  control={contaForm.control}
                  name="descricao"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Descricao</FormLabel>
                      <FormControl>
                        <Textarea placeholder="Observacoes sobre a conta" {...field} />
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

                <div className="flex justify-end gap-2">
                  <Button type="button" variant="outline" onClick={() => setOpen(false)}>
                    Cancelar
                  </Button>
                  <Button type="submit" disabled={isSubmittingConta}>
                    {isSubmittingConta ? "Salvando..." : "Salvar conta"}
                  </Button>
                </div>
              </form>
            </Form>
          </TabsContent>

          <TabsContent value="cartoes">
            <Form {...cartaoForm}>
              <form
                onSubmit={cartaoForm.handleSubmit(handleCadastrarCartao)}
                className="space-y-4"
              >
                <div className="grid gap-4 md:grid-cols-2">
                  <FormField
                    control={cartaoForm.control}
                    name="nomeCartao"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Nome do cartao</FormLabel>
                        <FormControl>
                          <Input placeholder="Ex: Visa principal" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={cartaoForm.control}
                    name="instituicao"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Instituicao</FormLabel>
                        <FormControl>
                          <Input placeholder="Ex: Nubank, Itau" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <div className="grid gap-4 md:grid-cols-3">
                  <FormField
                    control={cartaoForm.control}
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
                            <SelectItem value="0">Credito</SelectItem>
                            <SelectItem value="1">Debito</SelectItem>
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={cartaoForm.control}
                    name="bandeira"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Bandeira</FormLabel>
                        <FormControl>
                          <Input placeholder="Ex: Visa, Master" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={cartaoForm.control}
                    name="ultimos4Digitos"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Ultimos 4 digitos</FormLabel>
                        <FormControl>
                          <Input maxLength={4} placeholder="1234" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <div className="grid gap-4 md:grid-cols-3">
                  <FormField
                    control={cartaoForm.control}
                    name="diaFechamento"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Dia fechamento</FormLabel>
                        <FormControl>
                          <Input placeholder="Ex: 10" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={cartaoForm.control}
                    name="diaVencimento"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Dia vencimento</FormLabel>
                        <FormControl>
                          <Input placeholder="Ex: 15" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={cartaoForm.control}
                    name="saldo"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Saldo</FormLabel>
                        <FormControl>
                          <Input type="number" step="0.01" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <FormField
                  control={cartaoForm.control}
                  name="contaPadraoPagamento"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Conta padrao pagamento</FormLabel>
                      <FormControl>
                        <Input placeholder="Ex: Conta principal" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={cartaoForm.control}
                  name="descricao"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Descricao</FormLabel>
                      <FormControl>
                        <Textarea placeholder="Observacoes sobre o cartao" {...field} />
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

                <div className="flex justify-end gap-2">
                  <Button type="button" variant="outline" onClick={() => setOpen(false)}>
                    Cancelar
                  </Button>
                  <Button type="submit" disabled={isSubmittingCartao}>
                    {isSubmittingCartao ? "Salvando..." : "Salvar cartao"}
                  </Button>
                </div>
              </form>
            </Form>
          </TabsContent>
        </Tabs>
      </DialogContent>
    </Dialog>
  );
}
