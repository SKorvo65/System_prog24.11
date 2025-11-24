
// Синхронизация В многопоточном программировании возникают проблемы, когда несколько потоков одновременно
// обращаются к общим данным

// Состояние гонки(Race Condtion) - результат выполнения зависит от порядка выполнения потоков
// Взаимная блокировка(Deadlock) -  потоки блокируют друг друга в циклическом ожидании
// Голодание(Starvation) -  поток не может получить доступ к ресурсу из-за постоянного приоритета других потоков
// Инверсия приоритетов - низкоприоритетный поток удерживает ресурс, нужный высокоприоритетному

/*
 * using System;
using System.Threading;

class Program
{
    // ОБЩИЙ РЕСУРС - потенциальный источник проблем
    private static int counter = 0;
    
    static void Main()
    {
        Console.WriteLine("Демонстрация проблемы состояния гонки (Race Condition)");
        Console.WriteLine("Ожидаемое значение counter: 200000");
        Console.WriteLine("Реальное значение может быть меньше из-за параллельного доступа\n");
        
        // Создаем два потока, которые будут одновременно увеличивать счетчик
        Thread thread1 = new Thread(IncrementCounter);
        Thread thread2 = new Thread(IncrementCounter);
        
        // Запускаем оба потока
        thread1.Start();
        thread2.Start();
        
        // Ждем завершения обоих потоков
        thread1.Join();
        thread2.Join();
        
        Console.WriteLine($"\nФинальное значение счетчика: {counter}");
        Console.WriteLine("Почему значение меньше ожидаемого?");
        Console.WriteLine("Операция counter++ НЕ является атомарной!");
        Console.WriteLine("Она состоит из трех шагов:");
        Console.WriteLine("1. Чтение значения из памяти в регистр процессора");
        Console.WriteLine("2. Увеличение значения в регистре");
        Console.WriteLine("3. Запись результата обратно в память");
        Console.WriteLine("Между этими шагами может вклиниться другой поток!");
    }
    
    static void IncrementCounter()
    {
        // КАЖДЫЙ поток увеличивает счетчик 100000 раз
        for (int i = 0; i < 100000; i++)
        {
            // ПРОБЛЕМА: эта операция не атомарна
            // Возможный сценарий:
            // Поток 1: читает counter (значение 5)
            // Поток 2: читает counter (значение 5)
            // Поток 1: увеличивает до 6
            // Поток 2: увеличивает до 6
            // Поток 1: записывает 6
            // Поток 2: записывает 6
            // РЕЗУЛЬТАТ: вместо 7 получаем 6 - потерянное обновление!
            counter++;
        }
        
        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} завершил работу");
    }
}
 * 
 */

// Мьютикс - подробное обяснение 
// это примитив синхронизации, который предоставляет эксклюзивный доступ к ресурсу.
// Только один поток может владеть мьютексом в любой момент времени.



/*
using System;
using System.Threading;

class MutexExample
{
    // Создаем мьютекс - объект синхронизации
    private static Mutex mutex = new Mutex();
    private static int sharedResource = 0;

    static void Main()
    {
        Console.WriteLine("=== Демонстрация работы МЬЮТЕКСА ===");
        Console.WriteLine("Мьютекс гарантирует, что только один поток может работать с общим ресурсом\n");

        // Создаем 5 потоков, которые будут конкурировать за доступ
        for (int i = 1; i <= 5; i++)
        {
            Thread thread = new Thread(IncrementWithMutex);
            thread.Start(i); // Передаем номер потока как параметр
        }

        Thread.Sleep(3000); // Даем время всем потокам завершить работу
        Console.WriteLine($"\nФинальное значение sharedResource: {sharedResource}");
    }

    static void IncrementWithMutex(object id)
    {
        Console.WriteLine($"Поток {id}: ожидает мьютекс...");

        // КРИТИЧЕСКИЙ МОМЕНТ: поток пытается захватить мьютекс
        // Если мьютекс уже захвачен другим потоком, текущий поток БЛОКИРУЕТСЯ
        // и ждет, пока мьютекс не будет освобожден
        mutex.WaitOne();

        try
        {
            Console.WriteLine($"Поток {id}: ЗАХВАТИЛ мьютекс! Начинает работу...");

            // КРИТИЧЕСКАЯ СЕКЦИЯ - только один поток может выполнять этот код
            // Сохраняем текущее значение
            int temp = sharedResource;
            Console.WriteLine($"Поток {id}: прочитал значение {temp}");

            // Имитируем работу (чтение/обработка/запись)
            Thread.Sleep(1000); // Задержка для наглядности

            // Увеличиваем значение
            sharedResource = temp + 1;

            Console.WriteLine($"Поток {id}: записал значение {sharedResource}");
            Console.WriteLine($"Поток {id}: завершил работу в критической секции");
        }
        finally
        {
            // ВАЖНО: всегда освобождаем мьютекс в блоке finally
            // Это гарантирует, что мьютекс будет освобожден даже при исключении
            mutex.ReleaseMutex();
            Console.WriteLine($"Поток {id}: ОСВОБОДИЛ мьютекс");
        }
    }
}
*/



// Семафоры(Semaphore) -
//  Семафор ограничивает количество потоков, которые могут одновременно обращаться к ресурсу.
//  В отличие от мьютекса, семафор может разрешать доступ нескольким потокам одновременно.
/*
using System;
using System.Threading; // 

class SemaphoreExample
{
    // Создаем семафор, который разрешает одновременный доступ 2 потокам
    // Первый параметр (2): начальное количество свободных слотов
    // Второй параметр (2): максимальное количество свободных слотов
    private static Semaphore semaphore = new Semaphore(2, 2);
    private static int sharedResource = 0;

    static void Main()
    {
        Console.WriteLine("=== Демонстрация работы СЕМАФОРА ===");
        Console.WriteLine("Семафор разрешает одновременный доступ 2 потокам\n");

        // Создаем 5 потоков, которые будут конкурировать за доступ
        // 1итерация: при начале работы 2 потока активны, 2 ожидание, 1 блокирован
        // 2итерация: 2 потока активны, 1 ожидания
        for (int i = 1; i <= 5; i++)
        {
            Thread thread = new Thread(AccessWithSemaphore);
            thread.Start(i);
        }

        Thread.Sleep(10000); // Даем время всем потокам завершить работу
        Console.WriteLine($"\nВсе потоки завершили работу");
    }

    static void AccessWithSemaphore(object id)
    {
        Console.WriteLine($"Поток {id}: подошел к семафору...");

        // Запрашиваем доступ к семафору
        // Если есть свободные слоты (меньше 2 активных потоков) - проходим
        // Если нет - БЛОКИРУЕМСЯ и ждем, пока какой-нибудь поток не освободит слот
        semaphore.WaitOne();

        try
        {
            Console.WriteLine($"Поток {id}: ПРОШЕЛ через семафор! Начинает работу...");
            Console.WriteLine($"    [Сейчас работают: какие-то потоки, максимум 2]");

            // Работа с ресурсом
            // Имитируем длительную операцию
            Thread.Sleep(2000);

            // Безопасно увеличиваем счетчик (в реальном коде нужна дополнительная синхронизация)
            Interlocked.Increment(ref sharedResource);

            Console.WriteLine($"Поток {id}: завершил работу. Ресурс обновлен.");
        }
        finally
        {
            // ОСВОБОЖДАЕМ слот в семафоре, позволяя другому потоку войти
            semaphore.Release();
            Console.WriteLine($"Поток {id}: освободил слот в семафоре");
        }
    }
}
*/

// События(Events)
// ManualResetEvent - это примитив синхронизации, который позволяет одному или нескольким потокам ждать сигнала от другого потока.
// После установки сигнала он остается установленным до ручного сброса.

/*
using System;
using System.Threading;

class ManualResetEventExample
{
    // Создаем событие в неподписанном состоянии (false)
    // false - событие не произошло, потоки ждут
    // true - событие произошло, потоки могут продолжить
    private static ManualResetEvent manualEvent = new ManualResetEvent(false);
    private static string data = null;

    static void Main()
    {
        Console.WriteLine("=== Демонстрация MANUAL RESET EVENT ===");
        Console.WriteLine("Производитель готовит данные, потребитель ждет сигнала\n");

        // Поток-потребитель - ждет данные
        Thread consumer = new Thread(ConsumeData);
        consumer.Start();

        // Даем потребителю время начать ожидание
        Thread.Sleep(1000);

        // Поток-производитель - готовит данные
        Thread producer = new Thread(ProduceData);
        producer.Start();

        // Ждем завершения обоих потоков
        producer.Join();
        consumer.Join();

        Console.WriteLine("\nОсновной поток: демонстрация завершена");
    }

    static void ProduceData()
    {
        Console.WriteLine("ПРОИЗВОДИТЕЛЬ: начинаю генерацию данных...");

        // Имитируем длительную операцию подготовки данных
        Thread.Sleep(3000);

        // Подготавливаем данные
        data = $"Важные данные, сгенерированные в {DateTime.Now:T}";

        Console.WriteLine("ПРОИЗВОДИТЕЛЬ: данные готовы!");
        Console.WriteLine($"ПРОИЗВОДИТЕЛЬ: отправляю сигнал потребителю...");

        // КРИТИЧЕСКИЙ МОМЕНТ: УСТАНАВЛИВАЕМ событие в состояние "сигнал"
        // Все потоки, которые ждали этого события, будут разбужены
        // И будут продолжать работу, пока событие не будет сброшено
        manualEvent.Set();

        Console.WriteLine("ПРОИЗВОДИТЕЛЬ: сигнал отправлен. Работа завершена.");
    }

    static void ConsumeData()
    {
        Console.WriteLine("ПОТРЕБИТЕЛЬ: жду сигнал от производителя...");

        // ЖДЕМ СИГНАЛА: поток блокируется здесь до тех пор,
        // пока производитель не вызовет manualEvent.Set()
        manualEvent.WaitOne();

        Console.WriteLine("ПОТРЕБИТЕЛЬ: получил сигнал! Обрабатываю данные...");
        Console.WriteLine($"ПОТРЕБИТЕЛЬ: обрабатываю данные: {data}");

        // ManualResetEvent остается в установленном состоянии
        // Если бы был другой потребитель, он бы сразу прошел без ожидания
        Console.WriteLine("ПОТРЕБИТЕЛЬ: работа завершена. Событие остается установленным.");
    }
}
*/

/*
// AutoResetEvent автоматически сбрасывается после пробуждения одного потока. Каждый вызов Set() пробуждает только один ожидающий поток.
using System;
using System.Threading;

class AutoResetEventExample
{
    // Создаем автоматически сбрасываемое событие
    private static AutoResetEvent autoEvent = new AutoResetEvent(false);

    static void Main()
    {
        Console.WriteLine("=== Демонстрация AUTO RESET EVENT ===");
        Console.WriteLine("Каждый Set() будит только один поток\n");

        // Создаем 3 рабочих потока
        for (int i = 1; i <= 3; i++)
        {
            Thread thread = new Thread(Worker);
            thread.Start(i);
        }

        Console.WriteLine("Основной поток: потоки созданы и ждут сигнала");
        Console.WriteLine("Отправляю 3 сигнала с интервалом 1.5 секунды...\n");

        // Посылаем сигналы потокам
        for (int i = 0; i < 3; i++)
        {
            Thread.Sleep(1500); // Ждем 1.5 секунды
            Console.WriteLine($"Основной поток: отправляю сигнал #{i + 1}");

            // КАЖДЫЙ вызов Set() будит ТОЛЬКО ОДИН поток
            // После этого событие автоматически сбрасывается в несигнальное состояние
            // 1,2,3 -> 1 -> 2 -> 3
            autoEvent.Set();
        }

        Thread.Sleep(1000);
        Console.WriteLine("\nВсе потоки завершили работу");
    }

    static void Worker(object id)
    {
        Console.WriteLine($"    Рабочий {id}: жду сигнала...");

        // ОЖИДАНИЕ СИГНАЛА: поток блокируется здесь
        // Когда придет сигнал (Set()), один поток просыпается,
        // и событие автоматически сбрасывается в false
        autoEvent.WaitOne();

        Console.WriteLine($"    Рабочий {id}: ПРОСНУЛСЯ! Выполняю работу...");
        Thread.Sleep(500); // Имитируем работу
        Console.WriteLine($"    Рабочий {id}: работа завершена");
    }
}
*/

// Класс Monitor - предоставляет механизм синхронизации доступа к объектам. Это более гибкая альтернатива lock с возможностью установки таймаутов
/*
using System;
using System.Threading;

class MonitorExample
{
    // Объект-заглушка для синхронизации
    // Monitor работает на основе объектов, а не примитивов как Mutex
    private static object lockObject = new object();
    private static int counter = 0;

    static void Main()
    {
        Console.WriteLine("=== Демонстрация MONITOR ===");
        Console.WriteLine("Monitor предоставляет более гибкое управление блокировками\n");

        Thread[] threads = new Thread[5];

        // Создаем и запускаем 5 потоков
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(SafeIncrement);
            threads[i].Start(i + 1);
        }

        // Ждем завершения всех потоков
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine($"\nФинальное значение счетчика: {counter}");
        Console.WriteLine("Ожидаемое значение: 5000 (5 потоков × 1000 итераций)");
    }

    static void SafeIncrement(object threadId)
    {
        for (int i = 0; i < 1000; i++)
        {
            bool lockTaken = false;

            try
            {
                // ПЫТАЕМСЯ ЗАХВАТИТЬ БЛОКИРОВКУ
                // ref lockTaken - передаем по ссылке, чтобы знать, удалось ли захватить блокировку
                Monitor.Enter(lockObject, ref lockTaken);

                // КРИТИЧЕСКАЯ СЕКЦИЯ - только один поток может выполнять этот код
                // Безопасно увеличиваем счетчик
                counter++;

                // Периодически выводим информацию для демонстрации
                if (i % 200 == 0)
                {
                    Console.WriteLine($"Поток {threadId}: итерация {i}, counter = {counter}");
                    Thread.Sleep(10); // Маленькая задержка для наглядности
                }
            }
            finally
            {
                // ВАЖНО: всегда освобождаем блокировку в finally
                // Это гарантирует освобождение даже при возникновении исключения
                if (lockTaken)
                {
                    Monitor.Exit(lockObject);
                }
            }

            // Короткая пауза между итерациями
            Thread.Sleep(1);
        }

        Console.WriteLine($"Поток {threadId} завершил все итерации");
    }
}
*/
//Ключевое слово lock - это синтаксический сахар для Monitor.Enter/Monitor.Exit.
//Оно автоматически создает блок try-finally для безопасного управления блокировкой.
/*
using System;
using System.Threading;

class LockExample
{
    // Объект-заглушка для синхронизации
    private static object lockObject = new object();
    private static int counter = 0;

    static void Main()
    {
        Console.WriteLine("=== Демонстрация ключевого слова LOCK ===");
        Console.WriteLine("LOCK - это удобная обертка вокруг Monitor.Enter/Exit\n");

        Thread[] threads = new Thread[5];

        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(IncrementWithLock);
            threads[i].Start(i + 1);
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine($"\nФинальное значение счетчика: {counter}");
        Console.WriteLine("Ожидаемое значение: 5000 (5 потоков × 1000 итераций)");
        Console.WriteLine("\nКОМПИЛЯТОР преобразует lock в следующий код:");
        Console.WriteLine("bool lockTaken = false;");
        Console.WriteLine("try {");
        Console.WriteLine("    Monitor.Enter(lockObject, ref lockTaken);");
        Console.WriteLine("    // ваш код");
        Console.WriteLine("} finally {");
        Console.WriteLine("    if (lockTaken) Monitor.Exit(lockObject);");
        Console.WriteLine("}");
    }

    static void IncrementWithLock(object threadId)
    {
        for (int i = 0; i < 1000; i++)
        {
            // КЛЮЧЕВОЕ СЛОВО LOCK - автоматически создает блокировку
            // Компилятор преобразует это в вызовы Monitor.Enter и Monitor.Exit
            lock (lockObject)
            {
                // КРИТИЧЕСКАЯ СЕКЦИЯ - защищена блокировкой
                // Только один поток может выполнять этот блок кода одновременно
                counter++;

                if (i % 200 == 0)
                {
                    Console.WriteLine($"Поток {threadId}: итерация {i}, counter = {counter}");
                    Thread.Sleep(10); // Маленькая задержка для наглядности
                }
            }

            // Короткая пауза между итерациями
            Thread.Sleep(1);
        }

        Console.WriteLine($"Поток {threadId} завершил все итерации");
    }
}
*/
// volatile(поле) - указывает компилятору и процессору, что значение может изменяться разными потоками и его не следует кэшировать или оптимизировать.
/*
using System;
using System.Threading;

class VolatileExample
{
    // КЛЮЧЕВОЕ СЛОВО VOLATILE
    // Без ile volatкомпилятор может оптимизировать цикл в методе DoWork!!!!
    // и поток может никогда не увидеть изменение shouldStop из другого потока
    private volatile bool shouldStop = false;

    static void Main()
    {
        Console.WriteLine("=== Демонстрация VOLATILE ===");
        Console.WriteLine("Volatile предотвращает оптимизацию компилятором, которая может скрыть изменения между потоками\n");

        VolatileExample example = new VolatileExample();

        // Создаем рабочий поток
        Thread workerThread = new Thread(example.DoWork);
        Console.WriteLine("Основной поток: запускаю рабочий поток");
        workerThread.Start();

        // Даем рабочему потоку поработать 2 секунды
        Thread.Sleep(2000);

        Console.WriteLine("Основной поток: отправляю сигнал остановки...");

        // ИЗМЕНЯЕМ VOLATILE поле из другого потока
        // Без volatile рабочий поток может не увидеть это изменение
        // из-за кэширования значения в регистре процессора
        example.shouldStop = true;

        Console.WriteLine("Основной поток: сигнал отправлен. Жду завершения рабочего потока...");
        workerThread.Join();

        Console.WriteLine("Основной поток: рабочий поток завершен");
    }

    void DoWork()
    {
        Console.WriteLine("Рабочий поток: начинаю работу");

        int count = 0;

        // ЦИКЛ, КОТОРЫЙ МОЖЕТ БЫТЬ ОПТИМИЗИРОВАН
        // Без volatile компилятор может преобразовать это в:
        // if (!shouldStop) { while (true) { count++; } }
        // потому что он не видит, что shouldStop может измениться из другого потока
        while (!shouldStop) // Чтение volatile поля
        {
            count++;
            // Имитация работы
            if (count % 1000000 == 0)
            {
                Console.WriteLine($"Рабочий поток: выполнил {count / 1000000} миллионов итераций");
            }
        }

        Console.WriteLine($"Рабочий поток: остановлен после {count} итераций");
        Console.WriteLine("Без volatile этот цикл мог бы никогда не завершиться!");
    }
}
*/

// Золотые правила синххронизации 
// 1. Минимизируйте в критических секциях
// ПЛОХО: долгая операция в lock

using System.Threading;
using System;

lock (lockObject)
{
    var data = LoadDataFromDatabase(); // Долгая операция!
    ProcessData(data);
}

// ХОРОШО: выносим долгие операции за пределы lock
var data = LoadDataFromDatabase(); // Вне критической секции
lock (lockObject)
{
    ProcessData(data); // Только быстрая операция
}

// 2. Всегда освобождаете ресурсы в finally
Mutex mutex = new Mutex();
try
{
    mutex.WaitOne();
    // Работа с ресурсом
}
finally
{
    mutex.ReleaseMutex(); // Гарантированное освобождение
}

//  3. Избегайте вложенных блокировок
// ОПАСНО: может привести к взаимной блокировке
lock (lockA)
{
    lock (lockB)  // Если другой поток сделает наоборот - deadlock!
    {
        // ...
    }
}
// 4. Используете  Timeout для избежанния вечных блокировок
if (Monitor.TryEnter(lockObject, TimeSpan.FromSeconds(5)))
{
    try { /* работа */ }
    finally { Monitor.Exit(lockObject); }
}
else
{
    // Действия при невозможности получить блокировку
}