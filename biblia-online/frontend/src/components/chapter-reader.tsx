"use client";

import { useEffect, useMemo } from "react";
import Link from "next/link";
import { Heart, HeartOff } from "lucide-react";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useBibleStore } from "@/stores/bible-store";
import type { BibleVersion, Chapter, ReadingHistoryItem } from "@/lib/types";
import { cn } from "@/lib/utils";

function verseFavoriteId(versionCode: string, bookSlug: string, chapterNumber: number, verseNumber: number) {
  return `${versionCode}:${bookSlug}:${chapterNumber}:${verseNumber}`;
}

export function ChapterReader({
  version,
  chapter,
}: {
  version: BibleVersion;
  chapter: Chapter;
}) {
  const favorites = useBibleStore((state) => state.favorites);
  const toggleFavorite = useBibleStore((state) => state.toggleFavorite);
  const addHistory = useBibleStore((state) => state.addHistory);
  const cacheChapter = useBibleStore((state) => state.cacheChapter);

  useEffect(() => {
    cacheChapter(chapter);

    const historyItem: ReadingHistoryItem = {
      id: `${version.code}:${chapter.bookSlug}:${chapter.chapterNumber}`,
      bibleVersionId: version.id,
      versionCode: version.code,
      bookId: chapter.bookId,
      bookSlug: chapter.bookSlug,
      bookTitle: chapter.bookTitle,
      chapterNumber: chapter.chapterNumber,
      lastReadAtUtc: new Date().toISOString(),
    };

    addHistory(historyItem);
  }, [addHistory, cacheChapter, chapter, version.code, version.id]);

  const favoriteSet = useMemo(
    () => new Set(favorites.map((favorite) => favorite.id)),
    [favorites],
  );

  const chapterFavoriteCount = favorites.filter(
    (favorite) =>
      favorite.versionCode === version.code &&
      favorite.bookSlug === chapter.bookSlug &&
      favorite.chapterNumber === chapter.chapterNumber,
  ).length;

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>{`${chapter.bookTitle} ${chapter.chapterNumber}`}</CardTitle>
          <CardDescription>
            {version.name} • {chapter.verses.length} versículos
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-wrap items-center gap-3">
            <span className="rounded-full bg-muted px-3 py-1 text-xs font-semibold uppercase tracking-[0.22em] text-muted-foreground">
              {version.code}
            </span>
            <span className="rounded-full bg-primary/10 px-3 py-1 text-xs font-semibold uppercase tracking-[0.22em] text-primary">
              {chapterFavoriteCount} favorito(s)
            </span>
            <Link
              href={`/buscar?version=${encodeURIComponent(version.code)}`}
              className={cn(buttonVariants({ size: "sm", variant: "outline" }), "rounded-full")}
            >
              Buscar nesta versão
            </Link>
          </div>
          <p className="text-sm leading-7 text-muted-foreground">
            Favorite versículos individuais para revisitar depois. Navegação entre capítulos ao final da página.
          </p>
        </CardContent>
      </Card>

      <div className="grid gap-4">
        {chapter.verses.map((verse) => {
          const id = verseFavoriteId(version.code, chapter.bookSlug, chapter.chapterNumber, verse.verseNumber);
          const isFavorite = favoriteSet.has(id);

          return (
            <article
              key={verse.verseNumber}
              className="group rounded-3xl border border-border bg-muted/10 p-5 transition hover:border-primary/40"
            >
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="text-xs uppercase tracking-[0.24em] text-muted-foreground">Versículo {verse.verseNumber}</p>
                </div>
                <Button
                  size="icon"
                  variant={isFavorite ? "secondary" : "ghost"}
                  onClick={() =>
                    toggleFavorite({
                      id,
                      bibleVersionId: version.id,
                      versionCode: version.code,
                      bookId: chapter.bookId,
                      bookSlug: chapter.bookSlug,
                      bookTitle: chapter.bookTitle,
                      chapterNumber: chapter.chapterNumber,
                      verseNumber: verse.verseNumber,
                      createdAtUtc: new Date().toISOString(),
                    })
                  }
                  aria-label={isFavorite ? "Remover favorito" : "Adicionar aos favoritos"}
                >
                  {isFavorite ? <Heart className="size-4 text-destructive" /> : <HeartOff className="size-4" />}
                </Button>
              </div>
              <p className="mt-4 text-base leading-8 text-foreground">{verse.text}</p>
            </article>
          );
        })}
      </div>
    </div>
  );
}
