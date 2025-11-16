import React from 'react';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import './PainelDashboard.css';

export const PainelDashboard: React.FC = () => {
  return (
    <MainLayout currentProductName="Painel principal">
      <div className="painel-dashboard">
        <div className="dashboard-welcome">
          <h1>Bem-vindo ao Painel Principal</h1>
          <p className="dashboard-subtitle">
            Acompanhe as principais métricas e estatísticas dos seus produtos
          </p>
        </div>

        {/* Área de widgets - preparada para futuras implementações */}
        <div className="dashboard-widgets">
          <div className="widget-placeholder">
            <div className="placeholder-icon">📊</div>
            <h3>Widgets em breve</h3>
            <p>Esta área exibirá métricas e gráficos dos produtos ativos</p>
          </div>
        </div>
      </div>
    </MainLayout>
  );
};
