# L-trominós em tabuleiros deficientes

O L-tromino é a peça de três casas em L: três das quatro casas de um quadrado
2 por 2. Um tabuleiro **deficiente** é um retângulo com **uma** casa removida.

Em 1954 Solomon Golomb provou que todo tabuleiro `2^k × 2^k` deficiente se
cobre com L-trominós, não importa qual casa saia. E os outros retângulos?

Este repositório varre **todo retângulo até 8 por 8 e toda posição de buraco**,
por dois solucionadores independentes, e conta não só se cobre: conta de
quantos jeitos.

## Como rodar

```bash
dotnet test testes/Trominos.Testes/Trominos.Testes.csproj -c Release
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- tabela
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- excecoes
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- golomb
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- coberturas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- mapa
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- estrategias
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- desenho
```

## Os dois solucionadores

- **A busca** pega sempre a **primeira** casa vazia e tenta cobri-la com cada
  peça que a contém. O tabuleiro inteiro é uma máscara de 64 bits.
- **O juiz** pega a casa **mais apertada** — a que tem menos peças possíveis —
  e guarda as peças como vetores de índices, sem máscara nenhuma.

A ordem em que os ramos abrem é completamente diferente, e o número de nós
também. O que tem de bater é a **contagem de coberturas**, não só o sim ou
não — e bateu em 58 posições, com zero divergências.

## A varredura

Só vale a pena olhar os retângulos em que `3` divide `mn - 1`: nos outros
sobra casa.

```
retangulo   casas   posicoes de buraco que se cobrem
1 por 1         1       1 de 1
1 por 4         4       0 de 4
1 por 7         7       0 de 7
2 por 2         4       4 de 4
2 por 5        10       8 de 10
2 por 8        16      12 de 16
4 por 4        16      16 de 16
4 por 7        28      28 de 28
5 por 5        25       9 de 25
5 por 8        40      38 de 40
7 por 7        49      49 de 49
8 por 8        64      64 de 64
```

## O que isto mediu

**O 5 por 5 deficiente se cobre em exatamente 9 das 25 casas: as de linha e
coluna ambas pares.** Não é "quase sempre" nem "quase nunca" — é um padrão de
tabuleiro de damas espaçado, e nenhuma outra casa funciona:

```
5 por 5: coberturas por posicao do buraco
     8     0    16     0     8
     0     0     0     0     0
    16     0    32     0    16
     0     0     0     0     0
     8     0    16     0     8
```

O centro é a casa mais rica, com **32** coberturas; as quatro quinas têm 8
cada; 128 no total.

**O 4 por 4 deficiente tem exatamente UMA cobertura, qualquer que seja a casa
removida.** Dezesseis posições, dezesseis coberturas, uma por posição. O
teorema de Golomb garante que existe; a varredura mostra que é única.

**Quatro dos dez retângulos que aceitam peça dependem de ONDE está o buraco.**
O 2 por 5, o 2 por 8, o 5 por 5 e o 5 por 8. Nos outros seis, se cobre numa
posição então cobre em todas.

**As faixas de duas linhas falham exatamente nas colunas que deixam resto 2 na
divisão por 3.** O 2 por 5 falha na coluna 2; o 2 por 8 falha nas colunas 2 e
5, nas duas linhas. Oito posições boas no primeiro, doze no segundo.

**A estratégia da casa mais apertada rende 3,67 vezes na faixa longa e 1,93 no
quadrado.** No 2 por 8 são 925 nós contra 252; no 4 por 4, 359 contra 186. O
corte vale mais onde a geometria é estreita, porque é lá que a casa apertada
aparece cedo.

**O teorema de Golomb confere nos quatro lados potência de 2 que cabem numa
máscara de 64 bits**: 1, 2, 4 e 8, com todas as casas cobrindo — 1, 4, 16 e 64
posições, todas.

## O que as medidas me corrigiram

**Apostei que, se um retângulo se cobre com o buraco numa casa, ele se cobre em
todas.** Escrevi a aposta antes de varrer. Ela erra em quatro dos dez: o 5 por
5 cobre em 9 das 25, e o 5 por 8 em 38 das 40. Virou tabela e virou teste, com
a lista dos quatro.

**Fui atrás do tema errado primeiro, e só o catálogo pegou.** Este repositório
só existe porque o anterior, com outro assunto, colidiu com um projeto de 9 de
novembro que já cobria exatamente o mesmo terreno, em outra linguagem. A
conferência de tema é a primeira coisa do roteiro e eu li a lista rápido
demais. O repositório antigo não foi tocado: o push para um repositório que já
existe é recusado, e os commits ficaram no disco.

**O número de coberturas é a medida, não o "sim ou não".** A primeira versão do
juiz só respondia se cobria. Como os dois solucionadores concordavam sempre,
não dava para saber se eles concordavam por estarem certos ou por estarem
errados juntos. Contar as coberturas — 128 no 5 por 5, 16 no 4 por 4, 48 no
2 por 8 — é o que transforma "concordam" em prova.

## Os arquivos

```
src/Trominos/Nucleo.cs    as quatro orientacoes da peca, em mascara de 64 bits
src/Trominos/Busca.cs     cobre sempre a PRIMEIRA casa vazia
src/Trominos/Juiz.cs      cobre a casa MAIS APERTADA, com outra estrutura
src/Trominos/Medidas.cs   as tabelas deste README
ferramentas/Medidor/      o executavel que imprime as tabelas e desenha
testes/                   37 testes, com os numeros daqui presos um a um
```

37 testes.
