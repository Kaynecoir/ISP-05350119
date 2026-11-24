using System;
using System.Threading;
using ClassLibrary;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Ускоренные параметры для демонстрации
double demoStep = 0.000_01;
long demoDelay = 1_000;
int countThread = 7;

static string Bar(int p, int width = 20)
{
    int filled = p * width / 100;
    return "[" + new string('=', filled) + new string(' ', width - filled) + "]";
}

void ProgressEvent(IntegralProgressArgs args, int width = 20)
{
    //Console.SetCursorPosition(0, consTop + 2 * (args.ThreadId - 10));
    Console.WriteLine($"Поток {args.ThreadId:D10}: {Bar(args.Percent, width)} {args.Percent}%");
}
void FinishedEvent(IntegralFinishedArgs args)
{
    //Console.SetCursorPosition(args.PositionLeft, args.PositionTop);
    Console.WriteLine($"Поток {args.ThreadId:D10}: Завершён с результатом: {args.Result:F15}  время: {args.ElapsedTicks} тик.\n");
}

double x1, x2;
Console.Write("Enter x1: ");
x1 = Convert.ToDouble(Console.ReadLine());
Console.Write("Enter x2: ");
x2 = Convert.ToDouble(Console.ReadLine());

// Выполнение одного потока с прогрессом и змером времени
Console.WriteLine("\n=== 4.1 Одиночный поток ===");
var one = new IntegralCalculator(1);   // ограничение в один поток
one.Step = demoStep; one.DelayOps = demoDelay;
one.Progress += (_, e) => ProgressEvent(e, 10);
one.IntegralFinished += (_, e) => FinishedEvent(e);
var t = new Thread(() => one.Sin(x1, x2)) { Name = "Calc" };
t.Start();
t.Join();


// Выполняет два потока с приоритетами Highest и Lowest
Console.WriteLine("\n=== 4.2 Приоритеты Highest/Lowest ===\n");
var two = new IntegralCalculator(int.MaxValue); // без ограничений по количеству потоков
two.Step = demoStep; two.DelayOps = demoDelay;
two.Progress += (_, e) => ProgressEvent(e, 100);
two.IntegralFinished += (_, e) => FinishedEvent(e);

var hi = new Thread(() => two.Sin(x1, x2));
var lo = new Thread(() => two.Sin(x1, x2));
hi.Priority = ThreadPriority.Highest;
lo.Priority = ThreadPriority.Lowest;
hi.Start(); lo.Start();
hi.Join(); lo.Join();


// Выполняет ТОЛЬКО ОДИН поток выполняет
Console.WriteLine($"\n=== 5 Ограничение: 1 поток одновременно (запуск {countThread}) ===\n");
var gate1 = new IntegralCalculator(1);
gate1.Step = demoStep; gate1.DelayOps = demoDelay;
//gate1.Progress += (_, e) => ProgressEvent(e, 100);
gate1.IntegralFinished += (_, e) =>
{
    FinishedEvent(e);
};
for (int i = 0; i < countThread; i++)
{
    var tt = new Thread(() => gate1.Sin(x1, x2));
    tt.Start(); tt.Join();
}


// Выполняет N потоков одновременно 
Console.WriteLine($"\n=== 6 Ограничение: 3 потока одновременно (запуск {countThread}) ===\n");
int n = 3;
var gateN = new IntegralCalculator(n);
gateN.Step = demoStep; gateN.DelayOps = demoDelay;
gateN.Progress += (_, e) => ProgressEvent(e, 100);
gateN.IntegralFinished += (_, e) =>
{
    FinishedEvent(e);
};
Thread[] arr = new Thread[countThread];
for (int i = 0; i < countThread; i++) arr[i] = new Thread(() => gateN.Sin(x1, x2));
foreach (var tt in arr) tt.Start();
foreach (var tt in arr) tt.Join();

Console.WriteLine("\nЗадание 1 выполнено.");

// ------------------------------