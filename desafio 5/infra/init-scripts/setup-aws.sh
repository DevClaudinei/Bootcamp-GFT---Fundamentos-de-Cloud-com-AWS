#!/bin/bash
echo "===================================================="
echo "🚀 INICIALIZANDO INFRAESTRUTURA LOCAL NO LOCALSTACK 🚀"
echo "===================================================="

# 1. Criando o Bucket no Amazon S3
echo "📦 Criando bucket S3 'bucket-carga-dados'..."
awslocal s3 mb s3://bucket-carga-dados

# 2. Criando a Tabela no Amazon DynamoDB
echo "🗄️ Criando tabela 'TabelaMetadados' no DynamoDB..."
awslocal dynamodb create-table \
    --table-name TabelaMetadados \
    --attribute-definitions AttributeName=Id,AttributeType=S \
    --key-schema AttributeName=Id,KeyType=HASH \
    --billing-mode PAY_PER_REQUEST

echo "===================================================="
echo "✅ INFRAESTRUTURA CONFIGURADA COM SUCESSO! ✅"
echo "===================================================="