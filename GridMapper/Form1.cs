using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MapData;

//namespace GridMapper {
    public partial class Form1 : Form {
        Color selectedColor;
        char selectedTileType;

        string inputColumnsString;
        int inputColums;
        string inputRowString;
        int inputRows;
        char[][] tileData;
        MyLabel[][] labels;

        string lastSavedFile;

        Color[] myColors = {    Color.Brown,
                                Color.LawnGreen,
                                Color.Red,
                                Color.CornflowerBlue,
                                Color.LimeGreen,
                                Color.HotPink,
                                Color.Purple,
                                Color.Yellow,
                                Color.Orange};

        char[] tileTypes = { 'D', 'G', 'o', 'W', 'F', 'o', 'o', 'o', 'B' };
        int colorCount = 0;
        
        public Form1() {
            InitializeComponent();
            GeneratePaintTable();
        }

        private void GenerateTable(int columnCount, int rowCount) {
            tileData = new char[columnCount][];
            for (int i = 0; i < columnCount; i++) {
                tileData[i] = new char[rowCount];
            }

            labels = new MyLabel[columnCount][];
            for (int i = 0; i < columnCount; i++) {
                labels[i] = new MyLabel[rowCount];
            }

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnCount = columnCount;
            tableLayoutPanel1.RowCount = rowCount;
            tableLayoutPanel1.AutoScroll = true;

            for(int x = 0; x < columnCount; x++) {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Width = 50));
                for(int y = 0; y < rowCount; y++) {
                    if(x == 0) {
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, Height = 50));
                    }
                    MyLabel cmd = new MyLabel {
                        Width = 40,
                        Height = 40,
                        Text = string.Format("{0}, {1}", x, y),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    labels[x][y] = cmd;
                    
                    if (selectedColor != null) {
                        cmd.BackColor = selectedColor;
                        cmd.TileType = selectedTileType;
                    }
                    cmd.Click += Label_Click;
                    tableLayoutPanel1.Controls.Add(cmd, x, y);
                }
            }
        }

        void GenerateTileData() {
            for(int i = 0; i < labels.Length; i++) {
                for(int j = 0; j < labels[i].Length; j++) {
                    tileData[i][j] = labels[i][j].TileType;
                }
            }
        }

        private void GeneratePaintTable() {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.ColumnStyles.Clear();
            tableLayoutPanel2.RowStyles.Clear();
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.AutoScroll = true;

            for (int x = 0; x < 3; x++) {
                tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Width = 40));
                for (int y = 0; y < 3; y++) {
                    if (x == 0) {
                        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, Height = 40));
                    }
                    MyLabel cmd = new MyLabel {
                        Width = 35,
                        Height = 35,
                        BackColor = myColors[colorCount],
                        TileType = tileTypes[colorCount],
                        Text = "" + tileTypes[colorCount],
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    colorCount++;
                    if (colorCount >= myColors.Length) {
                        colorCount = 0;
                    }
                    cmd.Click += ColorSelect_Click;
                    tableLayoutPanel2.Controls.Add(cmd, x, y);
                }
            }
        }

        private void ColorSelect_Click(object sender, EventArgs e) {
            MyLabel toChange = sender as MyLabel;
            selectedColor = toChange.BackColor;
            selectedTileType = toChange.TileType;
            label6.BackColor = selectedColor;
            label6.Text = " " + selectedTileType;
        }

        private void Label_Click(object sender, EventArgs e) {
            MyLabel toChange = sender as MyLabel;
            if (selectedColor != null) {
                toChange.BackColor = selectedColor;
                toChange.TileType = selectedTileType;
            } else {
                toChange.BackColor = Color.White;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            inputColumnsString = textBox1.Text;
            if (!Int32.TryParse(inputColumnsString, out inputColums)){
                inputColums = 1;
            }
            inputRowString = textBox2.Text;
            if (!Int32.TryParse(inputRowString, out inputRows)){
                inputRows = 1;
            }
            GenerateTable(inputColums, inputRows);

        }

        private void button2_Click(object sender, EventArgs e) {//Export tile map
            GenerateTileData();
            TileMap myMap = new TileMap(tileData);
            //myMap.DisplayTileMapSimple();
            string fileNamePart1 ="D:/LevelData";
            string fileNameProvided = "/" + textBox3.Text;
            string fileNamePart3 = ".bin";
            string filePath = fileNamePart1 + fileNameProvided + fileNamePart3;
            lastSavedFile = filePath;

            Stream sfs = File.Create(filePath);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(sfs, myMap);
            sfs.Flush();
            sfs.Close();
           
            MessageBox.Show("tile map exported!");
        }

        private void button3_Click(object sender, EventArgs e) {
            TileMap loadedData;

            if (File.Exists(lastSavedFile)) {
                Console.WriteLine("Reading saved file");
                Stream openFileStream = File.OpenRead(lastSavedFile);
                BinaryFormatter deserializer = new BinaryFormatter();
                loadedData = (TileMap)deserializer.Deserialize(openFileStream);
                //loadedData.DisplayTileMapSimple();
            }
        }
    }


