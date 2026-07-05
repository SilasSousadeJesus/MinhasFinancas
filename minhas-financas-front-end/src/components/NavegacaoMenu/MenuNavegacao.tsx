"use client";

import * as React from "react";
import Link from "next/link";

import { cn } from "@/lib/utils";
import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";
import {
  CreditCardIcon,
  CurrencyIcon,
  DownloadIcon,
  FilesIcon,
  GoalIcon,
  LayoutDashboardIcon,
  PocketIcon,
  ProjectionIcon,
  SettingsIcon,
} from "../Icons/Icons";

interface MenuNavegacaoProps {
  isSidebarExpanded: boolean;
}

const components: { title: string; href: string; description: string }[] = [
  {
    title: "Relatório patrimonial",
    href: "relatorios/relatorio-patrimonial",
    description: "Acompanhe a evolução do seu patrimônio ao longo do tempo.",
  },
  {
    title: "Relatório anual",
    href: "relatorios/relatorio-anual",
    description: "Visualize a evolução financeira consolidada por ano.",
  },
  {
    title: "Relatório por categoria",
    href: "relatorios/relatorio-por-categoria",
    description: "Entenda como suas finanças se distribuem por categoria.",
  },
  {
    title: "Relatório de saldos e investimentos",
    href: "relatorios/relatorio-saldo-investimento-despesa",
    description: "Consulte saldos, investimentos e despesas em um só lugar.",
  },
];

export function MenuNavegacao({
  isSidebarExpanded = true,
}: MenuNavegacaoProps) {
  return (
    <NavigationMenu className="">
      <NavigationMenuList className="flex flex-col">
        <NavigationMenuItem className="w-full">
          <Link href="/dashboard" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <LayoutDashboardIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Dashboard
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/contas-e-cartoes" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <CreditCardIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Contas e cartões
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/lancamentos" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <DownloadIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Lançamentos
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/fluxo-de-caixa-simples" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <CurrencyIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Fluxo de caixa
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/orcamento" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <PocketIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Orçamento
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/projecao" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <ProjectionIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Projeções
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/metas" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <GoalIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Metas
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <NavigationMenuTrigger className="justify-center space-x-2 text-lg">
            <FilesIcon className="h-5 w-5" />
            <span
              className={`space-x-2 text-lg ${
                !isSidebarExpanded && "hidden"
              }`}
            >
              Relatórios
            </span>
          </NavigationMenuTrigger>
          <NavigationMenuContent>
            <ul className="grid w-[400px] gap-3 p-4 md:w-[500px] md:grid-cols-2 lg:w-[600px]">
              {components.map((component) => (
                <ListItem
                  key={component.title}
                  title={component.title}
                  href={component.href}
                >
                  {component.description}
                </ListItem>
              ))}
            </ul>
          </NavigationMenuContent>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/configuracoes" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                "justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <SettingsIcon className="h-5 w-5" />
              <span
                className={`space-x-2 text-lg ${
                  !isSidebarExpanded && "hidden"
                }`}
              >
                Configurações
              </span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>
      </NavigationMenuList>
    </NavigationMenu>
  );
}

const ListItem = React.forwardRef<
  React.ElementRef<"a">,
  React.ComponentPropsWithoutRef<"a">
>(({ className, title, children, ...props }, ref) => {
  return (
    <li>
      <NavigationMenuLink asChild>
        <a
          ref={ref}
          className={cn(
            "block select-none space-y-1 rounded-md p-3 leading-none no-underline outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground",
            className
          )}
          {...props}
        >
          <div className="text-sm font-medium leading-none">{title}</div>
          <p className="line-clamp-2 text-sm leading-snug text-muted-foreground">
            {children}
          </p>
        </a>
      </NavigationMenuLink>
    </li>
  );
});
ListItem.displayName = "ListItem";
