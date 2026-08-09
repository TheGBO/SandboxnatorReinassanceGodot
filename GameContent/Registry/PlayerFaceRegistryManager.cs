using System.Collections.Generic;
using System.Linq;
using Godot;
using NullGarel.Sandboxnator.Item;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util;
using NullGarel.Util.IO;
using NullGarel.Util.Log;

namespace NullGarel.Sandboxnator.Entity.PlayerCosmetics;

public class PlayerFaceRegistryManager : IRegistryManager
{
    private readonly string itemContentsPath = "res://GameContent/Skins";

    public void Register()
    {
        //GD.Print("REGISTAREN FECESFWOGHWAERG)");
        List<Resource> faceResources = ResourceIO.GetResources<PlayerFaceData>(itemContentsPath);
        //GD.Print(faceResources.Count);
        foreach (PlayerFaceData res in faceResources.Cast<PlayerFaceData>())
        {
            NcLogger.Log($"Valid playerFace resource is {res.playerFaceId}, registering...", NcLogger.LogType.Register);
            //Register the item via resource
            GameRegistries.Instance.PlayerFaceRegistry.Register(res.playerFaceId, res);
        }
    }

    public static Texture2D GetTextureByFaceId(string id) => GameRegistries.Instance.PlayerFaceRegistry.Get(id).faceTexture;
}