import { ReactNode } from 'react';
import './MetricCard.css';

interface MetricCardProps {
  icon: ReactNode;
  label: string;
  value: number | string;
  subtitle?: string;
  trend?: {
    value: number;
    isPositive: boolean;
  };
}

export const MetricCard = ({ icon, label, value, subtitle, trend }: MetricCardProps) => {
  return (
    <div className="metric-card">
      <div className="metric-card-header">
        <div className="metric-icon">{icon}</div>
        {trend && (
          <span className={`metric-trend ${trend.isPositive ? 'positive' : 'negative'}`}>
            {trend.isPositive ? '↑' : '↓'} {trend.value}%
          </span>
        )}
      </div>
      <div className="metric-content">
        <h3 className="metric-value">{value}</h3>
        <p className="metric-label">{label}</p>
        {subtitle && <p className="metric-subtitle">{subtitle}</p>}
      </div>
    </div>
  );
};
