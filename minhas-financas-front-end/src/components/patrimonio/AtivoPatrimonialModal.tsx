"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import { AtivoPatrimonialItem, AtivoPatrimonialPayload } from "@/types/patrimonio";
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
    .min(2, "Informe o nome do ativo.")
    .max(60, "O nome do ativo deve ter no máximo 60 caracteres."),
  tipo: z.string(),
  valorAtual: z.coerce.number().min(0, "Informe um valor válido."),
  dataAquisicao: z.string().optional(),
  descricao: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

interface AtivoPatrimonialModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  initialData?: AtivoPatrimonialItem | null;
  onSubmit: (payload: Omit<AtivoPatrimonialPayload, "usuarioId">) => Promise<void>;
}

export function AtivoPatrimonialModal({
  open,
  onOpenChange,
  mode,
  initialData,
  onSubmit,
}: AtivoPatrimonialModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      nomeBemPatrimonial: "",
      tipo: "6",
      valorAtual: 0,
      dataAquisicao: "",
      descricao: "",
    },
  });

  useEffect(() => {
    if (!open) {
      return;
    }

    form.reset({
      nomeBemPatrimonial: initialData?.nome ?? "",
      tipo: String(initialData?.tipo ?? 6),
      valorAtual: initialData?.valorAtual ?? 0,
      dataAquisicao: initialData?.dataAquisicao?.slice(0, 10) ?? "",
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
        dataAquisicao: values.dataAquisicao || null,
      });

      onOpenChange(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível salvar o ativo.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "Novo ativo" : "Editar ativo"}</DialogTitle>
          <DialogDescription>
            Cadastre bens e ativos que compõem positivamente o seu patrimônio.
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
                    <FormLabel>Nome do ativo</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: Reserva em corretora" {...field} />
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
                        <SelectItem value="0">Imóvel</SelectItem>
                        <SelectItem value="1">Automóvel</SelectItem>
                        <SelectItem value="2">Investimento</SelectItem>
                        <SelectItem value="3">Dinheiro em conta</SelectItem>
                        <SelectItem value="4">Equipamento</SelectItem>
                        <SelectItem value="5">Instrumento musical</SelectItem>
                        <SelectItem value="6">Outro</SelectItem>
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
                name="dataAquisicao"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data de aquisição</FormLabel>
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
                      placeholder="Contexto, instituição, localização ou observações úteis"
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
                {isSubmitting ? "Salvando..." : "Salvar ativo"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
