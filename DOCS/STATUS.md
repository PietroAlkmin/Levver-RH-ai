# 📖 README - Documentação Completa do Projeto

## 📚 Índice de Documentos

Esta pasta contém toda a documentação técnica do projeto **Levver.ai RH**.

### **Documentos Principais**

| # | Documento | Descrição | Link |
|---|-----------|-----------|------|
| 00 | **Visão Geral** | Overview completo do projeto, arquitetura geral, stack tecnológica | [00-VISAO-GERAL.md](./00-VISAO-GERAL.md) |
| 01 | **Arquitetura Backend** | Clean Architecture, camadas, entidades, repositories, services | [01-ARQUITETURA-BACKEND.md](./01-ARQUITETURA-BACKEND.md) |
| 02 | **Arquitetura Frontend** | React, TypeScript, feature-based architecture, componentes | [02-ARQUITETURA-FRONTEND.md](./02-ARQUITETURA-FRONTEND.md) |
| 03 | **Autenticação** | Fluxos de login (Email/Senha, Azure AD SSO), JWT, segurança | [03-AUTENTICACAO.md](./03-AUTENTICACAO.md) |
| 04 | **Banco de Dados** | Schema-based multi-tenancy, tabelas, migrations, queries | [04-BANCO-DE-DADOS.md](./04-BANCO-DE-DADOS.md) |
| 05 | **Design System** | Cores, tipografia, componentes, Levver.ai brand guidelines | [05-DESIGN-SYSTEM.md](./05-DESIGN-SYSTEM.md) |
| 06 | **Roadmap** | Próximos passos, features planejadas, timeline, bugs conhecidos | [06-ROADMAP.md](./06-ROADMAP.md) |

---

## 🎯 Como Usar Esta Documentação

### **Para Novos Desenvolvedores**

Leia os documentos nesta ordem:

1. **00-VISAO-GERAL.md** - Entenda o propósito e arquitetura geral
2. **01-ARQUITETURA-BACKEND.md** - Compreenda a estrutura do backend
3. **02-ARQUITETURA-FRONTEND.md** - Compreenda a estrutura do frontend
4. **03-AUTENTICACAO.md** - Entenda os fluxos de autenticação
5. **04-BANCO-DE-DADOS.md** - Conheça o modelo de dados
6. **05-DESIGN-SYSTEM.md** - Aprenda as regras visuais
7. **06-ROADMAP.md** - Veja o que está por vir

### **Para Implementar Features**

- Consulte **01-ARQUITETURA-BACKEND.md** para criar novas entidades/services
- Consulte **02-ARQUITETURA-FRONTEND.md** para criar novos componentes/pages
- Consulte **04-BANCO-DE-DADOS.md** para criar novas tabelas/migrations
- Consulte **05-DESIGN-SYSTEM.md** para manter consistência visual

### **Para Debugging**

- Veja **06-ROADMAP.md** para bugs conhecidos e soluções
- Veja **03-AUTENTICACAO.md** para problemas de login/token
- Veja **04-BANCO-DE-DADOS.md** para problemas de queries

---

## 🚀 Quick Start

### **1. Clone e Configure**

```bash
# Clone o repositório
git clone https://github.com/PietroAlkmin/Levver-RH-ai.git
cd Levver-RH-ai

# Checkout na branch de desenvolvimento
git checkout feat/logica-produto
```

### **2. Backend (.NET 8)**

```bash
# Navegar para o projeto WebApp
cd LevverRH.WebApp

# Restaurar pacotes
dotnet restore

# Configurar connection string
# Editar appsettings.Development.json
# {
#   "ConnectionStrings": {
#     "DefaultConnection": "Server=seu-servidor.database.windows.net;Database=levver.ai-RH-DEV;..."
#   }
# }

# Aplicar migrations
dotnet ef database update --project ../LevverRH.Infra.Data

# Rodar backend
dotnet run
```

Backend rodando em: `http://localhost:5113`

### **3. Frontend (React + Vite)**

```bash
# Navegar para o projeto Frontend
cd LevverRH.Frontend

# Instalar dependências
npm install

# Configurar .env
# VITE_API_URL=http://localhost:5113/api
# VITE_AZURE_AD_CLIENT_ID=seu-client-id
# VITE_AZURE_AD_TENANT_ID=seu-tenant-id

# Rodar frontend
npm run dev
```

Frontend rodando em: `http://localhost:5173`

---

## 📁 Estrutura do Projeto

```
Levver-RH-ai/
├── DOCS/                           # 📚 Esta documentação
│   ├── 00-VISAO-GERAL.md
│   ├── 01-ARQUITETURA-BACKEND.md
│   ├── 02-ARQUITETURA-FRONTEND.md
│   ├── 03-AUTENTICACAO.md
│   ├── 04-BANCO-DE-DADOS.md
│   ├── 05-DESIGN-SYSTEM.md
│   └── 06-ROADMAP.md
│
├── LevverRH.Domain/                # 🏛️ Camada de Domínio
├── LevverRH.Application/           # 📋 Camada de Aplicação
├── LevverRH.Infra.Data/            # 💾 Camada de Infraestrutura (EF Core)
├── LevverRH.Infra.IoC/             # 💉 Injeção de Dependências
├── LevverRH.WebApp/                # 🌐 API (Controllers)
└── LevverRH.Frontend/              # ⚛️ React App
    ├── src/
    │   ├── features/               # Módulos por feature
    │   ├── components/             # Componentes reutilizáveis
    │   ├── hooks/                  # Custom hooks
    │   ├── services/               # API clients
    │   ├── stores/                 # Zustand state
    │   ├── routes/                 # Configuração de rotas
    │   ├── styles/                 # Design System
    │   └── types/                  # TypeScript types
    └── public/
```

---

## 🔑 Conceitos-Chave

### **Multi-Tenancy**
Cada empresa (tenant) tem seus dados isolados em schemas separados no banco de dados (`tenant_[GUID].*`).

### **Multi-Produto**
A plataforma oferece um catálogo de produtos (MST, Ponto, Performance, etc.) que os tenants podem contratar.

### **Clean Architecture**
O backend segue os princípios da Clean Architecture:
- **Domain** (regras de negócio)
- **Application** (casos de uso)
- **Infrastructure** (persistência, integrações)
- **Presentation** (API)

### **Feature-Based Frontend**
Cada produto/módulo fica em sua própria pasta `features/[nome]` com componentes, pages, services e types isolados.

---

## 🛠️ Tecnologias Utilizadas

### **Backend**
- .NET 8 (LTS)
- ASP.NET Core Web API
- Entity Framework Core 8.0.21
- AutoMapper
- FluentValidation
- JWT Bearer Authentication

### **Frontend**
- React 19
- TypeScript 5.7
- Vite 6.0
- Zustand (state management)
- React Router 6
- Axios

### **Database**
- Azure SQL Server
- Schema-based Multi-tenancy

### **Cloud**
- Azure App Service
- Azure Static Web Apps
- Azure Blob Storage
- Azure Application Insights

---

## 📞 Contatos e Suporte

### **Time de Desenvolvimento**
- **Tech Lead**: Pietro Alkmin
- **Repository**: [github.com/PietroAlkmin/Levver-RH-ai](https://github.com/PietroAlkmin/Levver-RH-ai)
- **Branch Principal**: `feat/logica-produto`

### **Recursos Úteis**
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [React Documentation](https://react.dev/)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [Vite Documentation](https://vitejs.dev/)
- [Zustand Documentation](https://zustand-demo.pmnd.rs/)

---

## 📝 Convenções de Código

### **Backend (C#)**
- PascalCase para classes, métodos, propriedades
- camelCase para variáveis locais e parâmetros
- Prefixo `I` para interfaces
- Sufixo `DTO` para Data Transfer Objects
- Sufixo `Service` para services
- Sufixo `Repository` para repositories

### **Frontend (TypeScript/React)**
- PascalCase para componentes React
- camelCase para variáveis, funções, props
- SCREAMING_SNAKE_CASE para constantes
- Prefixo `use` para custom hooks
- Prefixo `I` para interfaces (ex: `IProductCardProps`)
- Sufixo `Props` para props de componentes

### **Git Commits**
```bash
# Formato: <tipo>: <descrição>

feat: adiciona login com Azure AD
fix: corrige redirect loop após login
docs: atualiza documentação de autenticação
refactor: reorganiza estrutura de pastas
style: ajusta cores do Design System
test: adiciona testes para ProductService
chore: atualiza dependências do projeto
```

---

## 🧪 Testes

### **Backend (xUnit)**
```bash
cd LevverRH.Tests
dotnet test
```

### **Frontend (Vitest)**
```bash
cd LevverRH.Frontend
npm run test
```

---

## 🚀 Deploy

### **Backend (Azure App Service)**
```bash
# Build
dotnet publish -c Release -o ./publish

# Deploy via Azure CLI
az webapp deployment source config-zip \
  --resource-group levver-rg \
  --name levver-api \
  --src ./publish.zip
```

### **Frontend (Azure Static Web Apps)**
```bash
# Build
npm run build

# Deploy via Azure CLI
az staticwebapp deploy \
  --name levver-frontend \
  --source ./dist
```

---

## 📊 Status do Projeto

**Versão Atual**: 1.0.0-beta  
**Última Atualização**: 30 de Novembro de 2025  
**Status**: 🟢 Levver Talents Completo - Pronto para Testes  

### **Progresso**

```
Backend:     ██████████████████████ 100% ✅
Frontend:    ███████████████████░░░ 95% ✅
Database:    ██████████████████████ 100% ✅
Docs:        ██████████████████████ 100% ✅
IA Features: ██████████████████████ 100% ✅ (OpenAI integrado)
Talents:     ██████████████████████ 100% 🚀 COMPLETO
Testes:      ████░░░░░░░░░░░░░░░░░░ 20%
```

### **Features Implementadas (Novembro 2025)**

✅ **Criação de Vagas com IA**
- Chat conversacional com GPT-4o-mini
- Extração automática de campos
- Edição manual com contexto preservado
- Indicador de progresso visual

✅ **Análise de Currículos com IA**
- Extração de texto de PDF (PdfPig)
- Análise comparativa com GPT-4o
- Scoring automático (0-100)
- Justificativa detalhada
- Rastreamento de custos

✅ **Aplicação Pública de Candidatos**
- Formulário sem autenticação
- Upload de currículo
- Criação automática de conta
- Auto-login pós-aplicação

---

**Última Atualização**: 30 de Novembro de 2025  
**Mantido por**: Pietro Alkmin
