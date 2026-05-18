# Orquestração Resiliente e Simulação de Pipelines com AWS Step Functions ⚙️

Este repositório consolida o desenvolvimento do **Desafio 2** do Bootcamp de Cloud AWS da GFT/DIO. O projeto demonstra a evolução de um pipeline de orquestração de microsserviços, cobrindo cenários de tratamento de exceções de infraestrutura e técnicas de *Mocking* para validação de fluxos lógicos.

---

## 🧠 Cenário de Negócio: Pipeline de Auditorias e Não-Conformidades

O workflow foi projetado para simular o ciclo de vida do processamento de uma auditoria corporativa integrada a um ecossistema .NET Core e banco de dados **Amazon RDS para SQL Server**:

1.  **Ingress & Persistência:** O fluxo recebe os dados brutos da auditoria e simula a gravação no banco relacional.
2.  **Tomada de Decisão Condicional (`Choice State`):** Avalia dinamicamente o metadado `$.tipoCliente`.
3.  **Processamento Paralelo Síncrono (`Parallel State`):** Caso o cliente seja classificado como `PREMIUM`, o motor da AWS dispara simultaneamente duas ações assíncronas de background:
    * Geração e salvamento de um snapshot de auditoria no **Amazon S3**.
    * Sincronização de dados via chamada HTTPS com uma API externa de faturamento.

---

## 📊 Arquitetura do Fluxo e Evidência de Sucesso

Abaixo está o grafo de execução extraído diretamente do console da AWS após a otimização do fluxo. O status **Execution status: Succeeded** comprova que o roteamento de dados e o paralelismo operaram com eficiência:

![Grafo de Execução do Step Functions](./Desafio%2002/images/step-functions-workflow.png)

### 🔍 Engenharia do Teste de Sucesso (*State Mocking*)
Para validar o comportamento do pipeline sem incorrer em custos de provisionamento de infraestrutura real, os nós de computação foram configurados utilizando o estado **`Pass`**. Isso permitiu injetar payloads controlados em memória e testar a árvore de decisão do `Choice State`, garantindo que o comportamento condicional responda exatamente às regras de negócio mapeadas.

---

## 🛠️ Código do Workflow Completo (Amazon States Language - ASL)

```json
{
  "Comment": "Pipeline de Processamento de Auditorias - Versão de Simulação de Sucesso",
  "StartAt": "GravarDadosSQLServer",
  "States": {
    "GravarDadosSQLServer": {
      "Type": "Pass",
      "Result": {
        "tipoCliente": "PREMIUM",
        "status": "Gravado com Sucesso no SQL Server"
      },
      "ResultPath": "$.ResultadosBanco.Payload",
      "Next": "VerificarNivelCliente"
    },
    "VerificarNivelCliente": {
      "Type": "Choice",
      "Choices": [
        {
          "Variable": "$.ResultadosBanco.Payload.tipoCliente",
          "StringEquals": "PREMIUM",
          "Next": "ProcessamentoPremiumParalelo"
        }
      ],
      "Default": "ProcessamentoStandard"
    },
    "ProcessamentoPremiumParalelo": {
      "Type": "Parallel",
      "End": true,
      "Branches": [
        {
          "StartAt": "GerarSnapshotNoS3",
          "States": {
            "GerarSnapshotNoS3": {
              "Type": "Pass",
              "Result": {
                "status": "PDF gerado com sucesso no S3"
              },
              "End": true
            }
          }
        },
        {
          "StartAt": "SincronizarAPIExterna",
          "States": {
            "SincronizarAPIExterna": {
              "Type": "Pass",
              "Result": {
                "status": "Sincronizado com API de Faturamento"
              },
              "End": true
            }
          }
        }
      ]
    },
    "ProcessamentoStandard": {
      "Type": "Pass",
      "Result": {
        "status": "Fluxo Standard Concluído"
      },
      "End": true
    }
  }
}