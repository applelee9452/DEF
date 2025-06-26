using System;
using System.Collections.Generic;

namespace DEF
{
    public class DEFUtils
    {
        static Dictionary<Type, string> DicComponentName { get; set; } = new();

        public static ContainerRpcAttribute GetContainerRpcAttribute<TContainerRpc>() where TContainerRpc : IContainerRpc
        {
            var t = typeof(TContainerRpc);
            return GetContainerRpcAttribute(t);
        }

        public static ContainerRpcAttribute GetContainerRpcAttribute(Type t)
        {
#if DEF_CLIENT
            //var t1 = (ILRuntime.Reflection.ILRuntimeType)t;
            //var arr = t1.GetCustomAttributes(false);
            var arr = Attribute.GetCustomAttributes(t);
#else
            var arr = Attribute.GetCustomAttributes(t);
#endif

            string name = string.Empty;

            if (arr != null)
            {
                foreach (var i in arr)
                {
                    if (i == null) continue;

                    if (i is ContainerRpcAttribute ca)
                    {
                        return ca;
                    }
                }
            }

            return null;
        }

        public static ContainerRpcObserverAttribute GetContainerRpcObserverAttribute<TContainerRpcObserver>() where TContainerRpcObserver : IContainerRpcObserver
        {
            var t = typeof(TContainerRpcObserver);
            return GetContainerRpcObserverAttribute(t);
        }

        public static ContainerRpcObserverAttribute GetContainerRpcObserverAttribute(Type t)
        {
#if DEF_CLIENT
            //var t1 = (ILRuntime.Reflection.ILRuntimeType)t;
            //var arr = t1.GetCustomAttributes(false);
            var arr = Attribute.GetCustomAttributes(t);
#else
            var arr = Attribute.GetCustomAttributes(t);
#endif

            string name = string.Empty;

            if (arr != null)
            {
                foreach (var i in arr)
                {
                    if (i == null) continue;

                    if (i is ContainerRpcObserverAttribute ca)
                    {
                        return ca;
                    }
                }
            }

            return null;
        }

        public static ComponentRpcAttribute GetComponentRpcAttribute<TComponentRpc>() where TComponentRpc : IComponentRpc
        {
            var t = typeof(TComponentRpc);
            return GetComponentRpcAttribute(t);
        }

        public static ComponentRpcAttribute GetComponentRpcAttribute(Type t)
        {
#if DEF_CLIENT
            //var t1 = (ILRuntime.Reflection.ILRuntimeType)t;
            //var arr = t1.GetCustomAttributes(false);
            var arr = Attribute.GetCustomAttributes(t);
#else
            var arr = Attribute.GetCustomAttributes(t);
#endif

            string name = string.Empty;

            if (arr != null)
            {
                foreach (var i in arr)
                {
                    if (i == null) continue;

                    if (i is ComponentRpcAttribute ca)
                    {
                        return ca;
                    }
                }
            }

            return null;
        }

        public static ComponentRpcObserverAttribute GetComponentRpcObserverAttribute<TComponentRpcObserver>() where TComponentRpcObserver : IComponentRpcObserver
        {
            var t = typeof(TComponentRpcObserver);
            return GetComponentRpcObserverAttribute(t);
        }

        public static ComponentRpcObserverAttribute GetComponentRpcObserverAttribute(Type t)
        {
#if DEF_CLIENT
            //var t1 = (ILRuntime.Reflection.ILRuntimeType)t;
            //var arr = t1.GetCustomAttributes(false);
            var arr = Attribute.GetCustomAttributes(t);
#else
            var arr = Attribute.GetCustomAttributes(t);
#endif

            string name = string.Empty;

            if (arr != null)
            {
                foreach (var i in arr)
                {
                    if (i == null) continue;

                    if (i is ComponentRpcObserverAttribute ca)
                    {
                        return ca;
                    }
                }
            }

            return null;
        }
        
        public static string GetComponentName(Type type)
        {
            if(DicComponentName.TryGetValue(type,out var n))
            {
                return n;
            }

#if DEF_CLIENT
            //var t = (ILRuntime.Reflection.ILRuntimeType)type;
            var t = type;
            var t2 = t.BaseType;
            var arr_t3 = t2.GenericTypeArguments;
            if (arr_t3 == null || arr_t3.Length == 0)
            {
                return t.Name;
            }
            var t4 = arr_t3[0];
            var arr = t4.GetCustomAttributes(false);
#else
            var t = type;
            var t2 = t.BaseType;
            var arr_t3 = t2.GenericTypeArguments;
            if (arr_t3 == null || arr_t3.Length == 0)
            {
                return t.Name;
            }
            var t4 = arr_t3[0];
            var arr = Attribute.GetCustomAttributes(t4);
#endif

            string name = string.Empty;

            if (arr != null)
            {
                foreach (var i in arr)
                {
                    if (i == null) continue;

                    if (i is ComponentAttribute ca)
                    {
                        name = ca.ComponentName;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                name = t.Name;
            }
            DicComponentName[type] = name;
            return name;
        }

        public static string GetComponentName<TComponent>() where TComponent : Component
        {
            return GetComponentName(typeof(TComponent));
        }
    }
}