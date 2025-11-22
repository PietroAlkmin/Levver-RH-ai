import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Menu, Sparkles, ChevronDown } from 'lucide-react';
import './Header.css';

interface HeaderProps {
  currentProductName?: string;
  onMenuClick?: () => void;
}

export const Header: React.FC<HeaderProps> = ({ 
  currentProductName = 'Painel', 
  onMenuClick 
}) => {
  const [showDropdown, setShowDropdown] = useState(false);
  const navigate = useNavigate();

  const handleAIClick = () => {
    console.log('Abrir assistente IA');
  };

  const handleProductChange = (path: string) => {
    navigate(path);
    setShowDropdown(false);
  };

  const dropdownLabel = currentProductName === 'Painel' ? 'Painel principal' : 'Painel Talents';

  return (
    <header className="main-header">
      <div className="header-left">
        <h1 className="header-title">{dropdownLabel}</h1>
      </div>
      
      <div className="header-right">
        <div className="product-dropdown">
          <button 
            className="dropdown-trigger"
            onClick={() => setShowDropdown(!showDropdown)}
          >
            <span>{dropdownLabel}</span>
            <ChevronDown size={16} />
          </button>

          {showDropdown && (
            <div className="dropdown-menu">
              <button 
                className="dropdown-item" 
                onClick={() => handleProductChange('/painel')}
              >
                Painel principal
              </button>
              <button 
                className="dropdown-item" 
                onClick={() => handleProductChange('/talents')}
              >
                Painel Talents
              </button>
            </div>
          )}
        </div>

        <button className="ai-button" onClick={handleAIClick}>
          <Sparkles size={18} />
          <span>Pergunte para a Levver</span>
        </button>
      </div>
    </header>
  );
};
