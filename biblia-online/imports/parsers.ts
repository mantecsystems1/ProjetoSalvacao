import * as fs from 'fs';
import * as xml2js from 'xml2js';

export interface CanonicalVerse {
    bookSlug: string;
    chapter: number;
    number: number;
    text: string;
}

export interface VersionMetadata {
    code: string;
    name: string;
    languageCode: string;
    copyright?: string;
    source: string;
}

const BOOK_MAP: Record<string, string> = {
    GEN: 'genesis',
    EXO: 'exodus',
    LEV: 'leviticus',
    NUM: 'numbers',
    DEU: 'deuteronomy',

    JOS: 'joshua',
    JDG: 'judges',
    RUT: 'ruth',

    '1SA': '1-samuel',
    '2SA': '2-samuel',

    '1KI': '1-kings',
    '2KI': '2-kings',

    '1CH': '1-chronicles',
    '2CH': '2-chronicles',

    EZR: 'ezra',
    NEH: 'nehemiah',
    EST: 'esther',
    JOB: 'job',
    PSA: 'psalms',
    PRO: 'proverbs',
    ECC: 'ecclesiastes',
    SNG: 'song-of-solomon',

    ISA: 'isaiah',
    JER: 'jeremiah',
    LAM: 'lamentations',
    EZK: 'ezekiel',
    DAN: 'daniel',

    HOS: 'hosea',
    JOL: 'joel',
    AMO: 'amos',
    OBA: 'obadiah',
    JON: 'jonah',
    MIC: 'micah',
    NAM: 'nahum',
    HAB: 'habakkuk',
    ZEP: 'zephaniah',
    HAG: 'haggai',
    ZEC: 'zechariah',
    MAL: 'malachi',

    MAT: 'matthew',
    MRK: 'mark',
    LUK: 'luke',
    JHN: 'john',
    ACT: 'acts',
    ROM: 'romans',

    '1CO': '1-corinthians',
    '2CO': '2-corinthians',

    GAL: 'galatians',
    EPH: 'ephesians',
    PHP: 'philippians',
    COL: 'colossians',

    '1TH': '1-thessalonians',
    '2TH': '2-thessalonians',

    '1TI': '1-timothy',
    '2TI': '2-timothy',

    TIT: 'titus',
    PHM: 'philemon',
    HEB: 'hebrews',
    JAS: 'james',

    '1PE': '1-peter',
    '2PE': '2-peter',

    '1JN': '1-john',
    '2JN': '2-john',
    '3JN': '3-john',

    JUD: 'jude',
    REV: 'revelation'
};

export class BibleParsers {

    /**
     * Parser USFM
     */
    static parseUSFM(content: string, metadata: VersionMetadata): CanonicalVerse[] {

        const verses: CanonicalVerse[] = [];

        let currentBook = '';
        let currentChapter = 0;

        const lines = content.split('\n');

        for (let line of lines) {

            line = line.trim();

            // Livro
            if (line.startsWith('\\id')) {

                const parts = line.split(' ');

                if (parts.length > 1) {

                    const code = parts[1].toUpperCase();

                    currentBook = BOOK_MAP[code] || code.toLowerCase();
                }

                continue;
            }

            // Capítulo
            if (line.startsWith('\\c')) {

                const parts = line.split(' ');

                if (parts.length > 1) {
                    currentChapter = parseInt(parts[1]);
                }

                continue;
            }

            // Versículo
            const verseRegex = /\\v\s+(\d+)\s+((?:(?!\\v\s+\d).)*)/gs;
            const verseMatches = [...line.matchAll(verseRegex)];

            for (const match of verseMatches) {
                const verseNumber = parseInt(match[1]);

                const verseText = match[2]
                    .replace(/\\f\s*\+[^\\]*\\f\*/g, '')
                    .replace(/\\f\s+[^\\]*\\f\*/g, '')
                    .replace(/\\w\s+([^|]+)\|[^\\]*\\w\*/g, '$1')
                    .replace(/\\w\s+([^\\]+)\\w\*/g, '$1')
                    .replace(/\\add\s+([^\\]+)\\add\*/g, '$1')
                    .replace(/\\[a-zA-Z]+\*?/g, '')
                    .replace(/\s+/g, ' ')
                    .trim();

                verses.push({
                    bookSlug: currentBook,
                    chapter: currentChapter,
                    number: verseNumber,
                    text: verseText
                });
            }
        }

        return verses;
    }

    /**
     * Parser XML
     */
    static async parseXML(path: string): Promise<CanonicalVerse[]> {

        const xml = fs.readFileSync(path, 'utf-8');

        const parser = new xml2js.Parser();

        const result = await parser.parseStringPromise(xml);

        const verses: CanonicalVerse[] = [];

        const books = result.XMLBIBLE.BIBLEBOOK;

        for (const book of books) {

            const bookName = book.$.bname.toLowerCase();

            for (const chapter of book.CHAPTER) {

                const cNum = parseInt(chapter.$.cnumber);

                for (const verse of chapter.VERS) {

                    verses.push({
                        bookSlug: bookName,
                        chapter: cNum,
                        number: parseInt(verse.$.vnumber),
                        text: verse._
                    });
                }
            }
        }

        return verses;
    }

    /**
     * Parser JSON
     */
    static parseJSON(path: string): CanonicalVerse[] {

        const data = JSON.parse(fs.readFileSync(path, 'utf-8'));

        const verses: CanonicalVerse[] = [];

        data.books.forEach((book: any) => {

            book.chapters.forEach((chapter: any) => {

                chapter.verses.forEach((verse: any) => {

                    verses.push({
                        bookSlug: book.slug,
                        chapter: chapter.number,
                        number: verse.number,
                        text: verse.text
                    });
                });
            });
        });

        return verses;
    }
}