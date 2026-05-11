"use client";

import { useMemo, useState } from "react";
import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { Button, buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { api } from "@/lib/api-client";
import type { BibleVersion, SearchHit } from "@/lib/types";

async function fetchVersions(): Promise<BibleVersion[]> {
  const res = await api.get<BibleVersion[]>("/versions");
  return res.data;
}

async function searchVerses(q: string, versionId?: string): Promise<SearchHit[]> {
  const res = await api.get<SearchHit[]>("/search", {
    params: { q, versionId, limit: 25 },
  });
  return res.data;
}

export default function SearchPage() {
  const [q, setQ] = useState("");
  const [versionId, setVersionId] = useState<string | undefined>(undefined);

  const versionsQuery = useQuery({ queryKey: ["versions"], queryFn: fetchVersions });

  const enabled = q.trim().length >= 2;
  const searchQuery = useQuery({
    queryKey: ["search", q, versionId],
    queryFn: () => searchVerses(q.trim(), versionId),
    enabled,
  });

  const hits = searchQuery.data ?? [];

  const versionOptions = useMemo(() => {
    const versions = versionsQuery.data ?? [];
    return versions.map((v) => (
      <option key={v.id} value={v.id}>
        {v.name} ({v.code})
      </option>
    ));
  }, [versionsQuery.data]);

  return (
    <div className="mx-auto max-w-5xl space-y-6 px-4 py-10">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">Busca</h1>
        <p className="text-muted-foreground">Powered by Meilisearch no backend.</p>
      </header>

      <Card>
        <CardHeader>
          <CardTitle>Consulta</CardTitle>
          <CardDescription>Digite pelo menos 2 caracteres.</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="q">Texto</Label>
            <Input id="q" value={q} onChange={(e) => setQ(e.target.value)} placeholder="Ex.: amor, fé, princípio…" />
          </div>
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="version">Versão (opcional)</Label>
            <select
              id="version"
              className="h-10 w-full rounded-md border bg-background px-3 text-sm"
              value={versionId ?? ""}
              onChange={(e) => setVersionId(e.target.value || undefined)}
            >
              <option value="">Todas</option>
              {versionOptions}
            </select>
          </div>
          <div className="md:col-span-2">
            <Button type="button" disabled={!enabled || searchQuery.isFetching} onClick={() => searchQuery.refetch()}>
              Buscar
            </Button>
          </div>
        </CardContent>
      </Card>

      <div className="space-y-3">
        {searchQuery.isError ? (
          <p className="text-sm text-red-600">Falha ao buscar. Verifique se a API e o Meilisearch estão no ar.</p>
        ) : null}

        {enabled && searchQuery.isSuccess && hits.length === 0 ? (
          <p className="text-sm text-muted-foreground">Nenhum resultado.</p>
        ) : null}

        {hits.map((h) => (
          <Card key={`${h.bibleVersionId}-${h.bookSlug}-${h.chapterNumber}-${h.verseNumber}`}>
            <CardHeader className="pb-2">
              <CardTitle className="text-base">
                {h.bookTitle} {h.chapterNumber}:{h.verseNumber}{" "}
                <span className="text-muted-foreground">({h.versionCode})</span>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <p className="leading-relaxed">{h.formatted ?? h.text}</p>
              <Link
                href={`/read/${encodeURIComponent(h.versionCode)}/${encodeURIComponent(h.bookSlug)}/${h.chapterNumber}`}
                className={cn(buttonVariants({ size: "sm", variant: "outline" }))}
              >
                Abrir capítulo
              </Link>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
