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


    //Placeholder to test name generation, this random name generation should only happen when there is no existing player profile.
    private void FillNameField()
    {
        NameGenerator nameGen = NameGenerator.Create();
        GD.Randomize();
        if (GD.Randf() >= 0.5)
        {
            nameGen.UseWesternPatterns();
        }
        else
        {
            nameGen.UseSimplePatterns();
        }
        string name = nameGen.GenerateName();
        string nameCorrected = char.ToUpper(name[0]) + name.Substring(1);
        nameEdit.Text = nameCorrected;
        GD.Print(nameCorrected);
    }
}
