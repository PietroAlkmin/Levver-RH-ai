import { useEffect } from 'react';
import { useAuthStore } from '../stores/authStore';

export const useWhiteLabel = () => {
  const whiteLabel = useAuthStore((state) => state.whiteLabel);

  useEffect(() => {
    if (whiteLabel) {
      const root = document.documentElement;
      
      // Aplicar cores do tenant
      root.style.setProperty('--primary-color', whiteLabel.primaryColor);
      root.style.setProperty('--secondary-color', whiteLabel.secondaryColor);
      root.style.setProperty('--accent-color', whiteLabel.accentColor);
      root.style.setProperty('--background-color', whiteLabel.backgroundColor);
      root.style.setProperty('--text-color', whiteLabel.textColor);
      root.style.setProperty('--border-color', whiteLabel.borderColor);

      // Atualizar favicon
      if (whiteLabel.faviconUrl) {
        const favicon = document.querySelector<HTMLLinkElement>("link[rel*='icon']");
        if (favicon) favicon.href = whiteLabel.faviconUrl;
      }

      // Atualizar título do sistema
      if (whiteLabel.systemName) {
        document.title = whiteLabel.systemName;
      }
    }
  }, [whiteLabel]);
};
