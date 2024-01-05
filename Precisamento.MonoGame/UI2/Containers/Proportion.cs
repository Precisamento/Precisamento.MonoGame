using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Containers
{
    public enum ProportionType
    {
        Auto,
        Part,
        Fill,
        Pixels
    }

    public class Proportion
    {
        public static Proportion Auto { get; } = new Proportion(ProportionType.Auto);
        public static Proportion Fill { get; } = new Proportion(ProportionType.Fill);

        public static Proportion GridDefault { get; } = new Proportion(ProportionType.Part, 1);
        public static Proportion StackPanelDefault { get; } = new Proportion(ProportionType.Auto);

        private ProportionType _type;
        private float _value = 1f;

        public ProportionType Type
        {
            get => _type;
            set
            {
                if (value == _type)
                    return;
                _type = value;
                FireChanged();
            }
        }

        public float Value
        {
            get => _value;
            set
            {
                if (value == _value)
                    return;
                _value = value;
                FireChanged();
            }
        }

        public event EventHandler? Changed;

        public Proportion()
        {
        }

        public Proportion(ProportionType type) 
        {
            _type = type;
        }

        public Proportion(ProportionType type, float value)
        {
            _type = type;
            _value = value;
        }

        public override string ToString()
        {
            return _type switch
            {
                ProportionType.Auto or ProportionType.Fill => _type.ToString(),
                ProportionType.Part => $"{_type} {_value:0.00}",
                _ => $"{_type} {(int)_value}",
            };
        }

        private void FireChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
