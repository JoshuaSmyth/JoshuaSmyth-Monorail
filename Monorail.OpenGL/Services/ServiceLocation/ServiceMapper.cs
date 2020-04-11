using System;
using System.Collections.Generic;

namespace Monorail.Framework.Services.ServiceLocation
{
    public class ServiceMapper : IServiceMapper
    {
        readonly Dictionary<Type, Object> m_DependancyMap;

        public ServiceMapper() {
            m_DependancyMap = new Dictionary<Type, Object>();
        }
 
        public void AddDepenancy<T>(T obj) {
            var type = typeof (T);
            if (!m_DependancyMap.ContainsKey(type)) {
                  m_DependancyMap.Add(type, obj);
            } else {
                throw new ApplicationException("The type " + type.FullName + " is already registered");
            }
        }
          
        public void Clear() {
            m_DependancyMap.Clear();
        }

        public void ResolveDependancies() {
            foreach (var o in m_DependancyMap) {
                var v = o.Value;
                if (v is IRequireServices) {
                    (v as IRequireServices).ResolveDependancies(this);
                }
            }
        }

        public T GetDependancy<T>() where T : class {
            var type = typeof(T);
            if (m_DependancyMap.ContainsKey(type)) {
                var rawObject = m_DependancyMap[type];
                if (rawObject is T) {
                    var returnObject = rawObject as T;
                    return returnObject;
                }
                var errorString = String.Format("Object is not of type:{0}", typeof(T));
                throw new ApplicationException(errorString);
            }
            throw new ApplicationException("The type " + type.FullName + " is not registered");
        }
    }
}
