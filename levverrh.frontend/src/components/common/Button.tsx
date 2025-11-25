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
  const baseStyles = 'font-semibold transition-all duration-200 flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed';

  const variantStyles = {
    primary: 'text-white',
    secondary: 'text-white',
    danger: 'text-white',
    outline: 'bg-white',
  };

  const getVariantStyle = (variant: string) => {
    switch(variant) {
      case 'primary':
        return {
          background: 'linear-gradient(135deg, #713BDB 0%, #CC12EF 100%)',
          borderRadius: '8px',
          border: 'none'
        };
      case 'secondary':
        return {
          background: '#6B7280',
          borderRadius: '8px',
          border: 'none'
        };
      case 'danger':
        return {
          background: '#E84358',
          borderRadius: '8px',
          border: 'none'
        };
      case 'outline':
        return {
          border: '1px solid #E5E7EB',
          borderRadius: '8px',
          color: '#111827'
        };
      default:
        return {};
    }
  };

  const sizeStyles = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2 text-base',
    lg: 'px-6 py-3 text-lg',
  };

  const widthStyle = fullWidth ? 'w-full' : '';

  return (
    <button
      className={`${baseStyles} ${variantStyles[variant]} ${sizeStyles[size]} ${widthStyle} ${className}`}
      style={getVariantStyle(variant)}
   disabled={disabled || isLoading}
      {...props}
    >
      {isLoading && <Loader2 className="animate-spin" size={18} />}
      {children}
    </button>
  );
};
