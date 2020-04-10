using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Monorail.Services
{
    // Any message you want to pass should implement IMessage
    public interface IMessage { }

    // Any subscriber should implement IMessage Handler
    public interface IMessageHandler { }

    // If we want to use the Message system with the Service locator (otherwise remove this)
    public interface IMessageSystem : IService { /*TODO Fillout */ }

    public class MessageService : IMessageSystem
    {
        Int32 ManagedThreadId;

        // Place to store queued messages (i.e non-main thread raised messages)
        private ConcurrentQueue<IMessage> m_QueuedMessages = new ConcurrentQueue<IMessage>();

        private struct MessageRecipiant
        {
            public Delegate Callback;

            public IMessageHandler Target;
        }

        public MessageService()
        {
            // We want to create this Message System on the main thread so any messages that come from
            // another thread will be enqued and published when Update() is called.
            ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        private Dictionary<Type, List<MessageRecipiant>> m_Subscribers = new Dictionary<Type, List<MessageRecipiant>>();

        private int m_SubCount;

        public Int32 ListenerCount
        {
            get
            {
                lock (m_Subscribers)
                {
#if DEBUG
                    // Note: The code below and the cached variable should always agree as to the subscriber count
                    var rv = 0;
                    foreach (var k in m_Subscribers.Values)
                    {
                        rv += k.Count;
                    }

                    Debug.Assert(m_SubCount == rv);
#endif

                    return m_SubCount;
                }
            }
        }

        public void Unsubscribe(IMessageHandler target)
        {
            lock (m_Subscribers)
            {
                foreach (var v in m_Subscribers.Values)
                {
                    var count = v.RemoveAll(x => x.Target == target);
                    m_SubCount -= count;
                }
            }
        }

        public void Subscribe<T>(IMessageHandler reciever, Action<T> callback) where T : class, IMessage
        {
            var type = typeof(T);

            lock (m_Subscribers)
            {
                if (m_Subscribers.ContainsKey(type))
                {
                    var listeners = m_Subscribers[type];
                    foreach (var listener in listeners)
                    {
                        if (listener.Callback.Equals(callback))
                        {
                            return;
                        }
                    }
                    listeners.Add(new MessageRecipiant() { Callback = (Delegate)callback, Target = reciever });
                    m_SubCount++;
                }
                else
                {
                    var list = new List<MessageRecipiant>
                    {
                        new MessageRecipiant() { Callback = (Delegate) callback, Target = reciever }
                    };

                    m_Subscribers.Add(type, list);
                    m_SubCount++;
                }
            }
        }

        public void Unsubscribe<T>(IMessageHandler target, Action<IMessage> callback)
        {
            lock (m_Subscribers)
            {
                var type = typeof(T);
                if (m_Subscribers.ContainsKey(type))
                {
                    var listeners = m_Subscribers[type];

                    for (int i = 0; i < listeners.Count; i++)
                    {
                        if (target == listeners[i].Target && callback.Equals(listeners[i].Callback))
                        {
                            listeners.Remove(listeners[i]);
                            m_SubCount--;
                            i--;
                        }
                    }
                }
            }
        }

        // You will need to pump this manually from your main thread.
        public void MainThreadUpdate(float delta)
        {
            // Publish all queued Messages
            while (m_QueuedMessages.Count > 0)
            {
                IMessage msg;
                if (m_QueuedMessages.TryDequeue(out msg))
                {
                    PublishMessage(msg);
                }
            }
        }

        public void PublishMessage(IMessage message)
        {
            if (message == null) // Can remove this guard if you are confidant no one will send you a null!
            {
                return;
            }

            if (Thread.CurrentThread.ManagedThreadId != ManagedThreadId)
            {
                m_QueuedMessages.Enqueue(message);
            }
            else
            {
                var type = message.GetType();
                lock (m_Subscribers)
                {
                    if (m_Subscribers.ContainsKey(type))
                    {
                        var listeners = m_Subscribers[type];
                        foreach (var listener in listeners)
                        {
                            var action = (Delegate)listener.Callback;
                            action.DynamicInvoke(message); // TODO Can we avoid a DynamicInvoke?
                        }
                    }
                }
            }
        }
    }
}