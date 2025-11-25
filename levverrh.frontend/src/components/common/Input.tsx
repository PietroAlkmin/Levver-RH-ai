import React, { InputHTMLAttributes, forwardRef } from 'react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
}

/**
 * Componente Input reutiliz�vel
 * Integrado com React Hook Form
 */
export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, helperText, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
        <label className="block text-sm font-medium mb-1" style={{ color: '#111827' }}>
            {label}
            {props.required && <span className="ml-1" style={{ color: '#E84358' }}>*</span>}
  </label>
 )}

        <input
          ref={ref}
    className={`w-full px-4 py-2 outline-none transition-all ${props.disabled ? 'cursor-not-allowed' : ''} ${className}`}
          style={{
            border: error ? '1px solid #E84358' : '1px solid #E5E7EB',
            borderRadius: '8px',
            background: props.disabled ? '#F9FAFB' : 'white',
            color: '#111827'
          }}
          onFocus={(e) => !error && (e.target.style.border = '1px solid #713BDB')}
          onBlur={(e) => !error && (e.target.style.border = '1px solid #E5E7EB')}
          {...props}
        />

        {error && (
          <p className="mt-1 text-sm" style={{ color: '#E84358' }}>{error}</p>
        )}

        {helperText && !error && (
          <p className="mt-1 text-sm" style={{ color: '#6B7280' }}>{helperText}</p>
        )}
      </div>
  );
  }
);

Input.displayName = 'Input';
