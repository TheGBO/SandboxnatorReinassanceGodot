using Godot;
using Godot.Collections;
using System;

public partial class Profile : Control
{
    public void _on_save_and_return_btn_pressed()
    {
        //TODO: Save profile settings lol
        GetTree().ChangeSceneToPacked(ScenesBank.Instance.mainMenuScene);
    }

    private void FillNameField()
    {
        NameGenerator nameGen = new NameGenerator(new Array<string>() { "p", "t", "k", "f", "s", "h", "m", "n", "r", "sh" }, "aeiou");
        string name = nameGen.GenerateName(3);
        string nameCorrected = char.ToUpper(name[0]) + name.Substring(1);
    }
}
