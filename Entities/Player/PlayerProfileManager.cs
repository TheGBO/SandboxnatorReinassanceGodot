using Godot;
using System.Collections.Generic;
using System;

//This class will manage the player profiles, it will hold a list of player profiles and will be able to add, remove and edit them.
//This class will also be a singleton, this means that there will only be one instance of this class in the game.
//It will be a client side node, there should be a way to share this via the network somehow and make the player profiles accessible to all clients.
//Other players should be able to see the profiles of other players.
public partial class PlayerProfileManager : Node
{
    public PlayerProfileData CurrentProfile { get; set; }

}

