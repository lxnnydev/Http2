# **OpenBullet2 HTTP Version Plugin**  

### **An OpenBullet2 Plugin That Allows For HTTP Requests Using Protocol Versions 2.0 And 3.0**  
**Customizable, extensible, and designed to fully utilize RuriLib for OpenBullet.**

---

## **Overview**  
This plugin enables OpenBullet2 to perform advanced HTTP requests with support for modern HTTP protocol versions, specifically HTTP/2 (`2.0`) and HTTP/3 (`3.0`). It now includes advanced features like retry logic, proxy rotation, multi-threaded requests, and custom User-Agent handling.  

This plugin is highly customizable, making it suitable for a variety of use cases such as API testing, web scraping, and load testing.

---

## **Features**  
- **Protocol Versioning:** Supports HTTP/2 and HTTP/3, with fallbacks to HTTP/1.1.  
- **Retry Mechanism:** Automatically retries failed requests up to a user-defined limit.  
- **Custom User-Agent Handling:** Specify a custom `User-Agent` or use a default User-Agent string to mimic browser behavior.  
- **Advanced Proxy Support:** Rotate between multiple proxies for each request, with support for authenticated proxies.  
- **Multi-Threaded Requests:** Execute multiple HTTP requests concurrently, collecting responses in parallel.  
- **Full Customization:**  
   - HTTP Method: `GET`, `POST`, `PUT`, etc.  
   - Headers and Cookies: Add custom request headers or cookies.  
   - Body Content: Support for payloads with `application/x-www-form-urlencoded` as the default content type.  
- **Timeout and Redirect Handling:** Set custom timeouts and manage auto-redirect behavior.  
- **Verbose Logging:** Logs every aspect of the request and response for debugging and analysis.

---

## **How It Works**  
The plugin uses **`HttpClient`** and **`HttpClientHandler`** from .NET to perform HTTP requests with specific protocol versions. It allows bots to execute advanced, configurable HTTP calls directly in OpenBullet2 workflows.  

### **Core Workflow**
1. The user configures the HTTP method, URL, headers, cookies, proxies, and other options.  
2. The plugin builds and sends the request:
   - Sets the desired protocol version (2.0, 3.0, or fallback to 1.1).  
   - Attaches headers, cookies, and payload if provided.  
3. Retry logic ensures transient errors are retried up to the user-defined limit.  
4. The request can optionally use proxy rotation to select a new proxy from a list.  
5. Responses from requests are logged and returned, including raw byte data if specified.

---

## **Use Cases**  

1. **API Testing:** Test APIs with modern HTTP protocols and custom configurations.  
2. **Web Scraping:** Fetch data from modern websites using HTTP/2 and HTTP/3.  
3. **Load Testing:** Execute parallel requests to test server performance.  
4. **Bot Configurations:** Create highly customizable workflows for bots in OpenBullet2.  
5. **Proxy Testing:** Rotate proxies to bypass rate limits or geo-restrictions.

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
7. **Retry Mechanism:**  
   - Retries failed requests up to a configurable `retry_count`.  
   - Logs retry attempts and ensures robust request handling.  
8. **Custom User-Agent:**  
   - Allows users to specify their own `User-Agent`.  
   - Defaults to:  
     ```
     Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36
     ```
9. **Advanced Proxy Support:**  
   - Rotate between multiple proxies using a predefined list.  
   - Proxies are randomly selected for each request to avoid detection.
10. **Multi-Threaded Requests:**  
   - Execute multiple HTTP requests concurrently for faster performance.  
   - Suitable for scraping, load testing, and bulk API calls.

---

## **Installation**  
1. Download the compiled plugin `.dll` file.  
2. Add the plugin to your OpenBullet2 configuration directory.  
3. In OpenBullet2, create or modify a workflow to include the **`Http2Request`** block.  

---

## **Configuration Options**  

### **http2_request**  
| Parameter       | Description                                                                 | Default                      | Required |
|-----------------|-----------------------------------------------------------------------------|------------------------------|----------|
| **`url`**      | The target URL for the HTTP request.                                        | -                            | ✅       |
| **`method`**   | The HTTP method (`GET`, `POST`, etc.).                                      | `GET`                        | ✅       |
| **`auto_redirect`** | Whether to allow auto-redirects.                                           | `true`                       | ❌       |
| **`body`**     | The payload/body content for the request.                                   | `""` (empty)                 | ❌       |
| **`headers`**  | Dictionary of custom request headers.                                       | `null`                       | ❌       |
| **`cookies`**  | Dictionary of cookies to include in the request.                            | `null`                       | ❌       |
| **`proxy_list`** | List of proxies to use for request rotation.                                | `null`                       | ❌       |
| **`timeout`**  | The request timeout in seconds.                                             | `100`                        | ❌       |
| **`retry_count`** | Number of retry attempts for failed requests.                              | `3`                          | ❌       |
| **`output_raw`**| Whether to log the raw byte content of the response.                        | `false`                      | ❌       |
| **`user_agent`** | The User-Agent string to send with the request.                             | Default User-Agent           | ❌       |
| **`content_type`** | The content type for the request body.                                      | `application/x-www-form-urlencoded` | ❌       |
| **`version`**  | The HTTP protocol version (`1.1`, `2.0`, `3.0`).                            | `2.0`                        | ❌       |

### **multi_threaded_requests**  
| Parameter       | Description                                                                 | Default                      | Required |
|-----------------|-----------------------------------------------------------------------------|------------------------------|----------|
| **`urls`**     | A list of URLs for concurrent HTTP requests.                                | -                            | ✅       |
| Other options   | Same as `http2_request`.                                                   | -                            | -        |

---

## **Example Usage**

### **Single Request Example**  
```csharp
http2_request(
    data, 
    "https://example.com", 
    RuriLib.Functions.Http.HttpMethod.GET, 
    true, 
    "", 
    null, 
    null, 
    new List<string>(), 
    30, 
    3, 
    false, 
    null, 
    "application/x-www-form-urlencoded", 
    "2.0"
);
```

### **Concurrent Requests Example**  
```csharp
multi_threaded_requests(
    data,
    new List<string> { "https://example1.com", "https://example2.com" },
    RuriLib.Functions.Http.HttpMethod.GET,
    true,
    "",
    null,
    null,
    new List<string> { "http://proxy1:8080", "http://proxy2:8080" },
    30,
    3,
    false,
    null,
    "application/x-www-form-urlencoded",
    "2.0"
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

