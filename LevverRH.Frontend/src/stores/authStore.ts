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
  _hasHydrated: boolean;

  // Actions
  setAuth: (token: string, user: UserInfo, tenant: TenantInfo, whiteLabel?: WhiteLabelInfo | null) => void;
  clearAuth: () => void;
  setLoading: (isLoading: boolean) => void;
  updateUser: (user: Partial<UserInfo>) => void;
  updateUserPhoto: (fotoUrl: string | null) => void;
  setHasHydrated: (hasHydrated: boolean) => void;
}

/**
 * Auth Store - Gerenciamento de estado de autentica��o
 * Usa Zustand com persist�ncia no localStorage
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
        _hasHydrated: false,

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

        updateUserPhoto: (fotoUrl: string | null) =>
          set(state => ({
            user: state.user ? { ...state.user, fotoUrl } : null,
          })),

        setHasHydrated: hasHydrated => set({ _hasHydrated: hasHydrated }),
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
        onRehydrateStorage: () => (state) => {
          state?.setHasHydrated(true);
        },
      }
    ),
    { name: 'AuthStore' }
  )
);
