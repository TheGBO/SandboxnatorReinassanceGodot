using Godot;
using System;

public partial class PlayerChatHud : AbstractPlayerComponent
{
    [Export] public Control chatRoot;
    [Export] public TextEdit messageEdit;
    [Export] public RichTextLabel messageBox;
    [Export] public AudioStreamPlayer notificationSound;

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) return;
        parent.playerInput.OnShowChat += ShowChat;
        parent.playerInput.OnUiEscape += HideChat;
        ChatManager.Instance.OnMessageReceived += ReceiveMessage;
    }

    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;
        parent.playerInput.IsChatOpen = chatRoot.Visible;
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
        if (message.PlayerId != -1)
        {
            messageBox.Text += $"[color=green][{message.PlayerId}][/color]:{message.Content}\n";
        }
        else
        {
            messageBox.Text += $"[color=yellow][{message.Content}][/color]\n";
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
        }
    }
}
