import React from 'react';
import { TenantProduct } from '../types/product.types';
import './ProductCard.css';

interface ProductCardProps {
  product: TenantProduct;
  onClick?: () => void;
}

export const ProductCard: React.FC<ProductCardProps> = ({ product, onClick }) => {
  const isLaunched = product.lancado;
  const hasAccess = product.acessoAtivo;

  const handleClick = () => {
    if (isLaunched && hasAccess && onClick) {
      onClick();
    }
  };

  return (
    <div
      className={`product-card ${!isLaunched ? 'coming-soon' : ''} ${!hasAccess ? 'no-access' : ''}`}
      onClick={handleClick}
      style={{
        borderColor: product.corPrimaria || '#ccc',
        cursor: isLaunched && hasAccess ? 'pointer' : 'default',
      }}
    >
      {/* Ícone do Produto */}
      <div 
        className="product-icon"
        style={{ backgroundColor: product.corPrimaria || '#f0f0f0' }}
      >
        {product.icone ? (
          <span className="icon">{product.icone}</span>
        ) : (
          <span className="icon-placeholder">📦</span>
        )}
      </div>

      {/* Nome e Descrição */}
      <div className="product-info">
        <h3 className="product-name">{product.produtoNome}</h3>
        <p className="product-description">
          {product.descricao || 'Produto sem descrição'}
        </p>
      </div>

      {/* Badge de Status */}
      <div className="product-status">
        {!isLaunched && (
          <span className="badge badge-coming-soon">Em breve</span>
        )}
        {isLaunched && !hasAccess && (
          <span className="badge badge-no-access">Sem acesso</span>
        )}
        {isLaunched && hasAccess && (
          <span className="badge badge-active">Ativo</span>
        )}
      </div>
    </div>
  );
};
