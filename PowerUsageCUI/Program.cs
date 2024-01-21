using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerUsageCUI.DataModel.Configuration;
using PowerUsageCUI.DataModel.PowerModel;
using PowerUsageCUI.DsmrReader;
using PowerUsageCUI.Logic;
using PowerUsageCUI.Services;

// Build the depency injection
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
builder.Services.AddHostedService<CollectorHostService>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<IMqttClientCollector, MqttClientCollector>(); 
builder.Services.AddScoped<IDsmrParser, DsmrParser>();
builder.Services.AddScoped<IP1HttpCollector, P1HttpCollector>();
builder.Services.AddSingleton(builder.Configuration.GetSection("MqttConfiguration").Get<MqttConfiguration>());
builder.Services.AddSingleton(builder.Configuration.GetSection("P1Configuration").Get<P1Configuration>());
builder.Services.AddDbContext<PowerModelContext>();
builder.Services.AddScoped<StatisticsReporterService>();
using IHost host = builder.Build();

// Start the program..
await host.RunAsync();

/* __TODO__ [PSMG] The following things are not yet implemented or needs to be improved:
/* - Better seperation of the logic. More HostServices needs to be created.
 * - Add groTT solar panels deseralization and obejct.
 * - DataModel needs to be improved.
 * - Handling of the 3x reporting from the ThreePhase meter from Zigbee2mqtt while only the last one contains all the changes
 * - Unit testing needs to be implemented
 */
