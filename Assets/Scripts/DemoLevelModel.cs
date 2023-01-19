using Crosswork.Core;
using System;

namespace Crosswork.Demo
{
    public class DemoLevelModel : IBoardModel, IWritableBoardModel
    {
        public class DemoCellModel : ICellModel
        {
            public IElementModel[] Elements
            { get; set; }
        }

        public int Width
        { get { return Cells.GetLength(1); } }
        public int Height
        { get { return Cells.GetLength(0); } }

        public DemoCellModel[,] Cells
        { get; set; }

        public void SetCells(int width, int height)
        {
            Cells = new DemoCellModel[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cells[y, x] = new DemoCellModel();
                }
            }
        }

        public void SetCells(Cell[,] cells)
        {
            var height = cells.GetLength(0);
            var width = cells.GetLength(1);

            Cells = new DemoCellModel[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (cells[y, x].Active)
                    {
                        Cells[y, x] = new DemoCellModel();
                    }
                }
            }
        }

        public void AddElement(int x, int y, IElementModel element)
        {
            var cell = Cells[y, x];
            if (cell.Elements == null)
            {
                cell.Elements = new IElementModel[1]
                {
                    element
                };
            }
            else
            {
                var newElements = new IElementModel[cell.Elements.Length + 1];
                Array.Copy(cell.Elements, newElements, cell.Elements.Length);
                newElements[newElements.Length - 1] = element;
                cell.Elements = newElements;
            }
        }

        public void SetElements(int x, int y, IElementModel[] elements, int size)
        {
            var cell = Cells[y, x];
            if (cell.Elements == null || cell.Elements.Length != size)
            {
                cell.Elements = new IElementModel[size];
            }

            Array.Copy(elements, 0, Cells[y, x].Elements, 0, size);
        }

        public IElementModel[] GetElements(int x, int y)
        {
            return Cells[y, x].Elements;
        }

        public bool TryGetCellModel(int x, int y, out ICellModel model)
        {
            model = Cells[y, x];
            return model != null;
        }
    }
}
