
namespace TourWriter.Modules.Dashboard
{
    public partial class Dashboard : ModuleBase
    {
        public Dashboard()
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
            return "Dashboard";
        }
        #endregion
    }
}
