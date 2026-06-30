using Autodesk.Revit.DB;

namespace AdnRme
{
    class PanelTreeNodeHelper
    {
        public PanelTreeNodeHelper(Element e)
        {
            Element = e;
        }

        public PanelTreeNodeHelper(Element e, System.Windows.Forms.TreeNode tn)
        {
            Element = e;
            TreeNode = tn;
        }

        public System.Windows.Forms.TreeNode TreeNode
        {
            get;
            //set { _tn = value; }
        }

        public Element Element
        {
            get;
            //set { _element = value; }
        }

        public override string ToString()
        {
            return Element.Name;
        }

        public static int CompareByName(PanelTreeNodeHelper x, PanelTreeNodeHelper y)
        {
            return string.Compare(x.ToString(), y.ToString());
        }
    }
}
