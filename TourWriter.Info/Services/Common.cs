using System;

namespace TourWriter.Info.Services
{
    /// <summary>
    /// Common methods to TourWriter business objects.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Calculate gross based on net and markup.
        /// </summary>
        public static decimal CalcGrossByNetMarkup(decimal net, decimal markup)
        {
            return net*(1 + (markup/100));
        }

        /// <summary>
        /// Calculate gross based on net and commission.
        /// </summary>
        public static decimal CalcGrossByNetCommission(decimal net, decimal commission)
        {
            if (commission == 0) return 0;
            if (commission == 100) commission = 99.99m;

            return net*100/(100 - commission);
        }

        /// <summary>
        /// Calculate gross based on gross commission.
        /// </summary>
        public static decimal CalcGrossByGrossCommission(decimal gross, decimal commission)
        {
		    // WARNING: assume 'user friendly' postive number, so subtract because taking comm OFF gross
            return gross * (1 - (commission / 100));
        }

        /// <summary>
        /// Calculate the markup value.
        /// </summary>
        /// <param name="net"></param>
        /// <param name="gross"></param>
        /// <returns></returns>
        public static decimal CalcMarkupByNetGross(decimal net, decimal gross)
        {
            if (net == 0) return 0;

            return (gross - net)/net*100;
        }

        /// <summary>
        /// Calculate the commission value.
        /// </summary>
        /// <param name="net"></param>
        /// <param name="gross"></param>
        /// <returns></returns>
        public static decimal CalcCommissionByNetGross(decimal net, decimal gross)
        {
            if (gross == 0) return 0;

            return (gross - net)/gross*100;
        }

        /// <summary>
        /// Calculates net based on gross and markup.
        /// </summary>
        /// <returns></returns>
        public static decimal CalcNetByGrossMarkup(decimal gross, decimal markup)
        {
            return gross*100/(100 + markup); 
        }

        /// <summary>
        /// Calculate the gross after removing tax.
        /// </summary>
        /// <param name="gross">The gross amount.</param>
        /// <param name="percentTax">The tax amount in percent.</param>
        /// <returns></returns>
        public static decimal CalcGrossLessTax(decimal gross, decimal percentTax)
        {
            return gross*100/(100 + percentTax);
        }

        /// <summary>
        /// Calculate the tax amount.
        /// </summary>
        /// <param name="gross">The gross amount.</param>
        /// <param name="percentTax">The tax amount in percent.</param>
        /// <returns></returns>
        public static decimal CalcTaxAmount(decimal gross, decimal percentTax)
        {
            return gross - CalcGrossLessTax(gross, percentTax);
        }

        /// <summary>
        /// Gets the full text of the payment terms, including deposit, balance, or full payment
        /// </summary>
        /// <param name="paymentDueID"></param>
        /// <param name="paymentDuePeriod"></param>
        /// <param name="depositAmount"></param>
        /// <param name="depositType"></param>
        /// <param name="depositDueID"></param>
        /// <param name="depositDuePeriod"></param>
        /// <param name="paymentDueTable"></param>
        /// <returns>Full payment terms.</returns>
        public static string GetPaymentTermsFullText(int? paymentDueID, int? paymentDuePeriod,
            decimal? depositAmount, char? depositType, int? depositDueID, int? depositDuePeriod,
            ToolSet.PaymentDueDataTable paymentDueTable)
        {
            var depositTerms = GetDepositTermsText(depositAmount, depositType, depositDueID, depositDuePeriod, paymentDueTable);
            var balanceTerms = GetPaymentTermsText(paymentDueID, paymentDuePeriod, paymentDueTable);

            if (string.IsNullOrEmpty(depositTerms))
            {
                return string.Format("Payment: {0}", balanceTerms);
            }
            else
            {
                return string.Format(
                    "Deposit: {0}\r\n" +
                    "Balance: {1}",
                    depositTerms, balanceTerms);
            }
        }
               
        /// <summary>
        /// Gets a text representation of the payment or balance (if deposit required) terms data
        /// </summary>
        /// <param name="paymentDueID"></param>
        /// <param name="paymentDuePeriod"></param>
        /// <param name="paymentDueTable">PaymentDue lookup table</param>
        /// <returns>Payment or balance terms</returns>
        private static string GetPaymentTermsText(int? paymentDueID, int? paymentDuePeriod,
            ToolSet.PaymentDueDataTable paymentDueTable)
        {
            if (paymentDueID.HasValue)
            {
                return string.Format("{0} {1}",
                                     (int) paymentDuePeriod,
                                     paymentDueTable.FindByPaymentDueID((int) paymentDueID).PaymentDueName);
            }
            return "";
        }
        
        /// <summary>
        /// Gets a text representation of the deposit terms data
        /// </summary>
        /// <param name="depositAmount"></param>
        /// <param name="depositType"></param>
        /// <param name="depositDueID"></param>
        /// <param name="depositDuePeriod"></param>
        /// <param name="paymentDueTable">PaymentDue lookup table</param>
        /// <returns>Deposit terms</returns>
        private static string GetDepositTermsText(decimal? depositAmount, char? depositType, 
            int? depositDueID, int? depositDuePeriod, ToolSet.PaymentDueDataTable paymentDueTable)
        {
            string text = String.Empty;

            if (depositAmount.HasValue)
            {
                // deposit amount
                if ((char)depositType == 'c')
                {
                    text += string.Format("{0:f2} (fixed amount)", (decimal)depositAmount);
                }
                else if ((char)depositType == 'p')
                {
                    text += string.Format("{0:f2} % (percentage)", (decimal)depositAmount);
                }

                // deposit terms
                if (depositDueID.HasValue)
                {
                    text += string.Format(", {0} {1}",
                                          (int)depositDuePeriod,
                                          paymentDueTable.FindByPaymentDueID((int)depositDueID).PaymentDueName);
                }
            }
            return text;
        }
    }
}