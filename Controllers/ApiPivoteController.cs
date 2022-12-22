using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PruebaBackend.Models;
using RestSharp;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using PruebaBackend.Helpers;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace PruebaBackend.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiPivoteController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly string _user;
        private readonly string _pass;
        private readonly string _url;
        
        public ApiPivoteController(IConfiguration configuration)
        {
            _configuration = configuration;
            _user = _configuration.GetValue<string>("CredencialesApi:user");
            _pass = _configuration.GetValue<string>("CredencialesApi:password");
            _url = _configuration.GetValue<string>("url");
        }


       
        [HttpGet]
        [Route("all")]
        public  IActionResult All()
        {
            ConvertBase64 obj = new ConvertBase64();
            EnPointRest endpoint = new EnPointRest();
            ErrorResponse error = new ErrorResponse();
            List<Image> image = new List<Image>();
            List<Image> imageVerify = new List<Image>();
            dynamic data;
            try
            {
               image = endpoint.GetAllImage(_url, _user, _pass);
        
            }catch(Exception ex)
            {
                error.value = 0;
                error.response = 0;
                error.message = "Error al consultar el api";
                error.descripcion = ex.Message;
                return BadRequest(error);

            }

            try
            {
                foreach (var item in image)
                {
                    if (obj.validarBase64(item.base64))
                    {
                        imageVerify.Add(item);
                    }
                }
            }catch(Exception ex)
            {
                error.value = 0;
                error.response = 0;
                error.message = "Error al Validar las imagenes en base64";
                error.descripcion = ex.Message;

                return BadRequest(error);
            }
      
            return Ok(imageVerify);

        }

        [HttpPost]
        [Route("saveImage")]
        public IActionResult SaveImage(IFormFile request)
        {
            ConvertBase64 cb = new ConvertBase64();
            EnPointRest endpoint = new EnPointRest();
            ErrorResponse error = new ErrorResponse();
            Image resp = new Image();
            dynamic data;
            dynamic bodyRequest;
            byte[] resultBase64;
            string extension;
            string fileName;


            var file = request;

            if (request == null)
            {
                error.value = 0;
                error.response = 0;
                error.message = "Archivo no enviado";
                error.descripcion = "No se encontro un archivo para procesar";
               

                return BadRequest(error);
            }

            fileName = request.FileName;
            extension = Path.GetExtension(fileName);

            if(
                !extension.ToLower().Equals(".png") && 
                !extension.ToLower().Equals(".jpg") && 
                !extension.ToLower().Equals(".gif") && 
                !extension.ToLower().Equals(".jpeg"))
            {
                error.value = 0;
                error.response = 0;
                error.message = "Formato de Archivo no Soportado";
                error.descripcion = "Los formatos soportados son png, jpg, gif, jpeg";
                return BadRequest(error);
            }

            try
            {
               
                byte[] res = new byte[file.Length];
                var resultInBytes = cb.ConvertToBytes(file);
                Array.Copy(resultInBytes,res,resultInBytes.Length);
                resultBase64 = res; 
            }
            catch(Exception ex)
            {
                error.value = 0;
                error.response = 0;
                error.message = "Error al Convertir el archivo a base64";
                error.descripcion = ex.Message;

            
                return BadRequest(error);
            }

            try
            {
                bodyRequest = new JObject();
                bodyRequest.nombre = fileName;
                bodyRequest.base64 = resultBase64;
                resp = endpoint.SaveImage(bodyRequest,_url,_user,_pass);

            }
            catch(Exception ex)
            {
                error.value = 0;
                error.response = 0;
                error.message = "Error al guardar la imagen";
                error.descripcion = ex.Message;

                return BadRequest(error);
            }
           
            return Ok(resp);
        }

        [HttpGet]
        [Route("oneImage")]
        public IActionResult OneImage(int id)
        {
            EnPointRest endpoint = new EnPointRest();
            ErrorResponse error = new ErrorResponse();
            Image image = new Image(); 
            dynamic data;

            try
            {
               image = endpoint.GetOneImage(id, _url, _user, _pass);

            }catch(Exception ex) {
                error.value = 0;
                error.response = 0;
                error.message = "Error al Obtener el registro de la imagen";
                error.descripcion = ex.Message;

                return BadRequest(error);
            }

            return Ok(image);

        }
        

        
    }
}
