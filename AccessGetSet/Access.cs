using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace AccessGetSet
{
    public static class Access<T>
    {

        static readonly Lazy<Dictionary<string, Func<T, object>>> AccessGetPropertys = new Lazy<Dictionary<string, Func<T, object>>>(
            () =>
            {
                var pr = typeof(T).GetProperties();
                var dictionary = new Dictionary<string, Func<T, object>>();
                foreach (var propertyInfo in pr)
                {

                    var instance = Expression.Parameter(typeof(T));
                    var property = Expression.Property(instance, propertyInfo);
                    var convert = Expression.TypeAs(property, typeof(object));
                    dictionary.Add(propertyInfo.Name, Expression.Lambda<Func<T, object>>(convert, instance).Compile());
                }
                return dictionary;
            }, LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<Dictionary<string, Action<T, object>>> AccessSetPropertys = new Lazy<Dictionary<string, Action<T, object>>>(
           () =>
           {
               var pr = typeof(T).GetProperties().Where(a => !a.IsDefined(typeof(NotInitializeAttribute),true));
               var dictionary = new Dictionary<string, Action<T, object>>();

               foreach (var propertyInfo in pr)
               {
                   var instance = Expression.Parameter(typeof(T));
                   var argument = Expression.Parameter(typeof(object));
                   var setterCall = Expression.Call(instance, propertyInfo.GetSetMethod(), Expression.Convert(argument, propertyInfo.PropertyType));
                   var c = Expression.Lambda<Action<T, object>>(setterCall, instance, argument).Compile();
                   dictionary.Add(propertyInfo.Name, c);
               }
               return dictionary;
           }, LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<Dictionary<string, Func<T, object>>> AccessGetFields = new Lazy<Dictionary<string, Func<T, object>>>(
           () =>
           {
               var pr = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
               var dictionary = new Dictionary<string, Func<T, object>>();
               foreach (var fieldInfo in pr)
               {
                   var param = Expression.Parameter(typeof(T));
                   var member = Expression.Field(param, fieldInfo.Name);
                   var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(member, typeof(Object)), param);
                   var c = lambda.Compile();
                   dictionary.Add(fieldInfo.Name, c);
               }
               return dictionary;
           }, LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<Dictionary<string, Action<T, object>>> AccessSetFields = new Lazy<Dictionary<string, Action<T, object>>>(
          () =>
          {
              var pr = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).
                  Where(a => !a.GetCustomAttributes(typeof(NotInitializeAttribute), true).Any()); ;
              var dictionary = new Dictionary<string, Action<T, object>>();
              foreach (var fieldInfo in pr)
              {
                  var param = Expression.Parameter(typeof(T));
                  var value = Expression.Parameter(typeof(object));
                  var member = Expression.Field(param, fieldInfo.Name);
                  var assignExp = Expression.Assign(member, Expression.Convert(value, fieldInfo.FieldType));

                  var c = Expression.Lambda<Action<T, object>>(assignExp, param, value).Compile();
                  dictionary.Add(fieldInfo.Name, c);
              }
              return dictionary;
          }, LazyThreadSafetyMode.PublicationOnly);

        public static Func<T, object> GetValuePropery(string propertyName)
        {
            return AccessGetPropertys.Value[propertyName];
        }

        public static Action<T, object> SetValuePropery(string propertyName)
        {
            return AccessSetPropertys.Value[propertyName];
        }

        public static Func<T, object> GetValueField(string fieldName)
        {
            return AccessGetFields.Value[fieldName];
        }

        public static Action<T, object> SetValueField(string propertyName)
        {
            return AccessSetFields.Value[propertyName];
        }

    }
}
