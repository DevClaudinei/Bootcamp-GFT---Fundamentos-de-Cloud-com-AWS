# Gerenciamento de Instâncias e Arquitetura de Eventos no Amazon EC2 ☁️

Este repositório foi desenvolvido para consolidar e documentar os conceitos práticos abordados no laboratório de gerenciamento de instâncias **Amazon EC2** no Bootcamp da GFT/DIO.

O projeto evoluiu da infraestrutura computacional clássica para um modelo híbrido de **Arquitetura Orientada a Eventos (Event-Driven Architecture)**, integrando serviços tradicionais e componentes serverless da AWS.

---

## 🗺️ Topologia da Arquitetura

O diagrama abaixo, modelado no **draw.io**, ilustra o fluxo contínuo de processamento de dados e o escopo de isolamento de rede de cada recurso:

![Arquitetura da Solução](./images/arquitetura-desafio1.png)

### 📈 Animação de Fluxo (Dynamics):
A arquitetura utiliza o recurso de *Flow Animation* ativo para monitorar visualmente o trajeto do dado desde o provisionamento no servidor até a sua persistência e transformação assíncrona.

---

## 🛠️ Detalhamento Técnico dos Componentes Utilizados

### 1. Camada de Computação Isolada (VPC & Subnet)
* **Amazon EC2 (Elastic Compute Cloud):** Instância provisionada para hospedar o backend da aplicação. O escopo foi delimitado dentro de uma Subnet para gerenciar as regras de entrada e saída por meio de Security Groups (Stateful).
* **Amazon EBS (Elastic Block Store):** Volume de armazenamento em bloco acoplado diretamente à instância EC2, atuando como o sistema de arquivos persistente local para logs e dados de execução de alta performance do sistema operacional.

### 2. Camada de Armazenamento e Desacoplamento Serverless
* **Amazon S3 (Simple Storage Service):** Utilizado como repositório global de objetos para arquivos estáticos e uploads efetuados pelo servidor backend. A escolha do S3 garante durabilidade de 99.999999999% (11 9s) e desacopla o disco local (EBS), permitindo escalabilidade horizontal do EC2.
* **AWS Lambda:** Função serverless configurada sob demanda. Ela elimina a necessidade de manter servidores ligados 24/7 para processamento esporádico de payloads, sendo executada apenas quando acionada.

---

## 🔄 Fluxo Lógico e Ciclo de Vida do Dado

1.  **Ingress & Persistência Local:** A aplicação backend no **Amazon EC2** processa as requisições dos usuários e gerencia a persistência volátil ou logs de auditoria no volume **Amazon EBS** associado.
2.  **Offloading de Arquivos:** Relatórios gerados ou uploads recebidos pelo servidor são enviados diretamente para um Bucket do **Amazon S3** via AWS SDK (`PutObject`), liberando espaço em disco da aplicação.
3.  **Gatilho Assíncrono (S3 Event Notification):** O upload bem-sucedido no S3 dispara automaticamente um evento que acorda a função **AWS Lambda**, sem nenhuma necessidade de polling por parte da aplicação do EC2.
4.  **Processamento Serverless & Callback:** O **AWS Lambda** processa o arquivo (ex: extração de metadados, compressão ou validação de assinaturas) e persiste o payload final tratado de volta no **Amazon S3**, fechando o pipeline assíncrono.

---

## 🎯 Insights de Boas Práticas (Well-Architected Framework)

* **Otimização de Custos (Cost Optimization):** Uso de arquitetura serverless (S3 + Lambda) para processamento de arquivos, garantindo cobrança estritamente pelo tempo de execução (Pay-per-use).
* **Eficiência de Performance (Performance Efficiency):** Uso de volumes EBS adequados ao throughput do EC2 e delegação de tarefas pesadas de manipulação de mídia para o Lambda, evitando gargalos de CPU na instância principal.
* **Conformidade de Segurança (Security):** Isolamento lógico do servidor web em camadas de rede internas, expondo publicamente apenas os endpoints gerenciados e buckets necessários.