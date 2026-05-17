import Link from "next/link";
import type { Metadata } from "next";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import type { BibleVersion } from "@/lib/types";
import { getPublicApiUrl } from "@/lib/env";

export const dynamic = "force-dynamic";

export const revalidate = 3600;

async function fetchVersions(): Promise<BibleVersion[]> {
  try {
    const res = await fetch(`${getPublicApiUrl().replace(/\/$/, "")}/api/v1/versions`, {
      headers: { Accept: "application/json" },
      next: { revalidate: 600 },
    });
    if (!res.ok) return [];
    return (await res.json()) as BibleVersion[];
  } catch {
    return [];
  }
}

export async function generateMetadata(): Promise<Metadata> {
  return { title: "Abrir Bíblia · Versões" };
}

export default async function BooksIndexPage() {
  const versions = await fetchVersions();

  return (
    <div className="mx-auto max-w-5xl space-y-8 px-4 py-10">
      <header className="space-y-3">
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Abrir Bíblia</p>
        <h1 className="text-4xl font-semibold tracking-tight">Selecione a versão para ler capítulo a capítulo</h1>
        <p className="max-w-3xl text-base leading-7 text-muted-foreground">
          Abra uma Bíblia completa e navegue por livros e capítulos como em uma edição física.
        </p>
      </header>

      <div className="grid gap-4 md:grid-cols-2">
        {versions.map((version) => (
          <Card key={version.id}>
            <CardHeader>
              <CardTitle>{version.name}</CardTitle>
              <CardDescription>{version.description ?? version.code}</CardDescription>
            </CardHeader>
            <CardContent className="flex flex-wrap gap-3">
              <Link
                href={`/livros/${encodeURIComponent(version.code)}`}
                className={cn(buttonVariants({ size: "sm" }))}
              >
                Abrir Bíblia
              </Link>
              <Link
                href={`/buscar?version=${encodeURIComponent(version.code)}`}
                className={cn(buttonVariants({ size: "sm", variant: "outline" }))}
              >
                Buscar nesta versão
              </Link>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
