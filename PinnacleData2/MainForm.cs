using PinnacleData2;
using System;
using System.Windows.Forms;

namespace PinnacleData2
{
    public partial class MainForm : Form
    {
        private FormManager formManager;

        // Declare the controls
        private TextBox DataInputTextBox;
        private TextBox SerialNumberTextBox;
        private DataGridView ResultDataGridView;
        private ListBox SerialNumberListBox;
        private Label CurrentFileLabel;

        // You might also want to add these buttons and menu items
        private Button? ProcessButton;
        private Button? ClearButton;
        private ToolStripMenuItem? SaveMenuItem;
        private ToolStripMenuItem? LoadMenuItem;
        private void InitializeControls()
        {
            // Create and configure DataInputTextBox
            DataInputTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 20),
                Size = new Size(300, 100)
            };
            this.Controls.Add(DataInputTextBox);

            // Create and configure SerialNumberTextBox
            SerialNumberTextBox = new TextBox
            {
                Location = new Point(10, 130),
                Size = new Size(150, 20)
            };
            this.Controls.Add(SerialNumberTextBox);

            // Create and configure ResultDataGridView
            ResultDataGridView = new DataGridView
            {
                Location = new Point(10, 160),
                Size = new Size(500, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(ResultDataGridView);

            // Create and configure SerialNumberListBox
            SerialNumberListBox = new ListBox
            {
                Location = new Point(520, 10),
                Size = new Size(150, 340)
            };
            SerialNumberListBox.SelectedIndexChanged += SerialNumberListBox_SelectedIndexChanged;
            this.Controls.Add(SerialNumberListBox);

            // Create and configure CurrentFileLabel
            CurrentFileLabel = new Label
            {
                Location = new Point(10, 360),
                Size = new Size(300, 20),
                Text = "No file loaded"
            };
            this.Controls.Add(CurrentFileLabel);

            // Create and configure ProcessButton
            ProcessButton = new Button
            {
                Text = "Process Data",
                Location = new Point(320, 10),
                Size = new Size(100, 30)
            };
            ProcessButton.Click += ProcessButton_Click;
            this.Controls.Add(ProcessButton);

            // Create and configure ClearButton
            ClearButton = new Button
            {
                Text = "Clear Form",
                Location = new Point(320, 50),
                Size = new Size(100, 30)
            };
            ClearButton.Click += ClearButton_Click;
            this.Controls.Add(ClearButton);

            // Create MenuStrip for SaveMenuItem and LoadMenuItem
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");

            // Create and configure SaveMenuItem
            SaveMenuItem = new ToolStripMenuItem("Save");
            SaveMenuItem.Click += SaveMenuItem_Click;
            fileMenu.DropDownItems.Add(SaveMenuItem);

            // Create and configure LoadMenuItem
            LoadMenuItem = new ToolStripMenuItem("Load");
            LoadMenuItem.Click += LoadMenuItem_Click1; ;
            fileMenu.DropDownItems.Add(LoadMenuItem);

            menuStrip.Items.Add(fileMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void LoadMenuItem_Click1(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                formManager.LoadData(openFileDialog.FileName);
                MessageBox.Show("Data loaded successfully!", "Load Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            InitializeControls();
            if (DataInputTextBox != null && SerialNumberTextBox != null && ResultDataGridView != null &&
                SerialNumberListBox != null && CurrentFileLabel != null)
            {
                formManager = new FormManager(DataInputTextBox, SerialNumberTextBox, ResultDataGridView, SerialNumberListBox, CurrentFileLabel);
            }
            else
            {
                throw new InvalidOperationException("One or more required controls are not initialized.");
            }
        }

        private void ProcessButton_Click(object? sender, EventArgs e)
        {
            formManager.ProcessData();
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            formManager.ClearForm();
        }

        private void SerialNumberListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (SerialNumberListBox.SelectedItem is string selectedSerialNumber)
            {
                formManager.DisplaySelectedData(selectedSerialNumber);
            }
        }

        private void SaveMenuItem_Click(object? sender, EventArgs e)
        {
            formManager.SaveDataAs();
        }



        private void MainForm_Load(object? sender, EventArgs e)
        {

        }

        // Add InitializeComponent method and other necessary Windows Forms designer-generated code here
    }
}