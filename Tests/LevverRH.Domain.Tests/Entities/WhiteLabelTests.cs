namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "WhiteLabel")]
public class WhiteLabelTests
{
    private Tenant CreateValidTenant()
    {
        return new Tenant("Levver Tech", "12345678000199", "test@levver.ai");
    }

    [Fact]
    public void Constructor_Should_CreateWhiteLabelWithDefaultColors()
    {
        // Arrange
        var tenant = CreateValidTenant();

        // Act
        var whiteLabel = new WhiteLabel(tenant.Id, "Talentos", tenant);

        // Assert
        whiteLabel.TenantId.Should().Be(tenant.Id);
        whiteLabel.SystemName.Should().Be("Talentos");
        whiteLabel.Active.Should().BeTrue();
        whiteLabel.PrimaryColor.Should().Be("#0066CC");
        whiteLabel.SecondaryColor.Should().Be("#00AAFF");
        whiteLabel.AccentColor.Should().Be("#FF6B35");
        whiteLabel.BackgroundColor.Should().Be("#FFFFFF");
        whiteLabel.TextColor.Should().Be("#333333");
        whiteLabel.BorderColor.Should().Be("#E0E0E0");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_TenantIsNull()
    {
        // Act
        var act = () => new WhiteLabel(Guid.NewGuid(), "Talentos", null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Tenant não existe.");
    }

    [Fact]
    public void Ativar_Should_SetActiveAsTrue()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());
        whiteLabel.Desativar();

        // Act
        whiteLabel.Ativar();

        // Assert
        whiteLabel.Active.Should().BeTrue();
    }

    [Fact]
    public void Desativar_Should_SetActiveAsFalse()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        whiteLabel.Desativar();

        // Assert
        whiteLabel.Active.Should().BeFalse();
    }

    [Fact]
    public void AtualizarCores_Should_UpdateAllColors()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        whiteLabel.AtualizarCores(
            "#FF0000",
            "#00FF00",
            "#0000FF",
            "#FFFFFF",
            "#000000",
            "#CCCCCC");

        // Assert
        whiteLabel.PrimaryColor.Should().Be("#FF0000");
        whiteLabel.SecondaryColor.Should().Be("#00FF00");
        whiteLabel.AccentColor.Should().Be("#0000FF");
        whiteLabel.BackgroundColor.Should().Be("#FFFFFF");
        whiteLabel.TextColor.Should().Be("#000000");
        whiteLabel.BorderColor.Should().Be("#CCCCCC");
    }

    [Fact]
    public void AtualizarCores_Should_ThrowDomainException_When_InvalidHexColor()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        var act = () => whiteLabel.AtualizarCores(
            "INVALID",
            "#00FF00",
            "#0000FF",
            "#FFFFFF",
            "#000000",
            "#CCCCCC");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*formato hex*");
    }

    [Fact]
    public void AtualizarCores_Should_AcceptShortHexFormat()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        whiteLabel.AtualizarCores("#F00", "#0F0", "#00F", "#FFF", "#000", "#CCC");

        // Assert
        whiteLabel.PrimaryColor.Should().Be("#F00");
        whiteLabel.SecondaryColor.Should().Be("#0F0");
        whiteLabel.AccentColor.Should().Be("#00F");
    }

    [Fact]
    public void AtualizarLogo_Should_UpdateLogoUrl()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());
        var logoUrl = "https://cdn.example.com/logo.png";

        // Act
        whiteLabel.AtualizarLogo(logoUrl);

        // Assert
        whiteLabel.LogoUrl.Should().Be(logoUrl);
    }

    [Fact]
    public void AtualizarFavicon_Should_UpdateFaviconUrl()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());
        var faviconUrl = "https://cdn.example.com/favicon.ico";

        // Act
        whiteLabel.AtualizarFavicon(faviconUrl);

        // Assert
        whiteLabel.FaviconUrl.Should().Be(faviconUrl);
    }

    [Fact]
    public void AtualizarDominioCustomizado_Should_UpdateDomain()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());
        var dominio = "talentos.levver.ai";

        // Act
        whiteLabel.AtualizarDominioCustomizado(dominio);

        // Assert
        whiteLabel.DominioCustomizado.Should().Be(dominio);
    }

    [Fact]
    public void AtualizarDominioCustomizado_Should_ThrowDomainException_When_InvalidDomain()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        var act = () => whiteLabel.AtualizarDominioCustomizado("dominiosemponto");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Domínio inválido.");
    }

    [Fact]
    public void AtualizarDominioCustomizado_Should_ThrowDomainException_When_DomainHasSpaces()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        var act = () => whiteLabel.AtualizarDominioCustomizado("dominio com espacos.com");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Domínio inválido.");
    }

    [Fact]
    public void AtualizarSystemName_Should_UpdateSystemName()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        whiteLabel.AtualizarSystemName("Recrutamento Pro");

        // Assert
        whiteLabel.SystemName.Should().Be("Recrutamento Pro");
    }

    [Fact]
    public void AtualizarSystemName_Should_ThrowDomainException_When_NameIsEmpty()
    {
        // Arrange
        var whiteLabel = new WhiteLabel(CreateValidTenant().Id, "Talentos", CreateValidTenant());

        // Act
        var act = () => whiteLabel.AtualizarSystemName("");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Nome do sistema é obrigatório.");
    }
}
