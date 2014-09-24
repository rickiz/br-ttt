using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class TrendPriorityViewModel
    {
        public TrendPriorityViewModel()
        {
            Priorities = new List<TrendPriorityDetails>();
        }

        public List<TrendPriorityDetails> Priorities { get; set; }
    }

    public class TrendPriorityDetails
    {
        public int TrendID { get; set; }
        public string TrendName { get; set; }
        public string Priority { get; set; }
    }

    public class ColourPriorityViewModel
    {
        public ColourPriorityViewModel()
        {
            Priorities = new List<ColourPriorityDetails>();
        }

        public List<ColourPriorityDetails> Priorities { get; set; }
    }

    public class ColourPriorityDetails
    {
        public int ColourID { get; set; }
        public string ColourName { get; set; }
        public string Priority { get; set; }
    }

    public class ZeroAreLast : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (y == 0 && x != 0)
            {
                return -1;
            }
            else if (y != 0 && x == 0)
            {
                return 1;
            }
            else
            {
                return String.Compare(x.ToString(), y.ToString());
            }
        }
    }
}