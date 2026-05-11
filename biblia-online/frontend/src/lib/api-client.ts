import axios from "axios";
import { getPublicApiUrl } from "@/lib/env";
import { useAuthStore } from "@/stores/auth-store";

export const api = axios.create({
  baseURL: `${getPublicApiUrl().replace(/\/$/, "")}/api/v1`,
});

api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token)
    config.headers.Authorization = `Bearer ${token}`;
  return config;
});
