using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util;

namespace NullGarel.Sandboxnator.Entity.PlayerCosmetics;

public class PlayerFaceRegistryManager : IRegistryManager
{

    public void Register()
    {
        foreach (PlayerFaceData face in GameRegistries.Instance.ContentDatabase.PlayerFaces)
        {
            GameRegistries.Instance.PlayerFaceRegistry.Register
            (
                face.PlayerFaceId,
                face
            );
        }
    }

    public static Texture2D GetTextureByFaceId(string id) => GameRegistries.Instance.PlayerFaceRegistry.Get(id).FaceTexture;
}