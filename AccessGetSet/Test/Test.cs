using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace AccessGetSet.Test
{
    static class EgualsTestIon
    {
        public static bool Eguals(TestIon ion1, TestIon ion2)
        {
            var r1 = new { ion1.P1, ion1.P2, ion1.P3, ion1.P4 }.GetHashCode();
            var r2 = new { ion2.P1, ion2.P2, ion2.P3, ion2.P4 }.GetHashCode();
            return r1 == r2;
        }
    }

    public class TestBase
    {
            public int Id { get; set; }
    }

    public class TestIon:TestBase
    {
        public string Name2 = "dfsd";
        public string Name;

      

        public string P1 { get; set; }

        public DateTime P2 { get; set; }

        public List<int> P3 { get; set; }

        public int P4 { get; set; }

        public TestIon()
        {
        }

        public TestIon(string p1, DateTime p2, List<int> p3, int p4)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;

        }
    }


    [TestFixture]
    public class Test
    {
        private const int Iteration = 100000;
        [Test]
        public void TestEguals()
        {
            Assert.True(EgualsTestIon.Eguals(new TestIon(), new TestIon()));
        }

        [Test]
        public void TestPropery()
        {
            var names = typeof(TestIon).GetProperties().Select(a => a.Name);
            var testo1 = new TestIon("sdsd", DateTime.Now, new List<int>() { 1, 2, 3, 4 }, 34);
            var dictionary = new Dictionary<string, object>();
            foreach (var name in names)
            {
                var res = Access<TestIon>.GetValuePropery(name)(testo1);
                dictionary.Add(name, res);
            }
            var testo2 = new TestIon();
            foreach (var o in dictionary)
            {
                Access<TestIon>.SetValuePropery(o.Key)(testo2, o.Value);
            }

            Assert.True(EgualsTestIon.Eguals(testo1, testo2));
        }


        [Test]
        public void TestField()
        {
            var testo1 = new TestIon("sdsd", DateTime.Now, new List<int> { 1, 2, 3, 4 }, 34);
            var pr = typeof(TestIon).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(a => a.Name);
            var dictionary = new Dictionary<string, object>();
            foreach (var name in pr)
            {
                var r = Access<TestIon>.GetValueField(name)(testo1);
                dictionary.Add(name, r);
            }
            var testo2 = new TestIon();
            foreach (var o in dictionary)
            {
                Access<TestIon>.SetValueField(o.Key)(testo2, o.Value);
            }
            Assert.True(EgualsTestIon.Eguals(testo1, testo2));
        }
        [Test]
        public void TestSpeedReflection()
        {
            var testBase = new TestIon();
            var pr = testBase.GetType().GetProperty("Id");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < Iteration; i++)
            {
                var r = pr.GetValue(testBase, null);
            }
            stopWatch.Stop();
            Debug.WriteLine("REFLECTION: " + stopWatch.Elapsed);
            Assert.True(true);
        }
        [Test]
        public void TestSpeedCore()
        {
            var testBase = new TestIon();
           
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < Iteration; i++)
            {
                var r = testBase.Id;
            }
            stopWatch.Stop();
            Debug.WriteLine("CORE: " + stopWatch.Elapsed);
            Assert.True(true);
        }

        [Test]
        public void TestSpeedExpression()
        {
            var testBase = new TestIon();
         
            var stopWatch = new Stopwatch();
            var ss = Access<TestIon>.GetValuePropery("Id")(testBase);
            stopWatch.Start();
            
            for (var i = 0; i < Iteration; i++)
            {
                var r = Access<TestIon>.GetValuePropery("Id")(testBase);
            }
            stopWatch.Stop();
            Debug.WriteLine("EXPRESSION: " + stopWatch.Elapsed);
            Assert.True(true);
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
    }

   
}