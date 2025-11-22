import React from 'react';
import { Briefcase } from 'lucide-react';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';

const TalentsDashboard: React.FC = () => {
  return (
    <MainLayout currentProductName="Talents">
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: 'calc(100vh - 80px)',
        textAlign: 'center'
      }}>
        <div>
          <div style={{ 
            marginBottom: '1rem',
            color: '#6B7280'
          }}>
            <Briefcase size={64} strokeWidth={1.5} />
          </div>
          <h2 style={{ 
            fontSize: '1.5rem', 
            fontWeight: 600,
            color: '#6B7280',
            margin: 0
          }}>
            Dashboard em desenvolvimento
          </h2>
        </div>
      </div>
    </MainLayout>
  );
};

export default TalentsDashboard;
