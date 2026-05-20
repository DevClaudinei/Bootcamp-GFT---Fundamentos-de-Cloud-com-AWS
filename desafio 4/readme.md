# 🚀 Desafio 4: Automatizando a Infraestrutura com AWS CloudFormation e .NET 7.0

Este módulo do projeto foi desenvolvido como parte do desafio de **Implementação de Infraestrutura Automatizada com AWS CloudFormation** do bootcamp **GFT - Fundamentos de Cloud com AWS** em parceria com a **DIO**.

O grande objetivo deste desafio foi aplicar os conceitos de **IaC (Infraestrutura como Código)** e **Cultura DevOps** para eliminar completamente qualquer configuração manual ou "quebra-galho" técnico no servidor de produção, garantindo que uma Minimal API legada em .NET 7.0 rode de forma nativa e isolada na nuvem.

---

## 🛠️ Arquitetura e Engenharia de Solução

A topologia da infraestrutura e o pipeline de entrega contínua foram modelados seguindo as melhores práticas de mercado:

* **Camada de Rede e Segurança (IaC):** Através do arquivo `template.yaml`, o AWS CloudFormation provisiona de forma declarativa uma VPC isolada, subnets públicas com mapeamento dinâmico de IPs, Internet Gateway, tabelas de roteamento e um Security Group (Firewall) configurado estritamente para liberar a porta SSH (22) para a esteira e a porta HTTP (5092) para a aplicação.
* **Provisionamento Automatizado (Boot da Instância):** O grande desafio técnico deste módulo foi adaptar a inicialização do Ubuntu 24.04 LTS para suportar o ecossistema do .NET 7.0 (já descontinuado pela Microsoft). O bloco `UserData` foi configurado para:
  1. Instalar a biblioteca de internacionalização e globalização `libicu-dev`, pré-requisito oculto do instalador do .NET em distribuições Linux recentes.
  2. Executar o script oficial de automação da Microsoft (`dotnet-install.sh`) apontando especificamente para o parâmetro `--runtime aspnetcore`, injetando no sistema o servidor Web Kestrel necessário para expor o Swagger.
  3. Criar links simbólicos globais no PATH do sistema (`/usr/bin/dotnet`).
* **Esteira de CI/CD (GitHub Actions):** Configurada no arquivo `.github/workflows/deploy.yml`, a esteira intercepta os commits na branch `main`, realiza o build nativo utilizando o SDK correto, transfere os artefatos compilados via protocolo seguro SCP e rein