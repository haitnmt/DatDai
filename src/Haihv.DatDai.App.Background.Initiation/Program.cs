using System.Text;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Haihv.DatDai.Aspire.ServiceDefault;
using Haihv.DatDai.Lib.Extension.Configuration;
using Haihv.DatDai.Lib.Extension.Logger.Elasticsearch.HostApp;
using Haihv.DatDai.Lib.Service.DbUp.PostgreSQL;
using Haihv.DatDai.Lib.Service.DvhcUpdate;
using Haihv.DatDai.Lib.Service.QuocTichUpdate;
using Haihv.DatDai.App.Background.Initiation.Entities;

var builder = Host.CreateApplicationBuilder(args);
// Thiết lập hỗ trợ Console cho tiếng Việt
Console.OutputEncoding = Encoding.UTF8;
builder.AddServiceDefaults();
builder.AddLogToElasticsearch();

// Lấy thông tin kết nối CSDL:
var postgreSqlConnection = builder.GetPostgreSqlConnectionString();

// Khởi tạo Elasticsearch
List<Node> nodes = [];
nodes.AddRange(from stringUri in builder.Configuration.GetSection("Elasticsearch:Uris").GetChildren()
    where !string.IsNullOrWhiteSpace(stringUri.Value)
    select new Node(new Uri(stringUri.Value!)));
var pool = new StaticNodePool(nodes);
var token = builder.Configuration["Elasticsearch:encoded"] ?? string.Empty;
var elasticsearchClientSettings = new ElasticsearchClientSettings(pool)
    .Authentication(new ApiKey(token))
    .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

builder.Services.AddSingleton(postgreSqlConnection);
builder.Services.AddSingleton(elasticsearchClientSettings);

// Đăng ký dịch vụ cập nhật cấu trúc cơ sở dữ liệu
builder.Services.AddSingleton<DataBaseInitializer>();

// Đăng ký dịch vụ cập nhật dữ liệu
builder.Services.AddHostedService<DvhcUpdateService>();
builder.Services.AddHostedService<DanTocUpdateService>();
builder.Services.AddHostedService<QuocTichUpdateService>();

var host = builder.Build();

// Thực hiện cập nhật cấu trúc cơ sở dữ liệu
var dataBaseInitializer = host.Services.GetRequiredService<DataBaseInitializer>();
await dataBaseInitializer.InitializeAsync();

host.Run();