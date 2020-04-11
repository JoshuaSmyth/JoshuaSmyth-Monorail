using System.Collections;

namespace Monorail.Framework.Services.Async
{
    interface IContinuation : IEnumerator
    {
         bool IsParallel();
    }

    public class Continuation : IContinuation
    {
        protected IEnumerator ActionItems;

        public Continuation() {
            
        }

        public Continuation(IEnumerator actionItems) {
            ActionItems = actionItems;
        }

        public IEnumerator GetEnumerator() {
            return ActionItems;
        }

        public bool MoveNext() {
            return ActionItems.MoveNext();
        }

        public void Reset() {
            ActionItems.Reset();
        }

        public object Current { get { return ActionItems.Current; } }

        public static Continuation Create() {
            return new Continuation();
        }

        public bool IsParallel() {
            return false;
        }
    }
}
