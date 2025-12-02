namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "User")]
public class UserTests
{
    private Tenant CriarTenantAtivo()
    {
        return new Tenant(
            nome: "Empresa Teste",
            cnpj: "12345678901234",
            email: "contato@empresa.com.br"
        );
    }

    #region Factory Methods Tests

    [Fact]
    public void CriarComAzureAd_DeveCreiarUsuarioCorretamente()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var email = "usuario@empresa.com";
        var nome = "João Silva";
        var azureAdId = "azure-123";

        // Act
        var user = User.CriarComAzureAd(
            tenantId: tenant.Id,
            email: email,
            nome: nome,
            role: UserRole.Viewer,
            tenant: tenant,
            azureAdId: azureAdId
        );

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.TenantId.Should().Be(tenant.Id);
        user.Email.Should().Be(email.ToLowerInvariant());
        user.Nome.Should().Be(nome);
        user.Role.Should().Be(UserRole.Viewer);
        user.AuthType.Should().Be(AuthType.AzureAd);
        user.AzureAdId.Should().Be(azureAdId);
        user.PasswordHash.Should().BeNull();
        user.Ativo.Should().BeTrue();
        user.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarComAzureAd_DeveLancarExcecao_QuandoEmailVazio()
    {
        // Arrange
        var tenant = CriarTenantAtivo();

        // Act
        var act = () => User.CriarComAzureAd(
            tenantId: tenant.Id,
            email: "",
            nome: "João Silva",
            role: UserRole.Viewer,
            tenant: tenant
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email é obrigatório.");
    }

    [Fact]
    public void CriarComAzureAd_DeveLancarExcecao_QuandoEmailInvalido()
    {
        // Arrange
        var tenant = CriarTenantAtivo();

        // Act
        var act = () => User.CriarComAzureAd(
            tenantId: tenant.Id,
            email: "email-invalido",
            nome: "João Silva",
            role: UserRole.Viewer,
            tenant: tenant
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email inválido.");
    }

    [Fact]
    public void CriarComSenha_DeveCriarUsuarioCorretamente()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var email = "usuario@empresa.com";
        var nome = "Maria Silva";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Senha@123");

        // Act
        var user = User.CriarComSenha(
            tenantId: tenant.Id,
            email: email,
            nome: nome,
            passwordHash: passwordHash,
            role: UserRole.Admin,
            tenant: tenant
        );

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(email.ToLowerInvariant());
        user.Nome.Should().Be(nome);
        user.AuthType.Should().Be(AuthType.Local);
        user.PasswordHash.Should().Be(passwordHash);
        user.AzureAdId.Should().BeNull();
        user.Ativo.Should().BeTrue();
    }

    [Fact]
    public void CriarComSenha_DeveLancarExcecao_QuandoPasswordHashVazio()
    {
        // Arrange
        var tenant = CriarTenantAtivo();

        // Act
        var act = () => User.CriarComSenha(
            tenantId: tenant.Id,
            email: "usuario@empresa.com",
            nome: "João Silva",
            passwordHash: "",
            role: UserRole.Viewer,
            tenant: tenant
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Hash de senha é obrigatório para autenticação local.");
    }

    #endregion

    #region Ativar/Desativar Tests

    [Fact]
    public void Ativar_DeveAtivarUsuario()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );
        user.Desativar();

        // Act
        user.Ativar();

        // Assert
        user.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Desativar_DeveDesativarUsuario()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        user.Desativar();

        // Assert
        user.Ativo.Should().BeFalse();
    }

    #endregion

    #region RegistrarLogin Tests

    [Fact]
    public void RegistrarLogin_DeveAtualizarUltimoLogin()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        user.RegistrarLogin();

        // Assert
        user.UltimoLogin.Should().NotBeNull();
        user.UltimoLogin.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region AtualizarRole Tests

    [Fact]
    public void AtualizarRole_DeveAlterarRole_QuandoAdminAltera()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var admin = User.CriarComSenha(
            tenant.Id,
            "admin@empresa.com",
            "Admin User",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Admin,
            tenant
        );
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        user.AtualizarRole(UserRole.Recruiter, admin);

        // Assert
        user.Role.Should().Be(UserRole.Recruiter);
    }

    [Fact]
    public void AtualizarRole_DeveLancarExcecao_QuandoNaoAdminTentaAlterar()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user1 = User.CriarComSenha(
            tenant.Id,
            "user1@empresa.com",
            "User 1",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );
        var user2 = User.CriarComSenha(
            tenant.Id,
            "user2@empresa.com",
            "User 2",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        var act = () => user2.AtualizarRole(UserRole.Admin, user1);

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Apenas Admin pode alterar roles.");
    }

    [Fact]
    public void AtualizarRole_DeveLancarExcecao_QuandoTentaAlterarPropiaRole()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var admin = User.CriarComSenha(
            tenant.Id,
            "admin@empresa.com",
            "Admin User",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Admin,
            tenant
        );

        // Act
        var act = () => admin.AtualizarRole(UserRole.Viewer, admin);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Usuário não pode alterar sua própria role.");
    }

    #endregion

    #region AtualizarNome Tests

    [Fact]
    public void AtualizarNome_DeveAlterarNome()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        user.AtualizarNome("João Pedro Silva");

        // Assert
        user.Nome.Should().Be("João Pedro Silva");
    }

    [Fact]
    public void AtualizarNome_DeveLancarExcecao_QuandoNomeVazio()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        var act = () => user.AtualizarNome("");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Nome é obrigatório.");
    }

    #endregion

    #region ValidatePassword Tests

    [Fact]
    public void ValidatePassword_DeveRetornarTrue_QuandoSenhaCorreta()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var senha = "Senha@123";
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword(senha),
            UserRole.Viewer,
            tenant
        );

        // Act
        var result = user.ValidatePassword(senha);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidatePassword_DeveRetornarFalse_QuandoSenhaIncorreta()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        var result = user.ValidatePassword("SenhaErrada");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidatePassword_DeveLancarExcecao_QuandoUsuarioAzureAd()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComAzureAd(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            UserRole.Viewer,
            tenant
        );

        // Act
        var act = () => user.ValidatePassword("qualquersenha");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Usuário não utiliza autenticação local.");
    }

    #endregion

    #region ValidarAcesso Tests

    [Fact]
    public void ValidarAcesso_DevePermitirAcesso_QuandoUsuarioAtivoEMesmoTenant()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        var act = () => user.ValidarAcesso(tenant.Id);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidarAcesso_DeveLancarExcecao_QuandoTenantDiferente()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );

        // Act
        var act = () => user.ValidarAcesso(Guid.NewGuid());

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Usuário não pertence a este tenant.");
    }

    [Fact]
    public void ValidarAcesso_DeveLancarExcecao_QuandoUsuarioInativo()
    {
        // Arrange
        var tenant = CriarTenantAtivo();
        var user = User.CriarComSenha(
            tenant.Id,
            "usuario@empresa.com",
            "João Silva",
            BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            UserRole.Viewer,
            tenant
        );
        user.Desativar();

        // Act
        var act = () => user.ValidarAcesso(tenant.Id);

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Usuário está inativo.");
    }

    #endregion
}
