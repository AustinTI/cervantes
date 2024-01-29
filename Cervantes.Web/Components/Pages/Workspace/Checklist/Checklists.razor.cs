using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Components.Pages.Workspace.Target;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class Checklists: ComponentBase
{
       [Inject] ISnackbar Snackbar { get; set; }
       [Inject] private ChecklistController _checklistController { get; set; }
       [Inject] private ProjectController _ProjectController { get; set; }

       [Parameter] public Guid project { get; set; }
    
        private List<ChecklistViewModel> model = new List<ChecklistViewModel>();
        private List<ChecklistViewModel> seleChecklists = new List<ChecklistViewModel>();

        private CORE.Entities.Project Project = new CORE.Entities.Project();
        private List<BreadcrumbItem> _items;
    private string searchString = "";

    protected override async Task OnInitializedAsync()
    {
        var user = await _ProjectController.VerifyUser(project);
        if (user == false)
        {
            Snackbar.Add(@localizer["noRolePermission"], Severity.Warning);
            navigationManager.NavigateTo("workspaces");
        }
        await Update();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Workspace", href: "/workspaces",icon: Icons.Material.Filled.Workspaces),
            new BreadcrumbItem(Project.Name, href: "/workspace/"+project,icon: Icons.Material.Filled.Folder),
            new BreadcrumbItem(@localizer["checklist"], href: null, disabled: true, icon: Icons.Material.Filled.Checklist)
        };
        await base.OnInitializedAsync();
    }

    private async Task Update()
    {
        model.RemoveAll(item => true);
        var wstg = _checklistController.GetWSTG(project);
        var mstg = _checklistController.GetMSTG(project);
        Project = new CORE.Entities.Project();
        Project = _ProjectController.GetById(project);
        foreach (var item in wstg)
        {
            var itemWstg  = new ChecklistViewModel
            {
                Id = item.Id,
                Target = item.Target.Name,
                CreatedDate = item.CreatedDate,
                User = item.User.FullName,
                Type = ChecklistType.OWASPWSTG,
            };
            
            model.Add(itemWstg);
        }
        
        foreach (var item in mstg)
        {
            var itemMstg = new ChecklistViewModel()
            {
                Id = item.Id,
                Target = item.Target.Name,
                CreatedDate = item.CreatedDate,
                User = item.User.FullName,
                Type = ChecklistType.OWASPMASVS,
                Platform = item.MobilePlatform,
            };
            
            model.Add(itemMstg);
        }
    }

    
    
    private Func<ChecklistViewModel, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Target.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.CreatedDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Type.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    private async Task OpenImportDialog(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = DialogService.Show<ImportDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private async Task OpenDialogCreate(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = DialogService.Show<CreateChecklistDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
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
    
    async Task RowClicked(DataGridRowClickEventArgs<ChecklistViewModel> args)
    {
        if (args.Item.Type == ChecklistType.OWASPWSTG)
        {
            var parameters = new DialogParameters {["project"]=project, ["checklist"]=args.Item };

            var dialog =  DialogService.Show<WstgDialog>("Edit", parameters, maxWidth);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await Update();
                StateHasChanged();
            }
        }
        else
        {
            var parameters = new DialogParameters { ["project"]=project,["checklist"]=args.Item };

            var dialog =  DialogService.Show<MastgDialog>("Edit", parameters, maxWidth);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await Update();
                StateHasChanged();
            }
        }
        
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["checklists"]=seleChecklists };

                var dialog =  DialogService.Show<DeleteChecklistBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<ChecklistViewModel> items)
    {
        
        seleChecklists = items.ToList();
    }

}