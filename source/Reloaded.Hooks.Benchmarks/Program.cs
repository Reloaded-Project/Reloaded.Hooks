using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;

namespace Reloaded.Hooks.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var dotnetCli32 = NetCoreAppSettings.NetCoreApp60.WithCustomDotNetCliPath(@"C:\Program Files (x86)\dotnet\dotnet.exe", "32 bit cli");
            var dotnetCli64 = NetCoreAppSettings.NetCoreApp60.WithCustomDotNetCliPath(@"C:\Program Files\dotnet\dotnet.exe", "64 bit cli");

            var job32   = Job.Default.WithPlatform(Platform.X86).WithToolchain(CsProjCoreToolchain.From(dotnetCli32));
            var job64   = Job.Default.WithPlatform(Platform.X64).WithToolchain(CsProjCoreToolchain.From(dotnetCli64));

            BenchmarkRunner.Run<HookBenchmark32<NativeCalculator86>>(ManualConfig.Create(DefaultConfig.Instance).AddJob(job32));
            BenchmarkRunner.Run<HookBenchmark<NativeCalculator64>>(ManualConfig.Create(DefaultConfig.Instance).AddJob(job64));
        }
    }
}
