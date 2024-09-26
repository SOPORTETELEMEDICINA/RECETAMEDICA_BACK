using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Catalogos;
using RMD.Models.Catalogos;
using RMD.Models.Responses;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RMD.Controllers.Catalogos
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogosController(ICatalogoService catalogoService) : ControllerBase
    {
        private readonly ICatalogoService _catalogoService = catalogoService;

        [HttpGet("GetByIdCP/{idCP}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetByIdCP(int idCP)
        {
            if (idCP <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdCP proporcionado no es válido."));
            }

            var result = await _catalogoService.GetCatalogoByIdCP(idCP);
            if (result == null || !result.Any())
            {
                return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(new List<Catalogo>(), "No se encontraron resultados."));
            }

            return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(result));
        }

        [HttpGet("GetByIdMunicipio/{idMunicipio}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetByIdMunicipio(int idMunicipio)
        {
            if (idMunicipio <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdMunicipio proporcionado no es válido."));
            }

            var result = await _catalogoService.GetCatalogoByIdMunicipio(idMunicipio);
            if (result == null || !result.Any())
            {
                return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(new List<Catalogo>(), "No se encontraron resultados."));
            }

            return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(result));
        }

        [HttpGet("GetByIdAsentamiento/{idAsentamiento}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetByIdAsentamiento(int idAsentamiento)
        {
            if (idAsentamiento <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdAsentamiento proporcionado no es válido."));
            }

            var result = await _catalogoService.GetCatalogoByIdAsentamiento(idAsentamiento);

            // Si no hay resultados (es null o lista vacía), devolvemos una lista vacía con un mensaje.
            if (result == null || !result.Any())
            {
                return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(new List<Catalogo>(), "No se encontraron resultados."));
            }

            // Si hay resultados, devolvemos los datos.
            return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(result));
        }


        [HttpGet("GetByIdCiudad/{idCiudad}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetByIdCiudad(int idCiudad)
        {
            if (idCiudad <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdCiudad proporcionado no es válido."));
            }

            var result = await _catalogoService.GetCatalogoByIdCiudad(idCiudad);
            if (result == null || !result.Any())
            {
                return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(new List<Catalogo>(), "No se encontraron resultados."));
            }

            return Ok(ResponseFromService<IEnumerable<Catalogo>>.Success(result));
        }

        [HttpGet("GetDetailsByIdCP/{idCP}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetDetailsByIdCP(int idCP)
        {
            if (idCP <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdCP proporcionado no es válido."));
            }

            var result = await _catalogoService.GetDetailsByIdCP(idCP);
            return Ok(ResponseFromService<CatalogoDetail>.Success(result));
        }

        [HttpGet("GetDetailsByIdAsentamiento/{idAsentamiento}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetDetailsByIdAsentamiento(int idAsentamiento)
        {
            if (idAsentamiento <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdAsentamiento proporcionado no es válido."));
            }

            var result = await _catalogoService.GetDetailsByIdAsentamiento(idAsentamiento);
            return Ok(ResponseFromService<CatalogoDetail>.Success(result));
        }

        [HttpGet("GetDetailsByIdMunicipio/{idMunicipio}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetDetailsByIdMunicipio(int idMunicipio)
        {
            if (idMunicipio <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdMunicipio proporcionado no es válido."));
            }

            var result = await _catalogoService.GetDetailsByIdMunicipio(idMunicipio);
            return Ok(ResponseFromService<CatalogoDetail>.Success(result));
        }

        [HttpGet("GetDetailsByIdCiudad/{idCiudad}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetDetailsByIdCiudad(int idCiudad)
        {
            if (idCiudad <= 0)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdCiudad proporcionado no es válido."));
            }

            var result = await _catalogoService.GetDetailsByIdCiudad(idCiudad);
            return Ok(ResponseFromService<CatalogoDetail>.Success(result));
        }

        [HttpPost("CreateOrUpdateCatalogo")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> CreateOrUpdateCatalogo([FromBody] Catalogo catalogo)
        {
            if (catalogo == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El modelo Catalogo proporcionado no es válido."));
            }

            var result = await _catalogoService.CreateOrUpdateCatalogo(catalogo);
            return Ok(ResponseFromService<string>.Success(result));
        }
    }
}
