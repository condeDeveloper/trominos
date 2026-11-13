namespace Conde.Trominos;

/// <summary>
/// A busca: pega sempre a PRIMEIRA casa vazia e tenta cobrir ela com cada
/// peca que a contem. Como toda casa tem de ser coberta, nenhuma solucao
/// escapa, e o ramo morre cedo quando a casa nao tem peca possivel.
///
/// O <see cref="Juiz"/> resolve o mesmo problema escolhendo a casa MAIS
/// APERTADA em vez da primeira, com a lista de pecas montada de outro jeito.
/// Os dois tem de dar o mesmo numero de coberturas, nao so o mesmo sim ou nao.
/// </summary>
public static class Busca
{
    /// <summary>Quantos nos a ultima chamada visitou.</summary>
    public static long UltimoNumeroDeNos { get; private set; }

    /// <summary>
    /// Conta as coberturas do retangulo com a casa <paramref name="buraco"/>
    /// removida. Para no <paramref name="teto"/> quando ele e positivo.
    /// </summary>
    public static long Conta(int altura, int largura, int buraco, long teto = 0)
    {
        var posicoes = Nucleo.Posicoes(altura, largura);
        int casas = altura * largura;

        // porCasa[i] = as pecas que cobrem a casa i
        var porCasa = new List<ulong>[casas];
        for (int i = 0; i < casas; i++) porCasa[i] = [];
        foreach (var p in posicoes)
            for (int i = 0; i < casas; i++)
                if ((p & (1UL << i)) != 0) porCasa[i].Add(p);

        ulong inicial = Nucleo.Cheio(altura, largura) & ~(1UL << buraco);
        long achados = 0, nos = 0;

        void Anda(ulong vazio)
        {
            nos++;
            if (vazio == 0)
            {
                achados++;
                return;
            }
            int primeira = System.Numerics.BitOperations.TrailingZeroCount(vazio);
            foreach (var peca in porCasa[primeira])
            {
                if ((peca & ~vazio) != 0) continue;
                Anda(vazio & ~peca);
                if (teto > 0 && achados >= teto) return;
            }
        }

        Anda(inicial);
        UltimoNumeroDeNos = nos;
        return achados;
    }

    /// <summary>O tabuleiro deficiente se cobre?</summary>
    public static bool Cobre(int altura, int largura, int buraco)
        => Conta(altura, largura, buraco, teto: 1) > 0;

    /// <summary>Uma cobertura, se existir: a lista de pecas.</summary>
    public static List<ulong>? UmaCobertura(int altura, int largura, int buraco)
    {
        var posicoes = Nucleo.Posicoes(altura, largura);
        int casas = altura * largura;
        var porCasa = new List<ulong>[casas];
        for (int i = 0; i < casas; i++) porCasa[i] = [];
        foreach (var p in posicoes)
            for (int i = 0; i < casas; i++)
                if ((p & (1UL << i)) != 0) porCasa[i].Add(p);

        var usadas = new List<ulong>();

        bool Anda(ulong vazio)
        {
            if (vazio == 0) return true;
            int primeira = System.Numerics.BitOperations.TrailingZeroCount(vazio);
            foreach (var peca in porCasa[primeira])
            {
                if ((peca & ~vazio) != 0) continue;
                usadas.Add(peca);
                if (Anda(vazio & ~peca)) return true;
                usadas.RemoveAt(usadas.Count - 1);
            }
            return false;
        }

        return Anda(Nucleo.Cheio(altura, largura) & ~(1UL << buraco)) ? usadas : null;
    }
}
