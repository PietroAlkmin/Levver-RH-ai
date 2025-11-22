import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Settings, Bell } from 'lucide-react';
import './Sidebar.css';

interface ProductItem {
  id: string;
  label: string;
  path: string;
}

export const Sidebar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const products: ProductItem[] = [
    { id: 'painel', label: 'PAINEL', path: '/painel' },
    { id: 'talents', label: 'TALENTS', path: '/talents' }
  ];

  const isActive = (path: string) => {
    return location.pathname === path || location.pathname.startsWith(path + '/');
  };

  return (
    <aside className="sidebar">
      {/* Logo Favicon */}
      <div className="sidebar-logo">
        <img 
          src="https://levverstorage.blob.core.windows.net/favicons/levver-favicon.png" 
          alt="Levver" 
        />
      </div>

      {/* Notificação */}
      <button className="sidebar-icon-button notification" title="Notificações">
        <Bell size={20} />
      </button>

      {/* Linha separadora */}
      <div className="sidebar-divider" />

      {/* Produtos (texto) */}
      <nav className="sidebar-products">
        {products.map(product => (
          <button
            key={product.id}
            className={`product-button ${isActive(product.path) ? 'active' : ''}`}
            onClick={() => navigate(product.path)}
          >
            {product.label}
          </button>
        ))}
      </nav>

      {/* Perfil + Configurações */}
      <div className="sidebar-bottom">
        <button className="sidebar-icon-button profile" title="Perfil">
          <img 
            src="https://api.dicebear.com/7.x/avataaars/svg?seed=User" 
            alt="Perfil"
          />
        </button>

        <button 
          className="sidebar-icon-button settings"
          title="Configurações"
        >
          <Settings size={20} />
        </button>
      </div>
    </aside>
  );
};
