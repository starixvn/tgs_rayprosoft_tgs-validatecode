using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TGS.Services.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Apis.ValidateCodeApi.Controllers
{
    /// <summary>
    /// ValidateCode Service
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("validate-code")]
    [ApiController]
    public class ValidateCodeController : ControllerBase
    {
        /// <summary>
        /// Save data by validate code.
        /// </summary>
        /// <param name="saveValidateDataService">The save validate data service.</param>
        /// <param name="validateCode">The validate code.</param>
        /// <param name="token">The token.</param>
        /// <param name="content">The content.</param>
        /// <response code="201">Save data success.</response>
        /// <response code="409">The validate data already set.</response>
        [HttpPost("{validateCode}/{token}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<IActionResult> SetValidateCodeAsync(
            [FromServices] ICommandService<SaveValidateDataCommand> saveValidateDataService,
            string validateCode,
            string token,
            [FromBody] JObject content)
        {
            try
            {
                await saveValidateDataService
                    .ExecuteAsync(new SaveValidateDataCommand(validateCode, token, content.ToString()))
                    .ConfigureAwait(false);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (ValidateCodeConflictException)
            {
                return StatusCode(StatusCodes.Status409Conflict, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get data by validate code.
        /// </summary>
        /// <param name="getValidateDataService">The get validate data service.</param>
        /// <param name="validateCode">The validate code.</param>
        /// <param name="token">The token.</param>
        /// <response code="204">Can't find data.</response>
        [HttpGet("{validateCode}/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JObject))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<JObject> GetValidateCodeDataAsync(
            [FromServices] IGetService<GetValidateDataQuery, string> getValidateDataService,
            string validateCode,
            string token)
        {
            var content = await getValidateDataService
                .GetAsync(new GetValidateDataQuery(validateCode, token))
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(content))
                return null;
            else
                return JObject.Parse(content);
        }
    }
}
