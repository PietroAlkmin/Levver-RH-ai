namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "IntegrationCredentials")]
public class IntegrationCredentialsTests
{
    private Tenant CreateValidTenant()
    {
        return new Tenant("Levver Tech", "12345678000199", "test@levver.ai");
    }

    [Fact]
    public void Constructor_Should_CreateValidCredentials_When_AllRequiredFieldsProvided()
    {
        // Arrange
        var tenant = CreateValidTenant();
        var plataforma = "LinkedIn";
        var token = "token123456";
        var refreshToken = "refresh123";
        var expiresAt = DateTime.UtcNow.AddDays(30);
        var config = "{\"apiVersion\":\"v2\"}";

        // Act
        var credentials = new IntegrationCredentials(
            tenant.Id,
            plataforma,
            token,
            tenant,
            refreshToken,
            expiresAt,
            config);

        // Assert
        credentials.Id.Should().NotBeEmpty();
        credentials.TenantId.Should().Be(tenant.Id);
        credentials.Plataforma.Should().Be(plataforma);
        credentials.Token.Should().Be(token);
        credentials.RefreshToken.Should().Be(refreshToken);
        credentials.ExpiresAt.Should().Be(expiresAt);
        credentials.ConfiguracoesJson.Should().Be(config);
        credentials.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_PlataformaIsEmpty()
    {
        // Arrange
        var tenant = CreateValidTenant();

        // Act
        var act = () => new IntegrationCredentials(
            tenant.Id,
            "",
            "token123",
            tenant);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Plataforma é obrigatória.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_TokenIsEmpty()
    {
        // Arrange
        var tenant = CreateValidTenant();

        // Act
        var act = () => new IntegrationCredentials(
            tenant.Id,
            "LinkedIn",
            "",
            tenant);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Token é obrigatório.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_TenantIsNull()
    {
        // Act
        var act = () => new IntegrationCredentials(
            Guid.NewGuid(),
            "LinkedIn",
            "token123",
            null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Tenant não existe.");
    }

    [Fact]
    public void Ativar_Should_SetAtivoAsTrue()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "token123",
            CreateValidTenant());
        credentials.Desativar();

        // Act
        credentials.Ativar();

        // Assert
        credentials.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Desativar_Should_SetAtivoAsFalse()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "token123",
            CreateValidTenant());

        // Act
        credentials.Desativar();

        // Assert
        credentials.Ativo.Should().BeFalse();
    }

    [Fact]
    public void AtualizarToken_Should_UpdateTokenAndRefreshToken()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "oldToken",
            CreateValidTenant());
        var newToken = "newToken456";
        var newRefreshToken = "newRefresh456";
        var newExpiresAt = DateTime.UtcNow.AddDays(60);

        // Act
        credentials.AtualizarToken(newToken, newRefreshToken, newExpiresAt);

        // Assert
        credentials.Token.Should().Be(newToken);
        credentials.RefreshToken.Should().Be(newRefreshToken);
        credentials.ExpiresAt.Should().Be(newExpiresAt);
    }

    [Fact]
    public void AtualizarToken_Should_ThrowDomainException_When_TokenIsEmpty()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "token123",
            CreateValidTenant());

        // Act
        var act = () => credentials.AtualizarToken("");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Token é obrigatório.");
    }

    [Fact]
    public void AtualizarConfiguracoes_Should_UpdateConfigJson()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "token123",
            CreateValidTenant());
        var newConfig = "{\"newSetting\":\"value\"}";

        // Act
        credentials.AtualizarConfiguracoes(newConfig);

        // Assert
        credentials.ConfiguracoesJson.Should().Be(newConfig);
    }

    [Fact]
    public void TokenExpirado_Should_ReturnTrue_When_ExpiresAtIsInPast()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "token123",
            CreateValidTenant(),
            expiresAt: DateTime.UtcNow.AddDays(-1));

        // Act
        var resultado = credentials.TokenExpirado();

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void TokenExpirado_Should_ReturnFalse_When_ExpiresAtIsInFuture()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "token123",
            CreateValidTenant(),
            expiresAt: DateTime.UtcNow.AddDays(30));

        // Act
        var resultado = credentials.TokenExpirado();

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void TokenExpirado_Should_ReturnFalse_When_ExpiresAtIsNull()
    {
        // Arrange
        var credentials = new IntegrationCredentials(
            CreateValidTenant().Id,
            "LinkedIn",
            "token123",
            CreateValidTenant());

        // Act
        var resultado = credentials.TokenExpirado();

        // Assert
        resultado.Should().BeFalse();
    }
}
