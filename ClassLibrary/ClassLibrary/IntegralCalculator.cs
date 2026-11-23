using System.Diagnostics;

namespace ClassLibrary
{
    public class IntegralCalculator
    {
        public Thread? thread;
        public delegate void IntegralCalculatorDelegate(string? message);
        public event IntegralCalculatorDelegate? Notification;
        public delegate void ProgressDelegate(long p);
        public event ProgressDelegate? Percenting;

        double Step = 0.000_000_01;
        double left_x, right_x, result;
        public IntegralCalculator(double x1, double x2)
        {
            left_x = x1;
            right_x = x2;
            result = 0;
        }

        public void Sin()
        {
            // Начало отсчета времени работы метода
            Stopwatch sw = Stopwatch.StartNew();
            Notification?.Invoke("Начало подсчёта интеграла sin");

            // Переменные для работы с процентным прогрессом
            long fullWork = Convert.ToInt64((right_x - left_x) / Step);
            long percentWork = fullWork / 10000, doneWork = 0;

            // Начало подсчета синуса
            double sin = 0;
            for (double x = left_x; x <= right_x; x += Step)
            {
                // Увеличение времени работы метода
                for (int i = 0; i < 100_000; i++)
                {
                    int t = i * i;
                }
                //Thread.Sleep(10);

                // Основная работа метода
                double s = Math.Sin(x);
                //Console.WriteLine(s);
                sin += s;
                doneWork++;
                if(doneWork % percentWork == 0) Percenting?.Invoke(doneWork / percentWork);
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
