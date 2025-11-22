import React, { useState } from 'react';
import { Sidebar } from '../Sidebar/Sidebar';
import { Header } from '../Header/Header';
import { DrawerMenu } from '../DrawerMenu/DrawerMenu';
import './MainLayout.css';

interface MainLayoutProps {
  children: React.ReactNode;
  currentProductName?: string;
}

export const MainLayout: React.FC<MainLayoutProps> = ({ 
  children, 
  currentProductName = 'Painel'
}) => {
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  return (
    <div className="main-layout">
      <Sidebar />
      <DrawerMenu 
        isOpen={isDrawerOpen} 
        onClose={() => setIsDrawerOpen(false)}
        productName={currentProductName}
      />
      <button 
        className={`floating-menu-button ${isDrawerOpen ? 'open' : ''}`}
        onClick={() => setIsDrawerOpen(!isDrawerOpen)}
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
          <line x1="3" y1="12" x2="21" y2="12"></line>
          <line x1="3" y1="6" x2="21" y2="6"></line>
          <line x1="3" y1="18" x2="21" y2="18"></line>
        </svg>
      </button>
      <div className="layout-content">
        <Header 
          currentProductName={currentProductName}
        />
        <main className="layout-main">
          {children}
        </main>
      </div>
    </div>
  );
};
