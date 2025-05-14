using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Scenes
{
    public class SceneFactory
    {
        public IScene EmptyScene() => new EmptyScene();
    }
}
