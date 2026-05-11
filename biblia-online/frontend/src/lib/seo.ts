import { getSiteUrl } from "@/lib/env";

export type BreadcrumbItem = {
  label: string;
  href: string;
};

export function getCanonicalUrl(path: string) {
  return `${getSiteUrl().replace(/\/$/, "")}${path}`;
}

export function buildChapterDescription(chapterTitle: string, chapterNumber: number, versionName: string, verses: string[]) {
  const snippet = verses.filter(Boolean).slice(0, 2).join(" ");
  return `Leia ${chapterTitle} ${chapterNumber} na versão ${versionName}. ${snippet}${snippet.length > 0 ? "..." : ""}`;
}

export function buildSchemaBreadcrumbList(items: BreadcrumbItem[]) {
  return {
    "@type": "BreadcrumbList",
    itemListElement: items.map((item, index) => ({
      "@type": "ListItem",
      position: index + 1,
      name: item.label,
      item: getCanonicalUrl(item.href),
    })),
  };
}

export function buildWebPageSchema({
  url,
  title,
  description,
  breadcrumb,
}: {
  url: string;
  title: string;
  description: string;
  breadcrumb: BreadcrumbItem[];
}) {
  return {
    "@context": "https://schema.org",
    "@type": "WebPage",
    url,
    name: title,
    description,
    breadcrumb: buildSchemaBreadcrumbList(breadcrumb),
    publisher: {
      "@type": "Organization",
      name: "Biblia Online",
      url: getSiteUrl(),
    },
    potentialAction: {
      "@type": "SearchAction",
      target: `${getSiteUrl().replace(/\/$/, "")}/buscar?q={search_term_string}`,
      "query-input": "required name=search_term_string",
    },
  };
}

export function buildArticleSchema({
  url,
  title,
  description,
  breadcrumb,
  section,
}: {
  url: string;
  title: string;
  description: string;
  breadcrumb: BreadcrumbItem[];
  section: string;
}) {
  return {
    "@context": "https://schema.org",
    "@type": "Article",
    headline: title,
    description,
    url,
    articleSection: section,
    author: {
      "@type": "Organization",
      name: "Biblia Online",
      url: getSiteUrl(),
    },
    publisher: {
      "@type": "Organization",
      name: "Biblia Online",
      url: getSiteUrl(),
    },
    breadcrumb: buildSchemaBreadcrumbList(breadcrumb),
  };
}
