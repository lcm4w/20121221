using System.Runtime.Serialization;

namespace TourWriter.Info
{
	public class ContactSet : Base_Classes.ContactSetBase
	{
		
		public ContactSet() : base()
		{
		}

		protected ContactSet(SerializationInfo info, StreamingContext context) : 
			base(info, context)
		{
		}
	}
}
