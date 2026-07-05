using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.CrossCutting.Util.Dicionary
{
    public class DicionarybemPatrimonial
    {
        private Dictionary<EnumBemPatrimonial, string> bemPatrimonialDict;

        public DicionarybemPatrimonial()
        {
            bemPatrimonialDict = new Dictionary<EnumBemPatrimonial, string>
            {
            { EnumBemPatrimonial.Imovel, "Imovel" },
            { EnumBemPatrimonial.Automovel, "Automovel" },
            { EnumBemPatrimonial.Investimento, "Investimento" },
            { EnumBemPatrimonial.DinheiroEmConta, "Dinheiro Em Conta" },
            { EnumBemPatrimonial.Equipamento, "Equipamento" },
            { EnumBemPatrimonial.InstrumentoMusical, "Instrumento Musical" },
            { EnumBemPatrimonial.Outro, "Outro" }
            };
        }

        public string PegarBemPatrimonialName(EnumBemPatrimonial key)
        {
            if (bemPatrimonialDict.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                return "Patrimonio não identificado";
            }
        }
    }
}
