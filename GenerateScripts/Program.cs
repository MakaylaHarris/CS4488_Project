using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GenerateScripts
{
    class Program
    {
        static void Main(string[] args)
        {
            string head = File.ReadAllText("create_pertDB_head.txt");
            File.WriteAllText(@"..\..\..\pertDB_create.sql", head + GenerateScripts());
        }

        static string GenerateScripts() 
        { 
            // Connect to the local, default instance of SQL Server. 
            Server srv = new Server(@"(localDB)\MSSQLLocalDB");

            // Reference the database.  
            Database db = srv.Databases["Pert"];

            Scripter scrp = new Scripter(srv);
            ScriptingOptions options = scrp.Options;
            options.ScriptDrops = false;
            options.WithDependencies = true;
            options.Indexes = true;   // To include indexes
            options.DriAllConstraints = true;   // to include referential constraints in the script
            options.Triggers = true;
            options.FullTextIndexes = true;
            options.NoCollation = true;
            options.Bindings = true;
            options.IncludeIfNotExists = false;
            options.ScriptBatchTerminator = true;
            options.ExtendedProperties = true;
            options.ChangeTracking = true;
            options.Default = true;
            scrp.PrefetchObjects = true; // some sources suggest this may speed things up

            var urns = new List<Urn>();

            // Iterate through the tables in database and script each one   
            foreach (Table tb in db.Tables)
            {
                // check if the table is not a system table
                if (tb.IsSystemObject == false)
                {
                    urns.Add(tb.Urn);
                }
            }

            // Iterate through the views in database and script each one. Display the script.   
            foreach (View view in db.Views)
            {
                // check if the view is not a system object
                if (view.IsSystemObject == false)
                {
                    urns.Add(view.Urn);
                }
            }

            // Iterate through the stored procedures in database and script each one. Display the script.   
            foreach (StoredProcedure sp in db.StoredProcedures)
            {
                // check if the procedure is not a system object
                if (sp.IsSystemObject == false)
                {
                    urns.Add(sp.Urn);
                }
            }

            StringBuilder builder = new StringBuilder();
            System.Collections.Specialized.StringCollection sc = scrp.Script(urns.ToArray());
            foreach (string st in sc)
            {
                // It seems each string is a sensible batch, and putting GO after it makes it work in tools like SSMS.
                // Wrapping each string in an 'exec' statement would work better if using SqlCommand to run the script.
                builder.AppendLine(st);
                builder.AppendLine("GO");
            }

            return builder.ToString();
        }
    }
}
