import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { LayoutDashboard, Plus, List } from 'lucide-react';
import './DrawerMenu.css';

interface DrawerMenuProps {
  isOpen: boolean;
  onClose: () => void;
  productName: string;
}

interface MenuItem {
  icon: React.ReactNode;
  label: string;
  path: string;
}

export const DrawerMenu: React.FC<DrawerMenuProps> = ({ isOpen, onClose, productName }) => {
  const location = useLocation();
  const navigate = useNavigate();

  const getMenuItems = (): MenuItem[] => {
    if (productName === 'Painel') {
      return [
        { icon: <LayoutDashboard size={20} />, label: 'Dashboard', path: '/painel' }
      ];
    } else if (productName === 'Talents') {
      return [
        { icon: <LayoutDashboard size={20} />, label: 'Dashboard', path: '/talents' },
        { icon: <Plus size={20} />, label: 'Nova vaga', path: '/talents/nova-vaga' },
        { icon: <List size={20} />, label: 'Lista de vagas', path: '/talents/vagas' }
      ];
    }
    return [];
  };

  const menuItems = getMenuItems();

  const handleNavigate = (path: string) => {
    navigate(path);
    onClose();
  };

  if (!isOpen) return null;

  return (
    <>
      <div className="drawer-overlay" onClick={onClose} />
      <aside className={`drawer-menu ${isOpen ? 'open' : ''}`}>
        <div className="drawer-header">
          <h2>{productName === 'Painel' ? 'Painel principal' : 'Talents'}</h2>
        </div>

        <nav className="drawer-nav">
          {menuItems.map((item, index) => (
            <button
              key={index}
              className={`drawer-item ${location.pathname === item.path ? 'active' : ''}`}
              onClick={() => handleNavigate(item.path)}
            >
              <span className="drawer-icon">{item.icon}</span>
              <span className="drawer-label">{item.label}</span>
            </button>
          ))}
        </nav>
      </aside>
    </>
  );
};
