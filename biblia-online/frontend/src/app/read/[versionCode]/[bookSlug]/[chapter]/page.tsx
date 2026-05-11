import Link from "next/link";
import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { getSiteUrl } from "@/lib/env";
import { fetchChapter, fetchVersionByCode } from "@/lib/server-api";
import { ReaderVerseList } from "./reader-verse-list";

type Props = {
  params: Promise<{ versionCode: string; bookSlug: string; chapter: string }>;
};

export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const { versionCode, bookSlug, chapter } = await params;
  const n = Number(chapter);
  const v = await fetchVersionByCode(versionCode);
  const ch = v && Number.isFinite(n) ? await fetchChapter(v.id, bookSlug, n) : null;

  const title = ch ? `${ch.bookTitle} ${ch.chapterNumber} (${ch.versionCode}) · Biblia Online` : "Leitura · Biblia Online";
  const description =
    ch?.verses?.[0]?.text?.slice(0, 160) ??
    "Leia a Bíblia online com múltiplas versões e idiomas, busca rápida, favoritos e histórico.";

  const url = `${getSiteUrl().replace(/\/$/, "")}/read/${encodeURIComponent(versionCode)}/${encodeURIComponent(bookSlug)}/${encodeURIComponent(chapter)}`;

  return {
    title,
    description,
    alternates: { canonical: url },
    openGraph: { title, description, url, type: "article" },
    twitter: { card: "summary_large_image", title, description },
    robots: { index: true, follow: true },
  };
}

export default async function ReaderPage({ params }: Props) {
  const { versionCode, bookSlug, chapter } = await params;
  const n = Number(chapter);
  if (!Number.isFinite(n) || n < 1) notFound();

  const v = await fetchVersionByCode(versionCode);
  if (!v) notFound();

  const ch = await fetchChapter(v.id, bookSlug, n);
  if (!ch) notFound();

  const prev = n > 1 ? n - 1 : null;
  const next = n + 1;

  const jsonLd = {
    "@context": "https://schema.org",
    "@type": "Article",
    headline: `${ch.bookTitle} ${ch.chapterNumber}`,
    inLanguage: v.code,
    isPartOf: { "@type": "Book", name: ch.bookTitle },
  };

  return (
    <article className="mx-auto max-w-3xl px-4 py-8">
      <script type="application/ld+json" dangerouslySetInnerHTML={{ __html: JSON.stringify(jsonLd) }} />

      <header className="mb-8 space-y-2">
        <p className="text-sm text-muted-foreground">{v.name}</p>
        <h1 className="text-3xl font-semibold tracking-tight">{ch.bookTitle}</h1>
        <p className="text-muted-foreground">Capítulo {ch.chapterNumber}</p>
      </header>

      <ReaderVerseList chapter={ch} />

      <div className="mt-10 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        {prev ? (
          <Link
            href={`/read/${encodeURIComponent(versionCode)}/${encodeURIComponent(bookSlug)}/${prev}`}
            className={cn(buttonVariants({ variant: "outline" }), "w-full sm:w-auto")}
          >
            Capítulo anterior
          </Link>
        ) : (
          <span />
        )}
        <Link
          href={`/read/${encodeURIComponent(versionCode)}/${encodeURIComponent(bookSlug)}/${next}`}
          className={cn(buttonVariants({ variant: "outline" }), "w-full sm:w-auto")}
        >
          Próximo capítulo
        </Link>
      </div>
    </article>
  );
}
