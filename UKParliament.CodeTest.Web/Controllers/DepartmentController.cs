
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Web.Validators;
using UKParliament.CodeTest.Web.ViewModels;

namespace UKParliament.CodeTest.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IMapper _mapper;

    public DepartmentController(IPersonService personService, IMapper mapper)
    {
        _personService = personService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllDepartments()
    {
        try
        {
            var departments = await _personService.GetAllDepartmentsAsync();
            var departmentsViewModel = _mapper.Map<List<DepartmentViewModel>>(departments);

            return Ok(departmentsViewModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the departments.", details = ex.Message });
        }
    }
}
