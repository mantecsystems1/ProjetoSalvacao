import Link from "next/link";
import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import type { BookListItem } from "@/lib/types";
import { getPublicApiUrl } from "@/lib/env";
import { fetchVersionByCode } from "@/lib/server-api";

type Props = { params: Promise<{ versionCode: string }> };

async function fetchBooks(versionId: string): Promise<BookListItem[]> {
  try {
    const res = await fetch(`${getPublicApiUrl().replace(/\/$/, "")}/api/v1/versions/${versionId}/books`, {
      headers: { Accept: "application/json" },
      next: { revalidate: 600 },
    });
    if (!res.ok) return [];
    return (await res.json()) as BookListItem[];
  } catch {
    return [];
  }
}

export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const { versionCode } = await params;
  const v = await fetchVersionByCode(versionCode);
  return { title: v ? `Livros · ${v.name}` : "Livros" };
}

export default async function BooksPage({ params }: Props) {
  const { versionCode } = await params;
  const v = await fetchVersionByCode(versionCode);
  if (!v) notFound();

  const books = await fetchBooks(v.id);

  return (
    <div className="mx-auto max-w-5xl space-y-6 px-4 py-10">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">Livros</h1>
        <p className="text-muted-foreground">{v.name}</p>
      </header>

      <div className="grid gap-3 md:grid-cols-2">
        {books.map((b) => (
          <Card key={b.id}>
            <CardHeader className="pb-3">
              <CardTitle className="text-lg">{b.title}</CardTitle>
              <CardDescription className="font-mono">{b.slug}</CardDescription>
            </CardHeader>
            <CardContent>
              <Link
                href={`/read/${encodeURIComponent(versionCode)}/${encodeURIComponent(b.slug)}/1`}
                className={cn(buttonVariants({ size: "sm" }))}
              >
                Abrir cap. 1
              </Link>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
