using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary;


public class Shooter
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // 10 выстрелов, каждый 0..10 очков.
    public List<int> Scores { get; set; } = new();

    // Сумма баллов — производное свойство (сериализуется, но при чтении пересчитывается из Scores).
    public int TotalScore
    {
        get
        {
            int sum = 0;
            foreach (var s in Scores) sum += s;
            return sum;
        }
    }

    public override string ToString() => $"{Id} {Name,-18} баллов: {TotalScore}";
}
