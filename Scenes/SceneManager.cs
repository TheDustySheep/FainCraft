namespace FainCraft.Scenes;

public class SceneManager
{
    public static SceneManager Instance { get; private set; } = null!;

    private IScene? _current;
    private SceneFactory _factory;

    private void SetInstance()
    {
        if (Instance != null)
            throw new Exception("Only one scene manager is allowed");


        Instance = this;
    }

    public SceneManager(SceneFactory factory, IScene? scene = null)
    {
        SetInstance();
        _current = scene;
        _factory = factory;
    }

    public void LoadScene(IScene scene)
    {
        _current?.OnUnload();
        _current = scene;
        _current.OnLoad();
    }

    public void LoadScene<T>() where T : IScene, new()
    {
        _current?.OnUnload();
        _current = new T();
        _current.OnLoad();
    }

    public void LoadScene(Func<SceneFactory, IScene> func)
    {
        _current?.OnUnload();
        _current = func.Invoke(_factory);
        _current.OnLoad();
    }
}
