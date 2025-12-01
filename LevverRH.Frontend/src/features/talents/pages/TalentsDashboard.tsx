import { useEffect } from 'react';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import { MetricCard } from '../components/MetricCard';
import { useDashboardStore } from '../../../stores/useDashboardStore';
import { Briefcase, Users, UserCheck, Calendar, TrendingUp } from 'lucide-react';
import './TalentsDashboard.css';

const TalentsDashboard = () => {
  const { stats, isLoading, error, fetchStats } = useDashboardStore();

  useEffect(() => {
    fetchStats();
  }, [fetchStats]);

  if (isLoading) {
    return (
      <MainLayout showHeader={false}>
        <div className="dashboard-container">
          <div className="dashboard-header">
            <h1>Painel de Controle</h1>
          </div>
          <div className="dashboard-content">
            <div className="loading-state">Carregando estatísticas...</div>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout showHeader={false}>
        <div className="dashboard-container">
          <div className="dashboard-header">
            <h1>Painel de Controle</h1>
          </div>
          <div className="dashboard-content">
            <div className="error-state">{error}</div>
          </div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout showHeader={false}>
      <div className="dashboard-container">
        {/* Header */}
        <div className="dashboard-header">
          <h1>Painel de Controle</h1>
        </div>

        {/* Content */}
        <div className="dashboard-content">
          <div className="metrics-grid">
            <MetricCard
              icon={<Briefcase />}
              label="Vagas Ativas"
              value={stats?.vagasAbertas || 0}
              subtitle="Vagas abertas para candidatura"
            />
            <MetricCard
              icon={<Users />}
              label="Total de Candidaturas"
              value={stats?.totalCandidaturas || 0}
              subtitle="Candidatos inscritos"
            />
            <MetricCard
              icon={<UserCheck />}
              label="Candidaturas Novas"
              value={stats?.candidaturasNovas || 0}
              subtitle="Aguardando análise"
            />
            <MetricCard
              icon={<Calendar />}
              label="Entrevistas Agendadas"
              value={stats?.entrevistasAgendadas || 0}
              subtitle="Em processo de entrevista"
            />
            <MetricCard
              icon={<TrendingUp />}
              label="Taxa de Conversão"
              value={`${stats?.taxaConversao || 0}%`}
              subtitle="Candidatos aprovados"
            />
          </div>
        </div>
      </div>
    </MainLayout>
  );
};

export default TalentsDashboard;

