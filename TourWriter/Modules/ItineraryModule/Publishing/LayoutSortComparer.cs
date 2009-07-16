using System;
using System.Collections;
using Infragistics.Win.UltraWinTree;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
	/// <summary>
	/// Custom sorting class for the Layout Sections (level 2) tree nodes
	/// </summary>
	internal class LayoutSortComparer : IComparer
	{
        int IComparer.Compare(object x, object y)
        {
            UltraTreeNode n1 = x as UltraTreeNode;
            UltraTreeNode n2 = y as UltraTreeNode;

            if (n1 == n2)
                return 0;

            // sort date nodes
            if (IsDayNode(n1) && IsDayNode(n2))
            {
                DateTime d1 = LayoutHelper.GetDaySectionDate(n1);
                DateTime d2 = LayoutHelper.GetDaySectionDate(n2);

                if (d1 < d2)
                    return -1;
                if (d1 > d2)
                    return 1;
                return 0;
            }

            // sort front/back section nodes
            if (IsFrontNode(n1))
                return -1;
            if (IsBackNode(n1))
                return 1;
            if (IsFrontNode(n2))
                return 1;
            if (IsBackNode(n2))
                return -1;
            return 0;
        }

		private bool IsDayNode(UltraTreeNode n)
		{
			return n.Key.StartsWith(FileBuilderAdvanced.DaysSectionNodeKey);
		}

		private bool IsFrontNode(UltraTreeNode n)
		{
			return n.Key.StartsWith(FileBuilderAdvanced.FrontSectionNodeKey);
		}

		private bool IsBackNode(UltraTreeNode n)
		{
			return n.Key.StartsWith(FileBuilderAdvanced.BackSectionNodeKey);
		}
	}
}
