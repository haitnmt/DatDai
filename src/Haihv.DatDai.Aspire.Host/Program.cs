var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

// Khởi động dịch vụ khởi tạo dữ liệu
var initiation = builder
    .AddProject<Projects.Haihv_DatDai_App_Background_Initiation>("initiation-service")
    .WithReference(cache)
    .WaitFor(cache);

var identityApi = builder
    .AddProject<Projects.Haihv_DatDa_App_Api_Identity>("identity-api")
    .WithReference(initiation)
    .WaitFor(initiation);

var diaChinhApi = builder
    .AddProject<Projects.Haihv_DatDai_App_Api_DiaChinh>("diachinh-api")
    .WithReference(identityApi)
    .WaitFor(identityApi);

// builder.AddProject<Projects.Haihv_DatDai_App_Web>("web-frontend")
//     .WithExternalHttpEndpoints()
//     .WithReference(diaChinhApi)
//     .WaitFor(diaChinhApi);

builder.Build().Run();