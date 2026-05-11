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

export class BibleParsers {
    /**
     * Parser para USFM (Unified Standard Format Markers)
     * Comum em eBible.org
     */
    static parseUSFM(content: string, metadata: VersionMetadata): CanonicalVerse[] {
        const verses: CanonicalVerse[] = [];
        let currentBook = '';
        let currentChapter = 0;

        const lines = content.split('\n');
        for (const line of lines) {
            if (line.startsWith('\\id')) currentBook = line.split(' ')[1].toLowerCase();
            if (line.startsWith('\\c')) currentChapter = parseInt(line.split(' ')[1]);
            if (line.startsWith('\\v')) {
                const match = line.match(/\\v\s+(\d+)\s+(.*)/);
                if (match) {
                    verses.push({
                        bookSlug: currentBook,
                        chapter: currentChapter,
                        number: parseInt(match[1]),
                        text: match[2].trim()
                    });
                }
            }
        }
        return verses;
    }

    /**
     * Parser para XML (Zefania ou similar)
     */
    static async parseXML(path: string): Promise<CanonicalVerse[]> {
        const xml = fs.readFileSync(path, 'utf-8');
        const parser = new xml2js.Parser();
        const result = await parser.parseStringPromise(xml);
        const verses: CanonicalVerse[] = [];

        // Exemplo simplificado de travessia XML (ajustar conforme DTD específico)
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
     * Parser para JSON
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