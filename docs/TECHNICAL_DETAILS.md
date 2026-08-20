# Detalhes Técnicos

## 1. Visão geral da solução

A solução foi desenvolvida utilizando uma arquitetura baseada em dois microserviços independentes no backend e uma aplicação Angular no frontend.

Os microserviços são:

- `Korp.Stock.Api`: responsável pelo cadastro de produtos e controle de estoque.
- `Korp.Billing.Api`: responsável pela criação, consulta e impressão das notas fiscais.

O frontend foi desenvolvido em Angular 22 e consome os dois serviços por HTTP.

Cada microserviço possui seu próprio banco de dados PostgreSQL, evitando compartilhamento direto de tabelas entre contextos de negócio.

---

## 2. Arquitetura

A comunicação principal ocorre da seguinte forma:

Angular
→ Stock API

Angular
→ Billing API
→ Stock API

O Billing Service não possui acesso direto ao banco do Stock Service.

Ao criar uma nota fiscal, o Billing consulta os dados do produto no Stock Service e armazena um snapshot contendo código e descrição do produto.

Na impressão da nota, o Billing solicita ao Stock Service a baixa dos itens em estoque. A nota somente é fechada após a confirmação de sucesso dessa operação.

Essa separação reduz o acoplamento entre os serviços e mantém cada microserviço responsável pelo seu próprio domínio.

---

# 3. Backend

## 3.1 Tecnologias

O backend foi desenvolvido utilizando:

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

As APIs utilizam Controllers e endpoints REST.

---

## 3.2 Stock Service

O `Korp.Stock.Api` possui responsabilidade sobre:

- cadastro de produtos;
- consulta de produtos;
- controle de saldo;
- baixa de estoque;
- idempotência das operações de débito;
- controle de concorrência.

Cada produto possui:

- Id
- Code
- Description
- StockQuantity
- CreatedAt
- UpdatedAt

A baixa de estoque é realizada pelo endpoint:

`POST /api/stock/debit`

Antes da atualização, todos os produtos e suas quantidades são validados.

A operação é executada dentro de uma transação para evitar baixas parciais.

---

## 3.3 Billing Service

O `Korp.Billing.Api` é responsável por:

- criação das notas fiscais;
- numeração sequencial;
- listagem;
- consulta por Id;
- impressão;
- fechamento da nota.

Uma nota é criada inicialmente com status:

`Open`

Após a impressão bem-sucedida, passa para:

`Closed`

e recebe o valor de `ClosedAt`.

Os itens armazenam um snapshot com:

- ProductId
- ProductCode
- ProductDescription
- Quantity

Dessa forma, o Billing não depende de uma relação de banco de dados com o Stock Service.

---

## 3.4 Comunicação entre microserviços

A comunicação entre Billing e Stock é feita por HTTP através de um typed `HttpClient`.

O contrato é abstraído através da interface:

`IStockServiceClient`

Ela possui operações para consultar produtos e solicitar a baixa de estoque.

Essa abordagem permite substituir a implementação real por mocks durante testes unitários.

---

## 3.5 Tratamento de erros e exceções

As duas APIs utilizam `IExceptionHandler` para tratamento global de exceções.

As respostas seguem o padrão `ProblemDetails`.

São tratados, entre outros, os seguintes cenários:

- 400 Bad Request: erros de validação;
- 404 Not Found: recurso inexistente;
- 409 Conflict: conflito de negócio ou concorrência;
- 503 Service Unavailable: indisponibilidade do Stock Service;
- 500 Internal Server Error: erros inesperados.

As validações de entrada são realizadas com FluentValidation.

---

## 3.6 Resiliência

A comunicação HTTP entre Billing e Stock utiliza `Microsoft.Extensions.Http.Resilience`.

Foram configuradas estratégias de:

- timeout por tentativa;
- timeout total;
- circuit breaker;
- retry para operações seguras.

Retries foram desabilitados para métodos HTTP não seguros, como `POST`, evitando repetição automática de operações que alteram estado.

Quando o Stock Service está indisponível, o Billing converte a falha de comunicação em:

`503 Service Unavailable`

A nota permanece aberta e o frontend apresenta uma mensagem amigável ao usuário.

---

## 3.7 Concorrência

O controle de concorrência do estoque utiliza concorrência otimista através do `xmin` do PostgreSQL.

O campo é mapeado pelo Entity Framework Core como `row version`.

Quando duas operações tentam atualizar o mesmo produto simultaneamente, somente uma delas consegue persistir a alteração.

A segunda recebe um `DbUpdateConcurrencyException`, convertido para:

`409 Conflict`

Isso evita que o estoque fique negativo em situações concorrentes.

---

## 3.8 Idempotência

Cada solicitação de baixa de estoque utiliza o `InvoiceId` como identificador da operação.

O Stock Service registra as operações na tabela:

`stock_debit_operations`

O campo `invoice_id` possui índice único.

Se uma requisição com o mesmo `InvoiceId` for enviada novamente, o estoque não é debitado uma segunda vez.

Dessa forma, a operação de baixa é idempotente.

---

## 3.9 Uso de LINQ

LINQ foi utilizado em diferentes partes do backend.

Exemplos:

- `Where`: filtragem de registros;
- `Select`: projeção para DTOs;
- `AnyAsync`: verificação de existência;
- `OrderBy` e `OrderByDescending`: ordenação;
- `Contains`: busca por múltiplos identificadores;
- `FirstOrDefaultAsync`: consulta de registro único;
- `Single` e `SingleAsync`: obtenção de registros que devem ser únicos.

Em consultas de leitura foi utilizado `AsNoTracking()` quando o objeto não precisa ser alterado, reduzindo o custo de tracking do Entity Framework Core.

---

# 4. Frontend Angular

## 4.1 Tecnologias

O frontend utiliza:

- Angular 22
- TypeScript
- Angular Material
- Reactive Forms
- RxJS
- Signals
- Angular Router
- SCSS

A aplicação foi desenvolvida utilizando standalone components.

---

## 4.2 Lifecycle hooks

O principal lifecycle hook utilizado foi:

`ngOnInit`

Ele é utilizado nas telas que precisam carregar dados ao serem inicializadas.

Exemplos:

- listagem de produtos;
- listagem de notas;
- detalhes da nota;
- criação de nota para carregar os produtos disponíveis.

O carregamento é iniciado após a criação do componente, mantendo a lógica de inicialização separada do construtor.

---

## 4.3 RxJS

Os métodos dos services Angular retornam `Observable`.

Exemplo:

`Observable<Product[]>`

As telas utilizam `subscribe()` para tratar:

- sucesso;
- erro;
- atualização de estado.

RxJS é utilizado principalmente na comunicação assíncrona com as APIs através do `HttpClient`.

---

## 4.4 Signals

Signals são utilizados para armazenar e atualizar estados reativos da interface.

Exemplos:

- produtos;
- notas fiscais;
- loading;
- mensagens de erro;
- mensagens de sucesso;
- estado de impressão.

Exemplo:

`isPrinting = signal(false);`

A escolha também é adequada para o modelo zoneless utilizado pela aplicação Angular.

---

## 4.5 Reactive Forms

O cadastro de produtos e a criação de notas utilizam Reactive Forms.

Na criação da nota foi utilizado `FormArray`, permitindo adicionar dinamicamente múltiplos produtos.

As validações incluem:

- campos obrigatórios;
- quantidade mínima;
- tamanho máximo;
- estoque inicial não negativo.

O frontend realiza validações de usabilidade, enquanto o backend continua sendo responsável pela validação definitiva das regras de negócio.

---

## 4.6 Angular Material

Angular Material foi utilizado como biblioteca visual.

Entre os componentes utilizados estão:

- MatTable;
- MatCard;
- MatFormField;
- MatInput;
- MatSelect;
- MatButton;
- MatIcon;
- MatSidenav;
- MatToolbar;
- MatProgressSpinner;
- MatChip.

O Material foi utilizado para manter consistência visual e acelerar o desenvolvimento sem criar uma biblioteca de componentes própria.

---

## 4.7 Tratamento de erros no frontend

As respostas das APIs são tratadas pelas páginas e convertidas em mensagens amigáveis.

Exemplos:

- produto duplicado;
- produto duplicado na mesma nota;
- estoque insuficiente;
- Stock Service indisponível.

Durante a impressão, o botão é desabilitado e um indicador visual de processamento é apresentado.

Em caso de sucesso, a tela atualiza a nota de `Open` para `Closed` sem necessidade de recarregar toda a página.

---

# 5. Testes

O backend possui testes com:

- xUnit;
- Moq;
- PostgreSQL via Testcontainers.

O uso de Testcontainers permite testar comportamentos específicos do PostgreSQL, como geração de número sequencial e concorrência através do `xmin`.

Os testes cobrem validações e handlers dos serviços Stock e Billing.

O frontend utiliza o ambiente de testes do Angular com Vitest.

Foram criados testes para:

- ProductService;
- InvoiceService;
- criação dinâmica de itens da nota;
- impressão de nota;
- tratamento de indisponibilidade do Stock Service.

As chamadas HTTP são isoladas utilizando `HttpTestingController`.

---

# 6. Fluxo de impressão

O fluxo de impressão é:

1. O usuário solicita a impressão da nota no Angular.
2. O Billing Service busca a nota.
3. O Billing valida que a nota está `Open`.
4. O Billing envia os produtos e quantidades ao Stock Service.
5. O Stock valida disponibilidade.
6. O Stock realiza a baixa de forma transacional.
7. O Billing altera a nota para `Closed`.
8. O campo `ClosedAt` é preenchido.
9. O Angular atualiza o status e apresenta a confirmação ao usuário.

Se qualquer etapa relacionada ao estoque falhar, a nota permanece `Open`.

---

# 7. Decisões técnicas e trade-offs

## Microserviços

Foi utilizada separação real entre Stock e Billing, incluindo bancos de dados distintos.

Essa decisão aumenta a complexidade de comunicação, porém evita acoplamento direto entre os domínios.

## Comunicação síncrona

Foi escolhida comunicação REST síncrona entre Billing e Stock.

Para o escopo do teste técnico, essa abordagem reduz complexidade e torna o fluxo mais fácil de acompanhar e demonstrar.

Em um cenário de maior escala, poderia ser avaliada uma arquitetura orientada a eventos.

## CQRS

A organização dos casos de uso segue uma abordagem inspirada em CQRS, separando handlers de criação, consulta e impressão.

Não foi utilizado MediatR para evitar adicionar abstração sem necessidade para o tamanho do projeto.

## Repository Pattern

Não foi criado um repository genérico.

O Entity Framework Core já fornece abstrações de Unit of Work e Repository através do `DbContext` e `DbSet`.

Adicionar uma camada genérica adicional aumentaria complexidade sem benefício relevante neste contexto.

## Consistência distribuída

Não existe transação distribuída entre Billing e Stock.

A solução utiliza:

- validação de status;
- idempotência;
- controle de concorrência;
- tratamento de falhas;
- circuit breaker.

Para um sistema de produção com requisitos mais complexos de consistência, poderiam ser aplicados padrões como Saga e Outbox.
