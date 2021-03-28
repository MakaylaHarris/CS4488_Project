using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PertTest.DAL
{
    /// <summary>
    /// TestDB initializes a test database
    /// Created 3/27/2021 by Robert Nelson
    /// </summary>
    public class TestDB
    {
        public static readonly string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Database=Test_SmartPertDB;Integrated Security=True;Timeout=5";

        public TestDB(List<string> fixtures = null)
        {
            List<string> args = new List<string> { "clean" };
            if (fixtures != null)
            {
                args.AddRange(fixtures);
            }
            string startupPath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            int code = run_script(Path.Combine(startupPath, "Scripts", "ManageTestDB.py"), args);
            if (code != 0)
                throw new Exception("ManageTestDB.py exited with code " + code);
            DBReader.Instance.SaveSettings = false;
            DBReader.Instance.TestNewConnection(ConnectionString);
        }


        private static int run_script(string script, List<string> args=null)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = string.Format("{0} {1}", script, string.Join(" ", args));
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.FileName = @"python.exe";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
                process.WaitForExit(5000);
                return process.ExitCode;
            }
        }

    }
}
