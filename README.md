# **OpenBullet2 HTTP Version Plugin**  

### **An OpenBullet2 Plugin That Allows For HTTP Requests Using Protocol Versions 2.0 And 3.0**  
**Customizable, extensible, and designed to fully utilize RuriLib for OpenBullet.**

---

## **Overview**  
This plugin enables OpenBullet2 to perform advanced HTTP requests with support for modern HTTP protocol versions, specifically HTTP/2 (`2.0`) and HTTP/3 (`3.0`). It leverages RuriLib's core utilities to provide a seamless integration for bots and allows customization of HTTP requests, headers, cookies, and responses.  

Additionally, this plugin can be modified to support even more HTTP protocols and extended for advanced use cases like load testing, web scraping, or API testing.  

---

## **Features**  
- **Protocol Versioning:** Supports HTTP/2 and HTTP/3 out of the box, with fallbacks to HTTP/1.1.  
- **Full Customization:**  
   - HTTP Method: `GET`, `POST`, `PUT`, etc.  
   - Headers and Cookies: Add custom request headers or cookies.  
   - Body Content: Support for payloads with `application/x-www-form-urlencoded` as the default content type.  
- **Proxy Support:** Seamless integration with authenticated proxies (host, port, username, password).  
- **Timeout and Redirect Handling:** Set custom timeouts and manage auto-redirect behavior.  
- **Detailed Logging:** Logs every aspect of the request and response, including headers, payload, cookies, and raw byte data.  
- **Extensible:** Can be easily modified to add more protocol versions or advanced RuriLib functionality.  

---

## **How It Works**  
The plugin uses **`HttpClient`** and **`HttpClientHandler`** from .NET to perform HTTP requests with specific protocol versions. It allows bots to execute advanced, configurable HTTP calls directly in OpenBullet2 workflows.  

### **Core Flow**  
1. The user configures the HTTP method, URL, headers, cookies, and other request options.  
2. The plugin builds the request:  
   - Sets the desired protocol version (2.0, 3.0, or fallback to 1.1).  
   - Attaches headers, cookies, and payload if provided.  
3. The request is sent, and the response is logged with:  
   - Status code  
   - Headers  
   - Payload (string and optional raw bytes)  
4. The plugin returns the response content for further processing.  

---

## **Use Cases**  
Here are some scenarios where this plugin shines:  

1. **API Testing with Modern Protocols:** Test REST APIs or HTTP/2-enabled services efficiently.  
2. **Web Scraping:** Fetch and parse content from modern websites using HTTP/2 and HTTP/3.  
3. **Load Testing:** Simulate concurrent requests with modern HTTP protocols for performance testing.  
4. **Advanced Bot Configurations:** Perform detailed, high-performance requests as part of OpenBullet2 workflows.  
5. **Reverse Engineering:** Analyze HTTP responses, headers, and raw payloads for research or debugging.  

---

## **Changes & Improvements**  

This plugin builds upon the existing functionality and includes the following enhancements:  

1. **Cleaned Code:**  
   - Removed redundant nullables (`?`) and unnecessary instantiations.  
   - Simplified `if` checks using null-coalescing operators (`??`).  
2. **Better Cookie Handling:**  
   - Consolidated cookies into a single `Cookie` header for proper HTTP compliance.  
3. **Improved Regions:**  
   - Logging is now grouped into clear "Request" and "Response" sections for better readability.  
4. **Optimized Logging:**  
   - Verbose logs are included for debugging every detail of the request and response:  
     - Request Method, URL, Headers, Cookies, Payload  
     - Response Status Code, Headers, Payload (including optional raw bytes).  
5. **Cleaner `HttpClient` Usage:**  
   - Proper disposal of `HttpClientHandler` and `HttpClient` to prevent memory leaks.  
6. **Extensibility:**  
   - Code structure allows for easy addition of new HTTP versions or further RuriLib functionalities.  

---

## **Installation**  
1. Download the compiled plugin `.dll` file.  
2. Add the plugin to your OpenBullet2 configuration directory.  
3. In OpenBullet2, create or modify a workflow to include the **`Http2Request`** block.  

---

## **Configuration Options**  

| Parameter       | Description                                                                 | Default                      | Required |
|-----------------|-----------------------------------------------------------------------------|------------------------------|----------|
| **`Url`**      | The target URL for the HTTP request.                                        | -                            | ✅       |
| **`Method`**   | The HTTP method (`GET`, `POST`, etc.).                                      | `GET`                        | ✅       |
| **`AutoRedirect`** | Whether to allow auto-redirects.                                           | `true`                       | ❌       |
| **`Body`**     | The payload/body content for the request.                                   | `""` (empty)                 | ❌       |
| **`Headers`**  | Dictionary of custom request headers.                                       | `null`                       | ❌       |
| **`Cookies`**  | Dictionary of cookies to include in the request.                            | `null`                       | ❌       |
| **`Timeout`**  | The request timeout in seconds.                                             | `100`                        | ❌       |
| **`OutputRaw`**| Whether to log the raw byte content of the response.                        | `false`                      | ❌       |
| **`ContentType`** | The content type for the request body.                                      | `application/x-www-form-urlencoded` | ❌       |
| **`Version`**  | The HTTP protocol version (`1.1`, `2.0`, `3.0`).                            | `2.0`                        | ❌       |

---

## **Example Usage**  

### **Minimal Example**  
```csharp
Http2Request(Data, "https://example.com", RuriLib.Functions.Http.HttpMethod.GET, true, "", null, null, 30, false);
```

### **Advanced Example with Headers and Payload**  
```csharp
var headers = new Dictionary<string, string> { { "Authorization", "Bearer YOUR_TOKEN" } };
var cookies = new Dictionary<string, string> { { "session", "abcdef123456" } };

Http2Request(
    Data,
    "https://example.com/api/data",
    RuriLib.Functions.Http.HttpMethod.POST,
    false,
    "key=value&anotherKey=anotherValue",
    headers,
    cookies,
    60,
    true
);
```

---

## **Contributing**  
Contributions and modifications are welcome! If you encounter bugs, have feature requests, or want to add more HTTP version support, feel free to submit a pull request.  

---

## **License**  
This project is licensed under the "GNU AFFERO GENERAL PUBLIC LICENSE".  

---

### **Author**  
**GitHub:** lxnnydev & ImmuneLion318

