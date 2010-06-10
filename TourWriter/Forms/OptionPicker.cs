using System;
using System.Windows.Forms;
using TourWriter.BusinessLogic;
using TourWriter.Info;
using TourWriter.Properties;
using TourWriter.Services;

// this form is used for the date kicker, to choose rates.
namespace TourWriter.Forms
{
    public partial class OptionPicker : Form
    {
        private DateTime _purchaseItemDate;
        public SupplierSet.ServiceRow SelectedService { get; private set; }
        public SupplierSet.RateRow SelectedRate { get; private set; }
        public SupplierSet.OptionRow SelectedOption { get; private set; }

        public OptionPicker(int supplierId, int optionId, DateTime purchaseItemDate)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            _purchaseItemDate = purchaseItemDate;
            LoadSupplier(supplierId, optionId);
        }

        private void LoadSupplier(int supplierId, int optionId)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                // TODO: don't need entire supplier set data here.
                serviceEditor1.SupplierSet = new Supplier().GetSupplierSet(supplierId);
            }
            catch (Exception ex)
            {
                if (ErrorHelper.IsServerConnectionError(ex))
                    App.ShowServerConnectionError();
                else throw;
            }
            finally { Cursor = Cursors.Default; }

            if (serviceEditor1.SupplierSet.Supplier.Rows.Count == 0)
            {
                App.ShowInfo("Failed to load Supplier with SupplierID: " + supplierId);
                return;
            }

            txtSupplierName.Text = serviceEditor1.SupplierSet.Supplier[0].SupplierName;

            SupplierSet.OptionRow option = serviceEditor1.SupplierSet.Option.FindByOptionID(optionId);
            SupplierSet.RateRow rate = serviceEditor1.SupplierSet.Rate.FindByRateID(option.RateID);

            serviceEditor1.OnOptionSelected += serviceEditor1_OnOptionSelected;

            // Set selected service/rate/option
            SelectedService = rate.ServiceRow;
            serviceEditor1.SetSelectedServiceRow(rate.ServiceID);
            SelectedRate = rate;
            serviceEditor1.SetSelectedRateRow(rate.ValidFrom);
            SelectedOption = option;
            serviceEditor1.SetSelectedOptionRow(option.OptionID);
        }

        private void SaveSupplier()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                // save changes
                Supplier s = new Supplier();
                SupplierSet changes = (SupplierSet)serviceEditor1.SupplierSet.GetChanges();
                
                SelectedService = changes.Service.FindByServiceID(SelectedService.ServiceID);
                SelectedRate = changes.Rate.FindByRateID(SelectedRate.RateID);
                SelectedOption = changes.Option.FindByOptionID(SelectedOption.OptionID);

                s.SaveSupplierSet(changes);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void serviceEditor1_OnOptionSelected(Modules.SupplierModule.OptionSelectedEventArgs e)
        {
            SelectedOption = serviceEditor1.SupplierSet.Option.FindByOptionID(e.OptionId);
            SelectedRate = serviceEditor1.SupplierSet.Rate.FindByRateID(SelectedOption.RateID);
            SelectedService = serviceEditor1.SupplierSet.Service.FindByServiceID(SelectedRate.ServiceID);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (SelectedOption == null || serviceEditor1.OptionsGrid.Rows.VisibleRowCount == 0)
            {
                App.ShowError("Please select an option.");
                DialogResult = DialogResult.None;
                return;
            }

            if (SelectedRate.ValidFrom.Date > _purchaseItemDate.Date ||
                SelectedRate.ValidTo.Date < _purchaseItemDate.Date)
            {
                if (!App.AskYesNo("Selected rate is not valid for current booking date of " +
                    _purchaseItemDate.ToShortDateString() + "\r\n\r\nContinue?"))
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            if (serviceEditor1.SupplierSet.HasChanges())
                if (App.AskYesNo("You have made changes to this supplier.\r\n\r\nDo you want to save the changes?"))
                    SaveSupplier();
            
            DialogResult = DialogResult.OK;
        }
    }
}
