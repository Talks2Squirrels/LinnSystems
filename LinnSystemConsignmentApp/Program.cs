using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinnSystemConsignmentApp
{
    class Program
    {
        /*
         * I have added the connection string to the app.config file, you can change the settings in there.
         * 
         * Had I more time I would have liked to have altered the item table to include the consignmentID to improve the select query.
         * Given more time I would have implemented more error checking on user input.
         * 
         * To test the system running the program will provide you with a console window where you can enter the consignment, package and item details, by following
         * the prompts on screen.
         */ 
        static void Main(string[] args)
        {
            int chosenMainFunction = DisplayMainMenu();

            switch(chosenMainFunction)
            {
                case 1:
                    {
                        Consignment currentConsignment = GetConsignmentDetail();
                        //move onto the get the package detail
                        currentConsignment = GetPackageDetail(currentConsignment);
                        //call a method/SP to add this consignment to the database
                        new Consignment().CreateConsignment(currentConsignment);

                        DisplayMainMenu();

                        break;
                    }
                case 2:
                    {
                        SearchConsignment();
                        break;
                    }
                case 3:
                    {
                        System.Environment.Exit(-1);
                        break;
                    }
                default:
                    {
                        DisplayMainMenu();
                        break;
                    }
            }
        }

        //method to display the main menu and return the int value chosen
        static public int DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Choose Function.");
            Console.WriteLine("1.     Enter a new consignment.");
            Console.WriteLine("2.     Find a consignment.");
            Console.WriteLine("3.     Exit application.");
            return Convert.ToInt32(Console.ReadLine());
        }

        //method to display the main menu and return the int value chosen
        static public Consignment GetConsignmentDetail()
        {
            Consignment currentConsignment = new Consignment();

            Console.Clear();
            Console.WriteLine("Enter Cosnsignment Details (or press Esc to return to main menu)");
            Console.WriteLine();
            Console.Write("Address 1: ");
            string userInput = readUserInput();

            if(userInput == null)
            {
                DisplayMainMenu();
            }
            else
            {
                currentConsignment.Address1 = userInput;
            }

            Console.WriteLine();
            Console.Write("Address 2: ");
            userInput = readUserInput();

            if(userInput == null)
            {
                DisplayMainMenu();
            }
            else
            {
                currentConsignment.Address2 = userInput;
            }

            Console.WriteLine();
            Console.Write("Address 3: ");
            userInput = readUserInput();

            if (userInput == null)
            {
                DisplayMainMenu();
            }
            else
            {
                currentConsignment.Address3 = userInput;
            }

            Console.WriteLine();
            Console.Write("City: ");
            userInput = readUserInput();

            if (userInput == null)
            {
                DisplayMainMenu();
            }
            else
            {
                currentConsignment.City = userInput;
            }

            Console.WriteLine();
            Console.Write("PostCode: ");
            userInput = readUserInput();

            if (userInput == null)
            {
                DisplayMainMenu();
            }
            else
            {
                currentConsignment.PostCode = userInput;
            }

            Console.WriteLine();
            Console.Write("Country Code: ");
            userInput = readUserInput();

            if (userInput == null)
            {
                DisplayMainMenu();
            }
            else
            {
                currentConsignment.CountryCode = userInput;
            }

            Console.WriteLine();
            Console.Write("Phone No: ");
            userInput = readUserInput();

            if (userInput == null)
            {
                DisplayMainMenu();
            }
            else
            {
                currentConsignment.PhoneNumber = userInput;
            }

            return currentConsignment;
        }

        static public Consignment GetPackageDetail(Consignment currentConsignment)
        {
            //set up a list of packages, which will be added to the consignment after all details (including items) have been retrieved for the packages
            List<Package> consignmentPackages = new List<Package>();

            Console.WriteLine();
            Console.WriteLine("Enter Package(s) Detail (or press ESC to return to main menu)");
            Console.WriteLine();

            //ask user for the number of packages being sent - requires validation!!!
            Console.Write("No. of Packages: ");
            int NoOfPackages = Convert.ToInt32(readUserInput());

            //count up to the no of packages entered, creating a new package each time
            for(int i = 0; i < NoOfPackages; i++)
            {
                //create a new package
                Package currentPackage = new Package();

                //create a list to hold the items in this package
                List<Item> packageItems = new List<Item>();

                int packageNo = i + 1;

                Console.WriteLine();
                Console.WriteLine("Package " + packageNo + " Dimensions: ");
                Console.WriteLine();
                Console.Write("Width: ");
                string userInput = readUserInput();

                if(userInput == null)
                {
                    DisplayMainMenu();
                    //or go back to consignment menu andload consignment etc
                }
                else
                {
                    currentPackage.PackageWidth = decimal.Parse(userInput);
                }

                Console.WriteLine();
                Console.Write("Height: ");
                userInput = readUserInput();

                if(userInput == null)
                {
                    DisplayMainMenu();
                }
                else
                {
                    currentPackage.PackageHeight = decimal.Parse(userInput);
                }

                Console.WriteLine();
                Console.Write("Depth: ");
                userInput = readUserInput();

                if (userInput == null)
                {
                    DisplayMainMenu();
                }
                else
                {
                    currentPackage.PackageDepth = decimal.Parse(userInput);
                }

                Console.WriteLine();
                Console.Write("Package Type: ");
                userInput = readUserInput();

                if (userInput == null)
                {
                    DisplayMainMenu();
                }
                else
                {
                    currentPackage.PackageType = userInput;
                }

                //now need to get the details about the item in this package
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                Console.Write("No of Item(s) in Package: ");    //requires validation!!!
                int noOfItems = Convert.ToInt32(readUserInput());

                for(int j = 0; j < noOfItems; j++)
                {
                    //create a new item
                    Item currentItem = new Item();

                    int itemNo = j + 1;

                    Console.WriteLine();
                    Console.WriteLine("Item " + itemNo + " Details: ");
                    Console.Write("Item Code: ");
                    userInput = readUserInput();

                    if (userInput == null)
                    {
                        DisplayMainMenu();
                    }
                    else
                    {
                        currentItem.ItemCode = userInput;
                    }

                    Console.WriteLine();
                    Console.Write("Quantity: ");
                    userInput = readUserInput();

                    if (userInput == null)
                    {
                        DisplayMainMenu();
                    }
                    else
                    {
                        currentItem.Quantity = Convert.ToInt32(userInput);
                    }

                    Console.WriteLine();
                    Console.Write("Unit Weight: ");
                    userInput = readUserInput();

                    if (userInput == null)
                    {
                        DisplayMainMenu();
                    }
                    else
                    {
                        currentItem.UnitWeight = decimal.Parse(userInput);
                    }

                    //add the item to the list of items
                    packageItems.Add(currentItem);
                }

                //add the item list to the package
                currentPackage.PackageItems = packageItems;

                //add the package to the list of packages
                consignmentPackages.Add(currentPackage);
            }

            //add the package list to the consignment
            currentConsignment.ConsignmentPackages = consignmentPackages;

            //return the newly formed consignment (now with packages)
            return currentConsignment;
        }

        static public void SearchConsignment()
        {
            Console.Clear();
            Console.WriteLine("Get Consignment Information (or ESC to main menu)");

            Console.WriteLine();
            Console.Write("Enter ConsignmentId: ");

            string userInput = readUserInput();

            if(userInput != null)
            {
                DataSet consignmentResults = new Consignment().GetConsignmentInformation(Convert.ToInt32(userInput));

                DataTable consignment = consignmentResults.Tables[0];           //we know that the consignment details are in the first table
                string shippingAddress = "";

                if (!string.IsNullOrEmpty(consignment.Rows[0]["Address1"].ToString()))
                {
                    //we know that there is only one row (because only one consignment) in the table - couldin future improve to return multiple consignments
                    shippingAddress += consignment.Rows[0]["Address1"].ToString();
                }

                if (!string.IsNullOrEmpty(consignment.Rows[0]["Address2"].ToString()))
                {
                    shippingAddress += Environment.NewLine + consignment.Rows[0]["Address2"].ToString();
                }

                if (!string.IsNullOrEmpty(consignment.Rows[0]["Address3"].ToString()))
                {
                    shippingAddress += Environment.NewLine + consignment.Rows[0]["Address3"].ToString();
                }

                if (!string.IsNullOrEmpty(consignment.Rows[0]["City"].ToString()))
                {
                    shippingAddress += Environment.NewLine + consignment.Rows[0]["City"].ToString();
                }

                if (!string.IsNullOrEmpty(consignment.Rows[0]["PostCode"].ToString()))
                {
                    shippingAddress += Environment.NewLine + consignment.Rows[0]["PostCode"].ToString();
                }

                if (!string.IsNullOrEmpty(consignment.Rows[0]["CountryISO2"].ToString()))
                {
                    shippingAddress += Environment.NewLine + consignment.Rows[0]["CountryISO2"].ToString();
                }

                Console.WriteLine();
                Console.WriteLine("Consignment - " + Convert.ToDateTime(consignment.Rows[0]["ConsignmentDate"]));
                Console.WriteLine("------------------------------------------");
                Console.Write(shippingAddress);

                //retrieve the packages table (which we know has an index of 1) and loop through the entries to display the results
                DataTable consignmentPackageResults = consignmentResults.Tables[1];
                for (int i = 0; i < consignmentPackageResults.Rows.Count; i++)
                {
                    DataRow currentPackage = consignmentPackageResults.Rows[i];

                    int packageNo = i + 1;

                    //print out the package details
                    Console.WriteLine();
                    Console.WriteLine("Package " + packageNo + " of " + consignmentPackageResults.Rows.Count + ":");
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Width: " + currentPackage["PackageWidth"].ToString());
                    Console.WriteLine("Height: " + currentPackage["PackageHeight"].ToString());
                    Console.WriteLine("Depth: " + currentPackage["PackageDepth"].ToString());
                    Console.WriteLine("Total Weight: " + currentPackage["TotalWeight"].ToString());
                    Console.WriteLine();
                    Console.WriteLine("Items:");

                    //now loop the items for this package (where packageID = currentPackage["packageId"])
                    DataTable packageItems = consignmentResults.Tables[2]; //we know that the items is at index 2
                    for (int j = 0; j < packageItems.Rows.Count; j++)
                    {
                        DataRow currentItem = packageItems.Rows[j];

                        if (Convert.ToInt32(currentItem["PackageId"]) == Convert.ToInt32(currentPackage["PackageId"]))
                        {
                            int itemNo = j + 1;

                            //print out the item details
                            Console.WriteLine(currentItem["Quantity"] + "x " + currentItem["ItemCode"].ToString() + " @ " + currentItem["UnitWeight"].ToString() + "Kgs");
                        }
                    }
                }

                Console.WriteLine();
                Console.Write("Search for another consignment (Y/N): ");
                userInput = readUserInput();

                if (userInput == "Y" || userInput == "y")
                {
                    SearchConsignment();
                }
                else
                {
                    DisplayMainMenu();
                }
            }
            else
            {
                DisplayMainMenu();
            }
        }

        //read in the characters entered by user and return as a string
        static public string readUserInput()
        {
            string userInput = null;
            StringBuilder userInputBuffer = new StringBuilder();

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            while (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Escape)
            {
                Console.Write(keyInfo.KeyChar);
                userInputBuffer.Append(keyInfo.KeyChar);
                keyInfo = Console.ReadKey(true);
            }

            if(keyInfo.Key == ConsoleKey.Enter)
            {
                userInput = userInputBuffer.ToString();
            }

            if(keyInfo.Key == ConsoleKey.Escape)
            {
                userInput = null;
            }

            return userInput;
        }
    }
}
