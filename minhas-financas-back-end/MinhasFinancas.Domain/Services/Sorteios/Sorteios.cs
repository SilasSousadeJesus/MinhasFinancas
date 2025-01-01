namespace MinhasFinancas.Domain.Services.Sorteios
{
    public class Sorteios
    {
        public Sorteios() { }

        public int[] MegaSena()
        {
            return Sorteio(6, 1, 60);
        }


        private int[] Sorteio(int quantidade, int min, int max)
        {
            Random random = new Random();
            return Enumerable.Range(min, max - min + 1)
                             .OrderBy(_ => random.Next())
                             .Take(quantidade)
                             .ToArray();
        }
    }
}


