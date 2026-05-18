"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { ThemeToggle } from "@/components/theme-toggle";
import { api } from "@/lib/api-client";
import { useAuthStore } from "@/stores/auth-store";
import { useBibleStore } from "@/stores/bible-store";
import type { BibleVersion } from "@/lib/types";

interface BeforeInstallPromptEvent extends Event {
  prompt: () => Promise<void>;
  userChoice: Promise<{ outcome: "accepted" | "dismissed"; platform: string }>;
}

export function SiteHeader() {
  const router = useRouter();
  const user = useAuthStore((s) => s.user);
  const clearSession = useAuthStore((s) => s.clearSession);
  const selectedVersionCode = useBibleStore((s) => s.selectedVersionCode);
  const setSelectedVersionCode = useBibleStore((s) => s.setSelectedVersionCode);
  const [installPromptEvent, setInstallPromptEvent] = useState<BeforeInstallPromptEvent | null>(null);
  const [isOnline, setIsOnline] = useState(true);

  async function fetchVersions(): Promise<BibleVersion[]> {
    const response = await api.get<BibleVersion[]>('/versions');
    return response.data;
  }

  const versionsQuery = useQuery({
    queryKey: ['header-versions'],
    queryFn: fetchVersions,
  });

  useEffect(() => {
    const handleBeforeInstallPrompt = (event: Event) => {
      const installEvent = event as BeforeInstallPromptEvent;
      event.preventDefault();
      setInstallPromptEvent(installEvent);
    };

    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);

    window.addEventListener("beforeinstallprompt", handleBeforeInstallPrompt);
    window.addEventListener("online", handleOnline);
    window.addEventListener("offline", handleOffline);

    setIsOnline(navigator.onLine);

    return () => {
      window.removeEventListener("beforeinstallprompt", handleBeforeInstallPrompt);
      window.removeEventListener("online", handleOnline);
      window.removeEventListener("offline", handleOffline);
    };
  }, []);

  const installApp = async () => {
    if (!installPromptEvent) return;
    await installPromptEvent.prompt();
    await installPromptEvent.userChoice;
    setInstallPromptEvent(null);
  };

  return (
    <header className="border-b bg-background/90 backdrop-blur supports-[backdrop-filter]:bg-background/65">
      <div className="mx-auto flex max-w-5xl flex-wrap items-center justify-between gap-3 px-4 py-4">
        <div className="flex items-center gap-3">
          <Link href="/" className="text-lg font-semibold tracking-tight">
            Biblia Online
          </Link>
          <label htmlFor="selected-version" className="sr-only">
            Selecionar versão
          </label>
          <select
            id="selected-version"
            value={selectedVersionCode}
            onChange={(event) => setSelectedVersionCode(event.target.value)}
            className={cn(
              buttonVariants({ variant: "outline", size: "sm" }),
              "rounded-full bg-background px-3 py-1 text-xs font-medium text-foreground",
            )}
          >
            {versionsQuery.isLoading ? (
              <option>Carregando versões...</option>
            ) : versionsQuery.data?.length ? (
              versionsQuery.data.map((version) => (
                <option key={version.id} value={version.code}>
                  {version.name} ({version.code})
                </option>
              ))
            ) : (
              <option>Nenhuma versão disponível</option>
            )}
          </select>
        </div>

        <nav className="flex flex-wrap items-center gap-2">
          <Link href="/buscar" className={cn(buttonVariants({ variant: "ghost" }))}>
            Buscar
          </Link>
          <Link href="/favoritos" className={cn(buttonVariants({ variant: "ghost" }))}>
            Favoritos
          </Link>
          <Link href="/perfil" className={cn(buttonVariants({ variant: "ghost" }))}>
            Perfil
          </Link>
          <span className={cn(buttonVariants({ variant: "ghost", size: "sm" }), "rounded-full px-3 py-1 text-xs")}>{isOnline ? "Online" : "Offline"}</span>
          {installPromptEvent ? (
            <button onClick={installApp} className={cn(buttonVariants({ variant: "outline", size: "sm" }))}>
              Instalar app
            </button>
          ) : null}
          <ThemeToggle />
          {user ? (
            <DropdownMenu>
              <DropdownMenuTrigger className={cn(buttonVariants({ variant: "outline" }))}>
                {user.displayName}
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => router.push("/perfil")}>Minha conta</DropdownMenuItem>
                <DropdownMenuItem
                  onClick={() => {
                    clearSession();
                    router.refresh();
                  }}
                >
                  Sair
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          ) : (
            <Link href="/login" className={cn(buttonVariants({ variant: "outline" }))}>
              Entrar
            </Link>
          )}
        </nav>
      </div>
    </header>
  );
}
