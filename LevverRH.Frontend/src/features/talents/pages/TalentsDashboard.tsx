import React, { useEffect, useState } from 'react';
import { 
  Briefcase, 
  Users, 
  FileText, 
  TrendingUp,
  Clock,
  CheckCircle,
  AlertCircle
} from 'lucide-react';
import { talentsService } from '../services/talentsService';
import { DashboardStatsDTO, JobDTO } from '../types/talents.types';

/**
 * 🎯 Levver Talents - Dashboard
 * Visão geral do produto Multi-Sourcing de Talentos
 */
const TalentsDashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStatsDTO | null>(null);
  const [recentJobs, setRecentJobs] = useState<JobDTO[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      
      // Buscar estatísticas
      const statsData = await talentsService.getDashboardStats();
      setStats(statsData);

      // Buscar vagas recentes (primeiras 5)
      const jobsData = await talentsService.getAllJobs();
      setRecentJobs(jobsData.slice(0, 5));
    } catch (error) {
      console.error('Erro ao carregar dashboard:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Carregando dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">
            🎯 Levver Talents
          </h1>
          <p className="text-gray-600 mt-1">
            Multi-Sourcing de Talentos - Dashboard
          </p>
        </div>
        <button className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg font-medium transition-colors">
          + Nova Vaga
        </button>
      </div>

      {/* Cards de Estatísticas */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white rounded-lg shadow-md p-6 border-l-4 border-purple-500">
          <div className="flex items-center justify-between mb-2">
            <div className="bg-purple-100 rounded-full p-3">
              <Briefcase className="text-purple-600" size={24} />
            </div>
          </div>
          <p className="text-3xl font-bold text-gray-900">{stats?.vagasAbertas || 0}</p>
          <p className="text-sm text-gray-600 mt-1">Vagas Abertas</p>
          <p className="text-xs text-green-600 mt-2">
            {stats?.vagasAbertas || 0} ativas
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6 border-l-4 border-blue-500">
          <div className="flex items-center justify-between mb-2">
            <div className="bg-blue-100 rounded-full p-3">
              <Users className="text-blue-600" size={24} />
            </div>
          </div>
          <p className="text-3xl font-bold text-gray-900">{stats?.totalCandidaturas || 0}</p>
          <p className="text-sm text-gray-600 mt-1">Candidaturas</p>
          <p className="text-xs text-blue-600 mt-2">
            {stats?.candidaturasNovas || 0} novas
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6 border-l-4 border-green-500">
          <div className="flex items-center justify-between mb-2">
            <div className="bg-green-100 rounded-full p-3">
              <CheckCircle className="text-green-600" size={24} />
            </div>
          </div>
          <p className="text-3xl font-bold text-gray-900">{stats?.entrevistasAgendadas || 0}</p>
          <p className="text-sm text-gray-600 mt-1">Entrevistas Agendadas</p>
          <p className="text-xs text-green-600 mt-2">
            Próximas semanas
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6 border-l-4 border-orange-500">
          <div className="flex items-center justify-between mb-2">
            <div className="bg-orange-100 rounded-full p-3">
              <TrendingUp className="text-orange-600" size={24} />
            </div>
          </div>
          <p className="text-3xl font-bold text-gray-900">{stats?.taxaConversao?.toFixed(1) || 0}%</p>
          <p className="text-sm text-gray-600 mt-1">Taxa de Conversão</p>
          <p className="text-xs text-orange-600 mt-2">
            Candidato → Aprovado
          </p>
        </div>
      </div>

      {/* Vagas Recentes */}
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-bold text-gray-900">
            📋 Vagas Recentes
          </h3>
          <a href="/talents/jobs" className="text-purple-600 hover:text-purple-700 text-sm font-medium">
            Ver todas →
          </a>
        </div>

        <div className="space-y-3">
          {recentJobs.length === 0 ? (
            <div className="text-center py-8 text-gray-500">
              <FileText size={48} className="mx-auto mb-2 opacity-30" />
              <p>Nenhuma vaga cadastrada ainda</p>
            </div>
          ) : (
            recentJobs.map((job) => (
              <div 
                key={job.id} 
                className="flex items-center justify-between p-4 border border-gray-200 rounded-lg hover:border-purple-300 hover:bg-purple-50 transition-all cursor-pointer"
              >
                <div className="flex items-center gap-4">
                  <div className="bg-purple-100 rounded-lg p-3">
                    <Briefcase className="text-purple-600" size={20} />
                  </div>
                  <div>
                    <h4 className="font-semibold text-gray-900">{job.titulo}</h4>
                    <div className="flex items-center gap-3 mt-1">
                      <span className="text-xs text-gray-500 flex items-center gap-1">
                        <Clock size={12} />
                        {new Date(job.dataCriacao).toLocaleDateString('pt-BR')}
                      </span>
                      <span className="text-xs text-gray-500">
                        {job.localizacao || 'Remoto'}
                      </span>
                    </div>
                  </div>
                </div>
                <div className="flex items-center gap-4">
                  <div className="text-right">
                    <p className="text-sm font-semibold text-gray-900">
                      {job.totalCandidaturas || 0} candidatos
                    </p>
                    <span className={`text-xs px-2 py-1 rounded-full ${
                      job.status === 'Aberta' 
                        ? 'bg-green-100 text-green-700' 
                        : 'bg-gray-100 text-gray-700'
                    }`}>
                      {job.status === 'Aberta' ? 'Ativa' : job.status}
                    </span>
                  </div>
                  {job.totalCandidaturas && job.totalCandidaturas > 5 && (
                    <AlertCircle className="text-orange-500" size={20} />
                  )}
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {/* Ações Rápidas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-gradient-to-br from-purple-500 to-purple-600 rounded-lg p-6 text-white">
          <Briefcase size={32} className="mb-3" />
          <h3 className="text-lg font-bold mb-2">Gerenciar Vagas</h3>
          <p className="text-sm opacity-90 mb-4">
            Crie, edite e publique vagas de emprego
          </p>
          <button className="bg-white text-purple-600 px-4 py-2 rounded-lg font-medium text-sm hover:bg-purple-50 transition-colors">
            Acessar →
          </button>
        </div>

        <div className="bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg p-6 text-white">
          <Users size={32} className="mb-3" />
          <h3 className="text-lg font-bold mb-2">Ver Candidatos</h3>
          <p className="text-sm opacity-90 mb-4">
            Analise candidaturas e gerencie processos seletivos
          </p>
          <button className="bg-white text-blue-600 px-4 py-2 rounded-lg font-medium text-sm hover:bg-blue-50 transition-colors">
            Acessar →
          </button>
        </div>

        <div className="bg-gradient-to-br from-green-500 to-green-600 rounded-lg p-6 text-white">
          <FileText size={32} className="mb-3" />
          <h3 className="text-lg font-bold mb-2">Relatórios</h3>
          <p className="text-sm opacity-90 mb-4">
            Acompanhe métricas e gere relatórios
          </p>
          <button className="bg-white text-green-600 px-4 py-2 rounded-lg font-medium text-sm hover:bg-green-50 transition-colors">
            Acessar →
          </button>
        </div>
      </div>
    </div>
  );
};

export default TalentsDashboard;
