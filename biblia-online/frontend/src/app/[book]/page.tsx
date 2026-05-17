import Link from "next/link";
import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { SeoBreadcrumbs } from "@/components/seo-breadcrumbs";
import { SeoJsonLd } from "@/components/seo-jsonld";
import type { BibleVersion } from "@/lib/types";
import { fetchBookBySlug, fetchVersions, fetchVersionByCode } from "@/lib/server-api";
import { buildWebPageSchema, getCanonicalUrl } from "@/lib/seo";

export const revalidate = 3600;

async function resolveVersion(searchParams: { version?: string } | undefined): Promise<BibleVersion | null> {
  if (searchParams?.version) {
    const version = await fetchVersionByCode(searchParams.version);
    if (version) return version;
  }

  const versions = await fetchVersions();
  return versions.length > 0 ? versions[0] : null;
}

export async function generateMetadata({ params, searchParams }: { params: Promise<{ book: string }>; searchParams?: Promise<{ version?: string }> }): Promise<Metadata> {
  const resolvedParams = await params;
  const resolvedSearchParams = await searchParams;
  const version = await resolveVersion(resolvedSearchParams);
  const book = version ? await fetchBookBySlug(version.id, resolvedParams.book) : null;
  const title = book ? `${book.title} · ${version?.name ?? "Biblia Online"}` : `Livro ${resolvedParams.book}`;
  const description = book
    ? `Navegue pelo livro ${book.title} na versão ${version?.name ?? "padrão"}. Leia capítulos, marque favoritos e use a busca inteligente.`
    : `Leitura bíblica do livro ${resolvedParams.book}.`;
  const canonical = getCanonicalUrl(`/${resolvedParams.book}`);

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
      type: "website",
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

export default async function BookPage({ params, searchParams }: { params: Promise<{ book: string }>; searchParams?: Promise<{ version?: string }> }) {
  const resolvedParams = await params;
  const resolvedSearchParams = await searchParams;
  const version = await resolveVersion(resolvedSearchParams);
  if (!version) notFound();

  const book = await fetchBookBySlug(version.id, resolvedParams.book);
  if (!book) notFound();

  const canonical = getCanonicalUrl(`/${resolvedParams.book}`);
  const breadcrumbItems = [
    { label: "Início", href: "/" },
    { label: book.title, href: `/${resolvedParams.book}` },
  ];
  const previewChapters = Array.from({ length: 12 }, (_, index) => index + 1);
  const versionQuery = `?version=${encodeURIComponent(version.code)}`;
  const jsonLd = buildWebPageSchema({
    url: canonical,
    title: `${book.title} · ${version.name}`,
    description: `Navegue pelo livro ${book.title} na versão ${version.name}. Leia capítulos completos, salve favoritos e descubra a Bíblia online.`,
    breadcrumb: breadcrumbItems,
  });

  return (
    <div className="mx-auto max-w-6xl space-y-8 px-4 py-10 sm:px-6 lg:px-8">
      <SeoJsonLd data={jsonLd} />
      <SeoBreadcrumbs items={breadcrumbItems} />
      <header className="space-y-3">
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Livro</p>
        <h1 className="text-4xl font-semibold tracking-tight">{book.title}</h1>
        <p className="max-w-3xl text-lg leading-8 text-muted-foreground">
          Versão selecionada: {version.name} ({version.code}). Navegue por capítulos e continue a leitura com rapidez.
        </p>
      </header>

      <div className="grid gap-4 lg:grid-cols-[1fr_280px]">
        <section className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Visão geral</CardTitle>
              <CardDescription>Utilize o parâmetro de versão para alternar o livro.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <p className="text-sm leading-7 text-muted-foreground">
                Os capítulos estão disponíveis por número. Comece pelo capítulo 1 ou utilize o caminho direto.
              </p>
              <Link href={`/${encodeURIComponent(book.slug)}/1${versionQuery}`} className={buttonVariants({ size: "sm" })}>
                Abrir capítulo 1
              </Link>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Capítulos rápidos</CardTitle>
              <CardDescription>Selecione a primeira seção do livro.</CardDescription>
            </CardHeader>
            <CardContent className="grid gap-3 sm:grid-cols-2">
              {previewChapters.map((chapterNumber) => (
                <Link
                  key={chapterNumber}
                  href={`/${encodeURIComponent(book.slug)}/${chapterNumber}${versionQuery}`}
                  className={buttonVariants({ size: "sm", variant: "outline" })}
                >
                  Cap. {chapterNumber}
                </Link>
              ))}
            </CardContent>
          </Card>
        </section>

        <aside className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Livro</CardTitle>
              <CardDescription>Slug e metadados</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="rounded-2xl bg-muted/50 p-4">
                <p className="text-sm text-muted-foreground">Slug</p>
                <p className="font-medium">{book.slug}</p>
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
