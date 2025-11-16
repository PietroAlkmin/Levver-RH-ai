import React, { useState, useEffect } from 'react';
import { productService } from '../../../features/painel/services/productService';
import { Product } from '../../../features/painel/types/product.types';
import './Header.css';

interface HeaderProps {
  currentProductName?: string;
}

export const Header: React.FC<HeaderProps> = ({ currentProductName = 'Painel principal' }) => {
  const [products, setProducts] = useState<Product[]>([]);
  const [selectedProduct, setSelectedProduct] = useState<string>(currentProductName);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      const data = await productService.getAllProducts();
      setProducts(data);
    } catch (error) {
      console.error('Erro ao carregar produtos:', error);
    }
  };

  const handleProductChange = (productName: string, productRoute?: string) => {
    setSelectedProduct(productName);
    setIsDropdownOpen(false);
    // Navegar para o produto selecionado
    if (productRoute) {
      window.location.href = productRoute;
    }
  };

  const handleAIClick = () => {
    // TODO: Implementar modal da IA
    console.log('Abrir assistente IA');
  };

  return (
    <header className="main-header">
      <div className="header-content">
        {/* Dropdown de Produtos */}
        <div className="product-selector">
          <button 
            className="product-dropdown-trigger"
            onClick={() => setIsDropdownOpen(!isDropdownOpen)}
          >
            <svg className="product-icon" width="20" height="20" viewBox="0 0 20 20" fill="none">
              <path d="M2 5h16M2 10h16M2 15h16" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
            </svg>
            <span className="product-name">{selectedProduct}</span>
            <svg className="dropdown-arrow" width="12" height="12" viewBox="0 0 12 12" fill="none">
              <path d={isDropdownOpen ? "M2 8l4-4 4 4" : "M2 4l4 4 4-4"} stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </button>

          {isDropdownOpen && (
            <div className="product-dropdown-menu">
              {products.map(product => (
                <button
                  key={product.id}
                  className={`product-dropdown-item ${selectedProduct === product.produtoNome ? 'active' : ''}`}
                  onClick={() => handleProductChange(product.produtoNome, product.rotaBase)}
                >
                  <div className="product-item-icon">
                    <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
                      <rect width="20" height="20" rx="6" fill="var(--color-purple)" fillOpacity="0.1"/>
                      <text x="10" y="14" textAnchor="middle" fill="var(--color-purple)" fontSize="12" fontWeight="600">{product.produtoNome.charAt(0)}</text>
                    </svg>
                  </div>
                  <div className="product-item-info">
                    <span className="product-item-name">{product.produtoNome}</span>
                    <span className="product-item-category">{product.categoria}</span>
                  </div>
                  {!product.lancado && (
                    <span className="product-item-badge">Em breve</span>
                  )}
                </button>
              ))}
            </div>
          )}
        </div>

        {/* Botão IA */}
        <button className="ai-button" onClick={handleAIClick}>
          <svg className="ai-icon" width="20" height="20" viewBox="0 0 24 24" fill="none">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" stroke="white" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
          </svg>
          <span className="ai-text">Pergunte para a Levver</span>
        </button>
      </div>
    </header>
  );
};
