using System.Windows.Forms;

namespace TourWriter.UserControls
{
    public class MyToolStrip : ToolStrip
    {
        public bool DisableAllMenuItems
        {
            get
            {
                foreach (ToolStripItem item in Items)
                    if (item.Enabled)
                        return true;
                return false;
            }
            set
            {
                foreach (ToolStripItem item in Items)
                    item.Enabled = value;
            }
        }

        protected override DockStyle DefaultDock
        {
            get
            {
                return DockStyle.None;
            }
        }
    }
}
