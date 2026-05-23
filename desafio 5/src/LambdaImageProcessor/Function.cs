using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Registro do Serializer nativo para a Lambda interpretar o JSON do S3
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaImageProcessor;

public class Function
{
    private readonly AmazonS3Client _s3Client;
    private readonly AmazonDynamoDBClient _dynamoClient;

    public Function()
    {
        // 🚨 SEGREDO DO LOCALSTACK: Forçamos o SDK a conversar com o nosso container (localhost:4566)
        var s3Config = new AmazonS3Config 
        { 
            ServiceURL = "http://localhost:4566",
            ForcePathStyle = true // Necessário para o LocalStack entender rotas locais de bucket
        };
        
        var dynamoConfig = new AmazonDynamoDBConfig 
        { 
            ServiceURL = "http://localhost:4566" 
        };

        // Como o LocalStack aceita credenciais fakes, passamos valores fictícios de chaves
        _s3Client = new AmazonS3Client("fake_access_key", "fake_secret_key", s3Config);
        _dynamoClient = new AmazonDynamoDBClient("fake_access_key", "fake_secret_key", dynamoConfig);
    }

    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        var eventRecord = evnt.Records?[0].S3;
        if (eventRecord == null) return;

        string bucketName = eventRecord.Bucket.Name;
        string fileKey = eventRecord.Object.Key;

        context.Logger.LogInformation($"[S3 Event] Novo arquivo detectado para processamento: {fileKey} no bucket {bucketName}");

        try
        {
            // 1. Busca os metadados e propriedades do arquivo no S3 Local
            var metadata = await _s3Client.GetObjectMetadataAsync(bucketName, fileKey);

            // 2. Estrutura o mapeamento NoSQL para salvar no DynamoDB
            var request = new PutItemRequest
            {
                TableName = "TabelaMetadados",
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = Guid.NewGuid().ToString() } },
                    { "NomeArquivo", new AttributeValue { S = fileKey } },
                    { "TamanhoBytes", new AttributeValue { N = metadata.Headers.ContentLength.ToString() } },
                    { "TipoConteudo", new AttributeValue { S = metadata.Headers.ContentType } },
                    { "DataProcessamento", new AttributeValue { S = DateTime.UtcNow.ToString("o") } }
                }
            };

            // 3. Persiste os dados na tabela local de Metadados
            await _dynamoClient.PutItemAsync(request);
            context.Logger.LogInformation($"✅ Sucesso: Metadados de '{fileKey}' processados e gravados no DynamoDB!");
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"❌ Erro crítico ao processar o evento do S3: {ex.Message}");
            throw;
        }
    }
}