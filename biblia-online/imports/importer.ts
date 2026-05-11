import { Client } from 'pg';
import * as fs from 'fs';
import * as path from 'path';
import { BibleParsers, CanonicalVerse, VersionMetadata } from './parsers';
import { franc } from 'franc';
import * as minimist from 'minimist'; // Adicione 'minimist' às dependências se quiser parsing de CLI robusto

const config = {
    connectionString: process.env.DATABASE_URL || 'postgresql://biblia:biblia_dev@localhost:5432/biblia_online'
};

async function runImport() {
    const args = process.argv.slice(2);
    const client = new Client(config);
    await client.connect();

    const logStream = fs.createWriteStream(path.join(__dirname, '../output/import.log'), { flags: 'a' });
    const log = (msg: string) => {
        const t = new Date().toISOString();
        console.log(`[${t}] ${msg}`);
        logStream.write(`[${t}] ${msg}\n`);
    };

    try {
        // Exemplo de Metadados para uma importação
        const metadata: VersionMetadata = {
            code: 'WEB',
            name: 'World English Bible',
            languageCode: 'en',
            source: 'eBible.org',
            copyright: 'Public Domain'
        };

        log(`Iniciando importação: ${metadata.name}`);
        await client.query('BEGIN');

        // 1. Garantir Idioma
        const langRes = await client.query(
            'INSERT INTO languages (code, name) VALUES ($1, $2) ON CONFLICT (code) DO UPDATE SET name = EXCLUDED.name RETURNING id',
            [
                metadata.languageCode, 
                metadata.languageCode === 'en' ? 'English' : (metadata.languageCode === 'pt' ? 'Português' : 'Auto-detected')
            ]
        );
        const languageId = langRes.rows[0].id;

        // 2. Garantir Versão
        const versionRes = await client.query(
            `INSERT INTO bible_versions (language_id, code, name, description) 
             VALUES ($1, $2, $3, $4) 
             ON CONFLICT (code) DO UPDATE SET name = EXCLUDED.name 
             RETURNING id`,
            [languageId, metadata.code, metadata.name, `Source: ${metadata.source} | Copyright: ${metadata.copyright}`]
        );
        const versionId = versionRes.rows[0].id;

        // 3. Carregar Versículos (Exemplo via JSON)
        const rawPath = path.join(__dirname, '../raw/web.json');
        if (!fs.existsSync(rawPath)) {
            throw new Error(`Arquivo não encontrado: ${rawPath}`);
        }
        
        const verses = BibleParsers.parseJSON(rawPath);
        
        // Detecção de idioma se não fornecido
        if (!metadata.languageCode && verses.length > 0) {
            const sampleText = verses.slice(0, 10).map(v => v.text).join(' ');
            metadata.languageCode = franc(sampleText);
            log(`Idioma detectado automaticamente: ${metadata.languageCode}`);
        }

        log(`Processando ${verses.length} versículos...`);

        for (const v of verses) {
            // 1. Import-Books: Encontrar ou criar Livro
            let bookRes = await client.query('SELECT id FROM books WHERE slug = $1', [v.bookSlug]);
            if (bookRes.rowCount === 0) {
                log(`Aviso: Livro ${v.bookSlug} não encontrado no banco. Pulando.`);
                continue;
            }
            const bookId = bookRes.rows[0].id;

            // 2. Import-Chapters: Garantir Capítulo
            const chapRes = await client.query(
                'INSERT INTO chapters (book_id, number) VALUES ($1, $2) ON CONFLICT (book_id, number) DO UPDATE SET updated_at = NOW() RETURNING id',
                [bookId, v.chapter]
            );
            const chapterId = chapRes.rows[0].id;

            // 3. Import-Verses: Inserir Versículo (Incremental/Upsert)
            await client.query(
                `INSERT INTO verses (chapter_id, bible_version_id, number, text)
                 VALUES ($1, $2, $3, $4)
                 ON CONFLICT (bible_version_id, chapter_id, number) 
                 DO UPDATE SET text = EXCLUDED.text, updated_at = NOW()`,
                [chapterId, versionId, v.number, v.text]
            );
        }

        await client.query('COMMIT');
        log('Importação concluída com sucesso.');
        
        // Mover para processados
        fs.renameSync(rawPath, path.join(__dirname, '../processed/', path.basename(rawPath)));

    } catch (err) {
        await client.query('ROLLBACK');
        log(`ERRO FATAL: ${err}`);
    } finally {
        await client.end();
    }
}

runImport();