import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ProductCard } from '../components/ProductCard';
import { productService } from '../services/productService';
import { Product } from '../types/product.types';
import './PainelDashboard.css';

export const PainelDashboard: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    console.log('📦 PainelDashboard - Componente montado');
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      console.log('📦 PainelDashboard - Iniciando carregamento de produtos...');
      setLoading(true);
      setError(null);
      const data = await productService.getAllProducts();
      console.log('✅ PainelDashboard - Produtos carregados:', data);
      setProducts(data);
    } catch (err) {
      console.error('❌ PainelDashboard - Erro ao carregar produtos:', err);
      setError('Não foi possível carregar os produtos. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  const handleProductClick = (product: Product) => {
    if (product.rotaBase) {
      navigate(product.rotaBase);
    }
  };

  if (loading) {
    return (
      <div className="painel-container">
        <div className="loading-state">
          <div className="spinner"></div>
          <p>Carregando produtos...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="painel-container">
        <div className="error-state">
          <p className="error-message">{error}</p>
          <button onClick={loadProducts} className="retry-button">
            Tentar novamente
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="painel-container">
      <header className="painel-header">
        <h1>Painel de Produtos</h1>
        <p className="subtitle">
          Gerencie e acesse seus produtos contratados
        </p>
      </header>

      {products.length === 0 ? (
        <div className="empty-state">
          <div className="empty-icon">📦</div>
          <h2>Nenhum produto disponível</h2>
          <p>Entre em contato com o suporte para ativar produtos.</p>
        </div>
      ) : (
        <div className="products-grid">
          {products.map((product) => (
            <ProductCard
              key={product.id}
              product={product}
              onClick={() => handleProductClick(product)}
            />
          ))}
        </div>
      )}
    </div>
  );
};
