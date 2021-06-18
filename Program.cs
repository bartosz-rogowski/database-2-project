using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Rogowski_Hierarchy_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Company company = new Company();
            while (true)
            {
                int option;

                //loop for entering valid option
                while (true)
                {
                    Console.Clear();
                    Console.Write("HierarchyID Project / Databases 2 / Bartosz Rogowski\n\n" +
                        "\t\tMain Menu:\n\n" +
                        "1 - Display all employees\n\n" +
                        "2 - Add sample prepared data into table\n\n" +
                        "3 - Add new employee into table\n\n" +
                        "4 - Delete all employees from table\n\n" +
                        "5 - Delete an employee from table\n\n" + 
                        "6 - Find an employee\n\n" + 
                        "7 - Find an employee and their subordinates\n\n" +
                        "8 - Show statistics (min, max, avg salary)\n\n" +
                        "9 - Display employees working for <condition> years\n\n" +
                        "0 - Exit program\n\n" +
                        "\nChoose option: "
                    );
                    try
                    {
                        option = Convert.ToInt32(Console.ReadLine());
                        break;
                    }
                    catch(Exception)
                    {
                        
                    }
                }
               
                switch (option)
                {
                    case 0:
                        Console.Clear();
                        Environment.Exit(0);
                        break;

                    case 1:
                        Console.Clear();
                        company.DisplayAllEmployees();
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Adding sample data...");
                        try
                        {
                            company.AddSampleData();
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            Console.WriteLine("An error occured.");
                            Console.WriteLine("HINT: Probably data you provided are already in the database or you have given wrong type(s).");
                        }
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("Provide hierarchy (for example: \"/1/2/3/\"): ");
                        String path = Console.ReadLine();
                        Console.WriteLine("Provide name: ");
                        String name = Console.ReadLine();
                        Console.WriteLine("Provide position: ");
                        String position = Console.ReadLine();
                        Console.WriteLine("Provide year since employee has started work: ");
                        String start_year_s = Console.ReadLine();
                        Console.WriteLine("Provide salary: ");
                        String salary_s = Console.ReadLine();
                        Console.WriteLine("Adding employee...");
                        try
                        {
                            int start_year = Int32.Parse(start_year_s);
                            float salary = float.Parse(salary_s);
                            if (start_year < 1900 || salary < 0)
                                throw new System.FormatException();
                            company.AddNewEmployee(path, name, position, start_year, salary);
                        }
                        catch (System.ArgumentNullException)
                        {
                            Console.WriteLine("At least one of the provided arguments is null.");
                        }
                        catch (System.FormatException)
                        {
                            Console.WriteLine("At least one of the provided arguments has wrong type or value.");
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            Console.WriteLine("An error occured.");
                            Console.WriteLine("HINT: Probably data you provided are already in the database or you have given wrong type(s).");
                        }
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine("WARNING: This operation cannot be undone!");
                        Console.WriteLine("Are you sure you want to delete all data?");
                        Console.Write("Type \"y\" if yes: ");
                        if (Console.ReadLine() != "y")
                            break;
                        try
                        {
                            company.DeleteAllData();
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            Console.WriteLine("An error occured.");
                        }
                        Console.WriteLine("Table cleared.");
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 5:
                        Console.Clear();
                        Console.WriteLine("Provide hierarchy (for example: \"/1/2/3/\"): ");
                        path = Console.ReadLine();
                        try
                        {
                            company.DeleteEmployee(path);
                            Console.WriteLine("Employee has been removed from the table.");
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            Console.WriteLine("An error occured.");
                            Console.WriteLine("HINT: Probably data you provided are already removed from" +
                                " the database or you have given wrong value.");
                        }
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 6:
                        Console.Clear();
                        Console.WriteLine("Provide hierarchy (for example: \"/1/2/3/\"): ");
                        path = Console.ReadLine();
                        company.FindEmployee(path);
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 7:
                        Console.Clear();
                        Console.WriteLine("Provide hierarchy (for example: \"/1/2/3/\"): ");
                        path = Console.ReadLine();
                        company.FindEmployeeWithSubordinates(path);
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 8:
                        Console.Clear();
                        int level;
                        Console.WriteLine("Provide path (for example: \"/1/2/3/\") or level (for example: \"3\")");
                        String path_or_level = Console.ReadLine();

                        try
                        {
                            if (path_or_level.StartsWith("/"))
                                company.DisplayStatisticsForPath(path_or_level);
                            else
                            {
                                level = Int32.Parse(path_or_level);
                                company.DisplayStatisticsForLevel(level);
                            }
                        }
                        catch (System.ArgumentNullException)
                        {
                            Console.WriteLine("Provided argument is null.");
                        }
                        catch (System.FormatException)
                        {
                            Console.WriteLine("Provided argument has wrong type or value.");
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            Console.WriteLine("An error occured.");
                            //Console.WriteLine("HINT: Probably data you provided are already in the database or you have given wrong type(s).");
                        }
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;

                    case 9:
                        Console.Clear();
                        Console.WriteLine("Provide condition (for example: \"= 3\" or \"> 5\"):\n");
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("HINT: Type one of 3 possible signs:");
                        Console.WriteLine("\t> - longer than\n\t< - shorter than\n\t= - equally");
                        Console.WriteLine("and then number of years.");
                        Console.WriteLine("-----------------------------------\n");
                        String condition = Console.ReadLine();
                        try
                        {
                            if (condition.Length == 0)
                                throw new System.ArgumentNullException();
                            String[] arguments = condition.Split(' ');
                            char sign = arguments[0][0];
                            int number = int.Parse(arguments[1]);
                            company.DisplayEmployeesWorkingFor(sign, number);
                        }
                        catch (System.ArgumentNullException)
                        {
                            Console.WriteLine("Provided argument is null.");
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Console.WriteLine("Provided argument has wrong type or value.");
                        }
                        catch (System.FormatException)
                        {
                            Console.WriteLine("Provided argument has wrong type or value.");
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            Console.WriteLine("An error occured.");
                        }
                        Console.WriteLine("\nPress any key to go back");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
