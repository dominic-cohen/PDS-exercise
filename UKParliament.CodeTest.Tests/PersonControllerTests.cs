using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Web.Validators;
using UKParliament.CodeTest.Web.ViewModels;
using UKParliament.CodeTest.Web.Controllers;
using Xunit;
using UKParliament.CodeTest.Data.Repositories;
using System.Runtime.CompilerServices;

namespace UKParliament.CodeTest.Tests;
public class PersonControllerTests
{
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<IPersonService> _personServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<PersonViewModel>> _validatorMock;
    private readonly PersonController _controller;

    public PersonControllerTests()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        _personServiceMock = new Mock<IPersonService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<PersonViewModel>>();

        _controller = new PersonController(
            _personServiceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object
        );
    }


    [Fact]
    public async Task GetAllPeople_ShouldReturnEmptyList_WhenNoPeopleExist()
    {
        _personServiceMock.Setup(s => s.GetAllPeopleAsync()).ReturnsAsync(new List<Person>());

        _mapperMock.Setup(m => m.Map<List<PersonViewModel>>(It.IsAny<List<Person>>()))
                   .Returns(new List<PersonViewModel>());

        var result = await _controller.GetAllPeople();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModel = Assert.IsType<List<PersonViewModel>>(okResult.Value);

        Assert.Empty(viewModel);
    }


    [Fact]
    public async Task GetAllPeople_ShouldReturnAllPeople()
    {

        var people = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe", DOB = DateOnly.Parse("10/10/2010"), DepartmentId = 1 },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith", DOB = DateOnly.Parse("11/10/2010"), DepartmentId = 1 }
        };

        _personServiceMock.Setup(s => s.GetAllPeopleAsync()).ReturnsAsync(people);

        var peopleViewModel = people.Select(p => new PersonViewModel {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            DepartmentId = p.DepartmentId
        }).ToList();
        _mapperMock.Setup(m => m.Map<List<PersonViewModel>>(people)).Returns(peopleViewModel);

        var result = await _controller.GetAllPeople();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModel = Assert.IsType<List<PersonViewModel>>(okResult.Value);

        Assert.Equal(2, viewModel.Count);
    }



    [Fact]
    public async Task GetPersonById_ShouldReturnCorrectPerson()
    {
        var key = 1;
        var person = new Person { Id = key, FirstName = "John", LastName = "Doe", DOB = DateOnly.Parse("27/02/2013"), DepartmentId = 1 };

        _personServiceMock.Setup(s => s.GetPersonByIdAsync(1)).ReturnsAsync(person);

        var personViewModel = new PersonViewModel {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DOB = person.DOB.ToString(),
            DepartmentId = person.DepartmentId
        };
        _mapperMock.Setup(m => m.Map<PersonViewModel>(person)).Returns(personViewModel);

        var result = await _controller.GetPersonById(key);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var detailViewModel = Assert.IsType<PersonViewModel>(okResult.Value);

        Assert.Equal(person.Id, detailViewModel.Id);
    }

    [Fact]
    public async Task AddPerson_ShouldReturnInvalidResult_WhenDateIsInvalid()
    {
        var key = 1;
        var personViewModel = new PersonViewModel
        {
            Id = key,
            FirstName = "Fred",
            LastName = "Bloggs",
            DOB = "00203/2006", // Invalid date
            DepartmentId = 1
        };
        

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("DOB", "Date of birth must be a valid date.")
        };
       
        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);
        _validatorMock.Setup(v => v.Validate(personViewModel)).Returns(validationResult);

        // Act
        var result = await _controller.AddPerson(personViewModel);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);

        // Assert
        Assert.Equal("Invalid person data.", errorResponse.Message);

        var dobError = errorResponse.Errors.FirstOrDefault(e => e.PropertyName == "DOB");
        Assert.NotNull(dobError);
        Assert.Equal("Date of birth must be a valid date.", dobError.ErrorMessage);
    }


    [Fact]
    public async Task AddPerson_ShouldReturnInvalidResult_MissingLastName()
    {
        var key = 1;
        var personViewModel = new PersonViewModel
        {
            Id = key,
            FirstName = "Fred",
            LastName = string.Empty, // Cause validation error
            DOB = "2006-03-21",
            DepartmentId = 1
        };
       

        // Simulate validation failure
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("LastName", "Last name is required.")
        };

        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);
        _validatorMock.Setup(v => v.Validate(personViewModel)).Returns(validationResult);

        // Act
        var result = await _controller.AddPerson(personViewModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);

        Assert.Equal("Invalid person data.", errorResponse.Message);

        var lastNameError = errorResponse.Errors.FirstOrDefault(e => e.PropertyName == "LastName");
        Assert.NotNull(lastNameError);
        Assert.Equal("Last name is required.", lastNameError.ErrorMessage);
    }


    [Fact]
    public async Task AddPerson_Valid_ReturnNewPersonId()
    {
        var key = 1;
        var personViewModel = new PersonViewModel { Id = key, FirstName = "Fred", LastName = "Bloggs", DOB = "21/03/2006", DepartmentId = 1 };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validatorMock.Setup(v => v.Validate(personViewModel)).Returns(validationResult);

        var person = new Person { Id = key, FirstName = "Fred", LastName = "Bloggs", DOB = new DateOnly(2006, 3, 21), DepartmentId = 1 };
        _mapperMock.Setup(m => m.Map<Person>(personViewModel)).Returns(person);

        _personServiceMock.Setup(s => s.AddPersonAsync(person)).ReturnsAsync(key);
        
        // Act
        var result = await _controller.AddPerson(personViewModel);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(key, ((PersonViewModel) createdResult.Value).Id);
    }


    [Fact]
    public async Task UpdatePerson_ShouldUpdateFirstNameAndReturnNoContent()
    {
        // Arrange
        var personId = 1;

        // Updated details (to simulate updating the person's LastName)
        var updatedPersonViewModel = new PersonViewModel
        {
            Id = personId,
            FirstName = "Fred",
            LastName = "Smith",
            DOB = "1980-01-01",
            DepartmentId = 1
        };
      

        // Simulate validation success
        var validationResult = new FluentValidation.Results.ValidationResult();
        _validatorMock.Setup(v => v.Validate(updatedPersonViewModel)).Returns(validationResult);

        // Simulate mapping from View Model
        var updatedPerson = new Person
        {
            Id = personId,
            FirstName = "Fred",
            LastName = "Smith", 
            DOB = DateOnly.Parse("1980-01-01"),
            DepartmentId = 1
        };

        _mapperMock.Setup(m => m.Map<Person>(updatedPersonViewModel)).Returns(updatedPerson);
        _personServiceMock.Setup(s => s.UpdatePersonAsync(updatedPerson)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePerson(personId, updatedPersonViewModel);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _personServiceMock.Verify(s => s.UpdatePersonAsync(It.Is<Person>(p =>
            p.Id == personId &&
            p.FirstName == "Fred" &&
            p.LastName == "Smith"
        )), Times.Once);
    }


    [Fact]
    public async Task DeletePerson_ShouldReturnNoContentForValidDelete()
    {
        _personServiceMock.Setup(s => s.DeletePersonAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePerson(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}




