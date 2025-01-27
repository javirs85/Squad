using System;
using System.Collections.Generic;

namespace Gtec.Bandpower
{
    public sealed class EventHandler
    {
        private static EventHandler instance = null;
        private static readonly object padlock = new object();

        private Queue<Action> _queue;

        EventHandler()
        {
            _queue = new Queue<Action>();
        }

        public static EventHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new EventHandler();
                    }
                    return instance;
                }
            }
        }

        public void Enqueue(Action a)
        {
            _queue.Enqueue(a);
        }

        public void Dequeue()
        {
            _queue.Dequeue()?.Invoke();
        }

        public void DequeueAll()
        {
            for (int i = 0; i < _queue.Count; i++)
                Dequeue();
        }
    }
}