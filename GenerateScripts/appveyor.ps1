
((Get-Content -path WPF\App.config -Raw) -replace 'Data Source=\(LocalDB\)\\MSSQLLocalDB;Database=Pert;Integrated Security=True','Server=(local)\SQL2016;Database=Pert;User ID=sa;Password=Password12!') | Set-Content -Path ..\test.txt
