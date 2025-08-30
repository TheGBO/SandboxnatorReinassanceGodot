using System;
using System.Collections.Generic;
using Godot;

public abstract partial class NetworkedEntityManager<T> : Node where T : Node
{
    [Export] protected Node3D spawnPath;

    protected Dictionary<int, T> entities = new Dictionary<int, T>();

    public T GetEntity(int id)
    {
        entities.TryGetValue(id, out T entity);
        return entity;
    }

    public IEnumerable<T> GetAllEntities() => entities.Values;

    public virtual void AddEntity(int id, T entity)
    {
        entities[id] = entity;

        if (spawnPath != null)
            spawnPath.AddChild(entity);
        else
            AddChild(entity);

        GD.Print($"{typeof(T).Name} {id} added");
    }


    public virtual void RemoveEntity(int id)
    {
        if (entities.TryGetValue(id, out T entity))
        {
            entity.QueueFree();
            entities.Remove(id);
            GD.Print($"{typeof(T).Name} {id} removed");
        }
    }

    public bool HasEntity(int id) => entities.ContainsKey(id);
}
