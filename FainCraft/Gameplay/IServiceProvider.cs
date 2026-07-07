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
