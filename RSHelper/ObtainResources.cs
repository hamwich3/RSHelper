using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSHelper
{
    public partial class ObtainImageResources : Form
    {

        Screen GameScreen;
        Graphics InfoGraphics;
        Graphics ChatGraphics;
        Graphics InventoryGraphics;
        Graphics MapGraphics;

        public ObtainImageResources()
        { 
            InitializeComponent();
            foreach(var screen in Screen.AllScreens)
            {
                cbScreens.Items.Add(screen);
            }
            cbScreens.SelectedIndex = 0;
            InfoGraphics = pbTopLeftInfo.CreateGraphics();
            ChatGraphics = pbChatInfo.CreateGraphics();
            InventoryGraphics = pbInventoryInfo.CreateGraphics();
            MapGraphics = pbMapInfo.CreateGraphics();
        }

        public void GetMapCircle()
        {

        }

        public void GetHealthCircle()
        {

        }

        public void GetAgilityCircle()
        {

        }

        public void GetCompassCircle()
        {

        }

        public void GetInventoryRectangle()
        {

        }

        public void GetInventoryTab()
        {

        }

        public void GetMagicTab()
        {

        }

        public void GetSkillsTab()
        {

        }

        public void GetTopLeftInfoRectangle()
        {

        }

        public void GetChatWindow()
        {

        }

        private void cbScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameScreen = (Screen)cbScreens.SelectedItem;
        }

        private void btnSetMapCircle_Click(object sender, EventArgs e)
        {

        }
    }
}
