"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Layers3, Pencil, Plus, Trash2 } from "lucide-react";
import { ApiError } from "@/types/api";
import {
  CadastrarCategoriaPayload,
  CategoriaResumo,
  EditarCategoriaPayload,
  TipoCategoria,
} from "@/types/categories";
import { useAuth } from "@/providers/auth-provider";
import {
  buscarCategorias,
  cadastrarCategoria,
  cadastrarSubCategoria,
  deletarCategoria,
  deletarSubCategoria,
  editarCategoria,
  editarSubCategoria,
} from "@/services/api/categories";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";

type TabValue = "1" | "0" | "2";

const TAB_ITEMS: Array<{ value: TabValue; tipo: TipoCategoria; label: string; singular: string }> = [
  { value: "1", tipo: 1, label: "Receitas", singular: "receita" },
  { value: "0", tipo: 0, label: "Despesas", singular: "despesa" },
  { value: "2", tipo: 2, label: "Investimentos", singular: "investimento" },
];

function getIconePorTipo(tipo: TipoCategoria) {
  switch (tipo) {
    case 1:
      return "Receita.png";
    case 0:
      return "Despesa.png";
    case 2:
      return "Investimento.png";
    default:
      return "Categoria.png";
  }
}

function getTipoBadgeLabel(tipo: TipoCategoria) {
  switch (tipo) {
    case 1:
      return "Receita";
    case 0:
      return "Despesa";
    case 2:
      return "Investimento";
    default:
      return "Transferencia";
  }
}

function ordenarCategorias(lista: CategoriaResumo[]) {
  return [...lista].sort((a, b) => a.nomeCategoria.localeCompare(b.nomeCategoria));
}

export function CategoriasManager() {
  const { session } = useAuth();
  const [categorias, setCategorias] = useState<CategoriaResumo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [activeTab, setActiveTab] = useState<TabValue>("1");

  const [categoriaDialogOpen, setCategoriaDialogOpen] = useState(false);
  const [categoriaDialogMode, setCategoriaDialogMode] = useState<"create" | "edit">("create");
  const [categoriaSelecionada, setCategoriaSelecionada] = useState<CategoriaResumo | null>(null);
  const [categoriaNome, setCategoriaNome] = useState("");
  const [categoriaTipo, setCategoriaTipo] = useState<TipoCategoria>(1);

  const [subCategoriaDialogOpen, setSubCategoriaDialogOpen] = useState(false);
  const [subCategoriaDialogMode, setSubCategoriaDialogMode] = useState<"create" | "edit">("create");
  const [subCategoriaPai, setSubCategoriaPai] = useState<CategoriaResumo | null>(null);
  const [subCategoriaSelecionadaId, setSubCategoriaSelecionadaId] = useState<string | null>(null);
  const [subCategoriaNome, setSubCategoriaNome] = useState("");

  const [deleteTarget, setDeleteTarget] = useState<
    | { kind: "categoria"; categoriaId: string; nome: string }
    | { kind: "subcategoria"; categoriaId: string; subCategoriaId: string; nome: string }
    | null
  >(null);

  const carregarCategorias = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setIsLoading(true);
      setErrorMessage("");

      const response = await buscarCategorias(session.usuario.id, session.token);
      setCategorias(ordenarCategorias(response.dados ?? []));
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel carregar as categorias.");
      }
    } finally {
      setIsLoading(false);
    }
  }, [session?.token, session?.usuario.id]);

  useEffect(() => {
    carregarCategorias();
  }, [carregarCategorias]);

  const categoriasPorTipo = useMemo(() => {
    return {
      receita: ordenarCategorias(categorias.filter((categoria) => categoria.tipo === 1)),
      despesa: ordenarCategorias(categorias.filter((categoria) => categoria.tipo === 0)),
      investimento: ordenarCategorias(categorias.filter((categoria) => categoria.tipo === 2)),
    };
  }, [categorias]);

  const totalSubCategorias = useMemo(() => {
    return categorias.reduce((total, categoria) => total + (categoria.subCategorias?.length ?? 0), 0);
  }, [categorias]);

  function abrirNovaCategoria(tipo: TipoCategoria) {
    setCategoriaDialogMode("create");
    setCategoriaSelecionada(null);
    setCategoriaNome("");
    setCategoriaTipo(tipo);
    setCategoriaDialogOpen(true);
  }

  function abrirEdicaoCategoria(categoria: CategoriaResumo) {
    setCategoriaDialogMode("edit");
    setCategoriaSelecionada(categoria);
    setCategoriaNome(categoria.nomeCategoria);
    setCategoriaTipo(categoria.tipo);
    setCategoriaDialogOpen(true);
  }

  function abrirNovaSubCategoria(categoria: CategoriaResumo) {
    setSubCategoriaDialogMode("create");
    setSubCategoriaPai(categoria);
    setSubCategoriaSelecionadaId(null);
    setSubCategoriaNome("");
    setSubCategoriaDialogOpen(true);
  }

  function abrirEdicaoSubCategoria(categoria: CategoriaResumo, subCategoriaId: string, nome: string) {
    setSubCategoriaDialogMode("edit");
    setSubCategoriaPai(categoria);
    setSubCategoriaSelecionadaId(subCategoriaId);
    setSubCategoriaNome(nome);
    setSubCategoriaDialogOpen(true);
  }

  async function salvarCategoria() {
    if (!session?.usuario.id || !session.token) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      return;
    }

    const nomeCategoria = categoriaNome.trim();

    if (nomeCategoria.length < 2) {
      setErrorMessage("Informe um nome de categoria com pelo menos 2 caracteres.");
      return;
    }

    const payload: CadastrarCategoriaPayload | EditarCategoriaPayload = {
      nomeCategoria,
      icone: getIconePorTipo(categoriaTipo),
      tipo: categoriaTipo,
      usuarioId: session.usuario.id,
    };

    try {
      setIsSubmitting(true);
      setErrorMessage("");
      setSuccessMessage("");

      if (categoriaDialogMode === "create") {
        await cadastrarCategoria(payload, session.token);
        setSuccessMessage("Categoria criada com sucesso.");
      } else if (categoriaSelecionada) {
        await editarCategoria(session.usuario.id, categoriaSelecionada.id, payload, session.token);
        setSuccessMessage("Categoria atualizada com sucesso.");
      }

      setActiveTab(String(categoriaTipo) as TabValue);
      setCategoriaDialogOpen(false);
      await carregarCategorias();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel salvar a categoria.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  async function salvarSubCategoria() {
    if (!session?.usuario.id || !session.token || !subCategoriaPai) {
      setErrorMessage("Sessao invalida. Faca login novamente.");
      return;
    }

    const nomeSubCategoria = subCategoriaNome.trim();

    if (nomeSubCategoria.length < 2) {
      setErrorMessage("Informe um nome de subcategoria com pelo menos 2 caracteres.");
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");
      setSuccessMessage("");

      if (subCategoriaDialogMode === "create") {
        await cadastrarSubCategoria(
          session.usuario.id,
          subCategoriaPai.id,
          {
            nomeSubCategoria,
            categoriaId: subCategoriaPai.id,
          },
          session.token
        );
        setSuccessMessage("Subcategoria criada com sucesso.");
      } else if (subCategoriaSelecionadaId) {
        await editarSubCategoria(
          session.usuario.id,
          subCategoriaPai.id,
          subCategoriaSelecionadaId,
          {
            id: subCategoriaSelecionadaId,
            nomeSubCategoria,
            categoriaId: subCategoriaPai.id,
          },
          session.token
        );
        setSuccessMessage("Subcategoria atualizada com sucesso.");
      }

      setSubCategoriaDialogOpen(false);
      await carregarCategorias();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel salvar a subcategoria.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  async function confirmarExclusao() {
    if (!session?.usuario.id || !session.token || !deleteTarget) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage("");
      setSuccessMessage("");

      if (deleteTarget.kind === "categoria") {
        await deletarCategoria(session.usuario.id, deleteTarget.categoriaId, session.token);
        setSuccessMessage("Categoria excluida com sucesso.");
      } else {
        await deletarSubCategoria(
          session.usuario.id,
          deleteTarget.categoriaId,
          deleteTarget.subCategoriaId,
          session.token
        );
        setSuccessMessage("Subcategoria excluida com sucesso.");
      }

      setDeleteTarget(null);
      await carregarCategorias();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Nao foi possivel concluir a exclusao.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="flex-1 px-6 py-8 md:px-8">
      <div className="mx-auto max-w-6xl space-y-6">
        <Card className="border-0 shadow-none">
          <CardHeader className="px-0 pt-0">
            <div className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
              <div>
                <CardTitle className="text-3xl">Categorias e subcategorias</CardTitle>
                <CardDescription className="mt-2 max-w-2xl text-base">
                  Organize os tipos de lancamento do usuario. As categorias criadas aqui
                  alimentam o modal de novo lancamento e a base inicial do produto.
                </CardDescription>
              </div>
              <Button onClick={() => abrirNovaCategoria(Number(activeTab) as TipoCategoria)}>
                <Plus className="mr-2 h-4 w-4" />
                Nova categoria
              </Button>
            </div>
          </CardHeader>
        </Card>

        <div className="grid gap-4 md:grid-cols-3">
          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Total de categorias</CardDescription>
              <CardTitle className="text-3xl">{categorias.length}</CardTitle>
            </CardHeader>
          </Card>
          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Total de subcategorias</CardDescription>
              <CardTitle className="text-3xl">{totalSubCategorias}</CardTitle>
            </CardHeader>
          </Card>
          <Card>
            <CardHeader className="pb-3">
              <CardDescription>Categorias padrao</CardDescription>
              <CardTitle className="text-3xl">JSON + CRUD</CardTitle>
            </CardHeader>
          </Card>
        </div>

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

        <Tabs value={activeTab} onValueChange={(value) => setActiveTab(value as TabValue)} className="space-y-4">
          <TabsList>
            {TAB_ITEMS.map((item) => (
              <TabsTrigger key={item.value} value={item.value}>
                {item.label}
              </TabsTrigger>
            ))}
          </TabsList>

          {TAB_ITEMS.map((item) => {
            const lista =
              item.tipo === 1
                ? categoriasPorTipo.receita
                : item.tipo === 0
                  ? categoriasPorTipo.despesa
                  : categoriasPorTipo.investimento;

            return (
              <TabsContent key={item.value} value={item.value}>
                <Card>
                  <CardHeader>
                    <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                      <div>
                        <CardTitle>{item.label}</CardTitle>
                        <CardDescription>
                          Gerencie as categorias de {item.singular} e as subcategorias associadas.
                        </CardDescription>
                      </div>
                      <Button variant="outline" onClick={() => abrirNovaCategoria(item.tipo)}>
                        <Plus className="mr-2 h-4 w-4" />
                        Nova categoria
                      </Button>
                    </div>
                  </CardHeader>
                  <CardContent>
                    {isLoading ? (
                      <div className="rounded-lg border border-dashed px-6 py-10 text-center text-sm text-muted-foreground">
                        Carregando categorias...
                      </div>
                    ) : lista.length === 0 ? (
                      <div className="rounded-lg border border-dashed px-6 py-10 text-center">
                        <Layers3 className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
                        <p className="text-sm text-muted-foreground">
                          Nenhuma categoria encontrada neste grupo ainda.
                        </p>
                      </div>
                    ) : (
                      <Accordion type="single" collapsible className="w-full">
                        {lista.map((categoria) => (
                          <AccordionItem key={categoria.id} value={categoria.id}>
                            <AccordionTrigger className="hover:no-underline">
                              <div className="flex w-full items-center justify-between gap-4 pr-4 text-left">
                                <div>
                                  <div className="font-medium">{categoria.nomeCategoria}</div>
                                  <div className="mt-1 flex flex-wrap items-center gap-2">
                                    <Badge variant="secondary">{getTipoBadgeLabel(categoria.tipo)}</Badge>
                                    <Badge variant="outline">
                                      {categoria.subCategorias?.length ?? 0} subcategorias
                                    </Badge>
                                  </div>
                                </div>
                              </div>
                            </AccordionTrigger>
                            <AccordionContent className="space-y-4">
                              <div className="flex flex-wrap gap-2">
                                <Button variant="outline" size="sm" onClick={() => abrirEdicaoCategoria(categoria)}>
                                  <Pencil className="mr-2 h-4 w-4" />
                                  Editar categoria
                                </Button>
                                <Button variant="outline" size="sm" onClick={() => abrirNovaSubCategoria(categoria)}>
                                  <Plus className="mr-2 h-4 w-4" />
                                  Nova subcategoria
                                </Button>
                                <Button
                                  variant="outline"
                                  size="sm"
                                  className="text-destructive hover:text-destructive"
                                  onClick={() =>
                                    setDeleteTarget({
                                      kind: "categoria",
                                      categoriaId: categoria.id,
                                      nome: categoria.nomeCategoria,
                                    })
                                  }
                                >
                                  <Trash2 className="mr-2 h-4 w-4" />
                                  Excluir categoria
                                </Button>
                              </div>

                              {categoria.subCategorias?.length ? (
                                <div className="grid gap-3 md:grid-cols-2">
                                  {categoria.subCategorias.map((subCategoria) => (
                                    <div
                                      key={subCategoria.id}
                                      className="flex items-center justify-between rounded-lg border px-4 py-3"
                                    >
                                      <div>
                                        <p className="font-medium">{subCategoria.nomeSubCategoria}</p>
                                        <p className="text-sm text-muted-foreground">
                                          Vinculada a {categoria.nomeCategoria}
                                        </p>
                                      </div>
                                      <div className="flex gap-2">
                                        <Button
                                          variant="ghost"
                                          size="icon"
                                          onClick={() =>
                                            abrirEdicaoSubCategoria(
                                              categoria,
                                              subCategoria.id,
                                              subCategoria.nomeSubCategoria
                                            )
                                          }
                                        >
                                          <Pencil className="h-4 w-4" />
                                        </Button>
                                        <Button
                                          variant="ghost"
                                          size="icon"
                                          className="text-destructive hover:text-destructive"
                                          onClick={() =>
                                            setDeleteTarget({
                                              kind: "subcategoria",
                                              categoriaId: categoria.id,
                                              subCategoriaId: subCategoria.id,
                                              nome: subCategoria.nomeSubCategoria,
                                            })
                                          }
                                        >
                                          <Trash2 className="h-4 w-4" />
                                        </Button>
                                      </div>
                                    </div>
                                  ))}
                                </div>
                              ) : (
                                <div className="rounded-lg border border-dashed px-4 py-6 text-sm text-muted-foreground">
                                  Nenhuma subcategoria cadastrada para esta categoria ainda.
                                </div>
                              )}
                            </AccordionContent>
                          </AccordionItem>
                        ))}
                      </Accordion>
                    )}
                  </CardContent>
                </Card>
              </TabsContent>
            );
          })}
        </Tabs>
      </div>

      <Dialog open={categoriaDialogOpen} onOpenChange={setCategoriaDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {categoriaDialogMode === "create" ? "Nova categoria" : "Editar categoria"}
            </DialogTitle>
            <DialogDescription>
              O icone continua sendo definido automaticamente pelo tipo da categoria.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="categoria-nome">Nome</Label>
              <Input
                id="categoria-nome"
                value={categoriaNome}
                onChange={(event) => setCategoriaNome(event.target.value)}
                placeholder="Ex: Alimentacao, Salario, Reserva"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="categoria-tipo">Tipo</Label>
              <select
                id="categoria-tipo"
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                value={String(categoriaTipo)}
                onChange={(event) => setCategoriaTipo(Number(event.target.value) as TipoCategoria)}
              >
                {TAB_ITEMS.map((item) => (
                  <option key={item.value} value={item.value}>
                    {item.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setCategoriaDialogOpen(false)}>
              Cancelar
            </Button>
            <Button type="button" onClick={salvarCategoria} disabled={isSubmitting}>
              {isSubmitting ? "Salvando..." : "Salvar categoria"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={subCategoriaDialogOpen} onOpenChange={setSubCategoriaDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {subCategoriaDialogMode === "create" ? "Nova subcategoria" : "Editar subcategoria"}
            </DialogTitle>
            <DialogDescription>
              {subCategoriaPai
                ? `Subcategoria vinculada a ${subCategoriaPai.nomeCategoria}.`
                : "Selecione a categoria antes de continuar."}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="space-y-2">
              <Label>Categoria</Label>
              <Input value={subCategoriaPai?.nomeCategoria ?? ""} disabled />
            </div>
            <div className="space-y-2">
              <Label htmlFor="subcategoria-nome">Nome</Label>
              <Input
                id="subcategoria-nome"
                value={subCategoriaNome}
                onChange={(event) => setSubCategoriaNome(event.target.value)}
                placeholder="Ex: Mercado, Farmacia, Combustivel"
              />
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setSubCategoriaDialogOpen(false)}>
              Cancelar
            </Button>
            <Button type="button" onClick={salvarSubCategoria} disabled={isSubmitting}>
              {isSubmitting ? "Salvando..." : "Salvar subcategoria"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar exclusao</AlertDialogTitle>
            <AlertDialogDescription>
              {deleteTarget
                ? `Tem certeza que deseja excluir ${deleteTarget.nome}? Essa acao nao pode ser desfeita.`
                : ""}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmarExclusao} disabled={isSubmitting}>
              {isSubmitting ? "Excluindo..." : "Excluir"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
