import React from 'react';
import { Sidebar } from '../Sidebar/Sidebar';
import { Header } from '../Header/Header';
import './MainLayout.css';

interface MainLayoutProps {
  children: React.ReactNode;
  showHeader?: boolean;
}

export const MainLayout: React.FC<MainLayoutProps> = ({ children, showHeader = true }) => {
  return (
    <div className="main-layout">
      <Sidebar />
      <div className="layout-content">
        {showHeader && <Header />}
        <main className="layout-main">
          {children}
        </main>
      </div>
    </div>
  );
};
