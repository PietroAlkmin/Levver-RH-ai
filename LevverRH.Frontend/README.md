# ?? LevverRH Frontend

Frontend React + TypeScript para o sistema LevverRH.

## ?? Pré-requisitos

- Node.js 18+
- npm ou yarn

## ??? Instalação

```bash
# Instalar dependências
npm install

# Rodar em desenvolvimento
npm run dev
```

## ?? URLs

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5113/api
- **Swagger**: http://localhost:5113/swagger

## ?? Estrutura do Projeto

```
src/
??? components/      # Componentes reutilizáveis
?   ??? common/      # Button, Input, Card, Loading
??? pages/           # Páginas da aplicação
?   ??? Auth/        # Login
?   ??? Dashboard/   # Dashboard simples
??? services/        # Chamadas à API
?   ??? api.ts    # Axios configurado
?   ??? authService.ts
??? stores/       # Zustand stores
?   ??? authStore.ts
??? hooks/      # Custom hooks
?   ??? useAuth.ts
??? types/           # TypeScript types
?   ??? auth.types.ts
?   ??? api.types.ts
??? routes/   # Configuração de rotas
?   ??? AppRoutes.tsx
?   ??? ProtectedRoute.tsx
??? App.tsx       # Componente principal
```

## ?? Funcionalidades Implementadas

### ? Autenticação
- [x] Login com email/senha
- [x] Armazenamento de token JWT
- [x] Logout
- [x] Rotas protegidas
- [x] Mensagens de sucesso/erro (Toast)

### ?? UI/UX
- [x] Design responsivo
- [x] Validação de formulários (React Hook Form + Zod)
- [x] Loading states
- [x] Tailwind CSS

## ?? Como Testar

1. **Inicie o backend (.NET)**:
   ```bash
   # Na pasta LevverRH.WebApp
   dotnet run
   ```

2. **Inicie o frontend (React)**:
   ```bash
   # Na pasta LevverRH.FrontEnd
   npm run dev
   ```

3. **Acesse**: http://localhost:5173

4. **Faça login** com credenciais do banco de dados

## ?? Tecnologias

- React 19.2.0
- TypeScript 5.9.3
- Vite 7.1.7
- React Router 7.9.5
- Axios 1.13.2
- Zustand 5.0.8
- React Hook Form 7.66.0
- Zod
- Tailwind CSS 4.1.17
- Lucide React 0.552.0
- React Hot Toast 2.6.0
