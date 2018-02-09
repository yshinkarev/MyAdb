using System.Threading;
using MyAdb.Commands;

namespace MyAdb
{
    public class MultiThreadObject
    {
        private static MultiThreadObject _instance;
        private static readonly object _lockFlag = new object();

        private readonly ManualResetEvent _event;
        private int _count;
        private int _result = Command.OK;

        public static MultiThreadObject Instance
        {
            get
            {
                lock (_lockFlag)
                {
                    if (_instance == null)
                        _instance = new MultiThreadObject();
                }

                return _instance;
            }
        }

        protected MultiThreadObject()
        {
            _event = new ManualResetEvent(false);
            
        }

        public void Start(int count)
        {
            _count = count;
        }

        public void CommandFinish(int commandResult)
        {
            lock (_lockFlag)
            {
                _count--;

                if (commandResult != Command.OK)
                {
                    _result = commandResult;
                    Utils.BeepIfCan();
                }

                if (_count == 0)
                    _event.Set();
            }
        }

        public int Wait()
        {
            _event.WaitOne();
            _event.Close();
            return _result;
        }
    }
}