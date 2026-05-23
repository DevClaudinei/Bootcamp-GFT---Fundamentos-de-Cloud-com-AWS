# 🚀 Automação de Infraestrutura Local com LocalStack e AWS Lambda em .NET 8

Este projeto consolida os conceitos práticos de **Infraestrutura como Código (IaC)**, conteinerização e arquiteturas modernas orientadas a eventos. O principal objetivo é provisionar um ecossistema de nuvem isolado, performático e com custo zero, simulando os serviços essenciais da **AWS** diretamente na máquina de desenvolvimento.

---

## 🏗️ Arquitetura do Ecossistema

Toda a infraestrutura foi modelada utilizando o **LocalStack** como orquestrador central, expondo e emulando os componentes de nuvem através de uma API unificada na porta local `4566`:

* **Amazon S3 (Simple Storage Service):** Configurado para simular o armazenamento de objetos e cargas de dados brutos (Bucket: `bucket-carga-dados`).
* **Amazon DynamoDB:** Banco de dados NoSQL de alta performance, estruturado com uma chave de partição (`Id`) para persistência rápida de metadados (Tabela: `TabelaMetadados`).
* **AWS Lambda (.NET 8):** Microsserviço construído em C# utilizando o SDK oficial da AWS, desacoplado da nuvem pública através do redirecionamento customizado do `ServiceURL` para o endpoint emulado.

---

## 🛠️ Guia de Execução e Construção do Projeto

### 1. Inicialização e Validação da Infraestrutura (IaC)
A criação e o provisionamento dos recursos ocorrem de forma 100% automatizada no momento do boot do contêiner Docker através do script `setup-aws.sh`:

```bash
# Subir o ambiente forçando a limpeza do estado anterior
docker-compose up -d --force-recreate

# Validar se o container está saudável
docker ps

# Validar se os recursos foram criados com sucesso pelo script de boot
docker exec -it localstack-portfolio awslocal s3 ls
docker exec -it localstack-portfolio awslocal dynamodb list-tables