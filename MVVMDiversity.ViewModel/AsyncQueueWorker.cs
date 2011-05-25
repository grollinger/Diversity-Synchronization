using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MVVMDiversity.ViewModel
{
    public class AsyncQueueWorker<T> where T : class
    {
        private Action<T> _operation;
        private Queue<T> _taskQueue = new Queue<T>();
        private AutoResetEvent _hasWork = new AutoResetEvent(false);

        public event Action QueueEmpty;

        public AsyncQueueWorker(Action<T> operation)
        {
            _operation = operation;

            Thread worker = new Thread(() =>
                {
                    while (_hasWork.WaitOne())
                    {
                        T currentTask;
                        while ((currentTask = safelyDequeue()) != null)
                        {
                            _operation(currentTask);
                        }
                        if (QueueEmpty != null)
                            QueueEmpty();
                    }
                });
            worker.IsBackground = true;
            worker.Start();
        }

        private T safelyDequeue()
        {
            lock (this)
            {
                if (_taskQueue.Count > 0)
                    return _taskQueue.Dequeue();
                else
                    return default(T);
            }
        }

        public void Enqueue(T task)
        {
            lock (this)
            {
                _taskQueue.Enqueue(task);
                _hasWork.Set();
            }
        }

        public int WorkItems
        {
            get
            {
                lock (this)
                {
                    return _taskQueue.Count;
                }
            }
        }


    }
}
