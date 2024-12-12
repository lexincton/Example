using System.Runtime.CompilerServices;
using Common;
using Grpc.Core;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GRPCGateway.Helpers
{
    internal static class IntegrationHelper
    {
        internal static async Task<T> ActionExecute<T>(ILogger logger, ServerCallContext context, Func<T> action, [CallerMemberName] string name = "")
            where T : new()
        {
            T result;

            try
            {
                logger.LogInformation($"START: {name}");
                Log.Information($"START: {name}");
                result = await Task.Run(action);
            }
            catch (AggregateException ex)
            {
                SetError(ex.GetJoinedError());
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
            finally
            {
                logger.LogInformation($"END: {name}");
                Log.Information($"END: {name}");
            }

            return result;

            void SetError(string msg)
            {
                logger.LogError($"ERROR: {name}; Message:{msg}");
                Log.Error($"ERROR: {name}; Message:{msg}");

                context.Status = new Status(StatusCode.Internal, msg);
                result = new T();
            }
        }

        internal static void ThrowIfNotSuccess(this IResponse response)
        {
            if (!response.IsSuccess)
                throw new Exception(response.ErrorMessage);
        }
    }
}
