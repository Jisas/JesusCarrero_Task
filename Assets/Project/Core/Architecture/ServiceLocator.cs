using System.Collections.Generic;
using System;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, IGameService> _services = new();

    public static void Register<T>(T service) where T : IGameService
    {
        _services[typeof(T)] = service;
    }

    public static T Get<T>() where T : IGameService
    {
        return (T)_services[typeof(T)];
    }
}