import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { Chapter, Favorite, ReadingHistoryItem } from "@/lib/types";

type BibleState = {
  theme: "light" | "dark";
  selectedVersionCode: string;
  favorites: Favorite[];
  history: ReadingHistoryItem[];
  cachedChapters: Record<string, Chapter>;
  setTheme: (theme: "light" | "dark") => void;
  toggleTheme: () => void;
  setSelectedVersionCode: (versionCode: string) => void;
  addFavorite: (favorite: Favorite) => void;
  removeFavorite: (id: string) => void;
  toggleFavorite: (favorite: Favorite) => void;
  addHistory: (item: ReadingHistoryItem) => void;
  clearHistory: () => void;
  cacheChapter: (chapter: Chapter) => void;
  removeCachedChapter: (chapterId: string) => void;
  clearCachedChapters: () => void;
};

export const useBibleStore = create<BibleState>()(
  persist(
    (set) => ({
      theme: "dark",
      selectedVersionCode: "ARC",
      favorites: [],
      history: [],
      cachedChapters: {},
      setTheme: (theme) => set({ theme }),
      toggleTheme: () => set((state) => ({ theme: state.theme === "dark" ? "light" : "dark" })),
      setSelectedVersionCode: (versionCode) => set({ selectedVersionCode: versionCode }),
      addFavorite: (favorite) =>
        set((state) => ({
          favorites: [favorite, ...state.favorites.filter((item) => item.id !== favorite.id)],
        })),
      removeFavorite: (id) =>
        set((state) => ({ favorites: state.favorites.filter((favorite) => favorite.id !== id) })),
      toggleFavorite: (favorite) =>
        set((state) => {
          const exists = state.favorites.some((item) => item.id === favorite.id);
          return {
            favorites: exists
              ? state.favorites.filter((item) => item.id !== favorite.id)
              : [favorite, ...state.favorites],
          };
        }),
      addHistory: (item) =>
        set((state) => ({
          history: [item, ...state.history.filter((entry) => entry.id !== item.id)].slice(0, 50),
        })),
      clearHistory: () => set({ history: [] }),
      cacheChapter: (chapter) =>
        set((state) => {
          const key = `${chapter.versionCode}:${chapter.bookSlug}:${chapter.chapterNumber}`;
          const entries = { ...state.cachedChapters, [key]: chapter };
          const cacheKeys = Object.keys(entries);
          if (cacheKeys.length > 20) {
            delete entries[cacheKeys[0]];
          }
          return { cachedChapters: entries };
        }),
      removeCachedChapter: (chapterId) =>
        set((state) => {
          const entries = { ...state.cachedChapters };
          delete entries[chapterId];
          return { cachedChapters: entries };
        }),
      clearCachedChapters: () => set({ cachedChapters: {} }),
    }),
    {
      name: "biblia-online-settings",
    },
  ),
);
