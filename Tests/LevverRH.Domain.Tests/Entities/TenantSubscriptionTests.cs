namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "TenantSubscription")]
public class TenantSubscriptionTests
{
    private Tenant CreateValidTenant()
    {
        return new Tenant("Levver Tech", "12345678000199", "test@levver.ai");
    }

    private ProductCatalog CreateValidProduct()
    {
        return new ProductCatalog("Talentos", "Gestão", ModeloCobranca.Mensal, 199.90m);
    }

    [Fact]
    public void Constructor_Should_CreateValidSubscription_When_AllRequiredFieldsProvided()
    {
        // Arrange
        var tenant = CreateValidTenant();
        var product = CreateValidProduct();
        var dataInicio = DateTime.UtcNow;
        var dataFim = dataInicio.AddMonths(12);

        // Act
        var subscription = new TenantSubscription(
            tenant.Id,
            product.Id,
            199.90m,
            dataInicio,
            tenant,
            product,
            dataFim);

        // Assert
        subscription.Id.Should().NotBeEmpty();
        subscription.TenantId.Should().Be(tenant.Id);
        subscription.ProductCatalogId.Should().Be(product.Id);
        subscription.ValorBaseContratado.Should().Be(199.90m);
        subscription.DataInicio.Should().Be(dataInicio);
        subscription.DataFim.Should().Be(dataFim);
        subscription.Status.Should().Be(SubscriptionStatus.Ativo);
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_ValorIsNegative()
    {
        // Arrange
        var tenant = CreateValidTenant();
        var product = CreateValidProduct();

        // Act
        var act = () => new TenantSubscription(
            tenant.Id,
            product.Id,
            -100m,
            DateTime.UtcNow,
            tenant,
            product);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Valor deve ser maior ou igual a zero.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_DataFimBeforeDataInicio()
    {
        // Arrange
        var tenant = CreateValidTenant();
        var product = CreateValidProduct();
        var dataInicio = DateTime.UtcNow;
        var dataFim = dataInicio.AddDays(-1);

        // Act
        var act = () => new TenantSubscription(
            tenant.Id,
            product.Id,
            199.90m,
            dataInicio,
            tenant,
            product,
            dataFim);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Data fim deve ser posterior à data início.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_TenantIsNull()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        var act = () => new TenantSubscription(
            Guid.NewGuid(),
            product.Id,
            199.90m,
            DateTime.UtcNow,
            null!,
            product);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Tenant não existe.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_ProductIsNull()
    {
        // Arrange
        var tenant = CreateValidTenant();

        // Act
        var act = () => new TenantSubscription(
            tenant.Id,
            Guid.NewGuid(),
            199.90m,
            DateTime.UtcNow,
            tenant,
            null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Produto não existe.");
    }

    [Fact]
    public void Ativar_Should_SetStatusAsAtivo()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());
        subscription.Suspender();

        // Act
        subscription.Ativar();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Ativo);
    }

    [Fact]
    public void Cancelar_Should_SetStatusAsCancelado_And_RaiseDomainEvent()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        subscription.Cancelar();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Cancelado);
        subscription.DataFim.Should().NotBeNull();
        subscription.DomainEvents.Should().HaveCount(1);
        subscription.DomainEvents.First().Should().BeOfType<SubscriptionCanceledEvent>();
    }

    [Fact]
    public void Suspender_Should_SetStatusAsSuspenso()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        subscription.Suspender();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Suspenso);
    }

    [Fact]
    public void Concluir_Should_SetStatusAsConcluido()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        subscription.Concluir();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Concluido);
        subscription.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Reativar_Should_SetStatusAsAtivo_And_ClearDataFim()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());
        subscription.Cancelar();

        // Act
        subscription.Reativar();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Ativo);
        subscription.DataFim.Should().BeNull();
    }

    [Fact]
    public void EstaVigente_Should_ReturnTrue_When_AtivoAndWithinDates()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow.AddDays(-10),
            CreateValidTenant(),
            CreateValidProduct(),
            DateTime.UtcNow.AddDays(10));

        // Act
        var resultado = subscription.EstaVigente();

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void EstaVigente_Should_ReturnFalse_When_StatusIsNotAtivo()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow.AddDays(-10),
            CreateValidTenant(),
            CreateValidProduct(),
            DateTime.UtcNow.AddDays(10));
        subscription.Cancelar();

        // Act
        var resultado = subscription.EstaVigente();

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void EstaVigente_Should_ReturnFalse_When_BeforeDataInicio()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow.AddDays(10),
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        var resultado = subscription.EstaVigente();

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void AtualizarValor_Should_UpdateValorBaseContratado()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        subscription.AtualizarValor(299.90m);

        // Assert
        subscription.ValorBaseContratado.Should().Be(299.90m);
    }

    [Fact]
    public void AtualizarValor_Should_ThrowDomainException_When_ValorIsNegative()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        var act = () => subscription.AtualizarValor(-100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Valor deve ser maior ou igual a zero.");
    }

    [Fact]
    public void RenovarContrato_Should_UpdateDataFim()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());
        var novaDataFim = DateTime.UtcNow.AddMonths(12);

        // Act
        subscription.RenovarContrato(novaDataFim);

        // Assert
        subscription.DataFim.Should().Be(novaDataFim);
    }

    [Fact]
    public void RenovarContrato_Should_ThrowDomainException_When_DataFimBeforeDataInicio()
    {
        // Arrange
        var dataInicio = DateTime.UtcNow;
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            dataInicio,
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        var act = () => subscription.RenovarContrato(dataInicio.AddDays(-1));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Data fim deve ser posterior à data início.");
    }

    [Fact]
    public void DiasRestantes_Should_ReturnNull_When_NoDataFim()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct());

        // Act
        var dias = subscription.DiasRestantes();

        // Assert
        dias.Should().BeNull();
    }

    [Fact]
    public void DiasRestantes_Should_ReturnCorrectDays_When_DataFimInFuture()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow,
            CreateValidTenant(),
            CreateValidProduct(),
            DateTime.UtcNow.AddDays(30));

        // Act
        var dias = subscription.DiasRestantes();

        // Assert
        dias.Should().HaveValue();
        dias!.Value.Should().BeGreaterThanOrEqualTo(29); // Aproximadamente 30 dias
        dias.Value.Should().BeLessThanOrEqualTo(30);
    }

    [Fact]
    public void DiasRestantes_Should_ReturnZero_When_DataFimInPast()
    {
        // Arrange
        var subscription = new TenantSubscription(
            CreateValidTenant().Id,
            CreateValidProduct().Id,
            199.90m,
            DateTime.UtcNow.AddDays(-60),
            CreateValidTenant(),
            CreateValidProduct(),
            DateTime.UtcNow.AddDays(-30));

        // Act
        var dias = subscription.DiasRestantes();

        // Assert
        dias.Should().Be(0);
    }
}
