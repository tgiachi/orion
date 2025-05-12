using System.Runtime.CompilerServices;

namespace Orion.Foundations.Extensions;

public static class MathExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> =>
        val.CompareTo(min) < 0 ? min :
        val.CompareTo(max) > 0 ? max : val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T val, T min) where T : IComparable<T> => val.CompareTo(min) < 0 ? val : min;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(T val, T max) where T : IComparable<T> => val.CompareTo(max) > 0 ? val : max;
}
