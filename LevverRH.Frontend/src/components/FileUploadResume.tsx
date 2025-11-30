import React, { useCallback, useState } from 'react';
import { FileText, Upload, X } from 'lucide-react';

interface FileUploadResumeProps {
  onFileSelect: (file: File | null) => void;
  error?: string;
}

const MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB
const ALLOWED_TYPES = ['.pdf', '.docx'];

export const FileUploadResume: React.FC<FileUploadResumeProps> = ({ onFileSelect, error }) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [validationError, setValidationError] = useState<string>('');

  const validateFile = (file: File): string | null => {
    // Validar tamanho
    if (file.size > MAX_FILE_SIZE) {
      return 'Arquivo muito grande. Tamanho máximo: 5MB';
    }

    // Validar tipo
    const extension = '.' + file.name.split('.').pop()?.toLowerCase();
    if (!ALLOWED_TYPES.includes(extension)) {
      return 'Tipo de arquivo não permitido. Aceito apenas: PDF, DOCX';
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
    <div className="space-y-2">
      {!selectedFile ? (
        <div
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          className={`
            border-2 border-dashed rounded-lg p-8 text-center cursor-pointer
            transition-colors duration-200
            ${isDragging 
              ? 'border-blue-500 bg-blue-50' 
              : displayError
              ? 'border-red-300 bg-red-50'
              : 'border-gray-300 hover:border-gray-400'
            }
          `}
        >
          <label htmlFor="resume-upload" className="cursor-pointer">
            <div className="flex flex-col items-center space-y-3">
              <Upload className={`w-12 h-12 ${isDragging ? 'text-blue-500' : 'text-gray-400'}`} />
              <div>
                <p className="text-sm font-medium text-gray-700">
                  Arraste seu currículo ou clique para selecionar
                </p>
                <p className="text-xs text-gray-500 mt-1">
                  PDF ou DOCX - Máximo 5MB
                </p>
              </div>
            </div>
          </label>
          <input
            id="resume-upload"
            type="file"
            accept=".pdf,.docx"
            onChange={handleInputChange}
            className="hidden"
          />
        </div>
      ) : (
        <div className="border border-gray-300 rounded-lg p-4 bg-gray-50">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <FileText className="w-8 h-8 text-blue-500" />
              <div>
                <p className="text-sm font-medium text-gray-700">{selectedFile.name}</p>
                <p className="text-xs text-gray-500">
                  {(selectedFile.size / 1024 / 1024).toFixed(2)} MB
                </p>
              </div>
            </div>
            <button
              type="button"
              onClick={handleRemoveFile}
              className="p-1 hover:bg-gray-200 rounded transition-colors"
            >
              <X className="w-5 h-5 text-gray-500" />
            </button>
          </div>
        </div>
      )}

      {displayError && (
        <p className="text-sm text-red-600">{displayError}</p>
      )}
    </div>
  );
};
