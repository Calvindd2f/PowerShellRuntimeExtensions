using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace PowerShellRuntimeExtensions
{
    public class WrappedHttpWebRequest
    {
        public static IWebProxy DefaultWebProxy
        {
            get { return WebRequest.DefaultWebProxy; }
        }

        public RequestCachePolicy CachePolicy
        {
            get { return this.request.CachePolicy; }
        }

        public TokenImpersonationLevel ImpersonationLevel
        {
            get { return this.request.ImpersonationLevel; }
        }

        public AuthenticationLevel AuthenticationLevel
        {
            get { return this.request.AuthenticationLevel; }
        }

        public static int DefaultMaximumErrorResponseLength
        {
            get { return HttpWebRequest.DefaultMaximumErrorResponseLength; }
            set { HttpWebRequest.DefaultMaximumErrorResponseLength = value; }
        }

        public static int DefaultMaximumResponseHeadersLength
        {
            get { return HttpWebRequest.DefaultMaximumResponseHeadersLength; }
            set { HttpWebRequest.DefaultMaximumResponseHeadersLength = value; }
        }

        public static RequestCachePolicy DefaultCachePolicy
        {
            get { return HttpWebRequest.DefaultCachePolicy; }
            set { HttpWebRequest.DefaultCachePolicy = value; }
        }

        public ServicePoint ServicePoint
        {
            get { return this.request.ServicePoint; }
        }

        public HttpContinueDelegate ContinueDelegate
        {
            get { return this.request.ContinueDelegate; }
            set { this.request.ContinueDelegate = value; }
        }

        public Uri Address
        {
            get { return this.request.Address; }
        }

        public int ReadWriteTimeout
        {
            get { return this.request.ReadWriteTimeout; }
            set { this.request.ReadWriteTimeout = value; }
        }

        public int Timeout
        {
            get { return this.request.Timeout; }
            set { this.request.Timeout = value; }
        }

        public long ContentLength
        {
            get { return this.request.ContentLength; }
            set { this.request.ContentLength = value; }
        }

        public Uri RequestUri
        {
            get { return this.request.RequestUri; }
        }

        public int MaximumAutomaticRedirections
        {
            get { return this.request.MaximumAutomaticRedirections; }
            set { this.request.MaximumAutomaticRedirections = value; }
        }

        public string Method
        {
            get { return this.request.Method; }
            set { this.request.Method = value; }
        }

        public bool UseDefaultCredentials
        {
            get { return this.request.UseDefaultCredentials; }
            set { this.request.UseDefaultCredentials = value; }
        }

        public virtual CookieContainer CookieContainer
        {
            get { return this.request.CookieContainer; }
            set { this.request.CookieContainer = value; }
        }

        public string ConnectionGroupName
        {
            get { return this.request.ConnectionGroupName; }
            set { this.request.ConnectionGroupName = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return this.request.Headers; }
            set { this.request.Headers = value; }
        }

        public IWebProxy Proxy
        {
            get { return this.request.Proxy; }
            set { this.request.Proxy = value; }
        }

        public Version ProtocolVersion
        {
            get { return this.request.ProtocolVersion; }
            set { this.request.ProtocolVersion = value; }
        }

        public string ContentType
        {
            get { return this.request.ContentType; }
            set { this.request.ContentType = value; }
        }

        public string MediaType
        {
            get { return this.request.MediaType; }
            set { this.request.MediaType = value; }
        }

        public string TransferEncoding
        {
            get { return this.request.TransferEncoding; }
            set { this.request.TransferEncoding = value; }
        }

        public string Connection
        {
            get { return this.request.Connection; }
            set { this.request.Connection = value; }
        }

        public string Accept
        {
            get { return this.request.Accept; }
            set { this.request.Accept = value; }
        }

        public string Referer
        {
            get { return this.request.Referer; }
            set { this.request.Referer = value; }
        }

        public string UserAgent
        {
            get { return this.request.UserAgent; }
            set { this.request.UserAgent = value; }
        }

        public string Expect
        {
            get { return this.request.Expect; }
            set { this.request.Expect = value; }
        }

        public DateTime IfModifiedSince
        {
            get { return this.request.IfModifiedSince; }
            set { this.request.IfModifiedSince = value; }
        }

        public ICredentials Credentials
        {
            get { return this.request.Credentials; }
            set { this.request.Credentials = value; }
        }

        public X509CertificateCollection ClientCertificates
        {
            get { return this.request.ClientCertificates; }
            set { this.request.ClientCertificates = value; }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get { return this.request.AutomaticDecompression; }
            set { this.request.AutomaticDecompression = value; }
        }

        public bool SendChunked
        {
            get { return this.request.SendChunked; }
            set { this.request.SendChunked = value; }
        }

        public bool UnsafeAuthenticatedConnectionSharing
        {
            get { return this.request.UnsafeAuthenticatedConnectionSharing; }
            set { this.request.UnsafeAuthenticatedConnectionSharing = value; }
        }

        public bool PreAuthenticate
        {
            get { return this.request.PreAuthenticate; }
            set { this.request.PreAuthenticate = value; }
        }

        public bool Pipelined
        {
            get { return this.request.Pipelined; }
            set { this.request.Pipelined = value; }
        }

        public bool KeepAlive
        {
            get { return this.request.KeepAlive; }
            set { this.request.KeepAlive = value; }
        }

        public bool HaveResponse
        {
            get { return this.request.HaveResponse; }
        }

        public bool AllowWriteStreamBuffering
        {
            get { return this.request.AllowWriteStreamBuffering; }
            set { this.request.AllowWriteStreamBuffering = value; }
        }

        public bool AllowAutoRedirect
        {
            get { return this.request.AllowAutoRedirect; }
            set { this.request.AllowAutoRedirect = value; }
        }

        public int MaximumResponseHeadersLength
        {
            get { return this.request.MaximumResponseHeadersLength; }
            set { this.request.MaximumResponseHeadersLength = value; }
        }

        public void Abort()
        {
            this.request.Abort();
        }

        public void AddRange(int from, int to)
        {
            this.request.AddRange(from, to);
        }

        public void AddRange(int range)
        {
            this.request.AddRange(range);
        }

        public void AddRange(string rangeSpecifier, int from, int to)
        {
            this.request.AddRange(rangeSpecifier, from, to);
        }

        public void AddRange(string rangeSpecifier, int range)
        {
            this.request.AddRange(rangeSpecifier, range);
        }

        public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return this.request.BeginGetRequestStream(callback, state);
        }

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return this.request.BeginGetResponse(callback, state);
        }

        public Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context)
        {
            return this.request.EndGetRequestStream(asyncResult, out context);
        }

        public Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return this.request.EndGetRequestStream(asyncResult);
        }

        public WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return this.request.EndGetResponse(asyncResult);
        }

        public Stream GetRequestStream(out TransportContext context)
        {
            return this.request.GetRequestStream(out context);
        }

        public Stream GetRequestStream()
        {
            return this.request.GetRequestStream();
        }

        public WebResponse GetResponse()
        {
            WebResponse response;
            try
            {
                response = this.request.GetResponse();
            }
            catch (WebException ex)
            {
                string text = "Web Exception Summary: " + ex.Message;
                try
                {
                    using (Stream responseStream = ex.Response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            text = text + ", Web Exception Details: " + streamReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception)
                {
                }

                throw new Exception(text, ex);
            }

            return response;
        }

        public static WrappedHttpWebRequest Create(Uri requestUri)
        {
            return new WrappedHttpWebRequest
            {
                request = (HttpWebRequest)WebRequest.Create(requestUri)
            };
        }

        public static WrappedHttpWebRequest Create(string requestUriString)
        {
            return new WrappedHttpWebRequest
            {
                request = (HttpWebRequest)WebRequest.Create(requestUriString)
            };
        }

        public static WrappedHttpWebRequest CreateDefault(Uri requestUri)
        {
            return new WrappedHttpWebRequest
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(requestUri)
            };
        }

        public static IWebProxy GetSystemWebProxy()
        {
            return WebRequest.GetSystemWebProxy();
        }

        public static bool RegisterPrefix(string prefix, IWebRequestCreate creator)
        {
            return WebRequest.RegisterPrefix(prefix, creator);
        }

        private HttpWebRequest request;
    }
}