import { Client } from 'pg';
import * as fs from 'fs';
import * as path from 'path';
import { randomUUID } from 'crypto';
import { BibleParsers, CanonicalVerse, VersionMetadata } from './parsers';

const config = {
    connectionString:
        process.env.DATABASE_URL ||
        'postgresql://biblia:biblia_dev@localhost:5432/biblia_online'
};

const versions: VersionMetadata[] = [
    {
        code: 'BSB',
        name: 'Berean Standard Bible',
        languageCode: 'en',
        source: 'berean.bible',
        copyright: 'Free Bible Hub License'
    },
    {
        code: 'KJV2006',
        name: 'King James Version 2006',
        languageCode: 'en',
        source: 'eBible.org',
        copyright: 'Public Domain'
    }
];

const BOOK_CODES_IN_ORDER = [
    'GEN', 'EXO', 'LEV', 'NUM', 'DEU',
    'JOS', 'JDG', 'RUT',
    '1SA', '2SA',
    '1KI', '2KI',
    '1CH', '2CH',
    'EZR', 'NEH', 'EST', 'JOB', 'PSA', 'PRO', 'ECC', 'SNG',
    'ISA', 'JER', 'LAM', 'EZK', 'DAN',
    'HOS', 'JOL', 'AMO', 'OBA', 'JON', 'MIC', 'NAM', 'HAB', 'ZEP', 'HAG', 'ZEC', 'MAL',
    'MAT', 'MRK', 'LUK', 'JHN', 'ACT', 'ROM',
    '1CO', '2CO', 'GAL', 'EPH', 'PHP', 'COL',
    '1TH', '2TH', '1TI', '2TI', 'TIT', 'PHM', 'HEB', 'JAS', '1PE', '2PE', '1JN', '2JN', '3JN', 'JUD', 'REV'
];

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

const BOOK_TITLES: Record<string, string> = Object.fromEntries(
    Object.entries(BOOK_MAP).map(([code, slug]) => [code, toBookTitle(slug)])
);

function getRowId(row: any): string {
    return row.Id ?? row.id;
}

function toBookTitle(slug: string): string {
    return slug
        .split('-')
        .map(part =>
            /^\d+$/.test(part)
                ? part
                : part.charAt(0).toUpperCase() + part.slice(1)
        )
        .join(' ');
}

async function runImport() {
    const client = new Client(config);

    await client.connect();

    const logStream = fs.createWriteStream(
        path.join(__dirname, 'output/import.log'),
        { flags: 'a' }
    );

    const log = (msg: string) => {
        const t = new Date().toISOString();

        console.log(`[${t}] ${msg}`);

        logStream.write(`[${t}] ${msg}\n`);
    };

    try {
        for (const metadata of versions) {
            log(`Importando versão ${metadata.code}`);

            let rawFolder = path.join(
                __dirname,
                'raw',
                metadata.code.toLowerCase()
            );

            // Para BSB, os arquivos estão em uma subpasta
            if (metadata.code === 'BSB') {
                const subFolder = path.join(rawFolder, 'bsb_usfm');
                if (fs.existsSync(subFolder)) {
                    rawFolder = subFolder;
                }
            }

            if (!fs.existsSync(rawFolder)) {
                log(`Pasta não encontrada: ${rawFolder}`);
                continue;
            }

            await client.query('BEGIN');

            try {
                // Language
                const langId = randomUUID();
                const langRes = await client.query(
                    `
                    INSERT INTO languages ("Id", "Code", "NativeName", "EnglishName", "IsRtl")
                    VALUES ($1, $2, $3, $4, $5)
                    ON CONFLICT ("Code")
                    DO UPDATE SET
                        "NativeName" = EXCLUDED."NativeName",
                        "EnglishName" = EXCLUDED."EnglishName",
                        "IsRtl" = EXCLUDED."IsRtl"
                    RETURNING "Id"
                    `,
                    [
                        langId,
                        metadata.languageCode,
                        metadata.languageCode === 'en'
                            ? 'English'
                            : 'Português',
                        metadata.languageCode === 'en'
                            ? 'English'
                            : 'Portuguese',
                        false
                    ]
                );

                const languageId = getRowId(langRes.rows[0]);

                // Version
                const versionSeedId = randomUUID();
                const versionRes = await client.query(
                    `
                    INSERT INTO bible_versions
                    ("Id", "LanguageId", "Code", "Name", "Description", "IsActive")
                    VALUES ($1, $2, $3, $4, $5, $6)
                    ON CONFLICT ("LanguageId", "Code")
                    DO UPDATE SET
                        "Name" = EXCLUDED."Name",
                        "Description" = EXCLUDED."Description",
                        "IsActive" = EXCLUDED."IsActive"
                    RETURNING "Id"
                    `,
                    [
                        versionSeedId,
                        languageId,
                        metadata.code,
                        metadata.name,
                        `Source: ${metadata.source}`,
                        true
                    ]
                );

                const versionId = getRowId(versionRes.rows[0]);

                const files = fs
                    .readdirSync(rawFolder)
                    .filter(f => f.toLowerCase().endsWith('.usfm'));

                const bookIds: Record<string, string> = {};

                for (let index = 0; index < BOOK_CODES_IN_ORDER.length; index++) {
                    const code = BOOK_CODES_IN_ORDER[index];
                    const slug = BOOK_MAP[code];
                    const title = BOOK_TITLES[code];
                    const canonicalNumber = index + 1;

                    const bookInsertId = randomUUID();
                    const bookRes = await client.query(
                        `
                        INSERT INTO books ("Id", "CanonicalNumber", "Slug")
                        VALUES ($1, $2, $3)
                        ON CONFLICT ("Slug")
                        DO UPDATE SET "CanonicalNumber" = EXCLUDED."CanonicalNumber"
                        RETURNING "Id"
                        `,
                        [bookInsertId, canonicalNumber, slug]
                    );

                    const bookId = getRowId(bookRes.rows[0]);
                    bookIds[slug] = bookId;

                    await client.query(
                        `
                        INSERT INTO book_titles ("Id", "BibleVersionId", "BookId", "Title")
                        VALUES ($1, $2, $3, $4)
                        ON CONFLICT ("BibleVersionId", "BookId")
                        DO UPDATE SET "Title" = EXCLUDED."Title"
                        `,
                        [randomUUID(), versionId, bookId, title]
                    );
                }

                let verses: CanonicalVerse[] = [];

                for (const file of files) {
                    const fullPath = path.join(rawFolder, file);

                    log(`Lendo ${file}`);

                    const content = fs.readFileSync(fullPath, 'utf-8');

                    const parsed = BibleParsers.parseUSFM(
                        content,
                        metadata
                    );

                    verses.push(...parsed);
                }

                log(`Total de versículos: ${verses.length}`);

                for (const v of verses) {
                    const bookId = bookIds[v.bookSlug];

                    if (!bookId) {
                        log(`Livro não encontrado: ${v.bookSlug}`);
                        continue;
                    }

                    await client.query(
                        `
                        INSERT INTO verses
                        ("Id", "BibleVersionId", "BookId", "ChapterNumber", "VerseNumber", "Text")
                        VALUES ($1, $2, $3, $4, $5, $6)
                        ON CONFLICT ("BibleVersionId", "BookId", "ChapterNumber", "VerseNumber")
                        DO UPDATE SET "Text" = EXCLUDED."Text"
                        `,
                        [
                            randomUUID(),
                            versionId,
                            bookId,
                            v.chapter,
                            v.number,
                            v.text
                        ]
                    );
                }

                await client.query('COMMIT');

                log(`Versão ${metadata.code} importada.`);
            } catch (err) {
                await client.query('ROLLBACK');

                log(`Erro na versão ${metadata.code}: ${err}`);
            }
        }

        log('Importação finalizada.');
    } finally {
        await client.end();
    }
}

runImport();