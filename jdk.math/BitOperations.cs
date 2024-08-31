using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics;

#if !NETCOREAPP3_0_OR_GREATER
internal static class BitOperations {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(uint value) {
        if (value == 0)
            return 32;

        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        return 32 - PopCount(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PopCount(uint x) {
        x -= (x >> 1) & 0x55555555;
        x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
        x = (x + (x >> 4)) & 0x0F0F0F0F;
        x += x >> 8;
        x += x >> 16;
        return (int) (x & 0x3F);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ulong value) {
        uint hi = (uint) (value >> 32);

        if (hi == 0) {
            return 32 + LeadingZeroCount((uint) value);
        }

        return LeadingZeroCount(hi);
    }
}
#endif
