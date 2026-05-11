export type Language = {
  id: string;
  code: string;
  nativeName: string;
  englishName: string;
  isRtl: boolean;
};

export type BibleVersion = {
  id: string;
  languageId: string;
  code: string;
  name: string;
  description?: string | null;
  isActive: boolean;
};

export type BookListItem = {
  id: string;
  canonicalNumber: number;
  slug: string;
  title: string;
};

export type Verse = {
  verseNumber: number;
  text: string;
};

export type Chapter = {
  bibleVersionId: string;
  versionCode: string;
  bookId: string;
  bookSlug: string;
  bookTitle: string;
  chapterNumber: number;
  verses: Verse[];
};

export type SearchHit = {
  bibleVersionId: string;
  versionCode: string;
  bookId: string;
  bookSlug: string;
  bookTitle: string;
  chapterNumber: number;
  verseNumber: number;
  text: string;
  formatted?: string | null;
};

export type Favorite = {
  id: string;
  bibleVersionId: string;
  versionCode: string;
  bookId: string;
  bookSlug: string;
  bookTitle: string;
  chapterNumber: number;
  verseNumber: number;
  createdAtUtc: string;
};

export type ReadingHistoryItem = {
  id: string;
  bibleVersionId: string;
  versionCode: string;
  bookId: string;
  bookSlug: string;
  bookTitle: string;
  chapterNumber: number;
  lastReadAtUtc: string;
};
