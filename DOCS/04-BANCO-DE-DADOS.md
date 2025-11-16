# 🗄️ Modelo de Dados e Banco de Dados - Levver.ai RH

## 🎯 Estratégia: Schema-based Multi-tenancy

### **Por que Schema-based?**

✅ **Isolamento Total**: Cada tenant tem seu próprio schema SQL  
✅ **Segurança**: Impossível acessar dados de outro tenant via query injection  
✅ **Backup/Restore por Tenant**: Fácil migrar ou restaurar dados de um cliente específico  
✅ **Escalabilidade**: No futuro, pode mover schema para outro servidor  
✅ **Compliance**: LGPD/GDPR - dados separados fisicamente  

### **Alternativas Descartadas**

❌ **Database-per-Tenant**: Custo alto, difícil manutenção  
❌ **Row-level Isolation**: Risco de vazamento de dados, queries complexas  

---

## 📊 Estrutura de Schemas

```sql
-- SCHEMA COMPARTILHADO (shared.*)
-- Tabelas globais - dados de todos os tenants
shared.tenants
shared.users
shared.products_catalog
shared.tenant_products
shared.tenant_subscriptions
shared.white_label
shared.integration_credentials
shared.audit_logs

-- SCHEMA POR TENANT (tenant_[GUID].*)
-- Dados isolados por tenant
tenant_12345678-1234-1234-1234-123456789abc.candidatos
tenant_12345678-1234-1234-1234-123456789abc.vagas
tenant_12345678-1234-1234-1234-123456789abc.processos_seletivos

tenant_87654321-4321-4321-4321-cba987654321.candidatos
tenant_87654321-4321-4321-4321-cba987654321.vagas
tenant_87654321-4321-4321-4321-cba987654321.processos_seletivos
```

---

## 🗂️ Tabelas do Schema Compartilhado (shared.*)

### **1. shared.tenants**

Armazena informações das empresas cadastradas.

```sql
CREATE TABLE shared.tenants (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nome_empresa NVARCHAR(200) NOT NULL,
    cnpj NVARCHAR(18) UNIQUE,
    email_empresa NVARCHAR(100),
    telefone_empresa NVARCHAR(20),
    endereco_empresa NVARCHAR(300),
    status NVARCHAR(20) NOT NULL DEFAULT 'Ativo',  -- Ativo, Inativo, Suspenso, PendenteSetup
    schema_name NVARCHAR(100) NOT NULL,  -- tenant_12345678-1234-1234...
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT CK_tenant_status CHECK (status IN ('Ativo', 'Inativo', 'Suspenso', 'PendenteSetup'))
);

CREATE INDEX IX_tenants_status ON shared.tenants(status);
CREATE INDEX IX_tenants_cnpj ON shared.tenants(cnpj);
```

**Exemplo de Dados:**
```sql
INSERT INTO shared.tenants (id, nome_empresa, cnpj, status, schema_name)
VALUES 
    ('12345678-1234-1234-1234-123456789abc', 'Empresa ABC LTDA', '12.345.678/0001-90', 'Ativo', 'tenant_12345678-1234-1234-1234-123456789abc'),
    ('87654321-4321-4321-4321-cba987654321', 'Tech Solutions', '98.765.432/0001-10', 'Ativo', 'tenant_87654321-4321-4321-4321-cba987654321');
```

---

### **2. shared.users**

Armazena todos os usuários do sistema.

```sql
CREATE TABLE shared.users (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    nome NVARCHAR(150) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    password_hash NVARCHAR(255),  -- NULL se AuthType = AzureAd
    role NVARCHAR(20) NOT NULL DEFAULT 'User',  -- Admin, Manager, User
    auth_type NVARCHAR(20) NOT NULL DEFAULT 'EmailSenha',  -- EmailSenha, AzureAd
    azure_ad_object_id NVARCHAR(100),  -- ID do usuário no Azure AD
    ativo BIT NOT NULL DEFAULT 1,
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_users_tenant FOREIGN KEY (tenant_id) REFERENCES shared.tenants(id) ON DELETE CASCADE,
    CONSTRAINT CK_user_role CHECK (role IN ('Admin', 'Manager', 'User')),
    CONSTRAINT CK_user_auth_type CHECK (auth_type IN ('EmailSenha', 'AzureAd')),
    CONSTRAINT UQ_user_email_tenant UNIQUE (email, tenant_id)
);

CREATE INDEX IX_users_tenant_id ON shared.users(tenant_id);
CREATE INDEX IX_users_email ON shared.users(email);
CREATE INDEX IX_users_azure_ad_object_id ON shared.users(azure_ad_object_id);
```

**Exemplo de Dados:**
```sql
INSERT INTO shared.users (id, tenant_id, nome, email, password_hash, role, auth_type)
VALUES 
    (NEWID(), '12345678-1234-1234-1234-123456789abc', 'Admin User', 'admin@empresa.com', '$2a$12$...', 'Admin', 'EmailSenha'),
    (NEWID(), '12345678-1234-1234-1234-123456789abc', 'João Silva', 'joao@empresa.com', NULL, 'User', 'AzureAd');
```

---

### **3. shared.products_catalog**

Catálogo global de produtos disponíveis.

```sql
CREATE TABLE shared.products_catalog (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    produto_nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    categoria NVARCHAR(50),  -- Recrutamento, Ponto, Performance, etc.
    icone NVARCHAR(50),  -- Emoji ou classe CSS
    cor_primaria NVARCHAR(7),  -- Hex color #A417D0
    rota_base NVARCHAR(100),  -- /mst, /ponto, etc.
    ordem_exibicao INT NOT NULL DEFAULT 0,
    lancado BIT NOT NULL DEFAULT 0,  -- 0 = Em Breve, 1 = Disponível
    ModeloCobranca NVARCHAR(20) NOT NULL DEFAULT 'Mensal',  -- Mensal, Anual, Unico, Uso
    ValorBasePadrao DECIMAL(10,2) NOT NULL DEFAULT 0,
    Ativo BIT NOT NULL DEFAULT 1,
    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT CK_product_modelo_cobranca CHECK (ModeloCobranca IN ('Mensal', 'Anual', 'Unico', 'Uso'))
);

CREATE INDEX IX_products_catalog_lancado ON shared.products_catalog(lancado);
CREATE INDEX IX_products_catalog_ordem ON shared.products_catalog(ordem_exibicao);
```

**Exemplo de Dados:**
```sql
INSERT INTO shared.products_catalog (id, produto_nome, descricao, categoria, icone, cor_primaria, rota_base, ordem_exibicao, lancado, ModeloCobranca, ValorBasePadrao)
VALUES 
    (NEWID(), 'Levver MST', 'Multi-Sourcing de Talentos', 'Recrutamento', '🎯', '#A417D0', '/mst', 1, 1, 'Mensal', 299.90),
    (NEWID(), 'Levver Ponto', 'Controle de Ponto Eletrônico', 'Ponto', '⏰', '#11005D', '/ponto', 2, 0, 'Mensal', 199.90),
    (NEWID(), 'Levver Performance', 'Avaliação de Desempenho', 'Performance', '📊', '#D4C2F5', '/performance', 3, 0, 'Mensal', 249.90);
```

---

### **4. shared.tenant_products**

Relaciona quais produtos cada tenant contratou (N:N com propriedades).

```sql
CREATE TABLE shared.tenant_products (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    product_id UNIQUEIDENTIFIER NOT NULL,
    ativo BIT NOT NULL DEFAULT 1,
    data_ativacao DATETIME2,
    data_desativacao DATETIME2,
    configuracao_json NVARCHAR(MAX),  -- Configurações específicas em JSON
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_tenant_products_tenant FOREIGN KEY (tenant_id) REFERENCES shared.tenants(id) ON DELETE CASCADE,
    CONSTRAINT FK_tenant_products_product FOREIGN KEY (product_id) REFERENCES shared.products_catalog(id) ON DELETE RESTRICT,
    CONSTRAINT UQ_tenant_product UNIQUE (tenant_id, product_id)
);

CREATE INDEX IX_tenant_products_tenant_id ON shared.tenant_products(tenant_id);
CREATE INDEX IX_tenant_products_product_id ON shared.tenant_products(product_id);
CREATE INDEX IX_tenant_products_ativo ON shared.tenant_products(ativo);
```

**Exemplo de Dados:**
```sql
-- Empresa ABC contratou Levver MST e Levver Ponto
INSERT INTO shared.tenant_products (id, tenant_id, product_id, ativo, data_ativacao)
VALUES 
    (NEWID(), '12345678-1234-1234-1234-123456789abc', 'product-mst-guid', 1, GETUTCDATE()),
    (NEWID(), '12345678-1234-1234-1234-123456789abc', 'product-ponto-guid', 1, GETUTCDATE());
```

---

### **5. shared.tenant_subscriptions**

Controla assinaturas e cobranças.

```sql
CREATE TABLE shared.tenant_subscriptions (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    product_id UNIQUEIDENTIFIER NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Ativa',  -- Ativa, Cancelada, Suspensa, Expirada
    data_inicio DATETIME2 NOT NULL,
    data_fim DATETIME2,
    valor_mensal DECIMAL(10,2) NOT NULL,
    forma_pagamento NVARCHAR(50),  -- CartaoCredito, Boleto, Pix
    gateway_subscription_id NVARCHAR(200),  -- ID da assinatura no gateway de pagamento
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_subscriptions_tenant FOREIGN KEY (tenant_id) REFERENCES shared.tenants(id) ON DELETE CASCADE,
    CONSTRAINT FK_subscriptions_product FOREIGN KEY (product_id) REFERENCES shared.products_catalog(id) ON DELETE RESTRICT,
    CONSTRAINT CK_subscription_status CHECK (status IN ('Ativa', 'Cancelada', 'Suspensa', 'Expirada'))
);

CREATE INDEX IX_subscriptions_tenant_id ON shared.tenant_subscriptions(tenant_id);
CREATE INDEX IX_subscriptions_status ON shared.tenant_subscriptions(status);
```

---

### **6. shared.white_label**

Personalização visual por tenant.

```sql
CREATE TABLE shared.white_label (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL UNIQUE,
    primary_color NVARCHAR(7) DEFAULT '#A417D0',
    secondary_color NVARCHAR(7) DEFAULT '#11005D',
    accent_color NVARCHAR(7) DEFAULT '#D4C2F5',
    background_color NVARCHAR(7) DEFAULT '#FBFBFF',
    text_color NVARCHAR(7) DEFAULT '#11005D',
    border_color NVARCHAR(7) DEFAULT '#EAEAF0',
    logo_url NVARCHAR(500),
    favicon_url NVARCHAR(500),
    system_name NVARCHAR(100) DEFAULT 'Levver RH',
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_white_label_tenant FOREIGN KEY (tenant_id) REFERENCES shared.tenants(id) ON DELETE CASCADE
);
```

---

### **7. shared.integration_credentials**

Credenciais de integrações (APIs externas).

```sql
CREATE TABLE shared.integration_credentials (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    integration_name NVARCHAR(100) NOT NULL,  -- Gupy, Kenoby, LinkedIn, etc.
    api_key NVARCHAR(500),
    api_secret NVARCHAR(500),
    webhook_url NVARCHAR(500),
    config_json NVARCHAR(MAX),  -- Outras configurações
    ativo BIT NOT NULL DEFAULT 1,
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_integration_credentials_tenant FOREIGN KEY (tenant_id) REFERENCES shared.tenants(id) ON DELETE CASCADE
);

CREATE INDEX IX_integration_credentials_tenant_id ON shared.integration_credentials(tenant_id);
```

---

### **8. shared.audit_logs**

Logs de auditoria para compliance.

```sql
CREATE TABLE shared.audit_logs (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    user_id UNIQUEIDENTIFIER NOT NULL,
    action NVARCHAR(100) NOT NULL,  -- Login, Logout, CreateUser, UpdateProduct, etc.
    entity_type NVARCHAR(50),  -- User, Tenant, Product, etc.
    entity_id UNIQUEIDENTIFIER,
    old_value NVARCHAR(MAX),  -- JSON antes da alteração
    new_value NVARCHAR(MAX),  -- JSON depois da alteração
    ip_address NVARCHAR(45),
    user_agent NVARCHAR(500),
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_audit_logs_tenant FOREIGN KEY (tenant_id) REFERENCES shared.tenants(id) ON DELETE CASCADE,
    CONSTRAINT FK_audit_logs_user FOREIGN KEY (user_id) REFERENCES shared.users(id) ON DELETE NO ACTION
);

CREATE INDEX IX_audit_logs_tenant_id ON shared.audit_logs(tenant_id);
CREATE INDEX IX_audit_logs_user_id ON shared.audit_logs(user_id);
CREATE INDEX IX_audit_logs_data_criacao ON shared.audit_logs(data_criacao);
```

---

## 🏢 Schemas por Tenant (tenant_[GUID].*)

### **Criação Automática**

Quando um novo tenant é criado:

```csharp
public async Task CreateTenantSchemaAsync(Guid tenantId)
{
    var schemaName = $"tenant_{tenantId:N}";
    
    // 1. Criar schema
    await _context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA [{schemaName}]");
    
    // 2. Criar tabelas do Levver Talents
    await _context.Database.ExecuteSqlRawAsync($@"
        -- Vagas
        CREATE TABLE [{schemaName}].vagas (
            id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
            titulo NVARCHAR(200) NOT NULL,
            descricao NVARCHAR(MAX),
            departamento NVARCHAR(100),
            status NVARCHAR(30) NOT NULL DEFAULT 'Aberta',
            data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE()
        );
        
        -- Candidaturas
        CREATE TABLE [{schemaName}].candidaturas (
            id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
            vaga_id UNIQUEIDENTIFIER NOT NULL,
            nome_candidato NVARCHAR(150) NOT NULL,
            email NVARCHAR(100) NOT NULL,
            status NVARCHAR(50) NOT NULL DEFAULT 'Nova',
            data_candidatura DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_candidatura_vaga FOREIGN KEY (vaga_id) 
                REFERENCES [{schemaName}].vagas(id) ON DELETE CASCADE
        );
        
        -- Entrevistas
        CREATE TABLE [{schemaName}].entrevistas (
            id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
            candidatura_id UNIQUEIDENTIFIER NOT NULL,
            tipo NVARCHAR(30) NOT NULL,
            data_hora DATETIME2 NOT NULL,
            status NVARCHAR(30) NOT NULL DEFAULT 'Agendada',
            data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_entrevista_candidatura FOREIGN KEY (candidatura_id) 
                REFERENCES [{schemaName}].candidaturas(id) ON DELETE CASCADE
        );
        
        -- Avaliações
        CREATE TABLE [{schemaName}].avaliacoes (
            id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
            candidatura_id UNIQUEIDENTIFIER NOT NULL,
            avaliador NVARCHAR(150) NOT NULL,
            nota DECIMAL(3,2),
            comentarios NVARCHAR(MAX),
            data_avaliacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_avaliacao_candidatura FOREIGN KEY (candidatura_id) 
                REFERENCES [{schemaName}].candidaturas(id) ON DELETE CASCADE
        );
        
        -- Etapas
        CREATE TABLE [{schemaName}].etapas (
            id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
            vaga_id UNIQUEIDENTIFIER NOT NULL,
            nome NVARCHAR(100) NOT NULL,
            ordem INT NOT NULL,
            data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_etapa_vaga FOREIGN KEY (vaga_id) 
                REFERENCES [{schemaName}].vagas(id) ON DELETE CASCADE
        );
        
        -- Habilidades
        CREATE TABLE [{schemaName}].habilidades (
            id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
            nome NVARCHAR(100) NOT NULL,
            categoria NVARCHAR(50),
            data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE()
        );
    ");
    
    // 3. Atualizar tenant com schema_name
    var tenant = await _context.Tenants.FindAsync(tenantId);
    tenant.SchemaName = schemaName;
    await _context.SaveChangesAsync();
}
```

### **Exemplo de Tabelas por Tenant**

```sql
-- Schema: tenant_12345678-1234-1234-1234-123456789abc
-- Tabelas do Levver Talents (Recrutamento e Seleção)

-- Vagas de emprego
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.vagas (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    titulo NVARCHAR(200) NOT NULL,
    descricao NVARCHAR(MAX),
    departamento NVARCHAR(100),
    localizacao NVARCHAR(100),
    salario_min DECIMAL(10,2),
    salario_max DECIMAL(10,2),
    tipo_contrato NVARCHAR(50),  -- CLT, PJ, Estágio, Temporário
    status NVARCHAR(30) NOT NULL DEFAULT 'Aberta',  -- Aberta, Fechada, Suspensa, Cancelada
    data_abertura DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_fechamento DATETIME2,
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Candidaturas
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.candidaturas (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    vaga_id UNIQUEIDENTIFIER NOT NULL,
    nome_candidato NVARCHAR(150) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    telefone NVARCHAR(20),
    linkedin_url NVARCHAR(300),
    curriculo_url NVARCHAR(500),
    carta_apresentacao NVARCHAR(MAX),
    status NVARCHAR(50) NOT NULL DEFAULT 'Nova',  -- Nova, EmAnalise, Entrevista, Aprovada, Reprovada
    fonte NVARCHAR(100),  -- LinkedIn, Site, Indicação, etc.
    data_candidatura DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_candidatura_vaga FOREIGN KEY (vaga_id) 
        REFERENCES tenant_12345678-1234-1234-1234-123456789abc.vagas(id) ON DELETE CASCADE
);

-- Entrevistas
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.entrevistas (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    candidatura_id UNIQUEIDENTIFIER NOT NULL,
    tipo NVARCHAR(30) NOT NULL,  -- Presencial, Online, Telefone
    data_hora DATETIME2 NOT NULL,
    duracao_minutos INT DEFAULT 60,
    local_ou_link NVARCHAR(500),
    entrevistador NVARCHAR(150),
    observacoes NVARCHAR(MAX),
    status NVARCHAR(30) NOT NULL DEFAULT 'Agendada',  -- Agendada, Realizada, Cancelada, Remarcada
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    data_atualizacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_entrevista_candidatura FOREIGN KEY (candidatura_id) 
        REFERENCES tenant_12345678-1234-1234-1234-123456789abc.candidaturas(id) ON DELETE CASCADE
);

-- Avaliações de candidatos
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.avaliacoes (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    candidatura_id UNIQUEIDENTIFIER NOT NULL,
    entrevista_id UNIQUEIDENTIFIER,
    avaliador NVARCHAR(150) NOT NULL,
    nota DECIMAL(3,2),  -- 0.00 a 10.00
    comentarios NVARCHAR(MAX),
    pontos_fortes NVARCHAR(MAX),
    pontos_fracos NVARCHAR(MAX),
    recomendacao NVARCHAR(50),  -- Aprovar, Reprovar, Reavaliar
    data_avaliacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_avaliacao_candidatura FOREIGN KEY (candidatura_id) 
        REFERENCES tenant_12345678-1234-1234-1234-123456789abc.candidaturas(id) ON DELETE CASCADE,
    CONSTRAINT FK_avaliacao_entrevista FOREIGN KEY (entrevista_id) 
        REFERENCES tenant_12345678-1234-1234-1234-123456789abc.entrevistas(id)
);

-- Etapas do processo seletivo (pipeline customizável)
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.etapas (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    vaga_id UNIQUEIDENTIFIER NOT NULL,
    nome NVARCHAR(100) NOT NULL,  -- Triagem, Entrevista RH, Entrevista Técnica, etc.
    descricao NVARCHAR(500),
    ordem INT NOT NULL,
    obrigatoria BIT NOT NULL DEFAULT 1,
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_etapa_vaga FOREIGN KEY (vaga_id) 
        REFERENCES tenant_12345678-1234-1234-1234-123456789abc.vagas(id) ON DELETE CASCADE
);

-- Habilidades/competências
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.habilidades (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nome NVARCHAR(100) NOT NULL,
    categoria NVARCHAR(50),  -- Técnica, Comportamental, Idioma
    nivel NVARCHAR(30),  -- Básico, Intermediário, Avançado
    data_criacao DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Relacionamento N:N entre Vagas e Habilidades
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.vaga_habilidades (
    vaga_id UNIQUEIDENTIFIER NOT NULL,
    habilidade_id UNIQUEIDENTIFIER NOT NULL,
    obrigatoria BIT NOT NULL DEFAULT 0,
    
    PRIMARY KEY (vaga_id, habilidade_id),
    CONSTRAINT FK_vaga_habilidade_vaga FOREIGN KEY (vaga_id) 
        REFERENCES tenant_12345678-1234-1234-1234-123456789abc.vagas(id) ON DELETE CASCADE,
    CONSTRAINT FK_vaga_habilidade_habilidade FOREIGN KEY (habilidade_id) 
        REFERENCES tenant_12345678-1234-1234-1234-123456789abc.habilidades(id) ON DELETE CASCADE
);

-- Índices para performance
CREATE INDEX IX_candidaturas_vaga_id ON tenant_12345678-1234-1234-1234-123456789abc.candidaturas(vaga_id);
CREATE INDEX IX_candidaturas_status ON tenant_12345678-1234-1234-1234-123456789abc.candidaturas(status);
CREATE INDEX IX_entrevistas_candidatura_id ON tenant_12345678-1234-1234-1234-123456789abc.entrevistas(candidatura_id);
CREATE INDEX IX_entrevistas_data_hora ON tenant_12345678-1234-1234-1234-123456789abc.entrevistas(data_hora);
CREATE INDEX IX_avaliacoes_candidatura_id ON tenant_12345678-1234-1234-1234-123456789abc.avaliacoes(candidatura_id);
```

---

## 🔍 Queries Multi-Schema

### **Query em Schema Dinâmico**

```csharp
public async Task<IEnumerable<Candidato>> GetCandidatosAsync(Guid tenantId)
{
    var tenant = await _context.Tenants.FindAsync(tenantId);
    var schemaName = tenant.SchemaName;
    
    var sql = $@"
        SELECT id, nome, email, telefone, status, data_criacao
        FROM [{schemaName}].candidatos
        WHERE status = 'EmAnalise'
        ORDER BY data_criacao DESC
    ";
    
    return await _context.Database
        .SqlQueryRaw<Candidato>(sql)
        .ToListAsync();
}
```

### **Query Join entre Shared e Tenant Schema**

```csharp
// Buscar produtos contratados com dados do tenant
var sql = $@"
    SELECT 
        t.nome_empresa,
        p.produto_nome,
        tp.ativo,
        tp.data_ativacao
    FROM shared.tenants t
    INNER JOIN shared.tenant_products tp ON t.id = tp.tenant_id
    INNER JOIN shared.products_catalog p ON tp.product_id = p.id
    WHERE t.id = @tenantId
    AND tp.ativo = 1
";
```

---

## 📈 Diagrama ER (Simplificado)

```
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│  shared.tenants │1──────N │ shared.users    │         │shared.products_ │
│                 │         │                 │         │    catalog      │
│ - id            │         │ - id            │         │                 │
│ - nome_empresa  │         │ - tenant_id (FK)│         │ - id            │
│ - cnpj          │         │ - nome          │         │ - produto_nome  │
│ - status        │         │ - email         │         │ - descricao     │
│ - schema_name   │         │ - password_hash │         │ - lancado       │
└────────┬────────┘         │ - role          │         └────────┬────────┘
         │                  └─────────────────┘                  │
         │                                                        │
         │                  ┌─────────────────┐                  │
         └────────────────N │shared.tenant_   │N─────────────────┘
                            │   products      │
                            │                 │
                            │ - tenant_id(FK) │
                            │ - product_id(FK)│
                            │ - ativo         │
                            │ - config_json   │
                            └─────────────────┘

┌────────────────────────────────────────────────────────────────┐
│          tenant_12345678-1234-1234-1234-123456789abc           │
│                                                                │
│  ┌──────────────┐    ┌──────────────┐    ┌─────────────────┐ │
│  │  candidatos  │    │    vagas     │    │   processos_    │ │
│  │              │    │              │    │   seletivos     │ │
│  │ - id         │    │ - id         │    │                 │ │
│  │ - nome       │    │ - titulo     │    │ - vaga_id (FK)  │ │
│  │ - email      │    │ - descricao  │    │ - candidato_id  │ │
│  │ - status     │    │ - status     │    │ - etapa_atual   │ │
│  └──────────────┘    └──────────────┘    └─────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

---

## 🚀 Migrations

### **Criação de Migrations**

```bash
# Navegar para o projeto de infraestrutura
cd LevverRH.Infra.Data

# Criar migration
dotnet ef migrations add AddTenantProductsTable --startup-project ../LevverRH.WebApp

# Aplicar migration
dotnet ef database update --startup-project ../LevverRH.WebApp
```

### **Migration Example**

```csharp
public partial class AddTenantProductsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "tenant_products",
            schema: "shared",
            columns: table => new
            {
                id = table.Column<Guid>(nullable: false),
                tenant_id = table.Column<Guid>(nullable: false),
                product_id = table.Column<Guid>(nullable: false),
                ativo = table.Column<bool>(nullable: false, defaultValue: true),
                data_ativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                data_desativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                configuracao_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tenant_products", x => x.id);
                table.ForeignKey(
                    name: "FK_tenant_products_tenant",
                    column: x => x.tenant_id,
                    principalSchema: "shared",
                    principalTable: "tenants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_tenant_products_product",
                    column: x => x.product_id,
                    principalSchema: "shared",
                    principalTable: "products_catalog",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_tenant_products_tenant_product",
            schema: "shared",
            table: "tenant_products",
            columns: new[] { "tenant_id", "product_id" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "tenant_products",
            schema: "shared");
    }
}
```

---

## 🔒 Segurança no Banco

### **1. Row-Level Security (RLS) - Futura Implementação**

```sql
-- Criar função de segurança
CREATE FUNCTION dbo.fn_tenantAccessPredicate(@TenantId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_securitypredicate_result
WHERE @TenantId = CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER);

-- Aplicar política
CREATE SECURITY POLICY TenantFilter
ADD FILTER PREDICATE dbo.fn_tenantAccessPredicate(tenant_id) ON shared.users,
ADD BLOCK PREDICATE dbo.fn_tenantAccessPredicate(tenant_id) ON shared.users;
```

### **2. Backup Strategy**

```sql
-- Backup por schema (tenant específico)
BACKUP DATABASE [levver.ai-RH-DEV]
FILEGROUP = 'tenant_12345678-1234-1234-1234-123456789abc'
TO DISK = 'C:\Backups\tenant_12345678_backup.bak'
WITH COMPRESSION;
```

---

**Última Atualização**: 16 de Novembro de 2025
