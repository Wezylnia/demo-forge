using DemoForge.Cli.Commands;
using DemoForge.Cli.Infrastructure;
using Spectre.Console.Cli;

var registrar = new TypeRegistrar();
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("demoforge");
    config.AddCommand<InitCommand>("init");
    config.AddCommand<ValidateCommand>("validate");
    config.AddCommand<RunCommand>("run");
    config.AddCommand<RenderCommand>("render");
});

return app.Run(args);

internal sealed class TypeRegistrar : ITypeRegistrar
{
    public ITypeResolver Build()
    {
        return new TypeResolver();
    }

    public void Register(Type service, Type implementation)
    {
    }

    public void RegisterInstance(Type service, object implementation)
    {
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
    }
}

internal sealed class TypeResolver : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type)
    {
        if (type is null)
        {
            return null;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var elementType = type.GetGenericArguments()[0];
            return Array.CreateInstance(elementType, 0);
        }

        return Activator.CreateInstance(type);
    }

    public void Dispose()
    {
    }
}
