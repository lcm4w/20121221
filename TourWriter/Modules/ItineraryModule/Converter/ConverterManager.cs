using System.Drawing;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.UserControls;

namespace TourWriter.Modules.ItineraryModule.Converter
{
	/// <summary>
	/// Summary description for ConverterManager.
	/// </summary>
	public class ConverterManager
	{
		public static ConverterArgs OpenWizard(ItinerarySet itinerarySet)
		{
			ConverterArgs args = new ConverterArgs(itinerarySet);

			Wizard wizard = new Wizard(new Size(410, 360));
			wizard.AddPage(new ConverterTypes());
            wizard.AddPage(new ConverterOptions());
            wizard.SetMinimumSize(410, 360);
            wizard.Text = "Converting itinerary: " + itinerarySet.Itinerary[0].ItineraryName;
            wizard.Params = args;
		    
			if (wizard.ShowDialog() == DialogResult.OK)
				return args;
			return null;
		}
	}
}