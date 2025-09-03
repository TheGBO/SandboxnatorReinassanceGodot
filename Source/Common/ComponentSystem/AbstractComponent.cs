using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;


/// <summary>
/// Holds the components of an entity
/// </summary>

//TODO: maybe consider to put properties as protected
public abstract partial class AbstractComponent<T> : Node3D where T : Node3D
{
	public T ComponentParent { get; protected set;}
	/// <summary>
	/// Set the component's parent.
	/// </summary>
	public void SetParent(T parent)
	{
		ComponentParent = parent;
	}

}
