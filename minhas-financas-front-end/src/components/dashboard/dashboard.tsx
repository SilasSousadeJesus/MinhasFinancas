import { LinechartChart, PiechartcustomChart } from "../Icons/Icons";
import { Button } from "../ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "../ui/card";

export function PainelDashboard() {
  return (
    <main className="flex-1 p-6 bg-gray-50 dark:bg-[#020817]">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Dashboard</h1>
          <p className="text-sm text-gray-500">
            Bem vindo de volta, Silas Sousa!
          </p>
        </div>
        <div className="flex space-x-2">
          <Button variant="outline">Gerenciar contas e cartões</Button>
          <Button variant="default">Novo Lançamento</Button>
        </div>
      </div>
      <div className="flex items-center mt-6 space-x-2">
        <Button variant="outline">Este Ano</Button>
        <Button variant="outline">Este Mês</Button>
        <Button variant="outline">Mês Passado</Button>
      </div>
      <div className="grid grid-cols-1 gap-4 mt-6 md:grid-cols-2 lg:grid-cols-4 justify-center">
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="flex items-center justify-center w-16 h-16 bg-green-100 rounded-full mt-4">
              <span className="text-2xl font-bold text-green-500">0%</span>
            </div>
            <p className="mt-2 text-lg font-medium">Receitas</p>
            <p className="text-2xl font-bold">R$ 0,00</p>
            <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="flex items-center justify-center w-16 h-16 bg-yellow-100 rounded-full mt-4">
              <span className="text-2xl font-bold text-yellow-500">0%</span>
            </div>
            <p className="mt-2 text-lg font-medium">Investimentos</p>
            <p className="text-2xl font-bold">R$ 0,00</p>
            <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="flex items-center justify-center w-16 h-16 bg-red-100 rounded-full mt-4">
              <span className="text-2xl font-bold text-red-500">0%</span>
            </div>
            <p className="mt-2 text-lg font-medium">Despesas</p>
            <p className="text-2xl font-bold">R$ 0,00</p>
            <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="flex items-center justify-center w-16 h-16 bg-gray-100 rounded-full mt-4">
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
            <CardDescription>Gráfico de percentual da receita</CardDescription>
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
    </main>
  );
}
