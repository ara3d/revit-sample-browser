// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
namespace Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS
{
    public class GenericStructuralConnectionOps
    {
        public static Result CreateGenericStructuralConnection(UIDocument activeDoc, ref string message)
        {
            var ret = Result.Succeeded;

            var ids = SelectionHelper.SelectConnectionElements(activeDoc);
            if (ids.Count() > 0)
            {
                using (var tran = new Transaction(activeDoc.Document, "Create generic structural connection"))
                {
                    tran.Start();

                    StructuralConnectionHandler.CreateGenericConnection(activeDoc.Document, ids);

                    var ts = tran.Commit();
                    if (ts != TransactionStatus.Committed)
                    {
                        message = "Failed to commit the current transaction !";
                        ret = Result.Failed;
                    }
                }
            }
            else
            {
                message = "There is no element selected !";
                ret = Result.Failed;
            }

            return ret;
        }

        public static Result DeleteGenericStructuralConnection(UIDocument activeDoc, ref string message)
        {
            var ret = Result.Succeeded;

            var conn = SelectionHelper.SelectConnection(activeDoc);

            if (conn != null)
            {
                using (var tran = new Transaction(activeDoc.Document, "Delete generic structural connection"))
                {
                    tran.Start();

                    activeDoc.Document.Delete(conn.Id);

                    var ts = tran.Commit();
                    if (ts != TransactionStatus.Committed)
                    {
                        message = "Failed to commit the current transaction !";
                        return Result.Failed;
                    }
                }
            }
            else
            {
                message = "There is no connection selected !";
                ret = Result.Failed;
            }

            return ret;
        }

        public static Result ReadGenericStructuralConnection(UIDocument activeDoc, ref string message)
        {
            var ret = Result.Succeeded;

            var conn = SelectionHelper.SelectConnection(activeDoc);
            if (conn != null)
            {
                var msgBuilder = new StringBuilder();
                msgBuilder.AppendLine($"Connection id : {conn.Id}");

                if (activeDoc.Document.GetElement(conn.GetTypeId()) is StructuralConnectionHandlerType connType)
                    msgBuilder.AppendLine($"Type : {connType.Name}");

                msgBuilder.Append("Connected elements ids : ");
                var connectedElemIds = conn.GetConnectedElementIds();
                foreach (var connId in connectedElemIds)
                {
                    msgBuilder.Append(connId);
                    if (connId != connectedElemIds.Last())
                        msgBuilder.Append(", ");
                }

                TaskDialog.Show("Info", msgBuilder.ToString());
            }
            else
            {
                message = "There is no connection selected !";
                ret = Result.Failed;
            }

            return ret;
        }

        public static Result UpdateGenericStructuralConnection(UIDocument activeDoc, ref string message)
        {
            var ret = Result.Succeeded;

            var conn = SelectionHelper.SelectConnection(activeDoc);
            if (conn != null)
            {
                var ids = SelectionHelper.SelectConnectionElements(activeDoc);
                if (ids.Count() > 0)
                    using (var transaction =
                           new Transaction(activeDoc.Document, "Update generic structural connection"))
                    {
                        transaction.Start();

                        conn.AddElementIds(ids);

                        var ts = transaction.Commit();
                        if (ts != TransactionStatus.Committed)
                        {
                            message = "Failed to commit the current transaction !";
                            ret = Result.Failed;
                        }
                    }
                else
                    message = "There are no connection input elements selected !";
            }

            return ret;
        }
    }
}
