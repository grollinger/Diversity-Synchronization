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

        private AutoResetEvent queuesNotEmpty;
        private ManualResetEvent queuesEmpty;

        private Queue<IISOViewModel> addQueue, removeQueue;        
        private Thread _worker;
        private bool _sealed;
        

        public AsyncTreeViewModel(IISOViewModelStore store )
            : base(store)
        {
            addQueue = new Queue<IISOViewModel>();
            removeQueue = new Queue<IISOViewModel>();
            queuesNotEmpty = new AutoResetEvent(false);
            queuesEmpty = new ManualResetEvent(false);

            _worker = new Thread(() =>
                {
                    while (queuesNotEmpty.WaitOne())
                    {
                        queuesEmpty.Reset();
                        IISOViewModel add,remove;
                        while ((add = safelyPop(addQueue)) != null |
                               (remove = safelyPop(removeQueue)) != null)
                        {
                            if (add != null)
                                base.addGenerator(add);
                            if (remove != null)
                                base.removeGenerator(remove);
                        }
                        queuesEmpty.Set();
                    }

                });
            _worker.IsBackground = true;
            _worker.Start();
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

       

       

        public override void addGenerator(IISOViewModel vm)
        {
            if (!_sealed)
            {
                lock (this)
                {
                    
                    addQueue.Enqueue(vm);
                    queuesNotEmpty.Set();
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
                    removeQueue.Enqueue(vm);
                    queuesNotEmpty.Set();
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
            queuesEmpty.WaitOne();
            selection = base.buildSelection();
            _sealed = false;

            selectionExclusion.ReleaseMutex();

            return selection;
        }

            
    }
}
