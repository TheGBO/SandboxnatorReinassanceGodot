using Godot;
using System;

public partial class BuildingTool : BaseTool
{
    [Export] private PackedScene buildingScene;
    [Export] private MeshInstance3D previewMesh;
    [Export] private float snapRange;

    public override void _Process(double delta)
    {
        if (!Ptu.parent.IsMultiplayerAuthority()) return;
        GeneratePreviewMesh();
    }

    private void GeneratePreviewMesh()
    {
        previewMesh.Visible = Ptu.rayCast.IsColliding();
        previewMesh.GlobalPosition = GetSnappedPosition(Ptu.rayCast.GetCollisionPoint(), Ptu.rayCast.GetCollisionNormal());
        previewMesh.GlobalRotation = Vector3.Zero;
    }

    //run on server
    //TODO: Reformulate method of passing tool usage data
    public override void UseTool(ToolUsageArgs args)
    {
        GD.Print($"block placed by {args.PlayerId}");
        Node3D building = (Node3D)buildingScene.Instantiate();
        building.Name = Guid.NewGuid().GetHashCode().ToString();
        building.Position = GetSnappedPosition(args.Position, args.Normal);
        World.Instance.neworkedEntities.CallDeferred("add_child", building);

    }

    private Vector3 GetSnappedPosition(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        return World.Instance.GetNearestSnapper(collisionPoint + collisionNormal / 2, snapRange);
        //return World.Instance.GetNearestSnapper(collisionPoint, snapRange);
    }
}
