using System;

namespace NullCyan.Sandboxnator.Commands;

public class ChatCommand()
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    //public CmdExecutionType ExecutionType { get; private set; }
    public Action<CommandContext> Handler { get; set; }


    public ChatCommand WithName(string name)
    {
        Name = name;
        return this;
    }

    public ChatCommand WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public ChatCommand WithHandler(Action<CommandContext> handler)
    {
        Handler = handler;
        return this;
    }

    public void Execute(CommandContext context)
    {
        Handler?.Invoke(context);
    }

}