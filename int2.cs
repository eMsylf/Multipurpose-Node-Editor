using System;

/// <summary>
/// A struct holding 2 int variables
/// </summary>
[Serializable]
public struct Int2
{
    public int a;
    public int b;

    public Int2(int a, int b)
    {
        this.a = a;
        this.b = b;
    }
}
