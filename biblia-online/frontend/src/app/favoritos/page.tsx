"use client";

import Link from "next/link";
import { useMemo } from "react";
import { buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useBibleStore } from "@/stores/bible-store";
import { cn } from "@/lib/utils";

export default function FavoritesPage() {
  const favorites = useBibleStore((state) => state.favorites);
  const removeFavorite = useBibleStore((state) => state.removeFavorite);

  const sortedFavorites = useMemo(
    () => [...favorites].sort((a, b) => b.createdAtUtc.localeCompare(a.createdAtUtc)),
    [favorites],
  );

  return (
    <div className="mx-auto max-w-6xl space-y-8 px-4 py-10 sm:px-6 lg:px-8">
      <header className="space-y-3">
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Favoritos</p>
        <h1 className="text-4xl font-semibold tracking-tight">Versículos salvos</h1>
        <p className="max-w-2xl text-base leading-7 text-muted-foreground">
          Acesse seus versículos favoritos mesmo offline. Toque nos corações durante a leitura para salvar.
        </p>
      </header>

      {sortedFavorites.length === 0 ? (
        <Card>
          <CardContent>
            <p className="text-sm text-muted-foreground">Você ainda não salvou nenhum favorito.</p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4">
          {sortedFavorites.map((favorite) => (
            <Card key={favorite.id}>
              <CardHeader>
                <CardTitle className="text-base">
                  {favorite.bookTitle} {favorite.chapterNumber}:{favorite.verseNumber}
                </CardTitle>
                <CardDescription>{favorite.versionCode}</CardDescription>
              </CardHeader>
              <CardContent className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                <div className="space-y-1 text-sm text-muted-foreground">
                  <p>Última vez salvo: {new Date(favorite.createdAtUtc).toLocaleString()}</p>
                  <p>Livro: {favorite.bookSlug}</p>
                </div>
                <div className="flex flex-wrap gap-2">
                  <Link
                    href={`/${encodeURIComponent(favorite.bookSlug)}/${favorite.chapterNumber}?version=${encodeURIComponent(favorite.versionCode)}`}
                    className={cn(buttonVariants({ size: "sm" }))}
                  >
                    Ler capítulo
                  </Link>
                  <button
                    type="button"
                    className={cn(buttonVariants({ size: "sm", variant: "outline" }))}
                    onClick={() => removeFavorite(favorite.id)}
                  >
                    Remover
                  </button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
