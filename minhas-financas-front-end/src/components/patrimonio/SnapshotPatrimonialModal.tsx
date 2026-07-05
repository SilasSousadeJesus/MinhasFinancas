"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@/types/api";
import { SnapshotPatrimonialPayload } from "@/types/patrimonio";
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
import { Textarea } from "@/components/ui/textarea";

const formSchema = z.object({
  dataReferencia: z.string().min(1, "Informe a data de referência."),
  observacao: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

interface SnapshotPatrimonialModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (payload: SnapshotPatrimonialPayload) => Promise<void>;
}

export function SnapshotPatrimonialModal({
  open,
  onOpenChange,
  onSubmit,
}: SnapshotPatrimonialModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      dataReferencia: "",
      observacao: "",
    },
  });

  useEffect(() => {
    if (!open) {
      return;
    }

    form.reset({
      dataReferencia: new Date().toISOString().slice(0, 10),
      observacao: "",
    });
    setErrorMessage("");
  }, [form, open]);

  async function handleSubmit(values: FormValues) {
    try {
      setIsSubmitting(true);
      setErrorMessage("");

      await onSubmit({
        dataReferencia: values.dataReferencia,
        observacao: values.observacao || "",
      });

      onOpenChange(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível gerar o snapshot patrimonial.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Gerar snapshot patrimonial</DialogTitle>
          <DialogDescription>
            Salve uma fotografia do patrimônio atual para acompanhar sua evolução ao longo do tempo.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="dataReferencia"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Data de referência</FormLabel>
                  <FormControl>
                    <Input type="date" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="observacao"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Observação</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Opcional: contexto desta fotografia patrimonial"
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
                {isSubmitting ? "Gerando..." : "Gerar snapshot"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
