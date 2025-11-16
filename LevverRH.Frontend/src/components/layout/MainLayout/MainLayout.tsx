import React from 'react';
import { Sidebar } from '../Sidebar/Sidebar';
import { Header } from '../Header/Header';
import './MainLayout.css';

interface MainLayoutProps {
  children: React.ReactNode;
  currentProductName?: string;
}

export const MainLayout: React.FC<MainLayoutProps> = ({ 
  children, 
  currentProductName 
}) => {
  return (
    <div className="main-layout">
      <Sidebar />
      <div className="layout-content">
        <Header currentProductName={currentProductName} />
        <main className="layout-main">
          {children}
        </main>
      </div>
    </div>
  );
};
