using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GrandTheftSpace.CoreGame.NativeMemory.Structures;

namespace GrandTheftSpace.CoreGame.NativeMemory
{
    internal static unsafe class MemoryAccess
    {
        private delegate ulong GetRenderingCamDelegate();

        private static GetRenderingCamDelegate GetRenderingCamera;

        static MemoryAccess()
        {
        }

        public static camBaseCamera* RenderingCameraBaseCamera {
            get {
                if (GetRenderingCamera == null)
                {
                    byte* address;
                    address = FindPattern("\x48\x85\xC0\x0F\x84\x00\x00\x00\x00\x44\x38\x2D\x00\x00\x00\x00\x75\x0D", "xxxxx????xxx????xx") - 11;
                    GetRenderingCamera = Marshal.GetDelegateForFunctionPointer<GetRenderingCamDelegate>(new IntPtr(address + *(int*)(address + 1) + 5));
                }

                return (camBaseCamera*)GetRenderingCamera();
            }
        }

        /// <summary>
        ///     Edited copy of source: https://github.com/crosire/scripthookvdotnet
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static byte* FindPattern(string pattern, string mask)
        {
            GetModuleAddress(out ulong address, out ulong endAddress);

            for (; address < endAddress; address++)
            {
                for (var i = 0; i < pattern.Length; i++)
                {
                    var addressPtr = (byte*)address;

                    if (addressPtr == null)
                    {
                        throw new NullReferenceException();
                    }

                    const char Delim = '?';

                    char maskChar = mask[i];

                    char patternChar = pattern[i];

                    byte addrByte = addressPtr[i];

                    if (maskChar != Delim && addrByte != patternChar)
                    {
                        break;
                    }

                    var next = i + 1;

                    if (next == pattern.Length)
                    {
                        return addressPtr;
                    }
                }
            }

            return null;
        }

        private static void GetModuleAddress(out ulong address, out ulong endAddress)
        {
            var process = Process.GetCurrentProcess();
            var module = process.MainModule;
            address = (ulong)module.BaseAddress.ToInt64();
            endAddress = address + (ulong)module.ModuleMemorySize;
        }
    }

    //private delegate ulong GetGameplayCamDirectorDelegate();
    //address = FindPattern("\x40\x53\x48\x83\xEC\x20\x48\x8B\xD9\xE8\x00\x00\x00\x00\x8B\x90\x00\x00\x00\x00\x89\x13", "xxxxxxxxxx????xx????xx");
    //var getGameplayCamDirDelegate = Marshal.GetDelegateForFunctionPointer<GetGameplayCamDirectorDelegate>(new IntPtr(address + *(int*)(address + 10) + 14));
    //CamGameplayDirector = (camGameplayDirector*)getGameplayCamDirDelegate();
}
