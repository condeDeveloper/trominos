using Conde.Trominos;
using Xunit;

namespace Conde.Trominos.Testes;

/// <summary>Todo numero do README esta preso aqui.</summary>
public class MedidasTestes
{
    // altura, largura, casas, posicoes de buraco que se cobrem
    public static TheoryData<int, int, int, int> ATabela() => new()
    {
        { 1, 1, 1, 1 },
        { 1, 4, 4, 0 },
        { 1, 7, 7, 0 },
        { 2, 2, 4, 4 },
        { 2, 5, 10, 8 },
        { 2, 8, 16, 12 },
        { 4, 4, 16, 16 },
        { 4, 7, 28, 28 },
        { 5, 5, 25, 9 },
        { 5, 8, 40, 38 },
        { 7, 7, 49, 49 },
        { 8, 8, 64, 64 },
    };

    [Theory]
    [MemberData(nameof(ATabela))]
    public void A_tabela_principal(int m, int n, int casas, int cobrem)
    {
        var l = Medidas.Candidatos().Single(x => x.Altura == m && x.Largura == n);
        Assert.Equal(casas, l.Casas);
        Assert.Equal(cobrem, l.Cobrem);
    }

    [Fact]
    public void So_doze_retangulos_ate_oito_por_oito_tem_a_conta_fechando()
    {
        Assert.Equal(12, Medidas.Candidatos().Count);
        foreach (var l in Medidas.Candidatos())
            Assert.Equal(0, (l.Casas - 1) % 3);
    }

    [Fact]
    public void As_unicas_que_nao_cobrem_nada_sao_as_faixas_de_uma_linha()
    {
        var excecoes = Medidas.Excecoes();
        Assert.Equal(2, excecoes.Count);
        Assert.All(excecoes, l => Assert.Equal(1, l.Altura));
        Assert.Equal(new[] { 4, 7 }, excecoes.Select(l => l.Largura).ToArray());
    }

    [Fact]
    public void Quatro_dos_dez_dependem_de_onde_esta_o_buraco()
    {
        // a aposta escrita antes de medir: "se cobre numa posicao, cobre em
        // todas". Ela erra em quatro dos dez retangulos que tem peca.
        var parciais = Medidas.Parciais();
        Assert.Equal(4, parciais.Count);
        Assert.Equal(new[] { (2, 5), (2, 8), (5, 5), (5, 8) },
                     parciais.Select(l => (l.Altura, l.Largura)).ToArray());
    }

    [Fact]
    public void O_cinco_por_cinco_se_cobre_so_nas_nove_casas_de_linha_e_coluna_pares()
    {
        var onde = Medidas.OndeOCincoCobre();
        Assert.Equal(9, onde.Count);
        foreach (var (linha, coluna) in onde)
        {
            Assert.Equal(0, linha % 2);
            Assert.Equal(0, coluna % 2);
        }
        // e as nove casas pares estao todas la
        Assert.Equal(9, onde.Count(x => x.Linha % 2 == 0 && x.Coluna % 2 == 0));
    }

    [Fact]
    public void O_centro_do_cinco_por_cinco_e_a_casa_mais_rica_com_32_coberturas()
    {
        var mapa = Medidas.Mapa(5, 5);
        Assert.Equal(32L, mapa[2 * 5 + 2]);
        Assert.Equal(8L, mapa[0]);
        Assert.Equal(16L, mapa[2]);
        Assert.Equal(128L, mapa.Sum());
    }

    [Fact]
    public void O_quatro_por_quatro_deficiente_tem_exatamente_uma_cobertura_em_cada_casa()
    {
        var mapa = Medidas.Mapa(4, 4);
        Assert.Equal(16, mapa.Length);
        Assert.All(mapa, q => Assert.Equal(1L, q));
    }

    [Fact]
    public void As_faixas_de_duas_linhas_falham_nas_colunas_que_deixam_resto_dois()
    {
        foreach (var (m, n) in new[] { (2, 5), (2, 8) })
            foreach (var (linha, coluna) in Medidas.OndeNaoCobre(m, n))
            {
                Assert.True(linha is 0 or 1);
                Assert.Equal(2, coluna % 3);
            }
        Assert.Equal(2, Medidas.OndeNaoCobre(2, 5).Count);
        Assert.Equal(4, Medidas.OndeNaoCobre(2, 8).Count);
    }

    [Fact]
    public void O_teorema_de_Golomb_vale_nos_quatro_lados_potencia_de_dois()
    {
        var g = Medidas.Golomb();
        Assert.Equal(4, g.Count);
        foreach (var (lado, casas, cobrem) in g)
        {
            Assert.Equal(lado * lado, casas);
            Assert.Equal(casas, cobrem);
        }
        Assert.Equal(new[] { 1, 2, 4, 8 }, g.Select(x => x.Lado).ToArray());
    }

    [Fact]
    public void As_duas_estrategias_nao_divergem_em_58_posicoes()
    {
        Assert.Equal((58, 0), Medidas.JuizContraBusca(16));
    }

    // altura, largura, nos pela primeira casa, nos pela casa mais apertada
    public static TheoryData<int, int, long, long> AsEstrategias() => new()
    {
        { 2, 2, 8, 8 },
        { 2, 5, 105, 62 },
        { 2, 8, 925, 252 },
        { 4, 4, 359, 186 },
    };

    [Theory]
    [MemberData(nameof(AsEstrategias))]
    public void O_custo_das_duas_estrategias(int m, int n, long busca, long juiz)
    {
        var linha = Medidas.Estrategias(16).Single(x => x.Altura == m && x.Largura == n);
        Assert.Equal(busca, linha.NosBusca);
        Assert.Equal(juiz, linha.NosJuiz);
    }

    [Fact]
    public void A_casa_mais_apertada_corta_mais_na_faixa_longa_que_no_quadrado()
    {
        var faixa = Medidas.Estrategias(16).Single(x => x.Altura == 2 && x.Largura == 8);
        var quadrado = Medidas.Estrategias(16).Single(x => x.Altura == 4 && x.Largura == 4);
        Assert.True(faixa.Razao > 3.6);
        Assert.True(quadrado.Razao < 2.0);
    }

    [Fact]
    public void As_coberturas_totais_de_cada_retangulo_pequeno()
    {
        var c = Medidas.Coberturas(25).ToDictionary(x => (x.Altura, x.Largura), x => x.Total);
        Assert.Equal(1L, c[(1, 1)]);
        Assert.Equal(4L, c[(2, 2)]);
        Assert.Equal(16L, c[(2, 5)]);
        Assert.Equal(48L, c[(2, 8)]);
        Assert.Equal(16L, c[(4, 4)]);
        Assert.Equal(128L, c[(5, 5)]);
    }

    [Fact]
    public void Toda_posicao_que_a_tabela_diz_que_cobre_tem_mesmo_uma_cobertura()
    {
        foreach (var l in Medidas.Candidatos())
        {
            if (l.Casas > 28) continue;
            int achadas = 0;
            for (int buraco = 0; buraco < l.Casas; buraco++)
                if (Busca.UmaCobertura(l.Altura, l.Largura, buraco) is not null) achadas++;
            Assert.Equal(l.Cobrem, achadas);
        }
    }
}
