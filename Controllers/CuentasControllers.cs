using System.Net;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiWeb.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using ApiWeb.Servicios;

namespace ApiWeb.Controllers 
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasControllers : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtection;

        public CuentasControllers(UserManager<IdentityUser> userManager,
            IConfiguration configuration, 
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService
        )
        {
            this.userManager    = userManager;
            this.configuration  = configuration;
            this.signInManager  = signInManager;
            this.hashService = hashService;
            dataProtection = dataProtectionProvider.CreateProtector("valor_unico");
        }

       /*  [HttpGet("hash/{textoplano}")]
        public ActionResult RealizarHash(string textoPlano)
        {
            var resultado1 = hashService.Hash(textoPlano);
            var resultado2 = hashService.Hash(textoPlano);

            return Ok(new 
            {
                textoPlano,
                resultado1,
                resultado2
            });
        }


        [HttpGet("enciptar")]
        public ActionResult Encriptar()
        {
            var textoPlano = "Emma";
            var textoCifrado = dataProtection.Protect(textoPlano);
            var textoDesencriptado = dataProtection.Unprotect(textoCifrado);

            return Ok(new 
            {
                textoPlano,
                textoCifrado,
                textoDesencriptado
            });
        }
 */
       
        [HttpPost("registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser(credencialesUsuario.Email); 
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if(resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
                var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email
                , credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

                if(resultado.Succeeded)
                {
                    return await  ConstruirToken(credencialesUsuario);
                }
                else
                {
                    return BadRequest("Login Incorrecto");
                }
        }


        [HttpGet("RenovarToken",Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };

            return await ConstruirToken(credencialesUsuario);
        }


        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email)
    
            };

            var usuario = await userManager.FindByNameAsync(credencialesUsuario.Email);
            var usuarioDb = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(usuarioDb);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keyjwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null
            , claims:claims, expires:expiracion,signingCredentials:creds );

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };

        }


        [HttpPost("HacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByNameAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }


        [HttpPost("RemoverAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByNameAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }
    }
}