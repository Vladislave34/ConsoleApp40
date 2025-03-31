using Bogus;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace ConsoleApp40
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //Обчислюємо час роботи програми


            ThreadAppContext threadAppContext = new ThreadAppContext();
            threadAppContext.banan.Any();
            DataBaseManager dataBaseManager = new DataBaseManager();
            dataBaseManager.GetConnectionEvent += DataBaseManager_GetConnectionEvent;
            dataBaseManager.UserAddedEvent += user => Console.WriteLine($"Event: User {user.Id} was added!");

            insert(10, dataBaseManager);
            stopwatch.Stop();
            //TimeSpan - змінна, яка може зберігаати, сек, мілісенди, хв, год, дні
            TimeSpan ts = stopwatch.Elapsed; //Оперує тіками - одиниці часу
            Console.WriteLine($"Run time {ts}");
        }
        private static void DataBaseManager_GetConnectionEvent(ThreadAppContext threadAppContext)
        {
            Console.WriteLine("Зєднання з БД успішно кількість бананів {0}", threadAppContext.banan.Count());
        }
        public static void insert(int count, DataBaseManager dataBaseManager)
        {
            var faker = new Faker<Banan>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Image, f => f.Internet.Avatar())
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("+380 ## ### ## ##"))
                .RuleFor(u => u.Sex, f => f.Random.Bool());

            for (int i = 0; i < count; i++)
            {
                Banan newUser = faker.Generate();
                newUser.Image = SaveImage( newUser.FirstName); 
                dataBaseManager.AddUser(newUser);
            }
        }
        public static string SaveImage(string userName)
        {
            string folderPath = "D:\\Git\\project_c_13\\ConsoleApp40\\image";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{userName}_{Guid.NewGuid()}.jpg";
            string filePath = Path.Combine(folderPath, fileName);

            string imageUrl = "https://thispersondoesnotexist.com/"; 

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(imageUrl), filePath);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Помилка завантаження фото: {ex.Message}");
                return "default.jpg"; 
            }

            return filePath;
        }
    }
}
