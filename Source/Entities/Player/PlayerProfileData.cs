using Godot;
using Godot.Collections;
using System;
namespace NullCyan.Sandboxnator.Entity;

//Individual player profile data class
//this should be serializable in order to save in the future.
public partial class PlayerProfileData : Resource
{
    [Export]
    public string PlayerName { get; set; } = $"DEFAULT_PLAYER";

    [Export]
    public Color PlayerColor { get; set; } = Colors.White;
    //TODO: PlayerTexture : The skin texture of the player, a Map that maps a string to a .tres material, this class will only get the texture, there will be a singleton in the future that will hold the registry for player skins in that dictionary.

    public Dictionary ToDictionary()
    {
        return new Dictionary
        {
            { "PlayerName", PlayerName },
            { "PlayerColor", PlayerColor.ToHtml() }
        };
    }

    public static PlayerProfileData FromDictionary(Dictionary dict)
    {
        var data = new PlayerProfileData();
        if (dict.TryGetValue("PlayerName", out var name))
            data.PlayerName = (string)name;

        if (dict.TryGetValue("PlayerColor", out var color))
            data.PlayerColor = Color.FromHtml((string)color);

        return data;
    }


}

