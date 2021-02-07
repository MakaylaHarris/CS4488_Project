
((Get-Content -path WPF\App.config -Raw) -replace \
'Data Source=(LocalDB)\MSSQLLocalDB;Database=Pert;Integrated Security=True','Server=(local)\SQL2016;Database=Pert;User ID=sa;Password=Password12!') \
| Set-Content -Path WPF\App.config
sqlcmd -S `(local`)\SQL2016 -U sa -P Password12! -i GenerateScripts\pertDB_create.sql
