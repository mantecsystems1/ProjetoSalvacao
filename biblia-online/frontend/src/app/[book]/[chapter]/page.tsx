import Link from "next/link";
import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ChapterReader } from "@/components/chapter-reader";
import { SeoBreadcrumbs } from "@/components/seo-breadcrumbs";
import { SeoJsonLd } from "@/components/seo-jsonld";
import type { BibleVersion } from "@/lib/types";
import { fetchChapter, fetchVersionByCode, fetchVersions } from "@/lib/server-api";
import { buildArticleSchema, buildChapterDescription, getCanonicalUrl } from "@/lib/seo";

export const revalidate = 3600;

async function resolveVersion(searchParams: { version?: string } | undefined): Promise<BibleVersion | null> {
  if (searchParams?.version) {
    const version = await fetchVersionByCode(searchParams.version);
    if (version) return version;
  }

  const versions = await fetchVersions();
  return versions.length > 0 ? versions[0] : null;
}

export async function generateMetadata({ params, searchParams }: { params: Promise<{ book: string; chapter: string }>; searchParams?: Promise<{ version?: string }> }): Promise<Metadata> {
  const resolvedParams = await params;
  const resolvedSearchParams = await searchParams;
  const version = await resolveVersion(resolvedSearchParams);
  const bookTitle = resolvedParams.book;
  const baseTitle = `${bookTitle} ${resolvedParams.chapter}`;
  const title = `${baseTitle} · ${version?.name ?? "Biblia Online"}`;
  const description = `Leitura do capítulo ${resolvedParams.chapter} do livro ${bookTitle} na versão ${version?.name ?? "padrão"}. Explore versículos, favoritos e navegação rápida.`;
  const canonical = getCanonicalUrl(`/${resolvedParams.book}/${resolvedParams.chapter}`);

  return {
    title,
    description,
    alternates: {
      canonical,
    },
    openGraph: {
      title,
      description,
      url: canonical,
      siteName: "Biblia Online",
      type: "article",
      images: [
        {
          url: `${getCanonicalUrl("")}/icons/icon-512.svg`,
          alt: "Biblia Online",
        },
      ],
    },
    twitter: {
      card: "summary_large_image",
      title,
      description,
    },
  };
}

export default async function ChapterPage({ params, searchParams }: { params: Promise<{ book: string; chapter: string }>; searchParams?: Promise<{ version?: string }> }) {
  const resolvedParams = await params;
  const resolvedSearchParams = await searchParams;
  const version = await resolveVersion(resolvedSearchParams);
  if (!version) notFound();

  const chapterNumber = Number(resolvedParams.chapter);
  if (Number.isNaN(chapterNumber) || chapterNumber < 1) notFound();

  const chapter = await fetchChapter(version.id, resolvedParams.book, chapterNumber);
  if (!chapter) notFound();

  const versionQuery = `?version=${encodeURIComponent(version.code)}`;
  const previousChapter = chapter.chapterNumber > 1 ? chapter.chapterNumber - 1 : null;
  const nextChapter = chapter.chapterNumber + 1;
  const canonical = getCanonicalUrl(`/${resolvedParams.book}/${resolvedParams.chapter}`);
  const breadcrumbItems = [
    { label: "Início", href: "/" },
    { label: chapter.bookTitle, href: `/${resolvedParams.book}` },
    { label: `Capítulo ${chapter.chapterNumber}`, href: `/${resolvedParams.book}/${resolvedParams.chapter}` },
  ];
  const description = buildChapterDescription(chapter.bookTitle, chapter.chapterNumber, version.name, chapter.verses.map((verse) => verse.text));
  const jsonLd = buildArticleSchema({
    url: canonical,
    title: `${chapter.bookTitle} ${chapter.chapterNumber} · ${version.name}`,
    description,
    breadcrumb: breadcrumbItems,
    section: chapter.bookTitle,
  });

  return (
    <div className="mx-auto max-w-6xl space-y-8 px-4 py-10 sm:px-6 lg:px-8">
      <SeoJsonLd data={jsonLd} />
      <SeoBreadcrumbs items={breadcrumbItems} />
      <header className="space-y-3">
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Leitura</p>
        <h1 className="text-4xl font-semibold tracking-tight">{chapter.bookTitle} {chapter.chapterNumber}</h1>
        <p className="max-w-3xl text-lg leading-8 text-muted-foreground">
          Versão: {version.name} ({version.code}). Navegação fluida entre capítulos com histórico e favoritos.
        </p>
      </header>

      <div className="grid gap-8 lg:grid-cols-[1fr_300px]">
        <section className="space-y-6">
          <div className="flex flex-wrap gap-3">
            {previousChapter ? (
              <Link
                href={`/${encodeURIComponent(resolvedParams.book)}/${previousChapter}${versionQuery}`}
                className={buttonVariants({ size: "sm", variant: "outline" })}
              >
                Capítulo anterior
              </Link>
            ) : null}
            <Link href={`/${encodeURIComponent(resolvedParams.book)}/${nextChapter}${versionQuery}`} className={buttonVariants({ size: "sm" })}>
              Próximo capítulo
            </Link>
            <Link href={`/buscar?version=${encodeURIComponent(version.code)}`} className={buttonVariants({ size: "sm", variant: "ghost" })}>
              Buscar em {version.code}
            </Link>
          </div>

          <ChapterReader version={version} chapter={chapter} />
        </section>

        <aside className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Detalhes do capítulo</CardTitle>
              <CardDescription>Rápido acesso e ajuda de navegação.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="rounded-2xl bg-muted/50 p-4">
                <p className="text-sm text-muted-foreground">Livro</p>
                <p className="font-medium">{chapter.bookTitle}</p>
              </div>
              <div className="rounded-2xl bg-muted/50 p-4">
                <p className="text-sm text-muted-foreground">Capítulo</p>
                <p className="font-medium">{chapter.chapterNumber}</p>
              </div>
              <div className="rounded-2xl bg-muted/50 p-4">
                <p className="text-sm text-muted-foreground">Versão</p>
                <p className="font-medium">{version.name}</p>
              </div>
            </CardContent>
          </Card>
        </aside>
      </div>
    </div>
  );
}
