/*
==========================================
📘 HTTP STATUS CODES REFERENCE
==========================================

✅ 1xx — Informational
------------------------------------------
100 Continue               → Request received; client should continue.
101 Switching Protocols    → Server switching to a new protocol.
102 Processing             → Request accepted but still processing.
103 Early Hints            → Used to preload resources.

✅ 2xx — Success
------------------------------------------
200 OK                     → Request succeeded.
201 Created                → Resource successfully created.
202 Accepted               → Request accepted for processing (async).
203 Non-Authoritative Info → Response from a third-party source.
204 No Content              → Success, but no content to return.
205 Reset Content           → Ask client to reset form/view.
206 Partial Content         → Partial resource delivered (used in ranges).

⚠️ 3xx — Redirection
------------------------------------------
300 Multiple Choices        → Multiple resource options.
301 Moved Permanently       → Resource moved permanently.
302 Found (Moved Temporarily) → Temporary redirection.
303 See Other               → Resource can be found under another URI.
304 Not Modified            → Cached version is still valid.
307 Temporary Redirect      → Resource temporarily at a new URI.
308 Permanent Redirect      → Resource permanently at a new URI.

🚫 4xx — Client Errors
------------------------------------------
400 Bad Request             → Invalid syntax or request parameters.
401 Unauthorized            → Missing or invalid authentication.
402 Payment Required         → Reserved for future use.
403 Forbidden               → Authenticated but not allowed.
404 Not Found               → Requested resource not found.
405 Method Not Allowed       → HTTP method not supported.
406 Not Acceptable           → Resource cannot return acceptable format.
407 Proxy Authentication Req → Client must authenticate with a proxy.
408 Request Timeout          → Server timed out waiting for request.
409 Conflict                 → Conflict with current resource state.
410 Gone                     → Resource no longer available.
411 Length Required          → Missing Content-Length header.
412 Precondition Failed      → Preconditions in headers not met.
413 Payload Too Large        → Request body too large.
414 URI Too Long             → Request URI too long.
415 Unsupported Media Type   → Request media type not supported.
416 Range Not Satisfiable    → Invalid range in request.
417 Expectation Failed       → Expect header unmet.
418 I'm a Teapot ☕          → (RFC joke, but valid code)
422 Unprocessable Entity     → Validation or semantic error.
426 Upgrade Required         → Client should switch protocol.
429 Too Many Requests        → Rate limit exceeded.

💥 5xx — Server Errors
------------------------------------------
500 Internal Server Error    → Generic server error.
501 Not Implemented          → Server doesn’t support the request.
502 Bad Gateway              → Invalid response from upstream server.
503 Service Unavailable       → Server temporarily overloaded or down.
504 Gateway Timeout           → Upstream server failed to respond.
505 HTTP Version Not Supported→ Server doesn’t support HTTP version.
507 Insufficient Storage      → Server cannot store the representation.
508 Loop Detected             → Infinite loop detected during request.

==========================================
📚 Common Custom Usage in APIs
------------------------------------------
200 → Success (GET, PUT, DELETE)
201 → Created (POST)
204 → No Content (DELETE success)
400 → Bad Request (Validation errors)
401 → Unauthorized (Missing token)
403 → Forbidden (Access denied)
404 → Not Found (Invalid resource)
500 → Internal Server Error
==========================================
*/
