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
    public partial class Form3 : Form
    {
        // attributes
        private int newWidth;
        private int newHeight;

        public Boolean newValues;

        public Form3()
        {
            InitializeComponent();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            if(!int.TryParse(widthBox.Text, out newWidth) || !int.TryParse(heightBox.Text, out newHeight))
            {
                if(!int.TryParse(widthBox.Text, out newWidth))
                    label7.Visible = true;
                if (!int.TryParse(heightBox.Text, out newHeight))
                    label8.Visible = true;
                return;
            }
            else
            {
                newValues = true;
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            newValues = false;
            this.Close();
        }

        // properties
        public int NewWidth
        {
            get { return newWidth; }
        }

        public int NewHeight
        {
            get { return newHeight; }
        }
    }
}
