using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using EcoAppApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using DAL.Models;

namespace EcoAppApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class  ProductsController(
    ProductsRepository repository) 
   : ControllerBase
{
    [HttpGet]
    public ActionResult GetProducts()
    {
        return Ok(repository.GetAll().Select(s => s.ToDto()));
    }

    [HttpGet("{id}")]
    public ActionResult GetProductsById(int id)
    {

        var product = repository.GetById(id);

        if (product is null)
        {
            return NotFound();
        }
        return Ok(product.ToDto());
    }
    //CAN ADD PRODUCT ONLY WITH VALID JWT

    [HttpPost]
    [Authorize(Roles = "admin")]
    public ActionResult AddProduct(CreateProductDto dto)
    {
        if (User.Claims.Any(c => c.Type == "isHappy" && c.Value == "true"))
        {
            Console.WriteLine("Welcome Happy Person");
        }

        if (ModelState.IsValid)
        {
            var product = dto.ToProduct();
            repository.Add(product);
            return CreatedAtAction(nameof(GetProductsById), new { id = product.Id }, product.ToDto());
        }

        return BadRequest(ModelState);
    }   
}
