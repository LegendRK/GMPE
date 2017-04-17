using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class Form2 : Form
    {
        // attributes
        private int mapWidth;
        private int mapHeight;
        public string mapName;
        public decimal mapBackground;

        public Boolean valuesAccepted;

        public Form2()
        {
            InitializeComponent();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(widthBox.Text, out mapWidth))
            {
                label7.Visible = true;
                return;
            }
            else
            {
                label7.Visible = false;
            }

            if(!int.TryParse(heightBox.Text, out mapHeight))
            {
                label8.Visible = true;
                return;
            }
            else
            {
                label8.Visible = false;
            }


            mapName = nameBox.Text;
            mapBackground = backgroundVal.Value;

            valuesAccepted = true;

            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            valuesAccepted = false;
            this.Close();
        }

        // properties
        public int MapWidth
        {
            get { return mapWidth; }
        }

        public int MapHeight
        {
            get { return mapHeight; }
        }

        public string MapName
        {
            get { return mapName; }
        }

        public decimal MapBackground
        {
            get { return mapBackground; }
        }
    }
}
