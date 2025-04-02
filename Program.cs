using Bogus;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace ConsoleApp40
{
    internal class Program
    {
        private static DataBaseManager _dataBaseManager;
        public static CancellationTokenSource cts; 
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _dataBaseManager = new DataBaseManager();
            _dataBaseManager.GetConnectionEvent += DataBaseManager_GetConnectionEvent;
            _dataBaseManager.DataInserted += _dataBaseManager_DataInserted;

            Console.WriteLine("Підготовка програми до запуску ...");

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine($"Run time {ts}");
        }
        //private static void OnProcessExit(object sender, EventArgs e)
        //{
        //    Console.WriteLine("Програма завершується... Очікуємо завершення потоків.");
        //    _dataBaseManager?.Dispose(); // Закриваємо підключення до БД
        //}
        private static void _dataBaseManager_DataInserted(int obj)
        {
            Console.WriteLine($"Insert data --{obj}--");
        }
        private static void DataBaseManager_GetConnectionEvent(ThreadAppContext threadAppContext)
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            DataBaseManager.mre.Set();
            Console.WriteLine("З'єднання з БД успішне, кількість бананів {0}", threadAppContext.banan.Count());

            Console.WriteLine("Вкажіть кількість користувачів");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Некоректне значення. Введіть додатне число.");
                return;
            }

            // Вимірюємо час додавання одного користувача
            Stopwatch estimateStopwatch = Stopwatch.StartNew();
            _dataBaseManager.AddSingleUser();
            estimateStopwatch.Stop();

            double timePerUser = estimateStopwatch.ElapsedMilliseconds;
            double estimatedTime = (timePerUser * count) / 1000.0;
            Console.WriteLine($"Прогнозований час: {estimatedTime:F2} секунд");

            // Вимірювання фактичного часу
            Stopwatch actualStopwatch = Stopwatch.StartNew();
            _dataBaseManager.AddUsersMultithreaded(count);
            actualStopwatch.Stop();

            double actualTime = actualStopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine($"Фактичний час: {actualTime:F2} секунд");
            Console.WriteLine($"Різниця прогнозу та реальності: {Math.Abs(estimatedTime - actualTime):F2} секунд");

            while (true)
            {
                Console.WriteLine("Натисніть p - пауза, r - відновити, q - вихід");
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.P)
                {
                    Console.WriteLine("Пауза ...");
                    DataBaseManager.mre.Reset();
                }
                else if (key == ConsoleKey.R)
                {
                    Console.WriteLine("Відновлено ...");
                    DataBaseManager.mre.Set();
                }
                else if (key == ConsoleKey.Q)
                {
                    Console.WriteLine("Вихід без збереження...");
                    DataBaseManager.mre.Set();
                    cts.Cancel();
                    break;
                }
            }
        }
        //public static void insert(int count, DataBaseManager dataBaseManager)
        //{
        //    var faker = new Faker<Banan>()
        //        .RuleFor(u => u.FirstName, f => f.Name.FirstName())
        //        .RuleFor(u => u.LastName, f => f.Name.LastName())
        //        .RuleFor(u => u.Image, f => f.Internet.Avatar())
        //        .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("+380 ## ### ## ##"))
        //        .RuleFor(u => u.Sex, f => f.Random.Bool());

        //    for (int i = 0; i < count; i++)
        //    {
        //        Banan newUser = faker.Generate();
        //        newUser.Image = SaveImage( newUser.FirstName); 
        //        dataBaseManager.AddUser(newUser);
        //    }
        //}
        //public static string SaveImage(string userName)
        //{
        //    string folderPath = "D:\\Git\\project_c_13\\ConsoleApp40\\image";
        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    string fileName = $"{userName}_{Guid.NewGuid()}.jpg";
        //    string filePath = Path.Combine(folderPath, fileName);

        //    string imageUrl = "https://thispersondoesnotexist.com/"; 

        //    try
        //    {
        //        using (WebClient client = new WebClient())
        //        {
        //            client.DownloadFile(new Uri(imageUrl), filePath);
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        Console.WriteLine($"Помилка завантаження фото: {ex.Message}");
        //        return "default.jpg"; 
        //    }

        //    return filePath;
        //}
    }
}
