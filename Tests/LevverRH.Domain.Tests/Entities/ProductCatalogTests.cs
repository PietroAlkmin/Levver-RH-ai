namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "ProductCatalog")]
public class ProductCatalogTests
{
    [Fact]
    public void Constructor_Should_CreateValidProduct_When_AllRequiredFieldsProvided()
    {
        // Arrange
        var produtoNome = "Talentos";
        var categoria = "Gestão de Pessoas";
        var modeloCobranca = ModeloCobranca.Mensal;
        var valorBase = 199.90m;
        var descricao = "Gestão completa de talentos";
        var icone = "users-icon";
        var corPrimaria = "#4F46E5";
        var rotaBase = "/talentos";
        var ordemExibicao = 1;

        // Act
        var produto = new ProductCatalog(
            produtoNome,
            categoria,
            modeloCobranca,
            valorBase,
            descricao,
            icone,
            corPrimaria,
            rotaBase,
            ordemExibicao,
            lancado: true);

        // Assert
        produto.Id.Should().NotBeEmpty();
        produto.ProdutoNome.Should().Be(produtoNome);
        produto.Categoria.Should().Be(categoria);
        produto.ModeloCobranca.Should().Be(modeloCobranca);
        produto.ValorBasePadrao.Should().Be(valorBase);
        produto.Descricao.Should().Be(descricao);
        produto.Icone.Should().Be(icone);
        produto.CorPrimaria.Should().Be(corPrimaria);
        produto.RotaBase.Should().Be(rotaBase);
        produto.OrdemExibicao.Should().Be(ordemExibicao);
        produto.Lancado.Should().BeTrue();
        produto.Ativo.Should().BeTrue();
        produto.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_ProdutoNomeIsEmpty()
    {
        // Arrange
        var categoria = "Gestão de Pessoas";
        var modeloCobranca = ModeloCobranca.Mensal;
        var valorBase = 199.90m;

        // Act
        var act = () => new ProductCatalog("", categoria, modeloCobranca, valorBase);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Nome do produto é obrigatório.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_ValorBaseIsNegative()
    {
        // Arrange
        var produtoNome = "Talentos";
        var categoria = "Gestão de Pessoas";
        var modeloCobranca = ModeloCobranca.Mensal;
        var valorBase = -10m;

        // Act
        var act = () => new ProductCatalog(produtoNome, categoria, modeloCobranca, valorBase);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Preço deve ser maior ou igual a zero.");
    }

    [Fact]
    public void Ativar_Should_SetAtivoAsTrue()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);
        produto.Desativar();

        // Act
        produto.Ativar();

        // Assert
        produto.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Desativar_Should_SetAtivoAsFalse()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);

        // Act
        produto.Desativar();

        // Assert
        produto.Ativo.Should().BeFalse();
    }

    [Fact]
    public void AtualizarPreco_Should_UpdateValorBasePadrao()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);
        var novoValor = 249.90m;

        // Act
        produto.AtualizarPreco(novoValor);

        // Assert
        produto.ValorBasePadrao.Should().Be(novoValor);
    }

    [Fact]
    public void AtualizarPreco_Should_ThrowDomainException_When_NovoValorIsNegative()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);

        // Act
        var act = () => produto.AtualizarPreco(-50m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Preço deve ser maior ou igual a zero.");
    }

    [Fact]
    public void AtualizarDescricao_Should_UpdateDescricao()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);
        var novaDescricao = "Nova descrição do produto";

        // Act
        produto.AtualizarDescricao(novaDescricao);

        // Assert
        produto.Descricao.Should().Be(novaDescricao);
    }

    [Fact]
    public void AtualizarConfiguracao_Should_UpdateConfigJsonPadrao()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);
        var configJson = "{\"limiteVagas\": 100}";

        // Act
        produto.AtualizarConfiguracao(configJson);

        // Assert
        produto.ConfigJsonPadrao.Should().Be(configJson);
    }

    [Fact]
    public void MarcarComoLancado_Should_SetLancadoAsTrue()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);

        // Act
        produto.MarcarComoLancado();

        // Assert
        produto.Lancado.Should().BeTrue();
    }

    [Fact]
    public void MarcarComoEmBreve_Should_SetLancadoAsFalse()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m, lancado: true);

        // Act
        produto.MarcarComoEmBreve();

        // Assert
        produto.Lancado.Should().BeFalse();
    }

    [Fact]
    public void AtualizarVisualizacao_Should_UpdateIconeCorAndOrdem()
    {
        // Arrange
        var produto = new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);
        var novoIcone = "new-icon";
        var novaCor = "#FF5733";
        var novaOrdem = 5;

        // Act
        produto.AtualizarVisualizacao(novoIcone, novaCor, novaOrdem);

        // Assert
        produto.Icone.Should().Be(novoIcone);
        produto.CorPrimaria.Should().Be(novaCor);
        produto.OrdemExibicao.Should().Be(novaOrdem);
    }
}
