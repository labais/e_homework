using deVoid.Utils;
using UnityEngine;

public class HoleGeneratedSignal : ASignal<Transform, Transform>
{
}

public class ShakeCameraSignal : ASignal<float, float>
{
}

public class LevelGeneratedSignal : ASignal<float, float>
{
}

public class PlayerDiedSignal : ASignal
{
}

public class MustShowGameOverSignal : ASignal
{
}

public class MustShowRestartSignal : ASignal<bool>
{
}

public class PlayerFinishedSignal : ASignal
{
}

public class PlayerGotHitSignal : ASignal
{
}

public class PlayerStartedControllingSignal : ASignal
{
}

public class DisableOnScreenGamepad : ASignal
{
}

public class EnemyDiedSignal : ASignal<bool>
{
}

public class PointsChangedSignal : ASignal
{
}

public class ShakeCoinsSignal : ASignal
{
}

public class UpgradesChangedSignal : ASignal
{
}
