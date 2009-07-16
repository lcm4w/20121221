using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;

namespace TourWriter.Modules.ItineraryModule
{
	/// <summary>
	/// Summary description for ItineraryFormLoader.
	/// </summary>
	public class ItineraryFormLoader
	{
		public ItineraryFormLoader(UltraTreeNode formTag, Form mdiReference)
		{
			// Get typeId from menu item info, or load ds and pass it to itinerary form ? 
			// ========================================================================
			//ItinerarySet itinerarySet = new Itinerary().GetItinerarySet(itineraryId);
			//int typeId = itinerarySet.Itinerary[0].IsItineraryFileTypeIDNull() ?
			//	itinerarySet.Itinerary[0].ItineraryFileTypeID : 2;
			int typeId = 2;
			// ========================================================================
			
			Form form = null;
			switch (typeId)
			{
				case 1 :
				{
					//form = new ItineraryQuick();
					break;
				}
				case 2 :
				{
					form = new ItineraryMain();
					break;
				}
				case 3 :
				{
					form = new ItineraryGroup();
					break;
				}
			}

			form.Tag = formTag; // reference to menu node					
			form.MdiParent = mdiReference;
			//form.AutoScroll = true;
			form.Show();
		}
	}
}
