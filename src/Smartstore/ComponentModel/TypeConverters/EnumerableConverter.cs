﻿using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using Smartstore.Collections;

namespace Smartstore.ComponentModel.TypeConverters
{
    internal class EnumerableConverter<T> : DefaultTypeConverter
    {
        private readonly Func<IEnumerable<T>, object> _activator;
        private readonly ITypeConverter _elementTypeConverter;

        public EnumerableConverter(Type sequenceType)
            : base(typeof(object))
        {
            _elementTypeConverter = TypeConverterFactory.GetConverter<T>();
            if (_elementTypeConverter == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);

            _activator = CreateSequenceActivator(sequenceType);
        }

        private static Func<IEnumerable<T>, object> CreateSequenceActivator(Type sequenceType)
        {
            // Default is IEnumerable<T>
            Func<IEnumerable<T>, object> activator = null;

            var t = sequenceType;

            if (t == typeof(IEnumerable<T>))
            {
                activator = (x) => x;
            }
            else if (t == typeof(T[]))
            {
                activator = (x) => x.ToArray();
            }
            else if (t == (typeof(IReadOnlyCollection<T>)) || t == (typeof(IReadOnlyList<T>)))
            {
                activator = (x) => x.AsReadOnly();
            }
            else if (t.IsAssignableFrom(typeof(List<T>)))
            {
                activator = (x) => x.ToList();
            }
            else if (t.IsAssignableFrom(typeof(HashSet<T>)))
            {
                if (typeof(T) == typeof(string))
                    activator = (x) => new HashSet<T>(x, (IEqualityComparer<T>)StringComparer.OrdinalIgnoreCase);
                else
                    activator = (x) => new HashSet<T>(x);
            }
            else if (t.IsAssignableFrom(typeof(Queue<T>)))
            {
                activator = (x) => new Queue<T>(x);
            }
            else if (t.IsAssignableFrom(typeof(Stack<T>)))
            {
                activator = (x) => new Stack<T>(x);
            }
            else if (t.IsAssignableFrom(typeof(LinkedList<T>)))
            {
                activator = (x) => new LinkedList<T>(x);
            }
            else if (t.IsAssignableFrom(typeof(ConcurrentBag<T>)))
            {
                activator = (x) => new ConcurrentBag<T>(x);
            }
            else if (t.IsAssignableFrom(typeof(SyncedCollection<T>)))
            {
                activator = (x) => new SyncedCollection<T>(new List<T>(x));
            }
            else if (t.IsAssignableFrom(typeof(ArraySegment<T>)))
            {
                activator = (x) => new ArraySegment<T>(x.ToArray());
            }

            if (activator == null)
            {
                throw new InvalidOperationException("'{0}' is not a valid type for enumerable conversion.".FormatInvariant(sequenceType.FullName));
            }

            return activator;
        }

        public override bool CanConvertFrom(Type type)
        {
            if (type.IsSequenceType(out var elementType))
            {
                return elementType.IsAssignableFrom(typeof(T))
                    || _elementTypeConverter.CanConvertFrom(elementType)
                    || TypeConverterFactory.GetConverter(elementType).CanConvertTo(typeof(T));
            }

            return type == typeof(string) || typeof(IConvertible).IsAssignableFrom(type);
        }

        public override bool CanConvertTo(Type type)
        {
            return type == typeof(string) && _elementTypeConverter.CanConvertTo(type);
        }

        public override object ConvertFrom(CultureInfo culture, object value)
        {
            if (value == null)
            {
                return _activator(Enumerable.Empty<T>());
            }

            var items = value as IEnumerable;

            if (value is string str)
            {
                items = GetStringArray(str);
            }
            else if (value is IConvertible c)
            {
                var result2 = (new object[] { value })
                    .Select(x => Convert.ChangeType(value, typeof(T)))
                    .Cast<T>();

                return _activator(result2);
            }

            if (items != null)
            {
                items.GetType().IsSequenceType(out var elementType);
                var elementConverter = _elementTypeConverter;
                var isOtherConverter = false;
                var isAssignable = elementType.IsAssignableFrom(typeof(T));
                if (!isAssignable && !elementConverter.CanConvertFrom(elementType))
                {
                    elementConverter = TypeConverterFactory.GetConverter(elementType);
                    isOtherConverter = true;
                }

                var result = items
                    .Cast<object>()
                    .Select(x =>
                    {
                        if (isAssignable)
                            return x;

                        return !isOtherConverter
                            ? elementConverter.ConvertFrom(culture, x)
                            : elementConverter.ConvertTo(culture, null, x, elementType);
                    })
                    .Where(x => x != null)
                    .Cast<T>();

                return _activator(result);
            }

            return base.ConvertFrom(culture, value);
        }

        public override object ConvertTo(CultureInfo culture, string format, object value, Type to)
        {
            if (to == typeof(string))
            {
                string result = string.Empty;

                if (value is IEnumerable<T> enumerable)
                {
                    // We don't use string.Join() because it doesn't support invariant culture
                    foreach (var token in enumerable)
                    {
                        var str = _elementTypeConverter.ConvertTo(culture, format, token, typeof(string));
                        result += str + ",";
                    }

                    result = result.TrimEnd(',');
                }

                return result;
            }

            return base.ConvertTo(culture, format, value, to);
        }

        protected virtual string[] GetStringArray(string input)
        {
            var result = input.SplitSafe(null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return result.ToArray();
        }
    }
}
