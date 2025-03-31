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

        public event UserAddedHandler UserAddedEvent;
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
        public void AddUser(Banan user)
        {
            Thread addUserThread = new Thread(() => AddUserThread(user));
            addUserThread.Start();
        }

        private void AddUserThread(Banan user)
        {
            using (var context = new ThreadAppContext())
            {
                context.banan.Add(user);
                context.SaveChanges();
            }

            UserAddedEvent?.Invoke(user);
        }
    }
}
