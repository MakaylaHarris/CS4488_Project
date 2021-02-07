
# (Get-Content -path WPF\App.config -Raw) -replace \
# 'Data Source=(LocalDB)\MSSQLLocalDB;Database=Pert;Integrated Security=True',''
sqlcmd -S (local)\SQL2016 -U sa -P Password12! -i GenerateScripts\pertDB_create.sql
