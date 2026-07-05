"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import { CartaoItem, CartaoPayload } from "@/types/contas-cartoes";
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

const formSchema = z.object({
  nomeCartao: z
    .string()
    .min(2, "Informe o nome do cartão.")
    .max(20, "O nome do cartão deve ter no máximo 20 caracteres."),
  instituicao: z.string().min(2, "Informe a instituição."),
  tipo: z.string(),
  bandeira: z.string().min(2, "Informe a bandeira."),
  ultimos4Digitos: z
    .string()
    .regex(/^\d{4}$/, "Informe exatamente 4 dígitos."),
  diaFechamento: z.string().min(1, "Informe o dia de fechamento."),
  diaVencimento: z.string().min(1, "Informe o dia de vencimento."),
  contaPadraoPagamento: z.string().optional(),
  saldo: z.coerce.number(),
  descricao: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

interface CartaoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  initialData?: CartaoItem | null;
  onSubmit: (payload: CartaoPayload) => Promise<void>;
}

export function CartaoModal({
  open,
  onOpenChange,
  mode,
  initialData,
  onSubmit,
}: CartaoModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
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

  useEffect(() => {
    if (!open) {
      return;
    }

    form.reset({
      nomeCartao: initialData?.nomeCartao ?? "",
      instituicao: initialData?.instituicao ?? "",
      tipo: String(initialData?.tipo ?? 0),
      bandeira: initialData?.bandeira ?? "",
      ultimos4Digitos: initialData?.ultimos4Digitos ?? "",
      diaFechamento: initialData?.diaFechamento ?? "",
      diaVencimento: initialData?.diaVencimento ?? "",
      contaPadraoPagamento: initialData?.contaPadraoPagamento ?? "",
      saldo: initialData?.saldo ?? 0,
      descricao: initialData?.descricao ?? "",
    });
    setErrorMessage("");
  }, [form, initialData, open]);

  async function handleSubmit(values: FormValues) {
    try {
      setIsSubmitting(true);
      setErrorMessage("");

      await onSubmit({
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
      });

      onOpenChange(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível salvar o cartão.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "Novo cartão" : "Editar cartão"}</DialogTitle>
          <DialogDescription>
            Cadastre e gerencie os cartões utilizados no seu controle financeiro.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="nomeCartao"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Nome do cartão</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: Visa principal" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="instituicao"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Instituição</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: Nubank, Itaú" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid gap-4 md:grid-cols-3">
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
                        <SelectItem value="0">Crédito</SelectItem>
                        <SelectItem value="1">Débito</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
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
                control={form.control}
                name="ultimos4Digitos"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Últimos 4 dígitos</FormLabel>
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
                control={form.control}
                name="diaFechamento"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Dia de fechamento</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: 10" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="diaVencimento"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Dia de vencimento</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: 15" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
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
              control={form.control}
              name="contaPadraoPagamento"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Conta padrão para pagamento</FormLabel>
                  <FormControl>
                    <Input placeholder="Ex: Conta principal" {...field} />
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
                  <FormLabel>Descrição</FormLabel>
                  <FormControl>
                    <Textarea placeholder="Observações sobre o cartão" {...field} />
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

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Salvando..." : "Salvar cartão"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
