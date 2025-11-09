-- ========================================
-- Script: Limpar TODOS os dados
-- Propósito: Remover todos os registros de TODAS as tabelas
-- Data: 2025-11-09
-- ========================================

USE [levver.ai-RH-DEV];
GO

PRINT '🧹 Iniciando limpeza do banco de dados...';
GO

-- Desabilitar verificação de FK temporariamente
PRINT '📌 Desabilitando constraints...';
ALTER TABLE [shared].[users] NOCHECK CONSTRAINT ALL;
ALTER TABLE [shared].[tenants] NOCHECK CONSTRAINT ALL;
ALTER TABLE [shared].[white_label] NOCHECK CONSTRAINT ALL;
ALTER TABLE [shared].[audit_logs] NOCHECK CONSTRAINT ALL;
ALTER TABLE [shared].[integration_credentials] NOCHECK CONSTRAINT ALL;
ALTER TABLE [shared].[tenant_subscriptions] NOCHECK CONSTRAINT ALL;
ALTER TABLE [shared].[products_catalog] NOCHECK CONSTRAINT ALL;
GO

-- Deletar todos os registros
PRINT '🗑️ Deletando registros...';
DELETE FROM [shared].[audit_logs];
DELETE FROM [shared].[integration_credentials];
DELETE FROM [shared].[tenant_subscriptions];
DELETE FROM [shared].[users];
DELETE FROM [shared].[white_label];
DELETE FROM [shared].[tenants];
DELETE FROM [shared].[products_catalog];
GO

-- Reabilitar verificação de FK
PRINT '✅ Reabilitando constraints...';
ALTER TABLE [shared].[users] WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE [shared].[tenants] WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE [shared].[white_label] WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE [shared].[audit_logs] WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE [shared].[integration_credentials] WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE [shared].[tenant_subscriptions] WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE [shared].[products_catalog] WITH CHECK CHECK CONSTRAINT ALL;
GO

-- Verificar resultados
PRINT '';
PRINT '📊 Resultado da limpeza:';
PRINT '========================';
SELECT 'tenants' AS Tabela, COUNT(*) AS Total FROM [shared].[tenants]
UNION ALL
SELECT 'users', COUNT(*) FROM [shared].[users]
UNION ALL
SELECT 'white_label', COUNT(*) FROM [shared].[white_label]
UNION ALL
SELECT 'audit_logs', COUNT(*) FROM [shared].[audit_logs]
UNION ALL
SELECT 'integration_credentials', COUNT(*) FROM [shared].[integration_credentials]
UNION ALL
SELECT 'tenant_subscriptions', COUNT(*) FROM [shared].[tenant_subscriptions]
UNION ALL
SELECT 'products_catalog', COUNT(*) FROM [shared].[products_catalog];
GO

PRINT '';
PRINT '✅✅✅ Banco de dados limpo com sucesso! ✅✅✅';
PRINT '';
GO
