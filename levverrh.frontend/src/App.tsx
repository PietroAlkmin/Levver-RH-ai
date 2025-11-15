import { useEffect } from 'react';
import { AppRoutes } from './routes/AppRoutes';
import './index.css';

function App() {
  useEffect(() => {
    console.log('🚀 App - Iniciando aplicação');
    console.log('🔑 App - Token no localStorage:', localStorage.getItem('token')?.substring(0, 50) + '...');
    console.log('👤 App - User no localStorage:', localStorage.getItem('user'));
    console.log('🏢 App - Tenant no localStorage:', localStorage.getItem('tenant'));
    console.log('🎨 App - auth-storage (Zustand):', localStorage.getItem('auth-storage'));
  }, []);

  return <AppRoutes />;
}

export default App;
