using Iris.Messaging;
using Iris.Samples.ApiController.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Iris.Samples.ApiController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageDispatcher _dispatcher;

        public MessageController(IMessageDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost]
        public ActionResult Post([FromBody]HelloMessage message)
        {
            _dispatcher.Dispatch(message);

            return Ok();
        }
    }
}