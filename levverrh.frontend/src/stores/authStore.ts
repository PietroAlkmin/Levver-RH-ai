import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import { UserInfo, TenantInfo, WhiteLabelInfo } from '../types/auth.types';

interface AuthState {
  // State
  token: string | null;
  user: UserInfo | null;
  tenant: TenantInfo | null;
  whiteLabel: WhiteLabelInfo | null;
  isAuthenticated: boolean;
  isLoading: boolean;

  // Actions
  setAuth: (token: string, user: UserInfo, tenant: TenantInfo, whiteLabel?: WhiteLabelInfo | null) => void;
  clearAuth: () => void;
  setLoading: (isLoading: boolean) => void;
  updateUser: (user: Partial<UserInfo>) => void;
}

/**
 * Auth Store - Gerenciamento de estado de autenticação
 * Usa Zustand com persistência no localStorage
 */
export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      set => ({
        // Initial State
        token: null,
        user: null,
        tenant: null,
        whiteLabel: null,
        isAuthenticated: false,
        isLoading: false,

        // Actions
        setAuth: (token, user, tenant, whiteLabel = null) =>
          set({
            token,
      user,
            tenant,
  whiteLabel,
     isAuthenticated: true,
            isLoading: false,
      }),

    clearAuth: () =>
          set({
          token: null,
            user: null,
    tenant: null,
whiteLabel: null,
            isAuthenticated: false,
    isLoading: false,
          }),

      setLoading: isLoading => set({ isLoading }),

        updateUser: user =>
       set(state => ({
            user: state.user ? { ...state.user, ...user } : null,
          })),
}),
      {
        name: 'auth-storage',
      partialize: state => ({
          token: state.token,
  user: state.user,
          tenant: state.tenant,
    whiteLabel: state.whiteLabel,
          isAuthenticated: state.isAuthenticated,
        }),
      }
    ),
    { name: 'AuthStore' }
  )
);
