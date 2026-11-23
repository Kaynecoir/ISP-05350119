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
        double x1, x2;
        Console.Write("Enter x1: ");
        x1 = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter x2: ");
        x2 = Convert.ToDouble(Console.ReadLine());

        IntegralCalculator IC = new IntegralCalculator(x1, x2);
        IC.Notification += delegate (string? message) { Console.WriteLine(message + "\n"); };
        Thread th = new Thread(IC.Sin);
        th.Name = "Поток 1";
        IC.thread = th;
        IC.ProgressBar.Percenting += IC.ProgressBar.GetProgressBar;
        th.Start();

        //-----------------------------------------------------------------------
    }
}