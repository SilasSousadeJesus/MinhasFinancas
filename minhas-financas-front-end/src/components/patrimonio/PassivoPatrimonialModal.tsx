"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import { PassivoPatrimonialItem, PassivoPatrimonialPayload } from "@/types/patrimonio";
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
  nomeBemPatrimonial: z
    .string()
    .min(2, "Informe o nome do passivo.")
    .max(60, "O nome do passivo deve ter no máximo 60 caracteres."),
  tipo: z.string(),
  valorAtual: z.coerce.number().min(0, "Informe um valor válido."),
  dataInicio: z.string().optional(),
  dataFim: z.string().optional(),
  descricao: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

interface PassivoPatrimonialModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  initialData?: PassivoPatrimonialItem | null;
  onSubmit: (payload: Omit<PassivoPatrimonialPayload, "usuarioId">) => Promise<void>;
}

export function PassivoPatrimonialModal({
  open,
  onOpenChange,
  mode,
  initialData,
  onSubmit,
}: PassivoPatrimonialModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      nomeBemPatrimonial: "",
      tipo: "0",
      valorAtual: 0,
      dataInicio: "",
      dataFim: "",
      descricao: "",
    },
  });

  useEffect(() => {
    if (!open) {
      return;
    }

    form.reset({
      nomeBemPatrimonial: initialData?.nome ?? "",
      tipo: String(initialData?.tipo ?? 0),
      valorAtual: initialData?.valorAtual ?? 0,
      dataInicio: initialData?.dataInicio?.slice(0, 10) ?? "",
      dataFim: initialData?.dataFim?.slice(0, 10) ?? "",
      descricao: initialData?.descricao ?? "",
    });
    setErrorMessage("");
  }, [form, initialData, open]);

  async function handleSubmit(values: FormValues) {
    try {
      setIsSubmitting(true);
      setErrorMessage("");

      await onSubmit({
        nomeBemPatrimonial: values.nomeBemPatrimonial,
        descricao: values.descricao || "",
        tipo: Number(values.tipo),
        valorAtual: values.valorAtual,
        dataInicio: values.dataInicio || null,
        dataFim: values.dataFim || null,
      });

      onOpenChange(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível salvar o passivo.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "Novo passivo" : "Editar passivo"}</DialogTitle>
          <DialogDescription>
            Cadastre financiamentos, empréstimos e outras obrigações que reduzem o patrimônio líquido.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="nomeBemPatrimonial"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Nome do passivo</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: Financiamento do apartamento" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

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
                        <SelectItem value="0">Financiamento</SelectItem>
                        <SelectItem value="1">Empréstimo</SelectItem>
                        <SelectItem value="2">Dívida</SelectItem>
                        <SelectItem value="3">Parcelamento</SelectItem>
                        <SelectItem value="4">Obrigação financeira</SelectItem>
                        <SelectItem value="5">Outro</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid gap-4 md:grid-cols-3">
              <FormField
                control={form.control}
                name="valorAtual"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Valor atual</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.01" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="dataInicio"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data de início</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} value={field.value ?? ""} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="dataFim"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data final</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} value={field.value ?? ""} />
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
                  <FormLabel>Observação</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Detalhes relevantes sobre a obrigação financeira"
                      {...field}
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

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Salvando..." : "Salvar passivo"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
