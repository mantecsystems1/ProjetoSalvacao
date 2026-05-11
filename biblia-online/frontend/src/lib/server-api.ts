import type { BibleVersion, Chapter } from "@/lib/types";
import { getPublicApiUrl } from "@/lib/env";

async function httpJson<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${getPublicApiUrl().replace(/\/$/, "")}${path}`, {
    ...init,
    headers: { Accept: "application/json", ...(init?.headers ?? {}) },
    next: init?.next ?? { revalidate: 300 },
  });
  if (!res.ok)
    throw new Error(`HTTP ${res.status} ${path}`);
  return (await res.json()) as T;
}

export async function fetchVersionByCode(code: string): Promise<BibleVersion | null> {
  try {
    return await httpJson<BibleVersion>(`/api/v1/versions/by-code/${encodeURIComponent(code)}`);
  } catch {
    return null;
  }
}

export async function fetchChapter(versionId: string, bookSlug: string, chapter: number): Promise<Chapter | null> {
  try {
    return await httpJson<Chapter>(
      `/api/v1/versions/${encodeURIComponent(versionId)}/books/${encodeURIComponent(bookSlug)}/chapters/${chapter}`,
    );
  } catch {
    return null;
  }
}
