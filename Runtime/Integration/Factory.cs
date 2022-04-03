using System;
using System.Reflection;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class Factory
    {
        public static readonly MethodInfo[] FactoryMethods =
        {
            typeof(Factory).GetMethod(nameof(Instantiate0)),
            typeof(Factory).GetMethod(nameof(Instantiate1)),
            typeof(Factory).GetMethod(nameof(Instantiate2)),
            typeof(Factory).GetMethod(nameof(Instantiate3)),
            typeof(Factory).GetMethod(nameof(Instantiate4)),
            typeof(Factory).GetMethod(nameof(Instantiate5)),
            typeof(Factory).GetMethod(nameof(Instantiate6)),
            typeof(Factory).GetMethod(nameof(Instantiate7)),
            typeof(Factory).GetMethod(nameof(Instantiate8)),
            typeof(Factory).GetMethod(nameof(Instantiate9)),
            typeof(Factory).GetMethod(nameof(Instantiate10))
        };

        private readonly Type _resultType;
        private readonly object[] _args;
        private readonly int _specifiedArgCount;

        public Factory(Type resultType, object[] steadyArgs, int argCount)
        {
            if (argCount < 0) throw new ArgumentOutOfRangeException(nameof(argCount));

            _resultType = resultType;
            _specifiedArgCount = argCount - steadyArgs.Length;
            _args = GetArgs(steadyArgs, argCount, _specifiedArgCount);
        }

        public TResult Instantiate0<TResult>() where TResult : class
        {
            Assert.AreEqual(0, _specifiedArgCount, "Used wrong instantiating method.");
            
            if (Instantiate() is not TResult product)
            {
                throw new Exception();
            }
            
            return product;
        }

        public TResult Instantiate1<T, TResult>(T arg) where TResult : class
        {
            Assert.AreEqual(1, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate2<T1, T2, TResult>(T1 arg1, T2 arg2) where TResult : class
        {
            Assert.AreEqual(2, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate3<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3) where TResult : class
        {
            Assert.AreEqual(3, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate4<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TResult : class
        {
            Assert.AreEqual(4, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            _args[3] = arg4!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate5<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where TResult : class
        {
            Assert.AreEqual(5, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            _args[3] = arg4!;
            _args[4] = arg5!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate6<T1, T2, T3, T4, T5, T6, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            where TResult : class
        {
            Assert.AreEqual(6, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            _args[3] = arg4!;
            _args[4] = arg5!;
            _args[5] = arg6!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate7<T1, T2, T3, T4, T5, T6, T7, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            where TResult : class
        {
            Assert.AreEqual(7, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            _args[3] = arg4!;
            _args[4] = arg5!;
            _args[5] = arg6!;
            _args[6] = arg7!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate8<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            where TResult : class
        {
            Assert.AreEqual(8, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            _args[3] = arg4!;
            _args[4] = arg5!;
            _args[5] = arg6!;
            _args[6] = arg7!;
            _args[7] = arg8!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate9<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
            where TResult : class
        {
            Assert.AreEqual(9, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            _args[3] = arg4!;
            _args[4] = arg5!;
            _args[5] = arg6!;
            _args[6] = arg7!;
            _args[7] = arg8!;
            _args[8] = arg9!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) 
            where TResult : class
        {
            Assert.AreEqual(10, _specifiedArgCount, "Used wrong instantiating method.");
            
            _args[0] = arg1!;
            _args[1] = arg2!;
            _args[2] = arg3!;
            _args[3] = arg4!;
            _args[4] = arg5!;
            _args[5] = arg6!;
            _args[6] = arg7!;
            _args[7] = arg8!;
            _args[8] = arg9!;
            _args[9] = arg10!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        private object Instantiate()
        {
            var instance = Activator.CreateInstance(_resultType, _args);
            return instance!;
        }

        private static object[] GetArgs(object[] steadyArgs, int argCount, int specifiedArgCount)
        {
            if (argCount == 0) return Array.Empty<object>();

            var args = new object[argCount];

            for (var i = specifiedArgCount; i < argCount; i++)
            {
                args[i] = steadyArgs[i - specifiedArgCount];
            }

            return args;
        }
    }
}