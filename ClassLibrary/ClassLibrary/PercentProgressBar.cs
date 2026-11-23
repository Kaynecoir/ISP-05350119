using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    internal class PercentProgressBar
    {
        public (int, int) BarPosition;
        public string Name;
        public long FullWork; // 100.00%
        public long OnePercentOfWork;
        public double PercentWork; // 0.01%
        private int TotalWork;
        public PercentProgressBar(string name, long fullWork)
        {
            BarPosition = (Console.CursorLeft, Console.CursorTop);
            Name = name;
            FullWork = fullWork;
            OnePercentOfWork = fullWork / 100_00;
            TotalWork = 0;
            PercentWork = 0;
        }
        public double GetTotalWork()
        {
            return 0;
        }
        public void SetWork(long work)
        {
            PercentWork = work / TotalWork / 100.0;
        }
        public void TotalWorkUp(int up = 1)
        {
            TotalWork += up;
            if (TotalWork % OnePercentOfWork == 0) PercentWork++;
        }
        public void GetProgressBar()
        {
            Console.SetCursorPosition(BarPosition.Item1, BarPosition.Item2);
            Console.Write($"{Name} [");
            for (int i = 0; i < 100; i++)
            {
                if (i < TotalWork) Console.Write("#");
                else Console.Write(" ");
            }
            Console.WriteLine($"] {TotalWork}%");
        }
    }
}
