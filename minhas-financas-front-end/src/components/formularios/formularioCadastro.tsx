"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/button";
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
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "../ui/card";
import Link from "next/link";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { useRouter } from "next/navigation";
import { useState } from "react";

const FormSchema = z
  .object({
    nome: z.string().min(2, "O nome deve ter pelo menos 2 caracteres."),
    email: z.string().email("E-mail inválido."),
    senha: z.string().min(6, "A senha deve ter pelo menos 6 caracteres."),
    confirmacaoSenha: z
      .string()
      .min(6, "A confirmação de senha é obrigatória."),
  })
  .refine((data) => data.senha === data.confirmacaoSenha, {
    message: "As senhas não coincidem.",
    path: ["confirmacaoSenha"],
  });

export function FormularioCadastro({ cardWidth = "w-[500px]" }) {
  const { registerUser } = useAuth();
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const router = useRouter();
  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      nome: "",
      email: "",
      senha: "",
      confirmacaoSenha: "",
    },
  });

  async function onSubmit(data: z.infer<typeof FormSchema>) {
    try {
      setIsSubmitting(true);
      setErrorMessage("");
      setSuccessMessage("");
      await registerUser(data);
      setSuccessMessage(
        "Cadastro realizado com sucesso. Redirecionando para o login..."
      );
      form.reset();
      window.setTimeout(() => {
        router.push("/");
      }, 1200);
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage(
          "Não foi possível concluir o cadastro agora. Tente novamente."
        );
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className={`${cardWidth} flex flex-col justify-center border-white`}>
      <CardHeader className="flex justify-center">
        <CardTitle className="text-center">Cadastre-se</CardTitle>
        <CardDescription className="text-center">
          Cuide bem das suas finanças.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="w-full space-y-6"
          >
            <FormField
              control={form.control}
              name="nome"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nome</FormLabel>
                  <FormControl>
                    <Input placeholder="Seu nome" {...field} type="text" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>E-mail</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="nome@exemplo.com"
                      {...field}
                      type="email"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="senha"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Senha</FormLabel>
                  <FormControl>
                    <Input placeholder="*********" {...field} type="password" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="confirmacaoSenha"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Confirmação da senha</FormLabel>
                  <FormControl>
                    <Input placeholder="*********" {...field} type="password" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <Button type="submit" className="w-full" disabled={isSubmitting}>
              {isSubmitting ? "Cadastrando..." : "Cadastrar"}
            </Button>
            {errorMessage ? (
              <p className="text-sm font-medium text-destructive">{errorMessage}</p>
            ) : null}
            {successMessage ? (
              <p className="text-sm font-medium text-green-600">
                {successMessage}
              </p>
            ) : null}
          </form>
        </Form>
        <div className="mt-5 flex flex-col justify-center">
          <span className="mx-2 whitespace-nowrap text-center text-sm text-gray-500">
            Já tem conta?{" "}
            <Link href="/" passHref>
              <Button variant="link">Clique aqui e faça login</Button>
            </Link>
          </span>
        </div>
      </CardContent>
      <CardFooter className="flex flex-row" />
    </div>
  );
}
