"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { api } from "@/lib/api-client";
import type { Favorite, ReadingHistoryItem } from "@/lib/types";
import { useAuthStore } from "@/stores/auth-store";

async function fetchFavorites(): Promise<Favorite[]> {
  const res = await api.get<Favorite[]>("/me/favorites");
  return res.data;
}

async function fetchHistory(): Promise<ReadingHistoryItem[]> {
  const res = await api.get<ReadingHistoryItem[]>("/me/reading-history", { params: { take: 50 } });
  return res.data;
}

export default function AccountPage() {
  const user = useAuthStore((s) => s.user);

  const favoritesQuery = useQuery({
    queryKey: ["me", "favorites"],
    queryFn: fetchFavorites,
    enabled: !!user,
  });

  const historyQuery = useQuery({
    queryKey: ["me", "history"],
    queryFn: fetchHistory,
    enabled: !!user,
  });

  if (!user) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle>Conta</CardTitle>
            <CardDescription>Você precisa entrar para ver favoritos e histórico.</CardDescription>
          </CardHeader>
          <CardContent>
            <Link href="/login" className={cn(buttonVariants())}>
              Ir para login
            </Link>
          </CardContent>
        </Card>
      </div>
    );
  }

  const favorites = favoritesQuery.data ?? [];
  const history = historyQuery.data ?? [];

  return (
    <div className="mx-auto max-w-5xl space-y-8 px-4 py-10">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">Conta</h1>
        <p className="text-muted-foreground">
          Olá, <span className="font-medium text-foreground">{user.displayName}</span> ({user.email})
        </p>
      </header>

      <Card>
        <CardHeader>
          <CardTitle>Favoritos</CardTitle>
          <CardDescription>Versículos salvos.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          {favoritesQuery.isError ? <p className="text-sm text-red-600">Não foi possível carregar favoritos.</p> : null}
          {favorites.length === 0 ? <p className="text-sm text-muted-foreground">Nenhum favorito ainda.</p> : null}
          {favorites.map((f) => (
            <div key={f.id} className="flex flex-col gap-2 rounded-lg border p-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="text-sm">
                <div className="font-medium">
                  {f.bookTitle} {f.chapterNumber}:{f.verseNumber}{" "}
                  <span className="text-muted-foreground">({f.versionCode})</span>
                </div>
              </div>
              <Link
                href={`/read/${encodeURIComponent(f.versionCode)}/${encodeURIComponent(f.bookSlug)}/${f.chapterNumber}`}
                className={cn(buttonVariants({ size: "sm", variant: "outline" }))}
              >
                Abrir
              </Link>
            </div>
          ))}
        </CardContent>
      </Card>

      <Separator />

      <Card>
        <CardHeader>
          <CardTitle>Histórico</CardTitle>
          <CardDescription>Últimos capítulos registrados.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          {historyQuery.isError ? <p className="text-sm text-red-600">Não foi possível carregar histórico.</p> : null}
          {history.length === 0 ? <p className="text-sm text-muted-foreground">Nenhum histórico ainda.</p> : null}
          {history.map((h) => (
            <div key={h.id} className="flex flex-col gap-2 rounded-lg border p-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="text-sm">
                <div className="font-medium">
                  {h.bookTitle} {h.chapterNumber}{" "}
                  <span className="text-muted-foreground">({h.versionCode})</span>
                </div>
                <div className="text-xs text-muted-foreground">{new Date(h.lastReadAtUtc).toLocaleString()}</div>
              </div>
              <Link
                href={`/read/${encodeURIComponent(h.versionCode)}/${encodeURIComponent(h.bookSlug)}/${h.chapterNumber}`}
                className={cn(buttonVariants({ size: "sm", variant: "outline" }))}
              >
                Continuar
              </Link>
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
