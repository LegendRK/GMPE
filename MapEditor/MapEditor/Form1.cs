using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
/*
 * Name: Rohit Kaushik 
 * Section: 1
 * Purpose: To hold the code for the map editor.
 * Edited: Throughout spring break
 */
namespace MapEditor
{
    public partial class Form1 : Form
    {
        // attributes
        private int height;
        private int width;


        string filepath;

        OpenFileDialog openFD = new OpenFileDialog();
        SaveFileDialog saveFD = new SaveFileDialog();

        Dictionary<RadioButton, string> bindings = new Dictionary<RadioButton, string>();
        List<RadioButton> radioButtons = new List<RadioButton>();
        List<Rectangle> rooms = new List<Rectangle>();

        // constructor
        public Form1()
        {
            InitializeComponent();

            bindings.Add(radioButton1, "0");
            bindings.Add(radioButton2, "1");
            bindings.Add(radioButton3, "2");
            bindings.Add(radioButton4, "3");
            bindings.Add(radioButton5, "4");
            bindings.Add(radioButton6, "5");
            bindings.Add(radioButton7, "6");
            bindings.Add(radioButton8, "7");
            bindings.Add(radioButton9, "a");
            bindings.Add(radioButton10, "b");
            bindings.Add(radioButton11, "c");
            bindings.Add(radioButton12, "p");
            bindings.Add(radioButton13, "e");

        }

        // methods
        #region Create New Map
        // for created a new map
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            using (var form2 = new Form2())
            {
                form2.ShowDialog();

                if(form2.valuesAccepted == true)
                {
                    // set the width and height
                    width = form2.MapWidth;
                    height = form2.MapHeight;

                    // get the name the user wanted for the map
                    nameBox.Text = form2.MapName;

                    // get the value wanted by the user
                    backgroundVal.Value = form2.MapBackground;

                    // makes sure columsn dont stack if they try to create the map again
                    mapView.Columns.Clear();

                    // create the columns first
                    mapView.ColumnCount = width;
                    for (int i = 0; i < mapView.ColumnCount; i++)
                    {
                        mapView.Columns[i].Name = i.ToString();
                        mapView.Columns[i].Width = 30;
                    }

                    // create the rows, initialize values to 0 except last line, which will have 1's (floors)
                    for (int i = 0; i < height; i++)
                    {
                        string[] row = new string[width];
                        if (i == height - 1)
                        {
                            for (int j = 0; j < row.Length; j++)
                            {
                                row[j] = "1";
                            }
                        }
                        else
                        {
                            for (int j = 0; j < row.Length; j++)
                            {
                                row[j] = "0";
                            }
                        }
                        mapView.Rows.Add(row);
                    }
                }
            }
               
        }
        #endregion

        #region Open Map
        // for opening a map file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFD.ShowDialog();

            try
            {
                filepath = Path.GetFullPath(openFD.FileName);
            }
            catch (ArgumentException)
            {

            }

            // open the file reader
            try
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(filepath));

                // get the name of the map
                nameBox.Text = reader.ReadString();

                // get the background value of the map
                backgroundVal.Value = reader.ReadDecimal();

                // get the size of the map
                width = reader.ReadInt32();
                height = reader.ReadInt32();

                mapView.ColumnCount = 0;

                mapView.ColumnCount = width;
                for (int i = 0; i < mapView.ColumnCount; i++)
                {
                    mapView.Columns[i].Name = i.ToString();
                    mapView.Columns[i].Width = 20;
                }

                try
                {
                    for (int i = 0; i < height; i++)
                    {
                        string[] row = new string[width];
                        for (int j = 0; j < width; j++)
                        {
                            row[j] = reader.ReadString();
                        }
                        mapView.Rows.Add(row);
                    }
                }
                catch (EndOfStreamException eose)
                {
                    
                }

                rooms.Clear();

                int roomCount = 0;

                try
                {
                    roomCount = reader.ReadInt32();
                }
                catch(EndOfStreamException eose)
                {
                    reader.Close();
                    return;
                }
                

                if(roomCount != 0)
                {
                    for (int i = 0; i < roomCount; i++)
                    {
                        rooms.Add(new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
                    }

                    foreach (Rectangle room in rooms)
                    {
                        for (int i = room.X; i < room.X + room.Width; i++)
                        {
                            for (int j = room.Y; j < room.Y + room.Height; j++)
                            {
                                mapView[i, j].Style.BackColor = Color.Gray;
                            }
                        }
                    }
                }

                

                reader.Close();
            }
            catch (ArgumentNullException anu)
            {
                
            }

        }
        #endregion

        #region Save Map 
        // for saving a map
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int roomCount = 0;
            try
            {
                // get the path user wants to save in
                saveFD.Filter = "Binary File (.dat)|*.dat";
                saveFD.Title = "Save File";
                saveFD.FileName = nameBox.Text;

                // show the dialog 
                saveFD.ShowDialog();

                filepath = Path.GetFullPath(saveFD.FileName);

                if (filepath != null && mapView.Columns != null)
                {
                    // create the writer
                    BinaryWriter writer = new BinaryWriter(File.Create(filepath));

                    // write to the file
                    writer.Write(nameBox.Text);             // map name
                    writer.Write(backgroundVal.Value);      // map background
                    writer.Write(width);                    // map width
                    writer.Write(height);                   // map height

                    // get paired objects
                    
                    // write the map to the file
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            writer.Write(mapView[j, i].Value.ToString());
                        }
                    }

                    roomCount = rooms.Count();

                    writer.Write(roomCount);

                    for (int i = 0; i < roomCount; i++)
                    {
                        // write the x start of the room
                        writer.Write(rooms[i].X);

                        // write the y of the room
                        writer.Write(rooms[i].Y);

                        // write the width of the room
                        writer.Write(rooms[i].Width);

                        // write the height of the room
                        writer.Write(rooms[i].Height);
                    }


                    writer.Close();
                }
            }
            catch(ArgumentException)
            {
                
            }
        }
        #endregion

        #region Edit Map Size
        // editing map size
        private void editMapSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var form3 = new Form3())
            {
                form3.ShowDialog();

                if (form3.newValues == true)
                {
                    int newWidth = form3.NewWidth;
                    int newHeight = form3.NewHeight;

                    if(width - newWidth != 0)
                    {
                        if (newWidth - width < 0)
                        {
                            for (int i = 0; i < Math.Abs(width - newWidth); i++)
                            {
                                mapView.Columns.RemoveAt(width - i - 1);
                            }
                            width = newWidth;
                        }

                        if(newWidth - width > 0)
                        {
                            mapView.ColumnCount = newWidth;
                            for (int i = 0; i < mapView.ColumnCount; i++)
                            {
                                mapView.Columns[i].Name = i.ToString();
                                mapView.Columns[i].Width = 30;
                                
                            }
                            width = newWidth;
                        }
                    }

                    if(newHeight - height != 0)
                    {
                        if (newHeight - height < 0)
                        {
                            for (int i = 0; i < Math.Abs(newHeight - height); i++)
                            {
                                mapView.Rows.RemoveAt(height - i - 1);
                            }
                            height = newHeight;
                        }

                        if(newHeight - height > 0)
                        {
                            for (int i = 0; i < newHeight - height; i++)
                            {
                                string[] row = new string[width];
                                
                                mapView.Rows.Add(row);
                            }
                            height = newHeight;
                        }
                    }
                }
            }
        }
        #endregion

        #region Radio Button Usage
        // if user clicks on cell with particul radio button checked
        private void mapView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var checkedButton = groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            /*
            if(checkedButton == radioButton12)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (mapView[j, i].ToString() == "p")
                        {
                            mapView[j, i].Value = "0";
                        }
                    }
                }
            }
            else
            {

            }
            */
            mapView.CurrentCell.Value = bindings[checkedButton];
            
            
        }
        #endregion

        #region Creating Rooms
        private void changeCellColor_Click(object sender, EventArgs e)
        {
            DataGridViewCell firstCell = mapView.Rows[0].Cells[0];
            DataGridViewCell lastCell = mapView.Rows[0].Cells[0];

            Boolean firstCellFlag = false;

            foreach (DataGridViewRow row in mapView.Rows)
            {
                foreach(DataGridViewCell cell in row.Cells)
                {
                    if (cell.Selected == true)
                    {
                        if(colorList.Text == "Red")
                        {
                            cell.Style.BackColor = Color.Red;
                        }
                        else if(colorList.Text == "Blue")
                        {
                            cell.Style.BackColor = Color.Blue;
                        }
                        else if(colorList.Text == "Yellow")
                        {
                            cell.Style.BackColor = Color.Yellow;
                        }

                        if(!firstCellFlag)
                        {
                            firstCell = cell;
                            firstCellFlag = true;
                        }

                        lastCell = cell;
                    }
                }
            }
            rooms.Add(new Rectangle(Convert.ToInt32(firstCell.ColumnIndex), Convert.ToInt32(firstCell.RowIndex), Convert.ToInt32(lastCell.ColumnIndex - firstCell.ColumnIndex) + 1, Convert.ToInt32(lastCell.RowIndex - firstCell.RowIndex) + 1));
        }
        #endregion

        #region Resetting Rooms
        private void resetButton_Click(object sender, EventArgs e)
        {
            // reset the rooms array
            rooms.Clear();

            // reset the grid colors
            foreach (DataGridViewRow row in mapView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.White;
                }
            }

        }


        #endregion

        #region Copy Pase Cut functions
        private void mapView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (mapView.SelectedCells.Count > 0)
                mapView.ContextMenuStrip = contextMenuStrip1;
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Copy to clipboard
            CopyToClipboard();

            //Clear selected cells
            foreach (DataGridViewCell dgvCell in mapView.SelectedCells)
                dgvCell.Value = string.Empty;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Perform paste Operation
            PasteClipboardValue();
        }


        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = mapView.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void PasteClipboardValue()
        {
            //Show Error if no cell is selected
            if (mapView.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the starting Cell
            DataGridViewCell startCell = GetStartCell(mapView);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue =
                    ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //Check if the index is within the limit
                    if (iColIndex <= mapView.Columns.Count - 1
                    && iRowIndex <= mapView.Rows.Count - 1)
                    {
                        DataGridViewCell cell = mapView[iColIndex, iRowIndex];

                        cell.Value = cbValue[rowKey][cellKey];
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }

        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }

        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionary with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }
        #endregion

    }
}