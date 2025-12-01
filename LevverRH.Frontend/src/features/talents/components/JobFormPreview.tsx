import React from 'react';
import { JobDetailDTO, ContractType, WorkModel } from '../types/talents.types';
import './JobFormPreview.css';

export interface JobFormPreviewProps {
  jobData: Partial<JobDetailDTO>;
  onFieldChange: (fieldName: string, value: string | number | string[] | undefined) => void;
  completionPercentage?: number;
  onSaveDraft?: () => void;
  onPublish?: () => void;
  isLoading?: boolean;
}

/**
 * Componente de Pré-visualização do Formulário
 * Lado direito da tela de criação de vaga
 */
export const JobFormPreview: React.FC<JobFormPreviewProps> = ({
  jobData,
  onFieldChange,
  completionPercentage = 0,
  onSaveDraft,
  onPublish,
  isLoading = false
}) => {
  const handleInputChange = (fieldName: string, value: string | number | string[] | undefined) => {
    onFieldChange(fieldName, value);
  };

  const handleArrayChange = (fieldName: string, value: string) => {
    const array = value.split(',').map(item => item.trim()).filter(item => item);
    onFieldChange(fieldName, array);
  };

  // Converte DateTime ISO para formato YYYY-MM-DD aceito por input[type="date"]
  const formatDateForInput = (dateString?: string): string => {
    if (!dateString) return '';
    try {
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return '';
      return date.toISOString().split('T')[0]; // YYYY-MM-DD
    } catch {
      return '';
    }
  };

  return (
    <div className="job-form-container">
      <div className="job-form-header">
        <h2>Preview da Vaga</h2>
      </div>

      <div className="job-form-content">
        {/* Informações Básicas */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Informações Básicas</h3>

          <div className="job-form-field">
            <label className="job-form-label">
              Título da Vaga <span className="job-form-required">*</span>
              <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', fontWeight: 'normal', marginLeft: '0.5rem' }}>
                (nome que aparece na lista de vagas)
              </span>
            </label>
            <input
              type="text"
              value={jobData.titulo || ''}
              onChange={(e) => handleInputChange('titulo', e.target.value)}
              className="job-form-input"
              placeholder="Ex: Vendedor Externo, Analista de Marketing Pleno"
            />
          </div>

          <div className="job-form-row">
            <div className="job-form-field">
              <label className="job-form-label">Departamento</label>
              <input
                type="text"
                value={jobData.departamento || ''}
                onChange={(e) => handleInputChange('departamento', e.target.value)}
                className="job-form-input"
                placeholder="Ex: Tecnologia"
              />
            </div>

            <div className="job-form-field">
              <label className="job-form-label">Número de Vagas</label>
              <input
                type="number"
                value={jobData.numeroVagas || ''}
                onChange={(e) => handleInputChange('numeroVagas', parseInt(e.target.value) || undefined)}
                className="job-form-input"
                min="1"
                placeholder="Ex: 1, 2, 3..."
              />
            </div>
          </div>
        </section>

        {/* Localização */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Localização</h3>

          <div className="job-form-row">
            <div className="job-form-field">
              <label className="job-form-label">Cidade</label>
              <input
                type="text"
                value={jobData.cidade || ''}
                onChange={(e) => handleInputChange('cidade', e.target.value)}
                className="job-form-input"
                placeholder="Ex: São Paulo"
              />
            </div>

            <div className="job-form-field">
              <label className="job-form-label">Estado</label>
              <input
                type="text"
                value={jobData.estado || ''}
                onChange={(e) => handleInputChange('estado', e.target.value)}
                className="job-form-input"
                placeholder="Ex: SP"
                maxLength={2}
              />
            </div>
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Localização Completa</label>
            <input
              type="text"
              value={jobData.localizacao || ''}
              onChange={(e) => handleInputChange('localizacao', e.target.value)}
              className="job-form-input"
              placeholder="Ex: Av. Paulista, 1000 - São Paulo/SP"
            />
          </div>
        </section>

        {/* Formato de Trabalho */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Formato de Trabalho</h3>

          <div className="job-form-row">
            <div className="job-form-field">
              <label className="job-form-label">Tipo de Contrato</label>
              <select
                value={jobData.tipoContrato || ''}
                onChange={(e) => handleInputChange('tipoContrato', e.target.value)}
                className="job-form-select"
              >
                <option value="">Selecione...</option>
                {Object.values(ContractType).map((type) => (
                  <option key={type} value={type}>
                    {type}
                  </option>
                ))}
              </select>
            </div>

            <div className="job-form-field">
              <label className="job-form-label">Modelo de Trabalho</label>
              <select
                value={jobData.modeloTrabalho || ''}
                onChange={(e) => handleInputChange('modeloTrabalho', e.target.value)}
                className="job-form-select"
              >
                <option value="">Selecione...</option>
                {Object.values(WorkModel).map((model) => (
                  <option key={model} value={model}>
                    {model}
                  </option>
                ))}
              </select>
            </div>
          </div>
        </section>

        {/* Requisitos */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Requisitos</h3>

          <div className="job-form-field">
            <label className="job-form-label">Anos de Experiência (mínimo)</label>
            <input
              type="number"
              value={jobData.anosExperienciaMinimo || ''}
              onChange={(e) => handleInputChange('anosExperienciaMinimo', parseInt(e.target.value) || 0)}
              className="job-form-input"
              min="0"
              placeholder="Ex: 3"
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Formação Necessária</label>
            <input
              type="text"
              value={jobData.formacaoNecessaria || ''}
              onChange={(e) => handleInputChange('formacaoNecessaria', e.target.value)}
              className="job-form-input"
              placeholder="Ex: Ensino Superior em Ciências da Computação"
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Conhecimentos Obrigatórios</label>
            <input
              type="text"
              value={jobData.conhecimentosObrigatorios?.join(', ') || ''}
              onChange={(e) => handleArrayChange('conhecimentosObrigatorios', e.target.value)}
              className="job-form-input"
              placeholder="Separe por vírgula: React, TypeScript, Node.js"
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Conhecimentos Desejáveis</label>
            <input
              type="text"
              value={jobData.conhecimentosDesejaveis?.join(', ') || ''}
              onChange={(e) => handleArrayChange('conhecimentosDesejaveis', e.target.value)}
              className="job-form-input"
              placeholder="Separe por vírgula: GraphQL, Docker, AWS"
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Competências Importantes</label>
            <input
              type="text"
              value={jobData.competenciasImportantes?.join(', ') || ''}
              onChange={(e) => handleArrayChange('competenciasImportantes', e.target.value)}
              className="job-form-input"
              placeholder="Separe por vírgula: Trabalho em equipe, Comunicação"
            />
          </div>
        </section>

        {/* Responsabilidades */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Responsabilidades</h3>

          <div className="job-form-field">
            <label className="job-form-label">Principais Responsabilidades</label>
            <textarea
              value={jobData.responsabilidades || ''}
              onChange={(e) => handleInputChange('responsabilidades', e.target.value)}
              className="job-form-textarea"
              placeholder="Descreva as principais responsabilidades..."
              rows={4}
            />
          </div>
        </section>

        {/* Remuneração */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Remuneração</h3>

          <div className="job-form-row">
            <div className="job-form-field">
              <label className="job-form-label">Salário Mínimo (R$)</label>
              <input
                type="number"
                value={jobData.salarioMin || ''}
                onChange={(e) => handleInputChange('salarioMin', parseFloat(e.target.value) || 0)}
                className="job-form-input"
                min="0"
                step="0.01"
                placeholder="Ex: 5000.00"
              />
            </div>

            <div className="job-form-field">
              <label className="job-form-label">Salário Máximo (R$)</label>
              <input
                type="number"
                value={jobData.salarioMax || ''}
                onChange={(e) => handleInputChange('salarioMax', parseFloat(e.target.value) || 0)}
                className="job-form-input"
                min="0"
                step="0.01"
                placeholder="Ex: 8000.00"
              />
            </div>
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Benefícios</label>
            <textarea
              value={jobData.beneficios || ''}
              onChange={(e) => handleInputChange('beneficios', e.target.value)}
              className="job-form-textarea"
              placeholder="Ex: VR, VT, Plano de Saúde..."
              rows={3}
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Bônus/Comissão</label>
            <input
              type="text"
              value={jobData.bonusComissao || ''}
              onChange={(e) => handleInputChange('bonusComissao', e.target.value)}
              className="job-form-input"
              placeholder="Ex: Bônus anual de até 2 salários"
            />
          </div>
        </section>

        {/* Processo Seletivo */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Processo Seletivo</h3>

          <div className="job-form-field">
            <label className="job-form-label">Etapas do Processo</label>
            <input
              type="text"
              value={jobData.etapasProcesso?.join(', ') || ''}
              onChange={(e) => handleArrayChange('etapasProcesso', e.target.value)}
              className="job-form-input"
              placeholder="Separe por vírgula: Triagem, Entrevista RH, Teste Técnico"
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Tipos de Teste/Entrevista</label>
            <input
              type="text"
              value={jobData.tiposTesteEntrevista?.join(', ') || ''}
              onChange={(e) => handleArrayChange('tiposTesteEntrevista', e.target.value)}
              className="job-form-input"
              placeholder="Separe por vírgula: Teste técnico, Entrevista comportamental"
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Previsão de Início</label>
            <input
              type="date"
              value={formatDateForInput(jobData.previsaoInicio)}
              onChange={(e) => handleInputChange('previsaoInicio', e.target.value)}
              className="job-form-input"
            />
          </div>
        </section>

        {/* Sobre a Vaga */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Sobre a Vaga</h3>

          <div className="job-form-field">
            <label className="job-form-label">Sobre o Time</label>
            <textarea
              value={jobData.sobreTime || ''}
              onChange={(e) => handleInputChange('sobreTime', e.target.value)}
              className="job-form-textarea"
              placeholder="Conte sobre o time..."
              rows={3}
            />
          </div>

          <div className="job-form-field">
            <label className="job-form-label">Diferenciais</label>
            <textarea
              value={jobData.diferenciais || ''}
              onChange={(e) => handleInputChange('diferenciais', e.target.value)}
              className="job-form-textarea"
              placeholder="Diferenciais da empresa/vaga..."
              rows={3}
            />
          </div>
        </section>

        {/* Descrição (ÚLTIMA SEÇÃO - Gerada pela IA) */}
        <section className="job-form-section">
          <h3 className="job-form-section-title">Descrição Final da Vaga</h3>

          <div className="job-form-field">
            <label className="job-form-label">
              Descrição <span className="job-form-required">*</span>
            </label>
            <textarea
              value={jobData.descricao || ''}
              onChange={(e) => handleInputChange('descricao', e.target.value)}
              className="job-form-textarea"
              placeholder="A IA irá gerar a descrição final baseada nas informações coletadas..."
              rows={6}
            />
          </div>
        </section>
      </div>

      {/* Footer com Botões de Ação */}
      {(onSaveDraft || onPublish) && (
        <div className="job-form-footer">
          {onSaveDraft && (
            <button
              onClick={onSaveDraft}
              disabled={isLoading}
              className="job-form-button job-form-button-secondary"
            >
              Salvar Rascunho
            </button>
          )}
          {onPublish && (
            <button
              onClick={onPublish}
              disabled={isLoading}
              className="job-form-button job-form-button-primary"
            >
              Publicar Vaga
            </button>
          )}
        </div>
      )}
    </div>
  );
};
