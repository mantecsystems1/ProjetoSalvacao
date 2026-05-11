# Importação de textos (domínio público)

Este diretório é para **artefatos estáticos** usados pelos scripts de importação (CSV/JSON) que você pode evoluir sem depender de APIs pagas.

## Contrato sugerido (JSON por versão)

Arquivo `imports/example-version.payload.json`:

```json
{
  "languageCode": "pt",
  "versionCode": "minha-versao-pd",
  "versionName": "Minha tradução PD",
  "books": [
    {
      "canonicalNumber": 1,
      "slug": "genesis",
      "title": "Gênesis",
      "chapters": [
        {
          "number": 1,
          "verses": [
            { "number": 1, "text": "..." }
          ]
        }
      ]
    }
  ]
}
```

## Observações legais

- Use apenas fontes com licença compatível (domínio público ou permissões explícitas).
- Mantenha um arquivo `imports/SOURCES.md` com **origem**, **licença** e **data de download** por versão importada.
