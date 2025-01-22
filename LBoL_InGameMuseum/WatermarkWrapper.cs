using System.Runtime.CompilerServices;

namespace LBoL_InGameMuseum
{
    internal class WatermarkWrapper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ActivateWatermark() => AddWatermark.API.ActivateWatermark();
    }
}