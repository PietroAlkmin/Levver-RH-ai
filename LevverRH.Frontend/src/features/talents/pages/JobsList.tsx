import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { Search, Plus, Briefcase, Activity, MapPin, X, Eye, Share2 } from 'lucide-react';
import { JobDTO } from '../types/talents.types';
import { talentsService } from '../services/talentsService';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import './JobsList.css';

export const JobsList: React.FC = () => {
  const navigate = useNavigate();
  const [jobs, setJobs] = useState<JobDTO[]>([]);
  const [loading, setLoading] = useState(true);
  
  // Filtros
  const [searchTerm, setSearchTerm] = useState('');
  const [departmentFilter, setDepartmentFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [locationFilter, setLocationFilter] = useState('');

  useEffect(() => {
    loadJobs();
  }, []);

  const loadJobs = async () => {
    try {
      setLoading(true);
      const data = await talentsService.getAllJobs();
      setJobs(data.filter(job => (job.iaCompletionPercentage ?? 0) >= 80));
    } catch (err) {
      toast.error('Erro ao carregar vagas');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNewJob = () => navigate('/talents/jobs/new');
  const handleViewJob = (jobId: string) => navigate(`/talents/vagas/${jobId}`);

  const handleShareJob = (jobId: string, e: React.MouseEvent) => {
    e.stopPropagation();
    navigator.clipboard.writeText(`${window.location.origin}/candidatura/${jobId}`)
      .then(() => toast.success('Link copiado!'))
      .catch(() => toast.error('Erro ao copiar'));
  };

  const clearFilters = () => {
    setSearchTerm('');
    setDepartmentFilter('');
    setStatusFilter('');
    setLocationFilter('');
  };

  // Extrair valores únicos do banco
  const departments = [...new Set(jobs.map(j => j.departamento).filter(Boolean))];
  const statuses = [...new Set(jobs.map(j => j.status).filter(Boolean))];
  const locations = [...new Set(jobs.map(j => j.localizacao).filter(Boolean))];

  // Filtrar vagas
  const filteredJobs = jobs.filter(job => {
    const matchesSearch = job.titulo.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesDept = !departmentFilter || job.departamento === departmentFilter;
    const matchesStatus = !statusFilter || job.status === statusFilter;
    const matchesLocation = !locationFilter || job.localizacao === locationFilter;
    return matchesSearch && matchesDept && matchesStatus && matchesLocation;
  });

  if (loading) {
    return (
      <MainLayout showHeader={false}>
        <div className="jobs-list-container">
          <div className="jobs-list-header">
            <h1>Vagas</h1>
          </div>
          <div className="loading-state">Carregando...</div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout showHeader={false}>
      <div className="jobs-list-container">
        {/* Header */}
        <div className="jobs-list-header">
          <h1>Vagas</h1>
          <button className="btn-create-job" onClick={handleCreateNewJob}>
            <Plus size={18} />
            Criar Vaga
          </button>
        </div>

        <div className="jobs-list-content">
          {/* Card de Busca e Filtros */}
          <div className="filters-card">
            <div className="search-row">
              <Search size={20} />
              <input
                type="text"
                placeholder="Buscar vagas..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="search-input"
              />
              {searchTerm && (
                <button className="search-clear-btn" onClick={() => setSearchTerm('')}>
                  <X size={16} />
                </button>
              )}
            </div>

            <div className="filters-row">
              <FilterDropdown
                icon={<Briefcase size={18} />}
                placeholder="Departamento"
                value={departmentFilter}
                onChange={setDepartmentFilter}
                options={departments}
              />
              <FilterDropdown
                icon={<Activity size={18} />}
                placeholder="Status"
                value={statusFilter}
                onChange={setStatusFilter}
                options={statuses}
              />
              <FilterDropdown
                icon={<MapPin size={18} />}
                placeholder="Localização"
                value={locationFilter}
                onChange={setLocationFilter}
                options={locations}
              />
              <button className="btn-clear" onClick={clearFilters}>
                <X size={18} />
                Limpar
              </button>
            </div>
          </div>

          {/* Tabela */}
          {filteredJobs.length === 0 ? (
            <div className="empty-state">
              <p>Nenhuma vaga encontrada</p>
            </div>
          ) : (
            <div className="jobs-table">
              <table>
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Data</th>
                    <th>Localização</th>
                    <th>Departamento</th>
                    <th>Status</th>
                    <th>Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredJobs.map(job => (
                    <tr key={job.id}>
                      <td className="job-name">{job.titulo}</td>
                      <td>{new Date(job.dataCriacao).toLocaleDateString('pt-BR')}</td>
                      <td>{job.localizacao || '-'}</td>
                      <td>{job.departamento || '-'}</td>
                      <td><span className={`status-badge status-${job.status.toLowerCase()}`}>{job.status}</span></td>
                      <td className="actions-cell">
                        <button className="btn-icon btn-primary" onClick={() => handleViewJob(job.id)} title="Ver vaga">
                          <Eye size={18} />
                        </button>
                        <button className="btn-icon" onClick={(e) => handleShareJob(job.id, e)} title="Copiar link">
                          <Share2 size={18} />
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </MainLayout>
  );
};

// Componente de Dropdown com Busca
interface FilterDropdownProps {
  icon: React.ReactNode;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
  options: string[];
}

const FilterDropdown: React.FC<FilterDropdownProps> = ({ icon, placeholder, value, onChange, options }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState('');

  const filteredOptions = options.filter(opt => 
    opt.toLowerCase().includes(search.toLowerCase())
  );

  const handleClear = (e: React.MouseEvent) => {
    e.stopPropagation();
    onChange('');
  };

  return (
    <div 
      className="filter-dropdown"
      onMouseEnter={() => setIsOpen(true)}
      onMouseLeave={() => setIsOpen(false)}
    >
      <button className="filter-trigger">
        {icon}
        <span>{value || placeholder}</span>
        {value && (
          <button className="filter-clear-btn" onClick={handleClear}>
            <X size={16} />
          </button>
        )}
      </button>
      {isOpen && (
        <div className="filter-menu">
          <div className="filter-search-wrapper">
            <input
              type="text"
              placeholder="Buscar..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="filter-search"
            />
            {search && (
              <button className="filter-search-clear" onClick={() => setSearch('')}>
                <X size={14} />
              </button>
            )}
          </div>
          <div className="filter-options">
            {filteredOptions.map(opt => (
              <div
                key={opt}
                className="filter-option"
                onClick={() => {
                  onChange(opt);
                  setIsOpen(false);
                  setSearch('');
                }}
              >
                {opt}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
