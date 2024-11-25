using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using ApiExercise.DTOs;
using Microsoft.AspNetCore.Authorization;
using DAL.Models;

namespace ApiExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class  ServicesController(
    ServicesRepository repository, 
    CategoryRepository categoryRepository) : ControllerBase
{
    [HttpGet]
    public ActionResult GetServices()
    {
        return Ok(repository.GetAll().Select(s => s.ToDto()));
    }

    [HttpGet("{id}")]
    public ActionResult GetServicesById(int id)
    {

        var service = repository.GetById(id);

        if (service is null)
        {
            return NotFound();
        }
        return Ok(service.ToDto());
    }
    //CAN ADD PRODUCT ONLY WITH VALID JWT

    [HttpPost]
    //[Authorize(Roles = "admin")]
    public ActionResult AddService(CreateServiceDto dto)
    {
        if (User.Claims.Any(c => c.Type == "isHappy" && c.Value == "true"))
        {
            Console.WriteLine("Welcome Happy Person");
        }

        if (ModelState.IsValid)
        {
            var category = categoryRepository.GetById(dto.CategoryId);
            if (category is null)
            {
                return BadRequest(new { message = "Invalid Category" });
            }


            var service = dto.ToService();
            repository.Add(service);
            service.Category = category;
            return CreatedAtAction(nameof(GetServicesById), new { id = service.Id }, service.ToDto());
        }

        return BadRequest(ModelState);
    }   
}
