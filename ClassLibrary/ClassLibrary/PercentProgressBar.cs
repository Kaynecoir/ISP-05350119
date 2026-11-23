using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ClassLibrary
{
    public class PercentProgressBar
    {
        public delegate void ProgressDelegate();
        public event ProgressDelegate? Percenting;
        public (int, int) BarPosition;
        public string? Name;
        public long FullWork; // 100.00%
        public long OnePercentOfWork; // 1%
        public int PercentWork; 
        private long TotalWork;
        public PercentProgressBar()
        {
            BarPosition = (Console.CursorLeft, Console.CursorTop);
            Name = "";
            FullWork = 0;
            OnePercentOfWork = 0;
            TotalWork = 0;
            PercentWork = 0;
            Percenting += ChangePercent;
        }
        public PercentProgressBar(string name, long fullWork)
        {
            BarPosition = (Console.CursorLeft, Console.CursorTop);
            Name = name;
            FullWork = fullWork;
            OnePercentOfWork = fullWork / 100;
            TotalWork = 0;
            PercentWork = 0;
            Percenting += ChangePercent;
        }
        public double GetTotalWork()
        {
            return 0;
        }
        public void SetWork(long work)
        {
            TotalWork = work;
            Percenting?.Invoke();
        }
        public void TotalWorkUp(int up = 1)
        {
            TotalWork += up;
            if (TotalWork % OnePercentOfWork == 0) Percenting?.Invoke();
        }
        public void GetProgressBar()
        {
            Console.SetCursorPosition(BarPosition.Item1, BarPosition.Item2);
            Console.Write($"{Name} [");
            for (int i = 0; i < 100; i++)
            {
                if (i < PercentWork) Console.Write("=");
                else if (i == PercentWork) Console.Write(">");
                else Console.Write(" ");
            }
            Console.WriteLine($"] {PercentWork}%");
        }

        public void ChangePercent()
        {
            PercentWork = (int)(TotalWork / OnePercentOfWork);
        }
    }
}
