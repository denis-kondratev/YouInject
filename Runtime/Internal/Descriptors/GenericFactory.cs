using System;
using System.Reflection;
using UnityEngine.Assertions;

namespace YouInject.Internal
{
    internal class GenericFactory
    {
        public static readonly MethodInfo[] FactoryMethods =
        {
            typeof(GenericFactory).GetMethod(nameof(Instantiate0)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate1)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate2)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate3)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate4)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate5)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate6)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate7)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate8)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate9)),
            typeof(GenericFactory).GetMethod(nameof(Instantiate10))
        };

        private readonly Type _resultType;
        private readonly object[] _parameters;
        private readonly int _specifiedParameterCount;

        public GenericFactory(Type resultType, object[] steadyParameters, int totalParameterCount)
        {
            if (totalParameterCount < 0) throw new ArgumentOutOfRangeException(nameof(totalParameterCount));

            _resultType = resultType;
            _specifiedParameterCount = totalParameterCount - steadyParameters.Length;
            _parameters = GetParameters(steadyParameters, totalParameterCount, _specifiedParameterCount);
        }

        public TResult Instantiate0<TResult>() where TResult : class
        {
            if (Instantiate() is not TResult product)
            {
                throw new Exception();
            }
            
            return product;
        }

        public TResult Instantiate1<T, TResult>(T arg) where TResult : class
        {
            Assert.AreEqual(1, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate2<T1, T2, TResult>(T1 arg1, T2 arg2) where TResult : class
        {
            Assert.AreEqual(2, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate3<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3) where TResult : class
        {
            Assert.AreEqual(3, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate4<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TResult : class
        {
            Assert.AreEqual(4, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            _parameters[3] = arg4!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate5<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where TResult : class
        {
            Assert.AreEqual(5, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            _parameters[3] = arg4!;
            _parameters[4] = arg5!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate6<T1, T2, T3, T4, T5, T6, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            where TResult : class
        {
            Assert.AreEqual(6, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            _parameters[3] = arg4!;
            _parameters[4] = arg5!;
            _parameters[5] = arg6!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate7<T1, T2, T3, T4, T5, T6, T7, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            where TResult : class
        {
            Assert.AreEqual(7, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            _parameters[3] = arg4!;
            _parameters[4] = arg5!;
            _parameters[5] = arg6!;
            _parameters[6] = arg7!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate8<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            where TResult : class
        {
            Assert.AreEqual(8, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            _parameters[3] = arg4!;
            _parameters[4] = arg5!;
            _parameters[5] = arg6!;
            _parameters[6] = arg7!;
            _parameters[7] = arg8!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate9<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
            where TResult : class
        {
            Assert.AreEqual(9, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            _parameters[3] = arg4!;
            _parameters[4] = arg5!;
            _parameters[5] = arg6!;
            _parameters[6] = arg7!;
            _parameters[7] = arg8!;
            _parameters[8] = arg9!;
            
            var product = Instantiate() as TResult;
            return product!;
        }

        public TResult Instantiate10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) 
            where TResult : class
        {
            Assert.AreEqual(10, _specifiedParameterCount, "Used wrong instantiating method.");
            
            _parameters[0] = arg1!;
            _parameters[1] = arg2!;
            _parameters[2] = arg3!;
            _parameters[3] = arg4!;
            _parameters[4] = arg5!;
            _parameters[5] = arg6!;
            _parameters[6] = arg7!;
            _parameters[7] = arg8!;
            _parameters[8] = arg9!;
            _parameters[9] = arg10!;
            
            var product = Instantiate() as TResult;
            return product!;
        }
        
        private object Instantiate()
        {
            var instance = Activator.CreateInstance(_resultType, _parameters);
            return instance!;
        }

        private static object[] GetParameters(object[] steadyParameters, int totalParameterCount, int specifiedParameterCount)
        {
            if (totalParameterCount == 0) return Array.Empty<object>();

            var parameters = new object[totalParameterCount];

            for (var i = specifiedParameterCount; i < totalParameterCount; i++)
            {
                parameters[i] = steadyParameters[i - specifiedParameterCount];
            }

            return parameters;
        }
    }
}