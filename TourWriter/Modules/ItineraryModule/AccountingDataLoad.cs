using System.Collections.Generic;
using System.Data;
using System.Threading;
using TourWriter.Info.Services;

namespace TourWriter.Modules.ItineraryModule
{
    class AccountingDataLoad
    {
        const string PurchasesSql = "select * from PurchaseItemPaymentsDetail where ItineraryID = {0} order by PaymentDueDate";
        const string AllocationsSql = "select * from ItinerarySaleAllocationDetail where ItineraryID = {0}";
        const string ReceiptsSql = "select * from ItineraryPaymentDetail where ItineraryID = {0}";
        const string ServiceTypesSql = "select ServiceTypeID, Gross from ItineraryServiceTypeDetail where ItineraryID = {0}";

        private List<Thread> threads;
        private readonly int itineraryId;
        public DataSet PurchasesDataSet { get; set; }
        public DataSet AllocationsDataSet { get; set; }
        public DataSet ReceiptsDataSet { get; set; }
        public DataSet ServiceTypesDataSet { get; set; }
        
        /// <summary>
        /// Loads Itinerary Accounting data on background threads.
        /// </summary>
        /// <param name="itineraryId"></param>
        internal AccountingDataLoad(int itineraryId)
        {
            this.itineraryId = itineraryId;
        }

        /// <summary>
        /// Loads each dataset, on asyncronis background threads.
        /// </summary>
        internal void LoadDataAsync()
        {
            InitialiseDataSets();

            threads = new List<Thread>
                          {
                              CreateThread(PurchasesDataSet, string.Format(PurchasesSql, itineraryId)),
                              CreateThread(AllocationsDataSet, string.Format(AllocationsSql, itineraryId)),
                              CreateThread(ReceiptsDataSet, string.Format(ReceiptsSql, itineraryId)),
                              CreateThread(ServiceTypesDataSet, string.Format(ServiceTypesSql, itineraryId))
                          };

            // Wait for threads to finish
            foreach (Thread t in threads)
            {
                t.Join();
            }
            threads.Clear();
        }

        private static Thread CreateThread(DataSet ds, string sql)
        {
            var thread = new Thread(() => DataSetHelper.FillDataset(ds, sql)) { Name = string.Format("Itinerary_AccountingDataLoad_{0}", ds.DataSetName) };
            thread.Start();

            return thread;
        }

        private void InitialiseDataSets()
        {
            if (PurchasesDataSet == null)
                PurchasesDataSet = new DataSet("PurchaseDataSet");

            if (AllocationsDataSet == null)
                AllocationsDataSet = new DataSet("SalesDataSet");

            if (ReceiptsDataSet == null)
                ReceiptsDataSet = new DataSet("ReceiptsDataSet");

            if (ServiceTypesDataSet == null)
                ServiceTypesDataSet = new DataSet("AllocationsDataSet");
        }
    }
}
