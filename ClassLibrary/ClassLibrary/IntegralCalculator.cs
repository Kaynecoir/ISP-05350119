using System;
using System.Diagnostics;
using System.Threading;

namespace ClassLibrary;

// 1) Аргументы для событий (пункт 2,3)
public sealed class IntegralProgressArgs : EventArgs
{
    public int ThreadId { get; init; }
    public int Percent { get; init; }
    public long Iteration { get; init; }
    public long TotalIterations { get; init; }
    public int PositionLeft { get; init; }
    public int PositionTop { get; init; }
}

public sealed class IntegralFinishedArgs : EventArgs
{
    public int ThreadId { get; init; }
    public double Result { get; init; }
    public long ElapsedTicks { get; init; }  
    public long TotalIterations { get; init; }
    public int PositionLeft { get; init; }
    public int PositionTop { get; init; }
}

public sealed class IntegralCalculator
{
    public double Step { get; set; } = 0.000_000_01;   
    public long DelayOps { get; set; } = 100_000;   

    private readonly SemaphoreSlim _gate;

    public IntegralCalculator(int maxConcurrent = int.MaxValue)
        => _gate = new SemaphoreSlim(maxConcurrent, maxConcurrent);

    public double Sin(double a, double b)
    {
        _gate.Wait();
        try
        {
            int tid = Thread.CurrentThread.ManagedThreadId;
            long fullWork = (long)Math.Ceiling((b - a) / Step);
            long doneWork = 0;
            double sin = 0;
            double sink = 0;       

            var sw = Stopwatch.StartNew();

            for (double x = a; x < b; x += Step)
            {
                double s = Math.Sin(x);
                sin += s;


                for (long i = 0; i < DelayOps; i++)
                    sink = sink * 1.0000000000001 + 1.0;

                doneWork++;


                if ((doneWork * 100) % fullWork == 0)
                    Progress?.Invoke(this, new IntegralProgressArgs
                    {
                        ThreadId = tid,
                        Percent = (int)(doneWork * 100 / fullWork),
                        Iteration = doneWork,
                        TotalIterations = fullWork
                    });
            }
            sin *= Step;

            if (sink < 0) throw new Exception(); // чтобы цикл не выбросили как мёртвый код


            IntegralFinished?.Invoke(this, new IntegralFinishedArgs
            {
                ThreadId = tid,
                Result = sin,
                ElapsedTicks = sw.ElapsedTicks,
                TotalIterations = fullWork
            });
            return sin;
        }
        finally { _gate.Release(); }
    }

    private static int PercentOf(long done, long total) => (int)(done * 100 / total);

    public event EventHandler<IntegralProgressArgs>? Progress;
    public event EventHandler<IntegralFinishedArgs>? IntegralFinished;
}