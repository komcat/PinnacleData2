using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace PinnacleData2
{
    public class FormManager
    {
        private TextBox dataInputTextBox;
        private TextBox serialNumberTextBox;
        private DataGridView resultDataGridView;
        private ListBox serialNumberListBox;
        private Label currentFileLabel;
        private DataExtractor dataExtractor;
        private Dictionary<string, DataTable> processedData;
        private string currentFilePath;

        public FormManager(TextBox dataInput, TextBox serialNumber, DataGridView dataGridView, ListBox listBox, Label fileLabel)
        {
            dataInputTextBox = dataInput;
            serialNumberTextBox = serialNumber;
            resultDataGridView = dataGridView;
            serialNumberListBox = listBox;
            currentFileLabel = fileLabel;
            dataExtractor = new DataExtractor();
            processedData = new Dictionary<string, DataTable>();
            currentFilePath = null;
        }

        public void ProcessData()
        {
            string data = dataInputTextBox.Text;
            string serialNumber = serialNumberTextBox.Text;

            if (string.IsNullOrWhiteSpace(data) || string.IsNullOrWhiteSpace(serialNumber))
            {
                MessageBox.Show("Please enter both data and serial number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable extractedData = dataExtractor.ExtractData(data);

            // Store the processed data
            processedData[serialNumber] = extractedData;

            // Add serial number to ListBox
            serialNumberListBox.Items.Add(serialNumber);

            // Populate the DataGridView
            resultDataGridView.DataSource = extractedData;

            MessageBox.Show($"Data processed successfully!\nSerial Number: {serialNumber}\nRows extracted: {extractedData.Rows.Count}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Auto-save
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                SaveData(currentFilePath);
            }
            else
            {
                // If no file is currently loaded, create a new one
                SaveDataAs();
            }

            ClearInputs();
        }

        public void ClearForm()
        {
            ClearInputs();
            resultDataGridView.DataSource = null;
            serialNumberListBox.Items.Clear();
            processedData.Clear();
            currentFilePath = null;
            UpdateCurrentFileLabel();
        }

        private void ClearInputs()
        {
            dataInputTextBox.Clear();
            serialNumberTextBox.Clear();
        }

        public void DisplaySelectedData(string selectedSerialNumber)
        {
            if (processedData.TryGetValue(selectedSerialNumber, out DataTable selectedData))
            {
                resultDataGridView.DataSource = selectedData;
            }
        }

        public void SaveData(string filePath)
        {
            var dataToSave = new Dictionary<string, List<Dictionary<string, object>>>();

            foreach (var kvp in processedData)
            {
                var serialNumber = kvp.Key;
                var dataTable = kvp.Value;
                if (dataTable == null) continue;

                var rows = new List<Dictionary<string, object>>();

                foreach (DataRow row in dataTable.Rows)
                {
                    var rowDict = new Dictionary<string, object>();
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        rowDict[col.ColumnName] = row[col] ?? DBNull.Value;
                    }
                    rows.Add(rowDict);
                }

                dataToSave[serialNumber] = rows;
            }

            string jsonData = JsonConvert.SerializeObject(dataToSave, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, jsonData);

            currentFilePath = filePath;
            UpdateCurrentFileLabel();
        }

        public void SaveDataAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveData(saveFileDialog.FileName);
                MessageBox.Show("Data saved successfully!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void LoadData(string filePath)
        {
            string jsonData = File.ReadAllText(filePath);
            var loadedData = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonData);

            processedData.Clear();
            serialNumberListBox.Items.Clear();

            foreach (var kvp in loadedData)
            {
                var serialNumber = kvp.Key;
                var rows = kvp.Value;

                DataTable dataTable = new DataTable();
                if (rows.Count > 0)
                {
                    foreach (var columnName in rows[0].Keys)
                    {
                        dataTable.Columns.Add(columnName);
                    }

                    foreach (var row in rows)
                    {
                        dataTable.Rows.Add(row.Values.ToArray());
                    }
                }

                processedData[serialNumber] = dataTable;
                serialNumberListBox.Items.Add(serialNumber);
            }

            if (serialNumberListBox.Items.Count > 0)
            {
                serialNumberListBox.SelectedIndex = 0;
            }

            currentFilePath = filePath;
            UpdateCurrentFileLabel();
        }

        private void UpdateCurrentFileLabel()
        {
            currentFileLabel.Text = string.IsNullOrEmpty(currentFilePath)
                ? "No file loaded"
                : $"Current file: {Path.GetFileName(currentFilePath)}";
        }
    }
}
