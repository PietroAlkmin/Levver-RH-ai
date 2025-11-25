import React, { useState } from 'react';
import { Sparkles, ChevronDown } from 'lucide-react';
import { useNavigate, useLocation } from 'react-router-dom';
import './Header.css';

export const Header: React.FC = () => {
  const [showDropdown, setShowDropdown] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  const currentPanel = location.pathname.startsWith('/talents') ? 'Painel Talents' : 'Painel principal';

  const handlePanelChange = (path: string) => {
    navigate(path);
    setShowDropdown(false);
  };

  return (
    <header className="main-header">
      <div className="header-right">
        <div className="header-product-dropdown">
          <button 
            className="header-dropdown-trigger"
            onClick={() => setShowDropdown(!showDropdown)}
          >
            <span>{currentPanel}</span>
            <ChevronDown size={16} />
          </button>
          {showDropdown && (
            <div className="header-dropdown-menu">
              <button 
                className={`header-dropdown-item ${currentPanel === 'Painel principal' ? 'active' : ''}`}
                onClick={() => handlePanelChange('/')}
              >
                <span className="header-dropdown-item-title">Painel principal</span>
                {currentPanel === 'Painel principal' && <span className="header-dropdown-check">✓</span>}
              </button>
              <button 
                className={`header-dropdown-item ${currentPanel === 'Painel Talents' ? 'active' : ''}`}
                onClick={() => handlePanelChange('/talents')}
              >
                <span className="header-dropdown-item-title">Painel Talents</span>
                {currentPanel === 'Painel Talents' && <span className="header-dropdown-check">✓</span>}
              </button>
            </div>
          )}
        </div>
        <button className="ai-button">
          <Sparkles size={18} />
          <span>Pergunte para a Levver</span>
        </button>
      </div>
    </header>
  );
};
