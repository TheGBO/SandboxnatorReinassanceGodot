using Godot;
using Godot.Collections;
using System;

//Individual player profile data class
//this should be serializable in order to save in the future.
public partial class PlayerProfileData : Resource
{
    [Export]
    public string PlayerName { get; set; } = "Player";

    [Export]
    public Color PlayerColor { get; set; } = Colors.White;
    //TODO: PlayerTexture : The skin texture of the player, a Map that maps a string to a .tres material, this class will only get the texture, there will be a singleton in the future that will hold the registry for player skins in that dictionary.

    public Dictionary<string, Variant> ToDictionary()
    {
        return new Dictionary<string, Variant>
        {
            { "PlayerName", PlayerName },
            { "PlayerColor", PlayerColor }
        };
    }

    public static PlayerProfileData FromDictionary(Dictionary<string, Variant> dict)
    {
        var data = new PlayerProfileData();
        if (dict.TryGetValue("PlayerName", out var name))
            data.PlayerName = (string)name;

        if (dict.TryGetValue("PlayerColor", out var color))
            data.PlayerColor = (Color)color;

        return data;
    }


}
