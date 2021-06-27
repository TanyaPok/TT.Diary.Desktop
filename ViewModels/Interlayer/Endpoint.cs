using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Interlayer
{
    internal static class Endpoint
    {
        internal static async Task<T> GetAsync<T>(string requestUri, string exceptionMessageFormat,
            params object[] exceptionMessageArgs) where T : class
        {
            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsAsync<T>();
                var args = exceptionMessageArgs.ToList();
                args.Add(response.StatusCode);
                throw new Exception(string.Format(ErrorMessages.GetSchedule.GetDescription(), args));
            }
        }

        internal static async Task UpdateAsync(Request request)
        {
            using (var response = await Context.DiaryHttpClient.PutAsJsonAsync(request.OperationContract, request.Data))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(string.Format(ErrorMessages.Edit.GetDescription(), request.AdditionalInfo,
                    errorMessage));
            }
        }

        internal static async Task<int> CreateAsync(Request request)
        {
            using (var response =
                await Context.DiaryHttpClient.PostAsJsonAsync(request.OperationContract, request.Data))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<int>();
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(string.Format(ErrorMessages.Add.GetDescription(), request.AdditionalInfo,
                    errorMessage));
            }
        }

        internal static async Task RemoveAsync(Request request)
        {
            using (var response = await Context.DiaryHttpClient.DeleteAsync(request.OperationContract))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(string.Format(ErrorMessages.Remove.GetDescription(), request.AdditionalInfo,
                    errorMessage));
            }
        }
    }
}