using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Scenes
{
    public interface IScene
    {
        public void OnLoad();
        public void OnUnload();
    }
}
