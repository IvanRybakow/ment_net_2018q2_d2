using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Task_2
{
    [TestClass]
    public class MapperTest
    {
        public class Mapper<TInput, TOutput>
        {
            private readonly Func<TInput, TOutput> _mapFunction;
            internal Mapper(Func<TInput, TOutput> func)
            {
                _mapFunction = func;
            }

            public TOutput Map(TInput source)
            {
                return _mapFunction(source);
            }
        }

        public class MappersGenerator
        {
            public static Mapper<TInput, TOutput> GetMapper<TInput, TOutput>()
            {
                var inputType = typeof(TInput);
                var outputType = typeof(TOutput);
                var param = Expression.Parameter(typeof(TInput));
                
                var inputProps = inputType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.CanRead);
                var inputFields = inputType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                var outputProps = outputType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.CanWrite);
                var outputFields = outputType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                var propBindings = inputProps.Join(outputProps, 
                    inputProp => inputProp.Name, 
                    outputProp => outputProp.Name,
                    (inputProp, outputProp) => new {inputProp, outputProp})
                    .Where(a => a.inputProp.PropertyType == a.outputProp.PropertyType)
                    .Select(a => Expression.Bind(a.outputProp, Expression.Property(param, a.inputProp)));

                var fieldBindings = inputFields.Join(outputFields,
                    inputField => inputField.Name,
                    outputField => outputField.Name,
                    (inputField, outputField) => new { inputField, outputField })
                    .Where(a => a.inputField.FieldType == a.outputField.FieldType)
                    .Select(a => Expression.Bind(a.outputField, Expression.Field(param, a.inputField)));

                var allBindings = propBindings.Concat(fieldBindings);

                var mapFunction = Expression.Lambda<Func<TInput, TOutput>>(
                    Expression.MemberInit(Expression.New(typeof(TOutput)), allBindings),
                    param
                );
                return new Mapper<TInput, TOutput>(mapFunction.Compile());
            }
        }

        public class Foo
        {
            public string Field1;
            public string Name { get; set; }
            public int Age { get; set; }
            public string Other { get; set; }

        }
        public class Bar
        {
            public string Field1;
            public int Name { get; set; }
            public int Age { get; set; }
            public string Other2 { get; set; }

        }

        [TestMethod]
        public void MapperTestMethod()
        {
            var mapper = MappersGenerator.GetMapper<Foo, Bar>();
            var barVariable = mapper.Map(new Foo { Age = 18, Name = "Joh Dow", Other = "Will not be mapped", Field1 = "dwdf" });
            Console.WriteLine($"{barVariable.Name}, {barVariable.Age}, {barVariable.Other2}, {barVariable.Field1}");
        }
    }
}
