namespace Conde.Trominos;

/// <summary>As tabelas do README. Todo numero de la sai daqui.</summary>
public static class Medidas
{
    private static readonly Dictionary<string, object> Cache = [];

    private static T Guardado<T>(string chave, Func<T> faz)
    {
        lock (Cache)
        {
            if (Cache.TryGetValue(chave, out var pronto)) return (T)pronto;
            var feito = faz()!;
            Cache[chave] = feito;
            return feito;
        }
    }

    /// <summary>Os retangulos varridos: todos com m &lt;= n &lt;= 8.</summary>
    public static IReadOnlyList<(int Altura, int Largura)> Tamanhos()
    {
        var saida = new List<(int, int)>();
        for (int m = 1; m <= 8; m++)
            for (int n = m; n <= 8; n++)
                saida.Add((m, n));
        return saida;
    }

    /// <summary>
    /// A tabela principal: para cada retangulo, em quantas das posicoes de
    /// buraco ele se cobre. Varre TODAS as casas, nao uma amostra.
    /// </summary>
    public static IReadOnlyList<Linha> Tabela()
        => Guardado("Tabela", () =>
        {
            var linhas = new List<Linha>();
            foreach (var (m, n) in Tamanhos())
            {
                if (m * n > Nucleo.MaximoDeCasas) continue;
                bool conta = Nucleo.AContaFecha(m, n);
                int cobrem = 0;
                if (conta)
                    for (int buraco = 0; buraco < m * n; buraco++)
                        if (Busca.Cobre(m, n, buraco)) cobrem++;
                linhas.Add(new Linha(m, n, m * n, conta, cobrem));
            }
            return (IReadOnlyList<Linha>)linhas;
        });

    /// <summary>Os retangulos em que a conta fecha, que sao os unicos candidatos.</summary>
    public static IReadOnlyList<Linha> Candidatos()
        => Tabela().Where(l => l.AContaFecha).ToList();

    /// <summary>
    /// Os retangulos em que a conta fecha e MESMO ASSIM nenhuma posicao de
    /// buraco se cobre. Sao as excecoes que a aritmetica nao explica.
    /// </summary>
    public static IReadOnlyList<Linha> Excecoes()
        => Candidatos().Where(l => l.Cobrem == 0).ToList();

    /// <summary>
    /// Os retangulos que se cobrem em ALGUMA posicao mas nao em todas.
    /// A aposta escrita antes de medir era que essa lista estaria vazia.
    /// </summary>
    public static IReadOnlyList<Linha> Parciais()
        => Candidatos().Where(l => l.Cobrem > 0 && l.Cobrem < l.Casas).ToList();

    /// <summary>
    /// Quantas coberturas distintas cada retangulo tem, somadas em todas as
    /// posicoes de buraco, e qual a posicao mais rica.
    /// </summary>
    public static IReadOnlyList<(int Altura, int Largura, long Total, int Buraco, long Melhor)> Coberturas(int ate = 25)
        => Guardado("Coberturas" + ate, () =>
        {
            var saida = new List<(int, int, long, int, long)>();
            foreach (var l in Candidatos())
            {
                if (l.Casas > ate || l.Cobrem == 0) continue;
                long total = 0, melhor = 0;
                int onde = -1;
                for (int buraco = 0; buraco < l.Casas; buraco++)
                {
                    long q = Busca.Conta(l.Altura, l.Largura, buraco);
                    total += q;
                    if (q > melhor) { melhor = q; onde = buraco; }
                }
                saida.Add((l.Altura, l.Largura, total, onde, melhor));
            }
            return (IReadOnlyList<(int, int, long, int, long)>)saida;
        });

    /// <summary>
    /// O juiz contra a busca, buraco por buraco, nos retangulos ate
    /// <paramref name="ate"/> casas. Conta coberturas, nao so sim ou nao.
    /// </summary>
    public static (int Conferidos, int Divergem) JuizContraBusca(int ate = 16)
        => Guardado("Juiz" + ate, () =>
        {
            int conferidos = 0, divergem = 0;
            foreach (var l in Candidatos())
            {
                if (l.Casas > ate) continue;
                for (int buraco = 0; buraco < l.Casas; buraco++)
                {
                    conferidos++;
                    if (Busca.Conta(l.Altura, l.Largura, buraco)
                        != Juiz.Conta(l.Altura, l.Largura, buraco)) divergem++;
                }
            }
            return (conferidos, divergem);
        });

    /// <summary>
    /// Os nos das duas estrategias no mesmo problema: primeira casa vazia
    /// contra casa mais apertada.
    /// </summary>
    public static IReadOnlyList<(int Altura, int Largura, long NosBusca, long NosJuiz, double Razao)> Estrategias(int ate = 16)
        => Guardado("Estrategias" + ate, () =>
        {
            var saida = new List<(int, int, long, long, double)>();
            foreach (var l in Candidatos())
            {
                if (l.Casas > ate) continue;
                long b = 0, j = 0;
                for (int buraco = 0; buraco < l.Casas; buraco++)
                {
                    Busca.Conta(l.Altura, l.Largura, buraco);
                    b += Busca.UltimoNumeroDeNos;
                    Juiz.Conta(l.Altura, l.Largura, buraco);
                    j += Juiz.UltimoNumeroDeNos;
                }
                saida.Add((l.Altura, l.Largura, b, j, j == 0 ? 0 : (double)b / j));
            }
            return (IReadOnlyList<(int, int, long, long, double)>)saida;
        });

    /// <summary>
    /// O mapa de um retangulo: para cada casa, quantas coberturas existem com
    /// o buraco ali. E o jeito de ver POR QUE um retangulo se cobre em umas
    /// posicoes e nao em outras.
    /// </summary>
    public static long[] Mapa(int altura, int largura)
        => Guardado($"Mapa{altura}x{largura}", () =>
        {
            var saida = new long[altura * largura];
            for (int buraco = 0; buraco < saida.Length; buraco++)
                saida[buraco] = Busca.Conta(altura, largura, buraco);
            return saida;
        });

    /// <summary>
    /// As casas em que o 5 por 5 se cobre, como pares (linha, coluna).
    /// </summary>
    public static IReadOnlyList<(int Linha, int Coluna)> OndeOCincoCobre()
    {
        var mapa = Mapa(5, 5);
        var saida = new List<(int, int)>();
        for (int i = 0; i < mapa.Length; i++)
            if (mapa[i] > 0) saida.Add((i / 5, i % 5));
        return saida;
    }

    /// <summary>
    /// As casas em que o retangulo NAO se cobre, como pares (linha, coluna).
    /// Usa o teto de uma cobertura: nao conta, so pergunta se existe.
    /// </summary>
    public static IReadOnlyList<(int Linha, int Coluna)> OndeNaoCobre(int altura, int largura)
        => Guardado($"Nao{altura}x{largura}", () =>
        {
            var saida = new List<(int, int)>();
            for (int b = 0; b < altura * largura; b++)
                if (!Busca.Cobre(altura, largura, b)) saida.Add((b / largura, b % largura));
            return (IReadOnlyList<(int, int)>)saida;
        });

    /// <summary>As potencias de dois, que e o teorema de Golomb de 1954.</summary>
    public static IReadOnlyList<(int Lado, int Casas, int Cobrem)> Golomb()
        => Tabela().Where(l => l.Altura == l.Largura
                            && (l.Altura & (l.Altura - 1)) == 0)
                   .Select(l => (l.Altura, l.Casas, l.Cobrem))
                   .ToList();
}

/// <summary>Uma linha da tabela principal.</summary>
public sealed record Linha(int Altura, int Largura, int Casas, bool AContaFecha, int Cobrem);
