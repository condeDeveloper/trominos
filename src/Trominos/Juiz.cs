namespace Conde.Trominos;

/// <summary>
/// O juiz resolve o mesmo problema por outro caminho: em vez de pegar sempre
/// a primeira casa vazia, ele pega a casa com MENOS pecas possiveis. A ordem
/// em que os ramos abrem e completamente diferente, o numero de nos tambem, e
/// a contagem de coberturas tem de ser identica.
///
/// Ele tambem guarda a lista de pecas de um jeito diferente: um vetor de
/// vetores por casa, montado a partir da enumeracao crua, sem mascara.
/// </summary>
public static class Juiz
{
    /// <summary>Quantos nos a ultima chamada visitou.</summary>
    public static long UltimoNumeroDeNos { get; private set; }

    /// <summary>Conta as coberturas, escolhendo sempre a casa mais apertada.</summary>
    public static long Conta(int altura, int largura, int buraco)
    {
        int casas = altura * largura;
        var pecas = new List<int[]>();
        for (int l = 0; l + 1 < altura; l++)
            for (int c = 0; c + 1 < largura; c++)
            {
                var quatro = new[]
                {
                    l * largura + c, l * largura + c + 1,
                    (l + 1) * largura + c, (l + 1) * largura + c + 1,
                };
                for (int fora = 0; fora < 4; fora++)
                    pecas.Add(quatro.Where((_, i) => i != fora).ToArray());
            }

        var ocupada = new bool[casas];
        ocupada[buraco] = true;
        int faltam = casas - 1;
        long achados = 0, nos = 0;

        bool Cabe(int[] peca)
        {
            foreach (int casa in peca)
                if (ocupada[casa]) return false;
            return true;
        }

        void Anda()
        {
            nos++;
            if (faltam == 0)
            {
                achados++;
                return;
            }

            // a casa vazia com menos pecas possiveis
            int alvo = -1, melhor = int.MaxValue;
            for (int i = 0; i < casas; i++)
            {
                if (ocupada[i]) continue;
                int quantas = 0;
                foreach (var peca in pecas)
                    if (peca.Contains(i) && Cabe(peca)) quantas++;
                if (quantas < melhor)
                {
                    melhor = quantas;
                    alvo = i;
                }
                if (melhor == 0) break;
            }

            if (melhor == 0) return;

            foreach (var peca in pecas)
            {
                if (!peca.Contains(alvo) || !Cabe(peca)) continue;
                foreach (int casa in peca) ocupada[casa] = true;
                faltam -= 3;
                Anda();
                faltam += 3;
                foreach (int casa in peca) ocupada[casa] = false;
            }
        }

        if (faltam % 3 != 0)
        {
            UltimoNumeroDeNos = 0;
            return 0;
        }

        Anda();
        UltimoNumeroDeNos = nos;
        return achados;
    }

    /// <summary>O tabuleiro deficiente se cobre?</summary>
    public static bool Cobre(int altura, int largura, int buraco)
        => Conta(altura, largura, buraco) > 0;
}
