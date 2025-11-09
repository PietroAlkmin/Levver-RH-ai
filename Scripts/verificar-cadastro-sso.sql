-- ========================================
-- Script: Verificar Dados SSO Completo
-- Propósito: Validar cadastro via SSO após finalização
-- Data: 2025-11-09
-- ========================================

USE [levver.ai-RH-DEV];
GO

PRINT '';
PRINT '🔍🔍🔍 VERIFICAÇÃO COMPLETA DO CADASTRO SSO 🔍🔍🔍';
PRINT '==================================================';
PRINT '';

-- ========================================
-- 1️⃣ VERIFICAR TENANTS
-- ========================================
PRINT '📋 1. DADOS DOS TENANTS:';
PRINT '------------------------';
SELECT 
    Id,
    Nome,
    Cnpj,
    Email,
    Dominio,
    Status,
    Telefone,
    Endereco,
    TenantIdMicrosoft,
    DataCriacao,
    DataAtualizacao
FROM [shared].[tenants];

PRINT '';

DECLARE @TotalTenantsCount INT = (SELECT COUNT(*) FROM [shared].[tenants]);
PRINT '✅ Total de tenants: ' + CAST(@TotalTenantsCount AS VARCHAR);

PRINT '';

-- Validações
IF EXISTS (SELECT 1 FROM [shared].[tenants] WHERE Status = 'Ativo')
    PRINT '✅ Tenant com status ATIVO encontrado!';
ELSE IF EXISTS (SELECT 1 FROM [shared].[tenants] WHERE Status = 'PendenteSetup')
    PRINT '⚠️ Tenant ainda está PENDENTE SETUP!';
ELSE
    PRINT '❌ Nenhum tenant encontrado!';

PRINT '';

-- ========================================
-- 2️⃣ VERIFICAR USUÁRIOS
-- ========================================
PRINT '👤 2. DADOS DOS USUÁRIOS:';
PRINT '-------------------------';
SELECT 
    u.Id,
    u.Nome,
    u.Email,
    u.Role,
    u.auth_type AS AuthType,
    u.Ativo,
    u.azure_ad_id AS AzureAdId,
    u.TenantId,
    t.Nome AS TenantNome,
    t.Status AS TenantStatus,
    u.DataCriacao,
    u.UltimoLogin
FROM [shared].[users] u
INNER JOIN [shared].[tenants] t ON u.TenantId = t.Id;

PRINT '';

DECLARE @TotalUsersCount INT = (SELECT COUNT(*) FROM [shared].[users]);
PRINT '✅ Total de usuários: ' + CAST(@TotalUsersCount AS VARCHAR);

PRINT '';

-- Validações
IF EXISTS (SELECT 1 FROM [shared].[users] WHERE Role = 1 AND auth_type = 2)
    PRINT '✅ Usuário ADMIN com autenticação AZURE AD encontrado!';
ELSE
    PRINT '❌ Usuário Admin SSO não encontrado!';

PRINT '';

-- ========================================
-- 3️⃣ VERIFICAR CONSISTÊNCIA
-- ========================================
PRINT '🔗 3. VERIFICAÇÕES DE CONSISTÊNCIA:';
PRINT '------------------------------------';

-- Verificar se todos os usuários têm tenant válido
DECLARE @UsersWithoutTenant INT = (SELECT COUNT(*) FROM [shared].[users] WHERE TenantId NOT IN (SELECT Id FROM [shared].[tenants]));
IF @UsersWithoutTenant = 0
    PRINT '✅ Todos os usuários têm tenant válido';
ELSE
    PRINT '❌ Existem ' + CAST(@UsersWithoutTenant AS VARCHAR) + ' usuários sem tenant válido!';

-- Verificar se tenant tem domínio preenchido
IF EXISTS (SELECT 1 FROM [shared].[tenants] WHERE Dominio IS NOT NULL AND Dominio != '')
    PRINT '✅ Tenant possui domínio configurado';
ELSE
    PRINT '⚠️ Tenant sem domínio!';

-- Verificar se tenant tem CNPJ após finalização
IF EXISTS (SELECT 1 FROM [shared].[tenants] WHERE Status = 'Ativo' AND (Cnpj IS NULL OR Cnpj = ''))
    PRINT '❌ Tenant ATIVO sem CNPJ!';
ELSE IF EXISTS (SELECT 1 FROM [shared].[tenants] WHERE Status = 'Ativo' AND Cnpj IS NOT NULL)
    PRINT '✅ Tenant ATIVO possui CNPJ';

-- Verificar se tenant tem nome após finalização
IF EXISTS (SELECT 1 FROM [shared].[tenants] WHERE Status = 'Ativo' AND (Nome IS NULL OR Nome = ''))
    PRINT '❌ Tenant ATIVO sem Nome!';
ELSE IF EXISTS (SELECT 1 FROM [shared].[tenants] WHERE Status = 'Ativo' AND Nome IS NOT NULL)
    PRINT '✅ Tenant ATIVO possui Nome';

PRINT '';

-- ========================================
-- 4️⃣ RESUMO GERAL
-- ========================================
PRINT '📊 4. RESUMO GERAL:';
PRINT '-------------------';

DECLARE @TotalTenants INT = (SELECT COUNT(*) FROM [shared].[tenants]);
DECLARE @TotalUsers INT = (SELECT COUNT(*) FROM [shared].[users]);
DECLARE @TenantsPendenteSetup INT = (SELECT COUNT(*) FROM [shared].[tenants] WHERE Status = 'PendenteSetup');
DECLARE @TenantsAtivos INT = (SELECT COUNT(*) FROM [shared].[tenants] WHERE Status = 'Ativo');
DECLARE @AdminsSSO INT = (SELECT COUNT(*) FROM [shared].[users] WHERE Role = 1 AND auth_type = 2);

PRINT 'Total de Tenants: ' + CAST(@TotalTenants AS VARCHAR);
PRINT 'Tenants Ativos: ' + CAST(@TenantsAtivos AS VARCHAR);
PRINT 'Tenants Pendente Setup: ' + CAST(@TenantsPendenteSetup AS VARCHAR);
PRINT 'Total de Usuários: ' + CAST(@TotalUsers AS VARCHAR);
PRINT 'Admins SSO: ' + CAST(@AdminsSSO AS VARCHAR);

PRINT '';

-- Verificação final
IF @TenantsAtivos > 0 AND @AdminsSSO > 0
BEGIN
    PRINT '✅✅✅ CADASTRO SSO COMPLETO E VÁLIDO! ✅✅✅';
    PRINT '';
    PRINT 'Status: Tenant ATIVO com Admin SSO configurado';
    PRINT 'Próximo passo: Testar login de segundo usuário do mesmo domínio';
END
ELSE IF @TenantsPendenteSetup > 0 AND @AdminsSSO > 0
BEGIN
    PRINT '⚠️⚠️⚠️ CADASTRO INCOMPLETO! ⚠️⚠️⚠️';
    PRINT '';
    PRINT 'Status: Tenant criado mas ainda PENDENTE SETUP';
    PRINT 'Ação necessária: Admin deve completar cadastro da empresa';
END
ELSE
BEGIN
    PRINT '❌❌❌ CADASTRO NÃO ENCONTRADO! ❌❌❌';
    PRINT '';
    PRINT 'Verifique se o login SSO foi realizado';
END

PRINT '';
PRINT '==================================================';
PRINT '';

GO
