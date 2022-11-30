using Autofac;
using Bassza;
using Bassza.Features;
using Bassza.Features.GoogleSheets;
using CommandLine;
using Serilog;
using Serilog.Events;

var builder = new ContainerBuilder();

builder.Register(r =>
{
    var options = new Options();
    Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed<Options>((o) => options = o);
    return options;
}).As<Options>().SingleInstance();

builder.Register(r =>
{
    var obj = new LoggerConfiguration();
    obj.MinimumLevel.Debug();
    obj.WriteTo.Console();
    return obj.CreateLogger();
    
}).As<ILogger>().SingleInstance();

builder.RegisterType<SetupLogger>().SingleInstance();
builder.RegisterType<Signals>().SingleInstance();

builder.RegisterType<SheetsApiManager>().SingleInstance();
builder.RegisterType<TotpManager>().SingleInstance();
builder.RegisterType<Main>();

var container = builder.Build();

container.Resolve<SetupLogger>().Run();

await container.Resolve<SheetsApiManager>().TryConnect();

await container.Resolve<Main>().Run();

container.Resolve<Signals>().ApplicationDone.Set();
