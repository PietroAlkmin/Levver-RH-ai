# ?? Script de Instalação - LevverRH Frontend

## Instalar dependências adicionais

Execute estes comandos no terminal dentro da pasta `LevverRH.FrontEnd`:

```powershell
# Navegue até a pasta do frontend
cd LevverRH.FrontEnd

# Instalar bibliotecas principais
npm install react-router-dom axios zustand

# Instalar formulários e validação
npm install react-hook-form zod @hookform/resolvers

# Instalar UI/Ícones
npm install lucide-react react-hot-toast

# Instalar utilitários
npm install date-fns

# Instalar Tailwind CSS
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p

# Instalar Prettier
npm install -D prettier eslint-config-prettier eslint-plugin-prettier
```

## Verificar instalação

```powershell
npm list react react-dom react-router-dom axios
```

## Rodar o projeto

```powershell
npm run dev
```

Deve abrir em: http://localhost:5173
