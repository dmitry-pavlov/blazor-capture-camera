using System;
using CaptureCamera.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CaptureCamera.Extensions;

public static class ConfigurationExtensions
{
    public static TAppSettings ConfigureAppSettings<TAppSettings>(this IServiceCollection services, IConfiguration configuration)
        where TAppSettings : class, ISelfCheck
    {
        services.Configure<TAppSettings>(configuration);
        var settings = configuration.Get<TAppSettings>() ?? throw new SelfCheckException($"Can't configure {typeof(TAppSettings)}.");
        settings.Check();

        services.TryAddSingleton(settings);
        return settings;
    }

    public static TSettings ConfigureSection<TSettings>(this IServiceCollection services,
        IConfiguration configuration)
        where TSettings : class
        => services.ConfigureSection<TSettings>(configuration, typeof(TSettings).Name);

    public static TSettings ConfigureSection<TSettings>(this IServiceCollection services,
        IConfiguration configuration, string name)
        where TSettings : class
    {
        // NOTE: add using IOptions pattern https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options to get from DI via IOptions<Settings>
        var section = configuration.GetSection(name);
        services.Configure<TSettings>(section);

        // NOTE: as well as singleton to get from DI directly without IOptions pattern
        var settings = section.Get<TSettings>();

        switch (settings)
        {
            case null:
                throw new SelfCheckException($"'{name}' section not found in configuration.");
            case ISelfCheck selfCheckSettings:
                selfCheckSettings.Check();
                break;
        }

        services.TryAddSingleton(settings);

        return settings;
    }

    public static void ThrowIfNullOrEmpty(this ISelfCheck? settings, string? value, string name)
    {
        settings.ThrowIf(() => string.IsNullOrWhiteSpace(value), $"{name} not provided.");
    }

    public static void ThrowIfZeroOrLess(this ISelfCheck? settings, int value, string name)
    {
        settings.ThrowIf(() => value <= 0, $"{name} must be greater than zero.");
    }

    public static void ThrowIfNotValidGuid(this ISelfCheck? settings, string? value, string name)
    {
        settings.ThrowIf(() => !Guid.TryParse(value, out _), $"{name} must be GUID.");
    }

    public static void ThrowIfNotValidUri(this ISelfCheck? settings, string? value, string name)
    {
        settings.ThrowIf(() => !Uri.TryCreate(value, UriKind.Absolute, out _), $"{name} must be an absolute URL");
    }

    internal static void ThrowIf(this ISelfCheck? settings, Func<bool> invalidCondition, string message)
    {
        var invalid = invalidCondition.Invoke();

        if (invalid) throw new SelfCheckException(message, settings);
    }

    private class SelfCheckException : Exception
    {
        public SelfCheckException(string? message = null, ISelfCheck? checker = null)
            : base(
                $"{(checker == null ? "Self check failed" : $"{checker.GetType().Name} self check failed")}. {message}")
        {
        }
    }
}