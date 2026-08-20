using System.Collections.Frozen;
using Bench_TypeIfsVsDict;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// // * Detailed results *
// Bench.Ifs: DefaultJob [N=200]
// Runtime = .NET 10.0.10 (10.0.1026.32716), X64 RyuJIT AVX2; GC = Concurrent Workstation
// Mean = 2.471 us, StdErr = 0.016 us (0.65%), N = 98, StdDev = 0.160 us
// Min = 2.228 us, Q1 = 2.341 us, Median = 2.449 us, Q3 = 2.569 us, Max = 2.927 us
// IQR = 0.228 us, LowerFence = 2.000 us, UpperFence = 2.911 us
// ConfidenceInterval = [2.416 us; 2.525 us] (CI 99.9%), Margin = 0.055 us (2.21% of Mean)
// Skewness = 0.71, Kurtosis = 2.92, MValue = 2.13
// -------------------- Histogram --------------------
// [2.183 us ; 2.265 us) | @@@@
// [2.265 us ; 2.356 us) | @@@@@@@@@@@@@@@@@@@@@@@@@@
// [2.356 us ; 2.488 us) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// [2.488 us ; 2.585 us) | @@@@@@@@@@@@@@@@
// [2.585 us ; 2.683 us) | @@@@@@@
// [2.683 us ; 2.774 us) | @@@@@@@@@@@
// [2.774 us ; 2.837 us) |
// [2.837 us ; 2.928 us) | @@@
// ---------------------------------------------------
//
// Bench.Dict: DefaultJob [N=200]
// Runtime = .NET 10.0.10 (10.0.1026.32716), X64 RyuJIT AVX2; GC = Concurrent Workstation
// Mean = 3.420 us, StdErr = 0.032 us (0.93%), N = 99, StdDev = 0.318 us
// Min = 3.035 us, Q1 = 3.134 us, Median = 3.341 us, Q3 = 3.653 us, Max = 4.211 us
// IQR = 0.519 us, LowerFence = 2.354 us, UpperFence = 4.432 us
// ConfidenceInterval = [3.312 us; 3.528 us] (CI 99.9%), Margin = 0.108 us (3.17% of Mean)
// Skewness = 0.58, Kurtosis = 2.29, MValue = 2.83
// -------------------- Histogram --------------------
// [3.030 us ; 3.210 us) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// [3.210 us ; 3.402 us) | @@@@@@@@@@@@@@@@
// [3.402 us ; 3.474 us) | @@@@
// [3.474 us ; 3.654 us) | @@@@@@@@@@@@@@@@@@@
// [3.654 us ; 3.860 us) | @@@@@@@@@@@@@
// [3.860 us ; 4.088 us) | @@@@@@@@
// [4.088 us ; 4.301 us) | @@@
// ---------------------------------------------------
//
// // * Summary *
//
// BenchmarkDotNet v0.14.0, Arch Linux
// AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
// .NET SDK 10.0.302
//   [Host]     : .NET 10.0.10 (10.0.1026.32716), X64 RyuJIT AVX2
//   DefaultJob : .NET 10.0.10 (10.0.1026.32716), X64 RyuJIT AVX2
//
//
// | Method | N   | Mean     | Error     | StdDev    | Gen0   | Allocated |
// |------- |---- |---------:|----------:|----------:|-------:|----------:|
// | Ifs    | 200 | 2.471 us | 0.0547 us | 0.1596 us | 0.0763 |     640 B |
// | Dict   | 200 | 3.420 us | 0.1083 us | 0.3175 us | 0.0763 |     640 B |
//
// // * Warnings *
// MultimodalDistribution
//   Bench.Dict: Default -> It seems that the distribution can have several modes (mValue = 2.83)
//
// // * Hints *
// Outliers
//   Bench.Ifs: Default  -> 2 outliers were removed (3.04 us, 3.40 us)
//   Bench.Dict: Default -> 1 outlier  was  removed (4.70 us)
//
// // * Legends *
//   N         : Value of the 'N' parameter
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Gen0      : GC Generation 0 collects per 1000 operations
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//   1 us      : 1 Microsecond (0.000001 sec)
//
// // * Diagnostic Output - MemoryDiagnoser *
//
//
// // ***** BenchmarkRunner: End *****
// Run time: 00:02:54 (174.19 sec), executed benchmarks: 2
//
// Global total time: 00:02:59 (179.76 sec), executed benchmarks: 2
// // * Artifacts cleanup *
// Artifacts cleanup is finished

BenchmarkRunner.Run<Bench>();

namespace Bench_TypeIfsVsDict
{
    [MemoryDiagnoser]
    public class Bench
    {
        private static readonly Type[] AllTypes =
        [
            typeof(bool),
            typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal),
            typeof(string),
            typeof(Guid), typeof(DateTime), typeof(DateTimeOffset),
            typeof(DateOnly), typeof(TimeOnly), typeof(TimeSpan),
            typeof(int?), typeof(bool?), typeof(DateTime?),
            typeof(DayOfWeek),
        ];

        // Pre-built random sample so both benchmarks hit the same sequence.
        private Type[] _sample = [];

        [Params(200)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var rng = new Random(42);
            _sample = new Type[N];
            for (int i = 0; i < N; i++)
                _sample[i] = AllTypes[rng.Next(AllTypes.Length)];
        }

        [Benchmark]
        public string Ifs()
        {
            string last = "";
            foreach (var t in _sample)
                last = SqlCastFor_Ifs(t);
            return last;
        }

        [Benchmark]
        public string Dict()
        {
            string last = "";
            foreach (var t in _sample)
                last = SqlCastFor_Dict(t);
            return last;
        }

        [Benchmark]
        public string ListFind()
        {
            string last = "";
            foreach (var t in _sample)
                last = SqlCastFor_ListFind(t);
            return last;
        }

        private static string SqlCastFor_Ifs(Type propType)
        {
            var t = Nullable.GetUnderlyingType(propType) ?? propType;

            if (t == typeof(bool))
                return "::BOOL";

            // Signed ints: cast to the matching signed width.
            if (t == typeof(sbyte) || t == typeof(short))
                return "::INT2";
            if (t == typeof(int))
                return "::INT4";
            if (t == typeof(long))
                return "::INT8";

            // Unsigned ints: postgres has no unsigned types, so promote one
            // width so the full CLR range fits. ulong exceeds INT8 -> NUMERIC.
            if (t == typeof(byte))
                return "::INT2";
            if (t == typeof(ushort))
                return "::INT4";
            if (t == typeof(uint))
                return "::INT8";
            if (t == typeof(ulong))
                return "::NUMERIC";

            // Binary floats -> matching binary width
            if (t == typeof(float))
                return "::FLOAT4";
            if (t == typeof(double))
                return "::FLOAT8";

            if (t == typeof(decimal))
                return "::NUMERIC";

            // already text.
            if (t == typeof(string))
                return "";

            // No direct jsonb cast exists for these; exit to text, the canonical
            // parse input, and name the matching pg type.
            if (t == typeof(Guid))
                return "::UUID";
            if (t == typeof(DateTime))
                return "::TIMESTAMP";
            if (t == typeof(DateTimeOffset))
                return "::TIMESTAMPTZ";
            if (t == typeof(DateOnly))
                return "::DATE";
            if (t == typeof(TimeOnly) || t == typeof(TimeSpan))
                return "::TIME";

            // Enum storage (int vs string) depends on the EF value converter,
            // invisible here; bare '->>' text is safe at the pg level against
            // either.
            if (t.IsEnum)
                return "";

            throw new InvalidOperationException($"Unhandled type: {t.FullName}");
        }

        private static readonly FrozenDictionary<Type, string> CastMap = new Dictionary<Type, string>
        {
            [typeof(bool)]           = "::BOOL",
            [typeof(sbyte)]          = "::INT2",
            [typeof(short)]          = "::INT2",
            [typeof(int)]            = "::INT4",
            [typeof(long)]           = "::INT8",
            [typeof(byte)]           = "::INT2",
            [typeof(ushort)]         = "::INT4",
            [typeof(uint)]           = "::INT8",
            [typeof(ulong)]          = "::NUMERIC",
            [typeof(float)]          = "::FLOAT4",
            [typeof(double)]         = "::FLOAT8",
            [typeof(decimal)]        = "::NUMERIC",
            [typeof(string)]         = "",
            [typeof(Guid)]           = "::UUID",
            [typeof(DateTime)]       = "::TIMESTAMP",
            [typeof(DateTimeOffset)] = "::TIMESTAMPTZ",
            [typeof(DateOnly)]       = "::DATE",
            [typeof(TimeOnly)]       = "::TIME",
            [typeof(TimeSpan)]       = "::TIME",
        }.ToFrozenDictionary();

        private static readonly List<(Type Type, string Cast)> CastList =
        [
            (typeof(bool),           "::BOOL"),
            (typeof(sbyte),          "::INT2"),
            (typeof(short),          "::INT2"),
            (typeof(int),            "::INT4"),
            (typeof(long),           "::INT8"),
            (typeof(byte),           "::INT2"),
            (typeof(ushort),         "::INT4"),
            (typeof(uint),           "::INT8"),
            (typeof(ulong),          "::NUMERIC"),
            (typeof(float),          "::FLOAT4"),
            (typeof(double),         "::FLOAT8"),
            (typeof(decimal),        "::NUMERIC"),
            (typeof(string),         ""),
            (typeof(Guid),           "::UUID"),
            (typeof(DateTime),       "::TIMESTAMP"),
            (typeof(DateTimeOffset), "::TIMESTAMPTZ"),
            (typeof(DateOnly),       "::DATE"),
            (typeof(TimeOnly),       "::TIME"),
            (typeof(TimeSpan),       "::TIME"),
        ];

        private static string SqlCastFor_ListFind(Type propType)
        {
            var t = Nullable.GetUnderlyingType(propType) ?? propType;

            var entry = CastList.Find(e => e.Type == t);
            if (entry.Type is not null)
                return entry.Cast;

            if (t.IsEnum)
                return "";

            throw new InvalidOperationException($"Unhandled type: {t.FullName}");
        }

        private static string SqlCastFor_Dict(Type propType)
        {
            var t = Nullable.GetUnderlyingType(propType) ?? propType;

            if (CastMap.TryGetValue(t, out var cast))
                return cast;

            if (t.IsEnum)
                return "";

            throw new InvalidOperationException($"Unhandled type: {t.FullName}");
        }
    }
}
