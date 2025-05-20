namespace FainCraft.Gameplay.WorldScripts.Signals;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var type = typeof(TEvent);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _handlers[type] = list;
        }
        list.Add(handler);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        var type = typeof(TEvent);
        if (_handlers.TryGetValue(type, out var list))
        {
            list.Remove(handler);
        }
    }

    public void Publish<TEvent>(TEvent evt)
    {
        var type = typeof(TEvent);
        if (_handlers.TryGetValue(type, out var list))
        {
            foreach (var handler in list)
            {
                ((Action<TEvent>)handler)(evt);
            }
        }
    }
}
