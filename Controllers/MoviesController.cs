using CinemaNowApi.Common.Model;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineamNowApi.Controllers
{
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        /// <summary>
        /// Lists movies
        /// </summary>
        [HttpGet]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Movie[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // AWS lambda invocation error codes
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Produces("application/json")]
        public IActionResult Get()
        {
            var list = UtilityFunctions.GetMovies();
            
            return new OkObjectResult(list.ToArray());
        }

        /// <summary>
        /// Creates a new movie
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        // AWS lambda invocation error codes
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Post([FromBody] Movie value)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(UtilityFunctions.GetErrorListFromModelState(ModelState));
            }
            return UtilityFunctions.AddMovie(value);
        }

        /// <summary>
        /// Deletes a movie
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // AWS lambda invocation error codes
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Delete(string id)
        {
            return UtilityFunctions.DeleteMovie(id);
        }
    }
}
