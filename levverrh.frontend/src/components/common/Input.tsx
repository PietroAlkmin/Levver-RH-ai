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
      <div>
        {label && (
          <label htmlFor={props.id} className="auth-label">
            {label}
            {props.required && <span style={{ color: 'var(--error-color)', marginLeft: '0.25rem' }}>*</span>}
          </label>
        )}

        <input
          ref={ref}
          className={`auth-input ${error ? 'auth-input-error' : ''} ${className}`}
          style={{
            borderColor: error ? 'var(--error-color)' : undefined,
            background: props.disabled ? 'var(--background-secondary)' : undefined,
            cursor: props.disabled ? 'not-allowed' : undefined,
          }}
          {...props}
        />

        {error && (
          <p className="auth-error">{error}</p>
        )}

        {helperText && !error && (
          <p className="auth-error" style={{ color: 'var(--text-secondary)' }}>{helperText}</p>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';
