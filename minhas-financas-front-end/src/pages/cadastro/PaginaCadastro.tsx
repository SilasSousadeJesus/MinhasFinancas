import { FormularioCadastro } from "@/components/formularios/formularioCadastro";
import Image from "next/image";
export default function PaginaCadastro() {
  return (
    <div className="flex h-screen">
      <div className="w-1/2 h-full relative">
        <Image
          src="/assets/img/background/fundoCinza.jpg"
          alt=""
          layout="fill"
          objectFit="cover"
          className="w-full h-full"
        />
      </div>
      <div className="w-1/2 h-full flex justify-center items-center">
        <FormularioCadastro cardWidth="w-[500px]" />
      </div>
    </div>
  );
}
