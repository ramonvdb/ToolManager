using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToolManagerLibrary;

namespace ToolManagerUI
{
    /// <summary>
    /// Interaction logic for EditToolWindow.xaml
    /// </summary>
    public partial class EditToolWindow : Window
    {
        public string toolID { get; set; }
        bool raiseClosingEvent;
        bool inputCorrect = true;
        ToolModel selectedTool;

        public EditToolWindow(int toolID)
        {
            this.toolID = toolID.ToString();

            InitializeComponent();
            FillForm();
            InitializeVendorComboBox();
            raiseClosingEvent = true;
        }
 
        private void AcceptInput(object sender, RoutedEventArgs e)
        {

            if (!inputCorrect)
            {
                MessageBox.Show("Description field can't be empty.", "Invalid description input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                // create new tool instance
                ToolModel updateTool = new ToolModel();


                if (SqliteDataAcces.SearchDataByText("ToolID", "Tools", "ToolID", toolID).ToList().Count == 0)
                {
                    MessageBox.Show("Tool not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    // Assign info to updateTool
                    updateTool.ToolID = Convert.ToInt32(toolID);
                    updateTool.Storage = StorageTextBox.Text;
                    updateTool.Location = LocationTextBox.Text;
                    updateTool.Description = DescriptionTextBox.Text;
                    updateTool.HolderVendor = HolderVendorComboBox.Text;
                    updateTool.HolderArt = HolderArtTextBox.Text;
                    updateTool.ExtensionVendor = ExtensionVendorComboBox.Text;
                    updateTool.ExtensionArt = ExtensionArtTextBox.Text;
                    updateTool.ToolVendor = ToolVendorComboBox.Text;
                    updateTool.ToolArt = ToolArtTextBox.Text;

                    

                    // Write new tool to Database and check if succesfull
                    bool writingSuccesFull = SqliteDataAcces.UpdateToolRecord(updateTool);

                    if (writingSuccesFull)
                    {
                        MessageBox.Show($"Changes succesfully written to database.", "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        //Close window
                        raiseClosingEvent = false;
                        Close();

                        //Refresh data grid
                        ((MainWindow)Owner).SetDataGrid();
                    }
                    else
                    {
                        MessageBox.Show($"Failed update tool!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void FillForm()
        {
            //Get data from selected tool
            selectedTool = SqliteDataAcces.LoadToolDataByID(Convert.ToInt32(toolID));

            //Fill in readonly data
            ToolTextBlock.Text = $"Edit Tool - {toolID}";
            TypeTextBlock.Text = selectedTool.Type;
            FunctionTextBlock.Text = selectedTool.Function;
            HolderTextBlock.Text = selectedTool.Holder;
            MachineTextBlock.Text = selectedTool.Machine;
            DiameterTextBlock.Text = selectedTool.Diameter.ToString("F");
            LengthTextBlock.Text = selectedTool.Length.ToString("F");
            StorageTextBox.Text = selectedTool.Storage;
            LocationTextBox.Text = selectedTool.Location;
            DescriptionTextBox.Text = selectedTool.Description;
            HolderArtTextBox.Text = selectedTool.HolderArt;
            ExtensionArtTextBox.Text = selectedTool.ExtensionArt;
            ToolArtTextBox.Text = selectedTool.ToolArt;

            if (selectedTool.HolderVendor != "-" && selectedTool.HolderVendor != "") 
            { 
                HolderVendorComboBox.Text = selectedTool.HolderVendor;
                HolderVendorComboBox.SelectedItem = selectedTool.HolderVendor;
            }
            if (selectedTool.ExtensionVendor != "-" && selectedTool.ExtensionVendor != "") 
            { 
                ExtensionVendorComboBox.Text = selectedTool.ExtensionVendor;
                ExtensionVendorComboBox.SelectedItem = selectedTool.ExtensionVendor;
            }
            if (selectedTool.ToolVendor != "-" && selectedTool.ToolVendor != "") 
            { 
                ToolVendorComboBox.Text = selectedTool.ToolVendor;
                ToolVendorComboBox.SelectedItem = selectedTool.ToolVendor;
            }

        }

        private void InitializeVendorComboBox()
        {
        // Load function list data and assign to combobox
        List<string> holderVendorData = SqliteDataAcces.LoadListData("HolderVendor", "Tools");
        List<string> extensionVendorData = SqliteDataAcces.LoadListData("ExtensionVendor", "Tools");
        List<string> toolVendorData = SqliteDataAcces.LoadListData("ToolVendor", "Tools");

        // Combine vendorData
        var vendorData = holderVendorData.Concat(extensionVendorData)
                                .Concat(toolVendorData)
                                .ToList();

        // Sort data and Remove duplicates
        vendorData = vendorData.OrderBy(q => q).Distinct().ToList();

        // Assign data to vendor comboboxes
        HolderVendorComboBox.ItemsSource = vendorData;
        ExtensionVendorComboBox.ItemsSource = vendorData;
        ToolVendorComboBox.ItemsSource = vendorData;
        }



        //
        // ---Exit window and cancel handling---
        //

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Check if closing message has to be raised
            if (raiseClosingEvent)
            {
                const string message =
                    "Are you sure you want to close without saving?";
                const string caption = "Closing without saving";
                var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Information);

                // If the no button was pressed ...
                if (result == MessageBoxResult.No)
                {
                    // cancel the closure of the form.
                    e.Cancel = true;
                }
            }

        }

        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            //Check user input and show rectangle if empty
            if (DescriptionTextBox.Text == "")
            {
                DescriptionRec.Visibility = Visibility.Visible;
                inputCorrect = false;
            }
            else
            {
                DescriptionRec.Visibility = Visibility.Hidden;
                inputCorrect = true;
            }
        }
    }



}
