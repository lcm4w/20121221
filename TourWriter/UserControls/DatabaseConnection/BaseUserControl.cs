using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConnection
{
    public class BaseUserControl : UserControl
    {
        public ConnectionInfo ConnectionInfo { get; set; }
        protected new MainForm ParentForm { get { return Parent.Parent as MainForm; } }
        protected Button BackButton { get { return ParentForm.BackButton; } }
        protected Button NextButton { get { return ParentForm.NextButton; } }
        protected Button CancelButton { get { return ParentForm.EndButton; } }
        internal UserControl NextControl { get { return ParentForm.NextControl; } set { ParentForm.NextControl = value; } }
        internal UserControl PrevControl { get { return ParentForm.PrevControl; } set { ParentForm.PrevControl = value; } }
        internal void GoBack() { ParentForm.GoBack(); }
        internal void GoNext() { ParentForm.GoNext(); }
        internal void GoCancel() { ParentForm.GoCancel(); }
        internal void GoTo(UserControl control) { ParentForm.GoTo(control); }
    }
}
