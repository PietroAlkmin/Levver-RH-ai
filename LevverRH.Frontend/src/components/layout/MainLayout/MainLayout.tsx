import React from 'react';
import { Sidebar } from '../Sidebar/Sidebar';
import { Header } from '../Header/Header';
import './MainLayout.css';

interface MainLayoutProps {
  children: React.ReactNode;
}

export const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
  return (
    <div className="main-layout">
      <Sidebar />
      <div className="layout-content">
        <Header />
        <main className="layout-main">
          {children}
        </main>
      </div>
    </div>
  );
};
