# 🚀 Roadmap e Próximos Passos - Levver.ai RH

## 📋 Estado Atual do Projeto

### ✅ **Completado (MVP 1.0)**

#### **Backend**
- [x] Clean Architecture implementada
- [x] Multi-tenancy com schema isolation
- [x] Autenticação Email/Senha
- [x] Autenticação Azure AD SSO
- [x] JWT Token generation e validation
- [x] Entity Framework Core com Migrations
- [x] Repositórios genéricos e específicos
- [x] DTOs e AutoMapper configurado
- [x] FluentValidation para validações
- [x] Entidades de domínio (User, Tenant, ProductCatalog, TenantProduct)
- [x] API de Produtos (/api/products)
- [x] API de Autenticação (/api/auth)

#### **Frontend**
- [x] React 19 + TypeScript + Vite
- [x] Feature-based architecture
- [x] State management com Zustand
- [x] Rotas protegidas (ProtectedRoute)
- [x] Custom hook useAuth
- [x] API client com Axios + interceptors
- [x] Design System Levver.ai (cores, tipografia, componentes)
- [x] Painel Principal (catálogo de produtos)
- [x] Componente ProductCard
- [x] Página de Login
- [x] Página de Registro de Tenant
- [x] Azure AD Login Button

#### **Database**
- [x] Schema `shared` para tabelas globais
- [x] Migrations aplicadas (InitialCreate, AddTenantProductsTable)
- [x] Tabelas criadas: tenants, users, products_catalog, tenant_products
- [x] Foreign keys e constraints configurados

---

## 🐛 **Bugs Conhecidos (Prioridade Alta)**

### **1. Redirect Loop após Login**
**Status**: 🔴 Em investigação  
**Descrição**: Usuário faz login → redireciona para /painel → volta para /login instantaneamente  
**Possíveis Causas**:
- Token não está sendo salvo corretamente no localStorage
- Zustand persist não está sincronizando
- API retorna 401 em alguma requisição (ex: GET /api/products/my-products)
- ProtectedRoute está verificando estado antes do Zustand hidratar

**Próximos Passos**:
1. Verificar logs do console (implementados para debug)
2. Verificar se token está sendo salvo: `localStorage.getItem('token')`
3. Verificar se API está retornando 401 (checar interceptor)
4. Adicionar delay no ProtectedRoute para aguardar hidratação do Zustand

---

### **2. Produtos não aparecem no Painel**
**Status**: ⚠️ Esperado (sem seed data)  
**Descrição**: Painel mostra "Nenhum produto disponível"  
**Causa**: Não há produtos cadastrados no banco de dados  
**Solução**: Criar seed de produtos iniciais

---

## 🔧 **Tarefas Técnicas Pendentes**

### **Alta Prioridade**

- [ ] **Corrigir redirect loop após login**
  - Adicionar debug logs completos
  - Verificar ordem de execução (login → setAuth → navigate)
  - Testar com localStorage vazio (clear cache)

- [ ] **Criar seed de produtos**
  ```sql
  INSERT INTO shared.products_catalog (...) VALUES
    ('Levver MST', '🎯', '#A417D0', '/mst', 1, 1),
    ('Levver Ponto', '⏰', '#11005D', '/ponto', 2, 0),
    ('Levver Performance', '📊', '#D4C2F5', '/performance', 3, 0);
  ```

- [ ] **Remover componente Dashboard.tsx antigo**
  - Deletar `pages/Dashboard/Dashboard.tsx`
  - Remover imports relacionados

- [ ] **Adicionar validação de tenant ativo em ProtectedRoute**
  - Verificar `tenant.status === 'Ativo'`
  - Redirecionar para página de "Conta Suspensa" se não ativo

### **Média Prioridade**

- [ ] **Implementar Refresh Token**
  - Token de curta duração (15 min)
  - Refresh token de longa duração (7 dias)
  - Endpoint /api/auth/refresh

- [ ] **Adicionar Rate Limiting**
  - Limitar tentativas de login (5 por minuto)
  - Limitar chamadas de API (100 por minuto por tenant)

- [ ] **Implementar Audit Logs**
  - Registrar todas as ações importantes
  - Login, Logout, Alterações de dados
  - Armazenar em `shared.audit_logs`

- [ ] **Melhorar tratamento de erros**
  - Criar componente ErrorBoundary
  - Exibir mensagens amigáveis ao usuário
  - Enviar erros para serviço de monitoring (Sentry)

### **Baixa Prioridade**

- [ ] **Implementar testes unitários**
  - Backend: xUnit + Moq
  - Frontend: Vitest + React Testing Library

- [ ] **Adicionar Dark Mode**
  - Implementar tema escuro no Design System
  - Persistir preferência do usuário

- [ ] **Otimizar performance**
  - Lazy loading de rotas
  - Virtualization de listas longas
  - Memoização de componentes pesados

---

## 🎯 **Próximas Features (Roadmap)**

### **Fase 1: Finalização do MVP (1-2 semanas)**

#### **Semana 1**
- [ ] Corrigir bugs críticos (redirect loop)
- [ ] Criar seed de produtos
- [ ] Implementar página "Meus Produtos" (admin)
- [ ] Adicionar loading states em todas as páginas
- [ ] Implementar error boundaries

#### **Semana 2**
- [ ] Criar página de gerenciamento de usuários
- [ ] Implementar convite de usuários (enviar email)
- [ ] Adicionar página de perfil do usuário
- [ ] Implementar alteração de senha
- [ ] Criar página de configurações do tenant

---

### **Fase 2: Primeiro Produto - Levver MST (2-4 semanas)**

#### **Módulo: Multi-Sourcing de Talentos**

**Backend:**
- [ ] Criar entidades: Candidato, Vaga, ProcessoSeletivo
- [ ] Criar repositories e services para MST
- [ ] Criar controllers: CandidatosController, VagasController
- [ ] Implementar upload de currículos (Azure Blob Storage)
- [ ] Criar API de integração com LinkedIn, Gupy, Kenoby

**Frontend:**
- [ ] Criar feature `features/mst/`
- [ ] Implementar dashboard MST
- [ ] Criar página de listagem de candidatos
- [ ] Criar página de detalhes do candidato
- [ ] Implementar formulário de criação de vaga
- [ ] Criar kanban de processos seletivos
- [ ] Implementar filtros e busca avançada

**Database:**
- [ ] Criar tabelas no schema por tenant:
  - `tenant_[guid].candidatos`
  - `tenant_[guid].vagas`
  - `tenant_[guid].processos_seletivos`
  - `tenant_[guid].candidatos_vagas` (N:N)

---

### **Fase 3: Sistema de Cobrança (2-3 semanas)**

- [ ] Integração com gateway de pagamento (Stripe ou PagSeguro)
- [ ] Criar página de assinaturas
- [ ] Implementar planos (Mensal, Anual)
- [ ] Criar fluxo de contratação de produtos
- [ ] Implementar cancelamento de assinatura
- [ ] Criar dashboard financeiro (admin)
- [ ] Implementar notificações de pagamento

---

### **Fase 4: Produtos Adicionais (3-6 meses)**

#### **Levver Ponto**
- [ ] Registro de ponto (web + mobile)
- [ ] Geolocalização
- [ ] Relatórios de horas
- [ ] Espelho de ponto
- [ ] Integração com folha de pagamento

#### **Levver Performance**
- [ ] Avaliações de desempenho
- [ ] Metas e OKRs
- [ ] Feedbacks 360°
- [ ] Planos de desenvolvimento individual (PDI)
- [ ] Relatórios de performance

#### **Levver Onboarding**
- [ ] Checklist de integração
- [ ] Envio de documentos
- [ ] Treinamentos obrigatórios
- [ ] Apresentação da equipe
- [ ] Pesquisa de satisfação

#### **Levver Treinamento**
- [ ] Catálogo de cursos
- [ ] Trilhas de aprendizado
- [ ] Certificados
- [ ] Gamificação
- [ ] Relatórios de conclusão

---

### **Fase 5: Melhorias de Plataforma (Contínuo)**

#### **Dashboard de Analytics**
- [ ] Métricas por produto
- [ ] KPIs do tenant
- [ ] Gráficos e relatórios
- [ ] Exportação de dados (CSV, Excel, PDF)

#### **Notificações**
- [ ] Sistema de notificações in-app
- [ ] Notificações por email
- [ ] Notificações push (PWA)
- [ ] Central de notificações

#### **Integrações**
- [ ] API pública (REST)
- [ ] Webhooks
- [ ] Integração com Slack
- [ ] Integração com Microsoft Teams
- [ ] Integração com Google Workspace

#### **Mobile App**
- [ ] React Native app
- [ ] Login biométrico
- [ ] Notificações push
- [ ] Offline-first

---

## 🏗️ **Arquitetura Futura**

### **Microserviços (Long-term)**

Migrar de monolito para microserviços:

```
┌─────────────────────────────────────────────────────────┐
│                    API Gateway                          │
└────────────────────┬────────────────────────────────────┘
                     │
        ┌────────────┼────────────┬──────────────┐
        │            │            │              │
┌───────▼──────┐ ┌──▼─────┐ ┌───▼──────┐ ┌─────▼──────┐
│ Auth Service │ │MST Svc │ │Ponto Svc │ │Payment Svc │
│              │ │        │ │          │ │            │
│ - Login      │ │- Vagas │ │- Registro│ │- Cobrança  │
│ - Register   │ │- Candid│ │- Relat.  │ │- Planos    │
└──────────────┘ └────────┘ └──────────┘ └────────────┘
```

### **Event-Driven Architecture**

Usar mensageria (RabbitMQ, Azure Service Bus):

```
Evento: UserCreated
├─> Envia email de boas-vindas
├─> Cria audit log
└─> Atualiza dashboard de analytics

Evento: ProductActivated
├─> Cria schema de tabelas do produto
├─> Envia notificação para admin
└─> Registra no billing
```

---

## 📊 **Métricas de Sucesso**

### **MVP 1.0**
- [ ] 10 tenants ativos
- [ ] 0 bugs críticos
- [ ] Tempo de carregamento < 2s
- [ ] Uptime > 99%

### **Produto MST (v2.0)**
- [ ] 50 tenants ativos
- [ ] 1000+ candidatos cadastrados
- [ ] 100+ vagas ativas
- [ ] Taxa de conversão > 20%

### **Plataforma Completa (v3.0)**
- [ ] 500+ tenants ativos
- [ ] 5+ produtos lançados
- [ ] MRR (Monthly Recurring Revenue) > R$ 100k
- [ ] NPS > 50

---

## 🎓 **Aprendizados e Melhorias**

### **Decisões Técnicas Importantes**

✅ **Clean Architecture**: Facilitou manutenção e testes  
✅ **Schema-based Multi-tenancy**: Isolamento total de dados  
✅ **Feature-based Frontend**: Organização clara por módulos  
✅ **Zustand**: State management simples e performático  

### **Lições Aprendidas**

⚠️ **Migrations em DB existente**: Requer cuidado especial, usar SQL manual quando necessário  
⚠️ **Persist + Zustand**: Pode causar problemas de sincronização, sempre validar hidratação  
⚠️ **Azure AD SSO**: Fluxo complexo, documentar bem os estados (PendenteSetup, Ativo)  

### **Melhorias para Próximas Iterações**

🔄 Implementar testes desde o início (TDD)  
🔄 Usar CI/CD desde o MVP  
🔄 Monitoramento e observabilidade (Application Insights)  
🔄 Code review obrigatório antes de merge  

---

## 📅 **Timeline Estimado**

```
Novembro 2025
├─ Semana 1-2: Correção de bugs + Seed data
├─ Semana 3-4: Gerenciamento de usuários

Dezembro 2025
├─ Semana 1-4: Desenvolvimento do MST (backend)

Janeiro 2026
├─ Semana 1-4: Desenvolvimento do MST (frontend)

Fevereiro 2026
├─ Semana 1-2: Sistema de cobrança
├─ Semana 3-4: Testes e ajustes

Março 2026
├─ Lançamento oficial do Levver MST
└─ Início do desenvolvimento do Levver Ponto
```

---

## 🚀 **Deployment Strategy**

### **Ambientes**

```
┌─────────────┐
│ Development │  localhost
├─────────────┤
│   Staging   │  Azure App Service (staging slot)
├─────────────┤
│ Production  │  Azure App Service (production slot)
└─────────────┘
```

### **CI/CD Pipeline**

```
Git Push → GitHub Actions
  ├─> Build Backend (.NET)
  ├─> Build Frontend (Vite)
  ├─> Run Tests
  ├─> Deploy to Staging
  ├─> Smoke Tests
  └─> Deploy to Production (manual approval)
```

### **Infraestrutura**

- **Backend**: Azure App Service (Linux)
- **Frontend**: Azure Static Web Apps
- **Database**: Azure SQL Database
- **Storage**: Azure Blob Storage (currículos, logos)
- **Cache**: Azure Redis Cache
- **CDN**: Azure CDN
- **Monitoring**: Azure Application Insights

---

## 🎯 **Prioridades Imediatas (Esta Semana)**

1. 🔴 **Corrigir redirect loop** (bloqueador)
2. 🟡 **Criar seed de produtos** (necessário para testar UI)
3. 🟡 **Limpar código antigo** (Dashboard.tsx)
4. 🟢 **Documentar fluxos** (já feito! 🎉)

---

## 💡 **Ideias para o Futuro**

- [ ] **IA para triagem de currículos** (ML.NET ou Azure Cognitive Services)
- [ ] **Chatbot de atendimento** (Azure Bot Service)
- [ ] **Marketplace de integrações** (plugins de terceiros)
- [ ] **White-label completo** (subdomínios personalizados)
- [ ] **Mobile apps nativos** (iOS + Android)
- [ ] **API pública** (permitir integrações externas)

---

**Última Atualização**: 14 de Novembro de 2025  
**Versão do Documento**: 1.0  
**Responsável**: Time de Desenvolvimento Levver.ai
