using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Users;

public partial class Users: ComponentBase
{
    private List<ApplicationUser> model = new List<ApplicationUser>();
    private List<ApplicationUser> seleUsers = new List<ApplicationUser>();

    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] private UserController _UserController { get; set; }
    [Inject ]private IExportToCsv ExportToCsv { get; set; }
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(@localizer["users"], href: null, disabled: true,icon: Icons.Material.Filled.Group)
        };
        await Update();
        if (navigationManager.Uri.Contains("/users/create"))
        {
             await OpenDialogCreate(maxWidth);
        }
    }
    
    protected async Task Update()
    {
        
        model.RemoveAll(item => true);
        model = _UserController.Get().ToList();

    }


    
    
    private Func<ApplicationUser, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    
    private async Task OpenDialogCreate(DialogOptions options)
    {
        var dialog = Dialog.Show<CreateUserDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    
    async Task RowClicked(DataGridRowClickEventArgs<ApplicationUser> args)
    {
        var parameters = new DialogParameters { ["user"]=args.Item };

        var dialog =  Dialog.Show<UserDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                /*var records = model.Select(e => new IFR.Export.ProjectExport()
                {
                    Name = e.Name,
                    Description = e.Description,
                    Client = e.Client.Name,
                    CreatedUser = e.User.FullName,
                    StartDate = e.StartDate.ToShortDateString(),
                    EndDate = e.EndDate.ToShortDateString(),
                    Template = e.Template,
                    Status = e.Status.ToString(),
                    ProjectType = e.ProjectType.ToString(),
                    Language = e.Language.ToString(),
                    Score = e.Score.ToString(),
                    FindingsId = e.FindingsId.ToString(),
                    ExecutiveSummary = e.ExecutiveSummary
                    
 
                }).ToList();
                var file = ExportToCsv.ExportProjects(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);*/
                break;
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["users"]=seleUsers };

                var dialog =  Dialog.Show<DeleteUsersBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<ApplicationUser> items)
    {
        
        seleUsers = items.ToList();
    }
    
}