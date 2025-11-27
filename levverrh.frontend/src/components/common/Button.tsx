import React, { ButtonHTMLAttributes } from 'react';
import { Loader2 } from 'lucide-react';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'outline';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  fullWidth?: boolean;
}

/**
 * Componente Button reutiliz�vel
 * Segue padr�es de design system
 */
export const Button: React.FC<ButtonProps> = ({
  children,
  variant = 'primary',
  size = 'md',
  isLoading = false,
  fullWidth = false,
  disabled,
  className = '',
  ...props
}) => {
  const getButtonClass = () => {
    let classes = 'auth-button';
    
    if (variant === 'outline') classes = 'auth-button-outline';
    if (variant === 'secondary') classes = 'auth-button-secondary';
    if (variant === 'danger') classes = 'auth-button-danger';
    
    if (size === 'sm') classes += ' auth-button-sm';
    if (size === 'lg') classes += ' auth-button-lg';
    
    if (!fullWidth) classes = classes.replace('width: 100%', '');
    
    return `${classes} ${className}`;
  };

  return (
    <button
      className={getButtonClass()}
      style={fullWidth ? undefined : { width: 'auto' }}
      disabled={disabled || isLoading}
      {...props}
    >
      {isLoading && <Loader2 className="animate-spin" size={18} />}
      {children}
    </button>
  );
};
