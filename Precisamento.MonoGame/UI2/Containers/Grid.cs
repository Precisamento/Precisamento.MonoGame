using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Containers
{
    public class Grid
    {
        public static readonly AttachedProperty<int> ColumnProperty =
            AttachedProperty.Create(typeof(Grid), "Column", 0, AttachedPropertyOption.AffectsArrange);

        public static readonly AttachedProperty<int> RowProperty =
            AttachedProperty.Create(typeof(Grid), "Column", 0, AttachedPropertyOption.AffectsArrange);

        public static readonly AttachedProperty<int> ColumnSpanProperty =
            AttachedProperty.Create(typeof(Grid), "Column", 1, AttachedPropertyOption.AffectsArrange);

        public static readonly AttachedProperty<int> RowSpanProperty =
            AttachedProperty.Create(typeof(Grid), "Column", 1, AttachedPropertyOption.AffectsArrange);

        public static int GetColumn(Control control)
        {
            return ColumnProperty.GetValue(control);
        }

        public static void SetColumn(Control control, int column)
        {
            ColumnProperty.SetValue(control, column);
        }

        public static int GetRow(Control control)
        {
            return RowProperty.GetValue(control);
        }

        public static void SetRow(Control control, int row)
        {
            RowProperty.SetValue(control, row);
        }

        public static int GetColumnSpan(Control control)
        {
            return ColumnSpanProperty.GetValue(control);
        }

        public static void SetColumnSpan(Control control, int columnSpan)
        {
            ColumnSpanProperty.SetValue(control, columnSpan);
        }

        public static int GetRowSpan(Control control)
        {
            return RowSpanProperty.GetValue(control);
        }

        public static void SetRowSpan(Control control, int rowSpan)
        {
            RowSpanProperty.SetValue(control, rowSpan);
        }
    }
}
