using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETCoreSeed.API.Controllers
{
    public class BaseController : Controller
    {
        //private readonly IUow _uow;
        protected readonly IAuthorizationService _authorizationService;
        protected readonly JsonSerializerSettings _serializerSettings;
        protected readonly IMapper _mapper;

        public BaseController(IMapper mapper, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _authorizationService = authorizationService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
        //public BaseController(IUow uow)
        //{
        //    _uow = uow;
        //}

        protected new IActionResult Response(object dataToReturn, ValidationResult validationResult)
        {
            if (validationResult.IsValid)
            {
                try
                {
                    //_uow.Commit();
                    return Ok(new
                    {
                        success = true,
                        data = dataToReturn
                    });
                }
                catch
                {
                    // Logar o erro (Elmah)
                    return BadRequest(new
                    {
                        success = false,
                        errors = new[] { "Ocorreu uma falha interna no servidor." }
                    });
                }
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    errors = validationResult.Errors
                });
            }
        }

        protected string BuildListOfErrors(ValidationResult result)
        {
            StringBuilder erros = new StringBuilder();

            foreach (var error in result.Errors)
            {
                erros.AppendLine(error.Message);
            }

            return erros.ToString();
        }
    }
}
