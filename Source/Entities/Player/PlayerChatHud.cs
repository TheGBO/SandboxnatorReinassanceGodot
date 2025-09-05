using Godot;
using System;
using NullCyan.Sandboxnator.Chat;
using NullCyan.Util.ComponentSystem;
namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerChatHud : AbstractComponent<Player>
{
    [Export] public Control chatRoot;
    [Export] public LineEdit messageEdit;
    [Export] public RichTextLabel messageBox;
    [Export] public AudioStreamPlayer notificationSound;

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) return;
        ComponentParent.playerInput.OnShowChat += ShowChat;
        ComponentParent.playerInput.OnUiEscape += HideChat;
        ChatManager.Instance.OnMessageReceived += ReceiveMessage;
    }

    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;
        ComponentParent.playerInput.IsChatOpen = chatRoot.Visible;
    }

    private void ShowChat()
    {
        chatRoot.Visible = true;
    }

    private void HideChat()
    {
        chatRoot.Visible = false;
    }

    private void ReceiveMessage(ChatMessage message)
    {
        //-1: System notifications
        if (message.PlayerId != -1)
        {
            messageBox.Text += $"[color=green][{message.PlayerId}][/color]:{message.Content}\n";
        }
        else
        {
            messageBox.Text += $"[color=yellow][System]:[/color]{message.Content}\n";
        }
        notificationSound.Play();
    }

    public void _on_send_btn_pressed()
    {
        string msg = messageEdit.Text;
        if (!string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(messageEdit.Text))
        {
            ChatManager.Instance.SendMessage(msg);
            messageEdit.Text = "";
            messageEdit.GrabFocus();
        }
    }
}
