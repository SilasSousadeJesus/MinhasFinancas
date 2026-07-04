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
  CardDescription,
  CardContent,
  CardFooter,
} from "../ui/card";
import { Separator } from "../ui/separator";
import Image from "next/image";
import Link from "next/link";
import { useAuth } from "@/providers/auth-provider";
import { ApiError } from "@/types/api";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

const FormSchema = z.object({
  email: z.string().email("Digite seu email de login."),
  senha: z.string().nonempty("Digite seu senha."),
});

export function FormularioLogin({ cardWidth = "w-[500px]" }) {
  const { loginWithCredentials, isAuthenticated } = useAuth();
  const [errorMessage, setErrorMessage] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const router = useRouter();
  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      email: "",
      senha: "",
    },
  });

  function getNextPath() {
    if (typeof window === "undefined") {
      return "/dashboard";
    }

    const params = new URLSearchParams(window.location.search);
    return params.get("next") || "/dashboard";
  }

  useEffect(() => {
    if (!isAuthenticated) {
      return;
    }

    const nextPath = getNextPath();
    router.replace(nextPath);
  }, [isAuthenticated, router]);

  async function onSubmit(data: z.infer<typeof FormSchema>) {
    try {
      setIsSubmitting(true);
      setErrorMessage("");
      await loginWithCredentials(data);
      const nextPath = getNextPath();
      router.push(nextPath);
      router.refresh();
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Não foi possível fazer login agora. Tente novamente.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className={`${cardWidth} flex flex-col justify-center`}>
      <CardHeader className="flex justify-center">
        <Image
          src="/assets/img/logo/logo02.webp"
          alt=""
          width={100}
          height={100}
          className="mx-auto"
        />
        <CardDescription className="text-center">
          Cuidando bem das suas finanças.
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
            <Button type="submit" className="w-full" disabled={isSubmitting}>
              {isSubmitting ? "Entrando..." : "Entrar"}
            </Button>
            {errorMessage ? (
              <p className="text-sm font-medium text-destructive">{errorMessage}</p>
            ) : null}
          </form>
        </Form>
        <div className="flex flex-col justify-center">
          <span className="text-center text-gray-500 text-sm whitespace-nowrap mt-2">
            <Link href="/cadastro" passHref>
              <Button variant="link">Esqueceu sua senha?</Button>
            </Link>
          </span>

          <div className="flex justify-between mt-5">
            <div className="flex items-center">
              <Separator className="flex-grow" />
              <span className="mx-2 text-gray-500 text-sm whitespace-nowrap">
                OU CONTINUE COM
              </span>
              <Separator className="flex-grow" />
            </div>
          </div>

          <div className="flex justify-evenly mt-5 mb-5">
            <Button variant="outline" className="w-40">
              <svg
                className="w-6 h-6 text-gray-800 dark:text-white"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="24"
                fill="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  fillRule="evenodd"
                  d="M12.006 2a9.847 9.847 0 0 0-6.484 2.44 10.32 10.32 0 0 0-3.393 6.17 10.48 10.48 0 0 0 1.317 6.955 10.045 10.045 0 0 0 5.4 4.418c.504.095.683-.223.683-.494 0-.245-.01-1.052-.014-1.908-2.78.62-3.366-1.21-3.366-1.21a2.711 2.711 0 0 0-1.11-1.5c-.907-.637.07-.621.07-.621.317.044.62.163.885.346.266.183.487.426.647.71.135.253.318.476.538.655a2.079 2.079 0 0 0 2.37.196c.045-.52.27-1.006.635-1.37-2.219-.259-4.554-1.138-4.554-5.07a4.022 4.022 0 0 1 1.031-2.75 3.77 3.77 0 0 1 .096-2.713s.839-.275 2.749 1.05a9.26 9.26 0 0 1 5.004 0c1.906-1.325 2.74-1.05 2.74-1.05.37.858.406 1.828.101 2.713a4.017 4.017 0 0 1 1.029 2.75c0 3.939-2.339 4.805-4.564 5.058a2.471 2.471 0 0 1 .679 1.897c0 1.372-.012 2.477-.012 2.814 0 .272.18.592.687.492a10.05 10.05 0 0 0 5.388-4.421 10.473 10.473 0 0 0 1.313-6.948 10.32 10.32 0 0 0-3.39-6.165A9.847 9.847 0 0 0 12.007 2Z"
                  clipRule="evenodd"
                />
              </svg>
            </Button>
            <Button variant="outline" className="w-40">
              <svg
                className="w-6 h-6 text-gray-800 dark:text-white"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="24"
                fill="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  fillRule="evenodd"
                  d="M12.037 21.998a10.313 10.313 0 0 1-7.168-3.049 9.888 9.888 0 0 1-2.868-7.118 9.947 9.947 0 0 1 3.064-6.949A10.37 10.37 0 0 1 12.212 2h.176a9.935 9.935 0 0 1 6.614 2.564L16.457 6.88a6.187 6.187 0 0 0-4.131-1.566 6.9 6.9 0 0 0-4.794 1.913 6.618 6.618 0 0 0-2.045 4.657 6.608 6.608 0 0 0 1.882 4.723 6.891 6.891 0 0 0 4.725 2.07h.143c1.41.072 2.8-.354 3.917-1.2a5.77 5.77 0 0 0 2.172-3.41l.043-.117H12.22v-3.41h9.678c.075.617.109 1.238.1 1.859-.099 5.741-4.017 9.6-9.746 9.6l-.215-.002Z"
                  clipRule="evenodd"
                />
              </svg>
            </Button>
          </div>

          <span className="text-center mx-2 text-gray-500 text-sm whitespace-nowrap">
            Ainda não tem conta?{" "}
            <Link href="/cadastro" passHref>
              <Button variant="link">Click aqui e Cadastre-se</Button>
            </Link>
          </span>
        </div>
      </CardContent>
      <CardFooter className="flex flex-row"></CardFooter>
    </div>
  );
}
