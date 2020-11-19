using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ToolManagerLibrary
{
    public class SqliteDataAcces
    {

        public static List<ToolModel> LoadAllToolData()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ToolModel>($"SELECT * FROM Tools", new DynamicParameters()); //Returns a IEnumerable of toolModel from SQLlite DB
                return output.ToList(); //Returns output as a list instead of a IEnumerable
            }
        }

        public static List<ToolModel> LoadFilteredData(string machine, string type, string function)
        {
            if (machine == "Machine")
            {
                machine = "";
            }
            if (type == "Type")
            {
                type = "";
            }
            if (function == "Function")
            {
                function = "";
            }

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ToolModel>($"SELECT * FROM Tools WHERE Machine LIKE '%{machine}%' AND Type LIKE '%{type}%' AND Function LIKE '%{function}%'", new DynamicParameters());
                return output.ToList(); //Returns output as a list instead of a IEnumerable
            }

        }

        public static List<string> LoadComboBoxData(string desiredValue, string searchColumn, string searchValue)
        {
            if (searchValue == "Machine")
            {
                searchValue = "";
            }

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"SELECT {desiredValue} FROM Tools WHERE {searchColumn} LIKE '%{searchValue}%'", new DynamicParameters());
                return output.ToList(); //Returns output as a list instead of a IEnumerable
            }

        }

        public static ToolModel LoadToolDataByID(int toolID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                ToolModel output = cnn.Query<ToolModel>("select * from Tools where toolID =" + toolID, new DynamicParameters()).ToList()[0];
                return output; //Returns output as a list instead of a IEnumerable
            }
        }

        public static List<string> LoadListData(string column, string table)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"select {column} from {table}", new DynamicParameters()); 
                return output.ToList(); //Returns output as a list instead of a IEnumerable
            }

        }

        public static List<string> SearchDataByNumeric(string column, string table, string targetColumn, string targetValue)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"select {column} from {table} where {targetColumn} = {targetValue}", new DynamicParameters());
                return output.ToList(); //Returns output as a list instead of a IEnumerable
            }

        }

        public static List<ToolModel> SearchDataByText(string column, string table, string targetColumn, string targetValue)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ToolModel>($"select {column} from {table} where {targetColumn} like '%{targetValue}%'", new DynamicParameters());
                return output.ToList(); //return code of specific tool ID feature
            }

        }
        public static List<string> SearchSingleValueByText(string column, string table, string targetColumn, string targetValue)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"select {column} from {table} where {targetColumn} like '%{targetValue}%'", new DynamicParameters());
                return output.ToList(); //return code of specific tool ID feature
            }

        }

        public static bool NewToolRecord(ToolModel toolModel)
        {
            // Check and Adjust toolmodel data
            if (toolModel.Storage == "")
            {
                toolModel.Storage = "-";
            }
            if (toolModel.HolderVendor == "Select" | toolModel.HolderVendor == "")
            {
                toolModel.HolderVendor = "-";
            }
            if (toolModel.HolderArt == "")
            {
                toolModel.HolderArt = "-";
            }
            if (toolModel.ExtensionVendor == "Select" | toolModel.ExtensionVendor == "")
            {
                toolModel.ExtensionVendor = "-";
            }
            if (toolModel.ExtensionArt == "-")
            {
                toolModel.ExtensionArt = "-";
            }
            if (toolModel.ToolVendor == "Select" | toolModel.ToolVendor == "")
            {
                toolModel.ToolVendor = "-";
            }
            if (toolModel.ToolArt == "")
            {
                toolModel.ToolArt = "-";
            }

            // Create SQL connection
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO Tools (ToolID, Description, Type, Function, Diameter, Length, Holder, Machine, Storage, HolderVendor, HolderArt, ExtensionVendor, ExtensionArt, ToolVendor, ToolArt) " +
                    "VALUES (@ToolID, @Description, @Type, @Function, @Diameter, @Length, @Holder, @Machine, @Storage, @HolderVendor, @HolderArt, @ExtensionVendor, @ExtensionArt, @ToolVendor, @ToolArt)", cnn);

                // Assign parameters
                insertSQL.Parameters.Add("@ToolID", DbType.Int32).Value = toolModel.ToolID;
                insertSQL.Parameters.Add("@Description", DbType.String).Value = toolModel.Description;
                insertSQL.Parameters.Add("@Type", DbType.String).Value = toolModel.Type;
                insertSQL.Parameters.Add("@Function", DbType.String).Value = toolModel.Function;
                insertSQL.Parameters.Add("@Diameter", DbType.String).Value = toolModel.Diameter;
                insertSQL.Parameters.Add("@Length", DbType.String).Value = toolModel.Length;
                insertSQL.Parameters.Add("@Holder", DbType.String).Value = toolModel.Holder;
                insertSQL.Parameters.Add("@Machine", DbType.String).Value = toolModel.Machine;
                insertSQL.Parameters.Add("@Storage", DbType.String).Value = toolModel.Storage;
                insertSQL.Parameters.Add("@HolderVendor", DbType.String).Value = toolModel.HolderVendor;
                insertSQL.Parameters.Add("@HolderArt", DbType.String).Value = toolModel.HolderArt;
                insertSQL.Parameters.Add("@ExtensionVendor", DbType.String).Value = toolModel.ExtensionVendor;
                insertSQL.Parameters.Add("@ExtensionArt", DbType.String).Value = toolModel.ExtensionArt;
                insertSQL.Parameters.Add("@ToolVendor", DbType.String).Value = toolModel.ToolVendor;
                insertSQL.Parameters.Add("@ToolArt", DbType.String).Value = toolModel.ToolArt;

                // Open Database
                cnn.Open();

                // Writing succesfull or not
                bool writingSuccesFull;

                // Try to execute command
                try
                {
                    insertSQL.ExecuteNonQuery();
                    writingSuccesFull = true;

                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        writingSuccesFull = false;
                    }
                    throw;
                }

                //Close Database
                cnn.Close();
                return writingSuccesFull;
            }

        }

        public static bool UpdateToolRecord(ToolModel updateToolModel)
        {
            // Check and Adjust toolmodel data
            if (updateToolModel.Storage == "")
            {
                updateToolModel.Storage = "-";
            }
            if (updateToolModel.Location == "")
            {
                updateToolModel.Location = "-";
            }
            if (updateToolModel.HolderVendor == "Select" | updateToolModel.HolderVendor == "")
            {
                updateToolModel.HolderVendor = "-";
            }
            if (updateToolModel.HolderArt == "")
            {
                updateToolModel.HolderArt = "-";
            }
            if (updateToolModel.ExtensionVendor == "Select" | updateToolModel.ExtensionVendor == "")
            {
                updateToolModel.ExtensionVendor = "-";
            }
            if (updateToolModel.ExtensionArt == "")
            {
                updateToolModel.ExtensionArt = "-";
            }
            if (updateToolModel.ToolVendor == "Select" | updateToolModel.ToolVendor == "")
            {
                updateToolModel.ToolVendor = "-";
            }
            if (updateToolModel.ToolArt == "")
            {
                updateToolModel.ToolArt = "-";
            }

            // Create SQL connection
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                SQLiteCommand updateSQL = new SQLiteCommand("UPDATE Tools SET Description= @Description, Storage= @Storage, Location= @Location, HolderVendor= @HolderVendor," +
                    "HolderArt= @HolderArt, ExtensionVendor= @ExtensionVendor, ExtensionArt= @ExtensionArt, ToolVendor= @ToolVendor, ToolArt= @ToolArt WHERE ToolID= @ToolID", cnn);

                // Assign parameters
                updateSQL.Parameters.Add("@Description", DbType.String).Value = updateToolModel.Description;
                updateSQL.Parameters.Add("@Storage", DbType.String).Value = updateToolModel.Storage;
                updateSQL.Parameters.Add("@Location", DbType.String).Value = updateToolModel.Location;
                updateSQL.Parameters.Add("@HolderVendor", DbType.String).Value = updateToolModel.HolderVendor;
                updateSQL.Parameters.Add("@HolderArt", DbType.String).Value = updateToolModel.HolderArt;
                updateSQL.Parameters.Add("@ExtensionVendor", DbType.String).Value = updateToolModel.ExtensionVendor;
                updateSQL.Parameters.Add("@ExtensionArt", DbType.String).Value = updateToolModel.ExtensionArt;
                updateSQL.Parameters.Add("@ToolVendor", DbType.String).Value = updateToolModel.ToolVendor;
                updateSQL.Parameters.Add("@ToolArt", DbType.String).Value = updateToolModel.ToolArt;
                updateSQL.Parameters.Add("@ToolID", DbType.Int32).Value = updateToolModel.ToolID;

                // Open Database
                cnn.Open();

                // Writing succesfull or not
                bool writingSuccesFull;

                // Try to execute command
                try
                {
                    updateSQL.ExecuteNonQuery();
                    writingSuccesFull = true;

                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        writingSuccesFull = false;
                    }
                    throw;
                }

                //Close Database
                cnn.Close();
                return writingSuccesFull;
            }

        }

        // Deletete tool from table by toolID
        public static bool DeleteToolData(int toolID)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                SQLiteCommand updateSQL = new SQLiteCommand("DELETE FROM Tools WHERE ToolID= @ToolID", cnn);

                // Assign parameters
                updateSQL.Parameters.Add("@ToolID", DbType.Int32).Value = toolID;

                // Open Database
                cnn.Open();

                // Writing succesfull or not
                bool deleteSuccesFull;

                // Try to execute command
                try
                {
                    updateSQL.ExecuteNonQuery();
                    deleteSuccesFull = true;

                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        deleteSuccesFull = false;
                    }
                    throw;
                }

                //Close Database
                cnn.Close();
                return deleteSuccesFull;
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString; //Returns the Connection string as specified in App.Config (WinFormUI)
        }

    }
}


