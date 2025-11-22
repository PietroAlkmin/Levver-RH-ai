using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevverRH.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedLevverWhiteLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Criar WhiteLabel da Levver (tenant já existe)
            migrationBuilder.Sql(@"
                DECLARE @LevverTenantId UNIQUEIDENTIFIER
                SELECT @LevverTenantId = Id FROM Tenants WHERE Nome = 'Levver'

                IF @LevverTenantId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM WhiteLabels WHERE TenantId = @LevverTenantId)
                BEGIN
                    INSERT INTO WhiteLabels (
                        TenantId,
                        LogoUrl,
                        PrimaryColor,
                        SecondaryColor,
                        AccentColor,
                        BackgroundColor,
                        TextColor,
                        BorderColor,
                        SystemName,
                        FaviconUrl,
                        DominioCustomizado,
                        EmailRodape,
                        Active,
                        CreatedAt,
                        UpdatedAt
                    )
                    VALUES (
                        @LevverTenantId,
                        'https://levverstorage.blob.core.windows.net/logos/levver-logo',
                        '#713BDB', -- Roxo principal
                        '#CC12EF', -- Roxo gradiente
                        '#F5F7FA', -- Cinza claro
                        '#FFFFFF', -- Branco
                        '#000000', -- Preto
                        '#713BDB', -- Roxo (border usa mesma cor primary)
                        'Levver.ai',
                        'https://levverstorage.blob.core.windows.net/favicons/levver-favicon',
                        'levver.ai',
                        'contato@levver.ai',
                        1, -- Active
                        GETUTCDATE(),
                        GETUTCDATE()
                    )
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @LevverTenantId UNIQUEIDENTIFIER
                SELECT @LevverTenantId = Id FROM Tenants WHERE Nome = 'Levver'
                
                IF @LevverTenantId IS NOT NULL
                BEGIN
                    DELETE FROM WhiteLabels WHERE TenantId = @LevverTenantId
                END
            ");
        }
    }
}
