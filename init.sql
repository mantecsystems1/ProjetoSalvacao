-- Habilitar extensões necessárias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "unaccent";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- Otimização para buscas LIKE/ILIKE se necessário

-- Função para atualizar o timestamp 'updated_at' automaticamente
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- 1. Tabela de Idiomas
CREATE TABLE languages (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    code VARCHAR(16) NOT NULL UNIQUE, -- ex: pt-BR, en-US
    name VARCHAR(128) NOT NULL,
    is_rtl BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 2. Tabela de Usuários (com Soft Delete)
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(320) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    display_name VARCHAR(120) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP WITH TIME ZONE
);
-- Índice parcial para permitir reuso do e-mail se o usuário anterior foi deletado
CREATE UNIQUE INDEX idx_users_email_active ON users(email) WHERE (deleted_at IS NULL);

-- 3. Versões da Bíblia (com Soft Delete)
CREATE TABLE bible_versions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    language_id UUID NOT NULL REFERENCES languages(id),
    code VARCHAR(64) NOT NULL UNIQUE, -- ex: NVI, KJA, ACF
    name VARCHAR(256) NOT NULL,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP WITH TIME ZONE
);
CREATE INDEX idx_bible_versions_language ON bible_versions(language_id) WHERE (deleted_at IS NULL);

-- 4. Livros (Estrutura Canônica)
CREATE TABLE books (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    canonical_order INTEGER NOT NULL UNIQUE, -- 1 a 66
    slug VARCHAR(64) NOT NULL UNIQUE, -- ex: genesis, joao
    name VARCHAR(128) NOT NULL, -- Nome padrão (ex: Gênesis)
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
-- Índice para ordenação rápida na listagem
CREATE INDEX idx_books_canonical_order ON books(canonical_order);

-- 5. Capítulos
CREATE TABLE chapters (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    book_id UUID NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    number INTEGER NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(book_id, number)
);
CREATE INDEX idx_chapters_book_id ON chapters(book_id);

-- 6. Versículos (Tabela Crítica para Performance)
CREATE TABLE verses (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    chapter_id UUID NOT NULL REFERENCES chapters(id) ON DELETE CASCADE,
    bible_version_id UUID NOT NULL REFERENCES bible_versions(id) ON DELETE CASCADE,
    number INTEGER NOT NULL,
    text TEXT NOT NULL,
    -- Coluna para busca textual otimizada (Português como padrão, mas expansível)
    search_vector tsvector GENERATED ALWAYS AS (to_tsvector('portuguese', unaccent(text))) STORED,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Índices de Performance para Versículos
-- 1. Unicidade e busca direta (ex: Versão X, Capítulo Y, Versículo Z)
CREATE UNIQUE INDEX idx_verses_identity ON verses (bible_version_id, chapter_id, number);
-- 2. Carregamento rápido de capítulo (ex: Versão X, Capítulo Y)
CREATE INDEX idx_verses_fetch_chapter ON verses (bible_version_id, chapter_id) INCLUDE (number, text);
-- 3. Busca Full-Text
CREATE INDEX idx_verses_search ON verses USING GIN (search_vector);

-- 7. Favoritos
CREATE TABLE favorite_verses (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    verse_id UUID NOT NULL REFERENCES verses(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(user_id, verse_id)
);
CREATE INDEX idx_favorite_verses_user ON favorite_verses(user_id);

-- 8. Histórico de Leitura
CREATE TABLE reading_history (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    bible_version_id UUID NOT NULL REFERENCES bible_versions(id),
    chapter_id UUID NOT NULL REFERENCES chapters(id),
    last_read_at_utc TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(user_id, bible_version_id, chapter_id)
);
CREATE INDEX idx_reading_history_user_date ON reading_history(user_id, last_read_at_utc DESC);

-- Triggers para UpdatedAt em todas as tabelas
DO $$ 
DECLARE 
    t text;
BEGIN
    FOR t IN (SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE') LOOP
        EXECUTE format('CREATE TRIGGER trigger_update_timestamp BEFORE UPDATE ON %I FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();', t);
    END LOOP;
END $$;

-- SEEDS MÍNIMOS
INSERT INTO languages (code, name) VALUES ('pt-BR', 'Português (Brasil)');

INSERT INTO books (canonical_order, slug, name) VALUES 
(1, 'genesis', 'Gênesis'),
(2, 'exodo', 'Êxodo'),
(19, 'salmos', 'Salmos'),
(40, 'mateus', 'Mateus'),
(43, 'joao', 'João'),
(66, 'apocalipse', 'Apocalipse');

INSERT INTO bible_versions (language_id, code, name) 
SELECT id, 'NVI', 'Nova Versão Internacional' FROM languages WHERE code = 'pt-BR';