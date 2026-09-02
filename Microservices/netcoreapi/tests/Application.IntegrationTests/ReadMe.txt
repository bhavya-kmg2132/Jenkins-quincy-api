Run all tests in the project in the current directory from developer powershell:
dotnet test

Run individual test in the project in the current directory from developer powershell:
dotnet test --filter "FullyQualifiedName=IntegrationTest.ocp1_test_1"

Run the tests in the project in the current directory, and generate a test results file in the trx format:
dotnet test --logger trx

Run the tests in the project in the current directory, and generate a code coverage file (Windows only):
dotnet test --collect "Code Coverage"

Run the tests in the project in the current directory, and log with detailed verbosity to the console:
dotnet test --logger "console;verbosity=detailed"


Git Workflow:
https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow   