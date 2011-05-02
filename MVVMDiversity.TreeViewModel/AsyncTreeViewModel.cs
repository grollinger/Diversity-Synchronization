using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using System.Threading;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace MVVMDiversity.ViewModel
{
    public class AsyncTreeViewModel : TreeViewModel
    {
        #region Global Singlethreaded worker 
        // Serializer is not Thread safe.

        private static Thread _worker;
        private static AutoResetEvent _treesHaveWork = new AutoResetEvent(false);
        private static Queue<AsyncTreeViewModel> workUnits = new Queue<AsyncTreeViewModel>();

        static AsyncTreeViewModel()
        {
            _worker = new Thread(() =>
            {
                while (_treesHaveWork.WaitOne())
                {
                    AsyncTreeViewModel workUnit;
                    while ((workUnit = nextWorkUnit()) != null)
                    {
                        workUnit.processQueues();
                    }
                }
            });
            _worker.IsBackground = true;
            _worker.Start();
        }

        private static AsyncTreeViewModel nextWorkUnit()
        {
            lock (_worker)
            {
                if (workUnits.Count > 0)
                    return workUnits.Dequeue();
                else
                    return null;
            }
        }
        private static void queueForWork(AsyncTreeViewModel unit)
        {
            lock (_worker)
            {
                if(!workUnits.Contains(unit))
                    workUnits.Enqueue(unit);
            }
            _treesHaveWork.Set();
        }
        #endregion



        private ManualResetEvent _queuesEmpty;

        private Queue<IISOViewModel> _addQueue, _removeQueue;        
        
        private bool _sealed;

        
        

        public AsyncTreeViewModel(IISOViewModelStore store )
            : base(store)
        {
            _addQueue = new Queue<IISOViewModel>();
            _removeQueue = new Queue<IISOViewModel>();
            
            _queuesEmpty = new ManualResetEvent(false);

            
        }

        private IISOViewModel safelyPop(Queue<IISOViewModel> queue)
        {
            lock (this)
            {
                if (queue.Count > 0)
                    return queue.Dequeue();
            }
            return null;
        }


        private void processQueues()
        {
            _queuesEmpty.Reset();
            IISOViewModel add, remove;
            while ((add = safelyPop(_addQueue)) != null |
                   (remove = safelyPop(_removeQueue)) != null)
            {
                if (add != null)
                    base.addGenerator(add);
                if (remove != null)
                    base.removeGenerator(remove);
            }
            _queuesEmpty.Set();
        }
       

        public override void addGenerator(IISOViewModel vm)
        {
            if (!_sealed)
            {
                lock (this)
                {
                    
                    _addQueue.Enqueue(vm);
                    queueForWork(this);
                }
            }
            else
                throw new InvalidOperationException("Object is currently locked against further changes!");
        }

        public override void removeGenerator(IISOViewModel vm)
        {
            if (!_sealed)
            {
                lock (this)
                {
                    _removeQueue.Enqueue(vm);
                    queueForWork(this);
                }
            }
            else
                throw new InvalidOperationException("Object is currently locked against further changes!");
        }

        Mutex selectionExclusion = new Mutex();

        public override IList<ISerializableObject> buildSelection()
        {
            IList<ISerializableObject> selection;
            selectionExclusion.WaitOne();

            _sealed = true;
            _queuesEmpty.WaitOne();
            selection = base.buildSelection();
            _sealed = false;

            selectionExclusion.ReleaseMutex();

            return selection;
        }

            
    }
}
