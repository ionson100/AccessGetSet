using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TestExz
{
    class MyClass
    {
        public string Name;
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            var p=new MyClass();
            var tt = H.GetField<MyClass>();
        }
    }
    static class H
    {
        public static Func<T, object> GetFieldAccessor<T>(string fieldName)
        {
            var param = Expression.Parameter(typeof(T), "df");
            var member = Expression.Field(param, fieldName);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(member, typeof(Object)), param);
            var c = lambda.Compile();
            return c;
        }

        public static Action<T, object> SetFieldAccessor<T>(string fieldName)
        {

            var param = Expression.Parameter(typeof(T), "df");
            var valueExp = Expression.Parameter(typeof(object));
            var c = Expression.Convert(valueExp, typeof(int));///////
            var member = Expression.Field(param, fieldName);
            var assignExp = Expression.Assign(member, c);
            var cc = Expression.Lambda<Action<T, object>>(assignExp, param, valueExp).Compile();
            return cc;
        }

        public static Func<string, object> GetField<T>()
        {
           
            var param1 = Expression.Variable(typeof(T), "df");
            var er = Expression.Variable(typeof (T));
            var param2 = Expression.Parameter(typeof(string));
            var member = Expression.Field(er,"Name");
            var lambda = Expression.Lambda<Func<string, object>>(Expression.Convert(member, typeof(Object)), param2);
            var c = lambda.Compile();
            return c;
        }
    }
}
