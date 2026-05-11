"use client";

import { useMutation } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { api } from "@/lib/api-client";
import { useAuthStore } from "@/stores/auth-store";
import type { Chapter } from "@/lib/types";

export function ReaderVerseList({ chapter }: { chapter: Chapter }) {
  const token = useAuthStore((s) => s.token);

  const touchHistory = useMutation({
    mutationFn: async () => {
      await api.post("/me/reading-history", {
        bibleVersionId: chapter.bibleVersionId,
        bookId: chapter.bookId,
        chapterNumber: chapter.chapterNumber,
      });
    },
  });

  const addFavorite = useMutation({
    mutationFn: async (verseNumber: number) => {
      await api.post("/me/favorites", {
        bibleVersionId: chapter.bibleVersionId,
        bookId: chapter.bookId,
        chapterNumber: chapter.chapterNumber,
        verseNumber,
      });
    },
  });

  return (
    <div className="space-y-4">
      {token ? (
        <div className="flex items-center justify-between gap-3 rounded-lg border bg-muted/30 p-3">
          <p className="text-sm text-muted-foreground">
            Autenticado: registre este capítulo no seu histórico de leitura.
          </p>
          <Button
            size="sm"
            variant="secondary"
            disabled={touchHistory.isPending}
            onClick={() => touchHistory.mutate()}
          >
            Registrar leitura
          </Button>
        </div>
      ) : (
        <p className="text-sm text-muted-foreground">
          Faça login para salvar histórico e favoritos por versículo.
        </p>
      )}

      <Separator />

      <div className="space-y-6">
        {chapter.verses.map((v) => (
          <section key={v.verseNumber} className="flex gap-3">
            <div className="w-10 shrink-0 pt-1 text-right text-sm font-semibold text-muted-foreground">
              {v.verseNumber}
            </div>
            <div className="flex-1 space-y-2">
              <p className="leading-relaxed text-[17px]">{v.text}</p>
              {token ? (
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  disabled={addFavorite.isPending}
                  onClick={() => addFavorite.mutate(v.verseNumber)}
                >
                  Favoritar versículo
                </Button>
              ) : null}
            </div>
          </section>
        ))}
      </div>
    </div>
  );
}
