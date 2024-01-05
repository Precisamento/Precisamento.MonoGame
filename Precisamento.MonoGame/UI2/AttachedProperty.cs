using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public interface INotifyAttachedPropertyChanged
    {
        void OnAttachedPropertyChanged(AttachedProperty property);
    }

    public enum AttachedPropertyOption
    {
        None,
        AffectsArrange,
        AffectsMeasure
    }

    public abstract class AttachedProperty
    {
        private static Dictionary<int, AttachedProperty> _properties = new();
        private static Dictionary<Type, AttachedProperty[]> _propertiesByType = new();
        private static object _mutex = new object();

        public Type OwnerType { get; }
        public int Id { get; }
        public string Name { get; }
        public AttachedPropertyOption Options { get; }
        public abstract Type PropertyType { get; }
        public abstract object? DefaultObject { get; }

        public AttachedProperty(int id, string name, Type owner, AttachedPropertyOption options)
        {
            Name = name;
            OwnerType = owner;
            Id = id;
            Options = options;
        }

        public abstract object? GetValueObject(Control control);
        public abstract void SetValueObject(Control control, object? value);

        public static AttachedProperty<T> Create<T>(Type type, string name, T defaultValue, AttachedPropertyOption option)
        {
            lock (_mutex)
            {
                var result = new AttachedProperty<T>(_properties.Count, name, type, defaultValue, option);
                _properties[result.Id] = result;

                return result;
            }
        }

        private static AttachedProperty[] GetPropertiesOfType(Type type)
        {
            if (_propertiesByType.TryGetValue(type, out var result))
                return result;

            lock (_mutex)
            {
                if (_propertiesByType.TryGetValue(type, out result))
                    return result;

                var properties = new List<AttachedProperty>();
                var currentType = type;

                while (currentType != null && type != typeof(object))
                {
                    RuntimeHelpers.RunClassConstructor(currentType.TypeHandle);
                    foreach (var pair in _properties)
                    {
                        if (pair.Value.OwnerType == currentType)
                            properties.Add(pair.Value);
                    }

                    currentType = currentType.BaseType;
                }

                result = properties.ToArray();
                _propertiesByType[type] = result;

                return result;
            }
        }
    }

    public class AttachedProperty<T> : AttachedProperty
    {
        public T DefaultValue { get; }

        public override object? DefaultObject => DefaultValue;
        public override Type PropertyType => typeof(T);

        public AttachedProperty(int id, string name, Type owner, T defaultValue, AttachedPropertyOption options)
            : base(id, name, owner, options)
        {
            DefaultValue = defaultValue;
        }

        public T GetValue(Control control)
        {
            if (control.AttachedPropertyValues.TryGetValue(Id, out var value)) 
            {
                return (T)value;
            }

            return DefaultValue;
        }

        public void SetValue(Control control, T value)
        {
            if (GetValue(control)?.Equals(value) ?? false)
                return;

            control.AttachedPropertyValues[Id] = value;
            switch(Options)
            {
                case AttachedPropertyOption.AffectsMeasure:
                    control.InvalidateMeasure();
                    break;
                case AttachedPropertyOption.AffectsArrange:
                    control.InvalidateArrange();
                    break;
            }

            control.OnAttachedPropertyChanged(this);
        }

        public override object? GetValueObject(Control control) => GetValue(control);
        public override void SetValueObject(Control control, object? value) => SetValue(control, (T)value);
    }
}
