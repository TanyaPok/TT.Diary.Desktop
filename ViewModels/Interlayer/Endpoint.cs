using System;
using System.Net.Http;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Interlayer
{
    internal static class Endpoint
    {
        internal static async Task UpdateEntity(Request request)
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

        internal static async Task<int> CreateEntity(Request request)
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

        internal static async Task RemoveEntity(Request request)
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