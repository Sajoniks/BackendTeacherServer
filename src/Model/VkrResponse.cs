using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LearnBotServer.Model;

public static class VkrResponse
{
    public static JsonResult OK<T>(T data)
    {
        return new JsonResult(new
        {
            is_ok = true,
            response_code = HttpStatusCode.OK,
            data
        });
    }

    public static JsonResult Failed(HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new JsonResult(
            new
            {
                is_ok = false,
                response_code = statusCode,
                data = default(object)
            }
        );
    }
}