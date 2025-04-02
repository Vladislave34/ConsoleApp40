using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp40
{
    public delegate void DeletageContextConnection(ThreadAppContext threadAppContext);
    public delegate void UserAddedHandler(Banan user);

    public class DataBaseManager
    {
        private ThreadAppContext _threadAppContext;


        public event DeletageContextConnection GetConnectionEvent;
        public event Action<int> DataInserted;
        
        public event UserAddedHandler UserAddedEvent;

        public static ManualResetEvent mre = new ManualResetEvent(false);
        public DataBaseManager()
        {
            Thread runConection = new Thread(RunAsyncConnection);
            runConection.Start();
        }

        private void RunAsyncConnection()
        {
            _threadAppContext = new ThreadAppContext();
            _threadAppContext.banan.Any();
            if (GetConnectionEvent != null)
                GetConnectionEvent(_threadAppContext);
        }
        //public void AddUser(Banan user)
        //{
        //    Thread addUserThread = new Thread(() => AddUserThread(user));
        //    addUserThread.Start();
        //}

        //private void AddUserThread(Banan user)
        //{
        //    using (var context = new ThreadAppContext())
        //    {
        //        context.banan.Add(user);
        //        context.SaveChanges();
        //    }

        //    UserAddedEvent?.Invoke(user);
        //}
        //public void AddBanansAsync(int count, CancellationToken? token = null)
        //{
        //    Thread thread = new Thread(() => AddBanans(count, token));
        //    thread.Start();

        //}
        //public void AddBanans(int count, CancellationToken? token = null)
        //{

        //        using (var transaction = _threadAppContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var faker = new Faker<Banan>("uk")
        //                    .RuleFor(b => b.FirstName, f => f.Name.FirstName())
        //                    .RuleFor(b => b.LastName, f => f.Name.LastName())
        //                    .RuleFor(b => b.Image, f => f.Internet.Avatar())
        //                    .RuleFor(b => b.Phone, f => f.Phone.PhoneNumber())
        //                    .RuleFor(b => b.Sex, f => f.Random.Bool());

        //                for (int i = 0; i < count; i++)
        //                {
        //                    var b = faker.Generate(1);
        //                    _threadAppContext.Add(b[0]);
        //                    _threadAppContext.SaveChanges();
        //                    DataInserted?.Invoke(i + 1);
        //                    mre.WaitOne(Timeout.Infinite);
        //                    if (token != null)
        //                    {
        //                        if (token.Value.IsCancellationRequested)  // проверяем наличие сигнала отмены задачи
        //                        {
        //                            return;     //  выходим из метода и тем самым завершаем задачу
        //                        }
        //                    }
        //                }

        //                transaction.Commit(); // Якщо все добре, підтверджуємо транзакцію
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Помилка: {ex.Message}");
        //                transaction.Rollback(); // Відкат змін у разі помилки
        //            }
        //        }
        //}
        public void AddSingleUser()
        {
            var user = GenerateUser();
            _threadAppContext.banan.Add(user);
            _threadAppContext.SaveChanges();
        }

        
        public void AddUsersMultithreaded(int count)
        {
            Thread[] threads = new Thread[count];
            for (int i = 0; i < count; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var user = GenerateUser();
                    lock (_threadAppContext)
                    {
                        _threadAppContext.banan.Add(user);
                        _threadAppContext.SaveChanges();
                        
                    }
                    
                });
                DataInserted?.Invoke(i + 1);
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private Banan GenerateUser()
        {
            var faker = new Faker<Banan>("uk")
                .RuleFor(b => b.FirstName, f => f.Name.FirstName())
                .RuleFor(b => b.LastName, f => f.Name.LastName())
                .RuleFor(b => b.Image, f => f.Internet.Avatar())
                .RuleFor(b => b.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(b => b.Sex, f => f.Random.Bool());
            return faker.Generate();
        }

        public void Dispose()
        {
            _threadAppContext?.Dispose();
            Console.WriteLine("БД з'єднання закрито");
        }
    }
}
