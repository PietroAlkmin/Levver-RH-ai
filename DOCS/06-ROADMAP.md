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
- [x] **Levver Talents - Backend Completo**
  - [x] 6 Entidades (Vaga, Candidatura, Entrevista, Avaliacao, Etapa, Habilidade)
  - [x] 4 Enums (StatusVaga, StatusCandidatura, TipoEntrevista, StatusEntrevista)
  - [x] 6 Repositórios específicos
  - [x] TalentsService com todos os métodos CRUD
  - [x] TalentsController com endpoints REST
  - [x] DTOs completos para todas as operações
  - [x] Relacionamentos entre entidades configurados
  - [x] Dashboard API com métricas em tempo real
  - [x] **Criação de Vagas Assistida por IA** (OpenAI GPT-4o-mini)
    - [x] JobAIService completo
    - [x] Chat conversacional para requisitos
    - [x] Extração inteligente de campos
    - [x] Geração automática de descrições
    - [x] API endpoints (/api/talents/jobs/ai/*)
  - [x] **Análise de Currículos com IA** (OpenAI GPT-4o)
    - [x] PdfExtractor para extração de texto
    - [x] CandidateAnalyzer com scoring automático
    - [x] API endpoint (/api/talents/applications/{id}/analyze)
    - [x] Integração com campos da entidade Application

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
- [x] **Levver Talents - Frontend Completo**
  - [x] TalentsDashboard com 4 cards de métricas
  - [x] Integração com MainLayout (Sidebar + Header)
  - [x] Ícone na Sidebar com navegação
  - [x] talentsService completo (API integration)
  - [x] Types TypeScript para todas as entidades
  - [x] Rota protegida /talents
  - [x] Design responsivo com gradiente Levver
  - [x] Loading states e error handling
  - [x] **NewJobPage - Criação com IA**
    - [x] Chat conversacional com histórico
    - [x] Campos editáveis manualmente
    - [x] Indicador de progresso visual
    - [x] Envio de mensagens e resposta da IA
    - [x] Salvamento de vaga após conclusão
  - [x] **JobDetailPage - Gestão e Análise**
    - [x] Listagem de candidaturas
    - [x] Botão "Analisar com IA" por candidato
    - [x] Loading individual por análise
    - [x] Exibição de scores e justificativa
    - [x] Toast notifications (success/error)
  - [x] **ApplyPage - Formulário Público**
    - [x] Aplicação sem autenticação
    - [x] Upload de currículo (PDF)
    - [x] Criação automática de conta
    - [x] Auto-login após aplicação
    - [x] Validação completa de campos

#### **Database**
- [x] Schema `shared` para tabelas globais
- [x] Migrations aplicadas (InitialCreate, AddTenantProductsTable)
- [x] Tabelas criadas: tenants, users, products_catalog, tenant_products
- [x] Foreign keys e constraints configurados

---

## 🐛 **Bugs Conhecidos (Prioridade Alta)**

Nenhum bug crítico conhecido no momento. Sistema estável após implementação do Levver Talents completo.

---

## 🔧 **Tarefas Técnicas Pendentes**

### **Alta Prioridade**

- [ ] **Criar seed de produtos**
  ```sql
  INSERT INTO shared.products_catalog (...) VALUES
    ('Levver Talents', '🎯', '#A417D0', '/talents', 1, 1),
    ('Levver Ponto', '⏰', '#11005D', '/ponto', 2, 0),
    ('Levver Performance', '📊', '#D4C2F5', '/performance', 3, 0);
  ```

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

### **Fase 2: Expansão do Levver Talents (2-4 semanas)**

#### **Módulo: Páginas Avançadas de Gestão**

**Frontend:**
- [ ] Criar página de listagem de vagas
  - [ ] Filtros avançados (status, departamento, localização)
  - [ ] Cards com informações resumidas
  - [ ] Ações rápidas (editar, publicar, arquivar)
- [ ] Melhorar página de gestão de candidatos
  - [ ] Filtros por vaga, status, score IA
  - [ ] Ordenação por score geral
  - [ ] Visualização de análise IA completa
  - [ ] Tags e classificações personalizadas
- [ ] Criar página de relatórios
  - [ ] Funil de conversão
  - [ ] Tempo médio de contratação
  - [ ] Eficácia da análise IA
  - [ ] Exportação de dados
- [ ] Implementar kanban de pipeline
  - [ ] Drag & drop de candidatos entre etapas
  - [ ] Customização de etapas por vaga
  - [ ] Ações rápidas inline

**Backend:**
- [x] Upload de currículos (FileStorage local)
- [ ] Migração para Azure Blob Storage
- [ ] Sistema de notificações por email
- [ ] Webhooks para eventos importantes
- [ ] Sistema de templates de email
- [ ] **Otimização de Custos de IA**
  - [ ] Cache de análises repetidas
  - [ ] Batch processing de currículos
  - [ ] Limite de análises por tenant/período

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
├─ Semana 1-2: ✅ Arquitetura base + Autenticação
├─ Semana 3-4: ✅ Levver Talents (backend + frontend dashboard)

Dezembro 2025
├─ Semana 1-2: Expansão Levver Talents (páginas de gestão)
├─ Semana 3-4: Upload de currículos + Notificações

Janeiro 2026
├─ Semana 1-2: Kanban de pipeline + Filtros avançados
├─ Semana 3-4: Relatórios e analytics

Fevereiro 2026
├─ Semana 1-2: Sistema de cobrança
├─ Semana 3-4: Testes e ajustes

Março 2026
├─ Lançamento oficial do Levver Talents v1.0
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

1. ✅ **Levver Talents Backend** (completado!)
2. ✅ **Levver Talents Frontend Dashboard** (completado!)
3. ✅ **Integração Sidebar** (completado!)
4. ✅ **Documentação atualizada** (completado!)
5. 🟡 **Criar páginas de gestão do Talents** (próximo passo)
6. 🟡 **Implementar filtros e busca avançada**
7. 🟡 **Sistema de upload de currículos**

---

## 💡 **Ideias para o Futuro**

- [x] **IA para triagem de currículos** ✅ (OpenAI GPT-4o implementado)
- [x] **IA para criação de vagas** ✅ (OpenAI GPT-4o-mini implementado)
- [ ] **IA para geração de perguntas de entrevista**
- [ ] **Chatbot de atendimento ao candidato** (Azure Bot Service)
- [ ] **Marketplace de integrações** (plugins de terceiros)
- [ ] **White-label completo** (subdomínios personalizados)
- [ ] **Mobile apps nativos** (iOS + Android)
- [ ] **API pública** (permitir integrações externas)
- [ ] **Análise de vídeo de entrevistas** (Azure Video Indexer)

---

**Última Atualização**: 30 de Novembro de 2025  
**Versão do Documento**: 2.0  
**Responsável**: Time de Desenvolvimento Levver.ai
