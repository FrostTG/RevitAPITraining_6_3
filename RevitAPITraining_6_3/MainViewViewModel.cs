using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_6_3
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public int ElementCount { get; set; }

        public List<FamilySymbol> ElementType { get; } = new List<FamilySymbol>();
        public DelegateCommand SaveCommand { get; }
        public FamilySymbol SelectedElementTypes { get; set; }
        public List<XYZ> Points { get; set; } = new List<XYZ>();

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            ElementCount = 0;
            Points = SelectionUtils.GetPoints(_commandData, "Выберите точки между которыми требуется вставить элементы", ObjectSnapTypes.Endpoints, 2);
            ElementType = FamilySymbolUtils.GetFamilySymbols(_commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<XYZ> PointEl = new List<XYZ>();
            double dX = (Points[1].X - Points[0].X) / (ElementCount - 1);
            double dY = (Points[1].Y - Points[0].Y) / (ElementCount - 1);
            for (int i = 0; i <= ElementCount - 1; i++)
            {
                XYZ xYZ = new XYZ(Points[0].X + dX * i, Points[0].Y + dY * i, 0);
                PointEl.Add(xYZ);
            }
            foreach (var iPoint in PointEl)
            {
                FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedElementTypes, iPoint, doc.ActiveView.GenLevel);
            }

            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
