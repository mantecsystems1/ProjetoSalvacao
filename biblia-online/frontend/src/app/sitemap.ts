import type { MetadataRoute } from "next";
import { getSiteUrl } from "@/lib/env";

export default function sitemap(): MetadataRoute.Sitemap {
  const base = getSiteUrl().replace(/\/$/, "");
  const now = new Date();

  return [
    { url: `${base}/`, lastModified: now, changeFrequency: "daily", priority: 1 },
    { url: `${base}/search`, lastModified: now, changeFrequency: "weekly", priority: 0.8 },
    { url: `${base}/login`, lastModified: now, changeFrequency: "yearly", priority: 0.3 },
    // URLs dinâmicas de leitura podem ser expandidas por build/job consultando a API.
    { url: `${base}/read/pt-pd-sample/genesis/1`, lastModified: now, changeFrequency: "weekly", priority: 0.7 },
    { url: `${base}/read/web-sample/genesis/1`, lastModified: now, changeFrequency: "weekly", priority: 0.7 },
  ];
}
