using Microsoft.AspNetCore.Mvc.Rendering;

public class AssignRoleViewModel
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public List<SelectListItem> Roles { get; set; } 

    public string SelectedRole { get; set; } 
}
