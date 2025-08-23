using Godot;
using Godot.Collections;
using System;

public partial class ProfileEditingMenu : Control
{
    [Export] LineEdit nameEdit;

    public void _on_save_and_return_btn_pressed()
    {
        //TODO: Save profile settings lol
        GetTree().ChangeSceneToPacked(ScenesBank.Instance.mainMenuScene);
    }

    public override void _Ready()
    {
        FillNameField();
    }

    private void FillNameField()
    {
        NameGenerator nameGen = new NameGenerator(new Array<string>() { "p", "t", "k", "f", "s", "h", "m", "n", "r", "sh", "l", "y", "w", "b", "d", "g", "v", "z", "gh" }, "aeiou");
        string name = nameGen.GenerateName(3);
        string nameCorrected = char.ToUpper(name[0]) + name.Substring(1);
        nameEdit.Text = nameCorrected;
        GD.Print(nameCorrected);
    }
}
