using System;
using Godot;
namespace NullCyan.Util;
/// <summary>
/// Singleton base code, use _Ready instead of _EnterTree() in case of overriding,
/// if using _EnterTree() is needed on singletons, call SetInstance();
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class Singleton<T> : Node where T : Singleton<T>
{

    public static T Instance;

    public override void _EnterTree()
    {
        SetInstance();
    }

    protected void SetInstance()
    {
        if (Instance != this && Instance != null)
        {
            GD.PrintErr("[ERROR] Invalid singleton state");
            Instance.QueueFree();
        }
        Instance = (T)this;
    }

    public static bool HasInstance()
    {
        return Instance != null && IsInstanceValid(Instance);
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }
}
