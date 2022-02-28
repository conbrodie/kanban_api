using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

public class ValidateDepartmentsExistsAttribute : TypeFilterAttribute
{
  public ValidateDepartmentsExistsAttribute():base(typeof
    (ValidateDepartmentsExistsFilterImpl))
  {
  }
  private class ValidateDepartmentsExistsFilterImpl : IAsyncActionFilter
  {
    private readonly AppDbContext _context;
    public ValidateDepartmentsExistsFilterImpl(AppDbContext context)
    {
        _context = context;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("departmentId"))
      {
        var allIds = context.ActionArguments["departmentId"] as List<int>;
        var validIds = await _context.Departments.Where(dep => allIds.Contains(dep.DepartmentId)).Select(dep => dep.DepartmentId).ToListAsync();

        if (allIds.Count != validIds.Count)
        {
            allIds.RemoveAll(id => validIds.Contains(id));
            AddModelErrors(context, allIds);
            return;
        } 
      }
      await next();
    }
    public void AddModelErrors(ActionExecutingContext context, List<int> ids)
    {
        foreach (var id in ids)
        {
            context.ModelState.AddModelError($"DepartmentId: {id}", "Department not found");
        }
        context.Result = new NotFoundError(context.ModelState);
    }
  }
}