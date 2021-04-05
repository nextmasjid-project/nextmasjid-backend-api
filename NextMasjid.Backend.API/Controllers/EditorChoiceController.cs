using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NextMasjid.Backend.Core.Data;
using NextMasjid.Backend.API.Models;

namespace NextMasjid.Backend.API.Controllers
{
    [ApiController]
    [Route("editorChoice")]
    public class EditorChoiceController : BaseController<EditorChoiceController>
    {
       
        [HttpGet("{lang}")]
        public IEnumerable<EditorChoiceModel> Get(string lang = "ar")
        {
            return Choices.Select(c => new EditorChoiceModel() { Lat = c.Location.Lat, Lng = c.Location.Lng, Notes = c.Notes });
        }
    }
}
