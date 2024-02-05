using Nancy.Json;
using System;
using System.IO;

public class HarEntry
{
    public HarRequest Request { get; set; }
    public HarResponse Response { get; set; }
}

public class HarRequest
{
    public string Url { get; set; }
    public HarQueryString[] QueryString { get; set; }
    public HarCookie[] Cookies { get; set; }
    public HarHeader[] Headers { get; set; }
    public string Method { get; set; }
    public HarPostData PostData { get; set; }
    public string IpAddress { get; set; }
}

public class HarResponse
{
    public int Status { get; set; }
    public HarHeader[] Headers { get; set; }
    public string Content { get; set; }
}

public class HarQueryString
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class HarCookie
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class HarHeader
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class HarPostData
{
    public HarPostParam[] Params { get; set; }
}

public class HarPostParam
{
    public string Name { get; set; }
    public string Value { get; set; }
}

class Program
{
    static void Main()
    {
        string folderPath = "D:\\Improtant\\NCKH\\HTTPToolKit";
        string outputFolderPath = "D:\\Improtant\\NCKH\\KeyValueToolkit";
        ProcessFilesInFolder(folderPath, outputFolderPath);
    }

    static void ProcessFilesInFolder(string folderPath, string outputFolderPath)
    {
        string[] harFiles = Directory.GetFiles(folderPath, "*.har");

        foreach (string harFilePath in harFiles)
        {
            Console.WriteLine($"Processing file: {harFilePath}");

            string outputFilePath = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(harFilePath) + "_output.txt");

            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                string jsonContent = File.ReadAllText(harFilePath);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic harData = serializer.Deserialize<dynamic>(jsonContent);

                SaveAllInfoToFile(harData, writer);
            }

            Console.WriteLine($"Results saved to {outputFilePath}");
        }

        Console.WriteLine($"All files processed.");
    }

    static void SaveAllInfoToFile(dynamic harData, StreamWriter writer)
    {
        // Biến đếm cho method GET và POST
        int getMethodCount = 0;
        int postMethodCount = 0;
        foreach (var entry in harData["log"]["entries"])
        {
            var request = entry["request"];
            var response = entry["response"];

            if (request != null)
            {
                // Lấy thông tin URL
                string url = request["url"];
                writer.WriteLine($"URL: {url}");

                // Lấy thông tin session từ cookies (nếu có)
                var cookies = request["cookies"];
                if (cookies != null)
                {
                    writer.WriteLine("Cookies:");
                    foreach (var cookie in cookies)
                    {
                        string cookieName = cookie["name"];
                        string cookieValue = cookie["value"];
                        writer.WriteLine($"{cookieName}: {cookieValue}");
                    }
                    writer.WriteLine();
                }

                // Lấy thông tin query string (nếu có)
                var queryString = request["queryString"];
                if (queryString != null)
                {
                    writer.WriteLine("Query String Parameters:");
                    foreach (var parameter in queryString)
                    {
                        string key = parameter["name"];
                        string value = parameter["value"];
                        writer.WriteLine($"{key}: {value}");
                    }
                    writer.WriteLine();
                }
                // Lấy thông tin HTTP Method
                string method = request["method"];
                writer.WriteLine($"HTTP Method: {method}");

                // Lấy thông tin Post Data (nếu có)
                // Lấy thông tin Post Data (nếu có)
                var postData = request.ContainsKey("postData") ? request["postData"] : null;
                if (postData != null && postData.ContainsKey("params"))
                {
                    writer.WriteLine("Post Data:");
                    foreach (var param in postData["params"])
                    {
                        string paramName = param["name"];
                        string paramValue = param["value"];
                        writer.WriteLine($"{paramName}: {paramValue}");
                    }
                    writer.WriteLine();
                }
                // Lấy thông tin IP Address
                string ipAddress = entry.ContainsKey("serverIPAddress") ? entry["serverIPAddress"] : "N/A";
                writer.WriteLine($"IP Address: {ipAddress}");

            }
            if (response != null)
            {
                // Lấy thông tin Response Status
                long status = response["status"]; // Thay đổi từ 'int' sang 'long'
                writer.WriteLine($"Response Status: {status}");

                // Lấy thông tin Response Headers (nếu có)
                var responseHeaders = response["headers"];
                if (responseHeaders != null)
                {
                    writer.WriteLine("Response Headers:");
                    foreach (var header in responseHeaders)
                    {
                        string headerName = header["name"];
                        string headerValue = header["value"];
                        writer.WriteLine($"{headerName}: {headerValue}");
                    }
                    writer.WriteLine();
                }
                // Lấy thông tin query string (nếu có)
                var queryString = request["queryString"];
                if (queryString != null)
                {
                    writer.WriteLine("Query String Parameters:");
                    foreach (var parameter in queryString)
                    {
                        string key = parameter["name"];
                        string value = parameter["value"];
                        writer.WriteLine($"{key}: {value}");

                        // Kiểm tra nếu là thông tin 'lat' và 'lon'
                        if (key.ToLower() == "lat")
                        {
                            // Lưu giá trị lat vào biến hoặc xử lý theo nhu cầu của bạn
                            string lat = value;
                            writer.WriteLine($"Latitude (lat): {lat}");
                        }
                        else if (key.ToLower() == "lon")
                        {
                            // Lưu giá trị lon vào biến hoặc xử lý theo nhu cầu của bạn
                            string lon = value;
                            writer.WriteLine($"Longitude (lon): {lon}");
                        }
                    }
                    writer.WriteLine();

                }
                string method = request["method"];
                writer.WriteLine($"HTTP Method: {method}");

                // Đếm số lượng method GET và POST
                if (method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    getMethodCount++;
                }
                else if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    postMethodCount++;
                }


            }
            writer.WriteLine(new string('-', 50));
        }
        writer.WriteLine($"Number of GET requests: {getMethodCount}");
        writer.WriteLine($"Number of POST requests: {postMethodCount}");
    }
}
