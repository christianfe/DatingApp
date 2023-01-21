using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Extensions;
using Serilog;

namespace API.Middleware
{
	public class LogMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IHostEnvironment _env;
		private readonly ILogger<Exception> _logger;

		public LogMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<Exception> logger)
		{
			_next = next;
			_env = env;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				LogRequest(context);
				await _next(context);
				LogResponse(context);
			}
			catch (Exception ex)
			{
				var str = context.Request.Protocol + " (" + context.Connection.RemoteIpAddress + " " + context.User.GetUsername() + ") --> "
						+ context.Response.StatusCode + ": " + context.Request.Path + " " + context.Request.QueryString;
				Log.Error(str);
				Log.Error("Headers: " + JsonSerializer.Serialize(context.Request.Headers));
				Log.Error(ex, ex.Message);
				_logger.LogError(str);
				_logger.LogError("Headers: " + JsonSerializer.Serialize(context.Request.Headers));
				_logger.LogError(ex, ex.Message);
			}
		}


		private void LogRequest(HttpContext c)
		{
			var str = c.Request.Protocol
				+ " ("
				+ c.Connection.RemoteIpAddress
				+ ") --> "
				+ c.Request.Method
				+ " "
				+ c.Request.Path
				+ " "
				+ c.Request.QueryString;
			Log.Information(str);
			_logger.LogInformation(str);
		}
		private void LogResponse(HttpContext c)
		{
			var str = c.Request.Protocol
				+ " ("
				+ c.Connection.RemoteIpAddress
				+ " "
				+ c.User.GetUsername()
				+ ") --> "
				+ c.Response.StatusCode;

			Log.Information(str);
			_logger.LogInformation(str);
		}
	}
}