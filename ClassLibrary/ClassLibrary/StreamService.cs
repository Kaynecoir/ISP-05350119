using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary;

public sealed class StreamService<T> where T : new()
{
    // (Task.Delay на Windows дискретен ~15 мс, поэтому задержка идёт ПАЧКАМИ, а не по каждой строке — иначе получится 1000 задержек ≈ 15 с.)
    // 1000 объектов пачками по 10 -> 100 задержек. 100 * 35 мс ≈ 3.5 с.
    private const int BatchSize = 10;
    private const int BatchDelayMs = 35;

    // СИНХРОНИЗАЦИЯ методов 1 и 2.
    // метод 2 (копирование) обязан ждать Release() из метода 1 (конец записи).
    private readonly SemaphoreSlim _writeFinished = new(0);

    public async Task WriteToStreamAsync(Stream stream, IEnumerable<T> data, IProgress<string> progress)
    {
        int thId = Thread.CurrentThread.ManagedThreadId;
        progress.Report($"Поток {thId:D10}: [МЕТОД 1] вход в метод записи");

        // Начало записи
        progress.Report($"Поток {thId:D10}: [МЕТОД 1] начало записи коллекции");

        long count = 0;
        foreach (var item in data)
        {
            count++;
            string line = System.Text.Json.JsonSerializer.Serialize(item) + "\n";
            var bytes = System.Text.Encoding.UTF8.GetBytes(line);

            await stream.WriteAsync(bytes);           // асинхронная запись
            if (count % BatchSize == 0)
                await Task.Delay(BatchDelayMs);       // медленная запись

            // прогресс раз в пачку
            if (count % BatchSize == 0)
                progress.Report($"Поток {thId:D10}: [МЕТОД 1] идёт запись... объектов: {count}");
        }
        await stream.FlushAsync();

        progress.Report($"Поток {thId:D10}: [МЕТОД 1] завершение записи. Всего: {count}");
        _writeFinished.Release();  // разрешаем методу 2 прочитать поток
    }

    public async Task CopyFromStreamAsync(Stream stream, string filename, IProgress<string> progress)
    {
        int tid = Thread.CurrentThread.ManagedThreadId;
        progress.Report($"Поток {tid:D10}: [МЕТОД 2] вход в метод копирования");

        progress.Report($"Поток {tid:D10}: [МЕТОД 2] ожидание окончания записи...");
        await _writeFinished.WaitAsync();

        progress.Report($"Поток {tid:D10}: [МЕТОД 2] начало чтения из потока");

        stream.Position = 0; 

        using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write,
                                       FileShare.Read, 4096, useAsync: true))
        {
            await stream.CopyToAsync(fs);   // асинхронное копирование поток->файл
            await fs.FlushAsync();
        }

        progress.Report($"Поток {tid:D10}: [МЕТОД 2] завершение копирования в файл '{filename}'");
        _writeFinished.Release();           // на случай повторного вызова — чтобы не зависло
    }

    public async Task<int> GetStatisticsAsync(string fileName, Func<T, bool> filter)
    {
        // асинхронное чтение файла в память и разбор построчного JSON
        var bytes = await File.ReadAllBytesAsync(fileName);
        using var ms = new MemoryStream(bytes);

        int total = 0, hit = 0;
        using (var reader = new StreamReader(ms))
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                if (line.Trim().Length == 0) continue;
                var obj = System.Text.Json.JsonSerializer.Deserialize<T>(line)!;
                total++;
                if (filter(obj)) hit++;
            }
        }
        return hit;
    }
}
