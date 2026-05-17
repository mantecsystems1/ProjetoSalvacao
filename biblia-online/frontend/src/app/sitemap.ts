import type { MetadataRoute } from "next";
import { getPublicApiUrl, getSiteUrl } from "@/lib/env";

export default async function sitemap(): Promise<MetadataRoute.Sitemap> {
  const base = getSiteUrl().replace(/\/$/, "");
  const now = new Date();
  const apiBase = getPublicApiUrl().replace(/\/$/, "");

  const routes: MetadataRoute.Sitemap = [
    { url: `${base}/`, lastModified: now, changeFrequency: "daily", priority: 1 },
    { url: `${base}/buscar`, lastModified: now, changeFrequency: "weekly", priority: 0.9 },
    { url: `${base}/favoritos`, lastModified: now, changeFrequency: "weekly", priority: 0.7 },
    { url: `${base}/perfil`, lastModified: now, changeFrequency: "weekly", priority: 0.7 },
    { url: `${base}/login`, lastModified: now, changeFrequency: "yearly", priority: 0.3 },
  ];

  try {
    const versionsRes = await fetch(`${apiBase}/api/v1/versions`, { next: { revalidate: 86400 } });
    if (!versionsRes.ok) return routes;
    const versions = (await versionsRes.json()) as Array<{ id: string; code: string }>;
    const defaultVersion = versions[0];

    if (defaultVersion) {
      const booksRes = await fetch(`${apiBase}/api/v1/versions/${defaultVersion.id}/books`, {
        next: { revalidate: 86400 },
      });
      if (booksRes.ok) {
        const books = (await booksRes.json()) as Array<{ id: string; slug: string; title: string }>;
        await Promise.all(
          books.map(async (book) => {
            routes.push({ url: `${base}/${book.slug}`, lastModified: now, changeFrequency: "weekly", priority: 0.8 });

            const chaptersRes = await fetch(`${apiBase}/api/v1/books/${book.id}/chapters`, {
              next: { revalidate: 86400 },
            });
            if (!chaptersRes.ok) return;

            const chapters = (await chaptersRes.json()) as Array<{ number: number }>;
            chapters.forEach((chapter) => {
              routes.push({
                url: `${base}/${book.slug}/${chapter.number}`,
                lastModified: now,
                changeFrequency: "weekly",
                priority: 0.7,
              });
            });
          }),
        );
      }
    }
  } catch {
    // Silently ignore sitemap generation errors and keep core routes.
  }

  return routes;
}
