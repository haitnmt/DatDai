using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Extension.Configuration.Elasticsearch;

public static class ElasticsearchExtensions
{
    private static ElasticsearchClientSettings GetElasticsearchClientSettings(this IConfigurationManager configurationManager,
        string sectionName = "Elasticsearch", string uriKey = "Uris", 
        string? tokenKey = null, string? usernameKey = null, string? passwordKey = null)
    {
        List<Node> nodes = [];
        var configurationSection = configurationManager.GetSection(sectionName);
        nodes.AddRange(from stringUri in configurationSection.GetSection(uriKey).GetChildren()
            where !string.IsNullOrWhiteSpace(stringUri.Value)
            select new Node(new Uri(stringUri.Value!)));
        var pool = new StaticNodePool(nodes);
        tokenKey ??= "Token";
        var token = configurationSection[tokenKey] ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(token))
        {
            return new ElasticsearchClientSettings(pool)
                .Authentication(new ApiKey(token))
                .ServerCertificateValidationCallback(CertificateValidations.AllowAll);
        }
        usernameKey ??= "Username";
        passwordKey ??= "Password";
        var username = configurationSection[usernameKey] ?? string.Empty;
        var password = configurationSection[passwordKey] ?? string.Empty;
        var elasticsearchClientSettings = new ElasticsearchClientSettings(pool)
            .Authentication(new BasicAuthentication(username, password))
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll);
        return elasticsearchClientSettings;
    }

    private static ElasticsearchClientSettings GetElasticsearchClientSettings(this IHostApplicationBuilder builder,
        string sectionName = "Elasticsearch", string uriKey = "Uris", 
        string? tokenKey = null, string? usernameKey = null, string? passwordKey = null)
        => GetElasticsearchClientSettings(builder.Configuration, sectionName, uriKey, tokenKey, usernameKey, passwordKey);
    

/// <summary>
/// Thêm <see cref="ElasticsearchClientSettings"/> đăng ký dưới dạng singleton.
/// </summary>
/// <param name="builder"><see cref="IHostApplicationBuilder"/> để cấu hình ứng dụng.</param>
/// <param name="sectionName">Tên của phần cấu hình Elasticsearch.</param>
/// <param name="uriKey">Khóa để lấy danh sách URI của Elasticsearch.</param>
/// <param name="tokenKey">Khóa để lấy token API (nếu có).</param>
/// <param name="usernameKey">Khóa để lấy tên người dùng (nếu có).</param>
/// <param name="passwordKey">Khóa để lấy mật khẩu (nếu có).</param>
public static void AddElasticsearchClient(this IHostApplicationBuilder builder,
    string sectionName = "Elasticsearch", string uriKey = "Uris", 
    string? tokenKey = null, string? usernameKey = null, string? passwordKey = null)
{
    var elasticsearchClientSettings = GetElasticsearchClientSettings(builder, sectionName, uriKey, tokenKey, usernameKey, passwordKey);
    builder.Services.AddSingleton(elasticsearchClientSettings);
}

}