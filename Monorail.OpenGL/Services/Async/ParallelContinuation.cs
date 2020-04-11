using System.Collections;
using System.Collections.Generic;

namespace Monorail.Framework.Services.Async
{
    public class ParallelContinuation : Continuation
    {
        readonly List<Continuation> m_Routines; 
        public ParallelContinuation(params Continuation[] args) {
            m_Routines = new List<Continuation>(args);
            ActionItems = Process();
        }

        IEnumerator Process() {
            while (m_Routines.Count > 0)
            {
                for (int i = 0; i < m_Routines.Count; i++)
                {
                    var continuation = m_Routines[i];
                    var enumerator = continuation.GetEnumerator();

                    if (enumerator.Current is IEnumerator) {
                        if (MoveNext((IEnumerator)enumerator.Current)) {
                            continue;
                        }
                    }
                    if (!enumerator.MoveNext()) {
                        m_Routines.RemoveAt(i--);
                    }
                }
                yield return null;
                
            }
        }

        bool MoveNext(IEnumerator routine)
        {
            if (routine.Current is IEnumerator) {
                if (MoveNext((IEnumerator)routine.Current)) {
                    return true;
                }
            }
            return routine.MoveNext();
        }
    }
}
