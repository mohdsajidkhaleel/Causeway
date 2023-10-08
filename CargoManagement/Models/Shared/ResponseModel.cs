using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CargoManagement.Models.Shared
{
    public class CMSResponse
    {
        public ActionResult<ResponseModel> Ok(object res)
        {
            return new OkObjectResult(new ResponseModel(true, res, "Success", HttpStatusCode.OK.ToString()));
        }
        public ActionResult<ResponseModel> Unauthorized(string message)
        {
            return new UnauthorizedObjectResult(new ResponseModel(false, null, message, HttpStatusCode.Unauthorized.ToString()));
        }
        public ActionResult<ResponseModel> Created(object res, string message = "Created")
        {
            return new OkObjectResult(new ResponseModel(true, res, message, HttpStatusCode.Created.ToString()));
        }
        public ActionResult<ResponseModel> Updated(object res, string message = "Updated")
        {
            return new OkObjectResult(new ResponseModel(true, res, message, HttpStatusCode.OK.ToString()));
        }
        public ActionResult<ResponseModel> UnprocessableEntity(object res, string message)
        {
            return new UnprocessableEntityObjectResult(new ResponseModel(false, res, message, HttpStatusCode.UnprocessableEntity.ToString()));
        }
        public ActionResult<ResponseModel> NotFound(object? res)
        {
            return new NotFoundObjectResult(new ResponseModel(false, res, "Not Found", HttpStatusCode.NotFound.ToString()));
        }
        public ActionResult<ResponseModel> BadRequest(string message)
        {
            return new BadRequestObjectResult(new ResponseModel(false, null, message, HttpStatusCode.BadRequest.ToString()));
        }
        public ActionResult<ResponseModel> UpdateFailed(string message)
        {
            return new BadRequestObjectResult(new ResponseModel(false, null, message, HttpStatusCode.OK.ToString()));
        }
        public ActionResult<ResponseModel> DeleteFailed(string message)
        {
            return new BadRequestObjectResult(new ResponseModel(false, null, message, HttpStatusCode.OK.ToString()));
        }


    }
    public class ResponseModel
    {
        public ResponseModel(bool isSuccess, object? data, string message, string statusCode)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }
        public bool IsSuccess { get; set; }
        public object? Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }




    }
}
