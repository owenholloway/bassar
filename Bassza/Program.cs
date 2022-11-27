using Autofac;
using Bassza;
using Bassza.Features;
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

builder.RegisterType<Signals>().SingleInstance();

builder.RegisterType<Main>();

var conainer = builder.Build();


conainer.Resolve<Main>().Run();

conainer.Resolve<Signals>().ApplicationDone.WaitOne();
