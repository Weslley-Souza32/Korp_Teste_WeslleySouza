# Korp - Sistema de Emissão de Notas Fiscais

Projeto técnico desenvolvido para demonstrar a implementação de um sistema de emissão de notas fiscais utilizando Angular no frontend e uma arquitetura baseada em microserviços no backend.

A solução possui dois serviços independentes:

- `Korp.Stock.Api`: responsável pelo cadastro de produtos e controle de estoque.
- `Korp.Billing.Api`: responsável pela criação, consulta e impressão de notas fiscais.

Cada serviço possui seu próprio banco PostgreSQL.

---

## Arquitetura

```text
Angular 22
   │
   ├──────────────► Korp.Stock.Api
   │                    │
   │                    └── PostgreSQL
   │                        korp_stock
   │
   └──────────────► Korp.Billing.Api
                        │
                        ├── PostgreSQL
                        │   korp_billing
                        │
                        └──────────► Korp.Stock.Api
```

O `Billing Service` não possui acesso direto ao banco de dados do `Stock Service`.

A comunicação entre os microserviços ocorre via HTTP utilizando um typed `HttpClient`.

---

## Tecnologias

### Backend

- .NET 10
- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- FluentValidation
- Microsoft.Extensions.Http.Resilience
- xUnit
- Moq
- Testcontainers

### Frontend

- Angular 22
- TypeScript
- Angular Material
- Reactive Forms
- RxJS
- Signals
- Angular Router
- SCSS
- Vitest

### Infraestrutura

- Docker
- Docker Compose
- PostgreSQL 18 Alpine

---

## Funcionalidades

### Produtos

- Cadastro de produtos
- Código único
- Descrição
- Saldo em estoque
- Listagem de produtos
- Validação de estoque inicial

### Notas Fiscais

- Criação de notas fiscais
- Número sequencial
- Status `Open` e `Closed`
- Inclusão de múltiplos produtos
- Quantidade por produto
- Consulta de notas
- Detalhes da nota
- Snapshot das informações do produto
- Impressão da nota
- Fechamento após impressão
- Atualização automática do estoque

### Resiliência

A comunicação entre `Billing` e `Stock` possui:

- timeout;
- circuit breaker;
- tratamento de indisponibilidade;
- retorno `503 Service Unavailable`;
- feedback amigável no frontend.

Retries automáticos são desabilitados para operações HTTP não seguras, como o débito de estoque via `POST`.

### Idempotência

A baixa de estoque utiliza o `InvoiceId` como identificador da operação.

As operações processadas são registradas na tabela:

```text
stock_debit_operations
```

O campo `invoice_id` possui índice único, evitando que a mesma nota provoque uma segunda baixa de estoque.

### Concorrência

O estoque utiliza concorrência otimista através do `xmin` do PostgreSQL.

Caso duas operações tentem consumir simultaneamente o mesmo saldo, somente uma consegue concluir a atualização.

A segunda operação recebe:

```text
409 Conflict
```

evitando saldo negativo.

---

## Estrutura do Repositório

```text
Korp_Teste_WeslleySouza
│
├── backend
│   └── Korp
│       ├── Korp.slnx
│       │
│       ├── src
│       │   ├── Korp.Stock.Api
│       │   └── Korp.Billing.Api
│       │
│       └── tests
│           ├── Korp.Stock.Tests
│           └── Korp.Billing.Tests
│
├── frontend
│   └── korp-web
│
├── docs
│   └── TECHNICAL_DETAILS.md
│
├── docker-compose.yml
├── .env.example
├── .gitignore
└── README.md
```

---

# Como executar o projeto

## Pré-requisitos

É necessário possuir instalado:

- .NET 10 SDK
- Node.js
- npm
- Angular CLI
- Docker Desktop
- Git

---

## 1. Clonar o repositório

```bash
git clone https://github.com/Weslley-Souza32/Korp_Teste_WeslleySouza.git
```

Entre na pasta:

```bash
cd Korp_Teste_WeslleySouza
```

---

## 2. Configurar as variáveis do Docker

Na raiz do projeto existe:

```text
.env.example
```

Crie uma cópia chamada:

```text
.env
```

e configure os valores do PostgreSQL.

Exemplo:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=korp_stock
POSTGRES_PORT=5434
```

O arquivo `.env` não deve ser versionado.

---

## 3. Iniciar o PostgreSQL

Na raiz do projeto:

```bash
docker compose up -d
```

Para verificar:

```bash
docker compose ps
```

---

# Configuração do backend

Os dois serviços utilizam User Secrets para armazenar suas connection strings durante o desenvolvimento.

## Stock API

Entre no projeto:

```bash
cd backend/Korp/src/Korp.Stock.Api
```

Configure:

```bash
dotnet user-secrets set "ConnectionStrings:StockDatabase" "Host=localhost;Port=5434;Database=korp_stock;Username=postgres;Password=SUA_SENHA"
```

## Billing API

Entre no projeto:

```bash
cd backend/Korp/src/Korp.Billing.Api
```

Configure:

```bash
dotnet user-secrets set "ConnectionStrings:BillingDatabase" "Host=localhost;Port=5434;Database=korp_billing;Username=postgres;Password=SUA_SENHA"
```

---

## Bancos de dados

A solução utiliza dois bancos lógicos:

```text
korp_stock
korp_billing
```

O banco `korp_billing` deve existir antes da execução das migrations do Billing.

---

## Executar migrations

### Stock

No diretório da solução:

```bash
dotnet ef database update \
  --project src/Korp.Stock.Api \
  --startup-project src/Korp.Stock.Api
```

### Billing

```bash
dotnet ef database update \
  --project src/Korp.Billing.Api \
  --startup-project src/Korp.Billing.Api
```

Também é possível executar as migrations pelo Package Manager Console do Visual Studio.

---

# Executar as APIs

## Stock API

```bash
dotnet run --project src/Korp.Stock.Api
```

Por padrão:

```text
https://localhost:7200
```

## Billing API

```bash
dotnet run --project src/Korp.Billing.Api
```

Por padrão:

```text
https://localhost:7066
```

O Billing deve conseguir acessar o Stock através da configuração:

```json
"Services": {
  "Stock": {
    "BaseUrl": "https://localhost:7200/"
  }
}
```

---

# Executar o frontend

Entre em:

```bash
cd frontend/korp-web
```

Instale as dependências:

```bash
npm install
```

Execute:

```bash
ng serve
```

A aplicação estará disponível em:

```text
http://localhost:4200
```

---

# Endpoints principais

## Stock API

### Listar produtos

```http
GET /api/products
```

### Consultar produto

```http
GET /api/products/{id}
```

### Criar produto

```http
POST /api/products
```

Exemplo:

```json
{
  "code": "PROD-001",
  "description": "Notebook Dell",
  "stockQuantity": 10
}
```

### Debitar estoque

```http
POST /api/stock/debit
```

Exemplo:

```json
{
  "invoiceId": "11111111-1111-1111-1111-111111111111",
  "items": [
    {
      "productId": "22222222-2222-2222-2222-222222222222",
      "quantity": 2
    }
  ]
}
```

---

## Billing API

### Listar notas

```http
GET /api/invoices
```

### Consultar nota

```http
GET /api/invoices/{id}
```

### Criar nota

```http
POST /api/invoices
```

Exemplo:

```json
{
  "items": [
    {
      "productId": "22222222-2222-2222-2222-222222222222",
      "quantity": 2
    }
  ]
}
```

### Imprimir nota

```http
POST /api/invoices/{id}/print
```

---

# Fluxo de impressão

O fluxo ocorre da seguinte maneira:

```text
Angular
   ↓
Billing API
   ↓
valida nota Open
   ↓
Stock API
   ↓
valida produtos e saldo
   ↓
debita estoque
   ↓
Billing API
   ↓
fecha a nota
   ↓
Angular
   ↓
atualiza a interface
```

Se o débito falhar, a nota permanece:

```text
Open
```

e:

```text
ClosedAt = null
```

---

# Cenário de indisponibilidade

Para testar a resiliência:

1. mantenha o `Billing API` executando;
2. interrompa somente o `Stock API`;
3. tente imprimir uma nota `Open`.

O Billing retornará:

```text
503 Service Unavailable
```

e a nota continuará aberta.

O frontend exibirá uma mensagem informando que o serviço de estoque está temporariamente indisponível.

---

# Testes

## Backend

Os projetos:

```text
Korp.Stock.Tests
Korp.Billing.Tests
```

utilizam:

- xUnit
- Moq
- Testcontainers
- PostgreSQL real em containers isolados

Para executar:

```bash
dotnet test
```

Os testes cobrem, entre outros:

- validações;
- criação de produtos;
- criação de notas;
- consulta;
- impressão;
- nota já fechada;
- falha no débito;
- persistência;
- comportamento específico do PostgreSQL.

---

## Frontend

Entre em:

```bash
cd frontend/korp-web
```

Execute:

```bash
ng test
```

Os testes utilizam Vitest e `HttpTestingController`.

São cobertos cenários de:

- chamadas HTTP de produtos;
- chamadas HTTP de notas;
- criação dinâmica de itens;
- impressão;
- indisponibilidade do Stock Service.

Para validar o build:

```bash
ng build
```

---

# Tratamento de erros

As APIs utilizam `IExceptionHandler` e retornam erros no padrão `ProblemDetails`.

Principais status utilizados:

| Status | Significado                         |
| ------ | ----------------------------------- |
| `400`  | Erro de validação                   |
| `404`  | Recurso não encontrado              |
| `409`  | Conflito de negócio ou concorrência |
| `503`  | Serviço dependente indisponível     |
| `500`  | Erro inesperado                     |

---

# Decisões técnicas

Algumas decisões importantes da implementação:

- separação entre Stock e Billing;
- bancos PostgreSQL independentes;
- comunicação REST síncrona;
- ausência de FK entre serviços;
- snapshot de produto na nota;
- handlers organizados por caso de uso;
- ausência de repository genérico;
- typed `HttpClient`;
- concorrência otimista;
- idempotência;
- resiliência HTTP;
- Reactive Forms;
- Signals;
- RxJS;
- Angular Material.

Uma explicação mais detalhada das decisões está disponível em:

```text
docs/TECHNICAL_DETAILS.md
```

---

## Autor

Weslley Souza
