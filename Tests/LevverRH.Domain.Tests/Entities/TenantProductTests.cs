namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "TenantProduct")]
public class TenantProductTests
{
    [Fact]
    public void Constructor_Should_CreateValidTenantProduct_When_ValidIdsProvided()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var configuracaoJson = "{\"limiteVagas\": 50}";

        // Act
        var tenantProduct = new TenantProduct(tenantId, productId, configuracaoJson);

        // Assert
        tenantProduct.Id.Should().NotBeEmpty();
        tenantProduct.TenantId.Should().Be(tenantId);
        tenantProduct.ProductId.Should().Be(productId);
        tenantProduct.Ativo.Should().BeTrue();
        tenantProduct.DataAtivacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        tenantProduct.DataDesativacao.Should().BeNull();
        tenantProduct.ConfiguracaoJson.Should().Be(configuracaoJson);
        tenantProduct.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_TenantIdIsEmpty()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var act = () => new TenantProduct(Guid.Empty, productId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("TenantId é obrigatório.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_ProductIdIsEmpty()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var act = () => new TenantProduct(tenantId, Guid.Empty);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("ProductId é obrigatório.");
    }

    [Fact]
    public void Ativar_Should_SetAtivoAsTrue_And_UpdateDataAtivacao()
    {
        // Arrange
        var tenantProduct = new TenantProduct(Guid.NewGuid(), Guid.NewGuid());
        tenantProduct.Desativar();

        // Act
        tenantProduct.Ativar();

        // Assert
        tenantProduct.Ativo.Should().BeTrue();
        tenantProduct.DataAtivacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        tenantProduct.DataDesativacao.Should().BeNull();
    }

    [Fact]
    public void Ativar_Should_ThrowDomainException_When_ProductIsAlreadyActive()
    {
        // Arrange
        var tenantProduct = new TenantProduct(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var act = () => tenantProduct.Ativar();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Produto já está ativo para este tenant.");
    }

    [Fact]
    public void Desativar_Should_SetAtivoAsFalse_And_UpdateDataDesativacao()
    {
        // Arrange
        var tenantProduct = new TenantProduct(Guid.NewGuid(), Guid.NewGuid());

        // Act
        tenantProduct.Desativar();

        // Assert
        tenantProduct.Ativo.Should().BeFalse();
        tenantProduct.DataDesativacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Desativar_Should_ThrowDomainException_When_ProductIsAlreadyInactive()
    {
        // Arrange
        var tenantProduct = new TenantProduct(Guid.NewGuid(), Guid.NewGuid());
        tenantProduct.Desativar();

        // Act
        var act = () => tenantProduct.Desativar();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Produto já está desativado para este tenant.");
    }

    [Fact]
    public void AtualizarConfiguracao_Should_UpdateConfiguracaoJson()
    {
        // Arrange
        var tenantProduct = new TenantProduct(Guid.NewGuid(), Guid.NewGuid());
        var novaConfiguracao = "{\"limiteVagas\": 100, \"recursosPremium\": true}";

        // Act
        tenantProduct.AtualizarConfiguracao(novaConfiguracao);

        // Assert
        tenantProduct.ConfiguracaoJson.Should().Be(novaConfiguracao);
    }

    [Fact]
    public void AtualizarConfiguracao_Should_AcceptNullConfiguration()
    {
        // Arrange
        var tenantProduct = new TenantProduct(Guid.NewGuid(), Guid.NewGuid(), "{\"limiteVagas\": 50}");

        // Act
        tenantProduct.AtualizarConfiguracao(null);

        // Assert
        tenantProduct.ConfiguracaoJson.Should().BeNull();
    }
}
