using ClassLibrary;
using System.Diagnostics;

class Reader
{
    // создаем семафор
    static Semaphore sem = new Semaphore(3, 3);
    Thread myThread;
    int count = 0;// счетчик чтения

    public Reader(int i)
    {
        myThread = new Thread(Read);
        myThread.Name = $"Читатель {i}";
        myThread.Start();
    }

    public void Read()
    {
        while (count < 3)
        {
            count++;
            sem.WaitOne();  // ожидаем, когда освободиться место

            Console.WriteLine($"{Thread.CurrentThread.Name} входит в библиотеку {count} раз");

            Thread.Sleep(1000);

            Console.WriteLine($"{Thread.CurrentThread.Name} покидает библиотеку");

            sem.Release();  // освобождаем место

            Thread.Sleep(1000);
        }
    }
}

internal class Program
{
    
    private static void Main(string[] args)
    {
        //-----------------------------------------------------------------------
        double x;
        Console.WriteLine("Enter x");
        x = Convert.ToDouble(Console.ReadLine());
        IntegralCalculator IC = new IntegralCalculator(x, x+1);
        IC.Notification += delegate (string? message) { Console.WriteLine(message + "\n"); };
        IC.Percenting += delegate (long p)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop-1);
            Console.Write("[");
            for(int i = 0; i < 100; i++)
            {
                if (i < p / 100) Console.Write("#");
                else Console.Write(" ");
            }
            Console.WriteLine($"] {Convert.ToDouble(p) / 100}%");
        };
        Thread th = new Thread(IC.Sin);
        IC.thread = th;
        Console.WriteLine(IC.thread.ManagedThreadId);
        IC.Sin();

        //-----------------------------------------------------------------------
    }
}