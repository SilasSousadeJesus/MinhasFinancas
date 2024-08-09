"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "../ui/card";
import { BotaoTrocaTema } from "../BotaoTrocaTema/botaoTrocaTema";
import { Separator } from "../ui/separator";
import Image from "next/image";
import Link from "next/link";

const FormSchema = z
  .object({
    nome: z.string().min(2, "O nome deve ter pelo menos 2 caracteres"),
    email: z.string().email("E-mail inválido"),
    senha: z.string().min(6, "A senha deve ter pelo menos 6 caracteres"),
    confirmacaoSenha: z.string().min(6, "Confirmação de senha é obrigatória"),
  })
  .refine((data) => data.senha === data.confirmacaoSenha, {
    message: "As senhas não coincidem",
    path: ["confirmacaoSenha"], // Define o campo onde o erro será mostrado
  });

export function FormularioCadastro({ cardWidth = "w-[500px]" }) {
  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      nome: "",
      email: "",
      senha: "",
      confirmacaoSenha: "",
    },
  });

  function onSubmit(data: z.infer<typeof FormSchema>) {
    alert(JSON.stringify(data));
  }

  return (
    <Card className={`${cardWidth} flex flex-col justify-center `}>
      <CardHeader className="flex justify-center">
        <div className="flex justify-end">
          <Link href="/" passHref>
            <Button variant="link" className="">
              Login
            </Button>
          </Link>
        </div>
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
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
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
                  <FormLabel>Confirmação da Senha</FormLabel>
                  <FormControl>
                    <Input placeholder="*********" {...field} type="password" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <Button type="submit" className="w-full">
              Cadastrar
            </Button>
          </form>
        </Form>
      </CardContent>
      <CardFooter className="flex flex-row"></CardFooter>
    </Card>
  );
}
