using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.Gamemodes;

public interface IGamemode
{
    public void EnterState();
    public void UpdatePosition(Vector2 moveInputs);
    public void ExitState();
}
