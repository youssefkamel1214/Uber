using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Serialization;
using Uber.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Uber.Models.Responses
{
    public class ApiResponse
    {
        public object?data { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HttpStatusCode statue { get; set; }
        public string Message { get; set; }
        public List<string> Error { get; set; }
        public  IActionResult HandleRespose() 
        {
            var dataToReturn =new Dictionary<string,object>();
            if(data!=null)
                dataToReturn.Add("data", data);
            if (Error!=null&&Error.Count != 0)
                dataToReturn.Add("Errors",Error);
            if (Message != null)
                dataToReturn.Add("Message", Message);

            return new ObjectResult(dataToReturn)
            {
                StatusCode = (int)statue
            };
        }
      
    }
}
