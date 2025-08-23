using Godot;
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

}
