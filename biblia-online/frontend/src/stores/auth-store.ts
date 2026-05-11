import { create } from "zustand";
import { persist } from "zustand/middleware";

export type AuthUser = {
  id: string;
  email: string;
  displayName: string;
};

type AuthState = {
  token: string | null;
  expiresAtUtc: string | null;
  user: AuthUser | null;
  setSession: (token: string, expiresAtUtc: string, user: AuthUser) => void;
  clearSession: () => void;
};

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      expiresAtUtc: null,
      user: null,
      setSession: (token, expiresAtUtc, user) => set({ token, expiresAtUtc, user }),
      clearSession: () => set({ token: null, expiresAtUtc: null, user: null }),
    }),
    { name: "biblia-online-auth" },
  ),
);
