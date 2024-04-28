using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CGame
{
    // [TypeConverter(typeof(ConverterTestConverter))]
    // public class ConverterTest
    // {
    //     public int a;
    //     public ConverterTest ab;
    //     public ConverterTest ABC { get; set; }
    //
    //     public ConverterTest()
    //     {
    //     }
    //
    //     public ConverterTest(int a)
    //     {
    //         this.a = a;
    //         ab = new ConverterTest();
    //     }
    //
    //     public override string ToString()
    //     {
    //         return $"a {a} ab:a {ab.a} ABC:a {ABC.a}";
    //     }
    // }
    //
    // public class ConverterTestConverter : ValueToStringConverter<ConverterTest>
    // {
    //     // string转CsvTest
    //     protected override ConverterTest ConvertFrom(string csvData) => new(int.Parse(csvData));
    //
    //     // CsvTest转string
    //     protected override string ConvertTo(ConverterTest value) => value.a.ToString();
    // }

    public abstract class ValueToStringConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(T) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string s ? ConvertFrom(s) : base.ConvertFrom(context, culture, value);
        }

        protected abstract T ConvertFrom(string csvData);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string) ? ConvertTo((T)value) : base.ConvertTo(context, culture, value, destinationType);
        }

        protected abstract string ConvertTo(T value);
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AnotherNameAttribute : Attribute
    {
        public string Name { get; }
    
        public AnotherNameAttribute(string name)
        {
            Name = name;
        }
    }
    
    public static class ValueToStringUtility<TValue>
    {
        // ValueToStringUtility<CsvData>.ToStringFunc(new CsvData(0,"abc",False));
        public static Func<TValue, List<string>> ValueToString { get; private set; }

        // ValueToStringUtility<CsvData>.ToValueFunc(new List<string>() { "0", "abc", "False" });
        public static Func<List<string>, TValue> StringToValue { get; private set; }
        
        // ValueToStringUtility<CsvData>.GetAllMemberName();
        public static Func<List<string>> GetAllMemberName { get; private set; }

        private static readonly Type CurrentType = typeof(TValue);

        static ValueToStringUtility()
        {
            GetAllMemberNameInit();
            ValueToStringInit();
            StringToValueInit();
        }

        private static void ValueToStringInit()
        {
            /*
             *  var result = new List<string>();
             *
             *  result.Add(value.num);
             *  result.Add(value.name);
             *  result.Add(value.IsActive);
             *  ...
             *
             *  return result;
             */
            var value = Expression.Parameter(CurrentType, "value");
            var result = Expression.Variable(typeof(List<string>), "result");
            var addMethod = typeof(List<string>).GetMethod("Add", new[] { typeof(string) })!;
            var expressions = new List<Expression> { Expression.Assign(result, Expression.New(typeof(List<string>))) };

            foreach (var fieldInfo in CurrentType.GetFields(~BindingFlags.Static).Where(field => !Attribute.IsDefined(field, typeof(ObsoleteAttribute))))
            {
                if (fieldInfo.Name.Contains("k__BackingField"))
                    continue;

                if (fieldInfo.FieldType == typeof(string))
                {
                    expressions.Add(Expression.Call(result, addMethod, Expression.Field(value, fieldInfo)));
                    continue;
                }
                
                // TypeDescriptor.GetConverter(fieldInfo.FieldType).ConvertToString((object)value.field);
                var converter = Expression.Call(
                    null,
                    typeof(TypeDescriptor).GetMethod("GetConverter", new[] { typeof(Type) })!,
                    Expression.Constant(fieldInfo.FieldType)
                );
                var convertToString = Expression.Call(
                    converter,
                    "ConvertToString",
                    null,
                    Expression.Convert(Expression.Field(value, fieldInfo), typeof(object))
                );
                expressions.Add(Expression.Call(result, addMethod, convertToString));
            }

            foreach (var propertyInfo in CurrentType.GetProperties(~BindingFlags.Static).Where(property => !Attribute.IsDefined(property, typeof(ObsoleteAttribute))))
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;
                
                if (propertyInfo.PropertyType == typeof(string))
                {
                    expressions.Add(Expression.Call(result, addMethod, Expression.Property(value, propertyInfo)));
                    continue;
                }
                
                // TypeDescriptor.GetConverter(propertyInfo.PropertyType).ConvertToString((object)value.property);
                var converter = Expression.Call(
                    null,
                    typeof(TypeDescriptor).GetMethod("GetConverter", new[] { typeof(Type) })!,
                    Expression.Constant(propertyInfo.PropertyType)
                );
                var convertToString = Expression.Call(
                    converter,
                    "ConvertToString",
                    null,
                    Expression.Convert(Expression.Property(value, propertyInfo), typeof(object))
                );
                expressions.Add(Expression.Call(result, addMethod, convertToString));
            }

            expressions.Add(result);

            var block = Expression.Block(new[] { result }, expressions);
            
            ValueToString = Expression.Lambda<Func<TValue, List<string>>>(block, value).Compile();
        }

        private static void StringToValueInit()
        {
            /*
             *  var result = new T
             *  {
             *      result.num = value[0];
             *      result.name = value[1];
             *      result.IsActive = value[2];
             *      ...
             *  };
             *
             *  return result;
             */
            var value = Expression.Parameter(typeof(List<string>), "value");
            var memberBindingList = new List<MemberBinding>();

            var index = 0;
            foreach (var fieldInfo in CurrentType.GetFields(~BindingFlags.Static).Where(field => !Attribute.IsDefined(field, typeof(ObsoleteAttribute))))
            {
                if (fieldInfo.Name.Contains("k__BackingField"))
                    continue;

                var makeIndex = Expression.MakeIndex(value, typeof(List<string>).GetProperty("Item"), new[] { Expression.Constant(index) });
                index++;
                
                if (fieldInfo.FieldType == typeof(string))
                {
                    memberBindingList.Add(Expression.Bind(fieldInfo, makeIndex));
                    continue;
                }
                
                // TypeDescriptor.GetConverter(fieldInfo.FieldType).ConvertFromString(value[index]);
                var converter = Expression.Call(
                    null,
                    typeof(TypeDescriptor).GetMethod("GetConverter", new[] { typeof(Type) })!,
                    Expression.Constant(fieldInfo.FieldType)
                );
                var convertFromString = Expression.Call(
                    converter,
                    "ConvertFromString",
                    null,
                    makeIndex
                );
                memberBindingList.Add(Expression.Bind(fieldInfo, Expression.Convert(convertFromString, fieldInfo.FieldType)));
            }

            foreach (var propertyInfo in CurrentType.GetProperties(~BindingFlags.Static).Where(property => !Attribute.IsDefined(property, typeof(ObsoleteAttribute))))
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;
                
                var makeIndex = Expression.MakeIndex(value, typeof(List<string>).GetProperty("Item"), new[] { Expression.Constant(index) });
                index++;
                
                if (propertyInfo.PropertyType == typeof(string))
                {
                    memberBindingList.Add(Expression.Bind(propertyInfo, makeIndex));
                    continue;
                }
                
                // TypeDescriptor.GetConverter(propertyInfo.PropertyType).ConvertFromString(value[index]);
                var converter = Expression.Call(
                    null,
                    typeof(TypeDescriptor).GetMethod("GetConverter", new[] { typeof(Type) })!,
                    Expression.Constant(propertyInfo.PropertyType)
                );
                var convertFromString = Expression.Call(
                    converter,
                    "ConvertFromString",
                    null,
                    makeIndex
                );
                memberBindingList.Add(Expression.Bind(propertyInfo, Expression.Convert(convertFromString, propertyInfo.PropertyType)));
            }

            var result = Expression.MemberInit(Expression.New(CurrentType), memberBindingList.ToArray());

            StringToValue = Expression.Lambda<Func<List<string>, TValue>>(result, value).Compile();
        }

        private static void GetAllMemberNameInit()
        {
            /*
             *  var result = new List<string>();
             *
             *  result.Add(num);
             *  result.Add(name);
             *  result.Add(IsActive);
             *  ...
             *
             *  return result;
             */
            var result = Expression.Variable(typeof(List<string>), "allMemberNameResult");
            var addMethod = typeof(List<string>).GetMethod("Add", new[] { typeof(string) })!;
            var expressions = new List<Expression> { Expression.Assign(result, Expression.New(typeof(List<string>))) };

            foreach (var fieldInfo in CurrentType.GetFields(~BindingFlags.Static).Where(field => !Attribute.IsDefined(field, typeof(ObsoleteAttribute))))
            {
                if (fieldInfo.Name.Contains("k__BackingField"))
                    continue;
                
                var name = fieldInfo.Name;
                var anotherNameAttribute = fieldInfo.GetCustomAttribute<AnotherNameAttribute>();
                if (anotherNameAttribute != null && !string.IsNullOrWhiteSpace(anotherNameAttribute.Name))
                    name = anotherNameAttribute.Name;
                expressions.Add(Expression.Call(result, addMethod, Expression.Constant(name)));
            }

            foreach (var propertyInfo in CurrentType.GetProperties(~BindingFlags.Static).Where(property => !Attribute.IsDefined(property, typeof(ObsoleteAttribute))))
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;
                
                var name = propertyInfo.Name;
                var anotherNameAttribute = propertyInfo.GetCustomAttribute<AnotherNameAttribute>();
                if (anotherNameAttribute != null && !string.IsNullOrWhiteSpace(anotherNameAttribute.Name))
                    name = anotherNameAttribute.Name;
                expressions.Add(Expression.Call(result, addMethod, Expression.Constant(name)));
            }
            
            expressions.Add(result);

            var block = Expression.Block(new[] { result }, expressions);

            GetAllMemberName = Expression.Lambda<Func<List<string>>>(block).Compile();
        }
    }
}