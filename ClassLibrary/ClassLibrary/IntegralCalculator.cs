using System.Diagnostics;

namespace ClassLibrary
{
    public class IntegralCalculator
    {
        public Thread? thread;
        public PercentProgressBar ProgressBar;
        public delegate void IntegralCalculatorDelegate(string? message);
        public event IntegralCalculatorDelegate? Notification;
        public delegate void ProgressDelegate(Thread? th, long p);
        public event ProgressDelegate? Percenting;

        double Step = 0.000_000_01;
        double left_x, right_x, result;
        public IntegralCalculator(double x1, double x2)
        {
            left_x = x1;
            right_x = x2;
            result = 0;
            ProgressBar = new PercentProgressBar();
        }

        public void Sin()
        {
            // Начало отсчета времени работы метода
            Stopwatch sw = Stopwatch.StartNew();
            Notification?.Invoke("Начало подсчёта интеграла sin");

            // Переменные для работы с процентным прогрессом
            ProgressBar.BarPosition = (Console.CursorLeft, Console.CursorTop);
            ProgressBar.FullWork = Convert.ToInt64((right_x - left_x) / Step);
            ProgressBar.OnePercentOfWork = ProgressBar.FullWork / 100;
            ProgressBar.Name = thread?.Name;

            // Начало подсчета синуса
            double sin = 0;
            for (double x = left_x; x <= right_x; x += Step)
            {
                // Увеличение времени работы метода
                for (int i = 0; i < 100; i++)
                {
                    int t = i * i;
                }
                //Thread.Sleep(10);

                // Основная работа метода
                double s = Math.Sin(x);
                //Console.WriteLine(s);
                sin += s;
                ProgressBar.TotalWorkUp();
                //if(doneWork % onePercentOfWork == 0) Percenting?.Invoke(thread, doneWork / onePercentOfWork);
                
            }
            sin *= Step;
            Console.WriteLine(sin);

            // Остановка отсчета времени
            sw.Stop();
            Notification?.Invoke($"Конец подсчёта интеграла sin за: {sw.ElapsedTicks} ticks или {sw.ElapsedMilliseconds} мс");

            result = sin;
        }
    }
}
