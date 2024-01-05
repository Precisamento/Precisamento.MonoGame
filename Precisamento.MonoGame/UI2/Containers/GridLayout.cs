using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Containers
{
    public class GridLayout : IContainerLayout
    {
        private List<int> _measureColumnWidths = new List<int>();
        private List<int> _measureRowHeights = new List<int>();
        private List<Control> _visibleControls = new List<Control>();
        private List<Control>[,]? _controlsByGridPosition;
        private Point _actualSize;

        public int ColumnSpacing { get; set; }
        public int RowSpacing { get; set; }
        public Proportion DefaultColumnProportion { get; set; } = Proportion.GridDefault;
        public Proportion DefaultRowProportion { get; set; } = Proportion.GridDefault;

        public ObservableCollection<Proportion> ColumnProportions { get; } = new();
        public ObservableCollection<Proportion> RowProportions { get; } = new();

        public List<int> GridLinesX { get; } = new();
        public List<int> GridLinesY { get; } = new();
        public List<int> ColumnWidths { get; } = new();
        public List<int> RowHeights { get; } = new();
        public List<int> CellLocationsX { get; } = new();
        public List<int> CellLocationsY { get; } = new();

        public Proportion GetColumnProportion(int col)
        {
            if (col < 0 || col >= ColumnProportions.Count)
                return DefaultColumnProportion;

            return ColumnProportions[col];
        }

        public Proportion GetRowProportion(int row)
        {
            if (row < 0 || row >= RowProportions.Count)
                return DefaultRowProportion;

            return RowProportions[row];
        }

        public Point GetGridPosition(Control control)
        {
            return new Point(Grid.GetColumn(control), Grid.GetRow(control));
        }

        private void LayoutProcessFixedPart()
        {
            var size = 0;

            for(var i = 0; i < _measureColumnWidths.Count; i++)
            {
                var prop = GetColumnProportion(i);
                if (prop.Type != ProportionType.Part)
                    continue;

                if (_measureColumnWidths[i] > size)
                {
                    size = _measureColumnWidths[i];
                }
            }

            for (var i = 0; i < _measureColumnWidths.Count; i++)
            {
                var prop = GetColumnProportion(i);
                if (prop.Type != ProportionType.Part)
                    continue;

                _measureColumnWidths[i] = (int)(size * prop.Value);
            }

            size = 0;

            for (var i = 0; i < _measureRowHeights.Count; i++)
            {
                var prop = GetRowProportion(i);
                if (prop.Type != ProportionType.Part)
                    continue;

                if (_measureRowHeights[i] > size)
                {
                    size = _measureRowHeights[i];
                }
            }

            for (var i = 0; i < _measureRowHeights.Count; i++)
            {
                var prop = GetRowProportion(i);
                if (prop.Type != ProportionType.Part)
                    continue;

                _measureRowHeights[i] = (int)(size * prop.Value);
            }
        }

        public Point Measure(IEnumerable<Control> controls, Point availableSize)
        {
            var rows = 0;
            var columns = 0;

            _visibleControls.Clear();

            foreach (var child in controls)
            {
                if (child.Visible)
                {
                    _visibleControls.Add(child);

                    var gridPosition = GetGridPosition(child);
                    var c = gridPosition.X + Math.Max(Grid.GetColumnSpan(child), 1);
                    if (c > columns)
                        columns = c;

                    var r = gridPosition.Y + Math.Max(Grid.GetRowSpan(child), 1);
                    if (r > rows)
                        rows = r;
                }
            }

            if (ColumnProportions.Count > columns)
            {
                columns = ColumnProportions.Count;
            }

            if (RowProportions.Count > rows)
            {
                rows = RowProportions.Count;
            }

            _measureColumnWidths.Clear();
            for(var i = 0; i < columns; i++)
            {
                _measureColumnWidths.Add(0);
            }

            _measureRowHeights.Clear();
            for (var i = 0; i < rows; i++)
            {
                _measureRowHeights.Add(0);
            }

            // Put all visible widget into 2d array
            if (_controlsByGridPosition == null ||
                _controlsByGridPosition.GetLength(0) < rows ||
                _controlsByGridPosition.GetLength(1) < columns)
            {
                _controlsByGridPosition = new List<Control>[rows, columns];
            }

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    if (_controlsByGridPosition[row, column] == null)
                    {
                        _controlsByGridPosition[row, column] = new List<Control>();
                    }

                    _controlsByGridPosition[row, column].Clear();
                }
            }

            foreach(var control in _visibleControls)
            {
                _controlsByGridPosition[Grid.GetRow(control), Grid.GetColumn(control)].Add(control);
            }

            availableSize.X -= (_measureColumnWidths.Count - 1) * ColumnSpacing;
            availableSize.Y -= (_measureRowHeights.Count - 1) * RowSpacing;

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    var rowProporton = GetRowProportion(row);
                    var columnProportion = GetColumnProportion(column);

                    if (columnProportion.Type == ProportionType.Pixels)
                    {
                        _measureColumnWidths[column] = (int)columnProportion.Value;
                    }

                    if (rowProporton.Type == ProportionType.Pixels)
                    {
                        _measureRowHeights[row] = (int)rowProporton.Value;
                    }

                    var controlsAtPosition = _controlsByGridPosition[row, column];
                    foreach(var control in  controlsAtPosition)
                    {
                        var gridPosition = GetGridPosition(control);

                        var measuredSize = Point.Zero;
                        if (rowProporton.Type != ProportionType.Pixels ||
                            columnProportion.Type != ProportionType.Pixels)
                        {
                            measuredSize = control.Measure(availableSize);
                        }

                        if (Grid.GetColumnSpan(control) != 1)
                        {
                            measuredSize.X = 0;
                        }

                        if (Grid.GetRowSpan(control) != 1)
                        {
                            measuredSize.Y = 0;
                        }

                        if (measuredSize.X > _measureColumnWidths[column] && columnProportion.Type != ProportionType.Pixels)
                        {
                            _measureColumnWidths[column] = measuredSize.X;
                        }

                        if (measuredSize.Y > _measureRowHeights[column] && columnProportion.Type != ProportionType.Pixels)
                        {
                            _measureRowHeights[row] = measuredSize.Y;
                        }
                    }
                }
            }

            // #181: All Part proportions must have maximum size
            LayoutProcessFixedPart();

            var result = Point.Zero;
            for (var i = 0; i < _measureColumnWidths.Count; i++)
            {
                var width = _measureColumnWidths[i];
                result.X += width;
                if (i < _measureColumnWidths.Count - 1)
                    result.X += ColumnSpacing;
            }

            for (var i = 0; i < _measureRowHeights.Count; i++)
            {
                var height = _measureRowHeights[i];
                result.Y += height;
                if (i < _measureRowHeights.Count - 1)
                    result.Y += RowSpacing;
            }

            return result;
        }

        public void Arrange(IEnumerable<Control> controls, Rectangle bounds) => throw new NotImplementedException();
    }
}
