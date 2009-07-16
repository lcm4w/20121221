using System.Collections.Generic;
using System.Data;
using System.Threading;
using TourWriter.Info.Services;

namespace TourWriter.Modules.SupplierModule
{
    class AccountingDataLoad
    {
        const string PurchasesSql = "select * from PurchaseItemPaymentsDetail where SupplierID = {0} order by PaymentDueDate";

        private List<Thread> threads;
        private readonly int supplierId;
        public DataSet PurchasesDataSet { get; set; }
        
        /// <summary>
        /// Loads Itinerary Accounting data on background threads.
        /// </summary>
        /// <param name="supplierId"></param>
        internal AccountingDataLoad(int supplierId)
        {
            this.supplierId = supplierId;
        }

        /// <summary>
        /// Loads each dataset, on asyncronis background threads.
        /// </summary>
        internal void LoadDataAsync()
        {
            InitialiseDataSets();

            threads = new List<Thread>
                          {
                              CreateThread(PurchasesDataSet, string.Format(PurchasesSql, supplierId))
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
            var thread = new Thread(() => DataSetHelper.FillDataset(ds, sql)) { Name = string.Format("Supplier_AccountingDataLoad_{0}", ds.DataSetName) };
            thread.Start();

            return thread;
        }

        private void InitialiseDataSets()
        {
            if (PurchasesDataSet == null)
                PurchasesDataSet = new DataSet("PurchaseDataSet");
        }
    }
}
