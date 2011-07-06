
namespace TourWriter.Modules.Accounting
{
    public partial class AccountingForm : ModuleBase
    {
        public AccountingForm()
        {
            InitializeComponent();
        }
        
        #region Override methods
        protected override bool IsDataDirty()
        {
            return false;
        }

        protected override string GetDisplayName()
        {
            return "Accounting Export";
        }
        #endregion
    }
}
