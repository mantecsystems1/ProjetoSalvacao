"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { ArrowLeft, ArrowRight, Search } from "lucide-react";
import { Button, buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { api } from "@/lib/api-client";
import type { BibleVersion, SearchResult } from "@/lib/types";

const PAGE_SIZE = 12;

async function fetchVersions(): Promise<BibleVersion[]> {
  const res = await api.get<BibleVersion[]>("/versions");
  return res.data;
}

async function searchVerses(q: string, versionId?: string, page = 1, pageSize = PAGE_SIZE): Promise<SearchResult> {
  const res = await api.get<SearchResult>("/search", {
    params: {
      q,
      versionId,
      page,
      pageSize,
    },
  });
  return res.data;
}

function buildHighlightPattern(query: string) {
  const normalized = query
    .trim()
    .replace(/[.*+?^${}()|[\]\\]/g, "")
    .split(/\s+/)
    .filter(Boolean);

  if (normalized.length === 0)
    return null;

  return new RegExp(`(${normalized.map((term) => term).join("|")})`, "gi");
}

function highlightText(text: string, query: string) {
  const pattern = buildHighlightPattern(query);
  if (!pattern)
    return text;

  const safePattern = new RegExp(pattern.source, pattern.flags.replace(/g/g, ""));
  return text.split(pattern).map((part, index) => {
    if (safePattern.test(part)) {
      return (
        <mark key={index} className="rounded bg-primary/20 px-0.5 text-primary">
          {part}
        </mark>
      );
    }

    return part;
  });
}

export default function BuscarPage() {
  const [query, setQuery] = useState("");
  const [debouncedQuery, setDebouncedQuery] = useState("");
  const [versionId, setVersionId] = useState<string | undefined>(undefined);
  const [page, setPage] = useState(1);

  useEffect(() => {
    const timer = setTimeout(() => setDebouncedQuery(query), 250);
    return () => clearTimeout(timer);
  }, [query]);

  useEffect(() => {
    setPage(1);
  }, [debouncedQuery, versionId]);

  const versionsQuery = useQuery({ queryKey: ["versions"], queryFn: fetchVersions });

  const enabled = debouncedQuery.trim().length >= 2;
  const searchQuery = useQuery({
    queryKey: ["search", debouncedQuery, versionId, page],
    queryFn: () => searchVerses(debouncedQuery.trim(), versionId, page, PAGE_SIZE),
    enabled,
  });

  const result = searchQuery.data;
  const hits = result?.items ?? [];
  const hasMore = result?.hasMore ?? false;

  const versionOptions = useMemo(() => {
    const versions = versionsQuery.data ?? [];
    return versions.map((version) => (
      <option key={version.id} value={version.id}>
        {version.name} ({version.code})
      </option>
    ));
  }, [versionsQuery.data]);

  return (
    <div className="mx-auto max-w-6xl space-y-8 px-4 py-10 sm:px-6 lg:px-8">
      <header className="space-y-3">
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Buscar</p>
        <h1 className="text-4xl font-semibold tracking-tight">Pesquisa ultra rápida de versículos</h1>
        <p className="max-w-2xl text-base leading-7 text-muted-foreground">
          Pesquise palavras, frases ou referências como <span className="font-semibold">João 3:16</span> e veja resultados instantâneos por relevância.
        </p>
      </header>

      <Card>
        <CardHeader>
          <CardTitle>Busca instantânea</CardTitle>
          <CardDescription>Resultados atualizados enquanto você digita.</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-[1.5fr_1fr]">
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="q">Pesquisar</Label>
            <div className="relative">
              <Input
                id="q"
                value={query}
                onChange={(event) => setQuery(event.target.value)}
                placeholder="Ex.: amor, fé, João 3:16, salmo 91"
              />
              <div className="pointer-events-none absolute inset-y-0 right-3 top-0 flex items-center text-muted-foreground">
                <Search className="h-4 w-4" />
              </div>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="version">Versão</Label>
            <select
              id="version"
              className="h-10 w-full rounded-md border bg-background px-3 text-sm"
              value={versionId ?? ""}
              onChange={(event) => setVersionId(event.target.value || undefined)}
            >
              <option value="">Todas</option>
              {versionOptions}
            </select>
          </div>

          <div className="md:col-span-2 flex items-center gap-3">
            <Button type="button" disabled={!enabled || searchQuery.isFetching} onClick={() => searchQuery.refetch()}>
              Buscar agora
            </Button>
            <p className="text-sm text-muted-foreground">
              {!enabled
                ? "Digite pelo menos 2 caracteres para iniciar a busca."
                : searchQuery.isFetching
                ? "Buscando..."
                : `Página ${page}${hasMore ? "+" : ""} • resultados parciais mostrados`}
            </p>
          </div>
        </CardContent>
      </Card>

      {enabled && hits.length > 0 ? (
        <Card>
          <CardHeader>
            <CardTitle>Sugestões</CardTitle>
            <CardDescription>Veja resultados relevantes imediatamente enquanto você digita.</CardDescription>
          </CardHeader>
          <CardContent className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            {hits.slice(0, 5).map((hit) => (
              <Link
                key={`${hit.bibleVersionId}-${hit.bookSlug}-${hit.chapterNumber}-${hit.verseNumber}`}
                href={`/${encodeURIComponent(hit.bookSlug)}/${hit.chapterNumber}?version=${encodeURIComponent(hit.versionCode)}`}
                className={cn(buttonVariants({ variant: "outline", size: "sm" }), "justify-start")}
              >
                <span className="font-semibold">
                  {hit.bookTitle} {hit.chapterNumber}:{hit.verseNumber}
                </span>
                <span className="ml-auto text-sm text-muted-foreground">{hit.versionCode}</span>
              </Link>
            ))}
          </CardContent>
        </Card>
      ) : null}

      <div className="space-y-4">
        {searchQuery.isError ? (
          <Card>
            <CardContent>
              <p className="text-sm text-red-600">Erro ao buscar. Confira a conexão com a API ou tente novamente.</p>
            </CardContent>
          </Card>
        ) : null}

        {enabled && !searchQuery.isFetching && hits.length === 0 ? (
          <Card>
            <CardContent>
              <p className="text-sm text-muted-foreground">Nenhum resultado encontrado para &quot;{debouncedQuery}&quot;.</p>
            </CardContent>
          </Card>
        ) : null}

        <div className="grid gap-4">
          {hits.map((hit) => (
            <Card key={`${hit.bibleVersionId}-${hit.bookSlug}-${hit.chapterNumber}-${hit.verseNumber}`}>
              <CardHeader className="pb-2">
                <CardTitle className="text-base">
                  {hit.bookTitle} {hit.chapterNumber}:{hit.verseNumber}
                  <span className="ml-2 text-sm text-muted-foreground">{hit.versionCode}</span>
                </CardTitle>
                <CardDescription className="text-sm text-muted-foreground">{hit.bookSlug}</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <p className="leading-7 text-foreground">{highlightText(hit.formatted ?? hit.text, debouncedQuery)}</p>
                <div className="flex flex-wrap gap-3">
                  <Link
                    href={`/${encodeURIComponent(hit.bookSlug)}/${hit.chapterNumber}?version=${encodeURIComponent(hit.versionCode)}`}
                    className={cn(buttonVariants({ size: "sm", variant: "outline" }), "rounded-full")}
                  >
                    Abrir capítulo
                  </Link>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {hasMore || page > 1 ? (
          <div className="flex items-center justify-between gap-3 rounded-3xl border border-border bg-muted p-4">
            <div className="text-sm text-muted-foreground">
              Página {page}{hasMore ? "+" : ""} • navegando entre páginas de resultados
            </div>
            <div className="flex items-center gap-2">
              <Button type="button" variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((current) => Math.max(1, current - 1))}>
                <ArrowLeft className="mr-2 h-4 w-4" /> Anterior
              </Button>
              <Button type="button" variant="outline" size="sm" disabled={!hasMore} onClick={() => setPage((current) => current + 1)}>
                Próximo <ArrowRight className="ml-2 h-4 w-4" />
              </Button>
            </div>
          </div>
        ) : null}
      </div>
    </div>
  );
}
