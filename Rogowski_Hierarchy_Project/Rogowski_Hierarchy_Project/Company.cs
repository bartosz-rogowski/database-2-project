using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using System.Collections.Generic;

namespace Rogowski_Hierarchy_Project
{
    public class Company
    {
        //attribute containing connection string to database
        private String connection_string = null;

        //attribute containing list of all employees
        private List<Employee> employee_list = new List<Employee>();


        //constructor defining connection string
        public Company()
        {
            this.connection_string = @"DATA SOURCE=MSSQLServer; " +
                "INITIAL CATALOG=Project_Hierarchy; " +
                "INTEGRATED SECURITY=SSPI;";
        }

        //destructor clearing list
        ~Company()
        {
            this.employee_list.Clear();
        }

        public List<Employee> GetEmployeeList()
        {
            return employee_list;
        }

        //method to load employees data to list (class attribute)
        public void LoadAllEmployees()
        {
            this.employee_list.Clear();
            string cmd = "SELECT Path as [Hierarchy], Name, Position, " +
                "Start_Year, Salary FROM Employees ORDER BY Path";
            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    this.employee_list.Add(
                        new Employee(
                            (SqlHierarchyId)datareader["Hierarchy"],
                            (String)datareader["Name"],
                            (String)datareader["Position"],
                            (int)datareader["Start_Year"],
                            float.Parse(datareader["Salary"].ToString())
                        )
                    );
                }
                datareader.Close();
            } //end of connection
        }

        //helper method for displaying legend
        private void ShowLegend()
        {
            Console.WriteLine("Legend:\n\tL - Level\n\tWS - Working since [year]\n" +
                "\tWF - Working for [in years]\n\n");
            Console.WriteLine(
                String.Format("{0,-8} {1,-4} {2,-25} {3,-30} {4,8}{5,8}{6,10}",
                "Path",
                "L",
                "Name",
                "Position",
                "WS",
                "WF",
                "Salary $"
                )
            );
            Console.WriteLine(new string('-', 16 + 25 + 30 + 8 + 8 + 10));
        }

        //metod for displaying in console all employees data
        public void DisplayAllEmployees()
        {
            //load actual data
            this.LoadAllEmployees();

            this.ShowLegend();
            foreach (Employee e in this.employee_list)
            {
                e.Display();
            }
        }

        //method adding new employee to the database's table
        public void AddNewEmployee(String Path, String Name, String Position, int Start_Year, float Salary)
        {
            string cmd = "INSERT INTO Employees VALUES (@path, @name, @position, @start_year, @salary)";
            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                command.Parameters.AddWithValue("@path", Path);
                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@position", Position);
                command.Parameters.AddWithValue("@start_year", Start_Year);
                command.Parameters.AddWithValue("@salary", Salary);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            } //end of connection
        }

        //method for adding sample data to table
        public void AddSampleData()
        {
            try
            {
                this.AddNewEmployee("/", "Taylor Alison Swift", "CEO", 2007, 13000);
                this.AddNewEmployee("/1/", "Billie Eilish", "Financial Manager", 2015, 8000);
                this.AddNewEmployee("/2/", "Tyler Joseph", "HR Manager", 2010, 7000);
                this.AddNewEmployee("/3/", "Selena Gomez", "Administration Manager", 2009, 8000);
                this.AddNewEmployee("/4/", "Ebba Tove Elsa Nilsson", "Technical Manager", 2013, 7000);
                this.AddNewEmployee("/1/1/", "Finneas O'Connell", "Financial Senior Manager", 2015, 6000);
                this.AddNewEmployee("/2/1/", "Joshua Dun", "HR Assistnant Manager", 2011, 5000);
                this.AddNewEmployee("/2/1/1/", "Dan Smith", "HR Assistnant", 2012, 3000);
                this.AddNewEmployee("/2/1/2/", "Josh Taylor", "HR Assistnant", 2018, 2200);
                this.AddNewEmployee("/4/1/", "Alma-Sofia Miettinen", "Technical Senior Manager", 2016, 6000);
                this.AddNewEmployee("/4/1/1/", "Zara Maria Larsson", "Technical Assistant Manager", 2016, 4000);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                Console.WriteLine("An error occured.\nSample data are (at least partially) already in the database.");
            }
        }

        //method for clearing all data from the database's table
        public void DeleteAllData()
        {
            string cmd = "DELETE FROM Employees";
            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                command.ExecuteNonQuery();
            } //end of connection
        }

        //method for deleting an employee's data from the database's table
        public void DeleteEmployee(String Path)
        {
            string cmd = "SELECT COUNT(*) FROM ( " +
                "SELECT * FROM Employees WHERE Path.IsDescendantOf(@path) = 1 " +
                ") as t";
            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                try
                {
                    SqlCommand command = new SqlCommand(cmd, conn);
                    command.Parameters.AddWithValue("@path", Path);
                    SqlDataReader datareader = command.ExecuteReader();
                    datareader.Read();
                    int count = (int)datareader[0];
                    datareader.Close();
                    if (count < 1)
                        throw new Exception("Such employee does not exist");
                    else if (count > 1)
                        throw new Exception("This employee has at least one subordinate!");
                    else if (count == 1)
                    {
                        cmd = "DELETE FROM Employees WHERE Path = @path";
                        command = new SqlCommand(cmd, conn);
                        command.Parameters.AddWithValue("@path", Path);
                        command.ExecuteNonQuery();
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
            } //end of connection
        }

        //method for finding and displaying data of employee by the given path
        public void FindEmployee(String Path)
        {
            this.employee_list.Clear();
            string cmd = "SELECT Path as [Hierarchy], Name, Position, " +
                "Start_Year, Salary FROM Employees WHERE Path = @path";
            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                command.Parameters.AddWithValue("@path", Path);
                try
                {
                    SqlDataReader datareader = command.ExecuteReader();
                    while (datareader.Read())
                    {
                        this.employee_list.Add(
                            new Employee(
                                (SqlHierarchyId)datareader["Hierarchy"],
                                (String)datareader["Name"],
                                (String)datareader["Position"],
                                (int)datareader["Start_Year"],
                                float.Parse(datareader["Salary"].ToString())
                            )
                        );
                    }
                    datareader.Close();
                    this.ShowLegend();
                    foreach (Employee e in this.employee_list)
                    {
                        e.Display();
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("An error occured.");
                    Console.WriteLine("HINT: Probably there is no such employee in the database or you have given wrong argument.");
                }
            } //end of connection
        }

        public void FindEmployeeWithSubordinates(String Path)
        {
            this.employee_list.Clear();
            string cmd = "SELECT Path as [Hierarchy], Name, Position, " +
                "Start_Year, Salary FROM Employees WHERE Path.IsDescendantOf(@path) = 1";
            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                command.Parameters.AddWithValue("@path", Path);
                try
                {
                    SqlDataReader datareader = command.ExecuteReader();
                    while (datareader.Read())
                    {
                        this.employee_list.Add(
                            new Employee(
                                (SqlHierarchyId)datareader["Hierarchy"],
                                (String)datareader["Name"],
                                (String)datareader["Position"],
                                (int)datareader["Start_Year"],
                                float.Parse(datareader["Salary"].ToString())
                            )
                        );
                    }
                    datareader.Close();
                    this.ShowLegend();
                    foreach (Employee e in this.employee_list)
                    {
                        e.Display();
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("An error occured.");
                    Console.WriteLine("HINT: Probably there is no such employee in the database or you have given wrong argument.");
                }
            } //end of connection
        }

        //method displaying simple statictics like min, max and avg salary
        //Path is string containing path (/1/2/)
        //method will display info for every descendant (with self)
        public void DisplayStatisticsForPath(String Path)
        {
            string cmd = @"SELECT MIN(Salary) as min_salary, MAX(Salary) as max_salary, 
	                        CAST(AVG(Salary) as NUMERIC(7, 2)) as avg_salary FROM (
		                        SELECT Path, Salary FROM Employees 
		                        WHERE Path.IsDescendantOf(@path) = 1
	                        ) as t";

            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                command.Parameters.AddWithValue("@path", Path);
                try
                {
                    SqlDataReader datareader = command.ExecuteReader();
                    while (datareader.Read())
                    {
                        Console.WriteLine(
                            "Minimum salary: " +
                            float.Parse(datareader["min_salary"].ToString()) +
                            "\nMaximum salary: " +
                            float.Parse(datareader["max_salary"].ToString()) +
                            "\nAverage salary: " +
                            float.Parse(datareader["avg_salary"].ToString())
                        );
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("An error occured.");
                }
            } //end of connection
        }

        //method displaying simple statictics like min, max and avg salary
        //Level is integer containing level
        //method will display info for every employee at given level
        public void DisplayStatisticsForLevel(int Level)
        {
            string cmd = @"SELECT MIN(Salary) as min_salary, MAX(Salary) as max_salary, 
	                        CAST(AVG(Salary) as NUMERIC(7, 2)) as avg_salary FROM (
		                        SELECT Path, Salary FROM Employees 
		                        WHERE Path.GetLevel() = @level
	                        ) as t";

            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                command.Parameters.AddWithValue("@level", Level);
                try
                {
                    SqlDataReader datareader = command.ExecuteReader();
                    while (datareader.Read())
                    {
                        Console.WriteLine(
                            "Minimum salary: " +
                            float.Parse(datareader["min_salary"].ToString()) +
                            "\nMaximum salary: " +
                            float.Parse(datareader["max_salary"].ToString()) +
                            "\nAverage salary: " +
                            float.Parse(datareader["avg_salary"].ToString())
                        );
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("An error occured.");
                }
            } //end of connection
        }

        //method displaying employees who have been working for @sign @number years
        //sign is char defining condition: 
        //  > - longer than
        //  < - shorter than
        //  = - equally
        public void DisplayEmployeesWorkingFor(char sign, int number)
        {
            String text = "Displaying employees working for #1 " + number.ToString() + " years\n";
            this.employee_list.Clear();
            string cmd = @"SELECT Path as [Hierarchy], Name, Position, 
	                        Start_Year, Salary FROM Employees WHERE 
                            DATEDIFF(year, CONVERT(datetime, CONVERT(varchar, Start_Year)), GETDATE()) ";

            if (sign == '>')
            {
                cmd += "> @number";
                text = text.Replace("#1", "more than");
            }
            else if (sign == '<')
            {
                cmd += "< @number";
                text = text.Replace("#1", "less than");
            }
            else if (sign == '=')
            {
                cmd += "= @number";
                text = text.Replace("#1", "exactly");
            }
            else throw new System.FormatException();

            using (SqlConnection conn = new SqlConnection(this.connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(cmd, conn);
                command.Parameters.AddWithValue("@number", number);
                try
                {
                    Console.WriteLine(text);
                    SqlDataReader datareader = command.ExecuteReader();
                    while (datareader.Read())
                    {
                        this.employee_list.Add(
                            new Employee(
                                (SqlHierarchyId)datareader["Hierarchy"],
                                (String)datareader["Name"],
                                (String)datareader["Position"],
                                (int)datareader["Start_Year"],
                                float.Parse(datareader["Salary"].ToString())
                            )
                        );
                    }
                    datareader.Close();
                    this.ShowLegend();
                    foreach (Employee e in this.employee_list)
                    {
                        e.Display();
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("An error occured.");
                }
            } //end of connection
        }
    }
}
