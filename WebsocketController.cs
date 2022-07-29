using Microsoft.AspNetCore.Mvc;

namespace CHATFREE.Controllers
{
    [AllowAnonymous]
    public class WebsocketController : Controller
    {
       public async Task Connect() {
       
            if(HttpContext.WebSockets.IsWebSocketRequest)
            {
                var Websockets = await HttpContext.WebSockets.AcceptWebSocketAsync();
            }
       }         
    }
}
