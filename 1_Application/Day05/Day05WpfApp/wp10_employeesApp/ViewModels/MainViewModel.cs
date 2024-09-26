using Caliburn.Micro;
using MahApps.Metro.Converters;
using System.Data.SqlClient;
using wp10_employeesApp.Models;

namespace wp10_employeesApp.ViewModels
{
    public class MainViewModel : Screen
    {
        private Employees employees;

        public BindableCollection<Employees> ListEmployees { get; set; }

        public int Idx
        {
            get => employees.Idx;
            set
            {
                employees.Idx = value;
                NotifyOfPropertyChange(nameof(Idx));
            }
        }

        public string FullName
        {
            get => employees.FullName;
            set
            {
                employees.FullName = value;
                NotifyOfPropertyChange(nameof(FullName));
            }
        }

        public int Salary
        {
            get => employees.Salary;
            set
            {
                employees.Salary = value;
                NotifyOfPropertyChange(nameof(Salary));
            }
        }
        public string DptName
        {
            get => employees.DptName;
            set
            {
                employees.DptName = value;
                NotifyOfPropertyChange(nameof(DptName));    
            }
        }

        public string Address
        {
            get => employees.Address;
            set
            {
                employees.Address = value;  
                NotifyOfPropertyChange(nameof(Address));
            }
        }


        public MainViewModel()
        {
            using(SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=pknu;User ID=sa;Password=12345"))
            {
                conn.Open();

                string selQuery = @"SELECT [Idx]
                                          ,[FullName]
                                          ,[Salary]
                                          ,[DeptName]
                                          ,[Address]
                                      FROM [dbo].[Employees]";
                SqlCommand selCommand = new SqlCommand(selQuery, conn);
                SqlDataReader reader = selCommand.ExecuteReader();
                ListEmployees = new BindableCollection<Employees>();

                while (reader.Read())
                {
                    var emp = new Employees
                    {
                        Idx = int.Parse(reader["Idx"].ToString()),
                        FullName = reader["FullName"].ToString(),
                        Salary = int.Parse(reader["Salary"].ToString()),
                        DptName = reader["DeptName"].ToString(),
                        Address = reader["Address"].ToString()
                    };
                    ListEmployees.Add(emp);
                }
            }
        }
    }
}
