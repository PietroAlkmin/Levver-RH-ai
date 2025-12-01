import React, { useCallback, useState } from 'react';
import { Upload, FileText, X } from 'lucide-react';
import './FileUploadResume.css';

interface FileUploadResumeProps {
  onFileSelect: (file: File | null) => void;
  error?: string;
}

const MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB
const ALLOWED_TYPES = ['.pdf'];

export const FileUploadResume: React.FC<FileUploadResumeProps> = ({ onFileSelect, error }) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [validationError, setValidationError] = useState<string>('');

  const validateFile = (file: File): string | null => {
    if (file.size > MAX_FILE_SIZE) {
      return 'Arquivo muito grande. Tamanho máximo: 5MB';
    }

    const extension = '.' + file.name.split('.').pop()?.toLowerCase();
    if (!ALLOWED_TYPES.includes(extension)) {
      return 'Tipo de arquivo não permitido. Aceito apenas: PDF';
    }

    return null;
  };

  const handleFileChange = useCallback((file: File | null) => {
    if (!file) {
      setSelectedFile(null);
      setValidationError('');
      onFileSelect(null);
      return;
    }

    const error = validateFile(file);
    if (error) {
      setValidationError(error);
      setSelectedFile(null);
      onFileSelect(null);
      return;
    }

    setValidationError('');
    setSelectedFile(file);
    onFileSelect(file);
  }, [onFileSelect]);

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(true);
  };

  const handleDragLeave = () => {
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);

    const files = e.dataTransfer.files;
    if (files.length > 0) {
      handleFileChange(files[0]);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      handleFileChange(files[0]);
    }
  };

  const handleRemoveFile = () => {
    handleFileChange(null);
  };

  const displayError = error || validationError;

  return (
    <div className="file-upload-container">
      {!selectedFile ? (
        <div
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          className={`file-upload-dropzone ${isDragging ? 'dragging' : ''} ${displayError ? 'error' : ''}`}
        >
          <label htmlFor="resume-upload" className="file-upload-label">
            <Upload size={32} />
            <div className="file-upload-text">
              <p className="file-upload-title">Arraste seu currículo ou clique para selecionar</p>
              <p className="file-upload-subtitle">Apenas PDF - Máximo 5MB</p>
            </div>
          </label>
          <input
            id="resume-upload"
            type="file"
            accept=".pdf"
            onChange={handleInputChange}
            className="file-upload-input"
          />
        </div>
      ) : (
        <div className="file-selected">
          <div className="file-info">
            <FileText size={24} />
            <div className="file-details">
              <p className="file-name">{selectedFile.name}</p>
              <p className="file-size">{(selectedFile.size / 1024 / 1024).toFixed(2)} MB</p>
            </div>
          </div>
          <button type="button" onClick={handleRemoveFile} className="file-remove">
            <X size={18} />
          </button>
        </div>
      )}

      {displayError && <p className="file-error">{displayError}</p>}
    </div>
  );
};
