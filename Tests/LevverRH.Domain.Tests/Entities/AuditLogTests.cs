namespace LevverRH.Domain.Tests.Entities;

[Trait("Category", "Unit")]
[Trait("Entity", "AuditLog")]
public class AuditLogTests
{
    [Fact]
    public void CriarLogLogin_Should_CreateLoginAuditLog()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";

        // Act
        var log = AuditLog.CriarLogLogin(tenantId, userId, ipAddress, userAgent);

        // Assert
        log.Id.Should().NotBeEmpty();
        log.TenantId.Should().Be(tenantId);
        log.UserId.Should().Be(userId);
        log.Acao.Should().Be("user_login");
        log.IpAddress.Should().Be(ipAddress);
        log.UserAgent.Should().Be(userAgent);
        log.DataHora.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarLogLogout_Should_CreateLogoutAuditLog()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";

        // Act
        var log = AuditLog.CriarLogLogout(tenantId, userId, ipAddress);

        // Assert
        log.Id.Should().NotBeEmpty();
        log.TenantId.Should().Be(tenantId);
        log.UserId.Should().Be(userId);
        log.Acao.Should().Be("user_logout");
        log.IpAddress.Should().Be(ipAddress);
        log.DataHora.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarLogMudancaPermissao_Should_CreateRoleChangedAuditLog()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var usuarioAfetadoId = Guid.NewGuid();
        var detalhes = "{\"oldRole\":\"Viewer\",\"newRole\":\"Admin\"}";
        var ipAddress = "192.168.1.1";

        // Act
        var log = AuditLog.CriarLogMudancaPermissao(
            tenantId,
            userId,
            usuarioAfetadoId,
            detalhes,
            ipAddress);

        // Assert
        log.Id.Should().NotBeEmpty();
        log.TenantId.Should().Be(tenantId);
        log.UserId.Should().Be(userId);
        log.Acao.Should().Be("role_changed");
        log.Entidade.Should().Be("user");
        log.EntidadeId.Should().Be(usuarioAfetadoId);
        log.DetalhesJson.Should().Be(detalhes);
        log.IpAddress.Should().Be(ipAddress);
    }

    [Fact]
    public void CriarLogVisualizaçãoCurrículo_Should_CreateCurriculoViewedAuditLog()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";

        // Act
        var log = AuditLog.CriarLogVisualizaçãoCurrículo(
            tenantId,
            userId,
            candidateId,
            ipAddress);

        // Assert
        log.Id.Should().NotBeEmpty();
        log.TenantId.Should().Be(tenantId);
        log.UserId.Should().Be(userId);
        log.Acao.Should().Be("curriculo_visualizado");
        log.Entidade.Should().Be("candidate");
        log.EntidadeId.Should().Be(candidateId);
        log.IpAddress.Should().Be(ipAddress);
    }

    [Fact]
    public void CriarLogDownloadCurrículo_Should_CreateCurriculoDownloadAuditLog()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";

        // Act
        var log = AuditLog.CriarLogDownloadCurrículo(
            tenantId,
            userId,
            candidateId,
            ipAddress);

        // Assert
        log.Id.Should().NotBeEmpty();
        log.TenantId.Should().Be(tenantId);
        log.UserId.Should().Be(userId);
        log.Acao.Should().Be("curriculo_download");
        log.Entidade.Should().Be("candidate");
        log.EntidadeId.Should().Be(candidateId);
        log.IpAddress.Should().Be(ipAddress);
    }
}
