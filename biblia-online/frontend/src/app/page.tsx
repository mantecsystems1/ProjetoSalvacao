import Link from "next/link";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import type { BibleVersion } from "@/lib/types";
import { getPublicApiUrl } from "@/lib/env";

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

export default async function HomePage() {
  const versions = await fetchVersions();

  return (
    <div className="mx-auto max-w-5xl space-y-10 px-4 py-10">
      <section className="space-y-3">
        <h1 className="text-4xl font-semibold tracking-tight">Biblia Online</h1>
        <p className="max-w-2xl text-lg text-muted-foreground">
          Leia em várias versões e idiomas, com busca rápida, favoritos e histórico — roda como PWA e foi pensado para SEO e performance.
        </p>
        <div className="flex flex-wrap gap-2">
          <Link href="/search" className={cn(buttonVariants())}>
            Buscar versículos
          </Link>
          <Link href="/login" className={cn(buttonVariants({ variant: "outline" }))}>
            Entrar
          </Link>
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-2">
        {versions.map((v) => (
          <Card key={v.id}>
            <CardHeader>
              <CardTitle>{v.name}</CardTitle>
              <CardDescription>
                Código: <span className="font-mono">{v.code}</span>
              </CardDescription>
            </CardHeader>
            <CardContent className="flex flex-wrap gap-2">
              <Link
                href={`/read/${encodeURIComponent(v.code)}/genesis/1`}
                className={cn(buttonVariants({ size: "sm" }))}
              >
                Começar em Gênesis 1
              </Link>
              <Link
                href={`/livros/${encodeURIComponent(v.code)}`}
                className={cn(buttonVariants({ size: "sm", variant: "outline" }))}
              >
                Listar livros
              </Link>
            </CardContent>
          </Card>
        ))}
      </section>

      <section className="rounded-lg border bg-muted/30 p-6 text-sm text-muted-foreground">
        Observação: os textos completos devem ser importados a partir de fontes em domínio público ou licença compatível com o seu projeto.
      </section>
    </div>
  );
}
