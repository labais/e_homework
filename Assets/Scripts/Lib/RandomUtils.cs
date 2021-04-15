using System;

public static class RandomUtils
{
    public static bool TryInvoke(this Action action)
    {
        if (action == null)
        {
            return false;
        }

        action();
        return true;
    }
  
}