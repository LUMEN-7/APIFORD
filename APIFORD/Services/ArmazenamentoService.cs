using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace APIFORD.Services;

public interface IArmazenamentoService
{
    Task<string> SalvarArquivoAsync(Stream conteudo, string nomeArquivo, string contentType);
    Task ExcluirArquivoAsync(string url);
    Task GarantirBucketExisteAsync();
}

public class ArmazenamentoService : IArmazenamentoService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _urlPublicaBase;

    public ArmazenamentoService(IConfiguration configuration)
    {
        _bucketName = configuration["Armazenamento:BucketName"]!;
        _urlPublicaBase = configuration["Armazenamento:UrlPublicaBase"]!;

        _s3Client = new AmazonS3Client(
            configuration["Armazenamento:AccessKey"],
            configuration["Armazenamento:SecretKey"],
            new AmazonS3Config { ServiceURL = configuration["Armazenamento:Endpoint"], ForcePathStyle = true }
        );
    }

    public async Task GarantirBucketExisteAsync()
    {
        bool existe = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucketName);
        if (existe) return;

        await _s3Client.PutBucketAsync(_bucketName);

        var politicaPublica = $$"""
        {
            "Version": "2012-10-17",
            "Statement": [{
                "Effect": "Allow",
                "Principal": "*",
                "Action": ["s3:GetObject"],
                "Resource": ["arn:aws:s3:::{{_bucketName}}/*"]
            }]
        }
        """;
        await _s3Client.PutBucketPolicyAsync(_bucketName, politicaPublica);
    }

    public async Task<string> SalvarArquivoAsync(Stream conteudo, string nomeArquivo, string contentType)
    {
        var chave = $"{Guid.NewGuid():N}-{nomeArquivo}";
        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = chave,
            InputStream = conteudo,
            ContentType = contentType
        });
        return $"{_urlPublicaBase}/{chave}";
    }

    public async Task ExcluirArquivoAsync(string url)
    {
        var chave = url.Split('/').Last();
        await _s3Client.DeleteObjectAsync(_bucketName, chave);
    }
}