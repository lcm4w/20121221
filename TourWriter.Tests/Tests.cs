using NUnit.Framework;
using TourWriter.DataAccess;
using TourWriter.Info;
using TourWriter.UserControls;

namespace TourWriter.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Copy_itinerary_set()
        {
            var db = new ItineraryDB(TestHelpers.GetLocalConnectionString("(local)"));
            var src = db.GetItinerarySet(5, new ItinerarySet());
            Assert.AreEqual(1, src.Itinerary.Count);

            var copy = CopyHelper.Copy(src);

            // test save
            copy = db.SaveItinerarySet(copy);
            var test = new ItinerarySet();
            test = db.GetItinerarySet(copy.Itinerary[0].ItineraryID, test);

            Assert.AreEqual(src.ItineraryMarginOverride.Count, test.ItineraryMarginOverride.Count);
            Assert.AreEqual(src.ItineraryPubFile.Count, test.ItineraryPubFile.Count);
            Assert.AreEqual(src.PurchaseLine.Count, test.PurchaseLine.Count);
            Assert.AreEqual(src.PurchaseItem.Count, test.PurchaseItem.Count);
            Assert.AreEqual(src.PurchaseItemNote.Count, test.PurchaseItemNote.Count);
            Assert.AreEqual(src.ItineraryGroup.Count, test.ItineraryGroup.Count);
            Assert.AreEqual(src.ItineraryMember.Count, test.ItineraryMember.Count);
            Assert.AreEqual(src.ItineraryPayment.Count, test.ItineraryPayment.Count);
            Assert.AreEqual(src.ItineraryPax.Count, test.ItineraryPax.Count);
            Assert.AreEqual(src.ItineraryPaxOverride.Count, test.ItineraryPaxOverride.Count);
            Assert.AreEqual(src.ItineraryMessage.Count, test.ItineraryMessage.Count);
            Assert.AreEqual(src.ItinerarySale.Count, test.ItinerarySale.Count);
            Assert.AreEqual(src.ItinerarySaleAllocation.Count, test.ItinerarySaleAllocation.Count);

            // test delete
            db.Delete(copy.Itinerary[0].ItineraryID);
            test = new ItinerarySet();
            test = db.GetItinerarySet(copy.Itinerary[0].ItineraryID, test);
            Assert.AreEqual(0, test.Itinerary.Count);
        }

        [Test]
        public void Copy_supplier_set()
        {
            var db = new SupplierDB(TestHelpers.GetLocalConnectionString("(local)"));
            var src = db.GetSupplierSet(1226);
            Assert.AreEqual(1, src.Supplier.Count);

            var copy = CopyHelper.Copy(src);

            // test save
            copy = db.SaveSupplierSet(copy);
            var test = new SupplierSet();
            test = db.GetSupplierSet(copy.Supplier[0].SupplierID);

            Assert.AreEqual(src.Service.Count, test.Service.Count);
            Assert.AreEqual(src.Rate.Count, test.Rate.Count);
            Assert.AreEqual(src.Option.Count, test.Option.Count);
            Assert.AreEqual(src.ServiceContent.Count, test.ServiceContent.Count);
            Assert.AreEqual(src.ServiceConfig.Count, test.ServiceConfig.Count);
            Assert.AreEqual(src.ServiceTime.Count, test.ServiceTime.Count);
            Assert.AreEqual(src.Discount.Count, test.Discount.Count);
            Assert.AreEqual(src.SupplierContent.Count, test.SupplierContent.Count);
            Assert.AreEqual(src.SupplierContact.Count, test.SupplierContact.Count);
            Assert.AreEqual(src.SupplierCreditCard.Count, test.SupplierCreditCard.Count);
            Assert.AreEqual(src.SupplierConfig.Count, test.SupplierConfig.Count);
            Assert.AreEqual(src.SupplierMessage.Count, test.SupplierMessage.Count);
            Assert.AreEqual(src.SupplierNote.Count, test.SupplierNote.Count);
            Assert.AreEqual(src.SupplierText.Count, test.SupplierText.Count);

            // test delete
            db.Delete(copy.Supplier[0].SupplierID);
            test = new SupplierSet();
            test = db.GetSupplierSet(copy.Supplier[0].SupplierID);
            Assert.AreEqual(0, test.Supplier.Count);
        }

        [Test]
        public void Copy_contact_set()
        {
            var db = new ContactDB(TestHelpers.GetLocalConnectionString("(local)"));
            var src = db.GetContactSet(2);
            Assert.AreEqual(1, src.Contact.Count);

            var copy = CopyHelper.Copy(src);

            // test save
            copy = db.SaveContactSet(copy);
            var test = new ContactSet();
            test = db.GetContactSet(copy.Contact[0].ContactID);

            Assert.AreEqual(src.ContactContactCategory.Count, test.ContactContactCategory.Count);

            // test delete
            db.Delete(copy.Contact[0].ContactID);
            test = new ContactSet();
            test = db.GetContactSet(copy.Contact[0].ContactID);
            Assert.AreEqual(0, test.Contact.Count);
        }
    }
}
