using System.Text;
using Haihv.DatDai.Aspire.ServiceDefault;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Extension.Logger.Elasticsearch.HostApp;
using Haihv.DatDai.Lib.Service.DbUp.PostgreSQL;
using Haihv.DatDai.Lib.Service.DvhcUpdate;
using Haihv.DatDai.Lib.Service.QuocTichUpdate;
using Haihv.DatDai.App.Background.Initiation.Entities;
using Haihv.DatDai.Lib.Extension.Audit.MongoDb;
using Haihv.DatDai.Lib.Extension.Configuration.Elasticsearch;

var builder = Host.CreateApplicationBuilder(args);
// Thiết lập hỗ trợ Console cho tiếng Việt
Console.OutputEncoding = Encoding.UTF8;
builder.AddServiceDefaults();
builder.AddLogToElasticsearch();

// Khởi tạo kết nối PostgreSQL
builder.AddPostgreSqlConnection();

// Khởi tạo kết nối Elasticsearch
builder.AddElasticsearchClient();

// Khỏi tạo Audit to MongoDB
builder.UseAuditDatToMongoDb();

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