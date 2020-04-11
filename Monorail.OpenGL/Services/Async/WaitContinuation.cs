using System;
using System.Collections;

namespace Monorail.Framework.Services.Async
{
    public class WaitContinuation : Continuation
    {
        readonly Int32 m_WaitForMs;
        Int32 m_CurrentTimer;

        public WaitContinuation(Int32 ms) : base() {
            m_WaitForMs = ms;
            m_CurrentTimer = 0;
            ActionItems = LoopForever();
        }

        public IEnumerator LoopForever() {
            while(m_CurrentTimer < m_WaitForMs) {
                m_CurrentTimer += Coroutines.DeltaMilliseconds;
                yield return false;
            }
        }
    }
}
