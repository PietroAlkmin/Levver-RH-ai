import { X, Upload, Camera } from 'lucide-react';
import { useState, useRef } from 'react';
import { useAuthStore } from '../../../stores/authStore';
import apiClient from '../../../services/api';
import './SettingsModal.css';

const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
const MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB

interface SettingsModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function SettingsModal({ isOpen, onClose }: SettingsModalProps) {
  const { user, updateUserPhoto } = useAuthStore();
  const [previewUrl, setPreviewUrl] = useState<string | null>(user?.fotoUrl || null);
  const [isUploading, setIsUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  if (!isOpen) return null;

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validar tipo de arquivo - apenas fotos
    if (!ALLOWED_IMAGE_TYPES.includes(file.type.toLowerCase())) {
      alert('Apenas imagens JPG, PNG ou GIF são permitidas');
      if (fileInputRef.current) fileInputRef.current.value = '';
      return;
    }

    // Validar tamanho
    if (file.size > MAX_FILE_SIZE) {
      alert('A imagem deve ter no máximo 5MB');
      if (fileInputRef.current) fileInputRef.current.value = '';
      return;
    }

    // Criar preview local
    const reader = new FileReader();
    reader.onloadend = () => {
      setPreviewUrl(reader.result as string);
    };
    reader.readAsDataURL(file);

    // Simular upload (aqui você faria o upload real para o backend)
    handleUpload(file);
  };

  const handleUpload = async (file: File) => {
    setIsUploading(true);
    try {
      const formData = new FormData();
      formData.append('file', file);

      const { data } = await apiClient.post('/user/photo', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });

      updateUserPhoto(data.fotoUrl);
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao fazer upload da foto';
      alert(message);
      setPreviewUrl(user?.fotoUrl || null);
      if (fileInputRef.current) fileInputRef.current.value = '';
    } finally {
      setIsUploading(false);
    }
  };

  const handleRemovePhoto = async () => {
    try {
      await apiClient.delete('/user/photo');
      setPreviewUrl(null);
      updateUserPhoto(null);
      if (fileInputRef.current) fileInputRef.current.value = '';
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao remover foto';
      alert(message);
    }
  };

  return (
    <div className="settings-modal-overlay" onClick={onClose}>
      <div className="settings-modal" onClick={(e) => e.stopPropagation()}>
        <button className="settings-modal-close" onClick={onClose}>
          <X size={24} />
        </button>

        <div className="settings-modal-container">
          <div className="settings-sidebar">
            <button className="settings-sidebar-item active">
              Perfil
            </button>
          </div>

          <div className="settings-content">
            <div className="settings-section">
              <h2 className="settings-title">Informações do Perfil</h2>
              <p className="settings-description">Personalize sua foto de perfil</p>

              <div className="profile-photo-section">
                <div className="profile-photo-preview">
                  {previewUrl ? (
                    <img src={previewUrl} alt="Foto de perfil" className="profile-photo-image" />
                  ) : (
                    <div className="profile-photo-placeholder">
                      <Camera size={40} />
                    </div>
                  )}
                </div>

                <div className="profile-photo-actions">
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept=".jpg,.jpeg,.png,.gif"
                    onChange={handleFileSelect}
                    className="profile-photo-input"
                    id="photo-upload"
                  />
                  <label htmlFor="photo-upload" className="profile-photo-button primary">
                    <Upload size={16} />
                    {isUploading ? 'Enviando...' : 'Alterar foto'}
                  </label>
                  {previewUrl && (
                    <button 
                      onClick={handleRemovePhoto}
                      className="profile-photo-button secondary"
                      disabled={isUploading}
                    >
                      Remover foto
                    </button>
                  )}
                  <p className="profile-photo-hint">
                    JPG, PNG ou GIF • Máximo 5MB
                  </p>
                </div>
              </div>

              <div className="profile-info">
                <div className="profile-info-item">
                  <label className="profile-info-label">Nome</label>
                  <p className="profile-info-value">{user?.nome}</p>
                </div>
                <div className="profile-info-item">
                  <label className="profile-info-label">Email</label>
                  <p className="profile-info-value">{user?.email}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
