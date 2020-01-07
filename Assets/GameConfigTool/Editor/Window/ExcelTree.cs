
using UnityEditor.IMGUI.Controls;

namespace GCT.Window
{
    internal class ExcelTreeItem : TreeViewItem
    {

    }

    internal class ExcelTreeView : TreeView
    {
        public ExcelTreeView(TreeViewState state) : base(state) { }

        protected override TreeViewItem BuildRoot()
        {
            return new ExcelTreeItem();
        }
    }
}