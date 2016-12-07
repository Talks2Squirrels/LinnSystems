using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinnSystemConsignmentApp
{
    class Consignment
    {
        public int? ConsignmentID { get; set; }      //ready to receive ID from database
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ConsignmentDate { get; set; }
        public List<Package> ConsignmentPackages { get; set; }

        public Consignment()
        {
            ConsignmentDate = DateTime.Now;
        }

        //string address1, string address2, string address3, string city, string postcode, string countryCode, 
        //string phoneNo, DateTime consignmentDate,
        public void CreateConsignment(Consignment createdConsignment)
        {
            //create the datatables that are going to be passed into the stored procedure
            DataTable packageDataTable = new DataTable();
            packageDataTable.Columns.Add("TempPackageID", typeof(int));     //"Dummy" packageID for use when creting in the stored procedure so can get the correct items
            packageDataTable.Columns.Add("PackageWidth", typeof(decimal));
            packageDataTable.Columns.Add("PackageHeight", typeof(decimal));
            packageDataTable.Columns.Add("PackageDepth", typeof(decimal));
            packageDataTable.Columns.Add("PackageType", typeof(string));

            DataTable itemDataTable = new DataTable();
            itemDataTable.Columns.Add("TempPackageID", typeof(int));    //"Dummy" packageID used to link the items to the correct package.
            itemDataTable.Columns.Add("ItemCode", typeof(string));
            itemDataTable.Columns.Add("Quantity", typeof(int));
            itemDataTable.Columns.Add("UnitWeight", typeof(decimal));

            List<Package> CurrentConsignmentPackages = createdConsignment.ConsignmentPackages;

            for (int i = 0; i < CurrentConsignmentPackages.Count; i++)
            {
                Package consignmentPackage = CurrentConsignmentPackages[i];
                DataRow consignmentPackageRow = packageDataTable.NewRow();
                consignmentPackageRow["TempPackageID"] = i + 1;
                consignmentPackageRow["PackageWidth"] = consignmentPackage.PackageWidth;
                consignmentPackageRow["PackageHeight"] = consignmentPackage.PackageHeight;
                consignmentPackageRow["PackageDepth"] = consignmentPackage.PackageDepth;
                consignmentPackageRow["PackageType"] = consignmentPackage.PackageType;
                packageDataTable.Rows.Add(consignmentPackageRow);
                
                //loop the items associated with this package and create a datatable of the items added to this package
                foreach (Item packagedItem in consignmentPackage.PackageItems)
                {
                    DataRow packagedItemRow = itemDataTable.NewRow();
                    packagedItemRow["TempPackageID"] = i + 1;
                    packagedItemRow["ItemCode"] = packagedItem.ItemCode;
                    packagedItemRow["Quantity"] = packagedItem.Quantity;
                    packagedItemRow["UnitWeight"] = packagedItem.UnitWeight;
                    itemDataTable.Rows.Add(packagedItemRow);
                }
            }

            //get the connection string from the config file
            string connectionString = ConfigurationManager.ConnectionStrings["LinnSystemsConsignmentConnectionString"].ToString();

            //create a connection to the SQL server and run the stored procedure to add the consignment etc
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "dbo.AddConsignment";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@address1", SqlDbType.NVarChar).Value = createdConsignment.Address1;
            command.Parameters.Add("@address2", SqlDbType.NVarChar).Value = createdConsignment.Address2;
            command.Parameters.Add("@address3", SqlDbType.NVarChar).Value = createdConsignment.Address3;
            command.Parameters.Add("@city", SqlDbType.NVarChar).Value = createdConsignment.City;
            command.Parameters.Add("@phoneNumber", SqlDbType.NVarChar).Value = createdConsignment.PhoneNumber;
            command.Parameters.Add("@countryCode", SqlDbType.NVarChar).Value = createdConsignment.CountryCode;
            command.Parameters.Add("@postCode", SqlDbType.NVarChar).Value = createdConsignment.PostCode;
            command.Parameters.Add("@consignmentDate", SqlDbType.SmallDateTime).Value = createdConsignment.ConsignmentDate;

            //add the parameters to the command - package list
            SqlParameter packageListParam = new SqlParameter();
            packageListParam = command.Parameters.AddWithValue("@packageList", packageDataTable);
            packageListParam.SqlDbType = SqlDbType.Structured;
            packageListParam.TypeName = "dbo.PackageTable";

            //items list
            SqlParameter itemListParam = new SqlParameter();
            packageListParam = command.Parameters.AddWithValue("@itemList", itemDataTable);
            packageListParam.SqlDbType = SqlDbType.Structured;
            packageListParam.TypeName = "dbo.ItemTable";

            command.ExecuteNonQuery();
        }

        public DataSet GetConsignmentInformation(int consignmentId)
        {
            //get the connection string from the config file
            string connectionString = ConfigurationManager.ConnectionStrings["LinnSystemsConsignmentConnectionString"].ToString();
            DataSet consignmentDetail = new DataSet();
            SqlDataAdapter consignmentAdapter = new SqlDataAdapter("dbo.GetConsignment", connectionString);
            consignmentAdapter.SelectCommand.Parameters.Add("@consignmentID", SqlDbType.Int).Value = consignmentId;
            consignmentAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            consignmentAdapter.Fill(consignmentDetail);

            return consignmentDetail;
        }
    }
}
