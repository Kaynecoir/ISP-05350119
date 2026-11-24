using System.Diagnostics;

namespace ClassLibrary
{
    public class IntegralCalculator
    {
        public Thread? thread;
        //public PercentProgressBar ProgressBar;
        public delegate void IntegralCalculatorDelegate(string? message);
        public event IntegralCalculatorDelegate? Notification;
        public delegate void ProgressDelegate();
        public event ProgressDelegate? Percenting;

        // Переменные для прогресс бара
        public int BarPositionLeft;
        public int BarPositionTop;
        public string? Name;
        public long FullWork; // 100.00%
        public long OnePercentOfWork; // 1%
        public int PercentWork;
        private long TotalWork;

        // Переменные для интеграла
        double Step = 0.000_000_01;
        double left_x, right_x, result;
        public IntegralCalculator(double x1, double x2)
        {
            left_x = x1;
            right_x = x2;
            result = 0;
            //ProgressBar = new PercentProgressBar();
            BarPositionLeft = 0;
            BarPositionTop = 0;
            Name = "";
            FullWork = 100;
            OnePercentOfWork = 1;
            TotalWork = 0;
            PercentWork = 0;
            Percenting += ChangePercent;
            Percenting += GetProgressBar;
        }

        public void Sin()
        {
            // Начало отсчета времени работы метода
            Stopwatch sw = Stopwatch.StartNew();
            Notification?.Invoke("Начало подсчёта интеграла sin в " + thread?.Name);

            // Переменные для работы с процентным прогрессом
            FullWork = Convert.ToInt64((right_x - left_x) / Step);
            OnePercentOfWork = FullWork / 100;
            Name = thread?.Name;

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
                TotalWorkUp();
                //if(doneWork % onePercentOfWork == 0) Percenting?.Invoke(thread, doneWork / onePercentOfWork);
            }
            sin *= Step;
            Console.WriteLine(sin);

            // Остановка отсчета времени
            sw.Stop();
            Console.SetCursorPosition(BarPositionLeft, BarPositionTop+1);
            Notification?.Invoke($"Конец подсчёта интеграла sin за: {sw.ElapsedTicks} ticks или {sw.ElapsedMilliseconds} мс");

            result = sin;
        }
        public void TotalWorkUp(int up = 1)
        {
            TotalWork += up;
            if (TotalWork % OnePercentOfWork == 0) Percenting?.Invoke();
        }
        public void ChangePercent()
        {
            PercentWork = (int)(TotalWork / OnePercentOfWork);
        }
        public void GetProgressBar()
        {
            lock (new object())
            {
                Console.SetCursorPosition(BarPositionLeft, BarPositionTop);
                Console.Write($"{Name} [");
                for (int i = 0; i < 100; i++)
                {
                    if (i < PercentWork) Console.Write("=");
                    else if (i == PercentWork) Console.Write(">");
                    else Console.Write(" ");
                }
                Console.WriteLine($"] {PercentWork}%");
                Thread.Sleep(1);
            }
        }
    }
}
