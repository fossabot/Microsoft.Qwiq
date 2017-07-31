using Castle.DynamicProxy;
using System.Diagnostics;

namespace Microsoft.Qwiq.Exceptions
{
    /// <summary>
    /// Provides a base class for Castle intercepted types
    /// </summary>
    /// <remarks>
    /// This inspects the incoming proxy objects and ensures Object.Equals(Object) and Object.GetHashCode() are directed to the appropriate location.
    /// </remarks>
    [DebuggerStepThrough]
    public class ProxyBase
    {
        /// <summary>
        /// Provides hook into object providing Equals(Object)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>
        /// Intercepted interfaces can provide IEquatable`1 implementations, but that will not forward calls to
        /// Object.Equals(Object) or Object.GetHashCode() since neither is part of the interface.
        /// </remarks>
        public override bool Equals(object obj)
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            var proxy = this as IProxyTargetAccessor;
            // ReSharper restore SuspiciousTypeConversion.Global
            if (proxy == null)
            {
#pragma warning disable RECS0149 // Finds potentially erroneous calls to Object.Equals
                return base.Equals(obj);
#pragma warning restore RECS0149 // Finds potentially erroneous calls to Object.Equals
            }
            var target = proxy.DynProxyGetTarget();
            if (target == null)
            {
#pragma warning disable RECS0149 // Finds potentially erroneous calls to Object.Equals
                return base.Equals(obj);
#pragma warning restore RECS0149 // Finds potentially erroneous calls to Object.Equals
            }
            return target.Equals(obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            var proxy = this as IProxyTargetAccessor;
            // ReSharper restore SuspiciousTypeConversion.Global
            if (proxy == null)
            {
                // ReSharper disable BaseObjectGetHashCodeCallInGetHashCode
                return base.GetHashCode();
                // ReSharper restore BaseObjectGetHashCodeCallInGetHashCode
            }
            var target = proxy.DynProxyGetTarget();
            if (target == null)
            {
                // ReSharper disable BaseObjectGetHashCodeCallInGetHashCode
                return base.GetHashCode();
                // ReSharper restore BaseObjectGetHashCodeCallInGetHashCode
            }
            return target.GetHashCode();
        }

        public override string ToString()
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            var proxy = this as IProxyTargetAccessor;
            // ReSharper restore SuspiciousTypeConversion.Global
            if (proxy == null)
            {
                return base.ToString();
            }
            var target = proxy.DynProxyGetTarget();
            if (target == null)
            {
                return base.ToString();
            }
            return target.ToString();
        }
    }
}