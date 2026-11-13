namespace Conde.Trominos;

/// <summary>
/// O L-tromino e a peca de tres casas em forma de L: tres das quatro casas de
/// um quadrado 2 por 2. Tem quatro orientacoes, uma para cada casa que falta.
///
/// Um tabuleiro DEFICIENTE e um retangulo m por n com UMA casa removida. A
/// pergunta de Golomb, de 1954: quais deles se cobrem com L-trominos?
///
/// O tabuleiro vira uma mascara de 64 bits, e a casa (linha, coluna) e o bit
/// linha * n + coluna.
/// </summary>
public static class Nucleo
{
    /// <summary>O maior tabuleiro que cabe numa mascara de 64 bits.</summary>
    public const int MaximoDeCasas = 64;

    /// <summary>A mascara com todas as casas do retangulo.</summary>
    public static ulong Cheio(int altura, int largura)
    {
        int casas = altura * largura;
        if (casas > MaximoDeCasas) throw new ArgumentOutOfRangeException(nameof(largura));
        return casas == 64 ? ulong.MaxValue : (1UL << casas) - 1;
    }

    /// <summary>
    /// Todas as posicoes de L-tromino que cabem no retangulo: para cada
    /// quadrado 2 por 2, as quatro maneiras de deixar uma casa de fora.
    /// </summary>
    public static List<ulong> Posicoes(int altura, int largura)
    {
        var saida = new List<ulong>();
        for (int l = 0; l + 1 < altura; l++)
            for (int c = 0; c + 1 < largura; c++)
            {
                int a = l * largura + c;
                int b = l * largura + c + 1;
                int d = (l + 1) * largura + c;
                int e = (l + 1) * largura + c + 1;
                var quatro = new[] { a, b, d, e };
                for (int fora = 0; fora < 4; fora++)
                {
                    ulong peca = 0;
                    for (int i = 0; i < 4; i++)
                        if (i != fora) peca |= 1UL << quatro[i];
                    saida.Add(peca);
                }
            }
        return saida;
    }

    /// <summary>A condicao de contagem: as casas restantes tem de ser multiplo de 3.</summary>
    public static bool AContaFecha(int altura, int largura)
        => (altura * largura - 1) % 3 == 0;

    /// <summary>Quantas casas tem o bit ligado.</summary>
    public static int Quantas(ulong mascara)
        => System.Numerics.BitOperations.PopCount(mascara);

    /// <summary>O desenho de uma cobertura, com uma letra por peca.</summary>
    public static string Desenha(int altura, int largura, int buraco, IReadOnlyList<ulong> pecas)
    {
        var letra = new char[altura * largura];
        for (int i = 0; i < letra.Length; i++) letra[i] = '.';
        letra[buraco] = '#';
        const string alfabeto = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int p = 0; p < pecas.Count; p++)
            for (int i = 0; i < letra.Length; i++)
                if ((pecas[p] & (1UL << i)) != 0) letra[i] = alfabeto[p % alfabeto.Length];

        var linhas = new List<string>();
        for (int l = 0; l < altura; l++)
            linhas.Add(new string(letra, l * largura, largura));
        return string.Join("\n", linhas);
    }
}
