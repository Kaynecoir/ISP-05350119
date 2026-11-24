using ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    static int countShooters = 10000;
    static async Task Main(string[] args)   
    {
        // Вариант 19: соревнования по стрельбе.
        // Статистика — спортсмены с суммой баллов > 80.
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        int mainTid = Thread.CurrentThread.ManagedThreadId;
        Console.WriteLine($"Поток {mainTid:D10}: Начало работы Main");

        // countShooters (1000) объектов (10 выстрелов при разбросе 0..10 их 0 на 1000;
        // диапазон 4..10 очков подобран, чтобы спортсменов с суммой > 80 было ~70)
        var rnd = Random.Shared;
        var shooters = new List<Shooter>(countShooters);
        for (int i = 1; i <= countShooters; i++)
        {
            var scores = new List<int>(10);
            for (int s = 0; s < 10; s++) scores.Add(rnd.Next(4, 11));
            shooters.Add(new Shooter { Id = i, Name = $"Спортсмен_{i}", Scores = scores });
        }

        var service = new StreamService<Shooter>();
        var memoryStream = new MemoryStream();

        var progress = new Progress<string>(Console.WriteLine);

        var task1 = Task.Run(() => service.WriteToStreamAsync(memoryStream, shooters, progress));
        await Task.Delay(150);
        var task2 = Task.Run(() => service.CopyFromStreamAsync(memoryStream, "shooters.json", progress));

        Console.WriteLine($"Поток {mainTid:D10}: Методы 1 и 2 запущены (await ещё не выполнен)");

        await Task.WhenAll(task1, task2);
        Console.WriteLine($"Поток {mainTid:D10}: Методы 1 и 2 завершены");

        int over80 = await service.GetStatisticsAsync("shooters.json", e => e.TotalScore >= 80);
        Console.WriteLine($"Поток {mainTid:D10}: Спортсменов с суммой баллов >= 80: {over80} из {countShooters}");

        memoryStream.Dispose();
    }
}