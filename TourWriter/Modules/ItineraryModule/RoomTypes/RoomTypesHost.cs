using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TourWriter.Modules.ItineraryModule.RoomTypes
{
    public partial class RoomTypesHost : Form
    {
        public RoomTypesHost(Info.ItinerarySet itinerarySet)
        {
            InitializeComponent();
            this.roomTypesControl1.ItinerarySet = itinerarySet;
        }
    }
}
