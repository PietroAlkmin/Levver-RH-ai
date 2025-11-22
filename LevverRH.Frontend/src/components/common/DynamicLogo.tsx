import React from 'react';
import { useAuthStore } from '../../stores/authStore';

interface DynamicLogoProps {
  size?: number;
  className?: string;
}

export const DynamicLogo: React.FC<DynamicLogoProps> = ({ size = 64, className = '' }) => {
  const whiteLabel = useAuthStore((state) => state.whiteLabel);
  const [imageError, setImageError] = React.useState(false);

  // Antes do login, mostra logo padrão Levver
  const logoUrl = whiteLabel?.logoUrl 
    ? `${whiteLabel.logoUrl}.png` 
    : 'https://levverstorage.blob.core.windows.net/logos/levver-logo.png';
  
  const systemName = whiteLabel?.systemName || 'levver.ai';
  const primaryColor = whiteLabel?.primaryColor || '#713BDB';

  return (
    <div className="text-center mb-8">
      {!imageError ? (
        <img
          src={logoUrl}
          alt={systemName}
          className={`mx-auto ${className}`}
          style={{
            height: 'auto',
            maxWidth: '280px',
            width: '100%'
          }}
          onError={() => setImageError(true)}
        />
      ) : (
        /* Fallback "V" se imagem falhar */
        <div
          className={`inline-flex items-center justify-center rounded-full mx-auto ${className}`}
          style={{
            width: size,
            height: size,
            backgroundColor: primaryColor,
          }}
        >
          <span className="text-white font-bold" style={{ fontSize: size * 0.6 }}>
            V
          </span>
        </div>
      )}
    </div>
  );
};
