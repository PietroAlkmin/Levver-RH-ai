import React from 'react';
import { Briefcase } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';

const TalentsDashboard: React.FC = () => {
  const navigate = useNavigate();

  return (
    <MainLayout>
      <div style={{
        padding: '2rem',
        minHeight: 'calc(100vh - 64px)'
      }}>
        <div style={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: 'calc(100vh - 64px - 4rem)',
          textAlign: 'center',
          gap: '2rem'
        }}>
          <div style={{ 
            marginBottom: '1rem',
            color: '#6B7280',
            display: 'flex',
            justifyContent: 'center'
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

          <div style={{
            display: 'flex',
            gap: '1rem',
            marginTop: '1rem'
          }}>
            <button
              onClick={() => navigate('/talents/vagas')}
              style={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                color: 'white',
                border: 'none',
                padding: '12px 24px',
                borderRadius: '8px',
                fontSize: '14px',
                fontWeight: 500,
                cursor: 'pointer',
                transition: 'transform 0.2s',
                boxShadow: '0 4px 12px rgba(102, 126, 234, 0.3)'
              }}
              onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-2px)'}
              onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
            >
              Ver Vagas
            </button>

            <button
              onClick={() => navigate('/talents/jobs/new')}
              style={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                color: 'white',
                border: 'none',
                padding: '12px 24px',
                borderRadius: '8px',
                fontSize: '14px',
                fontWeight: 500,
                cursor: 'pointer',
                transition: 'transform 0.2s',
                boxShadow: '0 4px 12px rgba(102, 126, 234, 0.3)'
              }}
              onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-2px)'}
              onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
            >
              Criar Vaga com IA
            </button>
          </div>
        </div>
      </div>
    </MainLayout>
  );
};

export default TalentsDashboard;

