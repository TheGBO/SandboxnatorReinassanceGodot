using System.Collections.Generic;
using Godot;
namespace NullCyan.Util.GodotHelpers;

public class NodeUtils
{
    public static List<Node> GetAllChildrenInNode(Node node, List<Node> nodes = null)
    {
        nodes ??= new List<Node>();
        nodes.Add(node);
        if (nodes != null)
        {
            foreach (Node child in node.GetChildren())
            {
                nodes = GetAllChildrenInNode(child, nodes);
            }
        }
        return nodes;
    }
}