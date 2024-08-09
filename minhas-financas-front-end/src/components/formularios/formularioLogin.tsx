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

const FormSchema = z.object({
  email: z.string().email("Digite seu email de login."),
  senha: z.string().nonempty("Digite seu senha."),
});

export function FormularioLogin() {
  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      email: "",
      senha: "",
    },
  });

  function onSubmit(data: z.infer<typeof FormSchema>) {
    alert(JSON.stringify(data));
  }

  return (
    <Card className="w-[500px]">
      <CardHeader>
        <CardTitle>Create project</CardTitle>
        <CardDescription>Insira seu e-mail e senha para efetuar o login.</CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="w-2/3 space-y-6"
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
            <Button type="submit">Entrar</Button>
          </form>
        </Form>
      </CardContent>

      <CardFooter className="flex justify-between">
        <div className="flex items-center">
          <Separator className="flex-grow" />
          <span className="mx-2 text-gray-500 text-sm whitespace-nowrap">OU CONTINUE COM</span>
          <Separator className="flex-grow" />
        </div>
      </CardFooter>
    </Card>
  );
}
