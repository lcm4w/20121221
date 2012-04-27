using System.Collections.Generic;
using NUnit.Framework;
using TourWriter.DataAccess;
using TourWriter.Info;
using TourWriter.Services;
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
            if(src.ItineraryPayment.Count > 0) Assert.AreNotEqual(src.ItineraryPayment.Count, test.ItineraryPayment.Count); // don't copy payments
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

        [Test]
        public void Booking_discounts()
        {
            var d1 = new ItinerarySet().Discount.NewDiscountRow(); d1.UnitsUsed =  5; d1.UnitsFree = 1;
            var d2 = new ItinerarySet().Discount.NewDiscountRow(); d2.UnitsUsed = 10; d2.UnitsFree = 2;
            var d3 = new ItinerarySet().Discount.NewDiscountRow(); d3.UnitsUsed = 15; d3.UnitsFree = 3;
            var d4 = new ItinerarySet().Discount.NewDiscountRow(); d4.UnitsUsed = 20; d4.UnitsFree = 4;
            var d5 = new ItinerarySet().Discount.NewDiscountRow(); d5.UnitsUsed = 25; d5.UnitsFree = 5;
            var discounts = new List<ItinerarySet.DiscountRow> {d1, d2, d3, d4, d5};

            Assert.AreEqual(0, Services.Discounts.CalcDiscount( 1, discounts));
            Assert.AreEqual(0, Services.Discounts.CalcDiscount( 2, discounts));
            Assert.AreEqual(0, Services.Discounts.CalcDiscount( 3, discounts));
            Assert.AreEqual(0, Services.Discounts.CalcDiscount( 4, discounts));
            Assert.AreEqual(0, Services.Discounts.CalcDiscount( 5, discounts));
            Assert.AreEqual(1, Services.Discounts.CalcDiscount( 6, discounts));
            Assert.AreEqual(1, Services.Discounts.CalcDiscount( 7, discounts));
            Assert.AreEqual(1, Services.Discounts.CalcDiscount( 8, discounts));
            Assert.AreEqual(1, Services.Discounts.CalcDiscount( 9, discounts));
            Assert.AreEqual(1, Services.Discounts.CalcDiscount(10, discounts));
            Assert.AreEqual(1, Services.Discounts.CalcDiscount(11, discounts));
            Assert.AreEqual(2, Services.Discounts.CalcDiscount(12, discounts));
            Assert.AreEqual(2, Services.Discounts.CalcDiscount(13, discounts));
            Assert.AreEqual(2, Services.Discounts.CalcDiscount(14, discounts));
            Assert.AreEqual(2, Services.Discounts.CalcDiscount(15, discounts));
            Assert.AreEqual(2, Services.Discounts.CalcDiscount(16, discounts));
            Assert.AreEqual(2, Services.Discounts.CalcDiscount(17, discounts));
            Assert.AreEqual(3, Services.Discounts.CalcDiscount(18, discounts));
            Assert.AreEqual(3, Services.Discounts.CalcDiscount(19, discounts));
            Assert.AreEqual(3, Services.Discounts.CalcDiscount(20, discounts));
            Assert.AreEqual(3, Services.Discounts.CalcDiscount(21, discounts));
            Assert.AreEqual(3, Services.Discounts.CalcDiscount(22, discounts));
            Assert.AreEqual(3, Services.Discounts.CalcDiscount(23, discounts));
            Assert.AreEqual(4, Services.Discounts.CalcDiscount(24, discounts));
            Assert.AreEqual(4, Services.Discounts.CalcDiscount(25, discounts));
            Assert.AreEqual(4, Services.Discounts.CalcDiscount(26, discounts));
            Assert.AreEqual(4, Services.Discounts.CalcDiscount(27, discounts));
            Assert.AreEqual(4, Services.Discounts.CalcDiscount(28, discounts));
            Assert.AreEqual(4, Services.Discounts.CalcDiscount(29, discounts));
            Assert.AreEqual(5, Services.Discounts.CalcDiscount(30, discounts));
        }
    }
}
