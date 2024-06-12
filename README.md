# VSG_Binance_WS_Task

Steps to run the application:
1. Clone the repo
2. Open the solution file
3. Build the Commons project in Release mode(it is used as a library for the other projects)
4. Build DataCollector project in Release mode
5. Navigate to the DataCollector output directory and adjust the ConnectionString under DefaultConnection 
needed to access the database (MySQL) in the appsettings.json file.
5. Double click on the created DataCollector.exe or run cmd (Command Prompt) as an administrator
and execute the following command to create a Windows Service which will work in the background
`sc create <ServiceName> binPath="path to the DataCollector executable"`
6. Navigate to the Services (Win + Services) or (Win + R + services.msc)
7. Start the created service to store the symbol prices in the database
8. Build RequestHandler project in Release mode
9. Navigate to the RequestHandler output directory and adjust the ConnectionString under DefaultConnection 
needed to access the database (MySQL) in the appsettings.json file.
10. Double click on the created RequestHandler.exe or run cmd (Command Prompt) as an administrator
and execute the following command to create a Windows Service which will work in the background
`sc create <ServiceName> binPath="path to the RequestHandler executable"`
6. Navigate to the Services (Win + Services) or (Win + R + services.msc)
7. Start the API service (it will be hosted at http://localhost:5000)
8. Build SimpleConsoleApp under Release mode
9. Open the output directory and double click on the executable file
10. Follow the prompted instructions 


Initial default MySQL setup will be needed.