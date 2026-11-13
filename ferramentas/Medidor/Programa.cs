using Conde.Trominos;

if (args.Length == 0)
{
    Console.WriteLine("medidor <tabela|excecoes|golomb|coberturas|mapa|juiz|estrategias|desenho|tudo>");
    return 1;
}

switch (args[0])
{
    case "tabela": Tabela(); break;
    case "excecoes": Excecoes(); break;
    case "golomb": Golomb(); break;
    case "coberturas": Coberturas(); break;
    case "mapa": Mapa(); break;
    case "juiz": JuizContraBusca(); break;
    case "estrategias": Estrategias(); break;
    case "desenho": Desenho(); break;
    case "tudo":
        Tabela(); Excecoes(); Golomb(); Coberturas(); Mapa(); JuizContraBusca();
        Estrategias(); Desenho();
        break;
    default:
        Console.WriteLine($"nao conheco '{args[0]}'");
        return 1;
}
return 0;

void Tabela()
{
    Console.WriteLine("os retangulos em que 3 divide mn-1, que sao os unicos candidatos:");
    Console.WriteLine("retangulo   casas   posicoes de buraco que se cobrem");
    foreach (var l in Medidas.Candidatos())
        Console.WriteLine($"{l.Altura} por {l.Largura}    {l.Casas,5}   {l.Cobrem,5} de {l.Casas}");
}

void Excecoes()
{
    var excecoes = Medidas.Excecoes();
    Console.WriteLine($"retangulos em que a conta fecha e NADA se cobre: {excecoes.Count}");
    foreach (var l in excecoes)
        Console.WriteLine($"   {l.Altura} por {l.Largura}: {l.Casas} casas, " +
                          $"{l.Cobrem} das {l.Casas} posicoes");
    var parciais = Medidas.Parciais();
    Console.WriteLine();
    Console.WriteLine($"retangulos que se cobrem em ALGUMAS posicoes e nao em todas: {parciais.Count}");
    foreach (var l in parciais)
        Console.WriteLine($"   {l.Altura} por {l.Largura}: {l.Cobrem} de {l.Casas}");
}

void Golomb()
{
    Console.WriteLine("o teorema de Golomb, de 1954: lado potencia de 2");
    Console.WriteLine("lado   casas   posicoes que se cobrem");
    foreach (var (lado, casas, cobrem) in Medidas.Golomb())
        Console.WriteLine($"{lado,-6} {casas,5}   {cobrem,5} de {casas}");
}

void Coberturas()
{
    Console.WriteLine("retangulo   coberturas ao todo   buraco mais rico   coberturas dele");
    foreach (var (m, n, total, buraco, melhor) in Medidas.Coberturas(25))
        Console.WriteLine($"{m} por {n}    {total,17:n0}   {$"({buraco / n},{buraco % n})",16}   {melhor,15:n0}");
}

void JuizContraBusca()
{
    var (conf, div) = Medidas.JuizContraBusca(16);
    Console.WriteLine($"primeira casa vazia contra casa mais apertada: " +
                      $"{conf} posicoes conferidas, {div} divergem");
}

void Estrategias()
{
    Console.WriteLine("retangulo   nos pela primeira casa   nos pela mais apertada   razao");
    foreach (var (m, n, b, j, razao) in Medidas.Estrategias(16))
        Console.WriteLine($"{m} por {n}    {b,21:n0}   {j,22:n0}   {razao,7:n2}");
}

void Desenho()
{
    foreach (var (m, n, buraco) in new[] { (4, 4, 0), (4, 4, 5), (2, 2, 3), (4, 7, 13) })
    {
        var pecas = Busca.UmaCobertura(m, n, buraco);
        Console.WriteLine($"{m} por {n}, buraco em ({buraco / n},{buraco % n}): " +
                          (pecas is null ? "nao se cobre" : $"{pecas.Count} pecas"));
        if (pecas is not null) Console.WriteLine(Nucleo.Desenha(m, n, buraco, pecas));
        Console.WriteLine();
    }
}

void Mapa()
{
    foreach (var (m, n) in new[] { (5, 5), (2, 5), (2, 8), (4, 4) })
    {
        Console.WriteLine($"{m} por {n}: coberturas por posicao do buraco");
        var mapa = Medidas.Mapa(m, n);
        for (int l = 0; l < m; l++)
        {
            for (int c = 0; c < n; c++) Console.Write($"{mapa[l * n + c],6:n0}");
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    var onde = Medidas.OndeOCincoCobre();
    Console.WriteLine($"o 5 por 5 se cobre em {onde.Count} das 25 casas:");
    Console.WriteLine("   " + string.Join("  ", onde.Select(x => $"({x.Linha},{x.Coluna})")));
}
