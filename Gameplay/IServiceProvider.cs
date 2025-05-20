using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay;

public interface IServiceProvider
{
    public T Get<T>();
}

public class ServiceContainer : IServiceProvider
{
    private readonly Dictionary<Type, Func<object>> _factories = new();
    private readonly Dictionary<Type, object> _instances = new();

    public T RegisterSingleton<T>() where T : new()
    {
        var instance = new T();
        _instances[typeof(T)] = instance;
        return instance;
    }

    public T RegisterSingleton<T>(T instance) where T : notnull
    {
        _instances[typeof(T)] = instance;
        return instance;
    }

    public void RegisterFactory<T>() where T : new()
    {
        _factories[typeof(T)] = () => new T();
    }

    public void RegisterFactory<T>(Func<T> factory) where T : notnull
    {
        _factories[typeof(T)] = () => factory();
    }

    public T Get<T>()
    {
        var type = typeof(T);

        if (_instances.TryGetValue(type, out var instance))
        {
            return (T)instance;
        }

        if (_factories.TryGetValue(type, out var factory))
        {
            var obj = factory();
            _instances[type] = obj; // lazy singleton
            return (T)obj;
        }

        throw new InvalidOperationException($"Service of type {type} not registered.");
    }
}
