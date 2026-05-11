# Sistema de Importação Bíblica

Este módulo é responsável por ingerir textos bíblicos de diversas fontes e normalizá-los no PostgreSQL.

## Estrutura de Pastas

- `raw/`: Coloque aqui os arquivos originais (USFM, XML, JSON).
- `processed/`: Arquivos movidos automaticamente após sucesso.
- `scripts/`: Lógica de parsing e banco de dados.
- `output/`: Logs detalhados de cada execução.

## Como usar

1. Certifique-se de que o banco de dados está rodando (`docker-compose up -d`).
2. Instale as dependências:
   ```bash
   cd imports
   npm install
   ```
3. Coloque um arquivo em `raw/` (ex: `web.json`).
4. Execute o importador:
   ```bash
   npm run import
   ```

## Funcionalidades Implementadas

- **Parser Multi-formato**: Suporte inicial para USFM (padrão eBible), XML e JSON.
- **Upsert Inteligente**: Usa `ON CONFLICT` para atualizar textos sem duplicar registros ou IDs.
- **Atomicidade**: Toda a importação de uma versão é feita dentro de uma transação SQL. Se falhar, nada é alterado (Rollback).
- **Rastreabilidade**: Metadata de copyright e fonte salvos na tabela `bible_versions`.

## Fontes Recomendadas

- **eBible.org**: Downloads em USFM e formato "Zefania XML".
- **World English Bible**: Excelente fonte em domínio público.

## Próximos Passos

- Implementar detecção automática de idioma via `franc` se o metadado for omitido.
- Adicionar suporte a arquivos compactados (.zip/.gz) para processar biblias inteiras.