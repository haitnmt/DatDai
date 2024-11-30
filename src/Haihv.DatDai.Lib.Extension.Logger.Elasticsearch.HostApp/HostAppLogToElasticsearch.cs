using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haihv.DatDai.Lib.Extension.Logger.Elasticsearch.HostApp;

public static class HostAppLogToElasticsearch
{
    /// <summary>
    /// Thêm cấu hình ghi log vào Elasticsearch.
    /// </summary>
    /// <param name="builder">Đối tượng HostApplicationBuilder.</param>
    /// <param name="uriKey">Khóa cấu hình cho URI của Elasticsearch (mặc định là "Elasticsearch:Uris").</param>
    /// <param name="tokenKey">Khóa cấu hình cho API token của Elasticsearch (mặc định là "Elasticsearch:encoded").</param>
    /// <param name="usernameKey">Khóa cấu hình cho tên người dùng của Elasticsearch (mặc định là "Elasticsearch:username").</param>
    /// <param name="passwordKey">Khóa cấu hình cho mật khẩu của Elasticsearch (mặc định là "Elasticsearch:password").</param>
    public static void AddLogToElasticsearch(
        this IHostApplicationBuilder builder,
        string? uriKey = null, string? tokenKey = null, string? usernameKey = null, string? passwordKey = null)
    {
        var loggerConfiguration = builder.CreateLoggerConfiguration(uriKey, tokenKey, usernameKey, passwordKey);
        builder.Services.AddSerilog(loggerConfiguration.CreateLogger());
    } 
}