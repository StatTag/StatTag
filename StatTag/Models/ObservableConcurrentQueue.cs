using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Models
{
    /// <summary>
    /// We will acknowledge that a similar class with the same name exists (https://github.com/cyounes/ObservableConcurrentQueue), but
    /// this implementation is not directly based on and does not use any of that code.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public delegate void ItemAddedHandler(ConcurrentQueue<T> queue, T item);
        public event ItemAddedHandler ItemAdded;

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            HandleItemAdded(item);
        }

        /// <summary>
        /// Enqueue an item, ensuring that only one instance of the item is within the queue.
        /// This ensures that a notification about an item added is sent, regardless if the Enqueue
        /// method is actually called.  This reduces multiple queue entries for the same option,
        /// while allowing us to notify listeners that the process of adding an item was invoked.
        /// </summary>
        /// <param name="item"></param>
        public void EnqueueDistinctWithNotification(T item)
        {
            if (!this.Contains(item))
            {
                Enqueue(item);
            }
            else
            {
                // If we haven't added the item (meaning, it was already in the list), our collection
                // still provides a notification to any listeners.
                HandleItemAdded(item);
            }
        }

        private void HandleItemAdded(T item)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, item);
            }
        }
    }
}
