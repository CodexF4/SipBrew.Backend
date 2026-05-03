using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SipBrew.Core.Contracts;
using SipBrew.Core.DTO;
using SipBrew.Core.Models;

namespace SipBrew.WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsBL _bl;
        private readonly IValidator<AddProductDTO> _createValidator;
        private readonly IValidator<ProductsModel> _updateValidator;

        public ProductsController(
            IProductsBL bl,
            IValidator<AddProductDTO> createValidator,
            IValidator<ProductsModel> updateValidator)
        {
            _bl = bl;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageResponseDTO<ProductsModel>))]
        public async Task<IActionResult> Get(
            [FromQuery] ProductFilterDTO filter,
            [FromQuery] PageRequestDTO pageRequest)
        {
            filter ??= new ProductFilterDTO();
            pageRequest ??= new PageRequestDTO();

            var result = await _bl.GetProducts(filter, pageRequest);
            return Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductsModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] AddProductDTO dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var result = await _bl.CreateProduct(dto);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductsModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] ProductsModel model) 
        {
            var validation = await _updateValidator.ValidateAsync(model);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var result = await _bl.UpdateProduct(model);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _bl.DeleteProduct(id);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}