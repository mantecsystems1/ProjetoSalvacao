"use client";

import Link from "next/link";
import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { api } from "@/lib/api-client";
import { useBibleStore } from "@/stores/bible-store";
import type { BibleVersion } from "@/lib/types";
import { cn } from "@/lib/utils";

async function fetchVersions(): Promise<BibleVersion[]> {
  const response = await api.get<BibleVersion[]>("/versions");
  return response.data;
}

export default function ProfilePage() {
  const theme = useBibleStore((state) => state.theme);
  const selectedVersionCode = useBibleStore((state) => state.selectedVersionCode);
  const setSelectedVersionCode = useBibleStore((state) => state.setSelectedVersionCode);
  const history = useBibleStore((state) => state.history);
  const favorites = useBibleStore((state) => state.favorites);
  const clearHistory = useBibleStore((state) => state.clearHistory);

  const versionsQuery = useQuery({ queryKey: ["versions"], queryFn: fetchVersions });

  const selectedVersion = useMemo(
    () => versionsQuery.data?.find((version) => version.code === selectedVersionCode),
    [selectedVersionCode, versionsQuery.data],
  );

  const historyItems = useMemo(
    () => [...history].sort((a, b) => b.lastReadAtUtc.localeCompare(a.lastReadAtUtc)).slice(0, 10),
    [history],
  );

  return (
    <div className="mx-auto max-w-6xl space-y-8 px-4 py-10 sm:px-6 lg:px-8">
      <header className="space-y-3">
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Perfil</p>
        <h1 className="text-4xl font-semibold tracking-tight">Minha biblioteca</h1>
        <p className="max-w-2xl text-base leading-7 text-muted-foreground">
          Gerencie o tema, a versão padrão e acompanhe seu histórico de leitura offline.
        </p>
      </header>

      <div className="grid gap-4 xl:grid-cols-[1.2fr_0.8fr]">
        <Card>
          <CardHeader>
            <CardTitle>Preferências</CardTitle>
            <CardDescription>Escolha o tema e a versão de leitura padrão.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <p className="text-sm font-semibold text-foreground">Tema atual</p>
                <p className="text-sm text-muted-foreground">{theme === "dark" ? "Escuro" : "Claro"}</p>
              </div>
              <div className="space-y-2">
                <p className="text-sm font-semibold text-foreground">Versão padrão</p>
                <p className="text-sm text-muted-foreground">{selectedVersion?.name ?? selectedVersionCode}</p>
              </div>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <label className="grid gap-2 text-sm">
                <span>Selecionar versão padrão</span>
                <select
                  className="h-10 w-full rounded-md border bg-background px-3 text-sm"
                  value={selectedVersionCode}
                  onChange={(event) => setSelectedVersionCode(event.target.value)}
                >
                  {versionsQuery.data?.map((version) => (
                    <option key={version.id} value={version.code}>
                      {version.name} ({version.code})
                    </option>
                  ))}
                </select>
              </label>
              <div className="space-y-2">
                <p className="text-sm font-semibold text-foreground">Favoritos salvos</p>
                <p className="text-sm text-muted-foreground">{favorites.length} item(s)</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Histórico</CardTitle>
            <CardDescription>Últimos capítulos acessados.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            {historyItems.length === 0 ? (
              <p className="text-sm text-muted-foreground">Seu histórico aparecerá aqui assim que você ler um capítulo.</p>
            ) : (
              <div className="space-y-3">
                {historyItems.map((item) => (
                  <div key={item.id} className="rounded-3xl border border-border bg-background/70 p-4">
                    <p className="font-semibold text-foreground">
                      {item.bookTitle} {item.chapterNumber}
                    </p>
                    <p className="text-sm text-muted-foreground">Último acesso: {new Date(item.lastReadAtUtc).toLocaleString()}</p>
                    <p className="text-sm text-muted-foreground">Versão: {item.versionCode}</p>
                  </div>
                ))}
              </div>
            )}
            <button
              type="button"
              className={cn(buttonVariants({ variant: "outline", size: "sm" }))}
              onClick={clearHistory}
            >
              Limpar histórico
            </button>
          </CardContent>
        </Card>
      </div>

      <Separator />

      <Card>
        <CardHeader>
          <CardTitle>Atalhos rápidos</CardTitle>
          <CardDescription>Tenha seu fluxo de leitura sempre à mão.</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4 sm:grid-cols-3">
          <Link href="/buscar" className={cn(buttonVariants())}>
            Buscar versículos
          </Link>
          <Link href="/favoritos" className={cn(buttonVariants({ variant: "outline" }))}>
            Meus favoritos
          </Link>
          <Link href="/" className={cn(buttonVariants({ variant: "ghost" }))}>
            Página inicial
          </Link>
        </CardContent>
      </Card>
    </div>
  );
}
