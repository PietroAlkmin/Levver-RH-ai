# 🎨 Design System - Levver.ai

## 🎯 Visão Geral

O **Levver Design System** é um conjunto de regras, componentes e estilos reutilizáveis que garantem **consistência visual** em toda a plataforma.

## 🎨 Paleta de Cores Oficial

### **Cores Primárias**

| Nome | Hex | RGB | Uso |
|------|-----|-----|-----|
| **Purple** | `#A417D0` | rgb(164, 23, 208) | Cor principal da marca, CTAs, links |
| **Dark** | `#11005D` | rgb(17, 0, 93) | Textos, headers, fundos escuros |
| **Light** | `#FBFBFF` | rgb(251, 251, 255) | Fundos claros, cards |
| **Lavender** | `#D4C2F5` | rgb(212, 194, 245) | Secundária, destaques suaves |
| **Gray** | `#EAEAF0` | rgb(234, 234, 240) | Bordas, divisores, backgrounds neutros |
| **Error** | `#E84358` | rgb(232, 67, 88) | Erros, alertas, validações |

### **Cores Semânticas**

```css
/* Success */
--levver-success: #10B981;
--levver-success-bg: #D1FAE5;
--levver-success-border: #6EE7B7;

/* Warning */
--levver-warning: #F59E0B;
--levver-warning-bg: #FEF3C7;
--levver-warning-border: #FCD34D;

/* Info */
--levver-info: #3B82F6;
--levver-info-bg: #DBEAFE;
--levver-info-border: #93C5FD;
```

### **Gradientes**

```css
--levver-gradient-primary: linear-gradient(135deg, #A417D0 0%, #D4C2F5 100%);
--levver-gradient-dark: linear-gradient(135deg, #11005D 0%, #A417D0 100%);
--levver-gradient-light: linear-gradient(135deg, #FBFBFF 0%, #EAEAF0 100%);
--levver-gradient-purple-blue: linear-gradient(135deg, #A417D0 0%, #3B82F6 100%);
```

---

## 📝 Tipografia

### **Font Family**

```css
--levver-font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
--levver-font-family-mono: 'Fira Code', 'Cascadia Code', Consolas, Monaco, monospace;
```

### **Font Sizes**

```css
--levver-text-xs: 0.75rem;    /* 12px */
--levver-text-sm: 0.875rem;   /* 14px */
--levver-text-base: 1rem;     /* 16px */
--levver-text-lg: 1.125rem;   /* 18px */
--levver-text-xl: 1.25rem;    /* 20px */
--levver-text-2xl: 1.5rem;    /* 24px */
--levver-text-3xl: 1.875rem;  /* 30px */
--levver-text-4xl: 2.25rem;   /* 36px */
--levver-text-5xl: 3rem;      /* 48px */
```

### **Font Weights**

```css
--levver-font-light: 300;
--levver-font-normal: 400;
--levver-font-medium: 500;
--levver-font-semibold: 600;
--levver-font-bold: 700;
--levver-font-extrabold: 800;
```

### **Line Heights**

```css
--levver-leading-tight: 1.25;
--levver-leading-normal: 1.5;
--levver-leading-relaxed: 1.75;
```

---

## 🔲 Espaçamento

### **Spacing Scale**

```css
--levver-space-1: 0.25rem;   /* 4px */
--levver-space-2: 0.5rem;    /* 8px */
--levver-space-3: 0.75rem;   /* 12px */
--levver-space-4: 1rem;      /* 16px */
--levver-space-5: 1.25rem;   /* 20px */
--levver-space-6: 1.5rem;    /* 24px */
--levver-space-8: 2rem;      /* 32px */
--levver-space-10: 2.5rem;   /* 40px */
--levver-space-12: 3rem;     /* 48px */
--levver-space-16: 4rem;     /* 64px */
--levver-space-20: 5rem;     /* 80px */
```

### **Border Radius**

```css
--levver-radius-sm: 4px;
--levver-radius-md: 8px;
--levver-radius-lg: 12px;
--levver-radius-xl: 16px;
--levver-radius-2xl: 24px;
--levver-radius-full: 9999px;
```

---

## 🌑 Sombras (Shadows)

```css
--levver-shadow-sm: 0 2px 4px rgba(164, 23, 208, 0.1);
--levver-shadow-md: 0 4px 8px rgba(164, 23, 208, 0.15);
--levver-shadow-lg: 0 8px 16px rgba(164, 23, 208, 0.2);
--levver-shadow-xl: 0 12px 24px rgba(164, 23, 208, 0.25);
--levver-shadow-2xl: 0 20px 40px rgba(164, 23, 208, 0.3);

/* Sombras neutras (sem cor) */
--levver-shadow-neutral-sm: 0 1px 2px rgba(0, 0, 0, 0.05);
--levver-shadow-neutral-md: 0 4px 6px rgba(0, 0, 0, 0.1);
--levver-shadow-neutral-lg: 0 10px 15px rgba(0, 0, 0, 0.15);
```

---

## 🧩 Componentes

### **1. Botões**

#### **Botão Primário**

```css
.levver-btn-primary {
  background: var(--levver-gradient-primary);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: var(--levver-radius-md);
  font-weight: var(--levver-font-semibold);
  font-size: var(--levver-text-base);
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: var(--levver-shadow-sm);
}

.levver-btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: var(--levver-shadow-lg);
}

.levver-btn-primary:active {
  transform: translateY(0);
  box-shadow: var(--levver-shadow-sm);
}

.levver-btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
}
```

#### **Botão Secundário**

```css
.levver-btn-secondary {
  background: white;
  color: var(--levver-purple);
  border: 2px solid var(--levver-purple);
  padding: 12px 24px;
  border-radius: var(--levver-radius-md);
  font-weight: var(--levver-font-semibold);
  cursor: pointer;
  transition: all 0.2s ease;
}

.levver-btn-secondary:hover {
  background: var(--levver-purple);
  color: white;
}
```

#### **Botão Ghost**

```css
.levver-btn-ghost {
  background: transparent;
  color: var(--levver-purple);
  border: none;
  padding: 12px 24px;
  border-radius: var(--levver-radius-md);
  font-weight: var(--levver-font-semibold);
  cursor: pointer;
  transition: all 0.2s ease;
}

.levver-btn-ghost:hover {
  background: var(--levver-lavender);
}
```

---

### **2. Cards**

```css
.levver-card {
  background: white;
  border-radius: var(--levver-radius-lg);
  padding: var(--levver-space-6);
  box-shadow: var(--levver-shadow-md);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.levver-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--levver-shadow-lg);
}

.levver-card-header {
  margin-bottom: var(--levver-space-4);
  padding-bottom: var(--levver-space-4);
  border-bottom: 1px solid var(--levver-gray);
}

.levver-card-title {
  font-size: var(--levver-text-xl);
  font-weight: var(--levver-font-bold);
  color: var(--levver-dark);
  margin: 0;
}

.levver-card-body {
  color: var(--levver-dark);
  line-height: var(--levver-leading-relaxed);
}
```

---

### **3. Inputs**

```css
.levver-input {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid var(--levver-gray);
  border-radius: var(--levver-radius-md);
  font-size: var(--levver-text-base);
  font-family: var(--levver-font-family);
  transition: all 0.2s ease;
  background: white;
  color: var(--levver-dark);
}

.levver-input:focus {
  outline: none;
  border-color: var(--levver-purple);
  box-shadow: 0 0 0 3px rgba(164, 23, 208, 0.1);
}

.levver-input::placeholder {
  color: #9CA3AF;
}

.levver-input:disabled {
  background: var(--levver-gray);
  cursor: not-allowed;
}

.levver-input-error {
  border-color: var(--levver-error);
}

.levver-input-error:focus {
  box-shadow: 0 0 0 3px rgba(232, 67, 88, 0.1);
}
```

---

### **4. Badges**

```css
.levver-badge {
  display: inline-flex;
  align-items: center;
  padding: 4px 12px;
  border-radius: var(--levver-radius-full);
  font-size: var(--levver-text-sm);
  font-weight: var(--levver-font-medium);
}

.levver-badge-success {
  background: var(--levver-success-bg);
  color: var(--levver-success);
  border: 1px solid var(--levver-success-border);
}

.levver-badge-warning {
  background: var(--levver-warning-bg);
  color: var(--levver-warning);
  border: 1px solid var(--levver-warning-border);
}

.levver-badge-error {
  background: #FEE2E2;
  color: var(--levver-error);
  border: 1px solid #FECACA;
}

.levver-badge-info {
  background: var(--levver-info-bg);
  color: var(--levver-info);
  border: 1px solid var(--levver-info-border);
}
```

---

### **5. Loading Spinner**

```css
.levver-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--levver-gray);
  border-top-color: var(--levver-purple);
  border-radius: 50%;
  animation: levver-spin 0.8s linear infinite;
}

@keyframes levver-spin {
  to { transform: rotate(360deg); }
}

.levver-spinner-sm {
  width: 20px;
  height: 20px;
  border-width: 2px;
}

.levver-spinner-lg {
  width: 60px;
  height: 60px;
  border-width: 6px;
}
```

---

### **6. Toast Notifications**

```css
.levver-toast {
  background: white;
  border-radius: var(--levver-radius-lg);
  padding: var(--levver-space-4);
  box-shadow: var(--levver-shadow-xl);
  display: flex;
  align-items: center;
  gap: var(--levver-space-3);
  min-width: 300px;
  border-left: 4px solid var(--levver-purple);
}

.levver-toast-success {
  border-left-color: var(--levver-success);
}

.levver-toast-error {
  border-left-color: var(--levver-error);
}

.levver-toast-warning {
  border-left-color: var(--levver-warning);
}
```

---

## 📱 Responsividade

### **Breakpoints**

```css
/* Mobile First Approach */
--levver-screen-sm: 640px;    /* Tablets pequenos */
--levver-screen-md: 768px;    /* Tablets */
--levver-screen-lg: 1024px;   /* Desktop */
--levver-screen-xl: 1280px;   /* Desktop grande */
--levver-screen-2xl: 1536px;  /* Telas ultrawide */
```

### **Media Queries**

```css
/* Mobile (base) */
.container {
  padding: var(--levver-space-4);
}

/* Tablet */
@media (min-width: 768px) {
  .container {
    padding: var(--levver-space-6);
  }
}

/* Desktop */
@media (min-width: 1024px) {
  .container {
    padding: var(--levver-space-8);
    max-width: 1200px;
    margin: 0 auto;
  }
}
```

---

## 🌓 Dark Mode (Futuro)

```css
@media (prefers-color-scheme: dark) {
  :root {
    --levver-purple: #C77DFF;
    --levver-dark: #FBFBFF;
    --levver-light: #1A1A1A;
    --levver-lavender: #9D4EDD;
    --levver-gray: #2D2D2D;
  }
}
```

---

## 🎯 Uso no TypeScript

### **levver-theme.ts**

```typescript
export const LevverColors = {
  purple: '#A417D0',
  dark: '#11005D',
  light: '#FBFBFF',
  lavender: '#D4C2F5',
  gray: '#EAEAF0',
  error: '#E84358',
  success: '#10B981',
  warning: '#F59E0B',
  info: '#3B82F6',
} as const;

export type LevverColor = typeof LevverColors[keyof typeof LevverColors];

export const LevverGradients = {
  primary: 'linear-gradient(135deg, #A417D0 0%, #D4C2F5 100%)',
  dark: 'linear-gradient(135deg, #11005D 0%, #A417D0 100%)',
  light: 'linear-gradient(135deg, #FBFBFF 0%, #EAEAF0 100%)',
} as const;

export const LevverShadows = {
  sm: '0 2px 4px rgba(164, 23, 208, 0.1)',
  md: '0 4px 8px rgba(164, 23, 208, 0.15)',
  lg: '0 8px 16px rgba(164, 23, 208, 0.2)',
  xl: '0 12px 24px rgba(164, 23, 208, 0.25)',
} as const;

// Helper functions
export const withOpacity = (color: string, opacity: number): string => {
  const hex = color.replace('#', '');
  const r = parseInt(hex.substring(0, 2), 16);
  const g = parseInt(hex.substring(2, 4), 16);
  const b = parseInt(hex.substring(4, 6), 16);
  return `rgba(${r}, ${g}, ${b}, ${opacity})`;
};

export const lighten = (color: string, amount: number): string => {
  // Implementação para clarear cor
  return color;
};

export const darken = (color: string, amount: number): string => {
  // Implementação para escurecer cor
  return color;
};
```

### **Uso em Componentes**

```typescript
import { LevverColors, withOpacity } from '@/styles/levver-theme';

const ProductCard = styled.div`
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 8px ${withOpacity(LevverColors.purple, 0.15)};
  
  &:hover {
    background: ${withOpacity(LevverColors.lavender, 0.1)};
  }
`;
```

---

## 📚 Boas Práticas

### ✅ **Fazer**
- Usar variáveis CSS sempre que possível
- Manter consistência de espaçamento (usar escala de spacing)
- Preferir bordas arredondadas (`border-radius`)
- Adicionar transições suaves (0.2s ease)
- Usar sombras sutis para profundidade

### ❌ **Evitar**
- Hardcoded colors (usar variáveis)
- Espaçamentos aleatórios (4px, 7px, 13px)
- Animações muito rápidas ou lentas
- Sombras muito pesadas
- Fontes não especificadas no Design System

---

## 🎨 Exemplos de Uso

### **Hero Section**

```jsx
<div className="hero" style={{
  background: 'var(--levver-gradient-primary)',
  padding: 'var(--levver-space-20) var(--levver-space-8)',
  borderRadius: 'var(--levver-radius-2xl)',
  color: 'white',
  textAlign: 'center'
}}>
  <h1 style={{ fontSize: 'var(--levver-text-5xl)' }}>
    Bem-vindo ao Levver.ai
  </h1>
  <p style={{ fontSize: 'var(--levver-text-xl)' }}>
    Gestão de RH simplificada
  </p>
</div>
```

### **Product Card**

```jsx
<div className="levver-card">
  <div className="product-icon" style={{
    background: 'var(--levver-gradient-primary)',
    width: '60px',
    height: '60px',
    borderRadius: 'var(--levver-radius-lg)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontSize: 'var(--levver-text-3xl)'
  }}>
    🎯
  </div>
  <h3 className="levver-card-title">Levver MST</h3>
  <p>Multi-Sourcing de Talentos</p>
  <button className="levver-btn-primary">Acessar</button>
</div>
```

---

**Última Atualização**: 14 de Novembro de 2025
