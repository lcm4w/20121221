using System;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Properties;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
    public partial class LayoutForm : Form
    {
        private int publisherFileId;
        
        private ItinerarySet itinerarySetCopy;
        
        private ItinerarySet itinerarySetOrig;


        public LayoutForm(ItinerarySet itinerarySet, int publisherFileId)
        {
            this.publisherFileId = publisherFileId;
            
            itinerarySetOrig = itinerarySet;
            itinerarySetCopy = itinerarySetOrig.CopyWithEvents();
            
            InitializeComponent();

            Icon = Resources.TourWriter16;

            }

        private void LayoutForm_Load(object sender, EventArgs e)
        {
            Size = Settings.Default.PublishingLayoutSize;
            Location = Settings.Default.PublishingLayoutLocation;

            layoutControl1.LoadLayout(itinerarySetCopy, publisherFileId);
            Text += ": " + itinerarySetCopy.ItineraryPubFile.FindByItineraryPubFileID(publisherFileId).ItineraryPubFileName;
        
        }

        private void LayoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.PublishingLayoutSize =
                WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;
            Settings.Default.PublishingLayoutLocation =
                WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            Settings.Default.Save();
            App.ClearBindings(this);
        }
        

        private void btnOk_Click(object sender, EventArgs e)
        {
            // Save the changed itinerarySet to the original itinerarySet.
            layoutControl1.SaveLayoutFile();

            try
            {
                itinerarySetOrig.Merge(itinerarySetCopy, false);
            }
            catch(System.Data.ConstraintException ex)
            {
                App.Error("An error occurred while adding the layout data to the itinerary. The layout changes may have been lost.", ex);                
            }
            finally
            {
                itinerarySetCopy.Dispose();
                DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Trash the itinerarySet copy that has changes.
            itinerarySetCopy.Dispose();

            DialogResult = DialogResult.Cancel;
        }
    }
}