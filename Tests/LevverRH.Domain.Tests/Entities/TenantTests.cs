namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "Tenant")]
public class TenantTests
{
    [Fact]
    public void Constructor_Should_CreateValidTenant_When_AllRequiredFieldsProvided()
    {
        // Arrange
        var nome = "Levver Tecnologia";
        var cnpj = "12345678000199";
        var email = "contato@levver.ai";
        var telefone = "11999999999";
        var endereco = "Rua Teste, 123";

        // Act
        var tenant = new Tenant(nome, cnpj, email, telefone, endereco);

        // Assert
        tenant.Id.Should().NotBeEmpty();
        tenant.Nome.Should().Be(nome);
        tenant.Cnpj.Should().Be(cnpj);
        tenant.Email.Should().Be(email);
        tenant.Telefone.Should().Be(telefone);
        tenant.Endereco.Should().Be(endereco);
        tenant.Status.Should().Be(TenantStatus.Ativo);
        tenant.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        tenant.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_NomeIsEmpty()
    {
        // Arrange
        var cnpj = "12345678000199";
        var email = "contato@levver.ai";

        // Act
        var act = () => new Tenant("", cnpj, email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Nome é obrigatório.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_CnpjIsEmpty()
    {
        // Arrange
        var nome = "Levver Tecnologia";
        var email = "contato@levver.ai";

        // Act
        var act = () => new Tenant(nome, "", email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CNPJ é obrigatório.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_CnpjIsInvalid()
    {
        // Arrange
        var nome = "Levver Tecnologia";
        var cnpj = "123"; // CNPJ inválido
        var email = "contato@levver.ai";

        // Act
        var act = () => new Tenant(nome, cnpj, email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CNPJ inválido.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_EmailIsEmpty()
    {
        // Arrange
        var nome = "Levver Tecnologia";
        var cnpj = "12345678000199";

        // Act
        var act = () => new Tenant(nome, cnpj, "");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email é obrigatório.");
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_When_EmailIsInvalid()
    {
        // Arrange
        var nome = "Levver Tecnologia";
        var cnpj = "12345678000199";
        var email = "emailinvalido"; // Sem @ e .

        // Act
        var act = () => new Tenant(nome, cnpj, email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email inválido.");
    }

    [Fact]
    public void Ativar_Should_SetStatusAsAtivo()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");
        tenant.Suspender();

        // Act
        tenant.Ativar();

        // Assert
        tenant.Status.Should().Be(TenantStatus.Ativo);
    }

    [Fact]
    public void Desativar_Should_SetStatusAsInativo_And_RaiseDomainEvent()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");

        // Act
        tenant.Desativar();

        // Assert
        tenant.Status.Should().Be(TenantStatus.Inativo);
        tenant.DomainEvents.Should().HaveCount(1);
        tenant.DomainEvents.First().Should().BeOfType<TenantDesativadoEvent>();
    }

    [Fact]
    public void Suspender_Should_SetStatusAsSuspenso()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");

        // Act
        tenant.Suspender();

        // Assert
        tenant.Status.Should().Be(TenantStatus.Suspenso);
    }

    [Fact]
    public void AtualizarNome_Should_UpdateNomeAndDataAtualizacao()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");
        var dataAnterior = tenant.DataAtualizacao;
        var novoNome = "Levver Tecnologia Ltda";

        // Act
        Thread.Sleep(10);
        tenant.AtualizarNome(novoNome);

        // Assert
        tenant.Nome.Should().Be(novoNome);
        tenant.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void AtualizarNome_Should_ThrowDomainException_When_NomeIsEmpty()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");

        // Act
        var act = () => tenant.AtualizarNome("");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Nome é obrigatório.");
    }

    [Fact]
    public void AtualizarCnpj_Should_UpdateCnpjAndDataAtualizacao()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");
        var novoCnpj = "98765432000188";

        // Act
        tenant.AtualizarCnpj(novoCnpj);

        // Assert
        tenant.Cnpj.Should().Be(novoCnpj);
    }

    [Fact]
    public void AtualizarCnpj_Should_ThrowDomainException_When_CnpjIsInvalid()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");

        // Act
        var act = () => tenant.AtualizarCnpj("123");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CNPJ inválido.");
    }

    [Fact]
    public void AtualizarEmail_Should_UpdateEmailAndDataAtualizacao()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");
        var novoEmail = "contato@levver.ai";

        // Act
        tenant.AtualizarEmail(novoEmail);

        // Assert
        tenant.Email.Should().Be(novoEmail);
    }

    [Fact]
    public void AtualizarEmail_Should_ThrowDomainException_When_EmailIsInvalid()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");

        // Act
        var act = () => tenant.AtualizarEmail("emailinvalido");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email inválido.");
    }

    [Fact]
    public void AtualizarDominio_Should_UpdateDominioInLowerCase()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");
        var dominio = "LEVVER.AI";

        // Act
        tenant.AtualizarDominio(dominio);

        // Assert
        tenant.Dominio.Should().Be("levver.ai");
    }

    [Fact]
    public void AtualizarDominio_Should_ThrowDomainException_When_DominioIsInvalid()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");

        // Act
        var act = () => tenant.AtualizarDominio("dominiosemponto");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Domínio inválido.");
    }

    [Fact]
    public void AtualizarDominio_Should_SetDominioAsNull_When_EmptyStringProvided()
    {
        // Arrange
        var tenant = new Tenant("Levver", "12345678000199", "test@levver.ai");
        tenant.AtualizarDominio("levver.ai");

        // Act
        tenant.AtualizarDominio("");

        // Assert
        tenant.Dominio.Should().BeNull();
    }

    [Fact]
    public void CriarPendenteSetupViaSSO_Should_CreateTenantWithPendenteSetupStatus()
    {
        // Arrange
        var dominio = "levver.ai";
        var email = "admin@levver.ai";
        var tenantIdMicrosoft = "tid-123456";

        // Act
        var tenant = Tenant.CriarPendenteSetupViaSSO(dominio, email, tenantIdMicrosoft);

        // Assert
        tenant.Id.Should().NotBeEmpty();
        tenant.Nome.Should().Be(dominio);
        tenant.Cnpj.Should().BeEmpty();
        tenant.Email.Should().Be(email);
        tenant.Dominio.Should().Be("levver.ai");
        tenant.TenantIdMicrosoft.Should().Be(tenantIdMicrosoft);
        tenant.Status.Should().Be(TenantStatus.PendenteSetup);
    }

    [Fact]
    public void CriarPendenteSetupViaSSO_Should_ThrowDomainException_When_DominioIsEmpty()
    {
        // Arrange
        var email = "admin@levver.ai";
        var tenantIdMicrosoft = "tid-123456";

        // Act
        var act = () => Tenant.CriarPendenteSetupViaSSO("", email, tenantIdMicrosoft);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Domínio é obrigatório.");
    }

    [Fact]
    public void CriarPendenteSetupViaSSO_Should_ThrowDomainException_When_DominioIsInvalid()
    {
        // Arrange
        var dominio = "dominiosemponto";
        var email = "admin@levver.ai";
        var tenantIdMicrosoft = "tid-123456";

        // Act
        var act = () => Tenant.CriarPendenteSetupViaSSO(dominio, email, tenantIdMicrosoft);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Domínio inválido.");
    }

    [Fact]
    public void CriarPendenteSetupViaSSO_Should_ThrowDomainException_When_EmailIsEmpty()
    {
        // Arrange
        var dominio = "levver.ai";
        var tenantIdMicrosoft = "tid-123456";

        // Act
        var act = () => Tenant.CriarPendenteSetupViaSSO(dominio, "", tenantIdMicrosoft);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email é obrigatório.");
    }
}
