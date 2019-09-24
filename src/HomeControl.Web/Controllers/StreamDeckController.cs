using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HomeControl.Web.Controllers
{
    [Route("api/streamdeck")]
    [ApiController]
    public class StreamDeckController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStreamDeckActivityService _activityService;

        public StreamDeckController(IWebHostEnvironment webHostEnvironment, IStreamDeckActivityService activityService)
        {
            _webHostEnvironment = webHostEnvironment;
            _activityService = activityService;
        }

        [HttpGet("{keyIndex}", Name = "GetImageForKey")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [ProducesResponseType(200, Type = typeof(FileResult))]
        public IActionResult GetImageForKey(int keyIndex)
        {
            const int NumKeys = 15;

            bool isNormalImage = keyIndex < NumKeys;

            int activityIdx = isNormalImage ? keyIndex : keyIndex - NumKeys;
            var keyInfo = _activityService.GetKeyInfoAtIndex(activityIdx);
            string fileName = isNormalImage ? keyInfo.NormalImageFileName : keyInfo.KeyPressedImageFileName;
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "keys", fileName);
            return PhysicalFile(filePath, "image/png");
        }

        [HttpPost("{keyIndex}", Name = "PressKey")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult PressKey(int keyIndex)
        {
            // todo: make PressKey an async method.
            _activityService
                .ExecuteActivityAtIndexAsync(keyIndex, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return Ok();
        }
    }
}
