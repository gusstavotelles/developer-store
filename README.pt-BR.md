# Developer Store — Guia de execução local e autorização (PT-BR)

Este repositório contém o desafio técnico Developer Store: uma API ASP.NET Core (NET 8) para cadastro, autenticação e gerenciamento de usuários.

Este documento explica, em detalhes, como executar a API localmente (Visual Studio ou CLI), como executar apenas as dependências usando Docker e como autenticar e usar o Swagger UI com JWT (Authorize).

---


## Resumo rápido

- Raiz do projeto: `backend/`
- Projeto da API: `backend/src/Ambev.DeveloperEvaluation.WebApi`
- Novo arquivo Docker com apenas dependências: `backend/docker-compose.deps.yml`
- A API deve ser executada localmente (Visual Studio ou dotnet run) enquanto Postgres/Mongo/Redis podem rodar em containers Docker.

---

## Pré-requisitos

- .NET 8 SDK instalado
- Docker e Docker Compose instalados (apenas para dependências)
- Visual Studio 2022/2023 ou VS Code (se usar Visual Studio, recomendado: abrir a solution `backend/Ambev.DeveloperEvaluation.sln`)

---

## 1) Subir somente dependências com Docker

Fornecemos um arquivo compose leve contendo apenas os serviços de banco/cache.

A partir da raiz do repositório:

cd backend
docker-compose -f docker-compose.deps.yml up -d

Isso iniciará:
- PostgreSQL (container name: `ambev_developer_evaluation_database`) → porta 5432
- MongoDB (container name: `ambev_developer_evaluation_nosql`) → porta 27017
- Redis (container name: `ambev_developer_evaluation_cache`) → porta 6379 (requer senha)

Verifique os containers com:
docker ps

Se preferir, o `docker-compose.yml` original foi ajustado para conter apenas dependências também. Use `docker-compose up -d` a partir da pasta `backend` se preferir.

---

## 2) Rodar a API localmente (Visual Studio)

Recomendado: rode a API localmente e use Docker apenas para dependências. Isso é o que os avaliadores provavelmente esperarão.

1. Abra o Visual Studio.
2. File → Open → Project/Solution → selecione `backend/Ambev.DeveloperEvaluation.sln`.
3. No Solution Explorer, clique com o botão direito em `Ambev.DeveloperEvaluation.WebApi` → Set as Startup Project.
4. Selecione o perfil de execução (barra superior): escolha `https` ou `http` (os perfis já estão pré-configurados).
   - Os perfis já incluem a variável de ambiente `ConnectionStrings__DefaultConnection` apontando para o Postgres local no Docker:
     Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n
5. Pressione F5 (Debug) ou Ctrl+F5 (Run).
6. Quando a aplicação iniciar, o Swagger UI abrirá automaticamente:
   - http://localhost:5119/swagger ou https://localhost:7181/swagger

Observações:
- A aplicação aplica as migrations do EF Core automaticamente no startup por padrão. Se quiser pular as migrations e aplicá-las manualmente, defina a variável de ambiente `SKIP_MIGRATIONS=true` no perfil de execução ou no ambiente, e então rode:
  cd backend/src/Ambev.DeveloperEvaluation.WebApi
  dotnet ef database update

---

## 3) Rodar a API localmente (CLI)

Se preferir rodar a API pelo terminal:

Abra um terminal e execute:

cd backend\src\Ambev.DeveloperEvaluation.WebApi
set "ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
set "ASPNETCORE_ENVIRONMENT=Development"
dotnet run --urls "http://localhost:5119;https://localhost:7181"

Após o startup, o Swagger estará disponível em:
http://localhost:5119/swagger

---

## 4) Como a autenticação funciona (resumo)

- A API usa tokens JWT.
- O endpoint de login (POST `/api/Auth`) retorna uma resposta JSON com os dados de autenticação. A camada de aplicação retorna um objeto com a propriedade `Token` (o JWT).
- A WebApi encapsula respostas usando `ApiResponseWithData<T>` — então o token fica disponível em `response.data.token` (ou `response.data.Token` dependendo do cliente).
- Todos os endpoints protegidos exigem o cabeçalho:
  Authorization: Bearer {token}

---

## 5) Passo-a-passo: criar usuário, autenticar, usar Authorize no Swagger

1) Criar um usuário (JSON de exemplo)
- Endpoint: POST /api/Users
- Exemplo de body:
{
  "username": "testuser",
  "password": "Password@123",
  "phone": "(11) 98765-4321",
  "email": "test.user@example.com",
  "status": 1,
  "role": 1
}

Você pode criar o usuário via Swagger `POST /api/Users` (Try it out) ou usando curl:

curl -X POST "http://localhost:5119/api/Users" -H "Content-Type: application/json" -d ^
"{\"username\":\"testuser\",\"password\":\"Password@123\",\"phone\":\"(11) 98765-4321\",\"email\":\"test.user@example.com\",\"status\":1,\"role\":1}"

Resposta: 201 Created em caso de sucesso.

2) Autenticar (obter token)
- Endpoint: POST /api/Auth
- Exemplo de body:
{
  "email": "test.user@example.com",
  "password": "Password@123"
}

Usando curl:

curl -X POST "http://localhost:5119/api/Auth" -H "Content-Type: application/json" -d ^
"{\"email\":\"test.user@example.com\",\"password\":\"Password@123\"}"

Exemplo de corpo de resposta (encapsulado):
{
  "success": true,
  "message": "User authenticated successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "id": "GUID",
    "name": "testuser",
    "email": "test.user@example.com",
    "phone": "...",
    "role": "..."
  }
}

Pegue o valor de `data.token` (o JWT).

3) Usar "Authorize" no Swagger
- Abra http://localhost:5119/swagger
- Clique em "Authorize" (ícone de cadeado).
- No input rotulado `Bearer` cole o token, prefixando com "Bearer ":
  Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...
- Clique em "Authorize" → Close.
- O Swagger agora enviará o cabeçalho Authorization nas requisições feitas pela UI.

4) Exemplo curl para chamar endpoint protegido:

curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6..." http://localhost:5119/api/Users/me

---

## 6) Troubleshooting

- Se a API não conectar ao Postgres:
  - Garanta que as dependências estejam iniciadas: `cd backend && docker-compose -f docker-compose.deps.yml up -d`
  - Verifique o estado dos containers: `docker ps`
  - Verifique logs do Postgres: `cd backend && docker-compose logs --tail=200 ambev.developerevaluation.database`
  - Verifique disponibilidade da porta (Windows): `netstat -a -n | findstr 5119`

- Se o Swagger mostrar "Failed to fetch" ao chamar um endpoint (comum em Try it out / POST):
  - Causa: a aplicação faz redirecionamento HTTP → HTTPS (HTTPS redirection). Quando o Swagger UI está carregado via HTTP e envia requisição para HTTP que é redirecionada (307) para HTTPS, o browser pode bloquear ou o fetch falhar. Também ocorre bloqueio de conteúdo misto se o UI estiver em HTTPS e a requisição for HTTP.
  - Solução rápida:
    1. Abra o endpoint HTTPS do Swagger no navegador:
       https://localhost:7181/swagger
    2. Se o navegador mostrar aviso de certificado (self-signed), clique em "Avançado" → prosseguir para o site (adicionar exceção de segurança). Alternativamente, confie no certificado de desenvolvimento:
       - No Windows, execute (PowerShell/cmd):
         dotnet dev-certs https --clean
         dotnet dev-certs https --trust
       - Reinicie o navegador após confiar no certificado.
    3. Refaça o "Try it out" no Swagger (ou faça a autenticação). A UI em HTTPS não sofrerá redirecionamento e o fetch deve funcionar.
  - Alternativas:
    - Usar curl/Postman:
      curl -k -X POST "https://localhost:7181/api/Auth" -H "Content-Type: application/json" -d '{"email":"test.user@example.com","password":"Password@123"}'
    - Ajustar `dotnet run --urls` para forçar HTTP apenas (não recomendado), ou desabilitar HTTPS redirection temporariamente no Program.cs (apenas para debugging).

- Migrations falhando:
  - Opção: defina SKIP_MIGRATIONS=true no ambiente do run profile e aplique migrations manualmente:
    cd backend/src/Ambev.DeveloperEvaluation.WebApi
    dotnet ef database update

---

## 7) Executar testes

A partir da raiz do repositório:

cd backend
dotnet test

Observação: os testes de integração/funcionais podem depender de serviços Docker em execução.

---

## 8) Orientações

- Suba as dependências
- Rode a WebAPI localmente (Visual Studio preferido)
- Use Swagger para criar um usuário → autenticar → pressione Authorize com o token → testar endpoints protegidos
- O repositório aplica migrations automaticamente no startup; o fluxo deve funcionar "out-of-the-box" se o Docker estiver em execução e as portas estiverem disponíveis.

---

## Arquitetura, escolhas e justificativas 

Este projeto segue abordagem em camadas (clean / layered architecture) para separar responsabilidades:

- Camada Domain
  - Contém entidades, validadores de domínio e regras de negócio.
- Camada Application
  - Implementa casos de uso com MediatR (Command/Handler), validações e mapeamentos de DTO.
- Infra / ORM
  - EF Core (Npgsql) com repositórios que encapsulam acesso a banco.
- WebApi
  - Controllers, DTOs, Perfis AutoMapper, middleware e configuração do Swagger/JWT.
- IoC / Module initializers
  - Registro de dependências centralizado para manter Program.cs limpo.

Racional:
- Testabilidade: handlers e repositórios são fáceis de testar isoladamente.
- Manutenibilidade: projetos pequenos e com responsabilidades claras.
- Padrões reconhecíveis: MediatR + FluentValidation + AutoMapper facilitam avaliação técnica.

Principais decisões técnicas e ferramentas
- JWT para autenticação; BCrypt para hashing de senha.
- FluentValidation em diferentes camadas para fornecer mensagens claras em 400 Bad Request.
- Swagger com definition de Bearer para testar endpoints protegidos via UI.
- Scripts e compose separados: `docker-compose.deps.yml` para dependências, mantendo a API rodando localmente por dotnet run.

Observações de segurança e operação
- Não comitar segredos; configurar variáveis de ambiente para produção.
- HTTPS recomendado em produção e dev-certs para desenvolvimento local.
- Migrations automáticas são conveniência; para ambiente controlado use `SKIP_MIGRATIONS=true` e aplique migrations manualmente.

