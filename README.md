
# dotnetperf-benchmarks

Live at https://www.jpcode.dev/dotnetperf

## Creating a new benchmark

Install the template (one-time):

```bash
dotnet new install ./templates/bench
```

Scaffold a new benchmark:

```bash
# Simple benchmark
dotnet new bench -n Bench_MyThing

# With [Params] and [GlobalSetup]
dotnet new bench -n Bench_MyThing --parameterized
```

Then add the new project to the solution:

```bash
dotnet sln add Bench_MyThing
```
