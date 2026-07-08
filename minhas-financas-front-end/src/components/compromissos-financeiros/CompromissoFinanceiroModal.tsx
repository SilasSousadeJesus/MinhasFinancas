"use client";

import { useEffect, useMemo, useState } from "react";

import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import {
  CompromissoFinanceiroItem,
  OrigemCompromissoFinanceiro,
  SalvarCompromissoFinanceiroPayload,
} from "@/types/compromissos-financeiros";

interface CompromissoFinanceiroModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  initialData?: CompromissoFinanceiroItem | null;
  initialDescricao?: string;
  initialObservacoes?: string;
  defaultOrigin?: OrigemCompromissoFinanceiro;
  title?: string;
  description?: string;
  submitLabel?: string;
  onSubmit: (payload: SalvarCompromissoFinanceiroPayload) => Promise<void>;
}

export function CompromissoFinanceiroModal({
  open,
  onOpenChange,
  mode,
  initialData,
  initialDescricao,
  initialObservacoes,
  defaultOrigin = OrigemCompromissoFinanceiro.Manual,
  title,
  description,
  submitLabel,
  onSubmit,
}: CompromissoFinanceiroModalProps) {
  const [descricao, setDescricao] = useState("");
  const [origem, setOrigem] = useState<OrigemCompromissoFinanceiro>(defaultOrigin);
  const [observacoes, setObservacoes] = useState("");
  const [salvando, setSalvando] = useState(false);

  const tituloModal = useMemo(() => {
    if (title) {
      return title;
    }

    return mode === "edit" ? "Editar compromisso financeiro" : "Novo compromisso financeiro";
  }, [mode, title]);

  const descricaoModal = useMemo(() => {
    if (description) {
      return description;
    }

    return mode === "edit"
      ? "Atualize o texto e as observações do compromisso."
      : "Registre uma intenção financeira que precisa ser acompanhada ao longo do tempo.";
  }, [description, mode]);

  useEffect(() => {
    if (!open) {
      return;
    }

    setDescricao(initialData?.descricao ?? initialDescricao ?? "");
    setOrigem(initialData?.origem ?? defaultOrigin);
    setObservacoes(initialData?.observacoes ?? initialObservacoes ?? "");
  }, [defaultOrigin, initialData, initialDescricao, initialObservacoes, open]);

  async function handleSubmit() {
    const descricaoFinal = descricao.trim();
    if (!descricaoFinal) {
      return;
    }

    try {
      setSalvando(true);
      await onSubmit({
        descricao: descricaoFinal,
        origem,
        observacoes: observacoes.trim() ? observacoes.trim() : null,
      });
    } finally {
      setSalvando(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{tituloModal}</DialogTitle>
          <DialogDescription>{descricaoModal}</DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div className="space-y-2">
            <label className="text-sm font-medium">Descrição</label>
            <Input
              value={descricao}
              onChange={(event) => setDescricao(event.target.value)}
              placeholder="Ex.: Não assumir novos financiamentos até concluir a reserva"
            />
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <label className="text-sm font-medium">Origem</label>
              <Select
                value={String(origem)}
                onValueChange={(valor) => setOrigem(Number(valor) as OrigemCompromissoFinanceiro)}
                disabled={mode === "edit"}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={String(OrigemCompromissoFinanceiro.Manual)}>Manual</SelectItem>
                  <SelectItem value={String(OrigemCompromissoFinanceiro.IA)}>IA</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium">Observações</label>
            <Textarea
              value={observacoes}
              onChange={(event) => setObservacoes(event.target.value)}
              placeholder="Contexto opcional do compromisso."
              className="min-h-[110px]"
            />
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSubmit} disabled={salvando || !descricao.trim()}>
            {salvando ? "Salvando..." : submitLabel ?? (mode === "edit" ? "Salvar alterações" : "Confirmar")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
