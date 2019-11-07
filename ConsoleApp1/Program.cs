using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();
        }

        /// <summary>
        /// Menu Items
        /// </summary>
        static void ShowMenuInConsole()
        {
            Console.WriteLine("\n Please choose one of the options:");
            Console.WriteLine("\t [1] Show Departments");
            Console.WriteLine("\t [2] Show Employee");
            Console.WriteLine("\t [3] Show employeer with .. ");
            Console.WriteLine("\t [4] list employeers sorted by nam, salary");
            Console.WriteLine("\t [5] insert");
            Console.WriteLine("\t [6] update");
            Console.WriteLine("\t [7] update[change dep]");
            Console.WriteLine("\t [8] delete");

            Console.WriteLine("\t [9] Exit the program");
        }

        private static void Menu()
        {

            ShowMenuInConsole();

            bool MQuit = false;

            int ChoiceNomMenu = 0;

            while (!MQuit)
            {

                if (!Int32.TryParse(Console.ReadLine(), out ChoiceNomMenu) || !(ChoiceNomMenu >= 1 && ChoiceNomMenu <= 9))
                {
                    Console.WriteLine("\t Invalid input. Try again:");

                    ShowMenuInConsole();
                    continue;
                }

                switch (ChoiceNomMenu)
                {
                    case 1:

                        ShowDep();

                        ShowMenuInConsole();

                        break;
                    case 2:

                        ShowEmp();

                        ShowMenuInConsole();
                        break;
                    case 3:
                        Console.Write("Input Name:");
                        string name = Console.ReadLine();

                        ShowFindItem(name);

                        ShowMenuInConsole();
                        break;
                    case 4:

                        ShowEmpSorted();

                        ShowMenuInConsole();
                        break;
                    case 5:
                        Insert();
                        ShowMenuInConsole();
                        break;
                    case 6:

                        ShowEmp();

                        Update();
                        ShowMenuInConsole();
                        break;
                    case 7:

                        ShowEmp();

                        UpdateWithParam();
                        ShowMenuInConsole();
                        break;
                    case 8:

                        ShowEmp();

                        Delete();
                        ShowMenuInConsole();
                        break;
                    case 9:
                        Console.WriteLine("\t Quitting...");
                        MQuit = true;
                        Thread.Sleep(1000);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void UpdateWithParam()
        {
            bool pr = false;
            int index, newDep;
            do
            {
                Console.Write("Input index employeer for update department:");
                pr = int.TryParse(Console.ReadLine(), out index);
            } while (!pr);

            ShowDep();
            pr = false;
            do
            {
                Console.Write("Input newDep for update:");
                pr = int.TryParse(Console.ReadLine(), out newDep);
            } while (!pr);

            string sqlExpression = $"update Employee set DepartmentId = {newDep} where id = {index}";

            using (SqlConnection connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine("\tОтредактирвоано объектов: {0} into Employee", number);
            }
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = @"Data Source=DARK\SQLEXPRESS;Initial Catalog=MYBb;Integrated Security=True";

            SqlConnection conn = new SqlConnection(connectionString.ToString());

            conn.Open();

            return conn;
        }

        private static void ShowEmpSorted()
        {
            using (SqlConnection connection = GetConnection())
            {
                SqlCommand command;
                string sqlExpression = "select id, name, salary, DepartmentId from Employee order by name, salary";
                command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"\t id: {reader.GetInt32(0)}, name:{reader.GetString(1)}, salary:{String.Format("{0:0.00}", reader.GetDecimal(2))}, DepartmentId: {reader.GetInt32(3)}");
                }
            }
        }

        private static void ShowFindItem(string name)
        {
            using (SqlConnection connection = GetConnection())
            {
                SqlCommand command;
                string sqlExpression = $"select id, name, salary from Employee where name like @param";
                command = new SqlCommand(sqlExpression, connection);
                //add one IN parameter                     
                SqlParameter nameParam = new SqlParameter("@param", value: name+"%");
                // добавляем параметр к команде
                command.Parameters.Add(nameParam);

                SqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows) Console.WriteLine($"Not found {name}");

                while (reader.Read())
                {
                    Console.WriteLine($"\tid: {reader.GetInt32(0)}, name:{reader.GetString(1)}, salary:{String.Format("{0:0.00}", reader.GetDecimal(2))}");
                }
            }
        }

        private static void ShowDep()
        {

            using (SqlConnection connection = GetConnection())
            {
                SqlCommand command;
                string sqlExpression = "select id, name from Department";
                command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("\t{0}\t{1}", reader.GetName(0), reader.GetName(1));
                while (reader.Read())
                {
                    Console.WriteLine($"\tid: {reader.GetInt32(0)},\tname:{reader.GetString(1)}");
                }

                reader.Close();

                sqlExpression = "SELECT COUNT(*) FROM Department";
                command = new SqlCommand(sqlExpression, connection);
                object count = command.ExecuteScalar();
                Console.WriteLine("\tВ таблице Department {0} объектов", count);
            }
        }

        private static void ShowEmp()
        {
            using (SqlConnection connection = GetConnection())
            {
                string sqlExpression = "select id, name, salary, DepartmentId from Employee";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("\t{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3));
                while (reader.Read())
                {
                    Console.WriteLine($"\tid: {reader.GetInt32(0)},\tname:{reader.GetString(1)},\tsalary:{String.Format("{0:0.00}", reader.GetDecimal(2))},\tDepartmentId: {reader.GetInt32(3)}");
                }

                reader.Close();

                sqlExpression = "SELECT COUNT(*) FROM Employee";
                command = new SqlCommand(sqlExpression, connection);
                object count = command.ExecuteScalar();
                Console.WriteLine("\tВ таблице Employee {0} объектов", count);
            }
        }

        private static void Insert()
        {

            string sqlExpression = "INSERT INTO Department (Name) VALUES ('ReserchSpace')";

            using (SqlConnection connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine("\tДобавлено объектов: {0} into Department", number);

                sqlExpression = "INSERT INTO Employee(ChiefId, Name, Salary, DepartmentId) VALUES(95, 'Mikkky', 100000, 1)";
                command = new SqlCommand(sqlExpression, connection);
                number = command.ExecuteNonQuery();
                Console.WriteLine("\tДобавлено объектов: {0} into Employee", number);
            }
        }

        private static void Update()
        {
            bool pr = false;
            int index, salary;
            do
            {
                Console.Write("Input index for update salary:");
                pr = int.TryParse(Console.ReadLine(), out index);
            } while (!pr);

            pr = false;
            do
            {
                Console.Write("Input salary for update:");
                pr = int.TryParse(Console.ReadLine(), out salary);
            } while (!pr);

            string sqlExpression = $"update Employee set salary = {salary} where id = {index}";

            using (SqlConnection connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine("\tОтредактирвоано объектов: {0} into Employee", number);
            }
        }

        private static void Delete()
        {
            bool pr = false;
            int index;
            do
            {
                Console.Write("Input index for del:");
                pr = int.TryParse(Console.ReadLine(), out index);
            } while (!pr);

            string sqlExpression = $"Delete from Employee where id = {index}";

            using (SqlConnection connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine("\tУдалено объектов: {0} from Employee", number);
            }
        }
    }
}
