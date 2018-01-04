using System.Runtime.InteropServices;

namespace GrandTheftSpace.CoreGame.NativeMemory.Structures
{
    [StructLayout(LayoutKind.Explicit, Size = 0x00E0)]
    public unsafe struct camFrame
    {
        [FieldOffset(0x0078)] public float farClip;
    }
}
