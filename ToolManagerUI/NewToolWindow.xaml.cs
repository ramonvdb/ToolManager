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
using System.Text.RegularExpressions;

namespace ToolManagerUI
{
    /// <summary>
    /// Interaction logic for NewToolWindow.xaml
    /// </summary>
    public partial class NewToolWindow : Window
    {

        bool raiseClosingEvent;

        public NewToolWindow()
        {
            InitializeComponent();
            InitializeComboBox();
            raiseClosingEvent = true;

        }

        // Only allow numeric input for "Diameter/Length"
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //
        // ---Create new tool when accepting input---
        //
        private void AcceptInput(object sender, RoutedEventArgs e)
        {
            bool userInputOK = CheckUserInput();
           
            if (userInputOK == false)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {   
                // create new tool instance
                ToolModel newTool = new ToolModel();

                // Create new toolID
                string toolID = CreateToolID();

                if (SqliteDataAcces.SearchDataByText("ToolID", "Tools", "ToolID", toolID).ToList().Count > 0)
                {
                    ToolModel existingTool = SqliteDataAcces.SearchDataByText("Description", "Tools", "ToolID", toolID)[0];
                    MessageBox.Show($"Tool already exists!\n\nToolID: {existingTool.ToolID}\nDescription: {existingTool.Description}", "Existing tool", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {                  
                    // Assign info to newTool
                    newTool.ToolID = Convert.ToInt32(toolID);
                    newTool.Description = DescriptionTextBox.Text;
                    newTool.Type = TypeComboBox.Text;
                    newTool.Function = FunctionComboBox.Text;
                    newTool.Diameter = Convert.ToDouble(DiameterTextBox.Text);
                    newTool.Length = Convert.ToDouble(LengthTextBox.Text);
                    newTool.Holder = HolderComboBox.Text;
                    newTool.Machine = MachineComboBox.Text;
                    newTool.Storage = StorageTextBox.Text;
                    newTool.HolderVendor = HolderVendorComboBox.Text;
                    newTool.HolderArt = HolderArtTextBox.Text;
                    newTool.ExtensionVendor = ExtensionVendorComboBox.Text;
                    newTool.ExtensionArt = ExtensionArtTextBox.Text;
                    newTool.ToolVendor = ToolVendorComboBox.Text;
                    newTool.ToolArt = ToolArtTextBox.Text;

                    // Write new tool to Database and check if succesfull
                    bool writingSuccesFull = SqliteDataAcces.NewToolRecord(newTool);

                    if (writingSuccesFull)
                    {
                        MessageBox.Show($"Succesfully created new tool.", "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Close window
                        raiseClosingEvent = false;
                        Close();

                        //Refresh data grid
                        ((MainWindow)Owner).SetDataGrid();

                    }
                    else
                    {
                        MessageBox.Show($"Failed to create new tool!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            
        }
       
        private string CreateToolID()
        {
            string toolID = null;

            // Get code for holder type
            if (TypeComboBox.Text == "Milling")
            {
                List<string> holderCode = SqliteDataAcces.SearchSingleValueByText("HolderCode_mill", "Holders", "HolderName", HolderComboBox.Text);
                toolID += holderCode[0];
            }
            else
            {
                List<string> holderCode = SqliteDataAcces.SearchSingleValueByText("HolderCode_turn", "Holders", "HolderName", HolderComboBox.Text);
                toolID += holderCode[0];
            }

            // Get code for diameter and length
            toolID += GetDiameter();
            toolID += GetLength();

            // Get code for function type
            if (TypeComboBox.Text == "Milling")
            {
                List<string> functionCode = SqliteDataAcces.SearchSingleValueByText("FunctionCode", "ToolFunctions_milling", "Function", FunctionComboBox.Text);
                toolID += functionCode[0];
            }
            else
            {
                List<string> functionCode = SqliteDataAcces.SearchSingleValueByText("FunctionCode", "ToolFunctions_turning", "Function", FunctionComboBox.Text);
                toolID += functionCode[0];
            }
            return toolID;
        }



        private string GetDiameter()
        {
            // Get diameter and convert to right output format
            double diameterInput = Convert.ToDouble(DiameterTextBox.Text);
            string diameterCode;

            if (diameterInput >= 10.0)
            {
                diameterCode = (diameterInput / 0.1).ToString();
            }
            else
            {
                diameterCode = "0" + (diameterInput / 0.1).ToString();
            }

            return diameterCode;
              
        }

        private string GetLength()
        {
            // Get diameter and convert to right output format
            int lengthInput = Convert.ToInt32(Math.Round(Convert.ToDouble(LengthTextBox.Text)));
            string lengthCode;
            
            if (lengthInput < 10)
            {
                lengthCode = "00" + (lengthInput.ToString());
            }
            else if (lengthInput < 100)
            {
                lengthCode = "0" + (lengthInput.ToString());
            }
            else
            {
                lengthCode = lengthInput.ToString();
            }
            return lengthCode;
            
        }

        private int CheckComboBox(ComboBox comboBox, Rectangle rectangle)
        { 
            //Check user input of combobox and toggle corresponding rectangle
            if (comboBox.Text == "" | comboBox.Text == "Select")
            {
                rectangle.Visibility = Visibility.Visible;
                return 1;
            }
            else
            {
                rectangle.Visibility = Visibility.Hidden;
                return 0;
            }
        }
        private int CheckTextBox(TextBox textBox, Rectangle rectangle)
        {
            //Get user input
            string userInput = textBox.Text.Replace(".", string.Empty);

            //Check user input of combobox and toggle corresponding rectangle
            if (userInput == "")
            {
                rectangle.Visibility = Visibility.Visible;
                return 1;
            }
            else
            {
                rectangle.Visibility = Visibility.Hidden;
                return 0;
            }
        }

        private int CheckNumberTextBox(int maxValue, TextBox textBox, Rectangle rectangle)
        {
            //Get user input
            string userInput = textBox.Text.Replace(".", string.Empty);

            //Check user input of combobox and toggle corresponding rectangle
            if (userInput == "")
            {
                rectangle.Visibility = Visibility.Visible;
                return 1;
            }
            else if(Convert.ToInt32(Math.Round(Convert.ToDouble(textBox.Text))) > maxValue)
            {
                rectangle.Visibility = Visibility.Visible;
                return 1;
            }
            else
            {
                rectangle.Visibility = Visibility.Hidden;
                return 0;
            }
            
        }

        private bool CheckUserInput()
        {
            int invalidInputCount = 0;

            //Check critical comboboxes
            invalidInputCount += CheckComboBox(TypeComboBox,TypeRec);
            invalidInputCount += CheckComboBox(FunctionComboBox, FunctionRec);
            invalidInputCount += CheckComboBox(HolderComboBox, HolderRec);
            invalidInputCount += CheckComboBox(MachineComboBox, MachineRec);

            //Check critical textfields
            invalidInputCount += CheckNumberTextBox(99, DiameterTextBox, DiameterRec);
            invalidInputCount += CheckNumberTextBox(999, LengthTextBox, LengthRec);
            invalidInputCount += CheckTextBox(DescriptionTextBox, DescriptionRec);

            if (invalidInputCount == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //
        // ---Handle Comboboxes---
        //
        private void InitializeComboBox()
        {
            // Load Type list data and assign to combobox
            List<string> typeData = SqliteDataAcces.LoadListData("ToolType", "ToolTypes");
            typeData = typeData.OrderBy(q => q).Distinct().ToList();
            TypeComboBox.ItemsSource = typeData;

            // Load Holder list data and assign to combobox
            List<string> holderData = SqliteDataAcces.LoadListData("HolderName", "Holders");
            holderData = holderData.OrderBy(q => q).Distinct().ToList();
            HolderComboBox.ItemsSource = holderData;

            // Load machine list data and assign to combobox
            List<string> machineData = SqliteDataAcces.LoadListData("MachineName", "Machines");
            machineData = machineData.OrderBy(q => q).Distinct().ToList();
            MachineComboBox.ItemsSource = machineData;

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

        private void RefreshComboBoxes(object sender, EventArgs e)
        {
            // Load function list data and assign to combobox
            if (TypeComboBox.Text == "Milling")
            {
                List<string> functionData = SqliteDataAcces.SearchDataByNumeric("Function", "ToolFunctions_milling", "ToolTypeID", "1");
                functionData = functionData.OrderBy(q => q).Distinct().ToList();
                FunctionComboBox.ItemsSource = functionData;
            }
            else if (TypeComboBox.Text == "Turning")
            {
                List<string> functionData = SqliteDataAcces.SearchDataByNumeric("Function", "ToolFunctions_turning", "ToolTypeID", "2");
                functionData = functionData.OrderBy(q => q).Distinct().ToList();
                FunctionComboBox.ItemsSource = functionData;
            }
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

        
    }
}
