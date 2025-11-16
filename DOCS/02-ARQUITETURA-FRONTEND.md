# ⚛️ Arquitetura Frontend - Levver.ai RH

## 📐 Arquitetura Component-Based

O frontend utiliza **React 19** com **TypeScript** seguindo padrões modernos de desenvolvimento:
- **Feature-Based Architecture** (organização por features/módulos)
- **Composition over Inheritance**
- **Unidirectional Data Flow**
- **Smart/Container vs Dumb/Presentational Components**

## 🗂️ Estrutura de Pastas

```
LevverRH.Frontend/
├── public/                      # Arquivos estáticos
│   ├── vite.svg
│   └── ...
│
├── src/
│   ├── features/               # 🎯 Features/Módulos da aplicação
│   │   ├── painel/            # Catálogo de Produtos
│   │   │   ├── components/    # Componentes específicos do painel
│   │   │   │   ├── ProductCard.tsx
│   │   │   │   └── ProductCard.css
│   │   │   ├── pages/         # Páginas do painel
│   │   │   │   ├── PainelDashboard.tsx
│   │   │   │   └── PainelDashboard.css
│   │   │   ├── services/      # Services específicos
│   │   │   │   └── productService.ts
│   │   │   ├── types/         # TypeScript types
│   │   │   │   └── product.types.ts
│   │   │   └── index.ts       # Exports públicos
│   │   │
│   │   ├── talents/           # ✅ Levver Talents (IMPLEMENTADO)
│   │   │   ├── pages/         # Páginas do Talents
│   │   │   │   ├── TalentsDashboard.tsx
│   │   │   │   └── TalentsDashboard.css
│   │   │   ├── services/      # API do Talents
│   │   │   │   └── talentsService.ts
│   │   │   └── types/         # Types do Talents
│   │   │       └── talents.types.ts
│   │   │
│   │   └── [produto]/         # Futuros módulos de produtos
│   │       └── ... (Ponto, Performance, etc.)
│   │
│   ├── components/            # 🧩 Componentes reutilizáveis
│   │   ├── auth/             # Componentes de autenticação
│   │   │   ├── AzureAdLoginButton.tsx
│   │   │   └── ...
│   │   └── common/           # Componentes genéricos
│   │       ├── Loading.tsx
│   │       └── ...
│   │
│   ├── pages/                # 📄 Páginas gerais
│   │   ├── Auth/
│   │   │   ├── Login.tsx
│   │   │   └── RegisterTenant.tsx
│   │   └── Dashboard/       # ⚠️ DEPRECATED (substituído por /painel)
│   │
│   ├── routes/              # 🛣️ Configuração de rotas
│   │   ├── AppRoutes.tsx    # Definição de rotas
│   │   └── ProtectedRoute.tsx  # Higher-Order Component para proteção
│   │
│   ├── hooks/               # 🪝 Custom Hooks
│   │   └── useAuth.ts       # Hook de autenticação
│   │
│   ├── services/            # 🌐 API Clients
│   │   ├── api.ts           # Axios instance + interceptors
│   │   └── authService.ts   # Serviço de autenticação
│   │
│   ├── stores/              # 📦 State Management (Zustand)
│   │   └── authStore.ts     # Estado global de autenticação
│   │
│   ├── types/               # 📝 TypeScript Types Globais
│   │   ├── auth.types.ts
│   │   └── api.types.ts
│   │
│   ├── styles/              # 🎨 Design System
│   │   ├── levver-design-system.css  # CSS Variables + Utilities
│   │   ├── levver-theme.ts           # TypeScript Theme Tokens
│   │   └── index.css                 # Global styles
│   │
│   ├── App.tsx              # Componente raiz
│   ├── main.tsx             # Entry point (ReactDOM.createRoot)
│   └── vite-env.d.ts        # Vite type definitions
│
├── index.html               # HTML template
├── package.json             # Dependencies
├── tsconfig.json            # TypeScript config
├── vite.config.ts           # Vite config
└── .env                     # Environment variables
```

---

## 🎯 Feature-Based Architecture

### **Padrão de Organização**

Cada **feature** (módulo de produto) é auto-contido:

```
features/painel/
├── components/          # Componentes específicos deste módulo
├── pages/              # Páginas deste módulo
├── services/           # Lógica de API específica
├── types/              # Types TypeScript específicos
├── hooks/              # Custom hooks específicos (opcional)
├── utils/              # Utilitários específicos (opcional)
└── index.ts            # Exports públicos (API do módulo)
```

#### **Exemplo: feature/painel**

```typescript
// features/painel/index.ts
export { PainelDashboard } from './pages/PainelDashboard';
export { ProductCard } from './components/ProductCard';
export { productService } from './services/productService';
export type { ProductDTO, TenantProductDTO } from './types/product.types';
```

**Importação em outros arquivos:**
```typescript
import { PainelDashboard } from '@/features/painel';
```

---

## 🧩 Componentes

### **Categorização**

#### **1. Presentational Components (Dumb Components)**

Componentes **puros** que apenas renderizam UI baseado em props.

**Características:**
- ✅ Recebem dados via props
- ✅ Emitem eventos via callbacks
- ✅ Não acessam stores/context
- ✅ Fáceis de testar
- ✅ Reutilizáveis

**Exemplo: ProductCard.tsx**
```typescript
interface ProductCardProps {
  product: TenantProductDTO;
  onClick: () => void;
}

export const ProductCard: React.FC<ProductCardProps> = ({ product, onClick }) => {
  return (
    <div className="product-card" onClick={onClick}>
      <div className="product-icon">{product.icone}</div>
      <h3>{product.productName}</h3>
      <p>{product.descricao}</p>
      {product.ativo ? (
        <span className="badge-active">Ativo</span>
      ) : (
        <span className="badge-inactive">Em Breve</span>
      )}
    </div>
  );
};
```

#### **2. Container Components (Smart Components)**

Componentes que **gerenciam lógica** e estado.

**Características:**
- ✅ Acessam stores/context
- ✅ Fazem chamadas de API
- ✅ Gerenciam estado local
- ✅ Passam dados para presentational components

**Exemplo: PainelDashboard.tsx**
```typescript
export const PainelDashboard: React.FC = () => {
  const [products, setProducts] = useState<TenantProduct[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productService.getMyProducts();
      setProducts(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleProductClick = (product: TenantProduct) => {
    navigate(product.rotaBase);
  };

  if (loading) return <Loading />;

  return (
    <div className="painel-container">
      {products.map((product) => (
        <ProductCard
          key={product.productId}
          product={product}
          onClick={() => handleProductClick(product)}
        />
      ))}
    </div>
  );
};
```

#### **3. Higher-Order Components (HOC)**

Componentes que **envolvem outros componentes** para adicionar funcionalidade.

**Exemplo: ProtectedRoute.tsx**
```typescript
interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: string[];
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ 
  children, 
  requiredRoles 
}) => {
  const { isAuthenticated, user, isLoading, token } = useAuthStore();

  if (isLoading) {
    return <Loading fullScreen text="Verificando autenticação..." />;
  }

  if (!isAuthenticated || !token) {
    return <Navigate to="/login" replace />;
  }

  // Verificar roles se especificado
  if (requiredRoles && requiredRoles.length > 0 && user) {
    const hasRequiredRole = requiredRoles.includes(user.role);
    if (!hasRequiredRole) {
      return <div>403 - Acesso Negado</div>;
    }
  }

  return <>{children}</>;
};
```

---

## 🛣️ Sistema de Rotas

### **AppRoutes.tsx**

```typescript
export const AppRoutes: React.FC = () => {
  const { isAuthenticated } = useAuthStore();

  return (
    <BrowserRouter>
      <Toaster position="top-right" />
      
      <React.Suspense fallback={<div>Carregando...</div>}>
        <Routes>
          {/* Rota raiz - redirect condicional */}
          <Route
            path="/"
            element={
              isAuthenticated 
                ? <Navigate to="/painel" replace /> 
                : <Navigate to="/login" replace />
            }
          />

          {/* Rotas públicas */}
          <Route path="/login" element={<Login />} />
          <Route path="/register-tenant" element={<RegisterTenant />} />

          {/* Rotas protegidas */}
          <Route
            path="/painel"
            element={
              <ProtectedRoute>
                <PainelDashboard />
              </ProtectedRoute>
            }
          />

          {/* Levver Talents */}
          <Route
            path="/talents"
            element={
              <ProtectedRoute>
                <TalentsDashboard />
              </ProtectedRoute>
            }
          />

          {/* Futuros produtos */}
          <Route
            path="/ponto/*"
            element={
              <ProtectedRoute>
                <PontoModule />
              </ProtectedRoute>
            }
          />

          {/* 404 */}
          <Route path="*" element={<div>404 - Página não encontrada</div>} />
        </Routes>
      </React.Suspense>
    </BrowserRouter>
  );
};
```

### **Lazy Loading de Rotas**

```typescript
// Carregamento sob demanda (performance)
const PainelDashboard = React.lazy(() => 
  import('../features/painel/pages/PainelDashboard')
    .then(module => ({ default: module.PainelDashboard }))
);
```

**Vantagens:**
- ⚡ Reduz bundle inicial
- ⚡ Carrega código apenas quando necessário
- ⚡ Melhora performance percebida

---

## 📦 State Management - Zustand

### **Por que Zustand?**

- ✅ **Simples**: Menos boilerplate que Redux
- ✅ **TypeScript-first**: Excelente suporte a tipos
- ✅ **Performance**: Apenas re-renderiza componentes que usam o estado alterado
- ✅ **DevTools**: Integração com Redux DevTools
- ✅ **Persistência**: Middleware `persist` para localStorage

### **authStore.ts**

```typescript
interface AuthState {
  // State
  token: string | null;
  user: UserInfo | null;
  tenant: TenantInfo | null;
  whiteLabel: WhiteLabelInfo | null;
  isAuthenticated: boolean;
  isLoading: boolean;

  // Actions
  setAuth: (token: string, user: UserInfo, tenant: TenantInfo, whiteLabel?: WhiteLabelInfo | null) => void;
  clearAuth: () => void;
  setLoading: (isLoading: boolean) => void;
  updateUser: (user: Partial<UserInfo>) => void;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      set => ({
        // Initial State
        token: null,
        user: null,
        tenant: null,
        whiteLabel: null,
        isAuthenticated: false,
        isLoading: false,

        // Actions
        setAuth: (token, user, tenant, whiteLabel = null) =>
          set({
            token,
            user,
            tenant,
            whiteLabel,
            isAuthenticated: true,
            isLoading: false,
          }),

        clearAuth: () =>
          set({
            token: null,
            user: null,
            tenant: null,
            whiteLabel: null,
            isAuthenticated: false,
            isLoading: false,
          }),

        setLoading: isLoading => set({ isLoading }),

        updateUser: user =>
          set(state => ({
            user: state.user ? { ...state.user, ...user } : null,
          })),
      }),
      {
        name: 'auth-storage', // Nome no localStorage
        partialize: state => ({
          token: state.token,
          user: state.user,
          tenant: state.tenant,
          whiteLabel: state.whiteLabel,
          isAuthenticated: state.isAuthenticated,
        }),
      }
    ),
    { name: 'AuthStore' } // Nome no Redux DevTools
  )
);
```

### **Uso em Componentes**

```typescript
// Pegar apenas o que precisa (evita re-renders desnecessários)
const { isAuthenticated, user } = useAuthStore();

// Ou usar seletores
const isAuthenticated = useAuthStore(state => state.isAuthenticated);
const setAuth = useAuthStore(state => state.setAuth);
```

---

## 🪝 Custom Hooks

### **useAuth.ts**

Centraliza toda lógica de autenticação.

```typescript
export const useAuth = () => {
  const navigate = useNavigate();
  const { setAuth, clearAuth, setLoading, user, tenant, whiteLabel, isAuthenticated, isLoading } =
    useAuthStore();

  const login = useCallback(
    async (credentials: LoginRequest) => {
      try {
        setLoading(true);
        const response = await authService.login(credentials);

        if (response.success && response.data) {
          setAuth(
            response.data.token,
            response.data.user,
            response.data.tenant,
            response.data.whiteLabel
          );
          authService.saveAuthData(response.data);
          toast.success(`Bem-vindo(a), ${response.data.user.nome}!`);
          navigate('/painel');
          return { success: true };
        } else {
          toast.error(response.message || 'Erro ao fazer login');
          return { success: false, message: response.message };
        }
      } catch (error: any) {
        const errorMessage = error.response?.data?.message || 'Erro ao fazer login';
        toast.error(errorMessage);
        return { success: false, message: errorMessage };
      } finally {
        setLoading(false);
      }
    },
    [navigate, setAuth, setLoading]
  );

  const logout = useCallback(() => {
    clearAuth();
    authService.clearAuthData();
    toast.success('Logout realizado com sucesso');
    navigate('/login');
  }, [clearAuth, navigate]);

  const hasRole = useCallback(
    (roles: string | string[]): boolean => {
      if (!user) return false;
      const allowedRoles = Array.isArray(roles) ? roles : [roles];
      return allowedRoles.includes(user.role);
    },
    [user]
  );

  return {
    // State
    user,
    tenant,
    whiteLabel,
    isAuthenticated,
    isLoading,

    // Actions
    login,
    logout,
    hasRole,
  };
};
```

**Uso:**
```typescript
const { login, isAuthenticated, user } = useAuth();

const handleSubmit = async (data: LoginRequest) => {
  const result = await login(data);
  if (result.success) {
    console.log('Login bem-sucedido!');
  }
};
```

---

## 🌐 API Client (Axios)

### **api.ts**

```typescript
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5113/api';

const apiClient: AxiosInstance = axios.create({
  baseURL: API_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor - Adiciona token JWT
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('token');

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Response Interceptor - Trata erros globalmente
apiClient.interceptors.response.use(
  response => response,
  (error: AxiosError<ErrorResponse>) => {
    // Unauthorized - Token expirado ou inválido
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }

    // Forbidden - Sem permissão
    if (error.response?.status === 403) {
      console.error('Acesso negado');
    }

    // Server Error
    if (error.response?.status && error.response.status >= 500) {
      console.error('Erro no servidor. Tente novamente mais tarde.');
    }

    return Promise.reject(error);
  }
);

export default apiClient;
```

### **authService.ts**

```typescript
class AuthService {
  private readonly endpoint = '/auth';

  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
      `${this.endpoint}/login`,
      credentials
    );
    return response.data;
  }

  async loginWithAzureAd(data: AzureAdLoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
      `${this.endpoint}/login/azure`,
      data
    );
    return response.data;
  }

  saveAuthData(data: LoginResponse): void {
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data.user));
    localStorage.setItem('tenant', JSON.stringify(data.tenant));

    if (data.whiteLabel) {
      localStorage.setItem('whiteLabel', JSON.stringify(data.whiteLabel));
      this.applyWhiteLabel(data.whiteLabel);
    }
  }

  clearAuthData(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('tenant');
    localStorage.removeItem('whiteLabel');
  }
}

export default new AuthService();
```

---

## 🎨 Design System

### **levver-design-system.css**

```css
:root {
  /* Cores Primárias */
  --levver-purple: #A417D0;
  --levver-dark: #11005D;
  --levver-light: #FBFBFF;
  --levver-lavender: #D4C2F5;
  --levver-gray: #EAEAF0;
  --levver-error: #E84358;

  /* Gradientes */
  --levver-gradient-primary: linear-gradient(135deg, #A417D0 0%, #D4C2F5 100%);
  --levver-gradient-dark: linear-gradient(135deg, #11005D 0%, #A417D0 100%);

  /* Sombras */
  --levver-shadow-sm: 0 2px 4px rgba(164, 23, 208, 0.1);
  --levver-shadow-md: 0 4px 8px rgba(164, 23, 208, 0.15);
  --levver-shadow-lg: 0 8px 16px rgba(164, 23, 208, 0.2);

  /* Tipografia */
  --levver-font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

/* Utility Classes */
.levver-btn-primary {
  background: var(--levver-gradient-primary);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.levver-btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: var(--levver-shadow-lg);
}

.levver-card {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: var(--levver-shadow-md);
  transition: transform 0.2s, box-shadow 0.2s;
}

.levver-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--levver-shadow-lg);
}
```

### **levver-theme.ts**

```typescript
export const LevverColors = {
  purple: '#A417D0',
  dark: '#11005D',
  light: '#FBFBFF',
  lavender: '#D4C2F5',
  gray: '#EAEAF0',
  error: '#E84358',
} as const;

export const LevverGradients = {
  primary: 'linear-gradient(135deg, #A417D0 0%, #D4C2F5 100%)',
  dark: 'linear-gradient(135deg, #11005D 0%, #A417D0 100%)',
} as const;

// Helper functions
export const withOpacity = (color: string, opacity: number): string => {
  return `${color}${Math.round(opacity * 255).toString(16).padStart(2, '0')}`;
};

export const darken = (color: string, amount: number): string => {
  // Implementação de darkening
};

export const lighten = (color: string, amount: number): string => {
  // Implementação de lightening
};
```

---

## ⚡ Performance Optimizations

### **1. Code Splitting com React.lazy**

```typescript
const PainelDashboard = React.lazy(() => import('./features/painel'));
```

### **2. Memoização**

```typescript
// Evita re-renders desnecessários
const MemoizedProductCard = React.memo(ProductCard);

// Memoiza valores computados
const sortedProducts = useMemo(() => 
  products.sort((a, b) => a.ordemExibicao - b.ordemExibicao),
  [products]
);

// Memoiza callbacks
const handleClick = useCallback(() => {
  navigate('/produto');
}, [navigate]);
```

### **3. Virtualization (para listas grandes)**

```typescript
import { FixedSizeList } from 'react-window';

<FixedSizeList
  height={600}
  itemCount={products.length}
  itemSize={100}
>
  {({ index, style }) => (
    <div style={style}>
      <ProductCard product={products[index]} />
    </div>
  )}
</FixedSizeList>
```

---

## 📱 Responsividade

### **Mobile-First Approach**

```css
/* Base (mobile) */
.product-card {
  width: 100%;
}

/* Tablet */
@media (min-width: 768px) {
  .product-card {
    width: calc(50% - 16px);
  }
}

/* Desktop */
@media (min-width: 1024px) {
  .product-card {
    width: calc(33.333% - 16px);
  }
}
```

---

**Última Atualização**: 16 de Novembro de 2025
