using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Orion.Foundations.Collections;
using Orion.Foundations.Utils;

namespace Orion.Foundations.Extensions;

public static class RandomExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Shuffle<T>(this T[] array) => BuiltInRng.Generator.Shuffle(array);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Shuffle<T>(this List<T> list) => BuiltInRng.Generator.Shuffle(CollectionsMarshal.AsSpan(list));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Shuffle<T>(this Span<T> span) => BuiltInRng.Generator.Shuffle(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Shuffle<T>(this PooledRefList<T> list) =>
        BuiltInRng.Generator.Shuffle(list._items.AsSpan(0, list.Count));

    /**
     * Gets a random sample from the source list.
     * Not meant for unbounded lists. Does not shuffle or modify source.
     */
    public static T[] RandomSample<T>(this T[] source, int count)
    {
        if (count <= 0)
        {
            return Array.Empty<T>();
        }

        var length = source.Length;
        Span<bool> list = stackalloc bool[length];
        list.Clear();

        var sampleList = new T[count];

        var i = 0;
        do
        {
            var rand = RandomUtils.Random(length);
            if (!list[rand])
            {
                list[rand] = true;
                sampleList[i++] = source[rand];
            }
        } while (i < count);

        return sampleList;
    }

    public static List<T> RandomSample<T>(this List<T> source, int count)
    {
        if (count <= 0)
        {
            return [];
        }

        var length = source.Count;
        Span<bool> list = stackalloc bool[length];
        list.Clear();

        var sampleList = new List<T>(count);

        var i = 0;
        do
        {
            var rand = RandomUtils.Random(length);
            if (!list[rand])
            {
                list[rand] = true;
                sampleList[i++] = source[rand];
            }
        } while (i < count);

        return sampleList;
    }

    public static void RandomSample<T>(this T[] source, int count, List<T> dest)
    {
        if (count <= 0)
        {
            return;
        }

        var length = source.Length;
        Span<bool> list = stackalloc bool[length];
        list.Clear();

        var i = 0;
        do
        {
            var rand = RandomUtils.Random(length);
            if (!list[rand])
            {
                list[rand] = true;
                dest.Add(source[rand]);
            }
        } while (++i < count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RandomList<T>(params ReadOnlySpan<T> list) => list.RandomElement();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RandomElement<T>(this ReadOnlySpan<T> list) =>
        list.Length == 0 ? default : list[RandomUtils.Random(list.Length)];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RandomElement<T>(this T[] list) => list.RandomElement();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RandomElement<T>(this IList<T> list) => list.RandomElement();

    /// <summary>
    ///     Random pink, blue, green, orange, red or yellow hue
    /// </summary>
    public static int RandomNondyedHue()
    {
        return RandomUtils.Random(6) switch
        {
            0 => RandomPinkHue(),
            1 => RandomBlueHue(),
            2 => RandomGreenHue(),
            3 => RandomOrangeHue(),
            4 => RandomRedHue(),
            5 => RandomYellowHue(),
            _ => 0
        };
    }

    /// <summary>
    ///     Random hue in the range 1201-1254
    /// </summary>
    public static int RandomPinkHue() => RandomUtils.Random(1201, 54);

    /// <summary>
    ///     Random hue in the range 1301-1354
    /// </summary>
    public static int RandomBlueHue() => RandomUtils.Random(1301, 54);

    /// <summary>
    ///     Random hue in the range 1401-1454
    /// </summary>
    public static int RandomGreenHue() => RandomUtils.Random(1401, 54);

    /// <summary>
    ///     Random hue in the range 1501-1554
    /// </summary>
    public static int RandomOrangeHue() => RandomUtils.Random(1501, 54);

    /// <summary>
    ///     Random hue in the range 1601-1654
    /// </summary>
    public static int RandomRedHue() => RandomUtils.Random(1601, 54);

    /// <summary>
    ///     Random hue in the range 1701-1754
    /// </summary>
    public static int RandomYellowHue() => RandomUtils.Random(1701, 54);

    /// <summary>
    ///     Random hue in the range 1801-1908
    /// </summary>
    public static int RandomNeutralHue() => RandomUtils.Random(1801, 108);

    /// <summary>
    ///     Random hue in the range 2001-2018
    /// </summary>
    public static int RandomSnakeHue() => RandomUtils.Random(2001, 18);

    /// <summary>
    ///     Random hue in the range 2101-2130
    /// </summary>
    public static int RandomBirdHue() => RandomUtils.Random(2101, 30);

    /// <summary>
    ///     Random hue in the range 2201-2224
    /// </summary>
    public static int RandomSlimeHue() => RandomUtils.Random(2201, 24);

    /// <summary>
    ///     Random hue in the range 2301-2318
    /// </summary>
    public static int RandomAnimalHue() => RandomUtils.Random(2301, 18);

    /// <summary>
    ///     Random hue in the range 2401-2430
    /// </summary>
    public static int RandomMetalHue() => RandomUtils.Random(2401, 30);

    public static int ClipDyedHue(int hue) => hue < 2 ? 2 :
        hue > 1001 ? 1001 : hue;

    /// <summary>
    ///     Random hue in the range 2-1001
    /// </summary>
    public static int RandomDyedHue() => RandomUtils.Random(2, 1000);

    /// <summary>
    ///     Random hue from 0x62, 0x71, 0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59
    /// </summary>
    public static int RandomBrightHue() =>
        RandomUtils.RandomDouble() < 0.1
            ? RandomList(0x62, 0x71)
            : RandomList(0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59);
}
