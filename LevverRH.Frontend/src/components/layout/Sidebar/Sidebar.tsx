import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { LayoutDashboard, Plus, List, Menu, ChevronDown, Bell, Settings, LogOut } from 'lucide-react';
import { useAuthStore } from '../../../stores/authStore';
import { SettingsModal } from './SettingsModal';
import './Sidebar.css';

interface MenuItem {
  icon: React.ReactNode;
  label: string;
  path: string;
}

export const Sidebar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [isExpanded, setIsExpanded] = useState(false);
  const [isPinned, setIsPinned] = useState(false);
  const [showDropdown, setShowDropdown] = useState(false);
  const [showUserMenu, setShowUserMenu] = useState(false);
  const [showSettingsModal, setShowSettingsModal] = useState(false);
  
  const { user, tenant, clearAuth } = useAuthStore();

  const currentProduct = location.pathname.startsWith('/talents') ? 'Talents' : 'Painel principal';

  const getMenuItems = (): MenuItem[] => {
    if (location.pathname.startsWith('/talents')) {
      return [
        { icon: <LayoutDashboard size={20} />, label: 'Dashboard', path: '/talents' },
        { icon: <Plus size={20} />, label: 'Nova vaga', path: '/talents/nova-vaga' },
        { icon: <List size={20} />, label: 'Lista de vagas', path: '/talents/vagas' }
      ];
    }
    return [
      { icon: <LayoutDashboard size={20} />, label: 'Dashboard', path: '/painel' }
    ];
  };

  const isActive = (path: string) => location.pathname === path;

  const handleMouseEnter = () => {
    if (!isPinned) setIsExpanded(true);
  };

  const handleMouseLeave = () => {
    if (!isPinned) {
      setIsExpanded(false);
      setShowDropdown(false);
      setShowUserMenu(false);
    }
  };

  const handleOpenSettings = () => {
    setShowUserMenu(false);
    setShowSettingsModal(true);
  };

  const handleLogout = () => {
    clearAuth();
    navigate('/login');
  };

  const togglePin = () => {
    setIsPinned(!isPinned);
    setIsExpanded(!isPinned);
  };

  const shouldExpand = isExpanded || isPinned;

  return (
    <>
      <SettingsModal 
        isOpen={showSettingsModal} 
        onClose={() => setShowSettingsModal(false)} 
      />
      <aside 
        className={`sidebar ${shouldExpand ? 'expanded' : ''}`}
        onMouseEnter={handleMouseEnter}
        onMouseLeave={handleMouseLeave}
      >
        <div className="sidebar-header">
          {shouldExpand ? (
            <div className="product-dropdown">
              <button 
                className="sidebar-brand"
                onClick={() => setShowDropdown(!showDropdown)}
              >
                <img 
                  src="https://levverstorage.blob.core.windows.net/favicons/levver-favicon.png" 
                  alt="Levver" 
                  className="logo-image"
                />
                <span className="product-name">{currentProduct}</span>
                <ChevronDown size={16} className="dropdown-arrow" />
              </button>
              {showDropdown && (
                <div className="dropdown-menu">
                  <button 
                    className={`dropdown-item ${currentProduct === 'Painel principal' ? 'active' : ''}`}
                    onClick={() => { navigate('/painel'); setShowDropdown(false); }}
                  >
                    <div className="dropdown-item-content">
                      <div className="dropdown-item-title">Painel principal</div>
                      <div className="dropdown-item-description">Dashboard geral com informações de todos os produtos</div>
                    </div>
                    {currentProduct === 'Painel principal' && (
                      <svg width="20" height="20" viewBox="0 0 16 16" fill="none" className="dropdown-check">
                        <path d="M13.3332 4L5.99984 11.3333L2.6665 8" stroke="#3B82F6" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      </svg>
                    )}
                  </button>
                  <button 
                    className={`dropdown-item ${currentProduct === 'Talents' ? 'active' : ''}`}
                    onClick={() => { navigate('/talents'); setShowDropdown(false); }}
                  >
                    <div className="dropdown-item-content">
                      <div className="dropdown-item-title">Talents</div>
                      <div className="dropdown-item-description">Sistema completo de recrutamento e seleção</div>
                    </div>
                    {currentProduct === 'Talents' && (
                      <svg width="20" height="20" viewBox="0 0 16 16" fill="none" className="dropdown-check">
                        <path d="M13.3332 4L5.99984 11.3333L2.6665 8" stroke="#3B82F6" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      </svg>
                    )}
                  </button>
                </div>
              )}
            </div>
          ) : (
            <img 
              src="https://levverstorage.blob.core.windows.net/favicons/levver-favicon.png" 
              alt="Levver" 
              className="logo-image"
            />
          )}
        </div>

      <nav className="sidebar-menu">
        {getMenuItems().map((item, index) => (
          <button
            key={index}
            className={`menu-item ${isActive(item.path) ? 'active' : ''}`}
            onClick={() => navigate(item.path)}
            title={!shouldExpand ? item.label : undefined}
          >
            <span className="menu-icon">{item.icon}</span>
            {shouldExpand && <span className="menu-label">{item.label}</span>}
          </button>
        ))}
      </nav>

      {/* User Footer */}
      <div className="sidebar-footer">
        <button 
          className="user-button"
          onClick={() => setShowUserMenu(!showUserMenu)}
        >
          <div className="user-avatar">
            {user?.fotoUrl ? (
              <img src={user.fotoUrl} alt={user.nome} className="user-avatar-image" />
            ) : (
              user?.nome?.charAt(0).toUpperCase() || 'U'
            )}
          </div>
          {shouldExpand && (
            <div className="user-info">
              <div className="user-name">{user?.nome || 'Usuário'}</div>
              <div className="user-tenant">{tenant?.nome || 'Empresa'}</div>
            </div>
          )}
        </button>

        {showUserMenu && shouldExpand && (
          <div className="user-menu">
            <button className="user-menu-item" onClick={() => setShowUserMenu(false)}>
              <Bell size={16} />
              <span>Notificações</span>
            </button>
            <button className="user-menu-item" onClick={handleOpenSettings}>
              <Settings size={16} />
              <span>Configurações</span>
            </button>
            <div className="user-menu-divider"></div>
            <button className="user-menu-item user-menu-item-danger" onClick={handleLogout}>
              <LogOut size={16} />
              <span>Sair</span>
            </button>
          </div>
        )}
      </div>
    </aside>
    
    <button 
      className={`floating-menu-button ${isPinned ? 'pinned' : ''}`}
      onClick={togglePin}
      title={isPinned ? 'Desafixar menu' : 'Fixar menu aberto'}
    >
      <Menu size={20} />
    </button>
    </>
  );
};
