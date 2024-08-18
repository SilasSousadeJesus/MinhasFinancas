import { FormularioLogin } from "@/components/formularios/formularioLogin";
import Image from "next/image";

export default function Login() {
  return (
    <div className="flex h-screen">
      {/* A imagem será escondida em telas de tamanho md (768px) ou menores */}
      <div className="hidden lg:block w-1/2 h-full relative">
        <Image
          src="/assets/img/background/fundoCinza.jpg"
          alt=""
          layout="fill"
          objectFit="cover"
          className="w-full h-full"
        />
      </div>
      {/* O formulário ocupará toda a tela em dispositivos md ou menores */}
      <div className="w-full lg:w-1/2 h-full flex justify-center items-center">
        <FormularioLogin cardWidth="w-[500px]" />
      </div>
    </div>
  );
}
