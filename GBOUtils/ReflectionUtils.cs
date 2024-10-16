using System;
using System.Reflection;

class ReflectionUtils
{
    public static Delegate[] GetInvocationList(Action action)
    {
        // Use reflection to get the invocation list
        if (action != null)
        {
            var field = action.GetType().GetField("invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Delegate[])field.GetValue(action);
        }
        return Array.Empty<Delegate>();
    }
}