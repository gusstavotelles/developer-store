## Evidências de verificação automática — Developer Store

Resumo
- Objetivo: Garantir que os endpoints descritos no README funcionem (criar usuário, autenticar, criar venda, consultar venda).
- Ambiente de execução: API rodando localmente em `http://localhost:5119` com dependências (Postgres/Mongo/Redis) via `backend/docker-compose.deps.yml`.
- Script usado para verificação automática: `backend/tmp/run_checks.ps1`

Execução (resultado resumido)
- POST /api/Users
  - Status: 201 Created
  - Exemplo de id criado: c32fff1a-3fb0-470c-8d9b-6a0425519e9a
- POST /api/Auth
  - Status: 200 OK
  - Token JWT gerado e salvo localmente em `backend/tmp/last_token.txt`
- POST /api/sales (protegido — Authorization: Bearer {token})
  - Status: 201 Created
  - Exemplo de sale id: d3c58d28-3446-460c-a966-2e474ef7c019
  - Regras aplicadas: desconto de 10% aplicado ao item com quantidade 4 (total calculado = 36.00)
- GET /api/sales/{id}
  - Status: 200 OK
  - Retorno contém informações da venda, total e itens com desconto percent.

Saída completa (trechos)
- Create user response (abridged):
  {
    "data": { "id":"c32fff1a-3fb0-470c-8d9b-6a0425519e9a" },
    "success": true,
    "message": "User created successfully"
  }

- Authenticate response (abridged):
  {
    "data": {
      "data": {
        "token": "{JWT}",
        "email": "test.user@example.com",
        "name": "testuser",
        "role": "Customer"
      },
      "success": true
    }
  }

- Create sale response (abridged):
  { "id": "d3c58d28-3446-460c-a966-2e474ef7c019" }

- Get sale response (abridged):
  {
    "id": "d3c58d28-3446-460c-a966-2e474ef7c019",
    "saleNumber": "T001",
    "total": 36.00,
    "items": [
      {
        "productId": "P1",
        "unitPrice": 10.00,
        "quantity": 4,
        "discountPercent": 0.10,
        "total": 36.00
      }
    ]
  }

Observações e próximos passos que apliquei
- Atualizei validadores para aceitar telefone em E.164 e no formato local (ex.: `(11) 98765-4321`).
- Tratei duplicidade de email como erro de validação (ValidationException) para retornar 400.
- Removi arquivos temporários de resultado do repositório (.gitignore já contém `backend/tmp/`).
- Criei este documento com resumo e exemplos para anexar como evidência.

Como reproduzir localmente (comando rápido)
1. cd backend
2. docker-compose -f docker-compose.deps.yml up -d --build
3. cd backend/src/Ambev.DeveloperEvaluation.WebApi
4. dotnet run
5. powershell -NoProfile -ExecutionPolicy Bypass -File backend/tmp/run_checks.ps1

Arquivo de referência:
- `backend/tmp/run_checks.ps1` — script de verificação automatizada (preservado no repositório para auditoria)
