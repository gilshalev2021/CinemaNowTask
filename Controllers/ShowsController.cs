using CinemaNowApi.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Common;
using System;

namespace CineamNowApi.Controllers
{
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        /// <summary>
        /// Fetches a show by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Show), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // AWS lambda invocation error codes
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Produces("application/json")]
        public IActionResult GetShowsByDate(string id)
        {
            Console.WriteLine("INSIDE GetShowsByDate, GOT date = " + id);

            var list = UtilityFunctions.GetShowsByDate(id);

            return new OkObjectResult(list.ToArray());
        }

        /// <summary>
        /// Lists shows
        /// </summary>
        [HttpGet]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Show[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // AWS lambda invocation error codes
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Produces("application/json")]
        public IActionResult Get()
        {
            var list = UtilityFunctions.GetShows();

            return new OkObjectResult(list.ToArray());
        }

        /// <summary>
        /// Creates a new show
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        // AWS lambda invocation error codes
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Post([FromBody] Show value)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(UtilityFunctions.GetErrorListFromModelState(ModelState));
            }
            return UtilityFunctions.AddShow(value);
        }

        /// <summary>
        /// Deletes a show
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
            return UtilityFunctions.DeleteShow(id);
        }

        /// <summary>
        /// Updates a show
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        // AWS lambda invocation error codes
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Put([FromBody] Show value)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(UtilityFunctions.GetErrorListFromModelState(ModelState));
            }
            return UtilityFunctions.UpdateShow(value);
        }

    }
}

