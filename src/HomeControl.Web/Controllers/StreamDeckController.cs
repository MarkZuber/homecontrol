using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HomeControl.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/streamdeck")]
    [ApiController]
    public class StreamDeckController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly Random s_random = new Random();

        public StreamDeckController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("{keyIndex}", Name = "GetImageForKey")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult GetImageForKey(int keyIndex)
        {
            var filePaths = new List<string>
            {
                Path.Combine(_webHostEnvironment.WebRootPath, "images", "keys", "smiles.png"),
                Path.Combine(_webHostEnvironment.WebRootPath, "images", "keys", "hellothere.png"),
            };

            var chance = s_random.NextDouble();
            string filePath = chance < 0.5 ? filePaths[0] : filePaths[1];

            return PhysicalFile(filePath, "image/png");
        }
    }
}
