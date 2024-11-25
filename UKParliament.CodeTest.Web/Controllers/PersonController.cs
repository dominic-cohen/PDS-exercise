
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
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IMapper _mapper;
    private readonly IValidator<PersonViewModel> _validator;

    public PersonController(IPersonService personService, IMapper mapper, IValidator<PersonViewModel> validator)
    {
        _personService = personService;
        _mapper = mapper;
        _validator = validator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllPeople()
    {
        try
        {
            var people = await _personService.GetAllPeopleAsync();
            var peopleViewModel = _mapper.Map<List<PersonViewModel>>(people);

            return Ok(peopleViewModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the people.", details = ex.Message });
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetPersonById(int id)
    {
        try
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound(new { message = $"Person with ID {id} was not found." });
            }

            var personViewModel = _mapper.Map<PersonViewModel>(person);
            //var viewModel = new PersonDetailViewModel
            //{
            //    Person = c,
            //    IsEditMode = false
            //};
            return Ok(personViewModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the person.", details = ex.Message });
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddPerson(PersonViewModel personViewModel)
    {
        var validationResult = _validator.Validate(personViewModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Invalid person data.",
                Errors = validationResult.Errors.Select(e => new ErrorDetail
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage
                })
            });
        }

        try
        {
            var person = _mapper.Map<Person>(personViewModel);
            var newPersonId = await _personService.AddPersonAsync(person);
            return CreatedAtAction(nameof(GetPersonById), new { id = newPersonId }, personViewModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while adding the person.", details = ex.Message });
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> UpdatePerson(int id, PersonViewModel personViewModel)
    {
        if (id != personViewModel.Id)
        {
            return BadRequest(new { message = "Person ID in the URL does not match the body." });
        }


        var validationResult = _validator.Validate(personViewModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Invalid person data.",
                errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        try
        {
            var person = _mapper.Map<Person>(personViewModel);
            await _personService.UpdatePersonAsync(person);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the person.", details = ex.Message });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        try
        {
            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the person.", details = ex.Message });
        }
    }
}
