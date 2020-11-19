using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using ToolManagerLibrary;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;

namespace ToolManagerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int toolID;

        public MainWindow()
        {

            InitializeComponent();

            InitializeComboBox();

            SetDataGrid();
        }

        // Fetch SQLite data and bind to ToolDataGrid
        private void LoadDataGrid(List<ToolModel> toolData)
        {
            //Display data and sort
            ToolDataGrid.DataContext = toolData;
            ToolDataGrid.Items.SortDescriptions.Add(new SortDescription("Description", ListSortDirection.Ascending));

            //Select first row
            ToolDataGrid.SelectedIndex = 0;
        }

        private void InitializeComboBox()
        {
            // Load machine list data and assign to combobox
            List<string> machineData = SqliteDataAcces.LoadListData("Machine", "Tools");
            machineData = machineData.OrderBy(q => q).Distinct().ToList();
            machineComboBox.ItemsSource = machineData;

            // Load type list data and assign to combobox
            List<string> typeData = SqliteDataAcces.LoadComboBoxData("Type", "Machine", machineComboBox.Text);
            typeData = typeData.OrderBy(q => q).Distinct().ToList();
            typeComboBox.ItemsSource = typeData;

            // Load type list data and assign to combobox
            List<string> functionData = SqliteDataAcces.LoadComboBoxData("Function", "Type", typeComboBox.Text);
            functionData = functionData.OrderBy(q => q).Distinct().ToList();
            functionComboBox.ItemsSource = functionData;

        }

        //
        // ----Search box----
        //
        // Clear Searchbox at focus
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
        }

        // Seach tool by user input
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text != "Search Tool..." && SearchBox.Text.Length > 0)
            {
                // Check if only numbers
                bool digitsOnly = SearchBox.Text.All(char.IsDigit);

                // Load data from SQLite
                var input = SqliteDataAcces.LoadAllToolData();

                // Create new collection
                var searchData = new ObservableCollection<ToolModel>();

                // Apply filter for every item
                foreach (var item in input)
                {
                    if (digitsOnly)
                    {
                        if (item.ToolID.ToString().Contains(SearchBox.Text))
                        {
                            searchData.Add(item);
                        }
                    }
                    else
                    {
                        string[] searchValues = SearchBox.Text.ToLower().Split(null);
                        int hitCounter = 0;

                        foreach (string value in searchValues)
                        {
                            if (item.Description.ToLower().Contains(value))
                            {
                                hitCounter += 1;
                            }

                            if (hitCounter == searchValues.Length)
                            {
                                searchData.Add(item);
                            }
                        }

                    }

                }
                //Display data
                ToolDataGrid.DataContext = searchData;
            }
        }

        // Reset searchbox if lost focus and empty
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "")
            {
                SearchBox.Text = "Search Tool...";
            }
        }

        // Update tool Selection
        private void ToolDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ToolDataGrid.SelectedIndex != -1)
            {
                // Get toolData from ToolDataGrid
                ToolModel selectedTool = (ToolModel)ToolDataGrid.SelectedItems[0];

                // Get toolID from toolData
                toolID = selectedTool.ToolID;

                // Load ToolData from SQLite DB
                var toolData = SqliteDataAcces.LoadToolDataByID(toolID);

                // Fill tool information
                functionTextBlock.Text = toolData.Function;
                typeTextBlock.Text = toolData.Type;
                holderTextBlock.Text = toolData.Holder;
                machineTextBlock.Text = toolData.Machine;
                diameterTextBlock.Text = toolData.Diameter.ToString("F");
                lengthTextBlock.Text = toolData.Length.ToString("F");
                storageTextBlock.Text = toolData.Storage;
                locationTextBlock.Text = toolData.Location;

                holderVendorTextBlock.Text = toolData.HolderVendor;
                holderArtTextBlock.Text = toolData.HolderArt;

                extensionVendorTextBlock.Text = toolData.ExtensionVendor;
                extensionArtTextBlock.Text = toolData.ExtensionArt;

                toolVendorTextBlock.Text = toolData.ToolVendor;
                toolArtTextBlock.Text = toolData.ToolArt;
            }
        }



        // 
        // ----Selection Filters----
        //
        private void AllButton_Click(object sender, RoutedEventArgs e)
        {
            SetDataGrid();
        }

        public void SetDataGrid()
        {
            LoadDataGrid(new ToolList().Complete());

            //Select first row
            ToolDataGrid.SelectedIndex = 0;

            //Deselect machine combobox
            machineComboBox.SelectedIndex = -1;
            machineComboBox.Text = "Machine";

            //Deselect type combobox
            typeComboBox.SelectedIndex = -1;
            typeComboBox.Text = "Type";

            //Deselect function combobox
            functionComboBox.SelectedIndex = -1;
            functionComboBox.Text = "Function";

            //Reset searchbar
            SearchBox.Text = "Search Tool...";
        }


        private void CheckComboBoxEmpty(ComboBox comboBox)
        {
            //Checks if combox "function" or "type" is empty en resets them if true
            if (comboBox.Text == "")
            {
                if (comboBox.Name.Contains("function"))
                {
                    comboBox.Text = "Function";
                }
                if (comboBox.Name.Contains("type"))
                {
                    comboBox.Text = "Type";
                }
            }
        }


        private void ComboBoxChanged(object sender, EventArgs e)
        {
            // Reload datagrid with applied filters
            var toolData = new ToolList();
            LoadDataGrid(toolData.Filter(machineComboBox.Text, typeComboBox.Text, functionComboBox.Text));

            //reload comboboxes
            InitializeComboBox();

            //Check if combobox empty
            CheckComboBoxEmpty(functionComboBox);
            CheckComboBoxEmpty(typeComboBox);

            //Clear searchbox
            SearchBox.Text = "Search Tool...";

        }

        //
        //---Handle New and Edit button events---
        //
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            Window newToolWindow = new NewToolWindow();
            newToolWindow.Owner = this;
            newToolWindow.ShowDialog();           
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditToolWindow editToolWindow = new EditToolWindow(toolID);
            editToolWindow.Owner = this;
            editToolWindow.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            bool deleteSuccesFull;

            var result = MessageBox.Show($"Are you sure you want to delete tool: {toolID}?", "Delete Tool", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
            {
                deleteSuccesFull =  SqliteDataAcces.DeleteToolData(toolID);

                if (deleteSuccesFull)
                {
                    MessageBox.Show($"Tool {toolID} succesfully deleted.", "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
                    SetDataGrid();
                }
                else
                {
                    MessageBox.Show($"Failed to delete Tool {toolID}!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

            
        }
    }
}
