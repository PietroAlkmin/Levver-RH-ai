import React from 'react';
import { Home } from 'lucide-react';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import './PainelDashboard.css';

export const PainelDashboard: React.FC = () => {
  return (
    <MainLayout>
      <div className="painel-dashboard">
        <div className="empty-state">
          <div className="empty-icon">
            <Home size={64} strokeWidth={1.5} />
          </div>
          <h2>Dashboard em desenvolvimento</h2>
        </div>
      </div>
    </MainLayout>
  );
};
