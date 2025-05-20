using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Signals;

public interface IEventBus
{
    public void Subscribe<TEvent>(Action<TEvent> handler);
    public void Unsubscribe<TEvent>(Action<TEvent> handler);
    public void Publish<TEvent>(TEvent evt);
}
