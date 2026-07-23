using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace PowerManager.Server.Controllers
{
    using Entity;
    using PowerManager.Server.Entity.Auth;

    [Route("rest/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("whoami")]
        [AllowAnonymous]
        public Task<CertificateInfo> WhoamiAsync()
        {
            X509Certificate2? certificate = Request.HttpContext.Connection.ClientCertificate;
            CertificateInfo certificateInfo;

            if (certificate is null)
            {
                certificateInfo = new CertificateInfo
                {
                    CommonName = string.Empty,
                    Email = string.Empty,
                    Before = 0,
                    After = 0,
                    Thumbprint = string.Empty
                };
            } 
            else
            {
                certificateInfo = new CertificateInfo
                {
                    CommonName = certificate.GetNameInfo(X509NameType.SimpleName, false),
                    Email = certificate.GetNameInfo(X509NameType.EmailName, false),
                    Before = certificate.NotBefore.ToMilliseconds(),
                    After = certificate.NotAfter.ToMilliseconds(),
                    Thumbprint = certificate.Thumbprint
                };
            }

            ConfigurationValidator.Validate(certificateInfo);
            return Task.FromResult(certificateInfo);
        }

        [HttpPost("certificateUpdate")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public IActionResult CertificateUpdate([FromForm(Name = "file")] IFormFile file)
        {
            if (file.Length > (1024 * 128))
                return StatusCode(400, RestResult.Fail("TOO_BIG_CERTIFICATE", $"certificate file length: {file.Length}"));
            X509Certificate2 certificate;
            using (MemoryStream memory = new MemoryStream())
            {
                file.CopyTo(memory);
                certificate = X509CertificateLoader.LoadCertificate(memory.ToArray());
            }

            throw new NotImplementedException();
        }

        [HttpPost("certificateEscalation")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public IActionResult CertificateEscalation([FromForm(Name = "file")] IFormFile file)
        {
            if (file.Length > (1024 * 128))
                return StatusCode(400, RestResult.Fail("TOO_BIG_CERTIFICATE", $"certificate file length: {file.Length}"));
            X509Certificate2 certificate;
            using (MemoryStream memory = new MemoryStream())
            {
                file.CopyTo(memory);
                certificate = X509CertificateLoader.LoadCertificate(memory.ToArray());
            }

            throw new NotImplementedException();
        }
    }
}
