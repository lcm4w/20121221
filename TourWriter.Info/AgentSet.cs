
using System;

namespace TourWriter.Info
{
    partial class AgentSet
	{
        partial class AgentRow
        {
        }

        partial class PaymentTermRow
        {
            public string GetCustomText(ToolSet.PaymentDueDataTable paymentDueTable)
            {
                return Services.Common.GetPaymentTermsFullText(
                    (!IsPaymentDueIDNull()) ? (int?)PaymentDueID : null,
                    (!IsPaymentDuePeriodNull()) ? (int?)PaymentDuePeriod : null,
                    (!IsDepositAmountNull()) ? (decimal?)DepositAmount : null,
                    (!IsDepositTypeNull()) ? (char?)DepositType : null,
                    (!IsDepositDueIDNull()) ? (int?)DepositDueID : null,
                    (!IsDepositDuePeriodNull()) ? (int?)DepositDuePeriod : null,
                    paymentDueTable);
            }
        }

        partial class AgentMarginDataTable
        {
            /// <summary>
            /// Adds or updates or deletes a row, depending on row existing and margin param having a value.
            /// </summary>
            /// <param name="agentId">The agent id.</param>
            /// <param name="serviceTypeId">The service type id.</param>
            /// <param name="margin">The margin, which if null will cause row to be deleted.</param>
            public void AddInsertOrDelete(int agentId, int serviceTypeId, decimal? margin)
            {
                AgentMarginRow existingRow = FindByAgentIDServiceTypeID(agentId, serviceTypeId);

                if (existingRow == null && margin.HasValue)
                {
                    // add new row
                    AgentMarginRow r = NewAgentMarginRow();
                    r.AgentID = agentId;
                    r.ServiceTypeID = serviceTypeId;
                    r.Margin = (decimal)margin;
                    AddAgentMarginRow(r);
                }
                else if (existingRow != null && margin.HasValue)
                {
                    // update existing row
                    if (existingRow.Margin != (decimal)margin)
                        existingRow.Margin = (decimal)margin;
                }
                else if (existingRow != null)
                {
                    // delete existing row
                    existingRow.Delete();
                }
            }
        }
	}
}
