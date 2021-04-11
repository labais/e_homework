using deVoid.Utils;
using UnityEngine;

public class HoleGeneratedSignal : ASignal<Transform, Transform>
{
}


public class ShakeCameraSignal : ASignal<float, float>
{
}

public class PlayerDiedSignal : ASignal
{
}
public class PlayerFinishedSignal : ASignal
{
}