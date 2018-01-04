using System;
using System.Runtime.InteropServices;

namespace GrandTheftSpace.CoreGame.NativeMemory.Structures
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct camGameplayDirector
    {
        [FieldOffset(0x0020)] camFrame m_frames;

        public camFrame* GetFrames()
        {
            fixed (camFrame* frames = &m_frames)
            {
                return frames;
            }
        }
    }
}
