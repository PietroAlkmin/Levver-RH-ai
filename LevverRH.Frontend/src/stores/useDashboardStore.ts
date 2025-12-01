import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import { dashboardService, DashboardStats } from '../services/dashboardService';

interface DashboardState {
  // State
  stats: DashboardStats | null;
  isLoading: boolean;
  error: string | null;

  // Actions
  fetchStats: () => Promise<void>;
  clearError: () => void;
}

export const useDashboardStore = create<DashboardState>()(
  devtools(
    (set) => ({
      // Initial State
      stats: null,
      isLoading: false,
      error: null,

      // Actions
      fetchStats: async () => {
        set({ isLoading: true, error: null });
        try {
          const stats = await dashboardService.getStats();
          set({ stats, isLoading: false });
        } catch (error: any) {
          set({ 
            error: error.response?.data?.message || 'Erro ao carregar estatísticas',
            isLoading: false 
          });
        }
      },

      clearError: () => set({ error: null }),
    }),
    { name: 'dashboard-store' }
  )
);
