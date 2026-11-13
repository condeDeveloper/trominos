using Conde.Trominos;
using Xunit;

namespace Conde.Trominos.Testes;

/// <summary>Casos pequenos, com a resposta sabida na mao.</summary>
public class NucleoTestes
{
    [Fact]
    public void Num_quadrado_2_por_2_existem_quatro_posicoes_de_peca()
    {
        var p = Nucleo.Posicoes(2, 2);
        Assert.Equal(4, p.Count);
        Assert.All(p, x => Assert.Equal(3, Nucleo.Quantas(x)));
        Assert.Equal(4, p.Distinct().Count());
    }

    [Fact]
    public void O_numero_de_posicoes_e_quatro_por_quadrado_2_por_2()
    {
        foreach (var (m, n) in new[] { (2, 2), (3, 3), (4, 4), (4, 7), (5, 5) })
            Assert.Equal(4 * (m - 1) * (n - 1), Nucleo.Posicoes(m, n).Count);
    }

    [Fact]
    public void Numa_faixa_de_uma_linha_nao_cabe_peca_nenhuma()
    {
        Assert.Empty(Nucleo.Posicoes(1, 7));
        Assert.Empty(Nucleo.Posicoes(1, 1));
    }

    [Fact]
    public void O_tabuleiro_cheio_tem_um_bit_por_casa()
    {
        Assert.Equal(16, Nucleo.Quantas(Nucleo.Cheio(4, 4)));
        Assert.Equal(64, Nucleo.Quantas(Nucleo.Cheio(8, 8)));
        Assert.Equal(ulong.MaxValue, Nucleo.Cheio(8, 8));
    }

    [Fact]
    public void A_conta_fecha_quando_tres_divide_as_casas_menos_uma()
    {
        Assert.True(Nucleo.AContaFecha(4, 4));   // 15 casas
        Assert.True(Nucleo.AContaFecha(5, 5));   // 24 casas
        Assert.False(Nucleo.AContaFecha(3, 3));  // 8 casas
        Assert.False(Nucleo.AContaFecha(3, 4));  // 11 casas
    }

    [Fact]
    public void O_2_por_2_deficiente_se_cobre_com_uma_peca_so()
    {
        for (int buraco = 0; buraco < 4; buraco++)
        {
            var pecas = Busca.UmaCobertura(2, 2, buraco);
            Assert.NotNull(pecas);
            Assert.Single(pecas!);
        }
    }

    [Fact]
    public void Toda_cobertura_usa_um_terco_das_casas_restantes_em_pecas()
    {
        foreach (var (m, n, buraco) in new[] { (4, 4, 0), (4, 4, 5), (4, 7, 13), (5, 5, 12) })
        {
            var pecas = Busca.UmaCobertura(m, n, buraco);
            Assert.NotNull(pecas);
            Assert.Equal((m * n - 1) / 3, pecas!.Count);

            // as pecas cobrem exatamente o tabuleiro menos o buraco
            ulong juntas = 0;
            foreach (var p in pecas)
            {
                Assert.Equal(0UL, juntas & p);
                juntas |= p;
            }
            Assert.Equal(Nucleo.Cheio(m, n) & ~(1UL << buraco), juntas);
        }
    }

    [Fact]
    public void O_desenho_tem_uma_linha_por_linha_do_tabuleiro()
    {
        var pecas = Busca.UmaCobertura(4, 4, 0)!;
        var linhas = Nucleo.Desenha(4, 4, 0, pecas).Split('\n');
        Assert.Equal(4, linhas.Length);
        Assert.All(linhas, l => Assert.Equal(4, l.Length));
        Assert.Equal('#', linhas[0][0]);
    }

    [Fact]
    public void Tabuleiro_maior_que_64_casas_nao_cabe_na_mascara()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Nucleo.Cheio(9, 9));
    }
}
