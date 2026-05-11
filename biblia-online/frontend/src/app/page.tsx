import Link from "next/link";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import type { BibleVersion } from "@/lib/types";
import { getPublicApiUrl } from "@/lib/env";

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

export default async function HomePage() {
  const versions = await fetchVersions();

  return (
    <div className="mx-auto max-w-6xl space-y-10 px-4 py-12 sm:px-6 lg:px-8">
      <section className="rounded-3xl border border-border bg-card/80 p-8 shadow-sm shadow-black/5">
        <div className="grid gap-8 lg:grid-cols-[1.4fr_1fr] lg:items-center">
          <div className="space-y-6">
            <p className="inline-flex rounded-full bg-primary/10 px-4 py-1 text-xs font-semibold uppercase tracking-[0.3em] text-primary">
              Biblia Online
            </p>
            <h1 className="text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
              Leitura bíblica moderna para todos os dispositivos.
            </h1>
            <p className="max-w-2xl text-lg leading-8 text-muted-foreground">
              Navegue por livros, capítulos e versículos com busca instantânea, favoritos, histórico, PWA offline e modo escuro.
            </p>
            <div className="flex flex-wrap gap-3">
              <Link href="/buscar" className={cn(buttonVariants())}>
                Buscar versículos
              </Link>
              <Link href="/favoritos" className={cn(buttonVariants({ variant: "outline" }))}>
                Meus favoritos
              </Link>
              <Link href="/perfil" className={cn(buttonVariants({ variant: "ghost" }))}>
                Perfil
              </Link>
            </div>
          </div>
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="rounded-3xl bg-background/95 p-6 shadow-sm ring-1 ring-border">
              <p className="text-sm font-semibold uppercase tracking-[0.24em] text-muted-foreground">
                Desenvolvido para
              </p>
              <ul className="mt-6 space-y-3 text-sm leading-7 text-foreground">
                <li>SSR rápido e conteúdo SEO-friendly</li>
                <li>Cache offline e manifest PWA</li>
                <li>Modo claro e escuro com preferência salva</li>
                <li>Busca instantânea e navegação por capítulo</li>
              </ul>
            </div>
            <div className="rounded-3xl bg-background/95 p-6 shadow-sm ring-1 ring-border">
              <p className="text-sm font-semibold uppercase tracking-[0.24em] text-muted-foreground">
                Versões disponíveis</p>
              <div className="mt-6 space-y-3 text-sm text-foreground">
                {versions.slice(0, 6).map((version) => (
                  <div key={version.id} className="flex items-center justify-between rounded-2xl bg-muted/40 p-3">
                    <div>
                      <p className="font-medium">{version.name}</p>
                      <p className="text-xs text-muted-foreground">{version.code}</p>
                    </div>
                    <Link href={`/buscar?version=${encodeURIComponent(version.code)}`} className={cn(buttonVariants({ size: "icon", variant: "outline" }))}>
                      ▶
                    </Link>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-2">
        {versions.map((version) => (
          <Card key={version.id}>
            <CardHeader>
              <CardTitle>{version.name}</CardTitle>
              <CardDescription>{version.description ?? `Código ${version.code}`}</CardDescription>
            </CardHeader>
            <CardContent className="flex flex-wrap gap-2">
              <Link href={`/buscar?version=${encodeURIComponent(version.code)}`} className={cn(buttonVariants({ size: "sm" }))}>
                Buscar nesta versão
              </Link>
              <Link href={`/buscar?version=${encodeURIComponent(version.code)}`} className={cn(buttonVariants({ size: "sm", variant: "outline" }))}>
                Buscar nesta versão
              </Link>
            </CardContent>
          </Card>
        ))}
      </section>
    </div>
  );
}
