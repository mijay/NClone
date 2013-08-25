using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests
{
    [Ignore]
    public class Benchmark: TestBase
    {
        private class ClassWithPrivateField
        {
            private int field;

            public ClassWithPrivateField(int field)
            {
                this.field = field;
            }

            public int GetField()
            {
                return field;
            }
        }

        [Test]
        public void BenchmarkGet()
        {
            var field = typeof (ClassWithPrivateField).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Single();

            var xEntity = Expression.Parameter(typeof (ClassWithPrivateField));

            var getViaExpression = Expression.Lambda<Func<ClassWithPrivateField, int>>(
                Expression.Field(xEntity, field), xEntity)
                                             .Compile();

            var method = new DynamicMethod(
                string.Format("getMember_[{0}.{1}]_From_[{2}]", field.DeclaringType.FullName, field.Name,
                    typeof (ClassWithPrivateField).FullName),
                typeof (int), new[] { typeof (ClassWithPrivateField) },
                typeof (ClassWithPrivateField), true);
            var ilGenerator = method.GetILGenerator();

            ilGenerator.EmitLoadArgumentAddress(typeof (ClassWithPrivateField), 0);
            ilGenerator.EmitLoadFieldValue(field);
            ilGenerator.Emit(OpCodes.Ret);

            var getViaMethod = (Func<ClassWithPrivateField, int>) method.CreateDelegate(typeof (Func<ClassWithPrivateField, int>));

            var source = new ClassWithPrivateField(RandomInt());
            var timer = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++) {
                var result = getViaExpression(source);
                if (result != source.GetField())
                    throw new Exception();
            }
            timer.Stop();

            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);

            timer.Restart();
            for (var i = 0; i < 10000; i++) {
                var result = getViaMethod(source);
                if (result != source.GetField())
                    throw new Exception();
            }
            timer.Stop();

            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);
        }

        [Test]
        public void BenchmarkSet()
        {
            var field = typeof (ClassWithPrivateField).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Single();

            var xEntity = Expression.Parameter(typeof (ClassWithPrivateField));
            var xMember = Expression.Parameter(typeof (int));

            var setViaExpression = Expression.Lambda<Func<ClassWithPrivateField, int, ClassWithPrivateField>>(
                Expression.Block(
                    Expression.Assign(
                        Expression.Field(xEntity, field),
                        xMember),
                    xEntity),
                xEntity, xMember)
                                             .Compile();

            var method = new DynamicMethod(
                string.Format("setMember_[{0}.{1}]_From_[{2}]", field.DeclaringType.FullName, field.Name,
                    typeof (ClassWithPrivateField).FullName),
                typeof (ClassWithPrivateField), new[] { typeof (ClassWithPrivateField), typeof (int) },
                typeof (ClassWithPrivateField), true);
            var ilGenerator = method.GetILGenerator();

            ilGenerator.EmitLoadArgumentAddress(typeof (ClassWithPrivateField), 0);
            ilGenerator.EmitLoadArgument(1);
            ilGenerator.EmitStoreFieldValue(field);
            ilGenerator.EmitLoadArgument(0);
            ilGenerator.Emit(OpCodes.Ret);

            var setViaMethod = (Func<ClassWithPrivateField, int, ClassWithPrivateField>)
                method.CreateDelegate(typeof (Func<ClassWithPrivateField, int, ClassWithPrivateField>));

            var entity = new ClassWithPrivateField(RandomInt());
            var value = RandomInt();
            var timer = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++) {
                entity = setViaExpression(entity, value);
                if (entity.GetField() != value)
                    throw new Exception();
            }
            timer.Stop();

            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);

            timer.Restart();
            for (var i = 0; i < 10000; i++) {
                entity = setViaMethod(entity, value);
                if (entity.GetField() != value)
                    throw new Exception();
            }
            timer.Stop();

            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);
        }

        [Test]
        public void BenchmarkReflection()
        {
            var method = typeof (Benchmark).GetMethod("GenericMethod", BindingFlags.Instance | BindingFlags.NonPublic);

            var timer = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++)
                method.MakeGenericMethod(typeof (string)).Invoke(this, new object[] { "test" });
            timer.Stop();

            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);
        }

        private void GenericMethod<T>(T argument) { }
    }
}