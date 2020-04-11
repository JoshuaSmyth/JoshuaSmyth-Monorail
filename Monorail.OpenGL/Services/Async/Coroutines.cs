using System;
using System.Collections;
using System.Collections.Generic;

namespace Monorail.Framework.Services.Async
{
    public class Coroutines
    {
        readonly List<Continuation> m_Routines = new List<Continuation>();

        public static Int32 DeltaMilliseconds;

        public Coroutines()
        {

        }

        public void Start(Continuation routine)
        {
            m_Routines.Add(routine);
        }

        public void Start(IEnumerator routine)
        {
            m_Routines.Add(new Continuation(routine));
        }

        public void StopAll()
        {
            m_Routines.Clear();
        }

        public void Update(Int32 deltaMs)
        {
            DeltaMilliseconds =deltaMs;
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

        public int Count
        {
            get { return m_Routines.Count; }
        }

        public bool Running
        {
            get { return m_Routines.Count > 0; }
        }
    }
}

