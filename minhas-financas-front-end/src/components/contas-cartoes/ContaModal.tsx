"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import { ContaItem, ContaPayload } from "@/types/contas-cartoes";
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
import { useState } from "react";

const formSchema = z.object({
  nomeConta: z
    .string()
    .min(2, "Informe o nome da conta.")
    .max(20, "O nome da conta deve ter no máximo 20 caracteres."),
  instituicao: z
    .string()
    .min(2, "Informe a instituição.")
    .max(20, "A instituição deve ter no máximo 20 caracteres."),
  tipo: z.string(),
  saldo: z.coerce.number(),
  saldoInvestimento: z.coerce.number(),
  descricao: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

interface ContaModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  initialData?: ContaItem | null;
  onSubmit: (payload: ContaPayload) => Promise<void>;
}

export function ContaModal({
  open,
  onOpenChange,
  mode,
  initialData,
  onSubmit,
}: ContaModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      nomeConta: "",
      instituicao: "",
      tipo: "0",
      saldo: 0,
      saldoInvestimento: 0,
      descricao: "",
    },
  });

  useEffect(() => {
    if (!open) {
      return;
    }

    form.reset({
      nomeConta: initialData?.nomeConta ?? "",
      instituicao: initialData?.instituicao ?? "",
      tipo: String(initialData?.tipo ?? 0),
      saldo: initialData?.saldo ?? 0,
      saldoInvestimento: initialData?.saldoInvestimento ?? 0,
      descricao: initialData?.descricao ?? "",
    });
    setErrorMessage("");
  }, [form, initialData, open]);

  async function handleSubmit(values: FormValues) {
    try {
      setIsSubmitting(true);
      setErrorMessage("");

      await onSubmit({
        nomeConta: values.nomeConta,
        instituicao: values.instituicao,
        tipo: Number(values.tipo),
        saldo: values.saldo,
        saldoInvestimento: values.saldoInvestimento,
        descricao: values.descricao || "",
      });

      onOpenChange(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível salvar a conta.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "Nova conta" : "Editar conta"}</DialogTitle>
          <DialogDescription>
            Cadastre e gerencie as contas que alimentam os lançamentos e o dashboard.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
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
                        <SelectItem value="0">Corrente</SelectItem>
                        <SelectItem value="1">Poupança</SelectItem>
                        <SelectItem value="2">Investimento</SelectItem>
                      </SelectContent>
                    </Select>
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
              <FormField
                control={form.control}
                name="saldoInvestimento"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Saldo em investimentos</FormLabel>
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
              name="descricao"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Descrição</FormLabel>
                  <FormControl>
                    <Textarea placeholder="Observações sobre a conta" {...field} />
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
                {isSubmitting ? "Salvando..." : "Salvar conta"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
