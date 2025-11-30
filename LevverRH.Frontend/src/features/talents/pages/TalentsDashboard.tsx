import React from 'react';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import './TalentsDashboard.css';

const TalentsDashboard: React.FC = () => {
  return (
    <MainLayout>
      <div className="talents-dashboard">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Dashboard Talents</h1>
          <p className="dashboard-subtitle">Visão geral do seu processo de recrutamento</p>
        </div>

        <div className="dashboard-content">
          {/* Conteúdo será adicionado futuramente */}
        </div>
      </div>
    </MainLayout>
  );
};

export default TalentsDashboard;

