"use client";

import {
  useState,
  useEffect,
  ClassAttributes,
  HTMLAttributes,
  SVGProps,
} from "react";
import { Button } from "@/components/ui/button";
import { Separator } from "../ui/separator";
import Link from "next/link";
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar";
import {
  ChartTooltipContent,
  ChartTooltip,
  ChartContainer,
} from "@/components/ui/chart";
import { Pie, PieChart, CartesianGrid, XAxis, Line, LineChart } from "recharts";
import { MenuNavegacao } from "../NavegacaoMenu/MenuNavegacao";
import { PowerIcon } from "../Icons/Icons";
import { BotaoTrocaTema } from "../BotaoTrocaTema/botaoTrocaTema";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "../ui/card";

export function Sidebar() {
  const [isSidebarExpanded, setIsSidebarExpanded] = useState(false);
  const [windowWidth, setWindowWidth] = useState(0);
  useEffect(() => {
    const handleResize = () => {
      setWindowWidth(window.innerWidth);
      setIsSidebarExpanded(window.innerWidth >= 1024);
    };
    window.addEventListener("resize", handleResize);
    handleResize();
    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);
  return (
    <div className="flex min-h-screen ">
      <aside
        className={`border-r transition-all duration-300 ${
          isSidebarExpanded ? "w-64" : "w-24"
        }`}
      >
        <div className="flex items-center justify-between h-16 border-b px-4">
          <div className="flex items-center">
            {/* // LOGO DA EMPRESA */}
            <LogInIcon className="w-8 h-8" />
            <span
              className={`ml-2 text-xl font-bold ${
                !isSidebarExpanded && "hidden"
              }`}
            >
              MinhasFinanças
            </span>
          </div>
          <Button
            variant="ghost"
            size="icon"
            onClick={() => setIsSidebarExpanded(!isSidebarExpanded)}
          >
            <SquareChevronLeftIcon className="w-5 h-5" />
            <span className="sr-only">Toggle sidebar</span>
          </Button>
        </div>
        <nav className="flex flex-col content-center items-center justify-center p-4 space-y-4">
          <MenuNavegacao />
        </nav>
        <BotaoTrocaTema />

        <Separator className="flex-grow mt-[330px]" />

        <div className="p-4 mt-auto flex items-center justify-between">
          <div className="flex items-center space-x-2">
            <Avatar>
              <AvatarImage src="/placeholder-user.jpg" alt="Silas Sousa" />
              <AvatarFallback>SS</AvatarFallback>
            </Avatar>
            <div>
              <p
                className={`text-sm font-medium ${
                  !isSidebarExpanded && "sr-only"
                }`}
              >
                Silas Sousa
              </p>
              <p
                className={`text-xs text-gray-600 truncate max-w-[ch-19] ${
                  !isSidebarExpanded && "sr-only"
                }`}
                style={{
                  maxWidth: "19ch",
                }}
              >
                silassousadejesus@gmail.com
              </p>
            </div>
          </div>
          <Button
            variant="ghost"
            size="icon"
            className={`rounded-full ${!isSidebarExpanded && "w-8 h-8"}`}
          >
            <PowerIcon
              className={`w-5 h-5 ${!isSidebarExpanded && "sr-only"}`}
            />
          </Button>
        </div>
      </aside>
      {/* <main className="flex-1 p-6 bg-gray-50">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold">Dashboard</h1>
            <p className="text-sm text-gray-600">
              Bem vindo de volta, Silas Sousa!
            </p>
          </div>
          <div className="flex space-x-2">
            <Button variant="outline">Gerenciar contas e cartões</Button>
            <Button variant="default">Novo Lançamento</Button>
          </div>
        </div>
        <div className="flex items-center mt-6 space-x-2">
          <Button variant="outline">Mês Passado</Button>
          <Button variant="outline">Este Mês</Button>
          <Button variant="outline">Este Ano</Button>
        </div>
        <div className="grid grid-cols-1 gap-4 mt-6 md:grid-cols-2 lg:grid-cols-5">
          <Card>
            <CardContent className="flex flex-col items-center">
              <div className="flex items-center justify-center w-16 h-16 bg-green-100 rounded-full">
                <span className="text-2xl font-bold text-green-500">0%</span>
              </div>
              <p className="mt-2 text-lg font-medium">Receitas</p>
              <p className="text-2xl font-bold">R$ 0,00</p>
              <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="flex flex-col items-center">
              <div className="flex items-center justify-center w-16 h-16 bg-yellow-100 rounded-full">
                <span className="text-2xl font-bold text-yellow-500">0%</span>
              </div>
              <p className="mt-2 text-lg font-medium">Investimentos</p>
              <p className="text-2xl font-bold">R$ 0,00</p>
              <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="flex flex-col items-center">
              <div className="flex items-center justify-center w-16 h-16 bg-red-100 rounded-full">
                <span className="text-2xl font-bold text-red-500">0%</span>
              </div>
              <p className="mt-2 text-lg font-medium">Despesas</p>
              <p className="text-2xl font-bold">R$ 0,00</p>
              <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="flex flex-col items-center">
              <div className="flex items-center justify-center w-16 h-16 bg-gray-100 rounded-full">
                <span className="text-2xl font-bold text-gray-500">0%</span>
              </div>
              <p className="mt-2 text-lg font-medium">Resultado</p>
              <p className="text-2xl font-bold">R$ 0,00</p>
              <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
            </CardContent>
          </Card>
        </div>
        <div className="grid grid-cols-1 gap-4 mt-6 md:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle>Percentual da receita</CardTitle>
              <CardDescription>
                Gráfico de percentual da receita
              </CardDescription>
            </CardHeader>
            <CardContent>
              <PiechartcustomChart className="w-full aspect-[4/3]" />
              <div className="flex justify-center mt-4 space-x-4">
                <div className="flex items-center space-x-2">
                  <div className="w-4 h-4 bg-red-500 rounded-full" />
                  <span className="text-sm">Fixa</span>
                </div>
                <div className="flex items-center space-x-2">
                  <div className="w-4 h-4 bg-yellow-500 rounded-full" />
                  <span className="text-sm">Variável</span>
                </div>
                <div className="flex items-center space-x-2">
                  <div className="w-4 h-4 bg-blue-500 rounded-full" />
                  <span className="text-sm">Parcelada</span>
                </div>
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle>Receitas e Despesas</CardTitle>
            </CardHeader>
            <CardContent>
              <LinechartChart className="w-full aspect-[4/3]" />
            </CardContent>
          </Card>
        </div>
      </main> */}
    </div>
  );
}


function LinechartChart(
  props: JSX.IntrinsicAttributes &
    ClassAttributes<HTMLDivElement> &
    HTMLAttributes<HTMLDivElement>
) {
  return (
    <div {...props}>
      <ChartContainer
        config={{
          desktop: {
            label: "Desktop",
            color: "hsl(var(--chart-1))",
          },
        }}
      >
        <LineChart
          accessibilityLayer
          data={[
            { month: "January", desktop: 186 },
            { month: "February", desktop: 305 },
            { month: "March", desktop: 237 },
            { month: "April", desktop: 73 },
            { month: "May", desktop: 209 },
            { month: "June", desktop: 214 },
          ]}
          margin={{
            left: 12,
            right: 12,
          }}
        >
          <CartesianGrid vertical={false} />
          <XAxis
            dataKey="month"
            tickLine={false}
            axisLine={false}
            tickMargin={8}
            tickFormatter={(value) => value.slice(0, 3)}
          />
          <ChartTooltip
            cursor={false}
            content={<ChartTooltipContent hideLabel />}
          />
          <Line
            dataKey="desktop"
            type="natural"
            stroke="var(--color-desktop)"
            strokeWidth={2}
            dot={false}
          />
        </LineChart>
      </ChartContainer>
    </div>
  );
}

function LogInIcon(props: JSX.IntrinsicAttributes & SVGProps<SVGSVGElement>) {
  return (
    <svg
      {...props}
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4" />
      <polyline points="10 17 15 12 10 7" />
      <line x1="15" x2="3" y1="12" y2="12" />
    </svg>
  );
}

function PiechartcustomChart(
  props: JSX.IntrinsicAttributes &
    ClassAttributes<HTMLDivElement> &
    HTMLAttributes<HTMLDivElement>
) {
  return (
    <div {...props}>
      <ChartContainer
        config={{
          visitors: {
            label: "Visitors",
          },
          chrome: {
            label: "Chrome",
            color: "hsl(var(--chart-1))",
          },
          safari: {
            label: "Safari",
            color: "hsl(var(--chart-2))",
          },
          firefox: {
            label: "Firefox",
            color: "hsl(var(--chart-3))",
          },
          edge: {
            label: "Edge",
            color: "hsl(var(--chart-4))",
          },
          other: {
            label: "Other",
            color: "hsl(var(--chart-5))",
          },
        }}
      >
        <PieChart>
          <ChartTooltip
            cursor={false}
            content={<ChartTooltipContent hideLabel />}
          />
          <Pie
            data={[
              { browser: "chrome", visitors: 275, fill: "var(--color-chrome)" },
              { browser: "safari", visitors: 200, fill: "var(--color-safari)" },
              {
                browser: "firefox",
                visitors: 187,
                fill: "var(--color-firefox)",
              },
              { browser: "edge", visitors: 173, fill: "var(--color-edge)" },
              { browser: "other", visitors: 90, fill: "var(--color-other)" },
            ]}
            dataKey="visitors"
            nameKey="browser"
          />
        </PieChart>
      </ChartContainer>
    </div>
  );
}

function SquareChevronLeftIcon(
  props: JSX.IntrinsicAttributes & SVGProps<SVGSVGElement>
) {
  return (
    <svg
      {...props}
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <rect width="18" height="18" x="3" y="3" rx="2" />
      <path d="m14 16-4-4 4-4" />
    </svg>
  );
}
