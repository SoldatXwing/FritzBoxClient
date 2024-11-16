using FritzBoxClient.Models.ErrorModels;
using FritzBoxClient.Models.NasModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using FritzBoxClient.Exceptions.NasExceptions;
using System.Text;
namespace FritzBoxClient
{
    /// <summary>
    /// Provides access to NAS storage on a FritzBox router, allowing for file retrieval, storage information access, and file uploads.
    /// </summary>
    public class FritzBoxNasAccesser : BaseAccesser
    {
        /// <summary>
        /// Initializes a new instance of the FritzBoxNasAccesser with specified FritzBox credentials.
        /// </summary>
        /// <param name="fritzBoxPassword">Password for FritzBox login.</param>
        /// <param name="fritzBoxUrl">URL of the FritzBox (default is https://fritz.box).</param>
        /// <param name="userName">Username for FritzBox login (optional).</param>
        public FritzBoxNasAccesser(string fritzBoxPassword, string fritzBoxUrl = "https://fritz.box", string userName = "") => (FritzBoxUrl, Password, FritzUserName) = (fritzBoxUrl, fritzBoxPassword, userName);
        /// <summary>
        /// Gets disk information of the NAS storage at the specified path.
        /// </summary>
        /// <param name="path">Path in the NAS directory (default is root "/").</param>
        /// <returns>A DiskInfo object containing storage information.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the path is invalid or disk information is unavailable.</exception>
        public async Task<DiskInfo> GetNasStorageDiskInfoAsync(string path = "/")
        {
            if (!path.StartsWith("/"))
                throw new InvalidOperationException(@"Path has to start with: ""/""");
            var response = await GetNasDirectoryInfoAsync(path);
            if (response?.DiskInfo is not null)
                return response.DiskInfo;

            throw new InvalidOperationException("Disk information is not available.");
        }
        /// <summary>
        /// Retrieves a list of directories (folders) in the specified NAS path.
        /// </summary>
        /// <param name="path">Path in the NAS directory (default is root "/").</param>
        /// <returns>A list of NasDirectory objects representing folders in the path.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the path is invalid or directories are unavailable.</exception>
        public async Task<List<NasDirectory>> GetNasFoldersAsync(string path = "/")
        {
            if (!path.StartsWith("/"))
                throw new InvalidOperationException(@"Path has to start with: ""/""");
            var response = await GetNasDirectoryInfoAsync(path);
            if (response?.Directories is not null)
                return response.Directories;

            throw new InvalidOperationException("Fodlers are not available.");
        }
        /// <summary>
        /// Gets the bytes of a file from the NAS at the specified path.
        /// </summary>
        /// <param name="path">Path to the file in the NAS directory.</param>
        /// <returns>A byte array containing the file data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the path is invalid or file retrieval fails.</exception>
        public async Task<List<NasFile>> GetNasFilesAsync(string path = "/")
        {
            if (!path.StartsWith("/"))
                throw new InvalidOperationException(@"Path has to start with: ""/""");
            var response = await GetNasDirectoryInfoAsync(path);
            if (response?.Files is not null)
                return response.Files;

            throw new InvalidOperationException("Files are not available");
        }
        /// <summary>
        /// Asynchronously retrieves information about a NAS directory from a Fritz!Box router.
        /// </summary>
        /// <param name="path">The path of the directory to retrieve information for. Must start with "/".</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the 
        /// <see cref="FirtzBoxNasResponse"/> object representing the directory information.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the provided path does not start with "/".
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the request to fetch NAS server information fails.
        /// </exception>
        private async Task<FirtzBoxNasResponse> GetNasDirectoryInfoAsync(string path = "/")
        {
            if (!path.StartsWith("/"))
                throw new InvalidOperationException(@"Path has to start with: ""/""");
            if (!IsSidValid)
                await GenerateSessionIdAsync();
            var content = new StringContent($"sid={CurrentSid}&path={path}&limit=10000&sorting=%2Bfilename&c=files&a=browse", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = HttpRequestFritzBox("/nas/api/data.lua", content, HttpRequestMethod.Post);
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<FirtzBoxNasResponse>(await response.Content.ReadAsStringAsync())!;
            throw new Exception("Failed to fetch nas server");
        }
        /// <summary>
        /// Gets the bytes of a file from the NAS at the specified path.
        /// </summary>
        /// <param name="path">Path to the file in the NAS directory.</param>
        /// <returns>A byte array containing the file data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the path is invalid or file retrieval fails.</exception>
        public async Task<byte[]> GetNasFileBytes(string path = "/")
        {
            if (!path.StartsWith("/"))
                throw new InvalidOperationException(@"Path has to start with: ""/""");
            if (!IsSidValid)
                await GenerateSessionIdAsync();
            var response = HttpRequestFritzBox($"/nas/api/data.lua?c=pictures&a=get&sid={CurrentSid}&path={path}", null, HttpRequestMethod.Get);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsByteArrayAsync();
            throw new InvalidOperationException("Failed to get file bytes");
        }
        /// <summary>
        /// Uploads a file to the NAS at the specified path.
        /// </summary>
        /// <param name="relativeUrl">Relative URL for the upload endpoint.</param>
        /// <param name="relativeNasUrl">Path in the NAS where the file will be uploaded.</param>
        /// <param name="modificationTime">Modification time of the file in Unix format.</param>
        /// <param name="fileBytes">Byte array of the file to upload.</param>
        /// <param name="fileName">Name of the file to upload.</param>
        /// <returns>A HttpResponseMessage indicating the result of the upload.</returns>
        /// <exception cref="Exception">Thrown if upload fails.</exception>
        public async Task<HttpResponseMessage> UploadFileAsync(string relativeUrl, string relativeNasUrl, long modificationTime, byte[] fileBytes, string fileName)
        {
            if (!IsSidValid)
                await GenerateSessionIdAsync();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");

            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(DetectMimeType(fileBytes));
            var form = new MultipartFormDataContent(boundary)
            {
                { new StringContent(CurrentSid), "sid" },
                { new StringContent(modificationTime.ToString()), "mtime" },
                { new StringContent(relativeNasUrl), "dir" },
                { new StringContent(""), "ResultScript" },
                { fileContent, "UploadFile", fileName}
            };
            var response = FormDataRequestFritzBox(relativeUrl, form, HttpRequestMethod.Post);
            return response;
        }
        /// <summary>
        /// Detects the MIME type of a file based on its byte content.
        /// </summary>
        /// <param name="fileBytes">Byte array of the file to inspect.</param>
        /// <returns>A string representing the MIME type.</returns>
        private static string DetectMimeType(byte[] fileBytes)
        {
            if (fileBytes.Length > 0)
            {
                if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46) // %PDF
                    return "application/pdf";
                else if (fileBytes.Length >= 4 && fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47) // PNG
                    return "image/png";
                else if (fileBytes.Length >= 2 && fileBytes[0] == 0xFF && fileBytes[1] == 0xD8) // JPEG
                    return "image/jpeg";
            }

            return "application/octet-stream";
        }
        /// <summary>
        /// Sends a multipart/form-data request to the FritzBox NAS.
        /// </summary>
        /// <param name="relativeUrl">Relative URL for the API endpoint.</param>
        /// <param name="formData">Form data content for the request.</param>
        /// <param name="method">HTTP request method (only POST is supported).</param>
        /// <returns>A HttpResponseMessage indicating the result of the request.</returns>
        /// <exception cref="NotImplementedException">Thrown if a method other than POST is specified.</exception>
        private static HttpResponseMessage FormDataRequestFritzBox(string relativeUrl, MultipartFormDataContent? formData, HttpRequestMethod method)
        {
            using var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true;

            using var httpClient = new HttpClient(handler) { BaseAddress = new Uri(FritzBoxUrl) };
            if (method is HttpRequestMethod.Post)
            {
                var response = httpClient.PostAsync(relativeUrl, formData)
                    .GetAwaiter()
                    .GetResult();
                return response;
            }
            throw new NotImplementedException("Only Post method is supported!");
        }
        public async Task DeleteFiles(List<NasFile> files)
        {
            if (!files.Select(c => c.Path.StartsWith("/")).Any())
                throw new InvalidOperationException(@"Path has to start with: ""/""");
            if (!IsSidValid)
                await GenerateSessionIdAsync();
            var parameters = new StringBuilder();
            parameters.Append($"sid={CurrentSid}&c=files&a=delete");

            int index = 0;
            foreach (var file in files)
            {
                parameters.Append($"&paths[{index}]={Uri.EscapeDataString(file.Path)}");
                index++;
            }

            var content = new StringContent(parameters.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = HttpRequestFritzBox($"/nas/api/data.lua", content, HttpRequestMethod.Post);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());
                if (error?.Error != null)
                {
                    throw new FritzBoxFileSystemException(
                        error.Error.Message,
                        error.Error.Code,
                        error.Error.Data?.Select(d => new FritzBoxFileErrorDetail(d.Path, d.Message, d.Code)).ToList()
                            ?? new List<FritzBoxFileErrorDetail>()
                    );
                }
            }   
        }

    }

}

