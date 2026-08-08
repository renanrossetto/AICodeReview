# AICodeReview

API para revisão de código utilizando Inteligência Artificial, com
integração ao Git e ao Ollama. O projeto também possui observabilidade
utilizando **OpenTelemetry + Prometheus + Grafana**.

Este documento apresenta o passo a passo para instalar e executar a
aplicação em uma máquina pessoal.

------------------------------------------------------------------------

## 1. Pré-requisitos

Antes de iniciar, instale:

-   .NET SDK compatível com o projeto
-   Docker Desktop
-   Ollama
-   Git

É recomendado utilizar o Windows 10/11 com o Docker Desktop configurado
para utilizar containers Linux.

------------------------------------------------------------------------

# 2. Arquitetura da execução local

Durante a execução local, os principais componentes ficam distribuídos
da seguinte forma:

``` text
┌─────────────────────┐
│     AICodeReview    │
│      ASP.NET API    │
│                     │
│  http://localhost   │
│       :5025         │
└──────────┬──────────┘
           │
           │ HTTP
           ▼
┌─────────────────────┐
│       Ollama        │
│                     │
│ localhost:11434     │
└─────────────────────┘

           ▲
           │ métricas
           │
┌──────────┴──────────┐
│      Prometheus     │
│     Docker          │
│ localhost:9090      │
└──────────┬──────────┘
           │
           │ PromQL
           ▼
┌─────────────────────┐
│       Grafana       │
│      Docker         │
│ localhost:3000      │
└─────────────────────┘
```

A API é executada localmente e o Prometheus/Grafana são executados em
containers Docker.

Como o Prometheus está dentro de um container Docker e a API está
rodando no Windows, o endereço utilizado pelo Prometheus para acessar a
API é:

``` text
host.docker.internal:5025
```

------------------------------------------------------------------------

# 3. Configuração do Ollama

O Ollama é responsável por executar o modelo de Inteligência Artificial
localmente.

## 3.1 Instalação

Instale o Ollama na máquina seguindo o instalador oficial:

https://ollama.com/download

Após a instalação, abra o terminal e verifique:

``` bash
ollama --version
```

Se o comando retornar a versão instalada, o Ollama está disponível.

------------------------------------------------------------------------

## 3.2 Verificar se o Ollama está executando

O serviço normalmente fica disponível em:

``` text
http://localhost:11434
```

É possível testar pelo navegador ou pelo terminal.

No Windows:

``` bash
curl http://localhost:11434
```

O Ollama deverá responder indicando que o serviço está disponível.

------------------------------------------------------------------------

## 3.3 Baixar um modelo

A aplicação precisa de um modelo instalado no Ollama.

Exemplo:

``` bash
ollama pull llama3.2
```

Depois, confirme que o modelo está instalado:

``` bash
ollama list
```

O nome do modelo utilizado pela aplicação deve ser configurado de acordo
com o modelo instalado.

Exemplo:

``` text
llama3.2
```

------------------------------------------------------------------------

## 3.4 Testar o Ollama

Antes de iniciar a API, é recomendado verificar se o modelo consegue
responder:

``` bash
ollama run llama3.2
```

Digite uma pergunta simples e confirme que o modelo responde.

Para sair:

``` text
/bye
```

------------------------------------------------------------------------

# 4. Configuração da API

A aplicação utiliza o Ollama como servidor de IA.

No `appsettings.json`, configure a URL do Ollama:

``` json
{
  "AiReview": {
    "BaseUrl": "http://localhost:11434"
  }
}
```

A API utiliza essa URL para realizar as chamadas ao Ollama.

> Importante: o Ollama está rodando diretamente na máquina. Portanto, a
> API local utiliza `localhost:11434`.

------------------------------------------------------------------------

# 5. Executando a API

A API pode ser executada pelo Visual Studio ou pelo terminal.

## Visual Studio

Execute o projeto utilizando o perfil HTTP.

A configuração utilizada no projeto é:

``` text
http://localhost:5025
```

A API disponibiliza o endpoint de métricas em:

``` text
http://localhost:5025/metrics
```

O Swagger pode ser acessado pela URL configurada pela aplicação.

Se estiver utilizando o perfil HTTP, a URL principal utilizada durante o
desenvolvimento é:

``` text
http://localhost:5025
```

------------------------------------------------------------------------

## Terminal

Na pasta do projeto:

``` bash
dotnet run --launch-profile http
```

A API deverá ficar disponível em:

``` text
http://localhost:5025
```

------------------------------------------------------------------------

# 6. Subindo Prometheus e Grafana com Docker

O projeto utiliza Docker para executar o Prometheus e o Grafana.

Primeiro, verifique se o Docker está funcionando:

``` bash
docker --version
```

Depois:

``` bash
docker ps
```

------------------------------------------------------------------------

## 6.1 Caso exista um docker-compose.yml

Na pasta onde está localizado o arquivo `docker-compose.yml`, execute:

``` bash
docker compose up -d
```

Para verificar os containers:

``` bash
docker ps
```

Para acompanhar os logs:

``` bash
docker compose logs -f
```

Para parar os containers:

``` bash
docker compose down
```

Para subir novamente:

``` bash
docker compose up -d
```

------------------------------------------------------------------------

## 6.2 Caso os containers já tenham sido criados

Se Prometheus e Grafana já foram criados anteriormente, eles podem ser
iniciados com:

``` bash
docker start prometheus
docker start grafana
```

Verifique:

``` bash
docker ps
```

Para parar:

``` bash
docker stop prometheus
docker stop grafana
```

> Caso os nomes dos containers sejam diferentes, utilize `docker ps -a`
> para descobrir os nomes corretos.

------------------------------------------------------------------------

# 7. Prometheus

O Prometheus coleta as métricas disponibilizadas pela API.

## URL

``` text
http://localhost:9090
```

Acesse pelo navegador:

``` text
http://localhost:9090
```

------------------------------------------------------------------------

## 7.1 Configuração do target

O Prometheus precisa acessar a API local.

A configuração utilizada é:

``` yaml
global:
  scrape_interval: 5s

scrape_configs:
  - job_name: "ai-code-review-api"

    static_configs:
      - targets:
          - host.docker.internal:5025
```

O endereço:

``` text
host.docker.internal:5025
```

é importante porque o Prometheus está dentro do Docker enquanto a API
está rodando diretamente no Windows.

------------------------------------------------------------------------

## 7.2 Verificar se o Prometheus está coletando a API

No Prometheus, acesse:

``` text
http://localhost:9090/targets
```

O target:

``` text
ai-code-review-api
```

deve aparecer como:

``` text
UP
```

O endpoint deverá aparecer aproximadamente como:

``` text
http://host.docker.internal:5025/metrics
```

------------------------------------------------------------------------

# 8. Métricas da aplicação

A aplicação utiliza OpenTelemetry para gerar métricas.

As principais métricas criadas pela aplicação são:

### Revisões manuais

``` promql
ai_manual_reviews_total
```

### Revisões de Pull Request/Git

``` promql
ai_pr_reviews_total
```

### Erros

``` promql
ai_review_errors_total
```

### Tempo de execução da IA

O OpenTelemetry gera as séries:

``` promql
ai_review_duration_ms_milliseconds_sum
```

``` promql
ai_review_duration_ms_milliseconds_count
```

Para calcular o tempo médio:

``` promql
rate(ai_review_duration_ms_milliseconds_sum[$__rate_interval])
/
rate(ai_review_duration_ms_milliseconds_count[$__rate_interval])
```

### Tamanho do código analisado

As séries geradas são:

``` promql
ai_review_input_size_chars_sum
```

``` promql
ai_review_input_size_chars_count
```

O comportamento dessas métricas depende do tipo de revisão, utilizando o
label:

``` text
review.type
```

que é exportado pelo Prometheus como:

``` text
review.type="manual"
```

ou:

``` text
review.type="diff"
```

------------------------------------------------------------------------

# 9. Métricas de runtime do .NET

A aplicação também utiliza:

``` csharp
.AddRuntimeInstrumentation()
```

Isso disponibiliza métricas relacionadas ao runtime .NET.

Entre as métricas observadas durante a configuração estão:

``` promql
dotnet_process_cpu_count
```

``` promql
dotnet_process_cpu_time_seconds_total
```

``` promql
dotnet_process_memory_working_set_bytes
```

Também são disponibilizadas métricas de:

-   Garbage Collector
-   Heap
-   JIT
-   Thread Pool
-   Timers
-   HTTP Client

As métricas de runtime normalmente possuem:

``` text
otel_scope_name="System.Runtime"
```

------------------------------------------------------------------------

# 10. Grafana

O Grafana é utilizado para criar o dashboard visual da aplicação.

## URL

``` text
http://localhost:3000
```

Acesse:

``` text
http://localhost:3000
```

------------------------------------------------------------------------

## 10.1 Configurar o Prometheus como Data Source

No Grafana:

1.  Acesse **Connections**
2.  Acesse **Data sources**
3.  Selecione **Prometheus**
4.  Informe a URL do Prometheus

Se o Grafana estiver rodando em Docker junto com o Prometheus,
normalmente o endereço interno deve utilizar o nome do serviço/container
do Prometheus, por exemplo:

``` text
http://prometheus:9090
```

Se ambos estiverem configurados em uma mesma rede Docker, essa é a forma
recomendada.

Clique em:

``` text
Save & test
```

O Grafana deverá informar que a conexão foi realizada com sucesso.

------------------------------------------------------------------------

# 11. Dashboard

Foi criado um dashboard chamado:

``` text
Code_Review
```

A ideia do dashboard é apresentar informações importantes sobre o
funcionamento da aplicação.

Entre os painéis utilizados:

-   Manual Reviews
-   Git Review
-   Review Errors
-   Tempo de resposta da IA
-   Tamanho médio do código
-   Processamento
-   Memória

Os painéis podem utilizar diferentes tipos de visualização, como:

-   Stat
-   Time series
-   Gauge

------------------------------------------------------------------------

# 12. Testando a instalação completa

Depois de configurar tudo, a sequência recomendada para iniciar o
ambiente é:

### 1. Iniciar o Docker Desktop

Confirme:

``` bash
docker ps
```

### 2. Iniciar Prometheus e Grafana

Com Docker Compose:

``` bash
docker compose up -d
```

ou, caso os containers já existam:

``` bash
docker start prometheus
docker start grafana
```

### 3. Iniciar o Ollama

Confirme:

``` bash
ollama list
```

Se necessário, execute:

``` bash
ollama run llama3.2
```

### 4. Iniciar a API

``` bash
dotnet run --launch-profile http
```

A API deverá estar em:

``` text
http://localhost:5025
```

### 5. Verificar as métricas

Abra:

``` text
http://localhost:5025/metrics
```

### 6. Verificar o Prometheus

Abra:

``` text
http://localhost:9090/targets
```

O target:

``` text
ai-code-review-api
```

deve estar:

``` text
UP
```

### 7. Abrir o Grafana

``` text
http://localhost:3000
```

Abra o dashboard:

``` text
Code_Review
```

------------------------------------------------------------------------

# 13. Ordem recomendada para iniciar o ambiente

Para evitar problemas de conexão, utilize esta ordem:

``` text
Docker Desktop
      ↓
Prometheus + Grafana
      ↓
Ollama
      ↓
API AICodeReview
      ↓
Testar /metrics
      ↓
Verificar Prometheus
      ↓
Abrir Grafana
```

------------------------------------------------------------------------

# 14. Problemas comuns

## Prometheus mostra o target como DOWN

Verifique:

``` text
http://localhost:5025/metrics
```

Se a página não abrir, a API não está executando.

Se abrir normalmente, confirme no `prometheus.yml`:

``` yaml
targets:
  - host.docker.internal:5025
```

Depois reinicie o Prometheus.

------------------------------------------------------------------------

## Grafana não mostra dados

Primeiro verifique o Prometheus.

Acesse:

``` text
http://localhost:9090
```

Teste:

``` promql
up
```

Depois:

``` promql
ai_manual_reviews_total
```

Se o Prometheus não possuir os dados, o problema não está no Grafana.

------------------------------------------------------------------------

## API não consegue acessar o Ollama

Verifique:

``` text
http://localhost:11434
```

Depois:

``` bash
ollama list
```

Confirme também o `BaseUrl`:

``` json
"BaseUrl": "http://localhost:11434"
```

------------------------------------------------------------------------

## Prometheus não consegue acessar a API

Não utilize:

``` text
localhost:5025
```

dentro do `prometheus.yml`.

Utilize:

``` text
host.docker.internal:5025
```

porque `localhost` dentro do container aponta para o próprio container.

------------------------------------------------------------------------

# 15. URLs principais

  Serviço              URL
  -------------------- ---------------------------------
  AICodeReview API     `http://localhost:5025`
  Métricas da API      `http://localhost:5025/metrics`
  Ollama               `http://localhost:11434`
  Prometheus           `http://localhost:9090`
  Prometheus Targets   `http://localhost:9090/targets`
  Grafana              `http://localhost:3000`

------------------------------------------------------------------------

# 16. Resumo rápido

Depois que tudo estiver instalado, para iniciar o ambiente:

``` bash
docker compose up -d
```

Depois confirme o Ollama:

``` bash
ollama list
```

E inicie a API:

``` bash
dotnet run --launch-profile http
```

Acesse:

``` text
API:
http://localhost:5025

Prometheus:
http://localhost:9090

Grafana:
http://localhost:3000

Ollama:
http://localhost:11434
```

Se o Prometheus estiver com o target `ai-code-review-api` como **UP** e
o endpoint `/metrics` estiver respondendo, a coleta de métricas está
funcionando.
