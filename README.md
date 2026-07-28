# blazor-todo-list

# Members
Rakell Bandeira 
Ovinson Abel Lugo Rosado
Nyantakyi Francis
Herzan Carcache Huerta
Emmanuel Oluwatosin Ologe


# Folder Structure
/BLAZOR-TODO-LIST
    /To-Do-App
        /Components      → M3 and M4 mostly touch this
            /Layout        → MainLayout.razor, NavMenu.razor
            /Pages         → routable pages (@page directive)
            /Shared        → reusable non-page components (TaskList.razor, TaskForm.razor)
        /Data            → M1 mostly touches this
            AppDbContext.cs
            /Migrations    
        /Models          → M1 mostly touches this
            TaskItem.cs
        /Services        → M2 mostly touches this
            ITaskService.cs
            TaskService.cs
        /wwwroot         → M3 and M4 mostly touch this: CSS, static assets
        Program.cs       → M1,  M2 and M4 mostly touch this
  
    /To-Do-App.Tests               ← M5's test folder
        /UnitTests
        /Services
            TaskServiceTests.cs
        /Data
            AppDbContextTests.cs
        /ComponentTests             ← bUnit tests for Razor components
        TaskListTests.cs
        TaskFormTests.cs
        /IntegrationTests
        AuthFlowTests.cs 




  