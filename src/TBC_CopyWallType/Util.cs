using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace BuildingCoder
{
    internal static partial class Util
    {
        private class ColumnType : EqualityComparer<ColumnType>
        {
            private readonly int[] _dim = new int[2];

            public ColumnType(int d1, int d2)
            {
                _dim = d1 > d2 ? (new[] { d1, d2 }) : (new[] { d2, d1 });
            }

            public int H => _dim[0];

            public int W => _dim[1];

            public string Name => $"{H}x{W}";

            public override bool Equals(ColumnType x, ColumnType y)
            {
                return x.H == y.H && x.W == y.W;
            }

            public override int GetHashCode(ColumnType obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        internal static Result CreateColumnTypes(Document doc)
        {
            var L1 = new[] { 100, 200, 150, 500, 400, 300, 250, 250 };
            var L2 = new[] { 200, 200, 150, 500, 400, 300, 250, 250 };

            List<ColumnType> all = new();

            for (var i = 0; i < L1.Length; ++i) all.Add(new ColumnType(L1[i], L2[i]));

            all = all.Distinct(new ColumnType(0, 0)).ToList();

            var symbols
                = new FilteredElementCollector(doc)
                    .WhereElementIsElementType()
                    .OfCategory(BuiltInCategory.OST_StructuralColumns);

            var column_name = "Concrete-Rectangular-Column";

            var existing
                = symbols.Cast<FamilySymbol>()
                    .Where(x
                        => x.FamilyName.Equals(column_name));

            if (0 == existing.Count()) return Result.Cancelled;

            List<ColumnType> AlreadyExists = new();
            List<ColumnType> ToBeMade = new();

            for (var i = 0; i < all.Count; ++i)
            {
                var fs = existing.FirstOrDefault(
                    x => x.Name == all[i].Name);

                if (fs == null)
                    ToBeMade.Add(all[i]);
                else
                    AlreadyExists.Add(all[i]);
            }

            if (ToBeMade.Count == 0) return Result.Cancelled;

            using Transaction tx = new(doc);
            if (tx.Start("Make types") == TransactionStatus.Started)
            {
                var first = existing.First();

                foreach (var ct in ToBeMade)
                {
                    var et = first.Duplicate(ct.Name);

                    et.LookupParameter("h").Set(304.8 * ct.H);
                    et.LookupParameter("b").Set(304.8 * ct.W);
                }

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
