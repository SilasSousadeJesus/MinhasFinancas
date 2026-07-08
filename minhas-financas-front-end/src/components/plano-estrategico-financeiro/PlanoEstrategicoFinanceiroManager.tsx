"use client"

import { useEffect, useMemo, useRef, useState } from "react"
import { Eye, Plus, RotateCcw, Save, Trash2 } from "lucide-react"

import { useAuth } from "@/providers/auth-provider"
import { ApiError } from "@/types/api"
import {
  ObjetivoPlanoEstrategico,
  PlanoEstrategicoFinanceiroDetalhe,
  PlanoEstrategicoFinanceiroResumo,
  PrioridadeObjetivoPlanoEstrategico,
  SalvarPlanoEstrategicoFinanceiroPayload,
  StatusObjetivoPlanoEstrategico,
} from "@/types/plano-estrategico-financeiro"
import {
  atualizarVersaoPlanoEstrategico,
  buscarPlanoEstrategico,
  buscarPlanoEstrategicoVigente,
  criarPlanoEstrategico,
  listarPlanosEstrategicos,
} from "@/services/api/plano-estrategico-financeiro"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Badge } from "@/components/ui/badge"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Sidebar } from "@/components/Sidebar/Sidebar"

interface ObjetivoEditor {
  id: string
  titulo: string
  descricao: string
  prioridade: PrioridadeObjetivoPlanoEstrategico
  status: StatusObjetivoPlanoEstrategico
  dataAlvo: string
  valorAlvo: string
  valorAtual: string
  observacao: string
}

interface PlanoEditor {
  nome: string
  descricao: string
  observacao: string
  dataInicioVigencia: string
  objetivos: ObjetivoEditor[]
}

const prioridadeLabels: Record<PrioridadeObjetivoPlanoEstrategico, string> = {
  [PrioridadeObjetivoPlanoEstrategico.Baixa]: "Baixa",
  [PrioridadeObjetivoPlanoEstrategico.Media]: "Média",
  [PrioridadeObjetivoPlanoEstrategico.Alta]: "Alta",
  [PrioridadeObjetivoPlanoEstrategico.Critica]: "Crítica",
}

const statusLabels: Record<StatusObjetivoPlanoEstrategico, string> = {
  [StatusObjetivoPlanoEstrategico.Planejado]: "Planejado",
  [StatusObjetivoPlanoEstrategico.EmAndamento]: "Em andamento",
  [StatusObjetivoPlanoEstrategico.Concluido]: "Concluído",
  [StatusObjetivoPlanoEstrategico.Cancelado]: "Cancelado",
}

function criarIdLocal() {
  if (typeof crypto !== "undefined" && "randomUUID" in crypto) {
    return crypto.randomUUID()
  }

  return `${Date.now()}-${Math.random().toString(36).slice(2)}`
}

function formatarMoeda(valor?: number | null) {
  if (valor == null || Number.isNaN(valor)) {
    return "—"
  }

  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(valor)
}

function formatarData(valor?: string | null) {
  if (!valor) {
    return "—"
  }

  const data = new Date(valor)
  if (Number.isNaN(data.getTime())) {
    return "—"
  }

  return data.toLocaleDateString("pt-BR")
}

function formatarDataHora(valor?: string | null) {
  if (!valor) {
    return "—"
  }

  const data = new Date(valor)
  if (Number.isNaN(data.getTime())) {
    return "—"
  }

  return data.toLocaleString("pt-BR", {
    dateStyle: "short",
    timeStyle: "short",
  })
}

function paraInputDate(valor?: string | null) {
  if (!valor) {
    return ""
  }

  return valor.slice(0, 10)
}

function criarObjetivoEditor(): ObjetivoEditor {
  return {
    id: criarIdLocal(),
    titulo: "",
    descricao: "",
    prioridade: PrioridadeObjetivoPlanoEstrategico.Media,
    status: StatusObjetivoPlanoEstrategico.Planejado,
    dataAlvo: "",
    valorAlvo: "",
    valorAtual: "",
    observacao: "",
  }
}

function criarEditorVazio(): PlanoEditor {
  return {
    nome: "",
    descricao: "",
    observacao: "",
    dataInicioVigencia: new Date().toISOString().slice(0, 10),
    objetivos: [criarObjetivoEditor()],
  }
}

function mapearObjetivoParaEditor(objetivo: ObjetivoPlanoEstrategico): ObjetivoEditor {
  return {
    id: objetivo.id ?? criarIdLocal(),
    titulo: objetivo.titulo ?? "",
    descricao: objetivo.descricao ?? "",
    prioridade: objetivo.prioridade,
    status: objetivo.status,
    dataAlvo: paraInputDate(objetivo.dataAlvo),
    valorAlvo: objetivo.valorAlvo != null ? String(objetivo.valorAlvo) : "",
    valorAtual: objetivo.valorAtual != null ? String(objetivo.valorAtual) : "",
    observacao: objetivo.observacao ?? "",
  }
}

function mapearDetalheParaEditor(plano?: PlanoEstrategicoFinanceiroDetalhe | null): PlanoEditor {
  if (!plano) {
    return criarEditorVazio()
  }

  return {
    nome: plano.nome ?? "",
    descricao: plano.descricao ?? "",
    observacao: plano.observacao ?? "",
    dataInicioVigencia: paraInputDate(plano.dataInicioVigencia) || new Date().toISOString().slice(0, 10),
    objetivos:
      plano.objetivos?.length > 0
        ? plano.objetivos.map((objetivo) => mapearObjetivoParaEditor(objetivo))
        : [criarObjetivoEditor()],
  }
}

function extrairNumero(valor: string) {
  if (!valor.trim()) {
    return null
  }

  const numero = Number(valor)
  return Number.isNaN(numero) ? null : numero
}

function obterStatusPlano(plano?: PlanoEstrategicoFinanceiroResumo | PlanoEstrategicoFinanceiroDetalhe | null) {
  if (!plano) {
    return "Sem plano ativo"
  }

  return plano.ativo ? "Vigente" : "Histórico"
}

export function PlanoEstrategicoFinanceiroManager() {
  const { session } = useAuth()
  const detalheRef = useRef<HTMLDivElement | null>(null)

  const [planos, setPlanos] = useState<PlanoEstrategicoFinanceiroResumo[]>([])
  const [planoVigente, setPlanoVigente] = useState<PlanoEstrategicoFinanceiroDetalhe | null>(null)
  const [planoVisualizado, setPlanoVisualizado] = useState<PlanoEstrategicoFinanceiroDetalhe | null>(null)
  const [editor, setEditor] = useState<PlanoEditor>(criarEditorVazio())
  const [isLoading, setIsLoading] = useState(true)
  const [isLoadingDetalhe, setIsLoadingDetalhe] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")
  const [successMessage, setSuccessMessage] = useState("")

  async function carregarDados() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessão inválida. Faça login novamente.")
      setIsLoading(false)
      return
    }

    try {
      setIsLoading(true)
      setErrorMessage("")
      setSuccessMessage("")

      const [listaResponse, vigenteResponse] = await Promise.allSettled([
        listarPlanosEstrategicos(session.usuario.id, session.token),
        buscarPlanoEstrategicoVigente(session.usuario.id, session.token),
      ])

      if (listaResponse.status === "fulfilled") {
        const dados = listaResponse.value.dados ?? []
        setPlanos(dados)
      } else if (listaResponse.reason instanceof ApiError) {
        setErrorMessage(listaResponse.reason.message)
      } else {
        setErrorMessage("Não foi possível carregar o histórico do plano estratégico.")
      }

      if (vigenteResponse.status === "fulfilled") {
        const plano = vigenteResponse.value.dados
        setPlanoVigente(plano)
        setEditor(mapearDetalheParaEditor(plano))
      } else {
        setPlanoVigente(null)
        setEditor(criarEditorVazio())
      }
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message)
      } else {
        setErrorMessage("Não foi possível carregar o plano estratégico financeiro.")
      }
      setPlanoVigente(null)
      setEditor(criarEditorVazio())
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void carregarDados()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [session?.token, session?.usuario.id])

  const planosOrdenados = useMemo(() => {
    return [...planos].sort((a, b) => {
      if (a.ativo !== b.ativo) {
        return a.ativo ? -1 : 1
      }

      const dataAtualizacaoA = new Date(a.dataAtualizacao).getTime()
      const dataAtualizacaoB = new Date(b.dataAtualizacao).getTime()
      if (dataAtualizacaoA !== dataAtualizacaoB) {
        return dataAtualizacaoB - dataAtualizacaoA
      }

      return b.numeroVersao - a.numeroVersao
    })
  }, [planos])

  const objetivoEmEdicaoQuantidade = editor.objetivos.length

  const resumoCards = useMemo(() => {
    return [
      {
        titulo: "Situação",
        valor: obterStatusPlano(planoVigente),
      },
      {
        titulo: "Versão vigente",
        valor: planoVigente ? `V${planoVigente.numeroVersao}` : "Sem versão ativa",
      },
      {
        titulo: "Objetivos em edição",
        valor: `${objetivoEmEdicaoQuantidade}`,
      },
      {
        titulo: "Versões no histórico",
        valor: `${planosOrdenados.length}`,
      },
    ]
  }, [objetivoEmEdicaoQuantidade, planoVigente, planosOrdenados.length])

  function atualizarCampo(campo: keyof PlanoEditor, valor: string) {
    setEditor((atual) => ({
      ...atual,
      [campo]: valor,
    }))
  }

  function atualizarObjetivo(
    id: string,
    campo: keyof ObjetivoEditor,
    valor: string | PrioridadeObjetivoPlanoEstrategico | StatusObjetivoPlanoEstrategico
  ) {
    setEditor((atual) => ({
      ...atual,
      objetivos: atual.objetivos.map((objetivo) =>
        objetivo.id === id
          ? {
              ...objetivo,
              [campo]: valor,
            }
          : objetivo
      ),
    }))
  }

  function adicionarObjetivo() {
    setEditor((atual) => ({
      ...atual,
      objetivos: [...atual.objetivos, criarObjetivoEditor()],
    }))
  }

  function removerObjetivo(id: string) {
    setEditor((atual) => {
      if (atual.objetivos.length <= 1) {
        return {
          ...atual,
          objetivos: [criarObjetivoEditor()],
        }
      }

      return {
        ...atual,
        objetivos: atual.objetivos.filter((objetivo) => objetivo.id !== id),
      }
    })
  }

  function construirPayload(): SalvarPlanoEstrategicoFinanceiroPayload {
    return {
      nome: editor.nome.trim(),
      descricao: editor.descricao.trim() ? editor.descricao.trim() : null,
      observacao: editor.observacao.trim() ? editor.observacao.trim() : null,
      dataInicioVigencia: editor.dataInicioVigencia || null,
      objetivos: editor.objetivos.map((objetivo, index) => ({
        titulo: objetivo.titulo.trim(),
        descricao: objetivo.descricao.trim() ? objetivo.descricao.trim() : null,
        prioridade: objetivo.prioridade,
        status: objetivo.status,
        ordem: index + 1,
        dataAlvo: objetivo.dataAlvo || null,
        valorAlvo: extrairNumero(objetivo.valorAlvo),
        valorAtual: extrairNumero(objetivo.valorAtual),
        observacao: objetivo.observacao.trim() ? objetivo.observacao.trim() : null,
      })),
    }
  }

  async function salvarPlano() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessão inválida. Faça login novamente.")
      return
    }

    if (!editor.nome.trim()) {
      setErrorMessage("Informe um nome para o plano estratégico.")
      return
    }

    if (editor.objetivos.length === 0) {
      setErrorMessage("Informe pelo menos um objetivo estratégico.")
      return
    }

    if (editor.objetivos.some((objetivo) => !objetivo.titulo.trim())) {
      setErrorMessage("Todos os objetivos precisam ter um título.")
      return
    }

    try {
      setIsSaving(true)
      setErrorMessage("")
      setSuccessMessage("")

      const payload = construirPayload()

      const response = planoVigente
        ? await atualizarVersaoPlanoEstrategico(
            session.usuario.id,
            planoVigente.id,
            payload,
            session.token
          )
        : await criarPlanoEstrategico(session.usuario.id, payload, session.token)

      const dados = response.dados
      setSuccessMessage(
        planoVigente
          ? "Nova versão do plano estratégico criada com sucesso."
          : "Plano estratégico criado com sucesso."
      )

      if (dados) {
        setPlanoVigente(dados)
        setEditor(mapearDetalheParaEditor(dados))
        await carregarDados()
      }
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message)
      } else {
        setErrorMessage("Não foi possível salvar o plano estratégico.")
      }
    } finally {
      setIsSaving(false)
    }
  }

  async function visualizarPlano(planoId: string) {
    if (!session?.usuario.id || !session.token) {
      return
    }

    try {
      setIsLoadingDetalhe(true)
      setErrorMessage("")

      const response = await buscarPlanoEstrategico(session.usuario.id, planoId, session.token)
      setPlanoVisualizado(response.dados ?? null)

      requestAnimationFrame(() => {
        detalheRef.current?.scrollIntoView({
          behavior: "smooth",
          block: "start",
        })
      })
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message)
      } else {
        setErrorMessage("Não foi possível carregar a versão selecionada.")
      }
    } finally {
      setIsLoadingDetalhe(false)
    }
  }

  function usarPlanoVisualizadoComoBase() {
    if (!planoVisualizado) {
      return
    }

    setEditor(mapearDetalheParaEditor(planoVisualizado))
    setSuccessMessage("A versão selecionada foi carregada como base para edição.")
  }

  return (
    <div className="flex flex-row bg-background">
      <Sidebar />

      <main className="flex-1 px-6 py-8 md:px-8">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
          <section className="space-y-2">
            <h1 className="text-3xl font-semibold tracking-tight">
              Plano Estratégico Financeiro
            </h1>
            <p className="max-w-3xl text-sm text-muted-foreground">
              Organize a direção financeira de longo prazo do usuário, edite a versão vigente
              criando histórico e consulte versões anteriores sem sobrescrever o passado.
            </p>
          </section>

          <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
            {resumoCards.map((card) => (
              <Card key={card.titulo}>
                <CardHeader className="pb-2">
                  <CardDescription>{card.titulo}</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-semibold">{card.valor}</div>
                </CardContent>
              </Card>
            ))}
          </section>

          {errorMessage ? (
            <div className="rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
              {errorMessage}
            </div>
          ) : null}

          {successMessage ? (
            <div className="rounded-md border border-emerald-500/20 bg-emerald-500/5 px-4 py-3 text-sm text-emerald-700 dark:text-emerald-300">
              {successMessage}
            </div>
          ) : null}

          <div className="grid gap-6 xl:grid-cols-[minmax(0,1.2fr)_minmax(340px,0.8fr)]">
            <Card>
              <CardHeader>
                <div className="flex items-start justify-between gap-4">
                  <div className="space-y-2">
                    <div className="flex items-center gap-2">
                      <CardTitle>Versão em edição</CardTitle>
                      <Badge variant="secondary">
                        {planoVigente ? `V${planoVigente.numeroVersao} vigente` : "Primeiro plano"}
                      </Badge>
                    </div>
                    <CardDescription>
                      {planoVigente
                        ? "Editar esta tela cria uma nova versão e preserva o histórico do usuário."
                        : "Cadastre a primeira versão do plano estratégico para começar a registrar a direção escolhida."}
                    </CardDescription>
                  </div>

                  <div className="flex gap-2">
                    <Button type="button" variant="outline" onClick={adicionarObjetivo}>
                      <Plus className="mr-2 h-4 w-4" />
                      Novo objetivo
                    </Button>
                    <Button type="button" onClick={salvarPlano} disabled={isSaving}>
                      <Save className="mr-2 h-4 w-4" />
                      {planoVigente ? "Salvar nova versão" : "Criar primeiro plano"}
                    </Button>
                  </div>
                </div>
              </CardHeader>

              <CardContent className="space-y-6">
                <div className="grid gap-4 md:grid-cols-2">
                  <div className="space-y-2">
                    <label className="text-sm font-medium">Nome do plano</label>
                    <Input
                      value={editor.nome}
                      onChange={(event) => atualizarCampo("nome", event.target.value)}
                      placeholder="Ex.: Direção 2026"
                    />
                  </div>

                  <div className="space-y-2">
                    <label className="text-sm font-medium">Início da vigência</label>
                    <Input
                      type="date"
                      value={editor.dataInicioVigencia}
                      onChange={(event) => atualizarCampo("dataInicioVigencia", event.target.value)}
                    />
                  </div>
                </div>

                <div className="grid gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium">Descrição</label>
                    <Textarea
                      value={editor.descricao}
                      onChange={(event) => atualizarCampo("descricao", event.target.value)}
                      placeholder="Explique o contexto estratégico deste plano."
                      className="min-h-[110px]"
                    />
                  </div>

                  <div className="space-y-2">
                    <label className="text-sm font-medium">Observação</label>
                    <Textarea
                      value={editor.observacao}
                      onChange={(event) => atualizarCampo("observacao", event.target.value)}
                      placeholder="Anotações complementares, premissas ou decisões importantes."
                      className="min-h-[90px]"
                    />
                  </div>
                </div>

                <div className="space-y-4">
                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <div>
                      <h3 className="text-lg font-semibold">Objetivos estratégicos</h3>
                      <p className="text-sm text-muted-foreground">
                        Liste os objetivos de longo prazo que orientam as decisões financeiras.
                      </p>
                    </div>

                    <Button type="button" variant="secondary" onClick={adicionarObjetivo}>
                      <Plus className="mr-2 h-4 w-4" />
                      Adicionar objetivo
                    </Button>
                  </div>

                  <div className="max-h-[72vh] space-y-4 overflow-auto pr-1">
                    {editor.objetivos.map((objetivo, index) => (
                      <Card key={objetivo.id} className="border-border/70 bg-muted/10">
                        <CardHeader className="space-y-4 pb-4">
                          <div className="flex items-start justify-between gap-4">
                            <div className="space-y-1">
                              <div className="flex flex-wrap items-center gap-2">
                                <Badge variant="outline">Objetivo {index + 1}</Badge>
                                <Badge variant="secondary">
                                  {prioridadeLabels[objetivo.prioridade]}
                                </Badge>
                                <Badge variant="secondary">{statusLabels[objetivo.status]}</Badge>
                              </div>
                              <CardDescription>
                                Cada objetivo representa uma intenção estratégica concreta.
                              </CardDescription>
                            </div>

                            <Button
                              type="button"
                              variant="ghost"
                              size="icon"
                              className="text-muted-foreground hover:text-destructive"
                              onClick={() => removerObjetivo(objetivo.id)}
                              disabled={editor.objetivos.length === 1}
                              title={
                                editor.objetivos.length === 1
                                  ? "Mantenha ao menos um objetivo"
                                  : "Remover objetivo"
                              }
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                        </CardHeader>

                        <CardContent className="grid gap-4 md:grid-cols-2">
                          <div className="space-y-2 md:col-span-2">
                            <label className="text-sm font-medium">Título</label>
                            <Input
                              value={objetivo.titulo}
                              onChange={(event) =>
                                atualizarObjetivo(objetivo.id, "titulo", event.target.value)
                              }
                              placeholder="Ex.: Formar reserva de emergência"
                            />
                          </div>

                          <div className="space-y-2 md:col-span-2">
                            <label className="text-sm font-medium">Descrição</label>
                            <Textarea
                              value={objetivo.descricao}
                              onChange={(event) =>
                                atualizarObjetivo(objetivo.id, "descricao", event.target.value)
                              }
                              placeholder="Explique o que esse objetivo significa na prática."
                              className="min-h-[90px]"
                            />
                          </div>

                          <div className="space-y-2">
                            <label className="text-sm font-medium">Prioridade</label>
                            <Select
                              value={String(objetivo.prioridade)}
                              onValueChange={(valor) =>
                                atualizarObjetivo(
                                  objetivo.id,
                                  "prioridade",
                                  Number(valor) as PrioridadeObjetivoPlanoEstrategico
                                )
                              }
                            >
                              <SelectTrigger>
                                <SelectValue placeholder="Selecione" />
                              </SelectTrigger>
                              <SelectContent>
                                {Object.values(PrioridadeObjetivoPlanoEstrategico)
                                  .filter((valor) => typeof valor === "number")
                                  .map((valor) => (
                                    <SelectItem key={valor} value={String(valor)}>
                                      {prioridadeLabels[valor as PrioridadeObjetivoPlanoEstrategico]}
                                    </SelectItem>
                                  ))}
                              </SelectContent>
                            </Select>
                          </div>

                          <div className="space-y-2">
                            <label className="text-sm font-medium">Status</label>
                            <Select
                              value={String(objetivo.status)}
                              onValueChange={(valor) =>
                                atualizarObjetivo(
                                  objetivo.id,
                                  "status",
                                  Number(valor) as StatusObjetivoPlanoEstrategico
                                )
                              }
                            >
                              <SelectTrigger>
                                <SelectValue placeholder="Selecione" />
                              </SelectTrigger>
                              <SelectContent>
                                {Object.values(StatusObjetivoPlanoEstrategico)
                                  .filter((valor) => typeof valor === "number")
                                  .map((valor) => (
                                    <SelectItem key={valor} value={String(valor)}>
                                      {statusLabels[valor as StatusObjetivoPlanoEstrategico]}
                                    </SelectItem>
                                  ))}
                              </SelectContent>
                            </Select>
                          </div>

                          <div className="space-y-2">
                            <label className="text-sm font-medium">Data alvo</label>
                            <Input
                              type="date"
                              value={objetivo.dataAlvo}
                              onChange={(event) =>
                                atualizarObjetivo(objetivo.id, "dataAlvo", event.target.value)
                              }
                            />
                          </div>

                          <div className="space-y-2">
                            <label className="text-sm font-medium">Valor alvo</label>
                            <Input
                              type="number"
                              step="0.01"
                              value={objetivo.valorAlvo}
                              onChange={(event) =>
                                atualizarObjetivo(objetivo.id, "valorAlvo", event.target.value)
                              }
                              placeholder="0,00"
                            />
                          </div>

                          <div className="space-y-2">
                            <label className="text-sm font-medium">Valor atual</label>
                            <Input
                              type="number"
                              step="0.01"
                              value={objetivo.valorAtual}
                              onChange={(event) =>
                                atualizarObjetivo(objetivo.id, "valorAtual", event.target.value)
                              }
                              placeholder="0,00"
                            />
                          </div>

                          <div className="space-y-2 md:col-span-2">
                            <label className="text-sm font-medium">Observação</label>
                            <Textarea
                              value={objetivo.observacao}
                              onChange={(event) =>
                                atualizarObjetivo(objetivo.id, "observacao", event.target.value)
                              }
                              placeholder="Contexto adicional deste objetivo."
                              className="min-h-[80px]"
                            />
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>

                  <div className="rounded-lg border border-dashed px-4 py-3 text-sm text-muted-foreground">
                    O plano estratégico precisa manter pelo menos um objetivo para ser salvo.
                  </div>
                </div>
              </CardContent>
            </Card>

            <div className="space-y-6">
              <Card>
                <CardHeader>
                  <CardTitle>Histórico de versões</CardTitle>
                  <CardDescription>
                    Consulte rapidamente as versões anteriores do plano sem alterar o que já
                    foi salvo.
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  {isLoading ? (
                    <div className="rounded-lg border border-dashed px-4 py-10 text-center text-sm text-muted-foreground">
                      Carregando histórico do plano estratégico...
                    </div>
                  ) : planosOrdenados.length === 0 ? (
                    <div className="rounded-lg border border-dashed px-4 py-10 text-center text-sm text-muted-foreground">
                      Ainda não existe um plano estratégico salvo. Crie o primeiro plano para começar.
                    </div>
                  ) : (
                    <div className="overflow-hidden rounded-lg border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Versão</TableHead>
                            <TableHead>Plano</TableHead>
                            <TableHead>Objetivos</TableHead>
                            <TableHead className="text-right">Ações</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {planosOrdenados.map((plano) => (
                            <TableRow key={plano.id}>
                              <TableCell>
                                <div className="flex flex-col gap-1">
                                  <span className="font-medium">V{plano.numeroVersao}</span>
                                  <Badge variant={plano.ativo ? "default" : "secondary"}>
                                    {plano.ativo ? "Vigente" : "Histórico"}
                                  </Badge>
                                </div>
                              </TableCell>
                              <TableCell>
                                <div className="space-y-1">
                                  <p className="font-medium">{plano.nome}</p>
                                  <p className="text-xs text-muted-foreground">
                                    {formatarData(plano.dataInicioVigencia)}
                                  </p>
                                </div>
                              </TableCell>
                              <TableCell>{plano.quantidadeObjetivos}</TableCell>
                              <TableCell className="text-right">
                                <Button
                                  type="button"
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => void visualizarPlano(plano.id)}
                                >
                                  <Eye className="mr-2 h-4 w-4" />
                                  Visualizar
                                </Button>
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  )}
                </CardContent>
              </Card>

              <Card ref={detalheRef}>
                <CardHeader>
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <CardTitle>
                        {planoVisualizado ? planoVisualizado.nome : "Detalhe da versão"}
                      </CardTitle>
                      <CardDescription>
                        {planoVisualizado
                          ? `Versão V${planoVisualizado.numeroVersao} · ${formatarDataHora(
                              planoVisualizado.dataAtualizacao
                            )}`
                          : "Selecione uma versão do histórico para visualizar os detalhes."}
                      </CardDescription>
                    </div>

                    {planoVisualizado ? (
                      <Button type="button" variant="outline" onClick={() => setPlanoVisualizado(null)}>
                        Limpar visualização
                      </Button>
                    ) : null}
                  </div>
                </CardHeader>

                <CardContent>
                  {isLoadingDetalhe ? (
                    <div className="rounded-lg border border-dashed px-4 py-10 text-center text-sm text-muted-foreground">
                      Carregando detalhe da versão...
                    </div>
                  ) : planoVisualizado ? (
                    <div className="space-y-6">
                      <div className="grid gap-3 md:grid-cols-2">
                        <div className="rounded-lg border bg-muted/30 p-4">
                          <p className="text-xs uppercase tracking-[0.2em] text-muted-foreground">
                            Vigência
                          </p>
                          <p className="mt-2 text-base font-semibold">
                            {formatarData(planoVisualizado.dataInicioVigencia)}
                          </p>
                        </div>
                        <div className="rounded-lg border bg-muted/30 p-4">
                          <p className="text-xs uppercase tracking-[0.2em] text-muted-foreground">
                            Situação
                          </p>
                          <p className="mt-2 text-base font-semibold">
                            {planoVisualizado.ativo ? "Vigente" : "Histórico"}
                          </p>
                        </div>
                      </div>

                      <div className="space-y-3">
                        <div>
                          <h4 className="font-semibold">Descrição</h4>
                          <p className="text-sm text-muted-foreground">
                            {planoVisualizado.descricao || "Sem descrição registrada."}
                          </p>
                        </div>

                        <div>
                          <h4 className="font-semibold">Observação</h4>
                          <p className="text-sm text-muted-foreground">
                            {planoVisualizado.observacao || "Sem observações adicionais."}
                          </p>
                        </div>
                      </div>

                      <div className="space-y-3">
                        <h4 className="font-semibold">Objetivos registrados</h4>
                        <div className="space-y-3">
                          {planoVisualizado.objetivos.map((objetivo, index) => (
                            <div key={objetivo.id ?? `${objetivo.titulo}-${index}`} className="rounded-lg border p-4">
                              <div className="flex flex-wrap items-center gap-2">
                                <Badge variant="outline"># {index + 1}</Badge>
                                <Badge variant="secondary">
                                  {prioridadeLabels[objetivo.prioridade]}
                                </Badge>
                                <Badge variant="secondary">{statusLabels[objetivo.status]}</Badge>
                              </div>
                              <p className="mt-3 font-semibold">{objetivo.titulo}</p>
                              <p className="mt-2 text-sm text-muted-foreground">
                                {objetivo.descricao || "Sem descrição."}
                              </p>
                              <div className="mt-3 grid gap-2 text-sm text-muted-foreground md:grid-cols-2">
                                <span>Data alvo: {formatarData(objetivo.dataAlvo)}</span>
                                <span>Valor alvo: {formatarMoeda(objetivo.valorAlvo)}</span>
                                <span>Valor atual: {formatarMoeda(objetivo.valorAtual)}</span>
                                <span>Observação: {objetivo.observacao || "—"}</span>
                              </div>
                            </div>
                          ))}
                        </div>
                      </div>

                      {planoVisualizado && planoVigente && planoVisualizado.id !== planoVigente.id ? (
                        <div className="rounded-lg border border-dashed px-4 py-3">
                          <p className="text-sm text-muted-foreground">
                            Esta é uma versão histórica. Para editar, mantenha a versão vigente em
                            edição e salve uma nova versão a partir dela.
                          </p>
                        </div>
                      ) : null}

                      {planoVisualizado ? (
                        <div className="flex flex-wrap gap-2">
                          <Button type="button" variant="secondary" onClick={usarPlanoVisualizadoComoBase}>
                            <RotateCcw className="mr-2 h-4 w-4" />
                            Usar como base
                          </Button>
                        </div>
                      ) : null}
                    </div>
                  ) : (
                    <div className="rounded-lg border border-dashed px-4 py-10 text-center text-sm text-muted-foreground">
                      Selecione uma versão no histórico para visualizar os detalhes aqui.
                    </div>
                  )}
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
