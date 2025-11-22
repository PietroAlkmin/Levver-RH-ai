import { useEffect } from 'react';
import { AppRoutes } from './routes/AppRoutes';
import { useWhiteLabel } from './hooks/useWhiteLabel';
import './index.css';

function App() {
  useWhiteLabel();

  useEffect(() => {
    console.log('🚀 App - Iniciando aplicação');
    console.log('🔑 App - Token no localStorage:', localStorage.getItem('token') ? localStorage.getItem('token')?.substring(0, 50) + '...' : 'NO TOKEN');
    console.log('👤 App - User no localStorage:', localStorage.getItem('user'));
    console.log('🏢 App - Tenant no localStorage:', localStorage.getItem('tenant'));
    console.log('🎨 App - auth-storage (Zustand):', localStorage.getItem('auth-storage'));
    
    // Verificar se há dados no localStorage mas Zustand não hidratou
    const hasLocalStorageData = !!localStorage.getItem('token');
    console.log('💾 App - Tem dados no localStorage:', hasLocalStorageData);
  }, []);

  return <AppRoutes />;
}

export default App;
