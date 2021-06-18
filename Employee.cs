using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;

namespace Rogowski_Hierarchy_Project
{
    public class Employee
    {
        //attributes
        public SqlHierarchyId Path;
        public String Name;
        public String Position;
        public int Start_Year;
        public float Salary;

        //class constructor
        public Employee(SqlHierarchyId Path, String Name, String Position, int Start_Year, float Salary)
        {
            this.Path = Path;
            this.Name = Name;
            this.Position = Position;
            this.Start_Year = Start_Year;
            this.Salary = Salary;
        }

        //method displaying an employee in console 
        public void Display()
        {
            Console.WriteLine(
                String.Format("{0,-8} {1,-4} {2,-25} {3,-30} {4,8}{5,8}{6,10}", 
                    this.Path.ToString(),
                    this.Path.GetLevel(),
                    this.Name,
                    this.Position, 
                    this.Start_Year,
                    (DateTime.Now.Year - this.Start_Year),
                    this.Salary
                )
            );
            /*Console.WriteLine(
                this.Path.ToString() + '\t' + this.Path.GetLevel() + '\t' +
                this.Name + "\t\t" + this.Position + "\t\t" + this.Start_Year + '\t' +
                (DateTime.Now.Year - this.Start_Year) + '\t' + this.Salary
            ); */
        }
    }
}
