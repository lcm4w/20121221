using System.Data;
using System.Linq;
using TourWriter.Info;

namespace TourWriter.UserControls
{
    public class CopyHelper
    {
        public static ItinerarySet CopyAndSaveItinerary(int id, string newName)
        {
            var db = new BusinessLogic.Itinerary();
            var src = db.GetItinerarySet(id, new ItinerarySet());

            var copy = Copy(src);
            copy.Itinerary[0].ItineraryName = newName;

            return db.SaveItinerarySet(copy);
        }

        public static SupplierSet CopyAndSaveSupplier(int id, string newName)
        {
            var db = new BusinessLogic.Supplier();
            var src = db.GetSupplierSet(id);

            var copy = Copy(src);
            copy.Supplier[0].SupplierName = newName;

            return db.SaveSupplierSet(copy);
        }

        public static ContactSet CopyAndSaveContact(int id, string newName)
        {
            var db = new BusinessLogic.Contact();
            var src = db.GetContactSet(id);

            var copy = Copy(src);
            copy.Contact[0].ContactName = newName;

            return db.SaveContactSet(copy);
        }
        

        public static ItinerarySet Copy(ItinerarySet src)
        {
            var id = -1;

            // copy data and schama
            var copy = (ItinerarySet)src.Copy();

            // update ids and set row states to 'Added'

            // itinerary
            foreach (var itin in copy.Itinerary.Rows.Cast<ItinerarySet.ItineraryRow>())
            {
                SetInserted(itin, "ItineraryID", id--);

                // purchase lines
                foreach (var line in itin.GetPurchaseLineRows())
                {
                    SetInserted(line, "PurchaseLineID", id--);

                    // purchase items
                    foreach (var item in line.GetPurchaseItemRows())
                    {
                        SetInserted(item, "PurchaseItemID", id--);

                        // purchase item notes
                        foreach (var row in item.GetPurchaseItemNoteRows())
                            SetInserted(row, "PurchaseItemNoteID", id--);
                    }
                }

                // itinerary margin overrides
                foreach (var row in itin.GetItineraryMarginOverrideRows())
                    SetInserted(row);

                // itinerary pub files
                foreach (var row in itin.GetItineraryPubFileRows())
                    SetInserted(row);

                // itinerary message
                foreach (var row in itin.GetItineraryMessageRows())
                    SetInserted(row);

                // itinerary group
                foreach (var group in itin.GetItineraryGroupRows())
                {
                    SetInserted(group, "ItineraryGroupID", id--);

                    // itinerary group members
                    foreach (var member in group.GetItineraryMemberRows())
                    {
                        SetInserted(member, "ItineraryMemberID", id--);
                        // don't copy itinerary member payments
                    }
                }

                // itinerary pax
                foreach (var pax in itin.GetItineraryPaxRows())
                {
                    SetInserted(pax, "ItineraryPaxID", id--);

                    // itinerary pax overrides
                    foreach (var row in pax.GetItineraryPaxOverrideRows())
                        SetInserted(row);
                }

                foreach (var sale in itin.GetItinerarySaleRows())
                {
                    SetInserted(sale, "ItinerarySaleID", id--);

                    foreach (var row in sale.GetItinerarySaleAllocationRows())
                        SetInserted(row);

                }
            }
            return copy;
        }

        public static SupplierSet Copy(SupplierSet src)
        {
            var id = -1;

            // copy data and schama
            var copy = (SupplierSet)src.Copy();

            // update ids and set row states to 'Added'

            // supplier
            foreach (var supplier in copy.Supplier.Rows.Cast<SupplierSet.SupplierRow>())
            {
                SetInserted(supplier, "SupplierID", id--);

                // services
                foreach (var service in supplier.GetServiceRows())
                {
                    SetInserted(service, "ServiceID", id--);

                    // rates
                    foreach (var rate in service.GetRateRows())
                    {
                        SetInserted(rate, "RateID", id--);

                        // options
                        foreach (var row in rate.GetOptionRows())
                            SetInserted(row, "OptionID", id--);
                    }

                    // service content
                    foreach (var row in service.GetServiceContentRows())
                        SetInserted(row, "ServiceContentID", id--);

                    // service configs 
                    foreach (var row in service.GetServiceConfigRows())
                        SetInserted(row);

                    // service times
                    foreach (var row in service.GetServiceTimeRows())
                        SetInserted(row, "ServiceTimeID", id--);

                    // discount
                    foreach (var row in service.GetDiscountRows())
                        SetInserted(row, "DiscountID", id--);
                }

                // supplier content
                foreach (var row in supplier.GetSupplierContentRows())
                    SetInserted(row, "SupplierContentID", id--);

                // supplier contact
                foreach (var row in supplier.GetSupplierContactRows())
                    SetInserted(row);

                // supplier credit card
                foreach (var row in supplier.GetSupplierCreditCardRows())
                    SetInserted(row);

                // supplier config
                foreach (var row in supplier.GetSupplierConfigRows())
                    SetInserted(row);

                // supplier message
                foreach (var row in supplier.GetSupplierMessageRows())
                    SetInserted(row);

                // supplier notes
                foreach (var row in supplier.GetSupplierNoteRows())
                    SetInserted(row, "SupplierNoteID", id--);

                // supplier text
                foreach (var row in supplier.GetSupplierTextRows())
                    SetInserted(row, "SupplierTextID", id--);
            }
            return copy;
        }

        public static ContactSet Copy(ContactSet src)
        {
            var id = -1;

            // copy data and schama
            var copy = (ContactSet)src.Copy();

            // update ids and set row states to 'Added'

            // contact
            foreach (var contact in copy.Contact.Rows.Cast<ContactSet.ContactRow>())
            {
                SetInserted(contact, "ContactID", id--);

                // categories
                foreach (var row in contact.GetContactContactCategoryRows())
                    SetInserted(row, "ContactCategoryID", id--);
            }
            return copy;
        }


        private static void SetInserted(DataRow row)
        {
            row.AcceptChanges();
            row.SetAdded();
        }

        private static void SetInserted(DataRow row, string key, int value)
        {
            row.Table.Columns[key].ReadOnly = false;
            row[key] = value;
            row.AcceptChanges();
            row.SetAdded();
        }
    }
}
