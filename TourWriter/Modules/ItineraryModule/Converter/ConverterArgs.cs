using TourWriter.Info;

namespace TourWriter.Modules.ItineraryModule.Converter
{
	/// <summary>
	/// Summary description for ConverterArgs.
	/// </summary>
	public class ConverterArgs
	{
		public readonly ItinerarySet _ItinerarySet;

		public ConverterArgs(ItinerarySet itinerarySet)
		{
			_ItinerarySet = itinerarySet;
		}
	}
}